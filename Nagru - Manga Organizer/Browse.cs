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
        int iPage = 0;

        /* Initialize form */
        public Browse()
        { InitializeComponent(); }

        /* Set form to fullscreen and grab files */
        private void Browse_Load(object sender, EventArgs e)
        {
            //set fullscreen
            TopMost = true;
            Location = new Point(0, 0);
            Bounds = Screen.PrimaryScreen.Bounds;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //set picBx positions
            int iWidth = Screen.PrimaryScreen.Bounds.Width / 2;
            picBx_Left.Location = new Point(0, 0);
            picBx_Left.Width = iWidth;
            picBx_Right.Location = new Point(iWidth, 0);
            picBx_Right.Width = iWidth;

            //get files
            lFiles = ExtDirectory.GetFiles(sPath, "*.jpg|*.png|*.bmp|*.jpeg",
                SearchOption.TopDirectoryOnly);

            lFiles.Sort(new TrueCompare());
            SetImage();
        }

        /* Navigate files or close form */
        private void Browse_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Up:
                    iPage -= 3;
                    SetImage();
                    break;
                case Keys.Left:
                case Keys.Down:
                    iPage++;
                    SetImage();
                    break;
                case Keys.Escape:
                case Keys.F11:
                case Keys.RWin:
                case Keys.LWin:
                    Close();
                    break;
            }
        }

        /* Replace current page */
        private void SetImage()
        {
            Reset();
            if (iPage < 0) iPage = lFiles.Count - 2;
            else if (iPage >= lFiles.Count) iPage = 0;

            Image imgOne = Image.FromStream(new FileStream(lFiles[iPage++], FileMode.Open, FileAccess.Read));

            if (imgOne.Width < imgOne.Height)
            {
                picBx_Right.Image = imgOne;

                if (iPage < 0) iPage = lFiles.Count - 1;
                else if (iPage >= lFiles.Count) iPage = 1;
                Image imgTwo = Image.FromStream(new FileStream(lFiles[iPage], FileMode.Open, FileAccess.Read));

                if (imgTwo.Width < imgTwo.Height)
                    picBx_Left.Image = imgTwo;
            }
            else
            {
                picBx_Left.Width = Screen.PrimaryScreen.Bounds.Width;
                picBx_Left.Image = imgOne;
            }
        }

        /* Clear picBxs before populating them again */
        private void Reset()
        {
            if (picBx_Left.Image != null)
            {
                picBx_Left.Image.Dispose();
                picBx_Left.Image = null;
            }
            if (picBx_Right.Image != null)
            {
                picBx_Right.Image.Dispose();
                picBx_Right.Image = null;
            }

            if (picBx_Left.Width == Screen.PrimaryScreen.Bounds.Width)
                picBx_Left.Width = Screen.PrimaryScreen.Bounds.Width / 2;
        }

        /* Alternative closing method */
        private void UserClick(object sender, MouseEventArgs e)
        {Close();}
    }
}
