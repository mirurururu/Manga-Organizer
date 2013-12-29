using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    public partial class Suggest : Form
    {
        delegate void DelListStringVoid(List<ExtString.stEXH> lsPass);
        DelListStringVoid delResults = null;
        public string sChoice = "";

        public Suggest()
        { InitializeComponent(); }
        
        private void Suggest_Load(object sender, EventArgs e)
        {
            delResults = DisplayResults;
            txbxSearch.Select();
            ResizeLV();

            //get system icon for help button
            tsbtnHelp.Image = System.Drawing.SystemIcons.Information.ToBitmap();
            
            //input user credentials
            txbxPass.Text = Properties.Settings.Default.pass_hash;
            txbxID.Text = Properties.Settings.Default.member_id;
        }
        
        private void tsbtn_Help_Clicked(object sender, EventArgs e)
        {
            MessageBox.Show("These credentials are the 'content' parameters of the exhentai cookies. If not provided, this program will be unable to establish a connection.", 
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txbxID.Text) || string.IsNullOrEmpty(txbxPass.Text)) {
                MessageBox.Show("Please provide credentials for connecting.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnSearch.Enabled = false;
            ThreadPool.QueueUserWorkItem(Search, txbxSearch.Text);
        }

        private void Search(object obj)
        {
            if(!(obj is string)) return;

            List<ExtString.stEXH> lresp = ExtString.EHSearch((string)obj);
            this.BeginInvoke(delResults, lresp);
        }
        
        private void DisplayResults(List<ExtString.stEXH> lResults)
        {
            if(lResults != null) {
                lvDetails.SuspendLayout();
                btnSearch.Enabled = true;

                for (int i = 0; i < lResults.Count; i++) {
                    lvDetails.Items.Add(new ListViewItem(new string[2] {
                        lResults[i].sURL,
                        lResults[i].sTitle
                    }));
                }

                lvDetails.ResumeLayout(); 
            }
            else {
                MessageBox.Show("The connection failed.",
                    Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(lvDetails.SelectedItems.Count > 0) {
                sChoice = lvDetails.SelectedItems[0].SubItems[0].Text;
                this.DialogResult = DialogResult.OK;
            }

            this.Close();
        }

        #region listview methods
        private void lvDetails_Resize(object sender, EventArgs e)
        { ResizeLV(); }

        private void ResizeLV()
        {
            const double dRow = 17.5; //approx height of each row
            const int iScroll = 20;   //vertical scrollbar width

            lvDetails.BeginUpdate();
            colTitle.Width = lvDetails.DisplayRectangle.Width - colURL.Width;

            /* account for scrollbar width */
            if (lvDetails.Items.Count > lvDetails.Height / dRow)
                colTitle.Width -= iScroll;
            lvDetails.EndUpdate();
        }

        //open selected url in browser
        private void lvDetails_DoubleClick(object sender, EventArgs e)
        {
            if(lvDetails.SelectedItems.Count > 0) {
                System.Diagnostics.Process.Start(
                    lvDetails.SelectedItems[0].SubItems[0].Text);
            }
        }
        #endregion

        #region credential handling
        private void tsbtnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("", Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
        private void txbxPass_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pass_hash = txbxPass.Text;
            Properties.Settings.Default.Save();
        }

        private void txbxID_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.member_id = txbxID.Text;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
