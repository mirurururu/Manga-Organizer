using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using Enc = System.Text.Encoding;

namespace Nagru___Manga_Organizer
{
    public partial class Browse_Img : Form
    {
        public Ionic.Zip.ZipFile zip;
        public List<string> lFiles;
        public int iPage;

        Image imgR, imgL;
        bool bWideL, bWideR, bNext, bZip;
        float fWidth;

        public Browse_Img()
        { InitializeComponent(); }

        private void Browse_Load(object sender, EventArgs e)
        {
            Cursor.Hide();

            picBx.BackColor = Properties.Settings.Default.DefColour;
            #if !DEBUG
                Bounds = Screen.PrimaryScreen.Bounds;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            #endif

            fWidth = (float)(Bounds.Width / 2.0);
            if (zip != null) bZip = true;
            if (iPage == -1) Next();
            else Prev();
        }

        private void Browse_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    Next();
                    break;
                case Keys.Right:
                    Prev();
                    break;
                case Keys.Up:
                    if ((iPage += 7) > lFiles.Count)
                        iPage = iPage - lFiles.Count;
                    Next();
                    break;
                case Keys.Down:
                    if ((iPage -= 7) < 0)
                        iPage = lFiles.Count + iPage;
                    Prev();
                    break;
                case Keys.Home:
                    iPage = -1;
                    Next();
                    break;
                case Keys.End:
                    iPage = lFiles.Count + 1;
                    Prev();
                    break;
                case Keys.F:
                    Cursor.Show();
                    BrowseTo fmGoTo = new BrowseTo();
                    fmGoTo.lFiles = lFiles;
                    fmGoTo.bWL = bWideL;
                    fmGoTo.bWR = bWideR;
                    if (bWideR || bWideL) 
                        fmGoTo.iPage = iPage;
                    else fmGoTo.iPage = iPage - 1;

                    if (fmGoTo.ShowDialog() == DialogResult.OK) 
                    {
                        bNext = false;
                        iPage = fmGoTo.iPage;
                        bWideL = fmGoTo.bWL;
                        bWideR = fmGoTo.bWR;
                        imgL = fmGoTo.imgL;
                        imgR = fmGoTo.imgR;
                        picBx.Refresh();
                    }

                    fmGoTo.Dispose();
                    this.Select();
                    Cursor.Hide();
                    break;
                case Keys.PrintScreen:
                case Keys.MediaNextTrack:
                case Keys.MediaPreviousTrack:
                case Keys.MediaPlayPause:
                case Keys.MediaStop:
                case Keys.VolumeUp:
                case Keys.VolumeDown:
                case Keys.VolumeMute:
                case Keys.LWin:
                case Keys.RWin:
                    break;
                default: Close();
                    break;
            }
        }

        private void Next()
        {
            bNext = true;
            Reset();

            do {
                if (++iPage >= lFiles.Count) iPage = 0;
                imgR = TrySet(iPage);
            }
            while (imgR == null);
            if (!(bWideR = imgR.Height < imgR.Width))
            {
                do {
                    if (++iPage >= lFiles.Count) iPage = 0;
                    imgL = TrySet(iPage);
                }
                while (imgL == null);

                if (bWideL = imgL.Height < imgL.Width)
                    iPage--;
            }
            picBx.Refresh();
        }

        private void Prev()
        {
            if (iPage != 0 && !(bWideR || bWideL)) iPage--;
            bNext = false;
            Reset();

            do {
                if (--iPage < 0) iPage = lFiles.Count - 1;
                else if (iPage >= lFiles.Count) iPage = 0;
                imgL = TrySet(iPage);
            }
            while (imgL == null);
            if (!(bWideL = imgL.Height < imgL.Width))
            {
                do {
                    if (--iPage < 0) iPage = lFiles.Count - 1;
                    imgR = TrySet(iPage);
                }
                while (imgR == null);
                iPage++;
                bWideR = imgR.Height < imgR.Width;
            }
            picBx.Refresh();
        }

        private Bitmap TrySet(int i)
        {
            try
            {
                if (bZip && !File.Exists(lFiles[i]))
                    zip[i].Extract(zip.TempFileFolder);
                return ExtImage.Scale(new Bitmap(
                    lFiles[i]), picBx.Width, picBx.Height);
            }
            catch { return null; }
        }

        private void Reset()
        {
            bWideL = false;
            bWideR = false;
            imgL = null;
            imgR = null;
            GC.Collect();
        }

        /* Process which images to draw & how */
        private void picBx_Paint(object sender, PaintEventArgs e)
        {
            picBx.SuspendLayout();
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (!bWideL && !bWideR) {
                DrawImage_L(g, imgL);
                DrawImage_R(g, imgR);
            }
            else if (bWideL) {
                if (!bNext)
                    DrawImage_L(g, imgL);
                else DrawImage_R(g, imgR);
            }
            else /*if (bWideR)*/ {
                if (bNext)
                    DrawImage_R(g, imgR);
                else DrawImage_L(g, imgL);
            }
            picBx.ResumeLayout();
        }

        private void DrawImage_L(Graphics g, Image img)
        {
            g.DrawImage(img,
                (bWideL) ? (int)(fWidth - img.Width / 2.0) : fWidth - img.Width - 5,
                (bWideL) ? (int)(picBx.Height / 2.0 - img.Height / 2.0) : 0,
                img.Width, img.Height);
        }
        private void DrawImage_R(Graphics g, Image img)
        {
            g.DrawImage(img,
                (bWideR) ? (int)(fWidth - img.Width / 2.0) : fWidth + 5,
                (bWideR) ? (int)(picBx.Height / 2.0 - img.Height / 2.0) : 0,
                img.Width, img.Height);
        }

        private void Browse_MouseUp(object sender, MouseEventArgs e)
        { this.Close(); }

        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        {
            imgL.Dispose();
            imgR.Dispose();
            Cursor.Show();
            iPage += 2;
        }
    }
}
