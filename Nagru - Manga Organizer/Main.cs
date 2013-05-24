/*
 * Author: Taylor Napier
 * Date: Aug15, 2012
 * Desc: Handles organization of manga library
 * 
 * This program is distributed under the
 * GNU General Public License v3
 * 
 * Ionic.Zip.dll is distributed under the 
 * Microsoft Public License (Ms-PL)
 */

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ionic.Zip;

namespace Nagru___Manga_Organizer
{
    public partial class Main : Form
    {
        delegate void DelVoid();
        delegate void DelInt(int iNum);
        delegate void DelString(string sMsg);

        List<csEntry> lData = new List<csEntry>(500);
        LVsorter lvSortObj = new LVsorter();
        bool bSavList = true, bSavText = true, bResize = false;
        short indx = -1, iPage = -1;

        /* Holds manga metadata */
        [Serializable]
        public class csEntry : ISerializable
        {
            public string sArtist, sTitle, 
                sLoc, sType, sDesc, sTags;
            public DateTime dtDate;
            public ushort iPages;
            public byte byRat;

            public csEntry(string _Artist, string _Title, string _Loc, string _Desc,
                string _Tags, string _Type, DateTime _Date, decimal _Pages, int _Rating)
            {
                sArtist = _Artist;
                sTitle = _Title;
                sLoc = _Loc;
                sDesc = _Desc;
                dtDate = _Date;
                iPages = (ushort)_Pages;
                sType = _Type;
                byRat = Convert.ToByte(_Rating);
                
                //trim, clean, and format tags
                sTags = "";
                if (_Tags == "") return;
                string[] sRaw = _Tags.Split(',').Select(
                    x => x.Trim()).Distinct().ToArray<string>();
                Array.Sort(sRaw);
                sTags = String.Join(", ", sRaw);
            }
            public csEntry(string _Path)
            {
                string sRaw = Path.GetFileName(_Path);
                if (sRaw.EndsWith(".zip"))
                    sRaw = sRaw.Substring(0, sRaw.Length - 4);

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
                byRat = 0;

                //Get filecount
                string[] sFiles = new string[0];
                if (Directory.Exists(_Path))
                    sFiles = ExtDir.GetFiles(
                        _Path, SearchOption.TopDirectoryOnly, "*.zip");
                else if (File.Exists(_Path))
                    sFiles = new string[1] { _Path };

                if (sFiles.Length > 0) {
                    if(ZipFile.IsZipFile(sFiles[0])) {
                        using (ZipFile zip = ZipFile.Read(sFiles[0]))
                            iPages = (ushort)zip.Count;
                    }
                }
                else iPages = (ushort)ExtDir.GetFiles(
                    _Path, SearchOption.TopDirectoryOnly).Length;
            }

            //Returns formatted title of Entry
            public override string ToString()
            {
                return (sArtist != "") ?
                    string.Format("[{0}] {1}", sArtist, sTitle)
                    : sTitle;
            }

            /* Override equals to compare entry titles */
            public override bool Equals(object obj)
            {
                csEntry en = obj as csEntry;
                if (obj != null)
                    return (en.sArtist + en.sTitle).Equals(
                        sArtist + sTitle, StringComparison.OrdinalIgnoreCase);
                return false;
            }

            /* 'Disable' hashtable */
            public override int GetHashCode() { return 1; }

            /* custom serialization to save datatypes manually */
            protected csEntry(SerializationInfo info, StreamingContext ctxt)
            {
                sTitle = info.GetString("TI");
                sArtist = info.GetString("AR");
                sLoc = info.GetString("LO");
                sDesc = info.GetString("DS");
                dtDate = info.GetDateTime("DT");
                sType = info.GetString("TY");
                byRat = info.GetByte("RT");
                sTags = info.GetString("TG");
                iPages = (ushort)info.GetInt32("PG");
            }

            /* custom serialization to read datatypes manually */
            [System.Security.Permissions.SecurityPermission(System.Security.Permissions.
                SecurityAction.LinkDemand, Flags = System.Security.Permissions.
                SecurityPermissionFlag.SerializationFormatter)]
            public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
            {
                info.AddValue("TI", sTitle);
                info.AddValue("AR", sArtist);
                info.AddValue("LO", sLoc);
                info.AddValue("DS", sDesc);
                info.AddValue("DT", dtDate);
                info.AddValue("PG", iPages);
                info.AddValue("TY", sType);
                info.AddValue("RT", byRat);
                info.AddValue("TG", sTags);
            }
        }

        /* Search term processing */
        private class csTerm
        {
            readonly bool bAllow;
            readonly string sType;
            readonly string sTerm;

            public csTerm(string _Raw)
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

            public bool Equals(csEntry en)
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
            const int iFileName = 18;

            //if database opened with "Shell->Open with..."
            if (sFile.Length > 0 && sFile[0].EndsWith("\\MangaDatabase.bin") &&
                Properties.Settings.Default.SavLoc != 
                sFile[0].Substring(0, sFile[0].Length - iFileName))
            {
                Properties.Settings.Default.SavLoc = sFile[0];
                MessageBox.Show("Default database location changed to:\n\"" + sFile[0] + "\"",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            //manually handle AssemblyResolve event
            AppDomain.CurrentDomain.AssemblyResolve +=
                new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /* Load custom library 
           Author: Calle Mellergardh (March 1, 2010) */
        System.Reflection.Assembly CurrentDomain_AssemblyResolve(
            object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Ionic.Zip"))
                return (sender as AppDomain).Load(Nagru___Manga_Organizer.
                    Properties.Resources.Ionic_Zip);
            return null;
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

            //check user settings
            this.Location = Properties.Settings.Default.Position.Location;
            this.Width = Properties.Settings.Default.Position.Width;
            this.Height = Properties.Settings.Default.Position.Height;
            if (Properties.Settings.Default.DefZip)
                MnTs_DefZip.Checked = true;
            if (Properties.Settings.Default.DefGrid) {
                LV_Entries.GridLines = true;
                MnTs_DefGrid.Checked = true;
            }
            PicBx_Cover.BackColor = Properties.Settings.Default.DefColour;

            //load database
            string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            sPath += "\\MangaDatabase.bin";
            if (lData.Count == 0 && File.Exists(sPath))
            {
                lData = FileSerializer.Deserialize<List<csEntry>>(sPath) ?? new List<csEntry>(0);

                if (lData.Count == 0) {
                    lData = FileSerializer.ConvertDB(sPath);
                    if (lData == null) {
                        MessageBox.Show("The database was invalid.", "Manga Organizer",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    bSavList = false;
                }
                UpdateLV();

                //set up CmbBx autocomplete
                List<string> lAuto = new List<string>(lData.Count);
                for (int i = 0; i < lData.Count; i++) lAuto.Add(lData[i].sArtist);
                lAuto.Sort(new TrueCompare());
                string[] sFinal = lAuto.Distinct().ToArray();
                CmbBx_Artist.Items.AddRange(sFinal);
            }
        }

        /* Prevent Form close if unsaved data present   */
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bSavText)
                Properties.Settings.Default.Notes = frTxBx_Notes.Text;

            if (!bSavList)
            {
                switch (MessageBox.Show("Save before exiting?", "Manga Organizer",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        this.Visible = false;
                        SaveData();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        bSavText = true;
                        break;
                }
            }

            Properties.Settings.Default.Position =
                new Rectangle(this.Location, this.Size);
            Properties.Settings.Default.Save();
        }

        /* Display image from selected folder   */
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            switch (TabControl.SelectedIndex)
            {
                case 0:
                    if (indx == -1)
                        Text = (TxBx_Search.Text == "" ? "Manga Organizer: " : "Returned: ")
                            + LV_Entries.Items.Count + " entries";
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
                case 2:
                    frTxBx_Notes.Select();
                    this.AcceptButton = null;
                    break;
            }
            this.ResumeLayout();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control) {
                switch (e.KeyCode) {
                    case Keys.D1:
                        TabControl.SelectedIndex = 0;
                        break;
                    case Keys.D2:
                        TabControl.SelectedIndex = 1;
                        break;
                    case Keys.D3:
                        TabControl.SelectedIndex = 2;
                        break;
                }
            }
        }
        #endregion

        #region Tab_Browse
        private void ClearSelection(object sender, EventArgs e)
        { if (indx != -1) Reset(); }

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

            if (TxBx_Search.Text == "") {
                TxBx_Search.Width += 30;
                Btn_Clear.Visible = false;
                UpdateLV();
            }
            else if (!Btn_Clear.Visible) {
                TxBx_Search.Width -= 30;
                Btn_Clear.Visible = true;
            }
            else Delay.Start();
        }
        private void Delay_Tick(object sender, EventArgs e)
        {
            Delay.Stop();
            Search();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            TxBx_Search.Focus();
            TxBx_Search.Clear();
            UpdateLV();
        }

        private void LV_Entries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LV_Entries.SelectedItems.Count > 0)
                SetData(Convert.ToInt32(LV_Entries.FocusedItem.SubItems[5].Text));
            else Reset();
        }

        private void LV_Entries_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //prevent sorting by tags
            if (e.Column == 3) return;

            if (e.Column != lvSortObj.ColToSort)
                lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
            else lvSortObj.SwapOrder();
            
            LV_Entries.Sort();
            Alternate();
        }

        /* Proportionally-resizes columns   */
        private void LV_Entries_Resize(object sender, EventArgs e)
        {
            const int iStatic = 200, iScroll = 20;
            int iMod = LV_Entries.Width / 20;
            LV_Entries.BeginUpdate();
            LV_Entries.Columns[0].Width = iMod * 3;
            LV_Entries.Columns[1].Width = iMod * 6;
            LV_Entries.Columns[3].Width = iMod * 6;

            /* set ColTags to remaining listview width  */
            ColTags.Width += LV_Entries.Width - (iMod * 15) - iStatic;
            if(LV_Entries.Items.Count > LV_Entries.Height / 20)
                ColTags.Width -= iScroll;
            LV_Entries.EndUpdate();
        }

        /* Prevent user from changing column widths */
        private void LV_Entries_ColumnWidthChanging(
            object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = LV_Entries.Columns[e.ColumnIndex].Width;
        }

        private void LV_Entries_DoubleClick(object sender, EventArgs e)
        { OpenFile(); }

        private void LV_Entries_MouseHover(object sender, EventArgs e)
        {
            if (!LV_Entries.Focused)
                LV_Entries.Focus();
        }

        private void ChkBx_ShowFav_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkBx_ShowFav.Checked) OnlyFavs();
            else UpdateLV();

            if (indx != -1 && lData[indx].byRat < 5) ReFocus();
            else Reset();

            LV_Entries.Select();
        }
        #endregion

        #region Tab_View
        /* Select location of manga entry   */
        private void Btn_Loc_Click(object sender, EventArgs e)
        {
            string sPath = TxBx_Loc.Text;
            if(Properties.Settings.Default.DefZip)
            {
                //Try to auto-magically grab folder path
                if (!(sPath != "" && File.Exists(sPath))) {
                    sPath = string.Format("{0}\\[{1}] {2}.zip", Properties.Settings.Default.DefLoc,
                        CmbBx_Artist.Text, TxBx_Title.Text);
                    if (!File.Exists(sPath)) 
                        sPath = Properties.Settings.Default.DefLoc;
                }

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = sPath;
                ofd.Filter = "Zip File (*.zip)|*.zip";
                ofd.Title = "Select the location of the current entry:";

                if(ofd.ShowDialog() == DialogResult.OK) {
                    TxBx_Loc.Text = ofd.FileName;
                    ThreadPool.QueueUserWorkItem(GetImage);
                }
                ofd.Dispose();
            }
            else
            {
                //Try to auto-magically grab folder path
                if (!(sPath != "" && Directory.Exists(sPath))) {
                    sPath = string.Format("{0}\\[{1}] {2}", Properties.Settings.Default.DefLoc,
                        CmbBx_Artist.Text, TxBx_Title.Text);
                    if (!Directory.Exists(sPath)) 
                        sPath = Properties.Settings.Default.DefLoc;
                }
                
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                fbd.Description = "Select the location of the current entry:";
                fbd.SelectedPath = sPath;

                if (fbd.ShowDialog() == DialogResult.OK) {
                    TxBx_Loc.Text = fbd.SelectedPath;
                    ThreadPool.QueueUserWorkItem(GetImage);
                }
                fbd.Dispose();
            }
        }

        /* Open URL in default Browser  */
        private void frTxBx_Desc_LinkClicked(object sender, LinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(e.LinkText); }

        /* Alternative to MnTS_Open */
        private void PicBx_Cover_Click(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image == null)
                return;

            Browse_Img fmBrowse = new Browse_Img();
            fmBrowse.iPage = iPage;

            string[] sFiles = new string[0];
            if (!File.Exists(TxBx_Loc.Text)) {
                if (Directory.Exists(TxBx_Loc.Text)) {
                    sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                        SearchOption.TopDirectoryOnly, "*.zip");
                }
            }
            else sFiles = new string[1] { TxBx_Loc.Text };

            if (sFiles.Length > 0
                && ZipFile.IsZipFile(sFiles[0]))
            {
                string sDir = Path.GetDirectoryName(sFiles[0]) + "\\!tmp";
                fmBrowse.lFiles = new List<string>(25);
                
                DirectoryInfo di = Directory.CreateDirectory(sDir);
                #if !DEBUG
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                #endif
                using (ZipFile zip = ZipFile.Read(sFiles[0])) {
                    for (int i = 0; i < zip.Count; i++) {
                        fmBrowse.lFiles.Add(sDir + '\\' + zip[i].FileName);
                    }

                    zip.TempFileFolder = sDir;
                    fmBrowse.zip = zip;
                    fmBrowse.ShowDialog();
                    iPage = (short)fmBrowse.iPage;
                    Directory.Delete(sDir, true);
                }
            }
            else if ((sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                SearchOption.TopDirectoryOnly)).Length > 0)
            {
                fmBrowse.lFiles = sFiles.ToList<string>();
                fmBrowse.ShowDialog();
                iPage = (short)fmBrowse.iPage;
            }
            fmBrowse.Dispose();
            GC.Collect();
        }

        /* Redraw cover image if size has changed */
        private void PicBx_Cover_Resize(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image == null) return;
            SizeF sf = PicBx_Cover.Image.PhysicalDimension;
            if (sf.Height < PicBx_Cover.Height)
                bResize = true;
            else if(sf.Width > PicBx_Cover.Width)
                bResize = true;
        }
        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            if (bResize) {
                ThreadPool.QueueUserWorkItem(GetImage);
                bResize = false;
            }
        }

        /* Dynamically update PicBx when user manually alters path */
        private void TxBx_Loc_TextChanged(object sender, EventArgs e)
        {
            if (indx != -1) MnTS_Edit.Visible = true;

            if (File.Exists(TxBx_Loc.Text)
                || Directory.Exists(TxBx_Loc.Text)) {
                iPage = -1;
                ThreadPool.QueueUserWorkItem(GetImage);
            }
            else {
                SetPicBxNull();
                SetOpenStatus(0);
            }
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
            int iCur = LV_Entries.SelectedItems.Count == 1 ?
                LV_Entries.SelectedItems[0].Index : -1;
            int iPos = 0;

            Random rnd = new Random();
            do {
                iPos = rnd.Next(LV_Entries.Items.Count);
            }
            while (iPos == iCur);
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
            int iWidth = TextRenderer.MeasureText(TxBx_Tags.Text, TxBx_Tags.Font).Width;
            if (iWidth > TxBx_Tags.Width) {
                ScrTags.Maximum = iWidth / 5;
                ScrTags.Value = TxBx_Tags.SelectionStart;
                ScrTags.Visible = true;
            }
            else ScrTags.Visible = false;
        }

        private void srRating_Click(object sender, EventArgs e)
        {
            if (indx == -1 || lData[indx].byRat == srRating.SelectedStar) 
                return;

            //update rating
            lData[indx].byRat = (byte)srRating.SelectedStar;
            LV_Entries.SelectedItems[0].SubItems[6].Text = 
                RatingFormat(lData[indx].byRat);
            bSavList = false;

            //set BackColor
            if (lData[indx].byRat == 5)
                LV_Entries.FocusedItem.BackColor = Color.LightYellow;
            else {
                if (LV_Entries.FocusedItem.Index % 2 == 0)
                    LV_Entries.FocusedItem.BackColor = Color.FromArgb(245, 245, 245);
                else LV_Entries.FocusedItem.BackColor = SystemColors.Window;
            }
        }

        /* Only enable edit when changes have been made */
        private void EntryAlt_Text(object sender, EventArgs e)
        {
            if (indx != -1) MnTS_Edit.Visible = true;
            if (TxBx_Tags.ContainsFocus) SetScroll();
        }
        private void EntryAlt_DtNum(object sender, EventArgs e)
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
        private void AddEntries()
        {
            UpdateLV();
            bSavList = false;
            LV_Entries.Select();
            if (indx != -1) ReFocus();
        }

        private void Alternate(int iStart = 0)
        {
            if (Properties.Settings.Default.DefGrid) return;
            for (int i = iStart; i < LV_Entries.Items.Count; i++) {
                if (LV_Entries.Items[i].BackColor != Color.LightYellow) {
                    if (i % 2 == 0)
                        LV_Entries.Items[i].BackColor = Color.FromArgb(245, 245, 245);
                    else LV_Entries.Items[i].BackColor = SystemColors.Window;
                }
            }
        }

        private void GetImage(Object obj)
        {
            BeginInvoke(new DelVoid(SetPicBxNull));

            //Get cover and filecount
            string[] sFiles = new string[0];
            if (!File.Exists(TxBx_Loc.Text)) {
                if (Directory.Exists(TxBx_Loc.Text)) {
                    sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                        SearchOption.TopDirectoryOnly, "*.zip");
                }
            }
            else sFiles = new string[1] { TxBx_Loc.Text };

            if (sFiles.Length > 0
                && ZipFile.IsZipFile(sFiles[0]))
            {
                using (ZipFile zip = ZipFile.Read(sFiles[0])) {
                    if (zip.Count > 0) {
                        SetPicBxImage(sFiles[0]);
                        BeginInvoke(new DelInt(SetNudCount), zip.Count);
                    }
                }
            }
            else if ((sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                SearchOption.TopDirectoryOnly)).Length > 0) {
                SetPicBxImage(sFiles[0]);
                BeginInvoke(new DelInt(SetNudCount), sFiles.Length);
            }
            else BeginInvoke(new DelInt(SetOpenStatus), 0);
        }

        private static int InsertText(Control c, string sAdd, int iStart)
        {
            int iNewStart = iStart + sAdd.Length;
            c.Text = c.Text.Insert(iStart, sAdd);
            return iNewStart;
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
            return string.Empty;
        }

        private void OnlyFavs()
        {
            Cursor = Cursors.WaitCursor;
            LV_Entries.BeginUpdate();
            for (int i = 0; i < LV_Entries.Items.Count; i++) {
                if(LV_Entries.Items[i].BackColor != Color.LightYellow)
                    LV_Entries.Items.RemoveAt(i--);
            }
            LV_Entries.EndUpdate();
            Text = "Returned: " + LV_Entries.Items.Count + " entries";
            Cursor = Cursors.Default;
        }

        private void OpenFile()
        {
            if (PicBx_Cover.Image == null)
                return;

            if(Directory.Exists(TxBx_Loc.Text)) {
                string[] sFiles = ExtDir.GetFiles(@TxBx_Loc.Text);
                if (sFiles.Length > 0) System.Diagnostics.Process.Start(sFiles[0]);
            }
            else System.Diagnostics.Process.Start(@TxBx_Loc.Text);
        }

        private static string RatingFormat(byte byVal)
        {
            System.Text.StringBuilder sb =
                new System.Text.StringBuilder("");
            sb.Append('☆', byVal);
            return sb.ToString();
        }

        private void ReFocus()
        {
            if (LV_Entries.Items.Count == 0)
                return;

            for (int i = 0; i < LV_Entries.Items.Count; i++)
                if (LV_Entries.Items[i].SubItems[1].Text == lData[indx].sTitle) {
                    ScrollTo(i);
                    break;
                }
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
            srRating.SelectedStar = 0;
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
                lData.Sort((eX, eY) => (eX.sArtist + eX.sTitle)
                    .CompareTo(eY.sArtist + eY.sTitle));
                if (File.Exists(sPath)) File.Delete(sPath);
                FileSerializer.Serialize(sPath, lData);
                bSavList = true;
            }
            if(!bSavText) {
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
            List<csTerm> lTerms = new List<csTerm>(5);
            List<ListViewItem> lItems = new List<ListViewItem>(lData.Count);

            for (int i = 0; i < sTags.Length; i++)
                lTerms.Add(new csTerm(sTags[i]));

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
                if (lData[x].byRat == 5) lvi.BackColor = Color.LightYellow;
                lvi.SubItems.AddRange(new string[] {
                        lData[x].sTitle,
                        lData[x].iPages.ToString(),
                        lData[x].sTags,
                        lData[x].sType,
                        x.ToString(),
                        RatingFormat(lData[x].byRat)
                    });
                lItems.Add(lvi);
            }

            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(lItems.ToArray());
            LV_Entries.Sort();
            Alternate();
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
            srRating.SelectedStar = lData[indx].byRat;
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
            SetOpenStatus(1);
        }

        private void SetOpenStatus(int iExists)
        {
            if (iExists == 1) {
                if (!MnTS_Open.Visible)
                    MnTS_Open.Visible = true;
            }
            else {
                if (MnTS_Open.Visible)
                    MnTS_Open.Visible = false;
            }
        }

        private void SetPicBxImage(string sPath)
        {
            if (sPath.EndsWith(".zip")) {
                using (ZipFile zip = ZipFile.Read(sPath)) {
                    if(zip.Count > 0) {
                        sPath = Path.GetDirectoryName(sPath);
                        Directory.CreateDirectory(sPath += "\\!tmp");
                        zip[0].Extract(sPath, ExtractExistingFileAction.OverwriteSilently);
                        TrySet(sPath + '\\' + zip[0].FileName);
                        Directory.Delete(sPath, true);
                    }
                }
            }
            else TrySet(sPath);
        }
        private void TrySet(string s)
        {
            try {
                using (Bitmap bmpTmp = new Bitmap(s)) {
                    PicBx_Cover.Image = ExtImage.Scale(
                        bmpTmp, PicBx_Cover.Width, PicBx_Cover.Height);
                }
            }
            catch {
                MessageBox.Show("The following file could not be loaded:\n" + s,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SetPicBxNull()
        {
            if (PicBx_Cover.Image != null) {
                PicBx_Cover.Image.Dispose();
                PicBx_Cover.Image = null;
                GC.Collect();
            }
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
            if (TxBx_Search.Text != "") { 
                Search(); return;
            }

            Cursor = Cursors.WaitCursor;
            Text = "Manga Organizer: " + lData.Count + " entries";
            ListViewItem[] aItems = new ListViewItem[lData.Count];

            for (int i = 0; i < lData.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                if (lData[i].byRat == 5) lvi.BackColor = Color.LightYellow;
                lvi.SubItems.AddRange(new string[] {
                    lData[i].sTitle,
                    lData[i].iPages.ToString(),
                    lData[i].sTags,
                    lData[i].sType,
                    i.ToString(),
                    RatingFormat(lData[i].byRat)
                });
                aItems[i] = lvi;
            }

            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(aItems);
            LV_Entries.Sort();
            Alternate();
            LV_Entries.EndUpdate();
            Cursor = Cursors.Default;

            //prevent loss of other parameters
            if (ChkBx_ShowFav.Checked) OnlyFavs();
        }
        #endregion

        #region Menu_EntryOps
        private void MnTS_New_Click(object sender, EventArgs e)
        {
            //reject when title is unfilled
            if (TxBx_Title.Text == "") {
                MessageBox.Show("Title cannot be empty.", "Manga Organizer",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            csEntry en = new csEntry(CmbBx_Artist.Text, TxBx_Title.Text, 
                TxBx_Loc.Text, frTxBx_Desc.Text, TxBx_Tags.Text, CmbBx_Type.Text,
                Dt_Date.Value.Date, Nud_Pages.Value, srRating.SelectedStar);

            if (!lData.Contains(en))
            {
                if (MessageBox.Show("Are you sure you wish to add:\n\"" + en + "\"",
                    "Manga Organizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
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
                }
            }
            else MessageBox.Show("This item already exists in the database.",
                "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MnTS_Edit_Click(object sender, EventArgs e)
        {
            if (indx == -1) return;

            //overwrite entry properties
            lData[indx] = new csEntry(CmbBx_Artist.Text, TxBx_Title.Text, 
                TxBx_Loc.Text, frTxBx_Desc.Text, TxBx_Tags.Text, CmbBx_Type.Text,
                Dt_Date.Value, Nud_Pages.Value, srRating.SelectedStar);
            Text = "Edited entry: " + lData[indx].ToString();
            TxBx_Tags.Text = lData[indx].sTags;

            //update LV_Entries & maintain selection
            ListViewItem lvi = LV_Entries.FocusedItem;
            if (lData[indx].byRat == 5) 
                lvi.BackColor = Color.LightYellow;
            lvi.SubItems[0].Text = lData[indx].sArtist;
            lvi.SubItems[1].Text = lData[indx].sTitle;
            lvi.SubItems[2].Text = lData[indx].iPages.ToString();
            lvi.SubItems[3].Text = lData[indx].sTags;
            lvi.SubItems[4].Text = lData[indx].sType;
            LV_Entries.Sort();

            int iPos = lvi.Index;
            if (ChkBx_ShowFav.Checked && !(lData[indx].byRat == 5))
            {
                lvi.Remove();
            }
            else if (TxBx_Search.Text != "")
            {
                string[] sTags = TxBx_Search.Text.Split(' ');
                List<csTerm> lTerms = new List<csTerm>(5);

                for (int i = 0; i < sTags.Length; i++)
                    lTerms.Add(new csTerm(sTags[i]));

                for (int y = 0; y < lTerms.Count; y++) {
                    if (!lTerms[y].Equals(lData[indx])) {
                        lvi.Remove();
                        break;
                    }
                }
            }
            else ReFocus();

            Alternate(iPos);
            bSavList = false;
            MnTS_Edit.Visible = false;
        }

        private void MnTS_Open_Click(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image != null)
                OpenFile();
        }

        private void MnTS_Delete_Click(object sender, EventArgs e)
        {
            if (indx == -1) return;

            //ensure deletion is intentional
            DialogResult dResult = MessageBox.Show("Do you want to delete the source directory/file as well?",
                "Manga Organizer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (dResult == DialogResult.Yes)
            {
                if (!ExtDir.Restricted(lData[indx].sLoc))
                {
                    //warn user before deleting large directory
                    int iNumDir = Directory.GetDirectories(lData[indx].sLoc).Length;
                    if (iNumDir > 0)
                    {
                        dResult = MessageBox.Show("This directory contains " + iNumDir + " subfolder(s),\n" +
                            "are you sure you want to delete them?", "Manga Organizer",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    if (File.Exists(lData[indx].sLoc))
                    {
                        this.Cursor = Cursors.WaitCursor;
                        File.Delete(lData[indx].sLoc);
                        this.Cursor = Cursors.Default;
                    }
                    dResult = DialogResult.No;
                }
            }

            if (dResult != DialogResult.Cancel)
            {
                if (dResult == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Directory.Delete(lData[indx].sLoc, true);
                    this.Cursor = Cursors.Default;
                }

                //remove from database
                bSavList = false;
                int iPos = LV_Entries.FocusedItem.Index;
                lData.RemoveAt(indx);
                UpdateLV();
                Reset();

                if(iPos < LV_Entries.Items.Count) {
                    LV_Entries.TopItem = LV_Entries.Items[iPos];
                    LV_Entries.TopItem = LV_Entries.Items[iPos];
                    LV_Entries.TopItem = LV_Entries.Items[iPos];
                }
            }
        }

        private void MnTS_Clear_Click(object sender, EventArgs e)
        { Reset(); }
        #endregion

        #region Menu_Main
        private void MnTS_CopyTitle_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Format(
                (CmbBx_Artist.Text != "") ? "[{0}] {1}" : "{1}",
                CmbBx_Artist.Text, TxBx_Title.Text));
            Text = "Name copied to clipboard";
        }

        private void MnTS_OpenSource_Click(object sender, EventArgs e)
        {
            if (TxBx_Loc.Text != "" && !ExtDir.Restricted(TxBx_Loc.Text))
                System.Diagnostics.Process.Start(TxBx_Loc.Text);
        }

        /* Uses EH API to get metadata from gallery URL */
        private void MnTS_LoadUrl_Click(object sender, EventArgs e)
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
                    ServicePointManager.DefaultConnectionLimit = 64;
                    HttpWebRequest rq = (HttpWebRequest)
                        WebRequest.Create("http://g.e-hentai.org/api.php");
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
                        HttpWebResponse)rq.GetResponse()).GetResponseStream())) {
                        Text = "Downloading page...";
                        asResp = ExtString.Split(sr.ReadToEnd(), "\",\"");
                        rq.Abort();
                    }

                    //parse metadata
                    Text = "Parsing metadata...";
                    SplitTitle(ExtString.DecodeNonAscii(asResp[2].Split(':')[1].Substring(1)));
                    CmbBx_Type.Text = asResp[4].Split(':')[1].Substring(1);
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

        private void MnTS_Save_Click(object sender, EventArgs e)
        { SaveData(); }

        private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
        {
            string sPath = Properties.Settings.Default.SavLoc != "" ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;

            if (Directory.Exists(sPath))
                System.Diagnostics.Process.Start(sPath);
            else MessageBox.Show("This directory no longer exists.", "Manga Organizer",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
        }

        private void MnTS_Stats_Click(object sender, EventArgs e)
        {
            Stats fmStats = new Stats();
            fmStats.lCurr = lData;
            fmStats.Show();
        }

        private void MnTS_About_Click(object sender, EventArgs e)
        {
            About fmAbout = new About();
            fmAbout.ShowDialog();
            fmAbout.Dispose();
        }

        private void MnTs_Quit_Click(object sender, EventArgs e)
        { this.Close(); }
        #endregion

        #region Menu_Settings
        private void MnTs_DefSav_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            string sPath = Properties.Settings.Default.SavLoc != "" ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK && 
                !ExtDir.Restricted(fbd.SelectedPath))
            {
                Properties.Settings.Default.SavLoc = fbd.SelectedPath;
                fbd.SelectedPath += "\\MangaDatabase.bin";
                sPath += "\\MangaDatabase.bin";

                //move old save to new location
                if (File.Exists(fbd.SelectedPath)
                    && MessageBox.Show("Open existing database at:\n" + fbd.SelectedPath,
                    "Manga Organizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    lData = FileSerializer.Deserialize<List<csEntry>>(sPath);

                    if (lData != null)
                    {
                        UpdateLV();

                        //set up CmbBx autocomplete
                        List<string> lAuto = new List<string>(lData.Count);
                        for (int i = 0; i < lData.Count; i++) lAuto.Add(lData[i].sArtist);
                        lAuto.Sort(new TrueCompare());
                        string[] sFinal = lAuto.Distinct().ToArray();
                        CmbBx_Artist.Items.Clear();
                        CmbBx_Artist.Items.AddRange(sFinal);
                    }
                }
                else if (File.Exists(sPath))
                    File.Move(sPath, fbd.SelectedPath);

                Text = "Default save location changed";
            }
            fbd.Dispose();
        }

        private void MnTs_DefRoot_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Properties.Settings.Default.DefLoc;

            if (fbd.ShowDialog() == DialogResult.OK
                && !ExtDir.Restricted(fbd.SelectedPath))
            {
                Properties.Settings.Default.DefLoc = fbd.SelectedPath;
                Text = "Default location changed";
            }
            fbd.Dispose();
        }

        private void MnTs_DefZip_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DefZip = !Properties.Settings.Default.DefZip;
            MnTs_DefZip.Checked = !MnTs_DefZip.Checked;
        }

        private void MnTs_DefGrid_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DefGrid = !Properties.Settings.Default.DefGrid;
            MnTs_DefGrid.Checked = !MnTs_DefGrid.Checked;
            LV_Entries.GridLines = !LV_Entries.GridLines;
            UpdateLV();
        }

        private void MnTS_DefColour_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.CustomColors = new int[2] {
                ColorTranslator.ToOle(Color.FromArgb(39,40,34)),
                ColorTranslator.ToOle(PicBx_Cover.BackColor)  };
            cd.Color = PicBx_Cover.BackColor;

            if (cd.ShowDialog() == DialogResult.OK) {
                PicBx_Cover.BackColor = cd.Color;
                Properties.Settings.Default.DefColour = cd.Color;
            }
        }
        #endregion

        #region Menu_Text
        private void MnTx_Undo_Click(object sender, EventArgs e)
        {
            switch (ActiveControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).Undo();
                    if (TabControl.SelectedIndex == 2) bSavText = true;
                    break;
                case "TextBox":
                    ((TextBox)ActiveControl).Undo();
                    break;
                case "ComboBox":
                    ((ComboBox)ActiveControl).ResetText();
                    break;
            }
        }

        private void MnTx_Cut_Click(object sender, EventArgs e)
        {
            switch (ActiveControl.GetType().Name)
            {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).Cut();
                    if (TabControl.SelectedIndex == 2) bSavText = false;
                    break;
                case "TextBox":
                    ((TextBox)ActiveControl).Cut();
                    break;
                case "ComboBox":
                    ComboBox cx = (ComboBox)ActiveControl;
                    if (cx.SelectedText != "") {
                        Clipboard.SetText(cx.SelectedText);
                        cx.SelectedText = "";
                    }
                    break;
            }
        }

        private void MnTx_Copy_Click(object sender, EventArgs e)
        {
            switch (ActiveControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).Copy();
                    break;
                case "TextBox":
                    ((TextBox)ActiveControl).Copy();
                    break;
                case "ComboBox":
                    Clipboard.SetText(((ComboBox)ActiveControl).SelectedText);
                    break;
            }
        }

        private void MnTx_Paste_Click(object sender, EventArgs e)
        {
            string sAdd = Clipboard.GetText();
            switch (ActiveControl.GetType().Name)
            {
                case "FixedRichTextBox":
                    FixedRichTextBox fr = ((FixedRichTextBox)ActiveControl);
                    fr.SelectionStart = InsertText(fr, sAdd, fr.SelectionStart);
                    if (TabControl.SelectedIndex == 2) bSavText = false;
                    break;
                case "TextBox":
                    TextBox txt = (TextBox)ActiveControl;
                    if (txt.Name == "TxBx_Tags" && sAdd.Contains("\r")) {
                        IEnumerable<string> ie = sAdd.Split('(', '\n');
                        ie = ie.Where(s => !s.EndsWith("\r") && !s.EndsWith(")") && !s.Equals(""));
                        ie = ie.Select(s => s.TrimEnd());
                        sAdd = string.Join(", ", ie.ToArray());
                    }
                    txt.SelectionStart = InsertText(
                        txt, sAdd, txt.SelectionStart);
                    if(txt.Name == "TxBx_Search")
                        TxBx_Search_TextChanged(new object(), new EventArgs());
                    break;
                case "ComboBox":
                    ComboBox cb = (ComboBox)ActiveControl;
                    if (cb.Text == string.Empty
                        && TxBx_Title.Text == string.Empty)
                        SplitTitle(sAdd);
                    else cb.SelectionStart = InsertText(cb, sAdd, cb.SelectionStart);
                    break;
            }
        }

        private void MnTx_Delete_Click(object sender, EventArgs e)
        {
            switch (ActiveControl.GetType().Name)
            {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).SelectedText = "";
                    break;
                case "TextBox":
                    ((TextBox)ActiveControl).SelectedText = "";
                    break;
                case "ComboBox":
                    ((ComboBox)ActiveControl).SelectedText = "";
                    break;
            }
        }

        private void MnTx_SelAll_Click(object sender, EventArgs e)
        {
            switch (ActiveControl.GetType().Name)
            {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).SelectAll();
                    break;
                case "TextBox":
                    ((TextBox)ActiveControl).SelectAll();
                    break;
                case "ComboBox":
                    ((ComboBox)ActiveControl).SelectAll();
                    break;
            }
        }

        private void Mn_TxBx_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool bUndo = false, bSelect = false;
            switch (ActiveControl.GetType().Name)
            {
                case "FixedRichTextBox":
                    FixedRichTextBox fr = (FixedRichTextBox)ActiveControl;
                    if (fr.CanUndo) bUndo = true;
                    if (fr.SelectionLength > 0) bSelect = true;
                    break;
                case "TextBox":
                    TextBox txt = (TextBox)ActiveControl;
                    if (txt.CanUndo) bUndo = true;
                    if (txt.SelectionLength > 0) bSelect = true;
                    break;
                case "ComboBox":
                    ComboBox cb = (ComboBox)ActiveControl;
                    if (cb.SelectionLength > 0) bSelect = true;
                    bUndo = true;
                    break;
            }

            if (bUndo) MnTx_Undo.Enabled = true;
            else MnTx_Undo.Enabled = false;

            if (bSelect) {
                MnTx_Cut.Enabled = true;
                MnTx_Copy.Enabled = true;
                MnTx_Delete.Enabled = true;
            }
            else {
                MnTx_Cut.Enabled = false;
                MnTx_Copy.Enabled = false;
                MnTx_Delete.Enabled = false;
            }
        }

        /* Ensure element recieves focus when right-clicked */
        private void TbPg_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            switch (sender.GetType().Name)
            {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)sender).Select();
                    break;
                case "TextBox":
                    ((TextBox)sender).Select();
                    break;
                case "ComboBox":
                    ((ComboBox)sender).Select();
                    break;
            }
        }

        /* Prevent mixed font types when c/p  */
        private void frTxBx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                if (TabControl.SelectedIndex == 2)
                {
                    frTxBx_Notes.SelectionStart = InsertText(
                        frTxBx_Notes, Clipboard.GetText(), frTxBx_Notes.SelectionStart);
                    bSavText = false;
                }
                else frTxBx_Desc.SelectionStart = InsertText(
                    frTxBx_Desc, Clipboard.GetText(), frTxBx_Desc.SelectionStart);
                e.Handled = true;
            }
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

            switch ((sender as Control).Name)
            {
                case "frTxBx_Desc":
                    frTxBx_Desc.SelectionStart = InsertText(
                        frTxBx_Desc, sAdd, frTxBx_Desc.SelectionStart);
                    break;
                case "frTxBx_Notes":
                    frTxBx_Notes.SelectionStart = InsertText(
                        frTxBx_Notes, sAdd, frTxBx_Notes.SelectionStart);
                    break;
                case "CmbBx_Artist":
                    if (sAdd.Contains('[')) SplitTitle(sAdd);
                    else CmbBx_Artist.SelectionStart = InsertText(
                        CmbBx_Artist, sAdd, CmbBx_Artist.SelectionStart);
                    break;
                case "TxBx_Title":
                    if (sAdd.Contains('[')) SplitTitle(sAdd);
                    else TxBx_Title.SelectionStart = InsertText(
                        TxBx_Title, sAdd, TxBx_Title.SelectionStart);
                    break;
                case "TxBx_Loc":
                    TxBx_Loc.SelectionStart = InsertText(
                        TxBx_Loc, sAdd, TxBx_Loc.SelectionStart);
                    break;
                case "TxBx_Tags":
                    if (sAdd.Contains("\r")) {
                        IEnumerable<string> ie = sAdd.Split('(', '\n');
                        ie = ie.Where(s => !s.EndsWith("\r") && !s.EndsWith(")") && !s.Equals(""));
                        ie = ie.Select(s => s.TrimEnd());
                        sAdd = string.Join(", ", ie.ToArray());
                    }
                    TxBx_Tags.SelectionStart = InsertText(
                        TxBx_Tags, sAdd, TxBx_Tags.SelectionStart);
                    break;
            }
        }

        /* Allow dropping of folders/zips onto LV_Entries */
        private void LV_Entries_DragEnter(object sender, DragEventArgs e)
        {
            string[] sTemp = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (sTemp == null) return;

            FileAttributes fa = File.GetAttributes(sTemp[0]);
            if (fa == FileAttributes.Directory || ZipFile.IsZipFile(sTemp[0]))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /* Adds folder(s) to database when dragged over LV_Entries */
        private void LV_Entries_DragDrop(object sender, DragEventArgs e)
        {
            string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
            short iTrack = indx;
            string sError = "";

            //add all remaining folders
            for (int i = 0; i < asDir.Length; i++)
            {
                if (asDir[i] == "") continue;

                //add item
                Main.csEntry en = new csEntry(asDir[i]);
                if (!lData.Contains(en))
                {
                    lData.Add(en);
                    indx = (short)(lData.Count - 1);

                    if (en.sArtist != "" 
                        && !CmbBx_Artist.Items.Contains(en.sArtist))
                        CmbBx_Artist.Items.Add(en.sArtist);
                }
                else sError += asDir[i] + '\n';
            }
            if (sError != "") {
                MessageBox.Show("The following path(s) already exists in the database:\n" + sError,
                    "Manga Organizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Update LV
            if (iTrack != indx)
                AddEntries();
        }
        #endregion

        #region Cruft
        /* Legacy Manga entry code 
           Used for converting old databases */
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
                sTags = _Tags;
            }

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
                iPages = (ushort)info.GetInt32("Pages");
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
        #endregion
    }
}