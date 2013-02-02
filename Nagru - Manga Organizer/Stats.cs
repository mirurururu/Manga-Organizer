using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    public partial class Stats : Form
    {
        public List<Main.stEntry> CurrentItems
        { set { lCurr = value; } }

        List<Main.stEntry> lCurr;
        LVsorter lvSortObj = new LVsorter();

        /* Show how many times a tag is used */
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

        /* Sort entries based on clicked column   */
        private void LV_Stats_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != lvSortObj.ColToSort)
                lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
            else lvSortObj.SwapOrder(); //reverse sort order

            LV_Stats.Sort();
        }

        /* Auto-resizes Col_Tags to match Form width   */
        private void LV_Stats_Resize(object sender, EventArgs e)
        { ResizeLV(); }
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
            Dictionary<string, int> sdtTags = new Dictionary<string, int>();
            double dCount = 0;

            for (int i = 0; i < lCurr.Count; i++)
            {
                if (ChkBx_FavStats.Checked && !lCurr[i].bFav) continue;
                foreach (string svar in lCurr[i].sTags.Split(','))
                {
                    string sItem = svar.TrimStart();
                    if (sdtTags.ContainsKey(sItem)) sdtTags[sItem]++;
                    else sdtTags.Add(sItem, 1);
                }
                dCount++;
            }

            LV_Stats.Items.Clear();
            foreach (KeyValuePair<string, int> kvpItem in sdtTags)
            {
                ListViewItem lvi = new ListViewItem(kvpItem.Key);
                lvi.SubItems.Add(kvpItem.Value.ToString());
                lvi.SubItems.Add((kvpItem.Value / dCount).ToString("P2"));
                LV_Stats.Items.Add(lvi);
            }
            lvSortObj.ColToSort = 2;
            lvSortObj.OrdOfSort = SortOrder.Descending;
            LV_Stats.Sort();

            Text = string.Format("Stats: {0} entries", dCount);
        }
    }
}
