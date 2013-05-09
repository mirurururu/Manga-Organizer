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
        short indx = -1, iPage = -1;

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
        }

        /* Load database */
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
            frTxBx_Desc.DragEnter += new DragEventHandler(DragEnterTxBx);
            frTxBx_Notes.DragEnter += new DragEventHandler(DragEnterTxBx);
            frTxBx_Notes.Text = Properties.Settings.Default.Notes;

            //Set size & position
            this.Location = Properties.Settings.Default.Position.Location;
            this.Width = Properties.Settings.Default.Position.Width;
            this.Height = Properties.Settings.Default.Position.Height;

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
            Properties.Settings.Default.Position = new Rectangle(this.Location, this.Size);
            Properties.Settings.Default.Save();

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
                    }
                    this.AcceptButton = null;
                    break;
                default:
                    frTxBx_Notes.Select();
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
            Reset();
            Delay.Stop();

            if (TxBx_Search.Text == string.Empty)
            {
                if (LV_Entries.Items.Count != lData.Count) UpdateLV();
                TxBx_Search.Width += 30;
                Btn_Clear.Visible = false;
            }
            else
            {
                if (!Btn_Clear.Visible)
                {
                    TxBx_Search.Width -= 30;
                    Btn_Clear.Visible = true;
                }
                Delay.Start();
            }
        }

        /* Clear searchbar  */
        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            TxBx_Search.Focus();
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

        /* Give listview priority whenever mouse enters it */
        private void LV_Entries_MouseEnter(object sender, EventArgs e)
        {
            if (!LV_Entries.Focused)
                LV_Entries.Focus();
        }

        /* Updates LV to only display favourited items */
        private void ChkBx_ShowFav_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkBx_ShowFav.Checked) OnlyFavs();
            else UpdateLV();

            if (indx != -1 && lData[indx].bFav) ReFocus();
            else Reset();

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
            string sPath;
            if (TxBx_Loc.Text != "" && Directory.Exists(TxBx_Loc.Text)) sPath = TxBx_Loc.Text;
            else
            {
                sPath = string.Format("{0}\\[{1}] {2}", Properties.Settings.Default.DefLoc,
                    CmbBx_Artist.Text, TxBx_Title.Text);
                if (!Directory.Exists(sPath)) sPath = Properties.Settings.Default.DefLoc;
            }
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

            /* Just in case user deletes folder midway */
            if (Directory.Exists(TxBx_Loc.Text) &&
                ExtDirectory.GetFiles(@TxBx_Loc.Text,
                    SearchOption.TopDirectoryOnly).Length > 0)
            {
                Browse_Img fmBrowse = new Browse_Img();
                fmBrowse.sPath = TxBx_Loc.Text;
                fmBrowse.iPage = iPage;

                fmBrowse.ShowDialog();
                iPage = (short)fmBrowse.iPage;
                fmBrowse.Dispose();
                GC.Collect();
            }
            else MessageBox.Show("The folder no longer exists.",
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (LV_Entries.Items.Count == 0) return;
            int iPos = 0;

            if (LV_Entries.SelectedItems.Count != 0)
            {
                iPos = LV_Entries.SelectedItems[0].Index;
                if (++iPos >= LV_Entries.Items.Count) iPos = 0;
            }

            ScrollTo(iPos);
        }

        /* Select previous item in listview */
        private void Btn_GoUp_Click(object sender, EventArgs e)
        {
            if (LV_Entries.Items.Count == 0) return;
            int iPos = LV_Entries.Items.Count - 1;

            if (LV_Entries.SelectedItems.Count != 0)
            {
                iPos = LV_Entries.SelectedItems[0].Index;
                if (--iPos < 0) iPos = LV_Entries.Items.Count - 1;
            }

            ScrollTo(iPos);
        }

        /* Selects a random doujin */
        private void Btn_Rand_Click(object sender, EventArgs e)
        {
            if (LV_Entries.Items.Count == 0) return;

            Random rnd = new Random();
            ScrollTo(rnd.Next(LV_Entries.Items.Count));
        }

        /* Move TxBx_Tags cursor pos. based on ScrTags value */
        #region ScrollTags
        private void ScrTags_Scroll(object sender, ScrollEventArgs e)
        {
            TxBx_Tags.Select(ScrTags.Value, 0);
            TxBx_Tags.ScrollToCaret();
        }

        private void TxBx_Tags_Leave(object sender, EventArgs e)
        { ScrTags.Visible = false; }

        private void TxBx_Tags_Click(object sender, EventArgs e)
        { SetScroll(); }

        private void SetScroll()
        {
            Size s = TextRenderer.MeasureText(TxBx_Tags.Text, TxBx_Tags.Font);
            if (s.Width > TxBx_Tags.Width)
            {
                ScrTags.Maximum = s.Width / 5;
                ScrTags.Value = TxBx_Tags.SelectionStart;
                ScrTags.Visible = true;
            }
            else ScrTags.Visible = false;
        }
        #endregion

        /* Only enable edit when changes have been made */
        #region EnableEdit
        private void EntryAlt_Text(object sender, EventArgs e)
        {
            if (indx != -1) MnTS_Edit.Visible = true;

            if (TxBx_Tags.ContainsFocus) SetScroll();
        }

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

            ReFocus();
            LV_Entries.Select();
        }

        /* Inserts text at specified point  */
        private void AddText_Rich(string sAdd, FixedRichTextBox frTxBx)
        {
            int iNewStart = frTxBx.SelectionStart + sAdd.Length;
            frTxBx.Text = frTxBx.Text.Insert(
                frTxBx.SelectionStart, sAdd);
            frTxBx.SelectionStart = iNewStart;
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

        /* Find list index of currently selected item */
        void GetIndex(Object obj)
        {
            ListViewItem lvi = (ListViewItem)obj;
            string sMatch = lvi.SubItems[0].Text + lvi.SubItems[1].Text;

            for (int i = 0; i < lData.Count; i++)
                if (sMatch == lData[i].sArtist + lData[i].sTitle)
                {
                    this.BeginInvoke(new DelVoidInt(SetData), i);
                    break;
                }
        }

        /* Used to simulate JS Object Literal for JSON 
           Inspired by Hupotronics' ExLinks   */
        public static string JSON(string sURL)
        {
            string[] asChunk = sURL.Split('/');
            if (asChunk.Length == 7)
                return string.Format(
                    "{{\"method\":\"gdata\",\"gidlist\":[[{0},\"{1}\"]]}}",
                    asChunk[4], asChunk[5]);
            else return string.Empty;
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
            Text = "Returned: " + LV_Entries.Items.Count + " entries";
            Cursor = Cursors.Default;

            //prevent loss of search parameters
            if (TxBx_Search.Text != string.Empty) Search();
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

        /* Bring focus back to selected entry in LV */
        private void ReFocus(int iPos = 0)
        {
            if (LV_Entries.Items.Count == 0 || iPos < 0) return;

            if (indx != -1 && iPos == 0)
                for (int i = 0; i < LV_Entries.Items.Count; i++)
                    if (LV_Entries.Items[i].SubItems[1].Text == lData[indx].sTitle)
                    {
                        iPos = i;
                        break;
                    }

            ScrollTo(iPos);
        }

        /* Set properties back to default   */
        void Reset()
        {
            //reset Form title
            Text = (TxBx_Search.Text == string.Empty && !ChkBx_ShowFav.Checked ? "Manga Organizer: "
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
            ScrTags.Visible = false;
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
            if (sPath != string.Empty && Directory.Exists(sPath))
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

        private void ScrollTo(int iPos)
        {
            LV_Entries.FocusedItem = LV_Entries.Items[iPos];
            LV_Entries.Items[iPos].Selected = true;

            /* Compensate for broken scroll-to function
             * by running it multiple times (3 is sweet-spot) */
            LV_Entries.TopItem = LV_Entries.Items[iPos];
            LV_Entries.TopItem = LV_Entries.Items[iPos];
            LV_Entries.TopItem = LV_Entries.Items[iPos];
        }

        /* Search database entries   */
        void Search(ListViewItem lvi = null)
        {
            Cursor = Cursors.WaitCursor;

            //process filters into usable format
            string[] sTags = TxBx_Search.Text.Split(' ');
            Dictionary<string, bool> dtFilter = new Dictionary<string, bool>(sTags.Length);
            for (int x = 0; x < sTags.Length; x++)
            {
                if (string.IsNullOrEmpty(sTags[x]) || sTags[x] == "-") continue;
                sTags[x] = sTags[x].Replace('_', ' ');
                if (!sTags[x].StartsWith("-"))
                {
                    if (!dtFilter.ContainsKey(sTags[x]))
                        dtFilter.Add(sTags[x], true);
                }
                else
                {
                    sTags[x] = sTags[x].Substring(1, sTags[x].Length - 1);
                    if (!dtFilter.ContainsKey(sTags[x]))
                        dtFilter.Add(sTags[x], false);
                }
            }

            //search by tags, title, artist, and type
            LV_Entries.BeginUpdate();
            if (lvi == null)
                for (int x = 0; x < LV_Entries.Items.Count; x++)
                    foreach (KeyValuePair<string, bool> kvp in dtFilter)
                    {
                        if (ExtString.Contains(LV_Entries.Items[x].SubItems[3].Text, kvp.Key) ||
                            ExtString.Contains(LV_Entries.Items[x].SubItems[1].Text, kvp.Key) ||
                            ExtString.Contains(LV_Entries.Items[x].SubItems[0].Text, kvp.Key) ||
                            ExtString.Contains(LV_Entries.Items[x].SubItems[4].Text, kvp.Key))
                        {
                            if (!kvp.Value) { LV_Entries.Items.RemoveAt(x--); break; }
                        }
                        else if (kvp.Value) { LV_Entries.Items.RemoveAt(x--); break; }
                    }
            else
                foreach (KeyValuePair<string, bool> kvp in dtFilter)
                {
                    if (ExtString.Contains(lvi.SubItems[3].Text, kvp.Key) ||
                        ExtString.Contains(lvi.SubItems[1].Text, kvp.Key) ||
                        ExtString.Contains(lvi.SubItems[0].Text, kvp.Key) ||
                        ExtString.Contains(lvi.SubItems[4].Text, kvp.Key))
                    {
                        if (!kvp.Value) { lvi.Remove(); Reset(); break; }
                    }
                    else if (kvp.Value) { lvi.Remove(); Reset(); break; }
                }
            LV_Entries.EndUpdate();

            Text = "Returned: " + LV_Entries.Items.Count + " entries";
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
            MnTS_Del.Visible = true;
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

            try
            {
                using (var bmpTemp = new Bitmap(obj as string))
                    PicBx_Cover.Image = new Bitmap(bmpTemp);
            }
            catch
            {
                MessageBox.Show("Error: Image may be corrupt",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Release old image resource */
        void SetPicBxNull()
        {
            if (PicBx_Cover.Image == null) return;
            PicBx_Cover.Image.Dispose();
            PicBx_Cover.Image = null;
            MnTS_Open.Visible = false;

            //force garbage collection
            GC.Collect();
        }
        #endregion

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
            if (sName.Length >= 2 && CmbBx_Artist.Text == "" && TxBx_Title.Text == "")
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
            ListViewItem[] aItems = new ListViewItem[lData.Count];

            //refresh LV_Entries
            for (int i = 0; i < lData.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                if (lData[i].bFav) lvi.BackColor = Color.LightYellow;
                lvi.SubItems.Add(lData[i].sTitle);
                lvi.SubItems.Add(lData[i].iPages.ToString());
                lvi.SubItems.Add(lData[i].sTags);
                lvi.SubItems.Add(lData[i].sType);
                aItems[i] = lvi;
            }

            //Update listview display
            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(aItems);
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

                    //add artist to autocomplete
                    if (!CmbBx_Artist.Items.Contains(CmbBx_Artist.Text))
                    {
                        CmbBx_Artist.AutoCompleteCustomSource.Add(CmbBx_Artist.Text);
                        CmbBx_Artist.Items.Add(CmbBx_Artist.Text);
                    }

                    //update LV_Entries & maintain scroll position
                    int iPos = LV_Entries.TopItem == null ? -1 : LV_Entries.TopItem.Index;
                    AddEntries();
                    Reset();
                    ReFocus(iPos);
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
            Text = "Edited entry: " + lData[indx].ToString();
            TxBx_Tags.Text = lData[indx].sTags;

            //update LV_Entries & maintain selection
            ListViewItem lvi = LV_Entries.Items[LV_Entries.SelectedItems[0].Index];
            if (lData[indx].bFav) lvi.BackColor = Color.LightYellow;
            else lvi.BackColor = SystemColors.Window;
            lvi.SubItems[0].Text = lData[indx].sArtist;
            lvi.SubItems[1].Text = lData[indx].sTitle;
            lvi.SubItems[2].Text = lData[indx].iPages.ToString();
            lvi.SubItems[3].Text = lData[indx].sTags;
            lvi.SubItems[4].Text = lData[indx].sType;
            LV_Entries.Sort();

            if (ChkBx_ShowFav.Checked && !lData[indx].bFav)
            {
                lvi.Remove();
                Reset();
            }
            else if (TxBx_Search.Text != "")
            {
                //check if still included
                Search(lvi);
            }
            else ReFocus();

            bSavList = false;
            MnTS_Edit.Visible = false;
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
            if (TxBx_Loc.Text != string.Empty && !ExtDirectory.Restricted(TxBx_Loc.Text))
                System.Diagnostics.Process.Start(TxBx_Loc.Text);
        }

        /* Display info about program, license & author */
        private void MnTS_About_Click(object sender, EventArgs e)
        {
            About fmAbout = new About();
            fmAbout.Show();
        }

        /* Display stats on tags */
        private void MnTS_Stats_Click(object sender, EventArgs e)
        {
            Stats fmStats = new Stats();
            fmStats.CurrentItems = lData;
            fmStats.Show();

            if (fmStats.DialogResult == DialogResult.OK)
                fmStats.Dispose();
        }

        /* Grabs entry data from passed URL */
        private void MnTS_GET_Click(object sender, EventArgs e)
        {
            GetUrl fmGet = new GetUrl();
            fmGet.ShowDialog();

            //process url
            if (fmGet.DialogResult == DialogResult.OK)
            {
                string[] asResp;
                this.Cursor = Cursors.WaitCursor;
                Text = "Sending request...";

                try
                {
                    //send formatted request to EH API
                    System.Net.ServicePointManager.DefaultConnectionLimit = 64;
                    System.Net.HttpWebRequest rq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://g.e-hentai.org/api.php");
                    rq.ContentType = "application/json";
                    rq.Method = "POST";
                    rq.Timeout = 5000;
                    rq.KeepAlive = false;

                    using (Stream s = rq.GetRequestStream())
                    {
                        byte[] byContent = System.Text.Encoding.ASCII.GetBytes(JSON(fmGet.Url));
                        s.Write(byContent, 0, byContent.Length);
                    }
                    using (StreamReader sr = new StreamReader(((System.Net.HttpWebResponse)rq.GetResponse()).GetResponseStream()))
                    {
                        //recieve metadata
                        Text = "Downloading page...";
                        asResp = sr.ReadToEnd().Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                        rq.Abort();
                    }

                    //parse metadata
                    Text = "Parsing metadata...";
                    SplitTitle(ExtString.DecodeNonAscii(System.Net.WebUtility.HtmlDecode(asResp[2].Split(':')[1].Substring(1))));
                    CmbBx_Type.Text = asResp[4].Split(':')[1].Substring(1);
                    frTxBx_Notes.Text = asResp[7] + '\n' + asResp[8];
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    Dt_Date.Value = dt.AddSeconds((long)Convert.ToDouble(asResp[7].Split(':')[1].Substring(1)));
                    Nud_Pages.Value = Convert.ToInt32(asResp[8].Split(':')[1].Substring(1));

                    for (int i = 11; i < asResp.Length; i++)
                    {
                        if (i == 11)
                            TxBx_Tags.Text += asResp[i].Split(':')[1].Substring(2) + ", ";
                        else if (i == asResp.Length - 1)
                            TxBx_Tags.Text += asResp[i].Substring(0, asResp[i].Length - 5);
                        else TxBx_Tags.Text += asResp[i] + ", ";
                    }
                }
                catch
                {
                    MessageBox.Show("URL was invalid or the connection timed out.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Text = "Finished";
                    this.Cursor = Cursors.Default;
                }
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
            if (TabControl.SelectedIndex == 2)
            {
                frTxBx_Notes.Cut();
                bSavText = false;
            }
            else frTxBx_Desc.Cut();
        }

        private void MnRTx_Copy_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.Copy();
            else frTxBx_Desc.Copy();
        }

        private void MnRTx_Paste_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 2)
            {
                AddText_Rich(Clipboard.GetText(), frTxBx_Notes);
                bSavText = false;
            }
            else AddText_Rich(Clipboard.GetText(), frTxBx_Desc);
        }

        private void MnRTx_SelectAll_Click(object sender, EventArgs e)
        {
            if (TabControl.SelectedIndex == 2) frTxBx_Notes.SelectAll();
            else frTxBx_Desc.SelectAll();
        }

        /* Prevent mixed font types when c/p  */
        private void frTxBx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                AddText_Rich(Clipboard.GetText(), (FixedRichTextBox)sender);
                e.Handled = true;
            }
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
                if ((ActiveControl as ComboBox).SelectedText == "") return;
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
                    AddText_Rich(sAdd, (FixedRichTextBox)sender);
                    break;
                case "TxBx_Title":
                    if (sAdd.Contains('[')) SplitTitle(sAdd);
                    else
                    {
                        iNewStart = TxBx_Title.SelectionStart + sAdd.Length;
                        TxBx_Title.Text = TxBx_Title.Text.Insert(
                            TxBx_Title.SelectionStart, sAdd);
                        TxBx_Title.SelectionStart = iNewStart;
                    }
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
                    if (sAdd.Contains('[')) SplitTitle(sAdd);
                    else
                    {
                        iNewStart = CmbBx_Artist.SelectionStart + sAdd.Length;
                        CmbBx_Artist.Text = CmbBx_Artist.Text.Insert(
                            CmbBx_Artist.SelectionStart, sAdd);
                        CmbBx_Artist.SelectionStart = iNewStart;
                    }
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
            string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
            short iTrack = indx;

            //remove restricted folders
            string sError = string.Empty;
            for (int i = 0; i < asDir.Length; i++)
                if (ExtDirectory.Restricted(asDir[i]))
                {
                    sError += asDir[i] + '\n';
                    asDir[i] = string.Empty;
                    continue;
                }
            if (sError != string.Empty)
            {
                MessageBox.Show("Access to the following folder(s) is restricted:\n" + sError,
                        "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sError = string.Empty;
            }

            //add all remaining folders
            for (int i = 0; i < asDir.Length; i++)
            {
                if (asDir[i] == string.Empty) continue;

                //Get formatted directory name
                string[] sTitle = Path.GetFileName(asDir[i]).Split(
                    new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                //add item
                Main.stEntry en;
                if (sTitle.Length == 2)
                    en = new Main.stEntry(sTitle[1].TrimStart(), sTitle[0], asDir[i], "", "", "",
                        DateTime.Now, ExtDirectory.GetFiles(asDir[i]).Length, false);
                else
                    en = new Main.stEntry(sTitle[0].TrimStart(), "", asDir[i], "", "", "",
                        DateTime.Now, ExtDirectory.GetFiles(asDir[i]).Length, false);

                if (!lData.Contains(en))
                {
                    lData.Add(en);
                    indx = (short)(lData.Count - 1);

                    if (en.sArtist != "" && !CmbBx_Artist.Items.Contains(en.sArtist))
                    {
                        CmbBx_Artist.AutoCompleteCustomSource.Add(en.sArtist);
                        CmbBx_Artist.Items.Add(en.sArtist);
                    }
                }
                else sError += asDir[i] + '\n';
            }
            if (sError != string.Empty)
            {
                MessageBox.Show("The following path(s) already exists in the database:\n" + sError,
                    "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Update LV
            if (iTrack != indx)
                AddEntries();
        }
        #endregion
    }
}