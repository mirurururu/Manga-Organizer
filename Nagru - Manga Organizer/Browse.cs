using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Nagru___Manga_Organizer
{
    public partial class Browse : Form
    {
        public int iPage { get; set; }
        public string sPath { get; set; }

        List<string> lFiles = new List<string>(25);
        int iWidth = Screen.PrimaryScreen.Bounds.Width / 2;
        Image imgR = null, imgL = null;
        bool bWideL, bWideR;

        /* Initialize form */
        public Browse()
        { InitializeComponent(); }

        /* Set form to fullscreen and grab files */
        private void Browse_Load(object sender, EventArgs e)
        {
            Cursor.Hide();

            //set fullscreen
            TopMost = true;
            Location = new Point(0, 0);
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

            if (iPage == -1) GoLeft();
            else GoRight();
        }

        /* Navigate files or close form */
        private void Browse_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    GoLeft();
                    break;
                case Keys.Right:
                    GoRight();
                    break;
                case Keys.Up:
                    if ((iPage += 7) > lFiles.Count)
                        iPage = iPage - lFiles.Count;
                    GoLeft();
                    break;
                case Keys.Down:
                    if ((iPage -= 7) < 0)
                        iPage = lFiles.Count + iPage;
                    GoRight();
                    break;
                case Keys.Home:
                    iPage = -1;
                    GoLeft();
                    break;
                case Keys.End:
                    iPage = lFiles.Count + 1;
                    GoRight();
                    break;
                case Keys.PrintScreen:
                case Keys.MediaNextTrack:
                case Keys.MediaPreviousTrack:
                case Keys.MediaPlayPause:
                case Keys.MediaStop:
                case Keys.VolumeUp:
                case Keys.VolumeDown:
                case Keys.VolumeMute:
                    break;
                default: Close();
                    break;
            }
        }

        /* Go to next page */
        void GoLeft()
        {
            Reset();

            if (++iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgR = Image.FromStream(fs);
            if (++iPage >= lFiles.Count) iPage = 0;
            using (FileStream fs = new FileStream(
                lFiles[iPage], FileMode.Open, FileAccess.Read))
                imgL = Image.FromStream(fs);

            bWideR = imgR.Height < imgR.Width;
            bWideL = imgL.Height < imgL.Width;

            if (!bWideL && !bWideR)
            {
                picBx_Right.Image = imgR;
                picBx_Left.Image = imgL;
            }
            else if (bWideR || bWideL && bWideR)
            {
                picBx_Left.Image = imgR;
                picBx_Left.Width =
                    Screen.PrimaryScreen.Bounds.Width;
                iPage--;
            }
            else
            {
                picBx_Right.Image = imgR;
                iPage--;
            }
        }

        /* Go to previous page */
        void GoRight()
        {
            if (iPage != 0 && picBx_Left.Width == iWidth) iPage--;
            Reset();

            if (--iPage < 0) iPage = lFiles.Count - 1;
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

        /* Alternative closing method */
        private void UserClick(object sender, MouseEventArgs e)
        { Close(); }

        /* Re-enable cursor when finished browsing */
        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        {
            TopMost = false;
            Cursor.Show();
        }
    }
}
