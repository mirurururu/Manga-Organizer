using System;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    public partial class Tutorial : Form
    {
        byte byPanel = 0;

        public Tutorial()
        { 
            InitializeComponent(); 
            this.Icon = Properties.Resources.dbIcon;
        }

        private void Tutorial_Shown(object sender, EventArgs e)
        { SwitchPanel(); }

        private void SwitchPanel()
        {
            this.SuspendLayout();
            switch (byPanel) {
                case 0: btnPrev.Enabled = false;
                    picBx_Ex.Image = Properties.Resources.AddManga;
                    break;
                case 1: btnPrev.Enabled = true;
                    picBx_Ex.Image = Properties.Resources.Tags;
                    break;
                case 2:
                    picBx_Ex.Image = Properties.Resources.Search;
                    break;
                case 3: btnNxt.Text = "Next >>";
                    picBx_Ex.Image = Properties.Resources.Browse;
                    break;
                case 4: btnNxt.Text = "Finish";
                    picBx_Ex.Image = Properties.Resources.Update;
                    break;
                case 5: this.Close();
                    return;
            }
            Controls["pnl" + byPanel].BringToFront();
            progBar.Step = byPanel +1;
            this.ResumeLayout();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            byPanel--;
            SwitchPanel();
        }

        private void btnNxt_Click(object sender, EventArgs e)
        {
            byPanel++;
            SwitchPanel();
            btnPrev.Enabled = true;
        }

        private void progBar_Click(object sender, EventArgs e)
        {
            byPanel = (byte)progBar.Step;
            
            if (btnPrev.Enabled && byPanel == 0)
                btnPrev.Enabled = false;
            else btnPrev.Enabled = true;
            if(byPanel == 4 && btnNxt.Text != "Finish")
                btnNxt.Text = "Finish";
            else btnNxt.Text = "Next >>";
            
            SwitchPanel();
        }
    }
}