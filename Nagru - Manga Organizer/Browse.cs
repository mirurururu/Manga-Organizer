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
        Image imgR = null, imgL = null;
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
            picBx_Left.Width = iWidth;
            picBx_Right.Location = new Point(iWidth, 0);
            picBx_Right.Width = iWidth;

            //get files
            lFiles = ExtDirectory.GetFiles(sPath, "*.jpg|*.png|*.bmp|*.jpeg",
                SearchOption.TopDirectoryOnly);

            lFiles.Sort(new TrueCompare());
            GoLeft();
        }

        /* Navigate files or close form */
        private void Browse_KeyDown(object sender, KeyEventArgs e)
        {
            this.SuspendLayout();
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
                default:
                    Close();
                    break;
            }
            this.ResumeLayout();
        }

        /* Traverse images leftwards */
        private void GoLeft()
        {
            Reset();

            if (++iPage >= lFiles.Count) iPage = 0;
            imgR = Image.FromStream(new FileStream(lFiles[iPage],
                FileMode.Open, FileAccess.Read));
            if (++iPage >= lFiles.Count) iPage = 0;
            imgL = Image.FromStream(new FileStream(lFiles[iPage],
                FileMode.Open, FileAccess.Read));

            bool bWideR = (imgR.Height < imgR.Width);
            bool bWideL = (imgL.Height < imgL.Width);

            if (!bWideL && !bWideR)
            {
                picBx_Left.Image = imgL;
                picBx_Right.Image = imgR;
            }
            else if (bWideL && bWideR || bWideR)
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

        /* Traverse images rightward */
        private void GoRight()
        {
            if (picBx_Left.Width == iWidth) iPage--;
            Reset();

            if (--iPage < 0) iPage = lFiles.Count - 1;
            imgL = Image.FromStream(new FileStream(lFiles[iPage],
                FileMode.Open, FileAccess.Read));
            if (iPage - 1 < 0) iPage = lFiles.Count;
            imgR = Image.FromStream(new FileStream(lFiles[iPage - 1],
                FileMode.Open, FileAccess.Read));

            bool bWideR = (imgR.Height < imgR.Width);
            bool bWideL = (imgL.Height < imgL.Width);
            picBx_Left.Image = imgL;

            if (!bWideL && !bWideR)
                picBx_Right.Image = imgR;
            else if (bWideL && bWideR || bWideL)
                picBx_Left.Width =
                    Screen.PrimaryScreen.Bounds.Width;
        }

        /* Clear picBxs before populating them again */
        private void Reset()
        {
            if (picBx_Left.Image != null)
                picBx_Left.Image.Dispose();
            if (picBx_Right.Image != null)
                picBx_Right.Image.Dispose();
            if (picBx_Left.Width != iWidth)
                picBx_Left.Width = iWidth;
        }

        /* Alternative closing method */
        private void UserClick(object sender, MouseEventArgs e)
        { Close(); }

        /* Re-enable cursor when finished browsing */
        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        { Cursor.Show(); }
    }
}
