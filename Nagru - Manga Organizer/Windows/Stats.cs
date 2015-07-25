using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Nagru___Manga_Organizer
{
  /// <summary>
  /// Show how many times a tag is used
  /// </summary>
  public partial class Stats : Form
  {
    #region Properties

		DataTable dtManga = null;
    SortedDictionary<string, ushort> sdtTags = new SortedDictionary<string, ushort>();
    bool bPrevState = true;

    enum Panel
    {
      PieChart,
      List
    }

    #endregion

    #region Constructor
    public Stats()
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;

      dtManga = SQL.GetAllManga();
			ResizeLV();
    }

    private void Stats_Load(object sender, EventArgs e)
    {
      SwitchView(Panel.List);
    }

    private void Stats_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (dtManga != null) {
        dtManga.Clear();
        dtManga.Dispose();
      }

      this.DialogResult = DialogResult.OK;
    }

    #endregion

    #region Events

    private void ChkBx_FavsOnly0_CheckedChanged(object sender, EventArgs e)
    {
      ShowStats((pnlView0.ContainsFocus) ? Panel.PieChart : Panel.List);
    }

    private void BtnSwitch_Click(object sender, EventArgs e)
    {
      SwitchView((pnlView0.ContainsFocus) ? Panel.List : Panel.PieChart);
    }

    private void lvStats_Resize(object sender, EventArgs e)
    {
			ResizeLV();
    }

    private void lvStats_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
      e.NewWidth = lvStats.Columns[e.ColumnIndex].Width;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Toggle visible panel (pie chart & listview)
    /// </summary>
    /// <param name="iView">Controls which Panel to switch to</param>
    private void SwitchView(Panel PanelChoice)
    {
      this.SuspendLayout();
      Controls["pnlView" + (int)PanelChoice].BringToFront();

      //move checkbox to new panel
      ChkBx_FavsOnly.Parent = Controls["pnlView" + (int)PanelChoice];
      ChkBx_FavsOnly.BringToFront();

      //change button position & title depending on the panel shown
      BtnSwitch.Text = (PanelChoice == Panel.List) ? "Chart" : "List";
      BtnSwitch.Parent = Controls["pnlView" + (int)PanelChoice];
      BtnSwitch.BringToFront();

      //display stats
      ShowStats(PanelChoice);
      this.ResumeLayout();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="PanelChoice"></param>
    private void ShowStats(Panel PanelChoice)
    {
      bool bFavsOnly = ChkBx_FavsOnly.Checked;
      int iManga = dtManga.Rows.Count;

      //get stats of tags
      if (bFavsOnly != bPrevState) {
        iManga = 0;
        sdtTags.Clear();
        for (int i = 0; i < dtManga.Rows.Count; i++) {
          if (bFavsOnly && Decimal.Parse(dtManga.Rows[i]["Rating"].ToString()) < 5) {
            continue;
          }
          foreach (string svar in Ext.Split(dtManga.Rows[i]["Tags"].ToString(), ",")) {
            string sItem = svar.TrimStart();
            if (sdtTags.ContainsKey(sItem)){
              sdtTags[sItem]++;
            }
            else{
              sdtTags.Add(sItem, 1);
            }
          }
          iManga++;
        }
      }

      if (PanelChoice == Panel.PieChart) {
        //purge minority tags
        SortedDictionary<string, ushort> dtPie = new SortedDictionary<string, ushort>();
        dtPie.Add("_MO_Other_", 0);
        foreach (var kvpItem in sdtTags) {
          if ((kvpItem.Value * 1.0 / iManga) < 0.15) {
            dtPie["_MO_Other_"] += kvpItem.Value;
          }
          else {
            dtPie.Add(kvpItem.Key, kvpItem.Value);
          }
        }
        dtPie.Remove("_MO_Other_");

        //send stats to pie chart
        chtTags.Series[0].Points.Clear();
        chtTags.Series[0].Points.DataBindXY(dtPie.Keys, dtPie.Values);
        chtTags.DataManipulator.Sort(PointSortOrder.Descending, chtTags.Series[0]);
      }
      else {
        //send stats to listview
        lvStats.BeginUpdate();
        lvStats.Items.Clear();
        int inc = 0;
        ListViewItem[] alItems = new ListViewItem[sdtTags.Count];
        foreach (var kvpItem in sdtTags) {
          ListViewItem lvi = new ListViewItem(kvpItem.Key);
          lvi.SubItems.Add(kvpItem.Value.ToString());
          lvi.SubItems.Add(((float)kvpItem.Value / iManga).ToString("P2"));
          alItems[inc++] = lvi;
        }
        lvStats.Items.AddRange(alItems);
        lvStats.SortRows();
        lvStats.EndUpdate();
      }

      Text = string.Format("Stats: {0} tags in {1} manga", sdtTags.Count, iManga);
      bPrevState = bFavsOnly;
    }

		private void ResizeLV()
		{
      lvStats.BeginUpdate();
      int iColWidth = 0;
      for (int i = 0; i < lvStats.Columns.Count; i++) {
        iColWidth += lvStats.Columns[i].Width;
      }

      colTag.Width += lvStats.DisplayRectangle.Width - iColWidth;
      lvStats.EndUpdate();
		}

    #endregion
  }
}