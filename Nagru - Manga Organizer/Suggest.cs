using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class Suggest : Form
    {
        public Main.stEntry enData { private set; get; }
        bool bNewCreds = false;

        public Suggest(string sTitle, string sArtist = "")
        {
            InitializeComponent();

            txbxPass.Text = Properties.Settings.Default.pass_hash;
            txbxID.Text = Properties.Settings.Default.member_id;
        }

        private void txbxPass_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pass_hash = txbxPass.Text;
            bNewCreds = true;
        }

        private void txbxID_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.member_id = txbxID.Text;
            bNewCreds = true;
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Suggest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(bNewCreds) {
                Properties.Settings.Default.Save();
            }
        }
    }
}
