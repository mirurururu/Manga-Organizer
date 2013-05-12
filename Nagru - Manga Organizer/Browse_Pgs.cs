using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class BrowseTo : Form
    {
        public List<string> lFiles;
        public Image imgL { get; private set; }
        public Image imgR { get; private set; }
        public int iPage { get; set; }
        public bool bWL, bWR;
        
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
            LV_SelectPages();
            
            if (LV_Pages.Items.Count == 0) return;
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

        private void BrowseTo_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    UpdatePage();
                    break;
                default:
                    this.Close();
                    break;
            }
            e.Handled = true;
        }

        private void LV_Pages_DoubleClick(object sender, EventArgs e)
        { UpdatePage(); }
        
        private void UpdatePage()
        {
            if (iPage == 0 && !(bWL || bWR))
                if(++iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgL = Image.FromStream(fs);
            if (!(bWL = imgL.Height < imgL.Width))
            {
                if (iPage - 1 < 0) iPage = lFiles.Count;
                using (FileStream fs = new FileStream(
                    lFiles[iPage - 1], FileMode.Open, FileAccess.Read))
                    imgR = Image.FromStream(fs);
                bWR = imgR.Height < imgR.Width;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}