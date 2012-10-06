/*
 * Author: Taylor Napier
 * Date: Aug15, 2012
 * Desc: Handles organization of manga library
 */

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nagru___Manga_Organizer
{
    public partial class Main : Form
    {
        delegate void DelVoidVoid();
        delegate void DelVoidInt(int iNum);
        delegate void DelVoidImage(System.Drawing.Image img);

        LVSorter lvSortObj = new LVSorter();
        List<stEntry> lData = new List<stEntry>();
        bool bSavList = true, bSavText = true;
        short indx = -1;

        /* Holds metadata about manga in library    */
        [Serializable]
        public struct stEntry : ISerializable
        {
            public string sTitle, sArtist,
                sLoc, sType, sDesc, sTags;
            public DateTime dtDate;
            public ushort iPages;
            public bool bFav;

            public string GetName //Returns formatted title of Entry
            {
                get
                {
                    if (sArtist != string.Empty)
                        return string.Format("[{0}] {1}", sArtist, sTitle);
                    else return sTitle;
                }
            }

            /* Initialize manga entry */
            public stEntry(string Title, string Artist, string Location, string Description,
                string Tags, string Type, DateTime Date, decimal Pages, bool Fav)
            {
                sTitle = Title;
                sArtist = Artist;
                sLoc = Location;
                sDesc = Description;
                dtDate = Date;
                iPages = (ushort)Pages;
                sType = Type;
                bFav = Fav;

                //Convert tags into string array
                sTags = "";
                List<string> lCheck = new List<string>(20);
                foreach (string s in Tags.Split(','))
                    if (s != string.Empty && !lCheck.Contains(s.Trim()))
                        lCheck.Add(s.Trim());

                lCheck.Sort();
                for (int i = 0; i < lCheck.Count; i++)
                {
                    if (i != 0) sTags += ", ";
                    sTags += lCheck[i];
                }
            }

            /* Override equals to compare entry titles */
            public override bool Equals(object obj)
            {
                if (!(obj is stEntry)) return false;
                stEntry en = (stEntry)obj;
                return ((en.sArtist + en.sTitle).Equals(sArtist
                    + sTitle, StringComparison.OrdinalIgnoreCase));
            }

            /* 'Disable' hashtable */
            public override int GetHashCode() { return 1; }

            /* custom serialization to save datatypes manually */
            private stEntry(SerializationInfo info, StreamingContext ctxt)
            {
                sTitle = info.GetString("Title");
                sArtist = info.GetString("Artist");
                sLoc = info.GetString("Loc");
                sDesc = info.GetString("Desc");
                dtDate = info.GetDateTime("Date");
                sType = info.GetString("Type");
                bFav = info.GetBoolean("Fav");
                sTags = info.GetString("Tags");
                iPages = (ushort)info.GetInt16("Pages");
            }

            /* custom serialization to read datatypes manually */
            [System.Security.Permissions.SecurityPermission(System.Security.Permissions.
                SecurityAction.LinkDemand, Flags = System.Security.Permissions.
                SecurityPermissionFlag.SerializationFormatter)]
            public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
            {
                info.AddValue("Title", sTitle);
                info.AddValue("Artist", sArtist);
                info.AddValue("Loc", sLoc);
                info.AddValue("Desc", sDesc);
                info.AddValue("Date", dtDate);
                info.AddValue("Pages", iPages);
                info.AddValue("Type", sType);
                info.AddValue("Fav", bFav);
                info.AddValue("Tags", sTags);
            }
        }

        #region FormMethods
        /* Load 'Main' Form   */
        public Main()
        { InitializeComponent(); }

        /* Load database   */
        private void Main_Load(object sender, EventArgs e)
        {
            LV_Entries.Select();
            LV_Entries.ListViewItemSorter = lvSortObj;
            frTxBx_Notes.Text = Properties.Settings.Default.Notes;
            Nud_Pages.ContextMenuStrip = new ContextMenuStrip();
            LV_Entries_Resize(sender, e);

            //set up frTxBx's to allow dragdrop
            frTxBx_Notes.AllowDrop = true;
            frTxBx_Desc.AllowDrop = true;
            frTxBx_Notes.DragDrop += new DragEventHandler(DragDropTxBx);
            frTxBx_Desc.DragDrop += new DragEventHandler(DragDropTxBx);

            //prevents visible form re-draw when user first uses TxBx_Search
            Btn_Clear.Visible = true;
            Btn_Clear.Visible = false;

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc;
            if (sPath != string.Empty) sPath += "\\MangaDatabase.bin";
            else sPath = Environment.CurrentDirectory + "\\MangaDatabase.bin";
            if (!File.Exists(sPath)) //if none exists, exit
            { return; }

            //load database
            Cursor = Cursors.WaitCursor;
            lData = FileSerializer.Deserialize<List<stEntry>>(sPath);
            Cursor = Cursors.Default;
            UpdateLV();
        }

        /* Prevent Form close if unsaved data present   */
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bSavList || !bSavText)
            {
                switch (MessageBox.Show("Save before exiting?", "Manga Organizer",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        Visible = false;
                        SaveData();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /* Display image from selected folder   */
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TabControl.SelectedIndex)
            {
                case 0:
                    //Return LV focus and update Form title
                    if (TxBx_Search.Text == string.Empty) Text = "Manga Organizer: " + lData.Count + " entries";
                    else Text = "Returned: " + LV_Entries.Items.Count + " entries";
                    this.AcceptButton = Btn_Clear;
                    LV_Entries.Focus();
                    break;
                case 1:
                    if (indx > -1)
                    {
                        this.AcceptButton = null;

                        //Update Form title
                        Text = "Selected: " + lData[indx].GetName;
                        MnTS_Del.Visible = true;
                        MnTS_Open.Visible = true;
                    }
                    break;
                default:
                    this.AcceptButton = null;
                    break;
            }
        }
        #endregion

        #region Tab_Browse
        /* Deselect current listview item  */
        private void ClearSelection(object sender, EventArgs e)
        { Reset(); }

        /* Starts scan of default directory */
        private void Btn_Scan_Click(object sender, EventArgs e)
        {
            Scan fmScan = new Scan();
            fmScan.delNewEntry = AddEntries;
            fmScan.CurrentItems = lData;
            fmScan.delDone = fmScanDone;
            Btn_Scan.Enabled = false;

            fmScan.Show();
            LV_Entries.Select();
        }
        private void fmScanDone() { Btn_Scan.Enabled = true; }

        /* Prevent edits of improper entries   */
        private void TxBx_Search_TextChanged(object sender, EventArgs e)
        {
            Delay.Stop();

            if (TxBx_Search.Text == string.Empty)
            {
                if (LV_Entries.Items.Count != lData.Count) UpdateLV();
                Btn_Clear.Visible = false;
            }
            else
            {
                Btn_Clear.Visible = true;
                Delay.Start();
            }
        }

        /* Clear searchbar  */
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            Reset();
            LV_Entries.Focus();
            TxBx_Search.Clear();
        }

        /* Display current entry to 'view' tab   */
        private void LV_Entries_SelectedIndexChanged(object sender, EventArgs e)
        {
            //prevent off-selection processing
            if (LV_Entries.FocusedItem == null) { Reset(); return; }

            //grab index of selected item
            int iOldIndx = indx;
            indx = (short)lData.FindIndex(new Search(LV_Entries.FocusedItem.SubItems[0].Text +
                LV_Entries.FocusedItem.SubItems[1].Text).Match);
            if (indx < 0) { Reset(); return; }
            Text = "Selected: " + lData[indx].GetName;

            //prevent replacing valid data
            if (indx == iOldIndx) return;

            //write item's metadata to Tb_View
            this.SuspendLayout();
            TxBx_Title.Text = lData[indx].sTitle;
            TxBx_Artist.Text = lData[indx].sArtist;
            TxBx_Loc.Text = lData[indx].sLoc;
            frTxBx_Desc.Text = lData[indx].sDesc;
            CmBx_Type.Text = lData[indx].sType;
            Dt_Date.Value = lData[indx].dtDate;
            ChkBx_Fav.Checked = lData[indx].bFav;
            Nud_Pages.Value = lData[indx].iPages;
            TxBx_Tags.Text = lData[indx].sTags;
            this.ResumeLayout();

            MnTS_New.Visible = false;
            MnTS_Edit.Visible = false;

            //Get image
            System.Threading.ThreadPool.QueueUserWorkItem(GetImage);
        }

        /* Sort entries based on clicked column   */
        private void LV_Entries_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //prevent sorting by tags
            if (e.Column == 3) return;

            if (e.Column != lvSortObj.ColToSort)
            {
                lvSortObj.ColToSort = e.Column;
                lvSortObj.OrdOfSort = SortOrder.Ascending;
            }
            else lvSortObj.SwapOrder(); //reverse sort order

            LV_Entries.Sort();
        }

        /* Auto-resizes Col_Tags to match Form width   */
        private void LV_Entries_Resize(object sender, EventArgs e)
        {
            LV_Entries.BeginUpdate();
            int iColWidth = 0;
            for (int i = 0; i < LV_Entries.Columns.Count; i++)
                iColWidth += LV_Entries.Columns[i].Width;

            ColTags.Width += LV_Entries.DisplayRectangle.Width - iColWidth;
            LV_Entries.EndUpdate();
        }

        /* Open folder or first image of Entry */
        private void LV_Entries_DoubleClick(object sender, EventArgs e)
        { OpenFile(); }

        /* Updates LV to only display favourited items */
        private void ChkBx_ShowFav_CheckedChanged(object sender, EventArgs e)
        {

            if (ChkBx_ShowFav.Checked) OnlyFavs();
            else UpdateLV();
        }
        #endregion

        #region Tab_View
        /* Select location of current entry   */
        private void Btn_Loc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.Description = "Select the location of the current entry.";

            //Try to auto-magically grab folder path
            string sPath = Properties.Settings.Default.DefLoc;
            if (TxBx_Loc.Text == string.Empty && Directory.Exists(string.Format("{0}\\[{1}] {2}",
                Properties.Settings.Default.DefLoc, TxBx_Artist.Text, TxBx_Title.Text)))
                sPath = string.Format("{0}\\[{1}] {2}", Properties.Settings.Default.DefLoc,
                    TxBx_Artist.Text, TxBx_Title.Text);
            else if (Directory.Exists(TxBx_Loc.Text)) sPath = TxBx_Loc.Text;
            fbd.SelectedPath = sPath;

            //Set folder path, and grab cover & page count from it
            if (fbd.ShowDialog() == DialogResult.OK && !ExtDirectory.Restricted(fbd.SelectedPath))
            {
                TxBx_Loc.Text = fbd.SelectedPath;

                //Get image
                System.Threading.ThreadPool.QueueUserWorkItem(GetImage);
            }
            fbd.Dispose();
        }

        /* Open URL in new Browser instance/tab if clicked   */
        private void frTxBx_Desc_LinkClicked(object sender, LinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(e.LinkText); }

        /* Alternative to MnTS_Open */
        private void PicBx_Cover_Click(object sender, EventArgs e)
        {
            if (TxBx_Loc.Text == null || ExtDirectory.Restricted(TxBx_Loc.Text))
                return;

            Browse fmBrowse = new Browse();
            fmBrowse.sPath = TxBx_Loc.Text;
            fmBrowse.ShowDialog();
            fmBrowse.Dispose();
        }

        /* Dynamically update PicBx when user manually alters path */
        private void TxBx_Loc_TextChanged(object sender, EventArgs e)
        {
            MnTS_Edit.Visible = true;

            if (!Directory.Exists(TxBx_Loc.Text))
            { SetPicBxNull(); return; }

            System.Threading.ThreadPool.QueueUserWorkItem(GetImage);
        }

        /* Only enable edit when changes have been made */
        #region EnableEdit
        private void EntryAlt_Text(object sender, EventArgs e)
        { MnTS_Edit.Visible = true; }

        private void EntryAlt_DtNum(object sender, EventArgs e)
        { MnTS_Edit.Visible = true; }

        private void ChkBx_Fav_CheckStateChanged(object sender, EventArgs e)
        { MnTS_Edit.Visible = true; }
        #endregion
        #endregion

        #region Menu_EntryOps
        /* Copies formatted title to clipboard   */
        private void MnTS_CopyTitle_Click(object sender, EventArgs e)
        {
            if (TxBx_Title.Text == string.Empty || TxBx_Artist.Text == string.Empty) return;
            Clipboard.SetText(string.Format("[{0}] {1}", TxBx_Artist.Text, TxBx_Title.Text));
            Text = "Name copied to clipboard";
        }

        /* Set default startup location   */
        private void MnTS_DefLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Properties.Settings.Default.DefLoc;

            if (fbd.ShowDialog() == DialogResult.OK && !ExtDirectory.Restricted(fbd.SelectedPath))
            {
                Properties.Settings.Default.DefLoc = fbd.SelectedPath;
                Properties.Settings.Default.Save();

                Text = "Default location changed";
            }
            fbd.Dispose();
        }

        /* Returns Form to default state   */
        private void MnTS_Clear_Click(object sender, EventArgs e)
        { Reset(); }

        /* Add Entry to database   */
        private void MnTS_New_Click(object sender, EventArgs e)
        {
            //reject when title unfilled
            if (TxBx_Title.Text == string.Empty)
            {
                MessageBox.Show("Title cannot be empty.", "Manga Organizer",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            stEntry en = new stEntry(TxBx_Title.Text, TxBx_Artist.Text, TxBx_Loc.Text, frTxBx_Desc.Text,
                TxBx_Tags.Text, CmBx_Type.Text, Dt_Date.Value, Nud_Pages.Value, ChkBx_Fav.Checked);

            if (!lData.Contains(en))
            {
                if (MessageBox.Show("Are you sure you wish to add \"" + en.GetName + "\"?",
                    "Manga Organizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lData.Add(en);

                    //update LV_Entries & maintain scroll position
                    AddEntries();
                    Reset();
                }
            }
            else MessageBox.Show("This item already exists in the database.",
                "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* Edit current selection in database   */
        private void MnTS_Edit_Click(object sender, EventArgs e)
        {
            //Update entry properties
            lData[indx] = new stEntry(TxBx_Title.Text, TxBx_Artist.Text, TxBx_Loc.Text, frTxBx_Desc.Text,
            TxBx_Tags.Text, CmBx_Type.Text, Dt_Date.Value, Nud_Pages.Value, ChkBx_Fav.Checked);

            //update LV_Entries & maintain scroll position
            AddEntries();
            MnTS_Edit.Visible = false;
            Text = "Edited entry: " + lData[indx].GetName;
        }

        /* Deletes current selection from database   */
        private void MnTS_Delete_Click(object sender, EventArgs e)
        {
            //ensure deletion is intentional
            DialogResult dResult = MessageBox.Show("Do you want to delete \"" +
                lData[indx].GetName + "\"s folder as well?", "Manga Organizer",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            //prevent user trying to delete locked folder
            if (ExtDirectory.Restricted(lData[indx].sLoc))
                dResult = DialogResult.No;

            //warn user before deleting large directory
            int iNumDir = Directory.GetDirectories(lData[indx].sLoc).Length;
            if (dResult == DialogResult.Yes && iNumDir > 0)
            {
                dResult = MessageBox.Show("This directory contains " + iNumDir + " subfolder(s),\n" +
                    "are you sure you wish to delete them?", "Manga Organizer",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            }

            //proceed
            if (dResult != DialogResult.Cancel)
            {
                //delete folder
                if (dResult == DialogResult.Yes)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        Delete, lData[indx].sLoc);
                }

                //remove from database
                LV_Entries.Items.Remove(LV_Entries.FocusedItem);
                lData.RemoveAt(indx);
                bSavList = false;
                Reset();
            }
        }
        private void Delete(object obj)
        { ExtDirectory.Delete(obj as string); }

        /* Open folder or first image of current   */
        private void MnTS_Open_Click(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image != null)
                OpenFile();
        }

        /* Name: MnTS_Save_Click
           Desc: Saves current database contents   */
        private void MnTS_Save_Click(object sender, EventArgs e)
        { SaveData(); }

        /* Opens database location   */
        private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
        {
            string sPath;
            if (Properties.Settings.Default.SavLoc != string.Empty)
                sPath = Properties.Settings.Default.SavLoc;
            else sPath = Environment.CurrentDirectory;

            System.Diagnostics.Process.Start(sPath);
        }

        /* Changes database save location   */
        private void MnTS_SavLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            string sPath;
            if (Properties.Settings.Default.SavLoc != string.Empty)
                sPath = Properties.Settings.Default.SavLoc;
            else sPath = Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK && !ExtDirectory.Restricted(fbd.SelectedPath))
            {
                sPath += "\\MangaDatabase.bin";
                Properties.Settings.Default.SavLoc = fbd.SelectedPath;

                //move old save to new location
                if (File.Exists(sPath))
                    File.Move(sPath, fbd.SelectedPath + "\\MangaDatabase.bin");
                else if (File.Exists(fbd.SelectedPath + "\\MangaDatabase.bin"))
                {
                    //load database
                    Cursor = Cursors.WaitCursor;
                    lData.Clear();
                    lData = FileSerializer.Deserialize<List<stEntry>>
                        (fbd.SelectedPath + "\\MangaDatabase.bin");
                    Cursor = Cursors.Default;
                    UpdateLV();
                }

                Properties.Settings.Default.Save();
                Text = "Default save location changed";
            }
            fbd.Dispose();
        }

        /* Opens current entry location   */
        private void MnTS_OpenEntryFolder_Click(object sender, EventArgs e)
        {
            if (TxBx_Loc.Text != string.Empty || !ExtDirectory.Restricted(TxBx_Loc.Text))
                System.Diagnostics.Process.Start(TxBx_Loc.Text);
        }

        /* Display info about program, license & author */
        private void MnTS_About_Click(object sender, EventArgs e)
        {
            About fmAbout = new About();
            fmAbout.ShowDialog();
        }
        #endregion

        #region Tab_Notes
        /* Prevent loss of changes in note text   */
        private void frTxBx_Notes_TextChanged(object sender, EventArgs e)
        { if (bSavText) bSavText = false; }

        /* Open URL in new Browser instance/tab if clicked   */
        private void frTxBx_Notes_LinkClicked(object sender, LinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(e.LinkText); }
        #endregion

        #region CustomMethods
        /* Add entries passed from fmScan to database */
        private void AddEntries()
        {
            //update LV_Entries & maintain scroll position
            int iPos = -1;
            if (LV_Entries.Items.Count > 0)
                iPos = LV_Entries.TopItem.Index;
            UpdateLV();
            bSavList = false;

            if (iPos > -1)
                for (int i = 0; i < 3; i++)
                    LV_Entries.TopItem = LV_Entries.Items[iPos];
        }

        /* Get first image in target folder */
        private void GetImage(Object obj)
        {
            BeginInvoke(new DelVoidVoid(SetPicBxNull));

            //Prevent operation on non-existent folder
            if (ExtDirectory.Restricted(TxBx_Loc.Text))
                return;

            //Get cover image
            List<string> lFiles = ExtDirectory.GetFiles(@TxBx_Loc.Text);
            if (lFiles.Count > 0)
            {
                FileStream fs = new FileStream(lFiles[0], FileMode.Open, FileAccess.Read);
                Invoke(new DelVoidImage(SetPicBxImage), System.Drawing.Image.FromStream(fs));
                fs.Close();
                fs.Dispose();
            }

            BeginInvoke(new DelVoidInt(SetNudCount), lFiles.Count);
        }

        #region SetImageCalls
        /* Set Nud_Pages to value passed from GetImage */
        private void SetNudCount(int iNum)
        {
            Nud_Pages.Value = iNum;

            //set display to end of line
            TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
        }

        /* Set cover to image passed from GetImage */
        private void SetPicBxImage(Object obj)
        {
            MnTS_Open.Visible = true;
            PicBx_Cover.Image = (System.Drawing.Image)obj;
        }

        /* Release old image resource */
        private void SetPicBxNull()
        {
            if (PicBx_Cover.Image != null)
            {
                PicBx_Cover.Image.Dispose();
                PicBx_Cover.Image = null;
            }
        }
        #endregion

        /* Remove non-fav'ed entries */
        private void OnlyFavs()
        {
            LV_Entries.BeginUpdate();
            for (int x = 0; x < LV_Entries.Items.Count; x++)
                if (LV_Entries.Items[x].BackColor != System.Drawing.Color.LightYellow)
                    LV_Entries.Items.RemoveAt(x--);
            LV_Entries.EndUpdate();

            Text = "Returned: " + LV_Entries.Items.Count + " entries";
        }

        /* Open folder or first image of current entry   */
        private void OpenFile()
        {
            if (TxBx_Loc.Text == string.Empty || ExtDirectory.Restricted(TxBx_Loc.Text))
                return;

            List<string> lFiles = ExtDirectory.GetFiles(@TxBx_Loc.Text);
            if (lFiles.Count > 0) System.Diagnostics.Process.Start(lFiles[0]);
            else System.Diagnostics.Process.Start(@TxBx_Loc.Text);
            lFiles.Clear();
        }

        /* Set properties back to default   */
        private void Reset()
        {
            //variables & Form
            Text = "Manga Organizer: " + lData.Count + " entries";
            indx = -1;

            //Tb_Browse
            LV_Entries.SelectedItems.Clear();

            //Tb_View
            TxBx_Loc.Clear();
            TxBx_Tags.Clear();
            TxBx_Title.Clear();
            TxBx_Artist.Clear();
            CmBx_Type.Text = "";
            Nud_Pages.Value = 0;
            frTxBx_Desc.Clear();
            PicBx_Cover.Image = null;
            ChkBx_Fav.Checked = false;
            Dt_Date.Value = DateTime.Now;

            //Mn_EntryOps
            MnTS_New.Visible = true;
            MnTS_Del.Visible = false;
            MnTS_Edit.Visible = false;
            MnTS_Open.Visible = false;
        }

        /* Saves current database & Note contents   */
        private void SaveData()
        {
            Cursor = Cursors.WaitCursor;

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc;
            if (sPath != string.Empty && !ExtDirectory.Restricted(sPath))
                sPath += "\\MangaDatabase.bin";
            else sPath = Environment.CurrentDirectory + "\\MangaDatabase.bin";

            if (!bSavList)
            {
                FileSerializer.Serialize(sPath, lData);
                bSavList = true;
            }

            if (!bSavText)
            {
                Properties.Settings.Default.Notes = frTxBx_Notes.Text;
                Properties.Settings.Default.Save();
                bSavText = true;
            }

            Cursor = Cursors.Default;
        }

        /* Search database entries   */
        private void Search()
        {
            Cursor = Cursors.WaitCursor;

            List<string> lTags = new List<string>();
            foreach (string s in TxBx_Search.Text.Split(',')) lTags.Add(s.Trim());

            //remove non-matching entries from LV_Entries
            LV_Entries.BeginUpdate();
            for (int x = 0; x < LV_Entries.Items.Count; x++)
                for (int y = 0; y < lTags.Count; y++)
                {
                    bool bContains = false;

                    //search by type, tags, title, and artist
                    if (LV_Entries.Items[x].SubItems[3].Text.Contains(lTags[y], StringComparison.OrdinalIgnoreCase) ||
                        LV_Entries.Items[x].SubItems[1].Text.Contains(lTags[y], StringComparison.OrdinalIgnoreCase) ||
                        LV_Entries.Items[x].SubItems[0].Text.Contains(lTags[y], StringComparison.OrdinalIgnoreCase) ||
                        LV_Entries.Items[x].SubItems[4].Text.Contains(lTags[y], StringComparison.OrdinalIgnoreCase))
                        bContains = true;

                    if (!bContains)
                    {
                        LV_Entries.Items.RemoveAt(x--);
                        break;
                    }
                }
            LV_Entries.EndUpdate();

            Text = "Returned: " + LV_Entries.Items.Count + " entries";
            Cursor = Cursors.Default;
        }

        /* Refresh displayed entries in ListView   */
        private void UpdateLV()
        {
            Cursor = Cursors.WaitCursor;
            Text = "Manga Organizer: " + lData.Count + " entries";
            List<ListViewItem> lItems = new List<ListViewItem>(LV_Entries.Items.Count + 1);

            //refresh LV_Entries
            for (int i = 0; i < lData.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                lvi.SubItems.Add(lData[i].sTitle);
                lvi.SubItems.Add(lData[i].iPages.ToString());
                lvi.SubItems.Add(lData[i].sTags);
                lvi.SubItems.Add(lData[i].sType);
                if (lData[i].bFav) lvi.BackColor = System.Drawing.Color.LightYellow;
                lItems.Add(lvi);
            }

            //Update listview display
            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(lItems.ToArray());
            lItems.Clear();
            LV_Entries.Sort();
            LV_Entries.EndUpdate();
            Cursor = Cursors.Default;

            //prevent loss of search parameters
            if (ChkBx_ShowFav.Checked) OnlyFavs();
            if (TxBx_Search.Text != string.Empty) Search();
        }
        #endregion

        #region Menu_RichText
        private void MnRTx_Undo_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.Undo();
            else frTxBx_Desc.Undo();
        }

        private void MnRTx_Cut_Click(object sender, EventArgs e)
        {
            bSavText = false;
            MnRTx_Undo.Enabled = true;
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.Cut();
            else frTxBx_Desc.Cut();
        }

        private void MnRTx_Copy_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.Copy();
            else frTxBx_Desc.Copy();
        }

        private void MnRTx_Paste_Click(object sender, EventArgs e)
        {
            bSavText = false;
            MnRTx_Undo.Enabled = true;
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.Paste();
            else frTxBx_Desc.Paste();
        }

        private void MnRTx_SelectAll_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.SelectAll();
            else frTxBx_Desc.SelectAll();
        }
        #endregion

        #region Menu_Text
        private void MnTx_Undo_Click(object sender, EventArgs e)
        {
            if (!(ActiveControl is TextBox)) return;
            (ActiveControl as TextBox).Undo();
        }

        private void MnTx_Cut_Click(object sender, EventArgs e)
        {
            MnTx_Undo.Enabled = true;
            if (!(ActiveControl is TextBox)) return;
            (ActiveControl as TextBox).Cut();
        }

        private void MnTx_Copy_Click(object sender, EventArgs e)
        {
            if (!(ActiveControl is TextBox)) return;
            (ActiveControl as TextBox).Copy();
        }

        private void MnTx_Paste_Click(object sender, EventArgs e)
        {
            MnTx_Undo.Enabled = true;
            if (!(ActiveControl is TextBox)) return;
            TextBox txbx = ActiveControl as TextBox;

            if (txbx == TxBx_Artist && TxBx_Artist.Text == string.Empty
                && TxBx_Title.Text == string.Empty)
            {
                txbx.Paste();
                string[] sName = (TxBx_Artist.Text).Split(
                    new string[] { "[", "]" }, StringSplitOptions.None);

                if (sName.Length == 3)
                {
                    TxBx_Artist.Text = sName[1].Trim();
                    TxBx_Title.Text = sName[2].Trim();
                }
            }
            else txbx.Paste();
        }

        private void MnTx_SelAll_Click(object sender, EventArgs e)
        {
            if (!(ActiveControl is TextBox)) return;
            (ActiveControl as TextBox).SelectAll();
        }
        #endregion

        #region DragDropEvents
        /* Try to break dragged text down into Artist & Title */
        private void TxBx_Artist_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string sAdd = (string)e.Data.GetData(DataFormats.Text);
                string[] sName = (sAdd).Split(new string[] { "[", "]" },
                    StringSplitOptions.None);

                if (TxBx_Artist.Text == string.Empty &&
                    TxBx_Title.Text == string.Empty &&
                    sName.Length == 3)
                {
                    TxBx_Artist.Text = sName[1].Trim();
                    TxBx_Title.Text = sName[2].Trim();
                }
                else
                {
                    int iNewStart = TxBx_Artist.SelectionStart + sAdd.Length;
                    TxBx_Artist.Text = TxBx_Artist.Text.Insert(
                        TxBx_Artist.SelectionStart, sAdd);
                    TxBx_Artist.SelectionStart = iNewStart;
                }
            }
        }

        /* Show proper cursor when text is dragged over TxBx */
        private void DragEnterTxBx(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /* Parse dropped text into TxBx */
        private void DragDropTxBx(object sender, DragEventArgs e)
        {
            string sAdd = (string)e.Data.GetData(DataFormats.Text);
            int iNewStart;

            //Add text at cursor position
            if (sender == frTxBx_Desc || sender == frTxBx_Notes)
            {
                FixedRichTextBox frTxBx = (FixedRichTextBox)sender;

                iNewStart = frTxBx.SelectionStart + sAdd.Length;
                frTxBx.Text = frTxBx.Text.Insert(frTxBx.SelectionStart, sAdd);
                frTxBx.SelectionStart = iNewStart;
            }
            else if (sender == TxBx_Loc)
            {
                string sPath = sAdd;
                TxBx_Loc.Text = sPath;

                //Get image
                System.Threading.ThreadPool.QueueUserWorkItem(GetImage);
            }
            else //if (sender == TxBx_Artist/Title/Loc)
            {
                TextBox TxBx = (TextBox)sender;

                iNewStart = TxBx.SelectionStart + sAdd.Length;
                TxBx.Text = TxBx.Text.Insert(TxBx.SelectionStart, sAdd);
                TxBx.SelectionStart = iNewStart;
            }
        }
        #endregion

        #region Timer
        /* Inserts delay before Search() to account for Human input speed */
        private void Pause_Tick(object sender, EventArgs e)
        {
            Delay.Stop();
            UpdateLV();
            Search();
        }
        #endregion
    }
}
