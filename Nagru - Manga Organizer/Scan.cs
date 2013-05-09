using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Permissions;

namespace Nagru___Manga_Organizer
{
    public partial class Scan : Form
    {
        private delegate void DelVoidEntry(Main.stEntry en);
        public delegate void DelVoidVoid();
        public DelVoidVoid delNewEntry;
        public DelVoidVoid delDone;

        public List<Main.stEntry> CurrentItems
        { set { lCurr = value; } }

        LVsorter lvSortObj = new LVsorter();
        List<Main.stEntry> lFound = new List<Main.stEntry>();
        List<Main.stEntry> lCurr = new List<Main.stEntry>();
        List<string> lIgnored = new List<string>();


        #region ScanOperation
        /* Load 'Scan' Form   */
        public Scan()
        { InitializeComponent(); }

        /* Initialize Scan Form */
        private void Scan_Load(object sender, EventArgs e)
        {
            //grab ignored files
            foreach (string sItem in Properties.Settings.Default.Ignore.Split('|'))
                if (sItem != string.Empty) lIgnored.Add(sItem);

            //bind LV_Found to sorter & set column size
            LV_Found.ListViewItemSorter = lvSortObj;
            LV_Found_Resize(sender, e);

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc;
            if (sPath == string.Empty || !Directory.Exists(sPath))
                sPath = Environment.CurrentDirectory;
            TxBx_Loc.Text = sPath;
            TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
        }

        /* Choose new folder path */
        private void TxBx_Loc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.Description = "Select the directory you want to scan.";

            //initialize selected folder
            if (TxBx_Loc.Text == string.Empty || !Directory.Exists(TxBx_Loc.Text))
                fbd.SelectedPath = Environment.CurrentDirectory;
            fbd.SelectedPath = TxBx_Loc.Text;

            //Set new folder path
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TxBx_Loc.Text = fbd.SelectedPath;
                TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
                TryScan();
            }
            fbd.Dispose();
        }

        /* Start scan manually */
        private void Btn_Scan_Click(object sender, EventArgs e)
        { TryScan(); }

        /* Start directory scan in new thread */
        private void TryScan()
        {
            if (!ExtDirectory.Restricted(TxBx_Loc.Text))
            {
                lFound.Clear();
                LV_Found.Items.Clear();
                Cursor = Cursors.WaitCursor;
                System.Threading.ThreadPool.QueueUserWorkItem(ScanDir);
            }
            else MessageBox.Show("Cannot read from the selected folder path.",
                "Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* Scan default directory for entries */
        private void ScanDir(Object obj)
        {
            string[] asDirs = Directory.GetDirectories(
                TxBx_Loc.Text, "*", SearchOption.TopDirectoryOnly);

            if (asDirs.Length > 0)
                for (int i = 0; i < asDirs.Length; i++)
                {
                    //check if filepath already in database
                    bool bExists = false;
                    for (int x = 0; x < lCurr.Count; x++)
                        if (asDirs[i].Equals(lCurr[x].sLoc,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            bExists = true;
                            break;
                        }

                    //if not, add to potential 'found' items
                    if (!bExists)
                    {
                        string[] sTitle = Path.GetFileName(asDirs[i]).Split(
                            new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                        Main.stEntry en;
                        if (sTitle.Length == 2)
                            en = new Main.stEntry(sTitle[1].TrimStart(), sTitle[0], asDirs[i], "", "Manga", "",
                                DateTime.Now, ExtDirectory.GetFiles(asDirs[i]).Length, false);
                        else
                            en = new Main.stEntry(sTitle[0].TrimStart(), "", asDirs[i], "", "Manga", "",
                                DateTime.Now, ExtDirectory.GetFiles(asDirs[i]).Length, false);

                        lFound.Add(en);
                        BeginInvoke(new DelVoidEntry(AddItem), en);
                    }
                }

            //finish form operations of scanning
            BeginInvoke(new DelVoidVoid(SetFoundItems));
        }

        /* Add new item to listview */
        private void AddItem(Main.stEntry en)
        {
            ListViewItem lvi = new ListViewItem(en.sArtist);
            lvi.SubItems.Add(en.sTitle);
            lvi.SubItems.Add(en.iPages.ToString());

            if (lIgnored.Contains(lvi.SubItems[0].Text + lvi.SubItems[1].Text))
            {
                if (ChkBx_All.Checked)
                {
                    lvi.BackColor = System.Drawing.Color.MistyRose;
                    LV_Found.Items.Add(lvi);
                }
            }
            else LV_Found.Items.Add(lvi);
        }

        /* Finish sorting display */
        private void SetFoundItems()
        {
            Cursor = Cursors.Default;
            Text = "Found " + LV_Found.Items.Count + " possible entries";
            LV_Found.Sort();
        }
        #endregion

        #region DisplayMethods
        /*Update listview with all 'missing' entries */
        private void UpdateLV()
        {
            Cursor = Cursors.WaitCursor;
            List<ListViewItem> lItems = new List<ListViewItem>(LV_Found.Items.Count + 1);

            //refresh LV_Entries
            for (int i = 0; i < lFound.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(lFound[i].sArtist);
                lvi.SubItems.Add(lFound[i].sTitle);
                lvi.SubItems.Add(lFound[i].iPages.ToString());

                if (lIgnored.Contains(lvi.SubItems[0].Text + lvi.SubItems[1].Text))
                {
                    if (ChkBx_All.Checked)
                    {
                        lvi.BackColor = System.Drawing.Color.MistyRose;
                        lItems.Add(lvi);
                    }
                }
                else lItems.Add(lvi);
            }

            //Update listview display
            LV_Found.BeginUpdate();
            LV_Found.Items.Clear();
            LV_Found.Items.AddRange(lItems.ToArray());
            LV_Found.Sort();
            LV_Found.EndUpdate();

            LV_Found.Select();
            Cursor = Cursors.Default;
            Text = "Found " + LV_Found.Items.Count + " possible entries";
        }

        /* Sort entries based on clicked column */
        private void LV_Found_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != lvSortObj.ColToSort)
            {
                lvSortObj.ColToSort = e.Column;
                lvSortObj.OrdOfSort = SortOrder.Ascending;
            }
            else lvSortObj.SwapOrder(); //reverse sort order

            LV_Found.Sort();
        }

        /* Auto-resizes Col_Title to match fmScan width   */
        private void LV_Found_Resize(object sender, EventArgs e)
        {
            LV_Found.BeginUpdate();
            int iColWidth = 0;
            for (int i = 0; i < LV_Found.Columns.Count; i++)
                iColWidth += LV_Found.Columns[i].Width;

            ColTitle.Width += LV_Found.DisplayRectangle.Width - iColWidth;
            LV_Found.EndUpdate();
        }

        /* Change whether LV shows ignored items or not */
        private void ChkBx_All_CheckedChanged(object sender, EventArgs e)
        { UpdateLV(); }
        #endregion

        #region ManipulateItems
        /*Open folder of double-clicked item */
        private void LV_Found_DoubleClick(object sender, EventArgs e)
        {
            short indx = -1;
            string sMatch = LV_Found.FocusedItem.SubItems[0].Text +
                LV_Found.FocusedItem.SubItems[1].Text;

            for (int i = 0; i < lFound.Count; i++)
                if (sMatch == lFound[i].sArtist + lFound[i].sTitle)
                {
                    indx = (short)i;
                    break;
                }

            if (indx < 0 || !Directory.Exists(lFound[indx].sLoc))
            {
                MessageBox.Show("Could not open location.", "Manga Organizer",
                  MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            System.Diagnostics.Process.Start(lFound[indx].sLoc);
        }

        /* Sends selected items back to Main Form */
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            if (LV_Found.SelectedItems.Count == 0)
                for (int i = 0; i < LV_Found.Items.Count; i++)
                    LV_Found.Items[i].Selected = true;

            for (int i = 0; i < LV_Found.SelectedItems.Count; i++)
            {
                string sMatch = LV_Found.SelectedItems[i].SubItems[0].Text
                    + LV_Found.SelectedItems[i].SubItems[1].Text;

                for (int x = 0; x < lFound.Count; x++)
                    if (sMatch == lFound[x].sArtist + lFound[x].sTitle)
                    {
                        lCurr.Add(lFound[x]);
                        break;
                    }
            }

            delNewEntry.Invoke();
            this.Close();
        }

        /* Unselect item(s) */
        private void Scan_Click(object sender, EventArgs e)
        { LV_Found.SelectedItems.Clear(); }

        /* Prevent multiple instances */
        private void Scan_FormClosing(object sender, FormClosingEventArgs e)
        {
            //preserve ignored items
            string sNewIgn = "";
            for (int i = 0; i < lIgnored.Count; i++)
                if (lIgnored[i] != string.Empty) sNewIgn += lIgnored[i] + '|';
            Properties.Settings.Default.Ignore = sNewIgn;
            Properties.Settings.Default.Save();

            //clear old data
            lIgnored.Clear();
            lFound.Clear();

            //update main form
            delDone.Invoke();
        }

        /* Add or remove item from ignored list based on context */
        private void Btn_Ignore_Click(object sender, EventArgs e)
        {
            if (LV_Found.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select at least one item.", "Manga Organizer",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (ChkBx_All.Checked)
                for (int i = 0; i < LV_Found.SelectedItems.Count; i++)
                {
                    string sItem = LV_Found.SelectedItems[i].SubItems[0].Text +
                            LV_Found.SelectedItems[i].SubItems[1].Text;

                    if (lIgnored.Contains(sItem))
                        for (int x = 0; x < lIgnored.Count; x++)
                        {
                            if (lIgnored[x] == sItem)
                            {
                                lIgnored.RemoveAt(x);
                                break;
                            }
                        }
                    else lIgnored.Add(sItem);
                }
            else for (int i = 0; i < LV_Found.SelectedItems.Count; i++)
                    lIgnored.Add(LV_Found.SelectedItems[i].SubItems[0].Text +
                            LV_Found.SelectedItems[i].SubItems[1].Text);

            UpdateLV();
        }
        #endregion

        #region Menu_Text
        private void MnTx_Undo_Click(object sender, EventArgs e)
        { if (TxBx_Loc.CanUndo) TxBx_Loc.Undo(); }

        private void MnTx_Cut_Click(object sender, EventArgs e)
        { TxBx_Loc.Cut(); }

        private void MnTx_Copy_Click(object sender, EventArgs e)
        { TxBx_Loc.Copy(); }

        private void MnTx_Paste_Click(object sender, EventArgs e)
        { TxBx_Loc.Paste(); }

        private void MnTx_SelAll_Click(object sender, EventArgs e)
        { TxBx_Loc.SelectAll(); }
        #endregion
    }
}
