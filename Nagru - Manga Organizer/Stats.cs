using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    public partial class Stats : Form
    {
        SortedDictionary<string, int> sdtTags = new SortedDictionary<string, int>();
        LVsorter lvSortObj = new LVsorter();

        /* Show how many times a tag is used */
        public Stats(List<Main.stEntry> lPass)
        { 
            InitializeComponent();
            LV_Stats.ListViewItemSorter = lvSortObj;

            for (int i = 0; i < lPass.Count; i++)
                foreach (string svar in lPass[i].sTags.Split(','))
                {
                    svar.TrimStart();
                    if (sdtTags.ContainsKey(svar)) sdtTags[svar]++;
                    else sdtTags.Add(svar, 1);
                }

            foreach (KeyValuePair<string, int> kvpItem in sdtTags)
            {
                ListViewItem lvi = new ListViewItem(kvpItem.Key);
                lvi.SubItems.Add(kvpItem.Value.ToString());
                lvi.SubItems.Add((kvpItem.Value / (lPass.Count * 1.0)).ToString("P2"));
                LV_Stats.Items.Add(lvi);
            }
        }

        private void Stats_Load(object sender, EventArgs e)
        { ResizeLV(); }

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
    }
}
