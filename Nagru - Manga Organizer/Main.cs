/*
 * Author: Taylor Napier
 * Date: Aug15, 2012
 * Desc: Handles organization of manga library
 */

#if DEBUG
#warning DEBUG version
#endif

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
        List<stEntry> lData = new List<stEntry>(500);
        LVsorter lvSortObj = new LVsorter();
        bool bSavList = true, bSavText = true;
        short indx = -1, iPage = -1;

        /* Holds manga metadata */
        [Serializable]
        public class stEntry : ISerializable
        {
            public string sTitle, sArtist,
                sLoc, sType, sDesc, sTags;
            public DateTime dtDate;
            public ushort iPages;
            public bool bFav;

            public stEntry(string _Title, string _Artist, string _Loc, string _Desc,
                string _Tags, string _Type, DateTime _Date, decimal _Pages, bool _Fav)
            {
                sTitle = _Title;
                sArtist = _Artist;
                sLoc = _Loc;
                sDesc = _Desc;
                dtDate = _Date;
                iPages = (ushort)_Pages;
                sType = _Type;
                bFav = _Fav;
                
                //trim, clean, and format tags
                sTags = "";
                if (_Tags == string.Empty) return;
                string[] sRaw = _Tags.Split(',').Select(
                    x => x.Trim()).Distinct().ToArray<string>();
                Array.Sort(sRaw);
                sTags = String.Join(", ", sRaw);
            }
            public stEntry(string _Path)
            {
                string sRaw = Path.GetFileNameWithoutExtension(_Path);

                //Get formatted title
                if (sRaw.StartsWith("(")) {
                    int iPos = sRaw.IndexOf(')') + 2;
                    if (sRaw.Length - 1 >= iPos)
                        sRaw = sRaw.Remove(0, iPos);
                }
                string[] sName = ExtString.Split(sRaw, "[", "]");

                //parse it out
                if (sName.Length >= 2) {
                    sArtist = sName[0].Trim();
                    sTitle = sName[1].Trim();
                }
                else {
                    sTitle = sRaw;
                    sArtist = "";
                }

                sLoc = _Path;
                sType = "Manga";
                sDesc = "";
                sTags = "";
                dtDate = DateTime.Now;
                iPages = (ushort)ExtDir.GetFiles(_Path).Length;
                bFav = false;
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
                stEntry en = obj as stEntry;
                if (obj != null)
                    return (en.sArtist + en.sTitle).Equals(
                        sArtist + sTitle, StringComparison.OrdinalIgnoreCase);
                else return false;
            }

            /* 'Disable' hashtable */
            public override int GetHashCode() { return 1; }

            /* custom serialization to save datatypes manually */
            protected stEntry(SerializationInfo info, StreamingContext ctxt)
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
            public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
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

        /* Search term processing */
        private class stTerm
        {
            readonly bool bAllow;
            readonly string sType;
            readonly string sTerm;

            public stTerm(string _Raw)
            {
                string[] sSplit = _Raw.Trim().Split(':');
                if (sSplit.Length == 2 && sSplit[1] != "") {
                    sType = sSplit[0];
                    sTerm = sSplit[1];
                }
                else sTerm = _Raw;

                sTerm = sTerm.Replace('_', ' ');
                if (sTerm.StartsWith("-")) {
                    sTerm = sTerm.Substring(1, sTerm.Length - 1);
                    bAllow = false;
                }
                else bAllow = true;
            }

            public bool Equals(stEntry en)
            {
                bool bMatch = false;
                switch (sType)
                {
                    case "artist":
                        bMatch = ExtString.Contains(en.sArtist, sTerm);
                        break;
                    case "title":
                        bMatch = ExtString.Contains(en.sTitle, sTerm);
                        break;
                    case "tags":
                        bMatch = ExtString.Contains(en.sTags, sTerm);
                        break;
                    case "desc":
                        bMatch = ExtString.Contains(en.sDesc, sTerm);
                        break;
                    case "type":
                        bMatch = ExtString.Contains(en.sType, sTerm);
                        break;
                    case "date":
                        DateTime dt = new DateTime();
                        char c = sTerm[0];

                        switch (c)
                        {
                            case '<':
                                if (DateTime.TryParse(sTerm.Substring(1), out dt))
                                    bMatch = dt >= en.dtDate;
                                break;
                            case '>':
                                if (DateTime.TryParse(sTerm.Substring(1), out dt))
                                    bMatch = dt <= en.dtDate;
                                break;
                            default:
                                if (DateTime.TryParse(sTerm, out dt))
                                    bMatch = dt == en.dtDate;
                                break;
                        }
                        break;
                    case "pages":
                        c = sTerm[0];
                        int i;

                        switch (c)
                        {
                            case '<':
                                if (int.TryParse(sTerm.Substring(1), out i))
                                    bMatch = en.iPages <= i;
                                break;
                            case '>':
                                if (int.TryParse(sTerm.Substring(1), out i))
                                    bMatch = en.iPages >= i;
                                break;
                            default:
                                bMatch = en.iPages.ToString().Equals(sTerm);
                                break;
                        }
                        break;
                    default:
                        bMatch = (ExtString.Contains(en.sTags, sTerm) ||
                        ExtString.Contains(en.sTitle, sTerm) ||
                        ExtString.Contains(en.sArtist, sTerm) ||
                        ExtString.Contains(en.sDesc, sTerm) ||
                        ExtString.Contains(en.dtDate.ToString(), sTerm) ||
                        ExtString.Contains(en.sType, sTerm));
                        break;
                }

                return bAllow ? bMatch : !bMatch;
            }
        }

        #region FormMethods
        public Main(string[] sFile)
        {
            InitializeComponent();

            //if database opened with "Shell->Open with..."
            if (sFile.Length > 0 && sFile[0].EndsWith("\\MangaDatabase.bin") &&
                Properties.Settings.Default.SavLoc != 
                sFile[0].Substring(0, sFile[0].Length - 18))
            {
                Properties.Settings.Default.SavLoc = sFile[0];
                Properties.Settings.Default.Save();
                MessageBox.Show("Default database location changed to:\n\"" + sFile[0] + "\"",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //disable ContextMenu in Nud_Pages
            Nud_Pages.ContextMenuStrip = new ContextMenuStrip();
            
            LV_Entries.ListViewItemSorter = lvSortObj;
            LV_Entries_Resize(sender, e);
            LV_Entries.Select();
            
            frTxBx_Desc.AllowDrop = true;
            frTxBx_Notes.AllowDrop = true;
            frTxBx_Notes.DragDrop += new DragEventHandler(DragDropTxBx);
            frTxBx_Desc.DragDrop += new DragEventHandler(DragDropTxBx);
            frTxBx_Desc.DragEnter += new DragEventHandler(DragEnterTxBx);
            frTxBx_Notes.DragEnter += new DragEventHandler(DragEnterTxBx);
            frTxBx_Notes.Text = Properties.Settings.Default.Notes;

            this.Location = Properties.Settings.Default.Position.Location;
            this.Width = Properties.Settings.Default.Position.Width;
            this.Height = Properties.Settings.Default.Position.Height;

            //load database
            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            sPath += "\\MangaDatabase.bin";
            if (lData.Count == 0 && File.Exists(sPath))
            {
                lData = FileSerializer.Deserialize<List<stEntry>>(sPath) ?? new List<stEntry>(0);

                if (lData.Count == 0) return;
                UpdateLV();

                //set up CmbBx autocomplete
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

            if(!e.Cancel) {
                Properties.Settings.Default.Position = 
                    new Rectangle(this.Location, this.Size);
                Properties.Settings.Default.Save();
            }
        }

        /* Display image from selected folder   */
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TabControl.SelectedIndex)
            {
                case 0:
                    if (indx == -1)
                        Text = (TxBx_Search.Text == string.Empty ? "Manga Organizer: "
                            + lData.Count : "Returned: " + LV_Entries.Items.Count) + " entries";
                    this.AcceptButton = Btn_Clear;
                    LV_Entries.Focus();
                    break;
                case 1:
                    if (indx != -1) {
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
            fmScan.lCurr = lData;
            fmScan.delDone = fmScanDone;
            Btn_Scan.Enabled = false;

            fmScan.Show();
            fmScan.Select();
        }
        private void fmScanDone() { Btn_Scan.Enabled = true; }
        
        /* Inserts delay before Search() to account for Human input speed */
        private void TxBx_Search_TextChanged(object sender, EventArgs e)
        {
            Reset();
            Delay.Stop();

            if (TxBx_Search.Text == string.Empty) {
                if (LV_Entries.Items.Count != lData.Count) UpdateLV();
                TxBx_Search.Width += 30;
                Btn_Clear.Visible = false;
            }
            else {
                if (!Btn_Clear.Visible) {
                    TxBx_Search.Width -= 30;
                    Btn_Clear.Visible = true;
                }
                Delay.Start();
            }
        }
        private void Delay_Tick(object sender, EventArgs e)
        {
            Delay.Stop();
            Search();
        }

        /* Reset Tb_Browse  */
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
            if (LV_Entries.FocusedItem == null || 
                LV_Entries.SelectedItems.Count == 0) {
                Reset(); return; 
            }

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
            else lvSortObj.SwapOrder();

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

        /* Give listview priority whenever mouse hovers in it */
        private void LV_Entries_MouseHover(object sender, EventArgs e)
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
        /* Select location of manga entry   */
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
                !ExtDir.Restricted(fbd.SelectedPath))
            {
                TxBx_Loc.Text = fbd.SelectedPath;

                thWork = new System.Threading.Thread(GetImage);
                thWork.IsBackground = true;
                thWork.Start();
            }
            fbd.Dispose();
        }

        /* Open URL in default Browser  */
        private void frTxBx_Desc_LinkClicked(object sender, LinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(e.LinkText); }

        /* Alternative to MnTS_Open */
        private void PicBx_Cover_Click(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image == null) return;

            List<string> lCheck = new List<string>(50);
            lCheck.AddRange(ExtDir.GetFiles(
                TxBx_Loc.Text, SearchOption.TopDirectoryOnly));

            /* Just in case user deletes folder midway */
            if (Directory.Exists(TxBx_Loc.Text) && lCheck.Count > 0)
            {
                Browse_Img fmBrowse = new Browse_Img();
                fmBrowse.sPath = TxBx_Loc.Text;
                fmBrowse.lFiles = lCheck;
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

        /* Programmatically select item in LV_Entries  */
        private void Btn_GoDn_Click(object sender, EventArgs e)
        {
            if (LV_Entries.Items.Count == 0) return;
            int iPos = 0;

            if (LV_Entries.SelectedItems.Count != 0) {
                iPos = LV_Entries.SelectedItems[0].Index;
                if (++iPos >= LV_Entries.Items.Count) iPos = 0;
            }

            ScrollTo(iPos);
        }
        private void Btn_GoUp_Click(object sender, EventArgs e)
        {
            if (LV_Entries.Items.Count == 0) return;
            int iPos = LV_Entries.Items.Count - 1;

            if (LV_Entries.SelectedItems.Count != 0) {
                iPos = LV_Entries.SelectedItems[0].Index;
                if (--iPos < 0) iPos = LV_Entries.Items.Count - 1;
            }

            ScrollTo(iPos);
        }
        private void Btn_Rand_Click(object sender, EventArgs e)
        {
            if (LV_Entries.Items.Count == 0) return;

            int iPos = 0;
            Random rnd = new Random();
            do {
                iPos = rnd.Next(LV_Entries.Items.Count);
            }
            while (iPos == indx);
            ScrollTo(iPos);
        }

        /* Move TxBx_Tags cursor pos. based on ScrTags value */
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
            if (s.Width > TxBx_Tags.Width) {
                ScrTags.Maximum = s.Width / 5;
                ScrTags.Value = TxBx_Tags.SelectionStart;
                ScrTags.Visible = true;
            }
            else ScrTags.Visible = false;
        }

        /* Only enable edit when changes have been made */
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

        #region Tab_Notes
        /* Prevent loss of changes in note text   */
        private void frTxBx_Notes_TextChanged(object sender, EventArgs e)
        { if (bSavText) bSavText = false; }

        /* Open URL in default Browser  */
        private void frTxBx_Notes_LinkClicked(object sender, LinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(e.LinkText); }
        #endregion

        #region CustomMethods
        /* Add entries passed from fmScan to database */
        private void AddEntries()
        {
            UpdateLV();
            bSavList = false;
            LV_Entries.Select();
            if (indx > -1) ReFocus();
        }

        private void AddText_Rich(string sAdd, FixedRichTextBox frTxBx)
        {
            int iNewStart = frTxBx.SelectionStart + sAdd.Length;
            frTxBx.Text = frTxBx.Text.Insert(
                frTxBx.SelectionStart, sAdd);
            frTxBx.SelectionStart = iNewStart;
        }

        private void GetImage(Object obj)
        {
            BeginInvoke(new DelVoidVoid(SetPicBxNull));
            if (ExtDir.Restricted(TxBx_Loc.Text))
                return;

            //Get cover and filecount
            string[] sFiles = ExtDir.GetFiles(
                @TxBx_Loc.Text, SearchOption.TopDirectoryOnly);
            if (sFiles.Length > 0)
                BeginInvoke(new DelVoidString(SetPicBxImage), sFiles[0]);
            BeginInvoke(new DelVoidInt(SetNudCount), sFiles.Length);
        }

        private void GetIndex(Object obj)
        {
            ListViewItem lvi = (ListViewItem)obj;
            string sMatch = lvi.SubItems[0].Text + lvi.SubItems[1].Text;
            for (int i = 0; i < lData.Count; i++)
                if (sMatch == lData[i].sArtist + lData[i].sTitle) {
                    this.BeginInvoke(new DelVoidInt(SetData), i);
                    break;
                }
        }

        /* Used to simulate JS Object Literal for JSON 
           Inspired by Hupotronics' ExLinks   */
        private static string JSON(string sURL)
        {
            string[] asChunk = sURL.Split('/');
            if (asChunk.Length == 7)
                return string.Format(
                    "{{\"method\":\"gdata\",\"gidlist\":[[{0},\"{1}\"]]}}",
                    asChunk[4], asChunk[5]);
            else return string.Empty;
        }

        private void OnlyFavs()
        {
            Cursor = Cursors.WaitCursor;
            LV_Entries.BeginUpdate();
            for (int i = 0; i < LV_Entries.Items.Count; i++) {
                if(LV_Entries.Items[i].BackColor != Color.LightYellow) {
                    LV_Entries.Items.RemoveAt(i--);
                }
            }
            LV_Entries.EndUpdate();
            Text = "Returned: " + LV_Entries.Items.Count + " entries";
            Cursor = Cursors.Default;
        }

        private void OpenFile()
        {
            if (TxBx_Loc.Text == string.Empty ||
                ExtDir.Restricted(TxBx_Loc.Text))
                return;

            string[] sFiles = ExtDir.GetFiles(@TxBx_Loc.Text);
            if (sFiles.Length > 0) System.Diagnostics.Process.Start(sFiles[0]);
            else System.Diagnostics.Process.Start(@TxBx_Loc.Text);
        }

        private void ReFocus(int iPos = 0)
        {
            if (LV_Entries.Items.Count == 0 || iPos < 0) return;

            if (indx != -1 && iPos == 0)
            {
                for (int i = 0; i < LV_Entries.Items.Count; i++)
                    if (LV_Entries.Items[i].SubItems[1].Text == lData[indx].sTitle)
                    {
                        iPos = i;
                        break;
                    }
            }

            ScrollTo(iPos);
        }

        private void Reset()
        {
            //reset Form title
            Text = (TxBx_Search.Text == string.Empty && 
                !ChkBx_ShowFav.Checked ? "Manga Organizer: " + lData.Count : 
                "Returned: " + LV_Entries.Items.Count) + " entries";

            //Tb_Browse
            LV_Entries.FocusedItem = null;
            LV_Entries.SelectedItems.Clear();
            iPage = -1;
            indx = -1;

            //Tb_View
            TxBx_Loc.Clear();
            TxBx_Tags.Clear();
            TxBx_Title.Clear();
            frTxBx_Desc.Clear();
            Nud_Pages.Value = 0;
            CmbBx_Artist.Text = "";
            CmbBx_Type.Text = "Manga";
            Dt_Date.Value = DateTime.Now;
            ChkBx_Fav.Checked = false;
            ScrTags.Visible = false;
            SetPicBxNull();

            //Mn_EntryOps
            MnTS_New.Visible = true;
            MnTS_Del.Visible = false;
            MnTS_Edit.Visible = false;
            MnTS_Open.Visible = false;
        }

        private void SaveData()
        {
            Cursor = Cursors.WaitCursor;
            Text = "Saving...";

            //grab filepath
            string sPath = Properties.Settings.Default.SavLoc;
            if (sPath != string.Empty && Directory.Exists(sPath))
                sPath += "\\MangaDatabase.bin";
            else sPath = Environment.CurrentDirectory + "\\MangaDatabase.bin";

            if (!bSavList) {
                if (File.Exists(sPath)) File.Delete(sPath);
                FileSerializer.Serialize(sPath, lData);
                bSavList = true;
            }

            if (!bSavText) {
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

        /* Filter lvi by search terms (Check MnTs_Edit after changes)  */
        private void Search()
        {
            Cursor = Cursors.WaitCursor;
            string[] sTags = TxBx_Search.Text.Split(' ');
            List<stTerm> lTerms = new List<stTerm>(5);
            List<ListViewItem> lItems = new List<ListViewItem>(lData.Count);

            for (int i = 0; i < sTags.Length; i++)
                lTerms.Add(new stTerm(sTags[i]));

            for (int x = 0; x < lData.Count; x++) {
                bool b = true;
                for (int y = 0; y < lTerms.Count; y++) {
                    if (!lTerms[y].Equals(lData[x])) {
                        b = false;
                        break;
                    }
                }
                if (!b) continue;

                ListViewItem lvi = new ListViewItem(lData[x].sArtist);
                if (lData[x].bFav) lvi.BackColor = Color.LightYellow;
                lvi.SubItems.AddRange(new string[] {
                        lData[x].sTitle,
                        lData[x].iPages.ToString(),
                        lData[x].sTags,
                        lData[x].sType
                    });
                lItems.Add(lvi);
            }

            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(lItems.ToArray());
            LV_Entries.Sort();
            LV_Entries.EndUpdate();
            Text = "Returned: " + LV_Entries.Items.Count + " entries";
            Cursor = Cursors.Default;

            //prevent loss of other parameters
            if (ChkBx_ShowFav.Checked) OnlyFavs();
        }

        private void SetData(int iNewIndx)
        {
            if ((indx = (short)iNewIndx) == -1)
            { Reset(); return; }
            
            Tb_View.SuspendLayout();
            Text = "Selected: " + lData[indx].ToString();
            TxBx_Title.Text = lData[indx].sTitle;
            CmbBx_Artist.Text = lData[indx].sArtist;
            TxBx_Loc.Text = lData[indx].sLoc;
            frTxBx_Desc.Text = lData[indx].sDesc;
            CmbBx_Type.Text = lData[indx].sType;
            Dt_Date.Value = lData[indx].dtDate;
            ChkBx_Fav.Checked = lData[indx].bFav;
            Nud_Pages.Value = lData[indx].iPages;
            TxBx_Tags.Text = lData[indx].sTags;
            
            MnTS_New.Visible = false;
            MnTS_Edit.Visible = false;
            MnTS_Del.Visible = true;
            Tb_View.ResumeLayout();
        }

        private void SetNudCount(int iNum)
        {
            Nud_Pages.Value = iNum;
            TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
        }

        private void SetPicBxImage(Object obj)
        {
            MnTS_Open.Visible = true;

            try {
                using (var bmpTemp = new Bitmap(obj as string))
                    PicBx_Cover.Image = new Bitmap(bmpTemp);
            }
            catch {
                MessageBox.Show("The following image could not be loaded:\n" + obj as string,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetPicBxNull()
        {
            if (PicBx_Cover.Image == null) return;
            PicBx_Cover.Image.Dispose();
            PicBx_Cover.Image = null;
            MnTS_Open.Visible = false;

            GC.Collect();
        }

        private void SplitTitle(string sRaw)
        {
            if (sRaw.StartsWith("(")) {
                int iPos = sRaw.IndexOf(')') + 2;
                if (sRaw.Length - 1 >= iPos)
                    sRaw = sRaw.Remove(0, iPos);
            }
            string[] sName = ExtString.Split(sRaw, "[", "]");

            if (sName.Length >= 2 && CmbBx_Artist.Text == "") {
                CmbBx_Artist.Text = sName[0].Trim();
                TxBx_Title.Text = sName[1].Trim();
            }
            else {
                int iNewStart = CmbBx_Artist.SelectionStart + sRaw.Length;
                CmbBx_Artist.Text = CmbBx_Artist.Text.Insert(
                    CmbBx_Artist.SelectionStart, sRaw);
                CmbBx_Artist.SelectionStart = iNewStart;
            }
        }

        private void UpdateLV()
        {
            if (TxBx_Search.Text != string.Empty) { 
                Search(); 
                return;
            }

            Cursor = Cursors.WaitCursor;
            Text = "Manga Organizer: " + lData.Count + " entries";
            ListViewItem[] aItems = new ListViewItem[lData.Count];

            for (int i = 0; i < lData.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                if (lData[i].bFav) lvi.BackColor = Color.LightYellow;
                lvi.SubItems.AddRange(new string[] {
                    lData[i].sTitle,
                    lData[i].iPages.ToString(),
                    lData[i].sTags,
                    lData[i].sType
                });
                aItems[i] = lvi;
            }

            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(aItems);
            LV_Entries.Sort();
            LV_Entries.EndUpdate();
            Cursor = Cursors.Default;

            //prevent loss of other parameters
            if (ChkBx_ShowFav.Checked) OnlyFavs();
        }
        #endregion

        #region Menu_EntryOps
        private void MnTS_CopyTitle_Click(object sender, EventArgs e)
        {
            if (TxBx_Title.Text == string.Empty || CmbBx_Artist.Text == string.Empty) return;
            Clipboard.SetText(string.Format("[{0}] {1}", CmbBx_Artist.Text, TxBx_Title.Text));
            Text = "Name copied to clipboard";
        }

        private void MnTS_DefLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Properties.Settings.Default.DefLoc;

            if (fbd.ShowDialog() == DialogResult.OK 
                && !ExtDir.Restricted(fbd.SelectedPath))
            {
                Properties.Settings.Default.DefLoc = fbd.SelectedPath;
                Properties.Settings.Default.Save();
                Text = "Default location changed";
            }
            fbd.Dispose();
        }

        private void MnTS_Clear_Click(object sender, EventArgs e)
        { Reset(); }

        private void MnTS_New_Click(object sender, EventArgs e)
        {
            //reject when title is unfilled
            if (TxBx_Title.Text == string.Empty) {
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
                    if (!CmbBx_Artist.Items.Contains(CmbBx_Artist.Text)) {
                        CmbBx_Artist.AutoCompleteCustomSource.Add(CmbBx_Artist.Text);
                        CmbBx_Artist.Items.Add(CmbBx_Artist.Text);
                    }

                    //update LV_Entries & maintain scroll position
                    int iPos = LV_Entries.TopItem == null ? -1 : LV_Entries.TopItem.Index;
                    AddEntries();
                    Reset();
                }
            }
            else MessageBox.Show("This item already exists in the database.",
                "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MnTS_Edit_Click(object sender, EventArgs e)
        {
            //overwrite entry properties
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

            if (ChkBx_ShowFav.Checked && !lData[indx].bFav) {
                lvi.Remove();
                Reset();
            }
            else if (TxBx_Search.Text != "")
            {
                string[] sTags = TxBx_Search.Text.Split(' ');
                List<stTerm> lTerms = new List<stTerm>(5);

                for (int i = 0; i < sTags.Length; i++)
                    lTerms.Add(new stTerm(sTags[i]));

                for (int y = 0; y < lTerms.Count; y++) {
                    if (!lTerms[y].Equals(lData[indx])){
                        lvi.Remove();
                        break;
                    }
                }
            }
            else ReFocus();

            bSavList = false;
            MnTS_Edit.Visible = false;
        }

        private void MnTS_Delete_Click(object sender, EventArgs e)
        {
            //ensure deletion is intentional
            DialogResult dResult = MessageBox.Show("Do you want to delete \"" +
                lData[indx] + "\"s folder as well?", "Manga Organizer",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            //prevent user trying to delete locked folder
            if (dResult == DialogResult.Yes)
            {
                if (ExtDir.Restricted(lData[indx].sLoc))
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

            if (dResult != DialogResult.Cancel)
            {
                if (dResult == DialogResult.Yes) {
                    this.Cursor = Cursors.WaitCursor;
                    ExtDir.Delete(lData[indx].sLoc);
                    this.Cursor = Cursors.Default;
                }

                //remove from database
                lData.RemoveAt(indx);
                LV_Entries.Items.Remove(LV_Entries.FocusedItem);
                bSavList = false;
            }
        }

        private void MnTS_Open_Click(object sender, EventArgs e)
        { if (PicBx_Cover.Image != null) OpenFile(); }

        private void MnTS_Save_Click(object sender, EventArgs e)
        { SaveData(); }

        private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
        {
            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                sPath = Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;

            if (Directory.Exists(sPath))
                System.Diagnostics.Process.Start(sPath);
            else MessageBox.Show("This directory no longer exists.", "Manga Organizer",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
        }

        private void MnTS_SavLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                sPath = Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK && !ExtDir.Restricted(fbd.SelectedPath))
            {
                sPath += "\\MangaDatabase.bin";
                Properties.Settings.Default.SavLoc = fbd.SelectedPath;
                fbd.SelectedPath += "\\MangaDatabase.bin";

                //move old save to new location
                if (File.Exists(sPath))
                    File.Move(sPath, fbd.SelectedPath);
                else if (File.Exists(fbd.SelectedPath))
                {
                    if (lData.Count > 0 && MessageBox.Show("Open existing database at:\n" + fbd.SelectedPath, 
                        "Manga Organizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        lData = FileSerializer.Deserialize<List<stEntry>>(sPath) ?? new List<stEntry>(0);

                        if (lData.Count > 0)
                        {
                            UpdateLV();

                            //set up CmbBx autocomplete
                            List<string> lAuto = new List<string>(lData.Count);
                            for (int i = 0; i < lData.Count; i++) lAuto.Add(lData[i].sArtist);
                            lAuto.Sort(new TrueCompare());
                            string[] sFinal = lAuto.Distinct().ToArray();
                            CmbBx_Artist.AutoCompleteCustomSource.AddRange(sFinal);
                            CmbBx_Artist.Items.AddRange(sFinal);
                        }
                    }
                }

                Properties.Settings.Default.Save();
                Text = "Default save location changed";
            }
            fbd.Dispose();
        }

        private void MnTS_OpenEntryFolder_Click(object sender, EventArgs e)
        {
            if (TxBx_Loc.Text != string.Empty && !ExtDir.Restricted(TxBx_Loc.Text))
                System.Diagnostics.Process.Start(TxBx_Loc.Text);
        }

        private void MnTS_About_Click(object sender, EventArgs e)
        {
            About fmAbout = new About();
            fmAbout.Show();

            if (fmAbout.DialogResult == DialogResult.OK)
                fmAbout.Dispose();
        }

        private void MnTS_Stats_Click(object sender, EventArgs e)
        {
            Stats fmStats = new Stats();
            fmStats.lCurr = lData;
            fmStats.Show();
            
            if (fmStats.DialogResult == DialogResult.OK)
                fmStats.Dispose();
        }

        /* Uses EH API to get metadata from gallery URL */
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

                try {
                    //send formatted request to EH API
                    System.Net.ServicePointManager.DefaultConnectionLimit = 64;
                    System.Net.HttpWebRequest rq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://g.e-hentai.org/api.php");
                    rq.ContentType = "application/json";
                    rq.Method = "POST";
                    rq.Timeout = 5000;
                    rq.KeepAlive = false;
                    rq.Proxy = null;

                    using (Stream s = rq.GetRequestStream()) {
                        byte[] byContent = System.Text.Encoding.ASCII.GetBytes(JSON(fmGet.Url));
                        s.Write(byContent, 0, byContent.Length);
                    }
                    using (StreamReader sr = new StreamReader(((
                        System.Net.HttpWebResponse)rq.GetResponse()).GetResponseStream())) {
                        Text = "Downloading page...";
                        asResp = ExtString.Split(sr.ReadToEnd(), "\",\"");
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

                    asResp[11] = asResp[11].Split(':')[1].Substring(2);
                    TxBx_Tags.Text = string.Join(", ", asResp, 11, asResp.Length - 11);
                    TxBx_Tags.Text = TxBx_Tags.Text.Substring(0, TxBx_Tags.Text.Length - 5);
                }
                catch {
                    MessageBox.Show("URL was invalid or the connection timed out.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally {
                    Text = "Finished";
                    this.Cursor = Cursors.Default;
                }
            }
            fmGet.Dispose();
        }

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
            if (TabControl.SelectedIndex == 2) {
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
            if (TabControl.SelectedIndex == 2) {
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
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
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
            else {
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
                        if (!sTags[i].EndsWith("\r") && !sTags[i].EndsWith(")")) {
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
                    && TxBx_Title.Text == string.Empty) {
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

        /* Parse dropped text into TxBx(s) */
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
                    else {
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
                    else {
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

        /* Adds folder(s) to database when dragged over LV_Entries */
        private void LV_Entries_DragDrop(object sender, DragEventArgs e)
        {
            string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
            short iTrack = indx;

            //remove restricted folders
            string sError = string.Empty;
            for (int i = 0; i < asDir.Length; i++)
                if (ExtDir.Restricted(asDir[i]))
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

                //add item
                Main.stEntry en = new stEntry(asDir[i]);
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