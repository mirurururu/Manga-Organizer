using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using SCA = SharpCompress.Archive;

namespace Nagru___Manga_Organizer
{
    public partial class BrowseTo : Form
    {
        public Dictionary<int, int> dtSort = new Dictionary<int, int>();
        public List<string> lFiles { private get; set; }
        public SCA.IArchiveEntry[] Archive { get; set; }
        public Rectangle rScale { private get; set; }
        public Image imgL { get; private set; }
        public Image imgR { get; private set; }
        public bool bWL { get; private set;}
        public bool bWR { get; private set; }
        public int iPage { get; set; }
        
        public BrowseTo()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Abort;
        }

        private void Browse_GoTo_Load(object sender, EventArgs e)
        {
            //set grid on/off
            if (Properties.Settings.Default.DefGrid)
                LV_Pages.GridLines = true;
            
            //add pages to listview
            for (int i = 0; i < lFiles.Count; i++) {
                LV_Pages.Items.Add(new ListViewItem(
                    Path.GetFileName(lFiles[i])));
            }
            Col_Page.Width = LV_Pages.DisplayRectangle.Width;
            LV_SelectPages();

            //wrap page values & select pages
            if (iPage < 0) iPage = lFiles.Count - 1;
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
            switch (e.KeyCode) {
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
        
        /* Return selected pages to Browse_Img */
        private void UpdatePage()
        {
            byte by = 0;
            iPage--;

            do {
                by++;
                if (++iPage >= lFiles.Count) iPage = 0;
                imgR = TrySet(iPage);
            } while (imgR == null && by < 10);

            if (!(bWR = imgR.Height < imgR.Width)) {
                by = 0;
                do {
                    by++;
                    if (++iPage >= lFiles.Count) iPage = 0;
                    imgL = TrySet(iPage);
                } while (imgL == null && by < 10);

                if (bWL = imgL.Height < imgL.Width)
                    iPage--;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private Bitmap TrySet(int i)
        {
            Bitmap bmpTmp = null;
            MemoryStream ms = new MemoryStream();

            try {
                if (Archive != null) {
                    Archive[dtSort[i]].WriteTo(ms);
                } else {
                    FileStream fs = new FileStream(lFiles[i], FileMode.Open);
                    fs.CopyTo(ms);
                    fs.Dispose();
                }

                bmpTmp = ExtImage.Scale(new Bitmap(ms),
                    rScale.Width, rScale.Height);
            } catch (Exception Ex) {
                Console.WriteLine(Ex.Message);
            } finally {
                ms.Dispose();
            }

            return bmpTmp;
        }
    }
}