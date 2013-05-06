using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Nagru___Manga_Organizer
{
    public partial class Browse_Img : Form
    {
        public int iPage { get; set; }
        public string sPath { get; set; }

        List<string> lFiles = new List<string>(25);
        int iWidth = Screen.PrimaryScreen.Bounds.Width / 2;
        Image imgR = null, imgL = null;
        bool bWideL, bWideR;
        bool bFocus = true;

        /* Initialize form */
        public Browse_Img()
        { InitializeComponent(); }

        /* Set form to fullscreen and grab files */
        private void Browse_Load(object sender, EventArgs e)
        {
            this.LostFocus += new EventHandler(Browse_LostFocus);
            Cursor.Hide();
            
            //set fullscreen
            Bounds = Screen.PrimaryScreen.Bounds;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //set picBx positions
            picBx_Left.Location = new Point(0, 0);
            picBx_Right.Location = new Point(iWidth, 0);
            picBx_Right.Width = iWidth;

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
                    fmGoTo.iPage = iPage;

                    if (fmGoTo.ShowDialog() == DialogResult.OK) 
                    {
                        iPage = fmGoTo.iPage;
                        if (iPage != 0 && picBx_Left.Width == iWidth) iPage++;
                        Prev();
                    }

                    fmGoTo.Dispose();
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
            Reset();

            if (++iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgR = Image.FromStream(fs);
                picBx_Right.Image = imgR;
            if (++iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgL = Image.FromStream(fs);

            bWideR = imgR.Height < imgR.Width;
            bWideL = imgL.Height < imgL.Width;

            if (!bWideL && !bWideR)
                picBx_Left.Image = imgL;
            else if (bWideR)
            {
                picBx_Left.Image = imgR;
                picBx_Left.Width =
                    Screen.PrimaryScreen.Bounds.Width;
                iPage--;
            }
            else iPage--;
        }

        /* Go to previous page */
        void Prev()
        {
            if (iPage != 0 && picBx_Left.Width == iWidth) iPage--;
            Reset();

            if (--iPage < 0) iPage = lFiles.Count - 1;
            else if (iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                picBx_Left.Image = imgL = Image.FromStream(fs);
            if (iPage - 1 < 0) iPage = lFiles.Count;
            using (FileStream fs = new FileStream(
                lFiles[iPage - 1], FileMode.Open, FileAccess.Read))
                imgR = Image.FromStream(fs);

            bWideR = imgR.Height < imgR.Width;
            bWideL = imgL.Height < imgL.Width;

            if (!bWideL && !bWideR)
                picBx_Right.Image = imgR;
            else if (bWideL)
                picBx_Left.Width =
                    Screen.PrimaryScreen.Bounds.Width;
            else iPage++;
        }

        /* Clear picBxs before populating them again */
        void Reset()
        {
            if (picBx_Left.Image != null)
                picBx_Left.Image.Dispose();
            if (picBx_Right.Image != null)
                picBx_Right.Image.Dispose();
            if (picBx_Left.Width != iWidth)
                picBx_Left.Width = iWidth;
            picBx_Left.Image = null;
            picBx_Right.Image = null;
        }

        /* Prevent form close when user is re-focusing */
        void Browse_LostFocus(object sender, EventArgs e)
        { bFocus = false; }

        /* Alternative form closing method  */
        private void Browse_MouseUp(object sender, MouseEventArgs e)
        {
            if (bFocus) this.Close();
            else bFocus = true;
        }

        /* Re-enable cursor when finished browsing */
        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        {
            iPage++;
            Cursor.Show();
        }
    }
}
