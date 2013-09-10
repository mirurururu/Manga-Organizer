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
        public Dictionary<int, int> Sort = new Dictionary<int, int>();
        public List<string> Files { get; set; }
        public Ionic.Zip.ZipFile ZipFile { get; set; }
        public int Page { get; set; }

        Image imgR, imgL;
        bool bWideL, bWideR, bNext;
        float fWidth;

        Timer trFlip;
        bool bAuto = false;

        public Browse_Img()
        { InitializeComponent(); }

        private void Browse_Load(object sender, EventArgs e)
        {
            //Compensate for zip files being improperly sorted
            if (ZipFile != null) {
                int i = 0;
                Files.Sort(new TrueCompare());
                ICollection<string> zeBase = ZipFile.EntryFileNames;
                foreach(string sFileName in zeBase) {
                    for (int b = 0; b < Files.Count; b++) {
                        if (sFileName != Path.GetFileName(Files[b]))
                            continue;
                        Sort.Add(b, i++);
                        break;
                    }
                }
            }

            //set up timer
            trFlip = new Timer();
            trFlip.Tick += trFlip_Tick;
            trFlip.Interval = Properties.Settings.Default.Interval;

            picBx.BackColor = Properties.Settings.Default.DefColour;
            #if !DEBUG
                Cursor.Hide();
                Bounds = Screen.PrimaryScreen.Bounds;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            #endif

            fWidth = (float)(Bounds.Width / 2.0);
            if (Page == -1) Next();
            else Prev();
        }

        private void Browse_KeyDown(object sender, KeyEventArgs e)
        {
            if (bAuto && e.Modifiers == Keys.Shift) {
                if (e.KeyCode == Keys.Oemplus) {
                    bAuto = true;
                    Console.Beep(700, 100);
                    trFlip.Interval += 500;
                    Tmr_Reset();
                }
                else if (e.KeyCode == Keys.OemMinus) {
                    if (trFlip.Interval >= 1500) {
                        bAuto = true;
                        Console.Beep(100, 100);
                        trFlip.Interval -= 500;
                        Tmr_Reset();
                    }
                }
                return;
            }

            switch (e.KeyCode)
            {
                #region Traversal
                case Keys.Left: 
                    Next();
                    break;
                case Keys.Right:
                    Prev();
                    break;
                case Keys.Up:
                    if ((Page += 7) > Files.Count)
                        Page = Page - Files.Count;
                    Next();
                    break;
                case Keys.Down:
                    if ((Page -= 7) < 0)
                        Page = Files.Count + Page;
                    Prev();
                    break;
                case Keys.Home:
                    Page = -1;
                    Next();
                    break;
                case Keys.End:
                    Page = Files.Count + 1;
                    Prev();
                    break;
                #endregion
                #region Special Functions
                case Keys.F:
                    if (bAuto) trFlip.Stop();
                    Cursor.Show();
                    BrowseTo fmGoTo = new BrowseTo();
                    fmGoTo.Zip = ZipFile;
                    fmGoTo.lFiles = Files;
                    fmGoTo.dtSort = Sort;
                    fmGoTo.iPage = (bWideR || bWideL) ? 
                        Page : Page - 1;

                    if (fmGoTo.ShowDialog() == DialogResult.OK) {
                        bNext = true;
                        Page = fmGoTo.iPage;
                        bWideL = fmGoTo.bWL;
                        bWideR = fmGoTo.bWR;
                        imgR = ExtImage.Scale(fmGoTo.imgR, picBx.Width, picBx.Height);
                        imgL = (fmGoTo.imgL != null) ? 
                            ExtImage.Scale(fmGoTo.imgL, picBx.Width, picBx.Height) : null;
                        picBx.Refresh();
                    }

                    fmGoTo.Dispose();
                    if (bAuto) trFlip.Start();
                    this.Select();
                    Cursor.Hide();
                    break;
                case Keys.S:
                    bAuto = !bAuto;
                    if (bAuto) {
                        Console.Beep(500, 100);
                        trFlip.Start();
                    }
                    else {
                        Console.Beep(300, 100);
                        trFlip.Stop();
                    }
                    break;
                #endregion
                #region Ignored Keys
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
                #endregion
                default: Close();
                    break;
            }
        }

        private void Next()
        {
            bNext = true;
            Reset();

            do {
                if (++Page >= Files.Count) Page = 0;
                imgR = TrySet(Page);
            } while (imgR == null);

            if (!(bWideR = imgR.Height < imgR.Width)) {
                do {
                    if (++Page >= Files.Count) Page = 0;
                    imgL = TrySet(Page);
                } while (imgL == null);

                if (bWideL = imgL.Height < imgL.Width)
                    Page--;
            }
            picBx.Refresh();
        }

        private void Prev()
        {
            if (Page != 0 && !(bWideR || bWideL)) Page--;
            bNext = false;
            Reset();

            do {
                if (--Page < 0) Page = Files.Count - 1;
                else if (Page >= Files.Count) Page = 0;
                imgL = TrySet(Page);
            } while (imgL == null);

            if (!(bWideL = imgL.Height < imgL.Width)) {
                do {
                    if (--Page < 0) Page = Files.Count - 1;
                    imgR = TrySet(Page);
                } while (imgR == null);
                
                Page++;
                bWideR = imgR.Height < imgR.Width;
            }
            picBx.Refresh();
        }

        private Bitmap TrySet(int i)
        {
            try {
                if (ZipFile != null && !File.Exists(Files[i]))
                    ZipFile[Sort[i]].Extract(ZipFile.TempFileFolder);
                return ExtImage.Scale(new Bitmap(Files[i]), 
                    picBx.Width, picBx.Height);
            }
            catch { return null; }
        }

        private void Reset()
        {
            if (bAuto) Tmr_Reset();
            bWideL = false;
            bWideR = false;
            imgL = null;
            imgR = null;
            GC.Collect(0);
        }
        private void Tmr_Reset()
        {
            trFlip.Stop();
            trFlip.Start();
        }

        void trFlip_Tick(object sender, EventArgs e)
        { Next(); }

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
            else if (bNext)
                DrawImage_R(g, imgR);
            else DrawImage_L(g, imgL);
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
            if(imgL != null) imgL.Dispose();
            if(imgR != null) imgR.Dispose();
            if (bAuto) {
                Properties.Settings.Default.Interval = trFlip.Interval;
                Properties.Settings.Default.Save();
                trFlip.Stop();
            }
            trFlip.Dispose();
            GC.Collect(0);

            Cursor.Show();
            Page += 2;
        }
    }
}
