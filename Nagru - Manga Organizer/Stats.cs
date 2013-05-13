using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    /* Show how many times a tag is used */
    public partial class Stats : Form
    {
        public List<Main.stEntry> lCurr { private get; set; }
        LVsorter lvSortObj = new LVsorter();
        
        public Stats()
        {
            InitializeComponent();
            LV_Stats.ListViewItemSorter = lvSortObj;
        }

        private void Stats_Load(object sender, EventArgs e)
        {
            ShowStats();
            ResizeLV();
        }

        private void LV_Stats_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != lvSortObj.ColToSort)
                lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
            else lvSortObj.SwapOrder();
            LV_Stats.Sort();
        }

        /* Auto-resizes Col_Tags to match Form width   */
        private void LV_Stats_Resize(object sender, EventArgs e) { ResizeLV(); }
        private void ResizeLV()
        {
            LV_Stats.BeginUpdate();
            int iColWidth = 0;
            for (int i = 0; i < LV_Stats.Columns.Count; i++)
                iColWidth += LV_Stats.Columns[i].Width;

            colTags.Width += LV_Stats.DisplayRectangle.Width - iColWidth;
            LV_Stats.EndUpdate();
        }

        private void ChkBx_FavStats_CheckedChanged(object sender, EventArgs e)
        { ShowStats(); }

        private void ShowStats()
        {
            Dictionary<string, ushort> sdtTags = new Dictionary<string, ushort>();
            ushort iCount = 0;

            for (int i = 0; i < lCurr.Count; i++)
            {
                if (ChkBx_FavStats.Checked && !lCurr[i].bFav) continue;
                foreach (string svar in lCurr[i].sTags.Split(','))
                {
                    string sItem = svar.TrimStart();
                    if (sdtTags.ContainsKey(sItem)) sdtTags[sItem]++;
                    else sdtTags.Add(sItem, 1);
                }
                iCount++;
            }

            LV_Stats.BeginUpdate();
            LV_Stats.Items.Clear();
            List<ListViewItem> lItems = new List<ListViewItem>(sdtTags.Count + 1);
            foreach (KeyValuePair<string, ushort> kvpItem in sdtTags)
            {
                ListViewItem lvi = new ListViewItem(kvpItem.Key);
                lvi.SubItems.Add(kvpItem.Value.ToString());
                lvi.SubItems.Add((kvpItem.Value * 1.0 / iCount).ToString("P2"));
                lItems.Add(lvi);
            }
            LV_Stats.Items.AddRange(lItems.ToArray());
            lvSortObj.ColToSort = 2;
            lvSortObj.OrdOfSort = SortOrder.Descending;
            LV_Stats.Sort();
            LV_Stats.EndUpdate();

            Text = string.Format("Stats: {0} tags in {1} manga", sdtTags.Count, iCount);
            LV_Stats.Select();
        }

        private void Stats_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
