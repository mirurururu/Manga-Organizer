/*
 * Author: Taylor Napier
 * Date: Aug15, 2012
 * Desc: Handles organization of manga library
 * 
 * This program is distributed under the
 * GNU General Public License v3 (GPLv3)
 * 
 * SharpCompress is distributed under the 
 * MIT\Expat License (MIT)
 * 
 * SQLite is in the public domain
 * Ergo, it does not require any license
 * 
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
using SCA = SharpCompress.Archive;
using System.Data.SQLite;
//using Finisar.SQLite;

namespace Nagru___Manga_Organizer
{
    public partial class Main : Form
    {
        #region Properties
		const int iLightGray = -657931;
        const int iLightYellow = -15;

        delegate void DelVoid();
        delegate void DelInt(int iNum);
        delegate void DelString(string sMsg);

        List<csEntry> lData = new List<csEntry>(500);
        LVsorter lvSortObj = new LVsorter();
        bool bSavList = true, bSavText = true, bResize = false;
        int indx = -1, iPage = -1;
        #endregion

        #region Classes
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
                iPages = (_Pages > ushort.MaxValue) ?
                        ushort.MaxValue : (ushort)_Pages;
                sType = _Type;
                byRat = Convert.ToByte(_Rating);
                sTags = "";

                //trim, clean, and format tags
                if (_Tags != "")
                {
                    string[] sRaw = _Tags.Split(',').Select(
                        x => x.Trim()).Distinct().Where(
                        x => !string.IsNullOrEmpty(x)).ToArray<string>();
                    sTags = String.Join(", ", sRaw.OrderBy(x => x));
                }
            }

            public csEntry(string _Path)
            {
                //Try to format raw title string
                string[] asTitle = Main.SplitTitle(
                    ExtString.GetNameSansExtension(_Path));
                sArtist = asTitle[0];
                sTitle = asTitle[1];

                sLoc = _Path;
                sType = "Manga";
                sDesc = "";
                sTags = "";
                dtDate = DateTime.Now;
                byRat = 0;
                iPages = 0;

                //Get filecount
                string[] sFiles = new string[0];
                if (File.Exists(_Path))
                    sFiles = new string[1] { _Path };
                else sFiles = ExtDir.GetFiles(_Path,
                    SearchOption.TopDirectoryOnly, "*.zip|*.cbz|*.cbr|*.rar|*.7z");

                if (sFiles.Length > 0)
                {
                    if (Main.IsArchive(sFiles[0]))
                    {
                        SCA.IArchive scArchive = SCA.ArchiveFactory.Open(sFiles[0]);
                        iPages = (scArchive.Entries.Count() > ushort.MaxValue) ?
                            ushort.MaxValue : (ushort)Math.Abs(scArchive.Entries.Count());
                        scArchive.Dispose();
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
                if (!(obj is csEntry)) return false;
                csEntry en = obj as csEntry;
                return (en.sArtist + en.sTitle).Equals(
                    sArtist + sTitle, StringComparison.OrdinalIgnoreCase);
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
        private class csSearchTerm
        {
            readonly bool[] bAllow;
            readonly string[] sTerm;
            readonly string sType;
			readonly int iTypeID;

            public csSearchTerm(string _Raw)
            {
                //check for type limiter
                sTerm = new string[1];
                string[] sSplit = _Raw.Trim().Split(':');
                if (sSplit.Length == 2) {
                    sType = sSplit[0];
					sTerm = ExtString.Split(sSplit[1], "&", ",");
                }
				else sTerm = ExtString.Split(_Raw, "&", ",");

                //check for chained terms
                bAllow = new bool[sTerm.Length];
                for(int i = 0; i < sTerm.Length; i++) {
                    sTerm[i] = sTerm[i].Replace('_', ' ');
                    if (sTerm[i].StartsWith("-")) {
                        sTerm[i] = sTerm[i].Substring(1);
                        bAllow[i] = false;
                    }
                    else bAllow[i] = true;
				}

				#region Set TypeID
				switch (sType) {
					case "artist":
					case "a":
						iTypeID = 0;
						break;
					case "title":
					case "t":
						iTypeID = 1;
						break;
					case "tag":
					case "tags":
					case "g":
						iTypeID = 2;
						break;
					case "description":
					case "desc":
					case "s":
						iTypeID = 3;
						break;
					case "type":
					case "y":
						iTypeID = 4;
						break;
					case "date":
					case "d":
						iTypeID = 5;
						break;
					case "pages":
					case "page":
					case "p":
						iTypeID = 6;
						break;
					default:
						iTypeID = -1;
						break;
				}
				#endregion
			}

            public bool Equals(csEntry en)
            {
                for(int i = 0; i < sTerm.Length; i++) {
                    bool bMatch = false;
                    switch (iTypeID) {
                        case 0:
                            bMatch = ExtString.Contains(en.sArtist, sTerm[i]);
                            break;
                        case 1:
                            bMatch = ExtString.Contains(en.sTitle, sTerm[i]);
                            break;
                        case 2:
                            bMatch = ExtString.Contains(en.sTags, sTerm[i]);
                            break;
                        case 3:
                            bMatch = ExtString.Contains(en.sDesc, sTerm[i]);
                            break;
                        case 4:
                            bMatch = ExtString.Contains(en.sType, sTerm[i]);
                            break;
                        case 5:
                            DateTime dt = new DateTime();
                            char c = sTerm[i] != "" ? sTerm[i][0] : ' ';

                            switch (c) {
                                case '<':
                                    if (DateTime.TryParse(sTerm[i].Substring(1), out dt))
                                        bMatch = dt >= en.dtDate;
                                    break;
                                case '>':
                                    if (DateTime.TryParse(sTerm[i].Substring(1), out dt))
                                        bMatch = dt <= en.dtDate;
                                    break;
                                default:
                                    if (DateTime.TryParse(sTerm[i], out dt))
                                        bMatch = dt == en.dtDate;
                                    break;
                            }
                            break;
                        case 6:
                            c = sTerm[i] != "" ? sTerm[i][0] : ' ';
                            int x;

                            switch (c) {
                                case '<':
                                    if (int.TryParse(sTerm[i].Substring(1), out x))
                                        bMatch = en.iPages <= x;
                                    break;
                                case '>':
                                    if (int.TryParse(sTerm[i].Substring(1), out x))
                                        bMatch = en.iPages >= x;
                                    break;
                                default:
                                    bMatch = en.iPages.ToString().Equals(sTerm[i]);
                                    break;
                            }
                            break;
                        default:
                            bMatch = (ExtString.Contains(en.sTags, sTerm[i]) ||
                                ExtString.Contains(en.sTitle, sTerm[i]) ||
                                ExtString.Contains(en.sArtist, sTerm[i]) ||
                                ExtString.Contains(en.sDesc, sTerm[i]) ||
                                ExtString.Contains(en.sType, sTerm[i]) ||
								ExtString.Contains(en.dtDate.ToString(), sTerm[i]));
                            break;
                    }

                    if (!(bAllow[i] ? bMatch : !bMatch))
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region Main Form
        public Main(string[] sFile)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.dbIcon;
			
            //if database opened with "Shell->Open with..."
            if (sFile.Length > 0 
                    && sFile[0].EndsWith("\\MangaDatabase.bin") 
                    && Properties.Settings.Default.SavLoc != sFile[0].Substring(
                        0, sFile[0].Length - "\\MangaDatabase.bin".Length))
            {
                Properties.Settings.Default.SavLoc = sFile[0];
                MessageBox.Show("Default database location changed to:\n\"" + sFile[0] + '\"',
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            //manually handle AssemblyResolve event
            AppDomain.CurrentDomain.AssemblyResolve +=
                new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /* Load custom library 
           Author: Calle Mellergardh (March 1, 2010) */
        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(
            object sender, ResolveEventArgs args)
        {
			System.Reflection.Assembly asm = null;
            if(ExtString.Equals(args.Name, "SharpCompress, Version=0.10.3.0, "
                + "Culture=neutral, PublicKeyToken=beaf6f427e128133"))
                asm = (AppDomain.CurrentDomain).Load(Properties.Resources.SharpCompress);
			/*else if (ExtString.Equals(args.Name, "SQLite.NET, Version=0.21.1869.3794, "
				+ "Culture=neutral, PublicKeyToken=c273bd375e695f9c"))
				asm = (AppDomain.CurrentDomain).Load(Properties.Resources.SQLite_NET);*/
			else if (ExtString.Equals(args.Name, "System.Data.SQLite, Version=1.0.90.0, "
				+ "Culture=neutral, PublicKeyToken=db937bc2d44ff139"))
				asm = (AppDomain.CurrentDomain).Load(Properties.Resources.System_Data_SQLite);
            return asm;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //disable ContextMenu in Nud_Pages
            Nud_Pages.ContextMenuStrip = new ContextMenuStrip();

            //allow dragdrop in richtextbox
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
            LV_Entries.GridLines = Properties.Settings.Default.DefGrid;
            PicBx_Cover.BackColor = Properties.Settings.Default.DefColour;
            if (Properties.Settings.Default.HideDate)
                LV_Entries.Columns[4].Width = 0;
            
            //set-up listview sorting & sizing
            LV_Entries.ListViewItemSorter = lvSortObj;
            LV_Entries.Select();

            //Load DB and run tutorial
			#if !DEBUG
			if (!ConvertCruft()
					&& Properties.Settings.Default.FirstRun) {
                //Run tutorial on first execution
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.Save();

                Tutorial fmTut = new Tutorial();
                fmTut.ShowDialog();
                fmTut.Dispose();

                //set runtime sensitive default locations
                Properties.Settings.Default.SavLoc = Environment.CurrentDirectory;
                Properties.Settings.Default.DefLoc = Environment.CurrentDirectory;
            }
			#endif
        }

		private bool ConvertCruft()
		{
			//load database
			string sPath = Properties.Settings.Default.SavLoc != string.Empty ?
				Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
			sPath += "\\MangaDatabase.bin";
			
			//check existence
			bool bExist = false;
			if (File.Exists(sPath)) bExist = true;
			else {
				sPath = ExtString.RelativePath(sPath);
				bExist = (sPath != null);
			}

			if (bExist) {
				lData = FileSerializer.Deserialize<List<csEntry>>(sPath)
					?? new List<csEntry>(0);
				UpdateLV();

				//set up artist, tag, and type autocomplete
				List<string> lTags = new List<string>(lData.Count);
				HashSet<string> hsArtists = new HashSet<string>();
				HashSet<string> hsTypes = new HashSet<string>();
				for (int i = 0; i < CmbBx_Type.Items.Count; i++)
					hsTypes.Add(CmbBx_Type.Items[i].ToString());

				for (int i = 0; i < lData.Count; i++)
				{
                    hsTypes.Add(lData[i].sType);
                    hsArtists.Add(lData[i].sArtist);
					lTags.AddRange(lData[i].sTags.Split(','));
				}
				CmbBx_Artist.Items.AddRange(hsArtists.Select(x => x).ToArray());
				acTxBx_Tags.KeyWords = lTags.Select(x => x.Trim()).Distinct()
					.OrderBy(x => x, new TrueCompare()).ToArray();
				//csSQL.Import(sPath);
			}
			return bExist;
		}

        /* Prevent Form close if unsaved data present   */
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save changes to text automatically
            if (!bSavText) {
                Properties.Settings.Default.Notes = frTxBx_Notes.Text;
                bSavText = true;
            }

            //save changes to manga on request
            if (!bSavList) {
                switch (MessageBox.Show("Save before exiting?", Application.ProductName,
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

            //save Form's last position
            Properties.Settings.Default.Position =
                new Rectangle(this.Location, this.Size);
            Properties.Settings.Default.Save();
        }

        /* Display image from selected folder   */
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            switch (TabControl.SelectedIndex) {
                case 0:
                    if (indx == -1) {
                        Text = string.Format("{0}: {1:n0} entries",
                            (TxBx_Search.Text == "" && !ChkBx_ShowFav.Checked ?
                            "Manga Organizer" : "Returned"), LV_Entries.Items.Count);
                    }
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

        /* Switch tabs with ctrl+n shortcuts */
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
                SetData(Convert.ToInt32(LV_Entries.FocusedItem.SubItems[7].Text));
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
        { ResizeLV(); }

        /* Prevent user from changing column widths */
        private void LV_Entries_ColumnWidthChanging(
            object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = LV_Entries.Columns[e.ColumnIndex].Width;
        }

        private void LV_Entries_DoubleClick(object sender, EventArgs e)
        { OpenFile(); }

        /* More convenient listview focusing */
        private void LV_Entries_MouseHover(object sender, EventArgs e)
        {
            if (!LV_Entries.Focused && !Delay.Enabled)
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
        /* Select location of manga entry */
        private void Btn_Loc_Click(object sender, EventArgs e)
        {
            //try to auto-magically grab folder\file path
            string sPath = FindPath(TxBx_Loc.Text, CmbBx_Artist.Text, acTxBx_Title.Text)
                ?? Properties.Settings.Default.DefLoc;

            ExtFolderBrowserDialog xfbd = new ExtFolderBrowserDialog();
            xfbd.ShowBothFilesAndFolders = true;
            xfbd.RootFolder = Environment.SpecialFolder.MyComputer;
            xfbd.SelectedPath = sPath;

            if (xfbd.ShowDialog() == DialogResult.OK) {
                TxBx_Loc.Text = xfbd.SelectedPath;
                ThreadPool.QueueUserWorkItem(GetImage);

                if (CmbBx_Artist.Text == "" && acTxBx_Title.Text == "")
                    SetTitle(ExtString.GetNameSansExtension(xfbd.SelectedPath));
            }
            xfbd.Dispose();
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
            fmBrowse.Page = iPage;

            if(Directory.Exists(TxBx_Loc.Text)) {
                //process 'loose' images
                string[] sFiles = new string[0];
                if ((sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                        SearchOption.TopDirectoryOnly)).Length > 0) {
                    fmBrowse.Files = new List<string>(sFiles.Length);
                    fmBrowse.Files.AddRange(sFiles);
                    fmBrowse.ShowDialog();
                    iPage = (fmBrowse.Page > int.MaxValue) ?
                        int.MaxValue : Math.Abs(fmBrowse.Page);
                }
            } else {
                //process compressed images
                SCA.IArchive scArchive = SCA.ArchiveFactory.Open(@TxBx_Loc.Text);
                if (scArchive.Entries.Count() > 0) {
                    SCA.IArchiveEntry[] scEntries = scArchive.Entries.ToArray();
                    fmBrowse.Files = new List<string>(scEntries.Length);
                    for (int i = 0; i < scEntries.Length; i++) {
                        fmBrowse.Files.Add(scEntries[i].FilePath);
                    }
                    fmBrowse.Archive = scEntries;

                    fmBrowse.ShowDialog();
                    iPage = (fmBrowse.Page > int.MaxValue) ?
                        int.MaxValue : Math.Abs(fmBrowse.Page);
                }
                scArchive.Dispose();
            }
            fmBrowse.Dispose();
            GC.Collect(0);
        }

        /* Redraw cover image if size has changed */
        private void PicBx_Cover_Resize(object sender, EventArgs e)
        {
            if (PicBx_Cover.Image == null) return;
            SizeF sf = PicBx_Cover.Image.PhysicalDimension;
            if(sf.Width < sf.Height) {
                if (PicBx_Cover.Height > sf.Height)
                    bResize = true;
            }
            else /*if (sf.Width >= sf.Height)*/ {
                if (PicBx_Cover.Width - 1 > sf.Width)
                    bResize = true;
            }
        }
        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            if (bResize) {
                ThreadPool.QueueUserWorkItem(GetImage);
                bResize = false;
            }
        }
        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized
                && PicBx_Cover.Image != null) {
                ThreadPool.QueueUserWorkItem(GetImage);
            }
        }

        /* Dynamically update PicBx when user manually alters path */
        private void TxBx_Loc_TextChanged(object sender, EventArgs e)
        {
            if (indx != -1) MnTS_Edit.Visible = true;

            if (Directory.Exists(TxBx_Loc.Text)
                    || File.Exists(TxBx_Loc.Text)) {
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

            if (LV_Entries.SelectedItems.Count == 1) {
                iPos = LV_Entries.SelectedItems[0].Index;
                if (++iPos >= LV_Entries.Items.Count) iPos = 0;
            }

            ScrollTo(iPos);
        }
        private void Btn_GoUp_Click(object sender, EventArgs e)
        {
            if (LV_Entries.Items.Count == 0) return;
            int iPos = LV_Entries.Items.Count - 1;

            if (LV_Entries.SelectedItems.Count == 1) {
                iPos = LV_Entries.SelectedItems[0].Index;
                if (--iPos < 0) iPos = LV_Entries.Items.Count - 1;
            }

            ScrollTo(iPos);
        }
        private void Btn_Rand_Click(object sender, EventArgs e)
        {
            switch (LV_Entries.Items.Count) {
                case 0: return;
                case 1: if(indx != -1) return;
                    break;
                default: break;
            }
            
            Random rnd = new Random();
            int iCur = LV_Entries.SelectedItems.Count == 1 ?
                LV_Entries.SelectedItems[0].Index : -1;
            int iNew = 0;
            
            do {
                iNew = rnd.Next(LV_Entries.Items.Count);
            } while (iNew == iCur);
            ScrollTo(iNew);
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
                LV_Entries.FocusedItem.BackColor = Color.FromArgb(iLightYellow);
            else {
                if (LV_Entries.FocusedItem.Index % 2 == 0)
                    LV_Entries.FocusedItem.BackColor = Color.FromArgb(iLightGray);
                else LV_Entries.FocusedItem.BackColor = SystemColors.Window;
            }
        }

        /* Only enable edit when changes have been made */
        private void EntryAlt_Text(object sender, EventArgs e)
        {
            if (indx != -1) 
                MnTS_Edit.Visible = true;
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

        #region Custom Methods
        private void AddEntries()
        {
            UpdateLV();
            bSavList = false;
            LV_Entries.Select();
            if (indx != -1) ReFocus();
        }

        /* Alternate row colors in listview */
        private void Alternate()
        {
            if (Properties.Settings.Default.DefGrid) return;
            for (int i = 0; i < LV_Entries.Items.Count; i++) {
                if (LV_Entries.Items[i].SubItems[6].Text.Length == 5)
                    continue;
                LV_Entries.Items[i].BackColor = (i % 2 != 0) ?
                    Color.FromArgb(iLightGray) : SystemColors.Window;
            }
        }

        private static string FindPath(
            string sPath, string sArtist, string sTitle)
        {
            if (!File.Exists(sPath) && !Directory.Exists(sPath))
            {
                sPath = (!string.IsNullOrEmpty(sArtist)) ?
                    string.Format("{0}\\[{1}] {2}",
                        (!string.IsNullOrEmpty(Properties.Settings.Default.DefLoc)) ? 
                            Properties.Settings.Default.DefLoc : Environment.CurrentDirectory,
                    sArtist, sTitle) : sTitle;

                if (!Directory.Exists(sPath))
                {
                    if (File.Exists(sPath + ".zip"))
                        sPath += ".zip";
                    else if (File.Exists(sPath + ".cbz"))
                        sPath += ".cbz";
                    else if (File.Exists(sPath + ".rar"))
                        sPath += ".rar";
                    else if (File.Exists(sPath + ".cbr"))
                        sPath += ".cbr";
                    else if (File.Exists(sPath + ".7z"))
                        sPath += ".7z";
                    else sPath = ExtString.RelativePath(sPath);

                    if (!Directory.Exists(sPath) && !File.Exists(sPath))
                        sPath = null;
                }
            }
            return sPath;
        }

        private void GetImage(Object obj)
        {
            BeginInvoke(new DelVoid(SetPicBxNull));

            //Get cover and filecount
            if(File.Exists(TxBx_Loc.Text)) {
                SetPicBxImage(TxBx_Loc.Text);
            } else {
                string[] sFiles = new string[0];
                if ((sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                        SearchOption.TopDirectoryOnly)).Length > 0) {
                    SetPicBxImage(sFiles[0]);
                    Invoke(new DelInt(SetNudCount), sFiles.Length);
                } else {
                    Invoke(new DelInt(SetOpenStatus), 0);
                }
            }
        }

        private static int InsertText(Control c, string sAdd, int iStart)
        {
            c.Text = c.Text.Insert(iStart, sAdd);
            return iStart + sAdd.Length;
        }

        /* Ensure file is a valid archive */
        public static bool IsArchive(string sPath)
        {
            bool bArchive = false;
            if(File.Exists(sPath)) {
				try {
					switch(Path.GetExtension(sPath)) {
						case ".zip":
						case ".cbz":
							bArchive = SCA.Zip.ZipArchive.IsZipFile(sPath);
							break;
						case ".rar":
						case ".cbr":
							bArchive = SCA.Rar.RarArchive.IsRarFile(sPath);
							break;
						case ".7z":
							bArchive = SCA.SevenZip.SevenZipArchive.IsSevenZipFile(sPath);
							break;
					}
				} catch (IOException) {
					MessageBox.Show("The following file is corrupted:\n" + sPath,
						Application.ProductName, MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
            }
            return bArchive;
        }

        /* Parse EH metadata into local fields */
        private void LoadEH(string sURL)
        {
            string[] asResp = new string[0];
            this.Cursor = Cursors.WaitCursor;
            Text = "Sending request...";

            asResp = csEHSearch.LoadMetadata(sURL);
            if (asResp.Length == 6) {
                Text = "Parsing metadata...";
                Tb_View.SuspendLayout();

                SetTitle(asResp[0]); //set artist/title
                CmbBx_Type.Text = asResp[1]; //set entry type

                //set date
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                Dt_Date.Value = dt.AddSeconds((long)Convert.ToDouble(asResp[2]));

                Nud_Pages.Value = Convert.ToInt32(asResp[3]); //set page count
                srRating.SelectedStar = (int)Convert.ToDouble(asResp[4]); //set star rating

                //set tags
                if (acTxBx_Tags.Text == string.Empty) {
                    acTxBx_Tags.Text = asResp[5];
                }
                else {
                    List<string> lRaw = new List<string>(20);
                    lRaw.AddRange(acTxBx_Tags.Text.Split(','));
                    lRaw.AddRange(asResp[5].Split(','));
                    string[] sRaw = lRaw.Select(
                        x => x.Trim()).Distinct().Where(
                        x => !string.IsNullOrEmpty(x)).ToArray<string>();
                    acTxBx_Tags.Text = String.Join(", ", sRaw);
                }

                Tb_View.ResumeLayout();
                Text = "Finished";
            }
            else {
                Text = "The URL was invalid or the connection timed out.";
                MessageBox.Show(this.Text,
                    Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            this.Cursor = Cursors.Default;
        }

        /* Remove all entries less than 5 stars */
        private void OnlyFavs()
        {
            Cursor = Cursors.WaitCursor;
            List<ListViewItem> lFavs = new List<ListViewItem>(LV_Entries.Items.Count);

            for (int i = 0; i < LV_Entries.Items.Count; i++) {
                if (LV_Entries.Items[i].SubItems[6].Text.Length == 5)
                    lFavs.Add(LV_Entries.Items[i]);
            }
            
            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(lFavs.ToArray());
            LV_Entries.EndUpdate();

            Text = string.Format("Returned: {0:n0} entries", LV_Entries.Items.Count);
            Cursor = Cursors.Default;
        }

        /* Open image\zip with default program */
        private void OpenFile()
        {
            if (PicBx_Cover.Image == null)
                return;

            string sPath = TxBx_Loc.Text;
            if(Directory.Exists(sPath)) {
                string[] sFiles = ExtDir.GetFiles(sPath);
                if (sFiles.Length > 0) sPath = sFiles[0];
            }

            string sProg = Properties.Settings.Default.DefProg;
            if(sProg == "")
                System.Diagnostics.Process.Start("\"" + sPath + "\"");
            else System.Diagnostics.Process.Start(sProg, "\"" + sPath + "\"");
        }

        /* Convert number to string of stars */
        private static string RatingFormat(byte byVal)
        { return new string('☆', byVal); }

        private void ReFocus()
        {
            for (int i = 0; i < LV_Entries.Items.Count; i++)
                if (LV_Entries.Items[i].SubItems[1].Text.Length == lData[indx].sTitle.Length
                    && LV_Entries.Items[i].SubItems[1].Text == lData[indx].sTitle) {
                    ScrollTo(i);
                    break;
                }
        }

        private void Reset()
        {
            //reset Form title
            Tb_View.SuspendLayout();
            Text = string.Format("{0}: {1:n0} entries",
                (TxBx_Search.Text == "" && !ChkBx_ShowFav.Checked ? 
                "Manga Organizer" : "Returned"), LV_Entries.Items.Count);

            //Tb_Browse
            LV_Entries.FocusedItem = null;
            LV_Entries.SelectedItems.Clear();
            iPage = -1;
            indx = -1;

            //Tb_View
            TxBx_Loc.Clear();
            acTxBx_Tags.Clear();
            acTxBx_Title.Clear();
            frTxBx_Desc.Clear();
            Nud_Pages.Value = 0;
            CmbBx_Artist.Text = "";
            CmbBx_Type.Text = "Manga";
            Dt_Date.Value = DateTime.Now;
            srRating.SelectedStar = 0;
            SetPicBxNull();

            //Mn_EntryOps
            acTxBx_Tags.SetScroll();
            MnTS_New.Visible = true;
            MnTS_Del.Visible = false;
            MnTS_Edit.Visible = false;
            MnTS_Open.Visible = false;
            Tb_View.ResumeLayout();
        }

        private void ResizeLV()
        {
            //remaining combined column width
            int iStatic = LV_Entries.Columns[2].Width + LV_Entries.Columns[4].Width
                + LV_Entries.Columns[5].Width + LV_Entries.Columns[6].Width;
            int iMod = (LV_Entries.Width - iStatic) / 10;

            LV_Entries.BeginUpdate();
            LV_Entries.Columns[0].Width = iMod * 2; //artist
            LV_Entries.Columns[1].Width = iMod * 4; //title
            LV_Entries.Columns[3].Width = iMod * 4; //tags

            /* append remaining width to colTags */
            ColTags.Width += LV_Entries.DisplayRectangle.Width - iStatic
                - LV_Entries.Columns[0].Width
                - LV_Entries.Columns[1].Width
                - LV_Entries.Columns[3].Width;
            LV_Entries.EndUpdate();
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
            LV_Entries.TopItem = LV_Entries.Items[iPos];
        }

        /* Filter lvi by search terms (Check MnTs_Edit after changes)  */
        private void Search()
        {
            Cursor = Cursors.WaitCursor;
            string[] sTags = TxBx_Search.Text.Split(' ');
            csSearchTerm[] aTerms = new csSearchTerm[sTags.Length];
            List<ListViewItem> lItems = new List<ListViewItem>(lData.Count);

            //format text into search parameters
            for (int i = 0; i < sTags.Length; i++)
                aTerms[i] = new csSearchTerm(sTags[i]);
            
            //compare entries to search parameters
            for (int i = 0; i < lData.Count; i++) {
                bool bInc = true;
                for (int n = 0; n < aTerms.Length; n++) {
                    if (!aTerms[n].Equals(lData[i])) {
                        bInc = false;
                        break;
                    }
                }
                //reject entry if no match (or filtered by param)
                if (!bInc) continue;

                //else add to list
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                if (lData[i].byRat == 5) lvi.BackColor = Color.FromArgb(iLightYellow);
                lvi.SubItems.AddRange(new string[] {
                    lData[i].sTitle,
                    lData[i].iPages.ToString(),
                    lData[i].sTags,
                    lData[i].dtDate.ToString("MM/dd/yy"),
                    lData[i].sType,
                    RatingFormat(lData[i].byRat),
                    i.ToString()
                });
                lItems.Add(lvi);
            }
            
            LV_Entries.BeginUpdate();
            LV_Entries.Items.Clear();
            LV_Entries.Items.AddRange(lItems.ToArray());
            LV_Entries.Sort();
            Alternate();
            LV_Entries.EndUpdate();
            Text = string.Format("Returned: {0:n0} entries", LV_Entries.Items.Count);
            Cursor = Cursors.Default;

            //prevent loss of other parameters
            if (ChkBx_ShowFav.Checked) OnlyFavs();
        }

        private void SetData(int iNewIndx)
        {
            if ((indx = iNewIndx) == -1)
            { Reset(); return; }
            
            Tb_View.SuspendLayout();
            Text = "Selected: " + lData[indx].ToString();
            acTxBx_Title.Text = lData[indx].sTitle;
            CmbBx_Artist.Text = lData[indx].sArtist;
            TxBx_Loc.Text = lData[indx].sLoc;
            frTxBx_Desc.Text = lData[indx].sDesc;
            CmbBx_Type.Text = lData[indx].sType;
            Dt_Date.Value = lData[indx].dtDate;
            srRating.SelectedStar = lData[indx].byRat;
            Nud_Pages.Value = lData[indx].iPages;
            acTxBx_Tags.Text = lData[indx].sTags;

            acTxBx_Tags.SetScroll();
            MnTS_New.Visible = false;
            MnTS_Edit.Visible = false;
            MnTS_Del.Visible = true;
            Tb_View.ResumeLayout();

            //check for relativity
            if(TxBx_Loc.Text != "")
            {
                string sResult = FindPath(TxBx_Loc.Text, CmbBx_Artist.Text, acTxBx_Title.Text);
                if (sResult != null) TxBx_Loc.Text = sResult;
            }
        }

        private void SetNudCount(int iNum)
        {
            Nud_Pages.Value = iNum;
            TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
            SetOpenStatus(1);
        }

        private void SetOpenStatus(int iExists)
        {
            MnTS_Open.Visible = (iExists == 1);
        }

        private void SetPicBxImage(string sPath)
        {
            if(IsArchive(sPath)) {
                SCA.IArchive scArch = SCA.ArchiveFactory.Open(sPath);
                int iCount = scArch.Entries.Count();
                BeginInvoke(new DelInt(SetNudCount), iCount);

                if(iCount > 0) {
                    //account for terrible default zip-sorting
                    int iFirst = 0;
                    SCA.IArchiveEntry[] scEntries = 
                        scArch.Entries.ToArray();
                    List<string> lEntries = new List<string>();
                    for(int i = 0; i < iCount; i++) {
                        if(scEntries[i].FilePath.EndsWith("jpg")
                                || scEntries[i].FilePath.EndsWith("jpeg")
                                || scEntries[i].FilePath.EndsWith("png")
                                || scEntries[i].FilePath.EndsWith("bmp"))
                            lEntries.Add(scEntries[i].FilePath);
                    }
                    if (lEntries.Count == 0) return;
                    lEntries.Sort(new TrueCompare());
                    for (int i = 0; i < iCount; i++) {
                        if(scEntries[i].FilePath.Equals(lEntries[0]))
                            break;
                        iFirst++;
                    }

                    //load image
                    MemoryStream ms = new MemoryStream();
                    try {
                        scEntries[iFirst].WriteTo(ms);
                        using (Bitmap bmpTmp = new Bitmap(ms)) {
                            PicBx_Cover.Image = ExtImage.Scale(bmpTmp,
                                PicBx_Cover.Width, PicBx_Cover.Height);
                        }
                    } catch (Exception Exc) {
                        MessageBox.Show("The following file could not be loaded:\n" + 
                            sPath + '\\' + scEntries[iFirst].FilePath,
                            Application.ProductName, MessageBoxButtons.OK, 
                            MessageBoxIcon.Exclamation);
                        Console.WriteLine(Exc.Message);
                    } finally {
                        ms.Dispose();
                    }
                }
                scArch.Dispose();
            } else {
                TrySet(sPath);
            }
        }
        private void TrySet(string s)
        {
            try {
                using (Bitmap bmpTmp = new Bitmap(s)) {
                    PicBx_Cover.Image = ExtImage.Scale(bmpTmp,
                        PicBx_Cover.Width, PicBx_Cover.Height);
                }
            } catch (Exception Exc) {
                MessageBox.Show("The following file could not be loaded:\n" + s,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Console.WriteLine(Exc.Message);
            } finally {
                GC.Collect(0);
            }
        }

        private void SetPicBxNull()
        {
            if (PicBx_Cover.Image != null) {
                PicBx_Cover.Image.Dispose();
                PicBx_Cover.Image = null;
                GC.Collect(0);
            }
        }

        private void SetTitle(string sRaw)
        {
            Tb_View.SuspendLayout();
            string[] asProc = SplitTitle(sRaw);

            if(asProc[0] != string.Empty) {
                if(CmbBx_Artist.Text == string.Empty) {
                    CmbBx_Artist.Text = asProc[0];
                }
                if(acTxBx_Title.Text == string.Empty) {
                    acTxBx_Title.Text = asProc[1];
                }
            }
            else {
                acTxBx_Title.Text = acTxBx_Title.Text.Insert(
                    acTxBx_Title.SelectionStart, asProc[1]);
                acTxBx_Title.SelectionStart += asProc[1].Length;
            }
            Tb_View.ResumeLayout();
        }

        public static string[] SplitTitle(string sRaw)
        {
            string[] asName = new string[2] { "", "" };
            string sCircle = "";

            //strip out circle info & store
            if (sRaw.StartsWith("(")) {
                int iPos = sRaw.IndexOf(')');
                if (iPos > -1 && ++iPos < sRaw.Length) {
                    sCircle = sRaw.Substring(0, iPos);
                    sRaw = sRaw.Remove(0, iPos).TrimStart();
                }
            }
            
            //split fields using EH format
            int iA = sRaw.IndexOf('['), 
                iB = sRaw.IndexOf(']');
            if ((iA > -1 && iB > -1) && iA < iB && iB + 1 < sRaw.Length) {
                //Re-format for Artist/Title fields
                asName[0] = sRaw.Substring(iA + 1, iB - iA - 1).Trim();
                asName[1] = sRaw.Substring(iB + 1).Trim();
                if (sCircle != "") asName[1] += " " + sCircle;
            } else  {
                asName[1] = sRaw;
            }
            return asName;
        }

        /* Refresh listview contents */
        private void UpdateLV()
        {
            if (!string.IsNullOrEmpty(TxBx_Search.Text)) { 
                Search(); return;
            }

            Cursor = Cursors.WaitCursor;
            Text = string.Format("Manga Organizer: {0:n0} entries", lData.Count);
            ListViewItem[] aItems = new ListViewItem[lData.Count];
            
            for (int i = 0; i < lData.Count; i++) {
                ListViewItem lvi = new ListViewItem(lData[i].sArtist);
                if (lData[i].byRat == 5) lvi.BackColor = Color.FromArgb(iLightYellow);
                lvi.SubItems.AddRange(new string[7] {
                    lData[i].sTitle,
                    lData[i].iPages.ToString(),
                    lData[i].sTags,
                    lData[i].dtDate.ToString("MM/dd/yy"),
                    lData[i].sType,
                    RatingFormat(lData[i].byRat),
                    i.ToString()
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

        #region Menu: Entry Operations
        private void MnTS_New_Click(object sender, EventArgs e)
        {
            //reject when title is unfilled
            if (acTxBx_Title.Text == "") {
                MessageBox.Show("Title cannot be empty.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            csEntry en = new csEntry(CmbBx_Artist.Text, acTxBx_Title.Text, 
                TxBx_Loc.Text, frTxBx_Desc.Text, acTxBx_Tags.Text, CmbBx_Type.Text,
                Dt_Date.Value.Date, Nud_Pages.Value, srRating.SelectedStar);

            if (!lData.Contains(en)) {
                if (MessageBox.Show("Are you sure you wish to add:\n\"" + en + "\"",
                        Application.ProductName, MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question) == DialogResult.Yes) {
                    lData.Add(en);

                    //add artist to autocomplete
                    if (!CmbBx_Artist.Items.Contains(CmbBx_Artist.Text)) {
                        HashSet<string> hsArtists = new HashSet<string>();
                        hsArtists.Add(CmbBx_Artist.Text);

                        for (int i = 0; i < lData.Count; i++) {
                            hsArtists.Add(lData[i].sArtist);
                        }

                        CmbBx_Artist.Items.Clear();
                        CmbBx_Artist.Items.AddRange(hsArtists.ToArray());
                    }
                    acTxBx_Tags.UpdateAutoComplete();

                    //update LV_Entries
                    AddEntries();
                    Reset();
                }
            }
            else MessageBox.Show("This item already exists in the database.",
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void MnTS_Edit_Click(object sender, EventArgs e)
        {
            if (indx == -1) return;

            //overwrite entry properties
            lData[indx] = new csEntry(CmbBx_Artist.Text, acTxBx_Title.Text, 
                TxBx_Loc.Text, frTxBx_Desc.Text, acTxBx_Tags.Text, CmbBx_Type.Text,
                Dt_Date.Value, Nud_Pages.Value, srRating.SelectedStar);
            Text = "Edited entry: " + lData[indx].ToString();
            acTxBx_Tags.Text = lData[indx].sTags;
            acTxBx_Tags.UpdateAutoComplete();

            //update LV_Entries & maintain selection
            ListViewItem lvi = LV_Entries.FocusedItem;
            if (lData[indx].byRat == 5)
                lvi.BackColor = Color.FromArgb(iLightYellow);
            lvi.SubItems[0].Text = lData[indx].sArtist;
            lvi.SubItems[1].Text = lData[indx].sTitle;
            lvi.SubItems[2].Text = lData[indx].iPages.ToString();
            lvi.SubItems[3].Text = lData[indx].sTags;
            lvi.SubItems[4].Text = lData[indx].dtDate.ToString("MM/dd/yy");
            lvi.SubItems[5].Text = lData[indx].sType;
            LV_Entries.Sort();

            //check if entry should still be displayed
            int iPos = lvi.Index;
            if (ChkBx_ShowFav.Checked 
                    && !(lData[indx].byRat == 5))  {
                lvi.Remove();
            }
            else if (TxBx_Search.Text != "") {
                string[] sTags = TxBx_Search.Text.Split(' ');
                List<csSearchTerm> lTerms = new List<csSearchTerm>(5);

                for (int i = 0; i < sTags.Length; i++)
                    lTerms.Add(new csSearchTerm(sTags[i]));

                for (int y = 0; y < lTerms.Count; y++) {
                    if (!lTerms[y].Equals(lData[indx])) {
                        lvi.Remove();
                        break;
                    }
                }
                Text = string.Format("Returned: {0:n0} entries", LV_Entries.Items.Count);
            }
            else ReFocus();

            Alternate();
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
            string msg = ""; bool bFile = false, bRst = true;
            if (!(bRst = ExtDir.Restricted(lData[indx].sLoc)))
                msg = "Do you want to delete the source directory as well?";
            else if (bFile = File.Exists(lData[indx].sLoc))
                msg = "Do you want to delete the source file as well?";
            else msg = "Are you sure you wish to delete this entry?";
            DialogResult dResult = MessageBox.Show(msg, Application.ProductName, 
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if ((bRst && !bFile) && dResult == DialogResult.No)
                dResult = DialogResult.Cancel;

            //delete source file\directory
            if (dResult == DialogResult.Yes) {
                if (!bRst) {
                    //warn user before deleting subdirectories
                    int iNumDir = Directory.GetDirectories(lData[indx].sLoc).Length;
                    if (iNumDir > 0) {
                        dResult = MessageBox.Show(string.Format("This directory contains {0} subfolder(s),\n" +
                            "are you sure you want to delete them?", iNumDir),
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    }
                    if (dResult == DialogResult.Yes) {
                        this.Cursor = Cursors.WaitCursor;
                        Directory.Delete(lData[indx].sLoc, true);
                        this.Cursor = Cursors.Default;
                    }
                }
                else if (bFile) {
                    this.Cursor = Cursors.WaitCursor;
                    File.Delete(lData[indx].sLoc);
                    this.Cursor = Cursors.Default;
                }
            }
            
            //remove from database
            if (dResult != DialogResult.Cancel) {
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

        #region Menu: Main
        private void MnTS_CopyTitle_Click(object sender, EventArgs e)
        {
            if(acTxBx_Title.Text == "") {
                MessageBox.Show("The title field cannot be empty.",
                    Application.ProductName, MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
                return;
            }

            Clipboard.SetText(string.Format((CmbBx_Artist.Text != "") 
                ? "[{0}] {1}" : "{1}", CmbBx_Artist.Text, acTxBx_Title.Text));
            Text = "Name copied to clipboard";
        }

        private void MnTS_OpenSource_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(TxBx_Loc.Text)) {
                System.Diagnostics.Process.Start("explorer.exe", "\"" + TxBx_Loc.Text + "\"");
            }
            else if (File.Exists(TxBx_Loc.Text)) {
                System.Diagnostics.Process.Start("\"" + TxBx_Loc.Text + "\"");
            }
        }

        /* Uses EH API to get metadata from gallery URL */
        private void MnTS_LoadUrl_Click(object sender, EventArgs e)
        {
            GetUrl fmGet = new GetUrl();
            fmGet.StartPosition = FormStartPosition.Manual;
            fmGet.Location = this.Location;
            fmGet.ShowDialog();

            if (fmGet.DialogResult == DialogResult.OK) {
                LoadEH(fmGet.Url);
            }
            fmGet.Dispose();
        }

        private void MnTs_SearchEH_Click(object sender, EventArgs e)
        {
            Suggest fmSuggest = new Suggest();
            fmSuggest.ShowDialog();

            if(fmSuggest.DialogResult == DialogResult.OK) {
                LoadEH(fmSuggest.sChoice);
            }
            fmSuggest.Dispose();
        }

        private void MnTS_Save_Click(object sender, EventArgs e)
        { SaveData(); }

        private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
        {
            string sPath = Properties.Settings.Default.SavLoc != "" ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;

            if (Directory.Exists(sPath))
                System.Diagnostics.Process.Start(sPath);
            else MessageBox.Show("This directory no longer exists.", Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MnTS_Stats_Click(object sender, EventArgs e)
        {
            Stats fmStats = new Stats();
            fmStats.lCurr = lData;
            fmStats.Show();
        }

        private void Mn_Settings_Click(object sender, EventArgs e)
        {
            string sOld = Properties.Settings.Default.SavLoc;
            Form fmSet = new Settings();
            fmSet.ShowDialog();

            if (fmSet.DialogResult == DialogResult.Yes)
            {
                LV_Entries.GridLines = Properties.Settings.Default.DefGrid;
                PicBx_Cover.BackColor = Properties.Settings.Default.DefColour;
                if (Properties.Settings.Default.HideDate)
                    LV_Entries.Columns[4].Width = 0;
                else LV_Entries.Columns[4].Width = 70;
                ResizeLV();

                //Update new DB save location
                if (sOld != Properties.Settings.Default.SavLoc)
                {
                    string sNew = Properties.Settings.Default.SavLoc + "\\MangaDatabase.bin";
                    sOld += "\\MangaDatabase.bin";

                    //move old save to new location
                    if (File.Exists(sNew)
                            && MessageBox.Show("Open existing database at:\n" + sNew,
                            Application.ProductName, MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        lData = FileSerializer.Deserialize<List<csEntry>>(sNew);

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
                    else if (File.Exists(sOld))
                        File.Move(sOld, sNew);

                    Text = "Default save location changed";
                }
            }
            fmSet.Dispose();
        }

        private void MnTS_Tutorial_Click(object sender, EventArgs e)
        {
            Tutorial fmTut = new Tutorial();
            fmTut.ShowDialog();
            fmTut.Dispose();
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

        #region Menu_Context
        private void Mn_TxBx_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //ensure element has focus
            if (Mn_TxBx.SourceControl.CanFocus) Mn_TxBx.SourceControl.Focus();
            if (Mn_TxBx.SourceControl.CanSelect) Mn_TxBx.SourceControl.Select();

            //check what properties are available
            bool bUndo = false, bSelect = false;
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    FixedRichTextBox fr = ActiveControl as FixedRichTextBox;
                    if (fr.CanUndo) bUndo = true;
                    if (fr.SelectionLength > 0) bSelect = true;
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
                    TextBox txt = ActiveControl as TextBox;
                    if (txt.CanUndo) bUndo = true;
                    if (txt.SelectionLength > 0) bSelect = true;
                    break;
                case "ComboBox":
                    ComboBox cb = ActiveControl as ComboBox;
                    if (cb.SelectionLength > 0) bSelect = true;
                    bUndo = true;
                    break;
            }

            MnTx_Undo.Enabled = bUndo;
            MnTx_Cut.Enabled = bSelect;
            MnTx_Copy.Enabled = bSelect;
            MnTx_Delete.Enabled = bSelect;
        }

        private void MnTx_Undo_Click(object sender, EventArgs e)
        {
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).Undo();
                    if (TabControl.SelectedIndex == 2 && !frTxBx_Notes.CanUndo) 
                        bSavText = true;
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
                    ((TextBox)ActiveControl).Undo();
                    break;
                case "ComboBox":
                    ((ComboBox)ActiveControl).ResetText();
                    break;
            }
        }

        private void MnTx_Cut_Click(object sender, EventArgs e)
        {
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).Cut();
                    if (TabControl.SelectedIndex == 2) bSavText = false;
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
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
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).Copy();
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
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
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    FixedRichTextBox fr = ((FixedRichTextBox)ActiveControl);
                    fr.SelectedText = sAdd;
                    if (TabControl.SelectedIndex == 2) bSavText = false;
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
                    if(ActiveControl.Name == "TxBx_Search") {
                        string[] asTitle = SplitTitle(sAdd);
                        sAdd = (asTitle[0] == "") ? sAdd : 
                            string.Format("artist:{0} title:{1}",
                            asTitle[0].Replace(' ', '_'), asTitle[1].Replace(' ', '_'));
                        TxBx_Search.SelectionStart = InsertText(
                            TxBx_Search, sAdd, TxBx_Search.SelectionStart);
                        Search();
                        break;
                    }

                    TextBox txt = (TextBox)ActiveControl;
                    if (txt.Name == "acTxBx_Tags" && sAdd.Contains("\r")) {
                        IEnumerable<string> ie = sAdd.Split('(', '\n');
                        ie = ie.Where(s => !s.EndsWith("\r") 
                            && !s.EndsWith(")") && !s.Equals(""));
                        ie = ie.Select(s => s.TrimEnd());
                        sAdd = string.Join(", ", ie.ToArray());
                    }
                    txt.SelectedText = sAdd;
                    break;
                case "ComboBox":
                    ComboBox cb = (ComboBox)ActiveControl;
                    
                    if(sAdd.Contains('[')) SetTitle(sAdd);
                    else cb.SelectedText = sAdd;
                    break;
            }
        }

        private void MnTx_Delete_Click(object sender, EventArgs e)
        {
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).SelectedText = "";
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
                    ((TextBox)ActiveControl).SelectedText = "";
                    break;
                case "ComboBox":
                    ((ComboBox)ActiveControl).SelectedText = "";
                    break;
            }
        }

        private void MnTx_SelAll_Click(object sender, EventArgs e)
        {
            switch (Mn_TxBx.SourceControl.GetType().Name) {
                case "FixedRichTextBox":
                    ((FixedRichTextBox)ActiveControl).SelectAll();
                    break;
                case "TextBox":
                case "AutoCompleteTagger":
                    ((TextBox)ActiveControl).SelectAll();
                    break;
                case "ComboBox":
                    ((ComboBox)ActiveControl).SelectAll();
                    break;
            }
        }

        private void frTxBx_KeyDown(object sender, KeyEventArgs e)
        {
            /* Prevent mixed font types when c/p  */
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
                FixedRichTextBox frTxBx = sender as FixedRichTextBox;
                frTxBx.SelectionStart = InsertText(
                    frTxBx, Clipboard.GetText(), frTxBx.SelectionStart);
                if (TabControl.SelectedIndex == 2) bSavText = false;
                e.Handled = true;
            }
            /* Prevent console beep when reaching start/end of txbx */
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left) {
                FixedRichTextBox frTxBx = sender as FixedRichTextBox;
                if ((e.KeyCode == Keys.Right && frTxBx.SelectionStart == frTxBx.TextLength) ||
                    (e.KeyCode == Keys.Left && frTxBx.SelectionStart == 0))
                    e.Handled = true;
            }
        }
        #endregion

        #region Drag/Drop Events
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

            switch ((sender as Control).Name) {
                case "frTxBx_Desc":
                    frTxBx_Desc.SelectionStart = InsertText(
                        frTxBx_Desc, sAdd, frTxBx_Desc.SelectionStart);
                    break;
                case "frTxBx_Notes":
                    frTxBx_Notes.SelectionStart = InsertText(
                        frTxBx_Notes, sAdd, frTxBx_Notes.SelectionStart);
                    break;
                case "CmbBx_Artist":
                    if (sAdd.Contains('[')) SetTitle(sAdd);
                    else CmbBx_Artist.SelectionStart = InsertText(
                        CmbBx_Artist, sAdd, CmbBx_Artist.SelectionStart);
                    break;
                case "acTxBx_Title":
                    if (sAdd.Contains('[')) SetTitle(sAdd);
                    else acTxBx_Title.SelectionStart = InsertText(
                        acTxBx_Title, sAdd, acTxBx_Title.SelectionStart);
                    break;
                case "acTxBx_Tags":
                    if (sAdd.Contains("\r")) {
                        IEnumerable<string> ie = sAdd.Split('(', '\n');
                        ie = ie.Where(s => !s.EndsWith("\r") && !s.EndsWith(")") && !s.Equals(""));
                        ie = ie.Select(s => s.TrimEnd());
                        sAdd = string.Join(", ", ie.ToArray());
                    }
                    acTxBx_Tags.SelectionStart = InsertText(
                        acTxBx_Tags, sAdd, acTxBx_Tags.SelectionStart);
                    break;
                case "TxBx_Search":
                    string[] asTitle = SplitTitle(sAdd);
                    sAdd = (asTitle[0] == "") ? sAdd : 
                        string.Format("artist:{0} title:{1}", 
                        asTitle[0].Replace(' ', '_'), asTitle[1].Replace(' ', '_'));
                    TxBx_Search.SelectionStart = InsertText(
                        TxBx_Search, sAdd, TxBx_Search.SelectionStart);
                    Search();
                    break;
            }
        }

        private void TxBx_Loc_DragDrop(object sender, DragEventArgs e)
        {
            string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
            TxBx_Loc.Text = asDir[0];

            if(Directory.Exists(asDir[0])) {
                if (CmbBx_Artist.Text == "" && acTxBx_Title.Text == "") {
                    SetTitle(Path.GetDirectoryName(asDir[0]));
                    ThreadPool.QueueUserWorkItem(GetImage);
                }
            }
            else if (File.Exists(asDir[0]) && IsArchive(asDir[0])) {
                if (CmbBx_Artist.Text == "" && acTxBx_Title.Text == "") {
                    SetTitle(ExtString.GetNameSansExtension(asDir[0]));
                    ThreadPool.QueueUserWorkItem(GetImage);
                }
            }
        }

        private void Dt_Date_DragDrop(object sender, DragEventArgs e)
        {
            DateTime dtDrop;
            if (DateTime.TryParse(
                    (string)e.Data.GetData(DataFormats.Text),
                    out dtDrop)) {
                Dt_Date.Value = dtDrop;
            }
            else MessageBox.Show("The dropped text was not a valid date.",
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void Nud_Pages_DragDrop(object sender, DragEventArgs e)
        {
            decimal dcDrop;
            if (decimal.TryParse(
                    (string)e.Data.GetData(DataFormats.Text), 
                    out dcDrop)) {
                Nud_Pages.Value = dcDrop;
            }
            else MessageBox.Show("The dropped text was not a valid value.",
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /* Allow dropping of folders/zips onto LV_Entries (& TxBx_Loc) */
        private void LV_Entries_DragEnter(object sender, DragEventArgs e)
        {
            string[] sTemp = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (sTemp == null) return;
            
            FileAttributes fa = File.GetAttributes(sTemp[0]);Text = fa.ToString();
            if (fa == FileAttributes.Directory || IsArchive(sTemp[0])
                    || fa.ToString() == "Directory, Archive")
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /* Adds folder(s) to database when dragged over LV_Entries */
        private void LV_Entries_DragDrop(object sender, DragEventArgs e)
        {
            string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
            string sError = "";
            int iTrack = indx;

            //add all remaining folders
            for (int i = 0; i < asDir.Length; i++) {
                if (asDir[i] == ""
                    || (!Directory.Exists(asDir[i])
                        && !IsArchive(asDir[i]))) 
                    continue;

                //add item
                csEntry en = new csEntry(asDir[i]);
                if (!lData.Contains(en)) {
                    lData.Add(en);
                    indx = lData.Count - 1;

                    if (en.sArtist != "" && !CmbBx_Artist.Items.Contains(en.sArtist))
                        CmbBx_Artist.Items.Add(en.sArtist);
                }
                else sError += '\n' + asDir[i];
            }
            if (sError != "") {
                MessageBox.Show("The following path(s) already exists in the database:" + sError,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Update LV
            if (iTrack != indx)
                AddEntries();
        }
        #endregion
    }
}