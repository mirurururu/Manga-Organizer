using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace Nagru___Manga_Organizer
{
    /* Show how many times a tag is used */
    public partial class Stats : Form
    {
        public List<Main.csEntry> lCurr { private get; set; }
        SortedDictionary<string, ushort> dtTags = new SortedDictionary<string, ushort>();
        LVsorter lvSortObj = new LVsorter();
        bool bPrev = true;
        int iCount = 0;

        public Stats()
        { InitializeComponent(); }

        private void Stats_Load(object sender, EventArgs e)
        {
            lvStats.ListViewItemSorter = lvSortObj;
            ShowStats(false);
        }

        private void SwitchView(int iView)
        {
            this.SuspendLayout();
            pnlView0.Visible = false;
            pnlView1.Visible = false;
            Controls["pnlView" + iView].Visible = true;
            this.ResumeLayout();

            ShowStats(ChkBx_FavsOnly0.Checked);
        }
        
        private void ShowStats(bool bFavsOnly)
        {
            if(bFavsOnly != bPrev) {
                iCount = 0;
                dtTags.Clear();
                for (int i = 0; i < lCurr.Count; i++) {
                    if (bFavsOnly && lCurr[i].byRat < 5) continue;
                    foreach (string svar in lCurr[i].sTags.Split(',')) {
                        string sItem = svar.TrimStart();
                        if (dtTags.ContainsKey(sItem)) dtTags[sItem]++;
                        else dtTags.Add(sItem, 1);
                    }
                    iCount++;
                }
            }

            if(pnlView0.Visible) {
                SortedDictionary<string, ushort> dtPie = new SortedDictionary<string, ushort>();
                dtPie.Add("_Other_", 0);
                foreach (KeyValuePair<string, ushort> kvpItem in dtTags) {
                    if ((kvpItem.Value * 1.0 / iCount) < 0.025)
                        dtPie["_Other_"] += kvpItem.Value;
                    else dtPie.Add(kvpItem.Key, kvpItem.Value);
                }
                dtPie.Remove("_Other_");
                chtTags.Series[0].Points.DataBindXY(dtPie.Keys, dtPie.Values);
            }
            else {
                lvStats.BeginUpdate();
                lvStats.Items.Clear();
                List<ListViewItem> lItems = new List<ListViewItem>(dtTags.Count + 1);
                foreach (KeyValuePair<string, ushort> kvpItem in dtTags) {
                    ListViewItem lvi = new ListViewItem(kvpItem.Key);
                    lvi.SubItems.Add(kvpItem.Value.ToString());
                    lvi.SubItems.Add((kvpItem.Value * 1.0 / iCount).ToString("P2"));
                    lItems.Add(lvi);
                }
                lvStats.Items.AddRange(lItems.ToArray());
                lvSortObj.ColToSort = 2;
                lvSortObj.OrdOfSort = SortOrder.Descending;
                lvStats.Sort();
                lvStats.EndUpdate();
                lvStats.Select();
            }
            
            Text = string.Format("Stats: {0} tags in {1} manga", dtTags.Count, iCount);
            bPrev = bFavsOnly;
        }

        private void lvStats_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != lvSortObj.ColToSort)
                lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
            else lvSortObj.SwapOrder();
            lvStats.Sort();
        }

        private void lvStats_Resize(object sender, EventArgs e)
        { ResizeLV(); }
        private void ResizeLV()
        {
            lvStats.BeginUpdate();
            int iColWidth = 0;
            for (int i = 0; i < lvStats.Columns.Count; i++)
                iColWidth += lvStats.Columns[i].Width;

            colTag.Width += lvStats.DisplayRectangle.Width - iColWidth;
            lvStats.EndUpdate();
        }

        private void lvStats_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvStats.Columns[e.ColumnIndex].Width;
        }
        
        private void Stats_FormClosing(object sender, FormClosingEventArgs e)
        { this.DialogResult = DialogResult.OK; }

        private void ChkBx_FavsOnly0_CheckedChanged(object sender, EventArgs e)
        {
            ChkBx_FavsOnly1.Checked = ChkBx_FavsOnly0.Checked;
            ShowStats(ChkBx_FavsOnly0.Checked);
        }
        private void ChkBx_FavsOnly1_CheckedChanged(object sender, EventArgs e)
        {
            ChkBx_FavsOnly0.Checked = ChkBx_FavsOnly1.Checked;
            ShowStats(ChkBx_FavsOnly1.Checked);
        }

        private void BtnSwitch_Click(object sender, EventArgs e)
        {
            SwitchView((pnlView0.Visible) ? 1 : 0);
        }
    }
}