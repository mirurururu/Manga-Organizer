using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class BrowseTo : Form
    {
        public List<string> lFiles;
        public int iPage { get; set; }

        public BrowseTo()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Abort;
        }

        private void Browse_GoTo_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < lFiles.Count; i++)
                LV_Pages.Items.Add(new ListViewItem(lFiles[i]));
            Col_Page.Width = LV_Pages.DisplayRectangle.Width;

            if (iPage > 0) iPage--;
            LV_SelectPages();

            /* Compensate for broken scroll-to function
             * by running it multiple times (3 is sweet-spot) */
            LV_Pages.TopItem = LV_Pages.Items[iPage];
            LV_Pages.TopItem = LV_Pages.Items[iPage];
            LV_Pages.TopItem = LV_Pages.Items[iPage];
        }

        private void LV_Pages_Resize(object sender, EventArgs e)
        { Col_Page.Width = LV_Pages.DisplayRectangle.Width; }

        private void LV_Pages_Click(object sender, EventArgs e)
        {
            iPage = LV_Pages.SelectedItems[0].Index;
            LV_SelectPages();
        }

        private void LV_SelectPages()
        {
            int iNext = iPage + 1;
            if (iNext > LV_Pages.Items.Count - 1) iNext = 0;

            LV_Pages.SelectedItems.Clear();
            LV_Pages.FocusedItem = LV_Pages.Items[iPage];
            LV_Pages.Items[iPage].Selected = true;
            LV_Pages.Items[iNext].Selected = true;
        }

        private void Browse_GoTo_KeyUp(object sender, KeyEventArgs e)
        { 
            if (e.KeyCode == Keys.Enter) 
                UpdatePage();
            e.Handled = true;
        }

        private void LV_Pages_DoubleClick(object sender, EventArgs e)
        { UpdatePage(); }

        private void UpdatePage()
        {
            Image img;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                img = Image.FromStream(fs);

            if (img.Height < img.Width)
            {
                for (int i = 0; i < 3; i++)
                    if (--iPage < 0)
                        iPage = LV_Pages.Items.Count - 1;
            }
            else iPage += 2;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}