using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
    public partial class Scan : Form
    {
        private delegate void DelVoidEntry(Main.stEntry en);
        public delegate void DelVoidVoid();
        public DelVoidVoid delNewEntry;
        public DelVoidVoid delDone;

        public List<Main.stEntry> lCurr { set; get; }

        LVsorter lvSortObj = new LVsorter();
        HashSet<string> hsPaths = new HashSet<string>();
        HashSet<string> hsIgnore = new HashSet<string>();
        List<Main.stEntry> lFound = new List<Main.stEntry>();

        #region ScanOperation
        /* Load 'Scan' Form   */
        public Scan()
        { InitializeComponent(); }

        /* Initialize Scan Form */
        private void Scan_Load(object sender, EventArgs e)
        {
            //set-up hashset for ignored files
            string[] sRaw = Properties.Settings.Default.Ignore.Split('|');
            for (int i = 0; i < sRaw.Length - 1; i++)
                hsIgnore.Add(sRaw[i]);

            //set-up hashset for path list
            for (int i = 0; i < lCurr.Count; i++)
                hsPaths.Add(lCurr[i].sLoc);

            //bind LV_Found to sorter & set column size
            LV_Found.ListViewItemSorter = lvSortObj;
            LV_Found_Resize(sender, e);

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc;
            if (sPath == string.Empty || !Directory.Exists(sPath))
                sPath = Environment.CurrentDirectory;
            TxBx_Loc.Text = sPath;
            TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
            TryScan();
        }

        /* Choose new folder path */
        private void TxBx_Loc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.Description = "Select the directory you want to scan.";

            //initialize selected folder
            if (TxBx_Loc.Text == string.Empty || !Directory.Exists(TxBx_Loc.Text))
                fbd.SelectedPath = Environment.CurrentDirectory;
            else fbd.SelectedPath = TxBx_Loc.Text;

            //Set new folder path
            if (fbd.ShowDialog() == DialogResult.OK) {
                TxBx_Loc.Text = fbd.SelectedPath;
                TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
                TryScan();
            }
            fbd.Dispose();
        }

        /* Start directory scan in new thread */
        private void TryScan()
        {
            if (!ExtDirectory.Restricted(TxBx_Loc.Text)) {
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
            if (asDirs.Length == 0) {
                BeginInvoke(new DelVoidVoid(SetFoundItems));
                return;
            }

            //lCurr.Contains
            for (int i = 0; i < asDirs.Length; i++) {
                if (hsPaths.Contains(asDirs[i])) continue;

                string[] sTitle = Path.GetFileName(asDirs[i]).Split(
                    new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                Main.stEntry en = new Main.stEntry((sTitle.Length == 2) ? sTitle[1].TrimStart() :
                    sTitle[0].TrimStart(), (sTitle.Length == 2) ? sTitle[0] : "", asDirs[i], "",
                    "Manga", "", DateTime.Now, ExtDirectory.GetFiles(asDirs[i]).Length, false);

                lFound.Add(en);
                BeginInvoke(new DelVoidEntry(AddItem), en);
            }
            BeginInvoke(new DelVoidVoid(SetFoundItems));
        }

        /* Add new item to listview */
        private void AddItem(Main.stEntry en)
        {
            ListViewItem lvi = new ListViewItem(en.sArtist);
            lvi.SubItems.Add(en.sTitle);
            lvi.SubItems.Add(en.iPages.ToString());

            if (hsIgnore.Contains(lvi.SubItems[0].Text + lvi.SubItems[1].Text)) {
                if (ChkBx_All.Checked) {
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
            for (int i = 0; i < lFound.Count; i++) {
                ListViewItem lvi = new ListViewItem(lFound[i].sArtist);
                lvi.SubItems.Add(lFound[i].sTitle);
                lvi.SubItems.Add(lFound[i].iPages.ToString());

                if (hsIgnore.Contains(lvi.SubItems[0].Text + lvi.SubItems[1].Text)) {
                    if (ChkBx_All.Checked) {
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
            if (e.Column != lvSortObj.ColToSort) {
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

        /* Give listview priority whenever mouse hovers in it */
        private void LV_Found_MouseHover(object sender, EventArgs e)
        {
            if (!LV_Found.Focused)
                LV_Found.Focus();
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
                if (sMatch == lFound[i].sArtist + lFound[i].sTitle) {
                    indx = (short)i;
                    break;
                }

            if (indx < 0 || !Directory.Exists(lFound[indx].sLoc)) {
                MessageBox.Show("Could not open location.", "Manga Organizer",
                  MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            System.Diagnostics.Process.Start(lFound[indx].sLoc);
        }

        /* Sends selected items back to Main Form */
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            if (LV_Found.SelectedItems.Count == 0) return;

            for (int i = 0; i < LV_Found.SelectedItems.Count; i++) {
                string sMatch = LV_Found.SelectedItems[i].SubItems[0].Text
                    + LV_Found.SelectedItems[i].SubItems[1].Text;

                for (int x = 0; x < lFound.Count; x++)
                    if (sMatch == lFound[x].sArtist + lFound[x].sTitle) {
                        lCurr.Add(lFound[x]);
                        break;
                    }
            }

            delNewEntry.Invoke();
        }

        /* Unselect item(s) */
        private void Scan_Click(object sender, EventArgs e)
        { LV_Found.SelectedItems.Clear(); }

        /* Prevent multiple instances */
        private void Scan_FormClosing(object sender, FormClosingEventArgs e)
        {
            //preserve ignored items
            string sNew = "";
            foreach (string svar in hsIgnore)
                if (svar != string.Empty) sNew += svar + '|';

            Properties.Settings.Default.Ignore = sNew;
            Properties.Settings.Default.Save();

            //clear old data
            hsIgnore.Clear();
            hsPaths.Clear();
            lFound.Clear();

            //update main form
            delDone.Invoke();
        }

        /* Add or remove item from ignored list based on context */
        private void Btn_Ignore_Click(object sender, EventArgs e)
        {
            if (LV_Found.SelectedItems.Count == 0) return;

            LV_Found.BeginUpdate();
            for (int i = 0; i < LV_Found.SelectedItems.Count; i++) {
                string sItem = LV_Found.SelectedItems[i].SubItems[0].Text +
                        LV_Found.SelectedItems[i].SubItems[1].Text;

                if (hsIgnore.Contains(sItem)) {
                    hsIgnore.Remove(sItem);
                    LV_Found.SelectedItems[i].BackColor = System.Drawing.SystemColors.Window;
                }
                else {
                    hsIgnore.Add(sItem);
                    if (!ChkBx_All.Checked) LV_Found.SelectedItems[i--].Remove();
                    else LV_Found.SelectedItems[i].BackColor = System.Drawing.Color.MistyRose;
                }
            }
            LV_Found.EndUpdate();
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
