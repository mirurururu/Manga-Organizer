using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;

namespace Nagru___Manga_Organizer
{
    public partial class Browse_Img : Form
    {
        public int iPage;
        public string sPath;

        List<string> lFiles = new List<string>(25);
        Image imgR = null, imgL = null;
        bool bWideL, bWideR, bNext;
        float fWidth;

        /* Initialize form */
        public Browse_Img()
        { InitializeComponent(); }

        /* Set form to fullscreen and grab files */
        private void Browse_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            
            //set fullscreen
            Bounds = Screen.PrimaryScreen.Bounds;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            fWidth = (float)(this.Width / 2.0);

            //get files
            lFiles.AddRange(ExtDirectory.GetFiles(sPath,
                SearchOption.TopDirectoryOnly));

            if (iPage == -1) Next();
            else Prev();
        }

        /* Navigate files or close form */
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
                    this.Focus();
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

        /* Go to next page */
        void Next()
        {
            bNext = true;
            Reset();

            if (++iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgR = Image.FromStream(fs);
            if (!(bWideR = imgR.Height < imgR.Width))
            {
                if (++iPage >= lFiles.Count) iPage = 0;
                using (FileStream fs = new FileStream(
                    lFiles[iPage], FileMode.Open, FileAccess.Read))
                    imgL = Image.FromStream(fs);
                if (bWideL = imgL.Height < imgL.Width)
                    iPage--;
            }
            picBx.Refresh();
        }

        /* Go to previous page */
        void Prev()
        {
            if (iPage != 0 && !(bWideR || bWideL)) iPage--;
            bNext = false;
            Reset();

            if (--iPage < 0) iPage = lFiles.Count - 1;
            else if (iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgL = Image.FromStream(fs);
            if (!(bWideL = imgL.Height < imgL.Width))
            {
                if (iPage - 1 < 0) iPage = lFiles.Count;
                using (FileStream fs = new FileStream(
                    lFiles[iPage - 1], FileMode.Open, FileAccess.Read))
                    imgR = Image.FromStream(fs);
                bWideR = imgR.Height < imgR.Width;
            }
            picBx.Refresh();
        }

        /* Set flags back to default */
        void Reset()
        {
            bWideL = false;
            bWideR = false;
        }

        /* Process which images to draw & how */
        private void picBx_Left_Paint(object sender, PaintEventArgs e)
        {
            picBx.SuspendLayout();
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (!bWideL && !bWideR)
            {
                DrawImage_L(g, ResizeImage(imgL, fWidth, picBx.Height));
                DrawImage_R(g, ResizeImage(imgR, fWidth, picBx.Height));
            }
            else if (bWideL)
            {
                if (!bNext)
                    DrawImage_L(g, ResizeImage(imgL, picBx.Width, picBx.Height));
                else DrawImage_R(g, ResizeImage(imgR, fWidth, picBx.Height));
            }
            else /* if (bWideR) */
            {
                if (bNext)
                    DrawImage_R(g, ResizeImage(imgR, picBx.Width, picBx.Height));
                else DrawImage_L(g, ResizeImage(imgL, fWidth, picBx.Height));
            }
            picBx.ResumeLayout();
        }

        /* Draw image to picBx */
        private void DrawImage_L(Graphics g, Image img)
        {
            g.DrawImage(img,
                (bWideL) ? (int)(fWidth - img.Width / 2.0) : fWidth - img.Width,
                (bWideL) ? (int)(picBx.Height / 2.0 - img.Height / 2.0) : 0,
                img.Width,
                img.Height);
        }
        private void DrawImage_R(Graphics g, Image img)
        {
            g.DrawImage(img,
                (bWideR) ? (int)(fWidth - img.Width / 2.0) : fWidth,
                (bWideR) ? (int)(picBx.Height / 2.0 - img.Height / 2.0) : 0,
                img.Width,
                img.Height);
        }

        /* Proper image scaling   
           Author: MBigglesWorth (May 5, 2011) */
        public static Image ResizeImage(Image img, float fWidth, int iHeight)
        {
            if (img.Width > img.Height && fWidth > img.Width) return img;

            float fPerWidth = fWidth / (float)img.Width;
            float fPerHeight = (float)iHeight / (float)img.Height;
            float fAdj = fPerHeight < fPerWidth ? fPerHeight : fPerWidth;
            int iNewWidth = (int)(img.Width * fAdj);
            int iNewHeight = (int)(img.Height * fAdj);

            Image newImage = new Bitmap(iNewWidth, iNewHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, iNewWidth, iNewHeight);
            }
            return newImage;
        }

        /* Alternative form closing method  */
        private void Browse_MouseUp(object sender, MouseEventArgs e)
        { this.Close(); }

        /* Re-enable cursor when finished browsing */
        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        {
            iPage += 2;
            Cursor.Show();
        }
    }
}
