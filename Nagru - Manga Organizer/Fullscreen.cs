using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Nagru___Manga_Organizer
{
    public partial class Fullscreen : Form
    {
        public string sPath { get; set; }
        List<string> lFiles = new List<string>(25);
        FileStream fs;
        int iPage = 0;

        /* Initialize form */
        public Fullscreen()
        { InitializeComponent(); }

        /* Set form to fullscreen and grab files */
        private void Fullscreen_Load(object sender, EventArgs e)
        {
            //set fullscreen
            TopMost = true;
            Location = new Point(0, 0);
            Bounds = Screen.PrimaryScreen.Bounds;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //get files
            lFiles = ExtDirectory.GetFiles(sPath, "*.jpg|*.png|*.bmp|*.jpeg",
                SearchOption.TopDirectoryOnly);

            lFiles.Sort(new TrueCompare());
            SetImage();
        }

        /* Navigate files or close form */
        private void Fullscreen_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Up:
                    iPage++;
                    PicBx_Entry.Image.Dispose();
                    SetImage();
                    break;
                case Keys.Left: 
                case Keys.Down:
                    iPage--;
                    PicBx_Entry.Image.Dispose();
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
            if (iPage < 0) iPage = lFiles.Count - 1;
            else if (iPage >= lFiles.Count) iPage = 0;
            
            fs = new FileStream(lFiles[iPage], FileMode.Open, FileAccess.Read);
            PicBx_Entry.Image = Image.FromStream(fs);
            fs.Close();
        }

        /* Alternative closing method */
        private void PicBx_Entry_MouseUp(object sender, MouseEventArgs e)
        { Close(); }

        /* Ensure disposal of resources */
        private void Fullscreen_FormClosing(object sender, FormClosingEventArgs e)
        { fs.Dispose(); }
    }
}
