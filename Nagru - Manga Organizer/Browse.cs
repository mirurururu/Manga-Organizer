using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Nagru___Manga_Organizer
{
    public partial class Browse : Form
    {
        public string sPath { get; set; }
        List<string> lFiles = new List<string>(25);
        int iWidth = Screen.PrimaryScreen.Bounds.Width / 2;
        int imgL, imgR;
        bool bWideL, bWideR;
        int iPage = -1;

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
            lFiles = ExtDirectory.GetFiles(sPath,
                SearchOption.TopDirectoryOnly);

            lFiles.Sort(new TrueCompare());
            GoLeft();
        }

        /* Navigate files or close form */
        private void Browse_KeyDown(object sender, KeyEventArgs e)
        {
            picBx_Left.SuspendLayout();
            picBx_Right.SuspendLayout();

            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Up:
                    GoRight();
                    break;
                case Keys.Left:
                case Keys.Down:
                    GoLeft();
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

            picBx_Right.ResumeLayout();
            picBx_Left.ResumeLayout();
        }

        /* Traverse images leftwards */
        void GoLeft()
        {
            Reset();

            if (++iPage >= lFiles.Count) iPage = 0;
            imgR = iPage;
            if (++iPage >= lFiles.Count) iPage = 0;
            imgL = iPage;

            bWideR = IsLandscape(lFiles[imgR]);
            bWideL = IsLandscape(lFiles[imgL]);
            picBx_Right.ImageLocation = lFiles[imgR];

            if (!bWideL && !bWideR)
                picBx_Left.ImageLocation = lFiles[imgL];
            else if (bWideL && bWideR || bWideR)
            {
                picBx_Left.ImageLocation = lFiles[imgR];
                picBx_Left.Width =
                    Screen.PrimaryScreen.Bounds.Width;
                iPage--;
            }
            else iPage--;
        }

        /* Traverse images rightward */
        void GoRight()
        {
            if (iPage != 0 && picBx_Left.Width == iWidth) iPage--;
            Reset();

            if (--iPage < 0) iPage = lFiles.Count - 1;
            imgL = iPage;
            if (iPage - 1 < 0) iPage = lFiles.Count;
            imgR = iPage - 1;

            bWideR = IsLandscape(lFiles[imgR]);
            bWideL = IsLandscape(lFiles[imgL]);
            picBx_Left.ImageLocation = lFiles[imgL];

            if (!bWideL && !bWideR)
                picBx_Right.ImageLocation = lFiles[imgR];
            else if (bWideL)
                picBx_Left.Width =
                    Screen.PrimaryScreen.Bounds.Width;
            else iPage++;
        }

        /* Returns compared proportions of image */
        bool IsLandscape(string path)
        {
            using (Bitmap b = new Bitmap(path))
            { return b.Width > b.Height; }
        }

        /* Clear picBxs before populating them again */
        void Reset()
        {
            picBx_Left.Width = iWidth;
            picBx_Left.ImageLocation = null;
            picBx_Right.ImageLocation = null;
        }

        /* Alternative closing method */
        private void UserClick(object sender, MouseEventArgs e)
        { Close(); }

        /* Re-enable cursor when finished browsing */
        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        { Cursor.Show(); }
    }
}
