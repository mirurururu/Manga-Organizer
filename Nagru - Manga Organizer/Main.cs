/*
 * Author: Taylor Napier
 * Date: Aug15, 2012
 * Desc: Handles organization of manga library
 */

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nagru___Manga_Organizer
{
    public partial class Main : Form
    {
        delegate void DelVoidVoid();
        delegate void DelVoidInt(int iNum);
        delegate void DelVoidString(string sMsg);

        Thread thWork;
        LVsorter lvSortObj = new LVsorter();
        List<stEntry> lData = new List<stEntry>(500);
        bool bSavList = true, bSavText = true;
        int indx = -1, iPage = -1;

        /* Holds metadata about manga in library */
        [Serializable]
        public struct stEntry : ISerializable
        {
            public string sTitle, sArtist,
                sLoc, sType, sDesc, sTags;
            public DateTime dtDate;
            public ushort iPages;
            public bool bFav;

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
                sTags = string.Empty;
                List<string> lCheck = new List<string>(20);
                foreach (string s in Tags.Split(','))
                    if (s != string.Empty) lCheck.Add(s.Trim());

                lCheck.Sort(new TrueCompare());
                lCheck = lCheck.Distinct().ToList<string>();
                for (int i = 0; i < lCheck.Count; i++)
                {
                    if (i != 0) sTags += ", ";
                    sTags += lCheck[i];
                }
            }

            //Returns formatted title of Entry
            public override string ToString()
            {
                if (sArtist != string.Empty)
                    return string.Format("[{0}] {1}", sArtist, sTitle);
                else return sTitle;
            }

            /* Override equals to compare entry titles */
            public override bool Equals(object obj)
            {
                if (obj is stEntry)
                {
                    stEntry en = (stEntry)obj;
                    return (en.sArtist + en.sTitle).Equals(sArtist
                        + sTitle, StringComparison.OrdinalIgnoreCase);
                }
                else if (obj is string)
                {
                    return (obj as string).Equals(sArtist + sTitle,
                        StringComparison.OrdinalIgnoreCase);
                }
                else return false;
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
        public Main(string[] sFile)
        {
            InitializeComponent();

            //if database opened with "Open with..."
            if (sFile.Length > 0 && File.Exists(sFile[0]) &&
                Path.GetFileName(sFile[0]) == "MangaDatabase.bin" &&
                Properties.Settings.Default.SavLoc != sFile[0])
            {
                Properties.Settings.Default.SavLoc = sFile[0];
                Properties.Settings.Default.Save();
                MessageBox.Show("Default database location changed to \"" + sFile + "\"",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //manually handle AssemblyResolve event
            AppDomain.CurrentDomain.AssemblyResolve +=
                new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /* Load non-MS library (HtmlAgilityPack) 
           Author: Calle Mellergardh (March 1, 2010) */
        System.Reflection.Assembly CurrentDomain_AssemblyResolve(
            object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("HtmlAgilityPack"))
                return (sender as AppDomain).Load(Nagru___Manga_Organizer.
                    Properties.Resources.HtmlAgilityPack);
            else return null;
        }

        /* Load database   */
        private void Main_Load(object sender, EventArgs e)
        {
            //disable ContextMenu in Nud_Pages
            Nud_Pages.ContextMenuStrip = new ContextMenuStrip();

            //ensure listview has focus & is properly sized/sorted
            LV_Entries.ListViewItemSorter = lvSortObj;
            LV_Entries_Resize(sender, e);
            LV_Entries.Select();

            //set frTxBx's to allow dragdrop & initialize Notes
            frTxBx_Notes.AllowDrop = true; frTxBx_Desc.AllowDrop = true;
            frTxBx_Notes.DragDrop += new DragEventHandler(DragDropTxBx);
            frTxBx_Desc.DragDrop += new DragEventHandler(DragDropTxBx);
            frTxBx_Notes.DragEnter += new DragEventHandler(DragEnterTxBx);
            frTxBx_Desc.DragEnter += new DragEventHandler(DragEnterTxBx);
            frTxBx_Notes.Text = Properties.Settings.Default.Notes;

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            sPath += "\\MangaDatabase.bin";

            //load database
            if (lData.Count == 0 && File.Exists(sPath))
            {
                Cursor = Cursors.WaitCursor;
                lData.AddRange(FileSerializer.Deserialize<List<stEntry>>(sPath));
                Cursor = Cursors.Default;

                if (lData != null) UpdateLV();
                else lData = new List<stEntry>(0);

                //populate list of artists
                List<string> lAuto = new List<string>(lData.Count);
                for (int i = 0; i < lData.Count; i++) lAuto.Add(lData[i].sArtist);
                lAuto.Sort(new TrueCompare());
                string[] sFinal = lAuto.Distinct().ToArray();
                CmbBx_Artist.AutoCompleteCustomSource.AddRange(sFinal);
                CmbBx_Artist.Items.AddRange(sFinal);
            }
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
                    if (indx == -1)
                        Text = (TxBx_Search.Text == string.Empty ? "Manga Organizer: "
                            + lData.Count : "Returned: " + LV_Entries.Items.Count) + " entries";
                    this.AcceptButton = Btn_Clear;
                    LV_Entries.Focus();
                    break;
                case 1:
                    if (indx != -1)
                    {
                        //Update Form title
                        if (!Text.StartsWith("S"))
                            Text = "Selected: " + lData[indx].ToString();
                        MnTS_Del.Visible = true;
                        MnTS_Open.Visible = true;
                    }
                    this.AcceptButton = null;
                    break;
                default:
                    this.AcceptButton = null;
                    break;
            }
        }

        /* Inserts delay before Search() to account for Human input speed */
        private void Delay_Tick(object sender, EventArgs e)
        {
            Delay.Stop();
            UpdateLV();
        }
        #endregion

        #region Tab_Browse
        /* Deselect current listview item  */
        private void ClearSelection(object sender, EventArgs e)
        { if (indx != -1) Reset(); }

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
            LV_Entries.Focus();
            TxBx_Search.Clear();
            UpdateLV();
        }

        /* Display current entry to 'view' tab   */
        private void LV_Entries_SelectedIndexChanged(object sender, EventArgs e)
        {
            //prevent off-selection processing
            if (LV_Entries.FocusedItem == null || LV_Entries.SelectedItems.Count == 0)
            { Reset(); return; }

            thWork = new System.Threading.Thread(GetIndex);
            thWork.IsBackground = true;
            thWork.Start(LV_Entries.FocusedItem);
        }

        /* Sort entries based on clicked column   */
        private void LV_Entries_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //prevent sorting by tags
            if (e.Column == 3) return;

            if (e.Column != lvSortObj.ColToSort)
                lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
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
            LV_Entries.Select();
        }
        #endregion

        #region Tab_View
        /* Select location of current entry   */
        private void Btn_Loc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.Description = "Select the location of the current entry:";

            //Try to auto-magically grab folder path
            string sPath = string.Format("{0}\\[{1}] {2}",
                Properties.Settings.Default.DefLoc,
                CmbBx_Artist.Text, TxBx_Title.Text);
            if (Directory.Exists(sPath)) { }
            else if (Directory.Exists(TxBx_Loc.Text)) sPath = TxBx_Loc.Text;
            else sPath = Properties.Settings.Default.DefLoc;
            fbd.SelectedPath = sPath;

            //Set folder path, and grab cover & page count from it
            if (fbd.ShowDialog() == DialogResult.OK &&
                !ExtDirectory.Restricted(fbd.SelectedPath))
            {
                TxBx_Loc.Text = fbd.SelectedPath;

                //Get image
                thWork = new System.Threading.Thread(GetImage);
                thWork.IsBackground = true;
                thWork.Start();
            }
            fbd.Dispose();
        }

        /* Open URL in new Browser instance/tab if clicked   */
        private void frTxBx_Desc_LinkClicked(object sender, LinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(e.LinkText); }

        /* Alternative to MnTS_Open */
        private void PicBx_Cover_Click(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image == null) return;

            Browse fmBrowse = new Browse();
            fmBrowse.sPath = TxBx_Loc.Text;
            fmBrowse.iPage = iPage;

            fmBrowse.ShowDialog();
            iPage = fmBrowse.iPage;
            fmBrowse.Dispose();
            GC.Collect();
        }

        /* Dynamically update PicBx when user manually alters path */
        private void TxBx_Loc_TextChanged(object sender, EventArgs e)
        {
            if (indx != -1) MnTS_Edit.Visible = true;

            if (Directory.Exists(TxBx_Loc.Text))
            {
                iPage = -1;
                thWork = new Thread(GetImage);
                thWork.IsBackground = true;
                thWork.Start();
            }
            else SetPicBxNull();
        }

        /* Select next item in listview */
        private void Btn_GoDn_Click(object sender, EventArgs e)
        {
            if (LV_Entries.SelectedItems.Count == 0) return;
            int iPos = LV_Entries.SelectedItems[0].Index;

            if (++iPos >= LV_Entries.Items.Count) iPos = 0;
            LV_Entries.FocusedItem = LV_Entries.Items[iPos];
            LV_Entries.Items[iPos].Selected = true;
        }

        /* Select previous item in listview */
        private void Btn_GoUp_Click(object sender, EventArgs e)
        {
            if (LV_Entries.SelectedItems.Count == 0) return;
            int iPos = LV_Entries.SelectedItems[0].Index;

            if (--iPos < 0) iPos = LV_Entries.Items.Count - 1;
            LV_Entries.FocusedItem = LV_Entries.Items[iPos];
            LV_Entries.Items[iPos].Selected = true;
        }

        /* Only enable edit when changes have been made */
        #region EnableEdit
        private void EntryAlt_Text(object sender, EventArgs e)
        { if (indx != -1) MnTS_Edit.Visible = true; }

        private void EntryAlt_DtNum(object sender, EventArgs e)
        { if (indx != -1) MnTS_Edit.Visible = true; }

        private void ChkBx_Fav_CheckStateChanged(object sender, EventArgs e)
        { if (indx != -1) MnTS_Edit.Visible = true; }
        #endregion
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
        void AddEntries()
        {
            //update LV_Entries & maintain selection
            UpdateLV();
            bSavList = false;
            if (indx == -1) return;

            int iPos = 0;
            for (int i = 0; i < LV_Entries.Items.Count; i++)
                if (LV_Entries.Items[i].SubItems[1].Text == lData[indx].sTitle)
                {
                    LV_Entries.FocusedItem = LV_Entries.Items[i];
                    LV_Entries.Items[i].Selected = true;
                    iPos = i;
                    break;
                }
            LV_Entries.Select();

            /* Compensate for broken scroll-to function
             * by running it multiple times (3 is sweet-spot) */
            LV_Entries.TopItem = LV_Entries.Items[iPos];
            LV_Entries.TopItem = LV_Entries.Items[iPos];
            LV_Entries.TopItem = LV_Entries.Items[iPos];
        }

        /* Get first image in target folder */
        void GetImage(Object obj)
        {
            BeginInvoke(new DelVoidVoid(SetPicBxNull));

            //Prevent operation on non-existent folder
            if (ExtDirectory.Restricted(TxBx_Loc.Text))
                return;

            //Get cover image
            string[] sFiles = ExtDirectory.GetFiles(
                @TxBx_Loc.Text, SearchOption.TopDirectoryOnly);
            if (sFiles.Length > 0)
                BeginInvoke(new DelVoidString(SetPicBxImage), sFiles[0]);
            BeginInvoke(new DelVoidInt(SetNudCount), sFiles.Length);
        }

        #region SetImageCalls
        /* Set Nud_Pages to value passed from GetImage */
        void SetNudCount(int iNum)
        {
            Nud_Pages.Value = iNum;

            //set display to end of line
            TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
        }

        /* Set cover to image passed from GetImage */
        void SetPicBxImage(Object obj)
        {
            MnTS_Open.Visible = true;

            using (var bmpTemp = new Bitmap(obj as string))
            {
                PicBx_Cover.Image = new Bitmap(bmpTemp);
            }
        }

        /* Release old image resource */
        void SetPicBxNull()
        {
            if (PicBx_Cover.Image == null) return;
            PicBx_Cover.Image.Dispose();
            PicBx_Cover.Image = null;

            //force garbage collection
            GC.Collect();
        }
        #endregion

        /* Find list index of currently selected item */
        void GetIndex(Object obj)
        {
            ListViewItem lvi = (ListViewItem)obj;

            //grab index of selected item
            this.BeginInvoke(new DelVoidInt(SetData), lData.FindIndex(new Search(
                lvi.SubItems[0].Text + lvi.SubItems[1].Text).Match));
        }

        /* Remove non-fav'ed entries */
        void OnlyFavs()
        {
            Cursor = Cursors.WaitCursor;
            List<ListViewItem> lItems = new List<ListViewItem>(LV_Entries.Items.Count + 1);

            //refresh LV_Entries
            for (int i = 0; i < lData.Count; i++)
            {
                if (!lData[i].bFav) continue;
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                lvi.BackColor = Color.LightYellow;
                lvi.SubItems.Add(lData[i].sTitle);
                lvi.SubItems.Add(lData[i].iPages.ToString());
                lvi.SubItems.Add(lData[i].sTags);
                lvi.SubItems.Add(lData[i].sType);
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
            if (TxBx_Search.Text != string.Empty) Search();
            Text = "Returned: " + LV_Entries.Items.Count + " entries";
        }

        /* Open folder or first image of current entry   */
        void OpenFile()
        {
            if (TxBx_Loc.Text == string.Empty || 
                ExtDirectory.Restricted(TxBx_Loc.Text))
                return;

            string[] sFiles = ExtDirectory.GetFiles(@TxBx_Loc.Text);
            if (sFiles.Length > 0) System.Diagnostics.Process.Start(sFiles[0]);
            else System.Diagnostics.Process.Start(@TxBx_Loc.Text);
        }

        /* Set properties back to default   */
        void Reset()
        {
            //reset Form title
            Text = (TxBx_Search.Text == string.Empty ? "Manga Organizer: "
                + lData.Count : "Returned: " + LV_Entries.Items.Count) + " entries";

            //Tb_Browse
            LV_Entries.FocusedItem = null;
            LV_Entries.SelectedItems.Clear();
            iPage = -1;
            indx = -1;

            //Tb_View
            TxBx_Loc.Clear();
            TxBx_Tags.Clear();
            TxBx_Title.Clear();
            CmbBx_Artist.Text = "";
            CmbBx_Type.Text = "Manga";
            Nud_Pages.Value = 0;
            frTxBx_Desc.Clear();
            ChkBx_Fav.Checked = false;
            Dt_Date.Value = DateTime.Now;
            SetPicBxNull();

            //Mn_EntryOps
            MnTS_New.Visible = true;
            MnTS_Del.Visible = false;
            MnTS_Edit.Visible = false;
            MnTS_Open.Visible = false;
        }

        /* Saves current database & Note contents   */
        void SaveData()
        {
            Cursor = Cursors.WaitCursor;
            Text = "Saving...";

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc;
            if (sPath != string.Empty && !ExtDirectory.Restricted(sPath))
                sPath += "\\MangaDatabase.bin";
            else sPath = Environment.CurrentDirectory + "\\MangaDatabase.bin";

            if (!bSavList)
            {
                if (File.Exists(sPath)) File.Delete(sPath);
                FileSerializer.Serialize(sPath, lData);
                bSavList = true;
            }

            if (!bSavText)
            {
                Properties.Settings.Default.Notes = frTxBx_Notes.Text;
                Properties.Settings.Default.Save();
                bSavText = true;
            }

            Text = "Saved";
            Cursor = Cursors.Default;
        }

        /* Update controls in 'View' tab */
        void SetData(int iNewIndx)
        {
            if ((indx = (short)iNewIndx) == -1)
            { Reset(); return; }

            //write item's metadata to Tb_View
            Text = "Selected: " + lData[indx].ToString();
            this.SuspendLayout();
            TxBx_Title.Text = lData[indx].sTitle;
            CmbBx_Artist.Text = lData[indx].sArtist;
            TxBx_Loc.Text = lData[indx].sLoc;
            frTxBx_Desc.Text = lData[indx].sDesc;
            CmbBx_Type.Text = lData[indx].sType;
            Dt_Date.Value = lData[indx].dtDate;
            ChkBx_Fav.Checked = lData[indx].bFav;
            Nud_Pages.Value = lData[indx].iPages;
            TxBx_Tags.Text = lData[indx].sTags;
            this.ResumeLayout();

            MnTS_New.Visible = false;
            MnTS_Edit.Visible = false;
        }

        /* Search database entries   */
        void Search()
        {
            Cursor = Cursors.WaitCursor;
            List<string> lTags = new List<string>();
            foreach (string s in TxBx_Search.Text.Split(',')) 
                lTags.Add(s.TrimStart());
            
            //remove non-matching entries from LV_Entries
            LV_Entries.BeginUpdate();
            for (int x = 0; x < LV_Entries.Items.Count; x++)
                for (int y = 0; y < lTags.Count; y++)
                {
                    //search by tags, title, artist, and type
                    if (ExtString.Contains(LV_Entries.Items[x].SubItems[3].Text, lTags[y]) ||
                        ExtString.Contains(LV_Entries.Items[x].SubItems[1].Text, lTags[y]) ||
                        ExtString.Contains(LV_Entries.Items[x].SubItems[0].Text, lTags[y]) ||
                        ExtString.Contains(LV_Entries.Items[x].SubItems[4].Text, lTags[y])) { }
                    else { LV_Entries.Items.RemoveAt(x--); break; }
                }
            LV_Entries.EndUpdate();

            Text = "Returned: " + LV_Entries.Items.Count + " entries";
            Cursor = Cursors.Default;
        }

        /* Split passed text into Artist & Title */
        void SplitTitle(string sRaw)
        {
            //Get formatted title
            if (sRaw.StartsWith("("))
            {
                int iPos = sRaw.IndexOf(')') + 2;

                if (sRaw.Length - 1 >= iPos)
                    sRaw = sRaw.Remove(0, iPos);
            }
            string[] sName = sRaw.Split(new char[] { '[', ']' },
                StringSplitOptions.RemoveEmptyEntries);

            //parse it out
            if (CmbBx_Artist.Text == string.Empty &&
                TxBx_Title.Text == string.Empty &&
                sName.Length >= 2)
            {
                CmbBx_Artist.Text = sName[0].Trim();
                TxBx_Title.Text = sName[1].Trim();
            }
            else
            {
                int iNewStart = CmbBx_Artist.SelectionStart + sRaw.Length;
                CmbBx_Artist.Text = CmbBx_Artist.Text.Insert(
                    CmbBx_Artist.SelectionStart, sRaw);
                CmbBx_Artist.SelectionStart = iNewStart;
            }
        }

        /* Refresh displayed entries in ListView   */
        void UpdateLV()
        {
            Cursor = Cursors.WaitCursor;
            Text = "Manga Organizer: " + lData.Count + " entries";
            List<ListViewItem> lItems = new List<ListViewItem>(LV_Entries.Items.Count + 1);

            //refresh LV_Entries
            for (int i = 0; i < lData.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                if (lData[i].bFav) lvi.BackColor = Color.LightYellow;
                lvi.SubItems.Add(lData[i].sTitle);
                lvi.SubItems.Add(lData[i].iPages.ToString());
                lvi.SubItems.Add(lData[i].sTags);
                lvi.SubItems.Add(lData[i].sType);
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
            else if (TxBx_Search.Text != string.Empty) Search();
        }
        #endregion

        #region Menu_EntryOps
        /* Copies formatted title to clipboard   */
        private void MnTS_CopyTitle_Click(object sender, EventArgs e)
        {
            if (TxBx_Title.Text == string.Empty || CmbBx_Artist.Text == string.Empty) return;
            Clipboard.SetText(string.Format("[{0}] {1}", CmbBx_Artist.Text, TxBx_Title.Text));
            Text = "Name copied to clipboard";
        }

        /* Set default startup location   */
        private void MnTS_DefLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Properties.Settings.Default.DefLoc;

            if (fbd.ShowDialog() == DialogResult.OK &&
                !ExtDirectory.Restricted(fbd.SelectedPath))
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
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            stEntry en = new stEntry(TxBx_Title.Text, CmbBx_Artist.Text,
                TxBx_Loc.Text, frTxBx_Desc.Text, TxBx_Tags.Text, CmbBx_Type.Text,
                Dt_Date.Value.Date, Nud_Pages.Value, ChkBx_Fav.Checked);

            if (!lData.Contains(en))
            {
                if (MessageBox.Show("Are you sure you wish to add:\n\"" + en + "\"?",
                    "Manga Organizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lData.Add(en);

                    //update LV_Entries & maintain scroll position
                    if (!CmbBx_Artist.Items.Contains(CmbBx_Artist.Text))
                    {
                        CmbBx_Artist.AutoCompleteCustomSource.Add(CmbBx_Artist.Text);
                        CmbBx_Artist.Items.Add(CmbBx_Artist.Text);
                    }
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
            lData[indx] = new stEntry(TxBx_Title.Text, CmbBx_Artist.Text,
                TxBx_Loc.Text, frTxBx_Desc.Text, TxBx_Tags.Text, CmbBx_Type.Text,
                Dt_Date.Value, Nud_Pages.Value, ChkBx_Fav.Checked);

            //update LV_Entries & maintain selection
            AddEntries();
            MnTS_Edit.Visible = false;
            Text = "Edited entry: " + lData[indx].ToString();
        }

        /* Deletes current selection from database   */
        private void MnTS_Delete_Click(object sender, EventArgs e)
        {
            //ensure deletion is intentional
            DialogResult dResult = MessageBox.Show("Do you want to delete \"" +
                lData[indx] + "\"s folder as well?", "Manga Organizer",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            //prevent user trying to delete locked folder
            if (dResult == DialogResult.Yes)
            {
                if (ExtDirectory.Restricted(lData[indx].sLoc))
                    dResult = DialogResult.No;
                else
                {
                    //warn user before deleting large directory
                    int iNumDir = Directory.GetDirectories(lData[indx].sLoc).Length;
                    if (iNumDir > 0)
                        dResult = MessageBox.Show("This directory contains " + iNumDir + " subfolder(s),\n" +
                            "are you sure you want to delete them?", "Manga Organizer",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                }
            }

            //proceed
            if (dResult != DialogResult.Cancel)
            {
                //delete folder
                if (dResult == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;
                    ExtDirectory.Delete(lData[indx].sLoc);
                    this.Cursor = Cursors.Default;
                }

                //remove from database
                lData.RemoveAt(indx);
                LV_Entries.Items.Remove(LV_Entries.FocusedItem);
                bSavList = false;
            }
        }

        /* Open folder or first image of current   */
        private void MnTS_Open_Click(object sender, EventArgs e)
        { if (PicBx_Cover.Image != null) OpenFile(); }

        /* Saves current database contents   */
        private void MnTS_Save_Click(object sender, EventArgs e)
        { SaveData(); }

        /* Opens database location   */
        private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
        {
            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                sPath = Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;

            System.Diagnostics.Process.Start(sPath);
        }

        /* Changes database save location   */
        private void MnTS_SavLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                sPath = Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK && !ExtDirectory.Restricted(fbd.SelectedPath))
            {
                sPath += "\\MangaDatabase.bin";
                Properties.Settings.Default.SavLoc = fbd.SelectedPath;
                fbd.SelectedPath += @"\MangaDatabase.bin";

                //move old save to new location
                if (File.Exists(sPath))
                    File.Move(sPath, fbd.SelectedPath);
                else if (File.Exists(fbd.SelectedPath))
                {
                    //load database
                    Cursor = Cursors.WaitCursor;
                    lData.Clear();
                    lData = FileSerializer.Deserialize
                        <List<stEntry>>(fbd.SelectedPath);
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

        /* Grabs entry data from passed URL */
        private void MnTS_GET_Click(object sender, EventArgs e)
        {
            GetUrl fmGet = new GetUrl();
            fmGet.ShowDialog();

            //process url
            if (fmGet.DialogResult == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                Text = "Downloading page...";

                HtmlAgilityPack.HtmlWeb htmlWeb = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument htmlDoc = htmlWeb.Load(fmGet.Url);

                //ensure page exists
                if (htmlWeb.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //grab artist & title
                    try
                    {
                        SplitTitle(htmlDoc.GetElementbyId("gn").InnerText.Replace("&#039;", "'"));

                        //split 'gdd' table down into usable elements
                        string[] sSplit = ExtString.Split(htmlDoc.GetElementbyId("gdd").InnerHtml, "\"gdt2\">");
                        Dt_Date.Value = Convert.ToDateTime(ExtString.Split(sSplit[1], "</td>")[0]);
                        Nud_Pages.Value = Convert.ToInt32(ExtString.Split(sSplit[2], "@")[0]);

                        //split taglist down into usable elements
                        sSplit = ExtString.Split(htmlDoc.GetElementbyId(
                            "taglist").InnerHtml, "this)\">");
                        for (int i = 1; i < sSplit.Length; i++)
                        {
                            TxBx_Tags.Text += ExtString.Split(sSplit[i], "</a>")[0].Trim();
                            if (i < sSplit.Length - 1) TxBx_Tags.Text += ", ";
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("This gallery is pining for the fjords\n(Sorry, no EX support yet).",
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else MessageBox.Show("URL was invalid. Please make sure it comes from an EH gallery page.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Text = "Finished";
                this.Cursor = Cursors.Default;
            }
            fmGet.Dispose();
        }

        /* Secondary way to close program */
        private void MnTs_Quit_Click(object sender, EventArgs e)
        { this.Close(); }
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
            if (ActiveControl is TextBox)
                (ActiveControl as TextBox).Undo();
            else (ActiveControl as ComboBox).ResetText();
        }

        private void MnTx_Cut_Click(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox)
                (ActiveControl as TextBox).Cut();
            else 
            {
                Clipboard.SetText((ActiveControl as ComboBox).SelectedText);
                (ActiveControl as ComboBox).SelectedText = "";
            }
        }

        private void MnTx_Copy_Click(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox)
                (ActiveControl as TextBox).Copy();
            else Clipboard.SetText((ActiveControl as ComboBox).SelectedText);
        }

        private void MnTx_Paste_Click(object sender, EventArgs e)
        {
            MnTx_Undo.Enabled = true;
            if (ActiveControl is TextBox)
            {
                TextBox txbx = ActiveControl as TextBox;

                if (txbx == TxBx_Tags)
                {
                    if (!Clipboard.GetText().Contains("\n"))
                    { txbx.Paste(); return; }

                    string[] sTags = Clipboard.GetText().Split(new char[] { '(', '\n' });

                    for (int i = 0; i < sTags.Length; i++)
                        if (!sTags[i].EndsWith("\r") && !sTags[i].EndsWith(")"))
                        {
                            TxBx_Tags.Text += sTags[i].TrimEnd();

                            if (i != sTags.Length - 2)
                                TxBx_Tags.Text += ", ";
                        }
                }
                else txbx.Paste();
            }
            else if (ActiveControl is ComboBox)
            {
                if (CmbBx_Artist.Text == string.Empty
                    && TxBx_Title.Text == string.Empty)
                {
                    SplitTitle(Clipboard.GetText());
                }
                else CmbBx_Artist.Text = Clipboard.GetText();
            }
        }

        private void MnTx_SelAll_Click(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox)
                (ActiveControl as TextBox).SelectAll();
            else (ActiveControl as ComboBox).SelectAll();
        }
        #endregion

        #region DragDropEvents
        /* Try to break dragged text down into Artist & Title */
        private void TxBx_Artist_DragDrop(object sender, DragEventArgs e)
        { SplitTitle((string)e.Data.GetData(DataFormats.Text)); }

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

            switch ((sender as Control).Name)
            {
                case "frTxBx_Desc":
                case "frTxBx_Notes":
                    FixedRichTextBox frTxBx = (FixedRichTextBox)sender;
                    iNewStart = frTxBx.SelectionStart + sAdd.Length;
                    frTxBx.Text = frTxBx.Text.Insert(frTxBx.SelectionStart, sAdd);
                    frTxBx.SelectionStart = iNewStart;
                    break;
                case "TxBx_Title":
                    iNewStart = TxBx_Title.SelectionStart + sAdd.Length;
                    TxBx_Title.Text = TxBx_Title.Text.Insert(
                        TxBx_Title.SelectionStart, sAdd);
                    TxBx_Title.SelectionStart = iNewStart;
                    break;
                case "TxBx_Loc":
                    TxBx_Loc.Text = sAdd;
                    thWork = new System.Threading.Thread(GetImage);
                    thWork.IsBackground = true;
                    thWork.Start();
                    break;
                case "TxBx_Tags":
                    if (!sAdd.Contains("\n"))
                    { TxBx_Tags.Text += sAdd; break; }

                    string[] sTags = sAdd.Split(new char[] { '(', '\n' });
                    for (int i = 0; i < sTags.Length; i++)
                        if (!sTags[i].EndsWith("\r") && !sTags[i].EndsWith(")"))
                        {
                            TxBx_Tags.Text += sTags[i].TrimEnd();

                            if (i != sTags.Length - 2)
                                TxBx_Tags.Text += ", ";
                        }
                    break;
                case "CmbBx_Artist":
                    iNewStart = CmbBx_Artist.SelectionStart + sAdd.Length;
                    CmbBx_Artist.Text = CmbBx_Artist.Text.Insert(
                        CmbBx_Artist.SelectionStart, sAdd);
                    CmbBx_Artist.SelectionStart = iNewStart;
                    break;
            }
        }

        /* Allow dropping of folders into LV_Entries */
        private void LV_Entries_DragEnter(object sender, DragEventArgs e)
        {
            string[] sTemp = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (sTemp == null) return;

            FileAttributes fa = File.GetAttributes(sTemp[0]);
            if (fa == FileAttributes.Directory)
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /* Adds folder to database when dragged over LV_Entries */
        private void LV_Entries_DragDrop(object sender, DragEventArgs e)
        {
            string sFile = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];

            if (!ExtDirectory.Restricted(sFile))
            {
                //Get formatted directory name
                string[] sTitle = Path.GetFileName(sFile).Split(
                    new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                //add item
                Main.stEntry en;
                if (sTitle.Length == 2)
                    en = new Main.stEntry(sTitle[1].TrimStart(), sTitle[0], sFile, "", "", "",
                        DateTime.Now, ExtDirectory.GetFiles(sFile).Length, false);
                else
                    en = new Main.stEntry(sTitle[0].TrimStart(), "", sFile, "", "", "",
                        DateTime.Now, ExtDirectory.GetFiles(sFile).Length, false);

                if (!lData.Contains(en))
                {
                    lData.Add(en);
                    indx = lData.Count - 1;
                    AddEntries();
                }
            }
        }
        #endregion
    }
}
