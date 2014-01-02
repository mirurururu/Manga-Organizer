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
        LVsorter lvSortObj = new LVsorter();
        public string sChoice = "";

        public Suggest()
        { InitializeComponent(); }
        
        private void Suggest_Load(object sender, EventArgs e)
        {
            lvDetails.ListViewItemSorter = lvSortObj;
            delResults = DisplayResults;
            txbxSearch.Select();
            ResizeLV();

            ToggleButtonEnabled(btnOK, false);

            //remove active border on form acceptbutton
            this.AcceptButton.NotifyDefault(false);

            //set-up user choices
            if (Properties.Settings.Default.DefGrid)
                lvDetails.GridLines = true;
            else lvDetails.GridLines = false;

            //get system icon for help button
            tsbtnHelp.Image = SystemIcons.Information.ToBitmap();
            
            //input user credentials
            txbxPass.Text = Properties.Settings.Default.pass_hash;
            txbxID.Text = Properties.Settings.Default.member_id;
        }
        
        private void tsbtn_Help_Clicked(object sender, EventArgs e)
        {
            MessageBox.Show("These credentials are the 'content' parameters of the exhentai cookies. "
            + "If not provided, this program will use g.e-hentai instead.", 
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ToggleButtonEnabled(btnSearch);
            ThreadPool.QueueUserWorkItem(Search, txbxSearch.Text);
            this.Cursor = Cursors.WaitCursor;
        }

        private void Search(object obj)
        {
            if(!(obj is string)) return;

            List<ExtString.stEXH> lresp = ExtString.EHSearch((string)obj);
            this.Invoke(delResults, lresp);
        }
        
        private void DisplayResults(List<ExtString.stEXH> lResults)
        {
            if(lResults != null) {
                lvDetails.SuspendLayout();
                lvDetails.Items.Clear();

                for (int i = 0; i < lResults.Count; i++) {
                    lvDetails.Items.Add(new ListViewItem(new string[2] {
                        lResults[i].sURL,
                        lResults[i].sTitle
                    }));
                }

                Alternate();
                lvDetails.ResumeLayout(); 
            }
            else {
                MessageBox.Show("A connection to EH could not be established.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
            this.Cursor = Cursors.Default;
            ToggleButtonEnabled(btnSearch);
        }
        
        private void lvDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            //en/disable button depending on whether an item is selected
            ToggleButtonEnabled(btnOK,
                lvDetails.SelectedItems.Count > 0);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(lvDetails.SelectedItems.Count > 0) {
                sChoice = lvDetails.SelectedItems[0].SubItems[0].Text;
                this.DialogResult = DialogResult.OK;
            }

            this.Close();
        }

        private void ToggleButtonEnabled(Button bRef, bool? bEnabled = null)
        {
            bRef.Enabled = (bEnabled != null) ? (bool)bEnabled : !bRef.Enabled;
            if (bRef.Enabled) bRef.BackColor = SystemColors.ButtonFace;
            else bRef.BackColor = SystemColors.ScrollBar;
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
        
        private void lvDetails_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != lvSortObj.ColToSort)
                lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
            else lvSortObj.SwapOrder();

            lvDetails.Sort();
            Alternate();
        }

        //open selected url in browser
        private void lvDetails_DoubleClick(object sender, EventArgs e)
        {
            if(lvDetails.SelectedItems.Count > 0) {
                System.Diagnostics.Process.Start(
                    lvDetails.SelectedItems[0].SubItems[0].Text);
            }
        }

        private void Alternate(int iStart = 0)
        {
            if (Properties.Settings.Default.DefGrid) return;
            for (int i = iStart; i < lvDetails.Items.Count; i++) {
                lvDetails.Items[i].BackColor = (i % 2 != 0) ? 
                    Color.FromArgb(245, 245, 245) : SystemColors.Window;
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
