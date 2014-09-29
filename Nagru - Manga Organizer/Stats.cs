using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  /* Show how many times a tag is used */
  public partial class Stats : Form
  {
		DataTable dtTags = null;
    SortedDictionary<string, ushort> sdtTags = new SortedDictionary<string, ushort>();
    bool bPrevState = true;

    public Stats()
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;
			dtTags = SQL.GetAllManga();
			ResizeLV();
    }

    private void Stats_Load(object sender, EventArgs e)
    {
      SwitchView(1);
    }

    private void ChkBx_FavsOnly0_CheckedChanged(object sender, EventArgs e)
    {
      ShowStats((pnlView0.ContainsFocus) ? 0 : 1);
    }

    private void BtnSwitch_Click(object sender, EventArgs e)
    {
      SwitchView((pnlView0.ContainsFocus) ? 1 : 0);
    }

    /* Toggle visible panel (pie chart & listview) */
    private void SwitchView(int iView)
    {
      this.SuspendLayout();
      //move checkbox to new panel
      ChkBx_FavsOnly.Parent = Controls["pnlView" + iView];
      Controls["pnlView" + iView].BringToFront();
      ChkBx_FavsOnly.BringToFront();

      //change button position & title depending on the panel shown
      BtnSwitch.Location = (iView == 1) ? new Point(82, 12) : new Point(12, 41);
      BtnSwitch.Text = (iView == 1) ? "Chart" : "List";

      BtnSwitch.Parent = Controls["pnlView" + iView];
      Controls["pnlView" + iView].BringToFront();
      BtnSwitch.BringToFront();

      //display stats
      ShowStats(iView);
      this.ResumeLayout();
    }

    private void ShowStats(int iPanel)
    {
      bool bFavsOnly = ChkBx_FavsOnly.Checked;
			int iCount = 0;

      //get stats of tags
      if (bFavsOnly != bPrevState) {
        iCount = 0;
        sdtTags.Clear();
				for (int i = 0; i < dtTags.Rows.Count; i++) {
          if (bFavsOnly
							&& Convert.ToDecimal(dtTags.Rows[i]["Rating"].ToString()) < 5)
            continue;
					foreach (string svar in dtTags.Rows[i]["Tags"].ToString().Split(',')) {
            string sItem = svar.TrimStart();
            if (sdtTags.ContainsKey(sItem))
              sdtTags[sItem]++;
            else
              sdtTags.Add(sItem, 1);
          }
          iCount++;
        }
      }

      if (iPanel == 0) {
        //purge minority tags
        SortedDictionary<string, ushort> dtPie = new SortedDictionary<string, ushort>();
        dtPie.Add("_MO_Other_", 0);
        foreach (KeyValuePair<string, ushort> kvpItem in sdtTags) {
          if ((kvpItem.Value * 1.0 / iCount) < 0.05)
            dtPie["_MO_Other_"] += kvpItem.Value;
          else
            dtPie.Add(kvpItem.Key, kvpItem.Value);
        }
        dtPie.Remove("_MO_Other_");

        //send tag data to pie chart
        chtTags.Series[0].Points.Clear();
        chtTags.Series[0].Points.DataBindXY(dtPie.Keys, dtPie.Values);
      }
      else {
        //send stats to listview
        lvStats.BeginUpdate();
        lvStats.Items.Clear();
        List<ListViewItem> lItems = new List<ListViewItem>(sdtTags.Count + 1);
        foreach (KeyValuePair<string, ushort> kvpItem in sdtTags) {
          ListViewItem lvi = new ListViewItem(kvpItem.Key);
          lvi.SubItems.Add(kvpItem.Value.ToString());
          lvi.SubItems.Add((kvpItem.Value * 1.0 / iCount).ToString("P2"));
          lItems.Add(lvi);
        }
        lvStats.Items.AddRange(lItems.ToArray());
        lvStats.SortRows();
        lvStats.EndUpdate();
      }

      Text = string.Format("Stats: {0} tags in {1} manga", sdtTags.Count, iCount);
      bPrevState = bFavsOnly;
    }

		private void ResizeLV()
		{
      lvStats.BeginUpdate();
      int iColWidth = 0;
      for (int i = 0; i < lvStats.Columns.Count; i++)
        iColWidth += lvStats.Columns[i].Width;

      colTag.Width += lvStats.DisplayRectangle.Width - iColWidth;
      lvStats.EndUpdate();
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

    private void Stats_FormClosing(object sender, FormClosingEventArgs e)
    {
			if (dtTags != null) {
				dtTags.Dispose();
      }

      this.DialogResult = DialogResult.OK;
    }
  }
}