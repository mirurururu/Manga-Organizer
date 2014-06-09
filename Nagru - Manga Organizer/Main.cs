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
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SCA = SharpCompress.Archive;

namespace Nagru___Manga_Organizer
{
  public partial class Main : Form
  {
    #region Properties
    delegate void DelVoid();
    delegate void DelInt(int iNum);
    delegate void DelString(string sMsg);

    LVsorter lvSortObj = new LVsorter(true);
    bool bSavNotes = true, bResize = false;
    int mangaID = -1, page = -1;
    #endregion

    #region Main Form
    public Main(string[] sFile)
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;

      //if database opened with "Shell->Open with..."
      if (sFile.Length > 0
              && sFile[0].EndsWith("\\MangaDB.sqlite")
              && Properties.Settings.Default.SavLoc != sFile[0].Substring(
                  0, sFile[0].Length - "\\MangaDB.sqlite".Length)) {
        Properties.Settings.Default.SavLoc = sFile[0];
        MessageBox.Show("Default database location changed to:\n\"" + sFile[0] + '\"',
            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void Main_Load(object sender, EventArgs e)
    {
      //disable ContextMenu in Nud_Pages
      Nud_Pages.ContextMenuStrip = new ContextMenuStrip();

      //set up global event catch for exceptions
      Application.ThreadException += Application_ThreadException;

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
    }

    private void Main_Shown(object sender, EventArgs e)
    {
      Cursor = Cursors.WaitCursor;
      Text = "Loading Database...";
      System.Threading.ThreadPool.QueueUserWorkItem(Database_Load);

      //run tutorial on first run
      if (Properties.Settings.Default.FirstRun) {
        //Run tutorial on first execution
        Properties.Settings.Default.FirstRun = false;
        Properties.Settings.Default.Save();

        Tutorial fmTut = new Tutorial();
        fmTut.Show();

        //set runtime sensitive default locations
        Properties.Settings.Default.SavLoc = Environment.CurrentDirectory;
        Properties.Settings.Default.DefLoc = Environment.CurrentDirectory;
      }
    }

    /* Prevent Form close if unsaved data present   */
    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
      //save changes to text automatically
      if (!bSavNotes) {
        Properties.Settings.Default.Notes = frTxBx_Notes.Text;
        bSavNotes = true;
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
          if (mangaID == -1) {
            Text = string.Format("{0}: {1:n0} entries",
                (TxBx_Search.Text == "" && !ChkBx_ShowFav.Checked ?
                "Manga Organizer" : "Returned"), LV_Entries.Items.Count);
          }
          this.AcceptButton = Btn_Clear;
          LV_Entries.Focus();
          break;
        case 1:
          if (mangaID != -1) {
            Text = "Selected: " + SQL.GetMangaTitle(mangaID);
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

    /* Send myself a report whenver an exception happens*/
    private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      //exit if the user has opted out
      if (!Properties.Settings.Default.SendReports)
        return;

      //exit if there (probably) isn't an internet connection
      if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        return;

      //set up addresses and error message
      const string sHost = "smtp.gmail.com"
        , sFromAddress = "nullJacobin@gmail.com"
        , sToAddress = "nagru@live.ca";
      string sMessage = string.Format("Message:\n{0}\n\nStackTrace:\n{1}\n\nTargetSite:\n{2}\n\nData:\n{3}"
          , e.Exception.Message
          , e.Exception.StackTrace
          , e.Exception.TargetSite
          , e.Exception.Data
        );

      //set up the mail provider to send through
      using (System.Net.Mail.SmtpClient smClient = new System.Net.Mail.SmtpClient {
        Host = sHost,
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(sFromAddress, Properties.Resources.mail)
      }) {
        //populate the message to send
        using (System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(sFromAddress, sToAddress) {
          Subject = "Manga Organizer: Exception Report",
          Body = sMessage
        }) {
          smClient.Send(msg);
        }
      }
    }
    #endregion

    #region Tab_Browse
    private void ClearSelection(object sender, EventArgs e)
    {
      if (mangaID != -1)
        Reset();
    }

    private void Btn_Scan_Click(object sender, EventArgs e)
    {
      Scan fmScan = new Scan();
      fmScan.delNewEntry = AddEntries;
      fmScan.delDone = fmScanDone;
      Btn_Scan.Enabled = false;

      fmScan.Show();
      fmScan.Select();
    }
    private void fmScanDone()
    {
      Btn_Scan.Enabled = true;
    }

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
      else
        Delay.Start();
    }
    private void Delay_Tick(object sender, EventArgs e)
    {
      Delay.Stop();
      UpdateLV();
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
        SetData(Int32.Parse(LV_Entries.FocusedItem.SubItems[colID.Index].Text));
      else
        Reset();
    }

    private void LV_Entries_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      //prevent sorting by tags
      if (e.Column == ColTags.Index)
        return;

      if (e.Column != lvSortObj.SortingColumn)
        lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
      else
        lvSortObj.SwapOrder();

      LV_Entries.Sort();
      Alternate();
    }

    /* Proportionally-resizes columns   */
    private void LV_Entries_Resize(object sender, EventArgs e)
    {
      ResizeLV();
    }

    /* Prevent user from changing column widths */
    private void LV_Entries_ColumnWidthChanging(
        object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
      e.NewWidth = LV_Entries.Columns[e.ColumnIndex].Width;
    }

    private void LV_Entries_DoubleClick(object sender, EventArgs e)
    {
      OpenFile();
    }

    /* More convenient listview focusing */
    private void LV_Entries_MouseHover(object sender, EventArgs e)
    {
      if (!LV_Entries.Focused && !Delay.Enabled)
        LV_Entries.Focus();
    }

    private void ChkBx_ShowFav_CheckedChanged(object sender, EventArgs e)
    {
      if (ChkBx_ShowFav.Checked)
        OnlyFavs();
      else
        UpdateLV();

      if (mangaID != -1 && SQL.GetMangaDetail(mangaID, "Rating") == "5")
        ReFocus();
      else
        Reset();

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
    {
      System.Diagnostics.Process.Start(e.LinkText);
    }

    /* Alternative to MnTS_Open */
    private void PicBx_Cover_Click(object sender, EventArgs e)
    {
      if (PicBx_Cover.Image == null)
        return;

      Browse_Img fmBrowse = new Browse_Img();
      fmBrowse.Page = page;

      if (Directory.Exists(TxBx_Loc.Text)) {
        //process 'loose' images
        string[] sFiles = new string[0];
        if ((sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
                SearchOption.TopDirectoryOnly)).Length > 0) {
          fmBrowse.Files = new List<string>(sFiles.Length);
          fmBrowse.Files.AddRange(sFiles);
          fmBrowse.ShowDialog();
          page = Math.Abs(fmBrowse.Page);
        }
      }
      else if (IsArchive(TxBx_Loc.Text)) {
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
          page = Math.Abs(fmBrowse.Page);
        }
        scArchive.Dispose();
      }
      else {
        MessageBox.Show("The following path is no longer valid:\n" + TxBx_Loc.Text,
          Application.ProductName, MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
      fmBrowse.Dispose();
      GC.Collect(0);
    }

    /* Redraw cover image if size has changed */
    private void PicBx_Cover_Resize(object sender, EventArgs e)
    {
      if (PicBx_Cover.Image == null)
        return;
      SizeF sf = PicBx_Cover.Image.PhysicalDimension;
      if (sf.Width < sf.Height) {
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
      if (mangaID != -1) {
        MnTS_Edit.Visible = true;
      }
      MnTS_Clear.Visible = true;

      if (Directory.Exists(TxBx_Loc.Text)
              || File.Exists(TxBx_Loc.Text)) {
        page = -1;
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
      if (LV_Entries.Items.Count == 0)
        return;
      int iPos = 0;

      if (LV_Entries.SelectedItems.Count == 1) {
        iPos = LV_Entries.SelectedItems[0].Index;
        if (++iPos >= LV_Entries.Items.Count)
          iPos = 0;
      }

      ScrollTo(iPos);
    }
    private void Btn_GoUp_Click(object sender, EventArgs e)
    {
      if (LV_Entries.Items.Count == 0)
        return;
      int iPos = LV_Entries.Items.Count - 1;

      if (LV_Entries.SelectedItems.Count == 1) {
        iPos = LV_Entries.SelectedItems[0].Index;
        if (--iPos < 0)
          iPos = LV_Entries.Items.Count - 1;
      }

      ScrollTo(iPos);
    }
    private void Btn_Rand_Click(object sender, EventArgs e)
    {
      /* if there are no items to select, or we have already
       * selected the only one, skip operation */
      if (LV_Entries.Items.Count == 0
          || (LV_Entries.Items.Count == 1 && mangaID != -1)) {
            return;
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
      if (mangaID == -1 || SQL.GetMangaDetail(mangaID, "Rating") == srRating.SelectedStar.ToString())
        return;

      //update rating
      SQL.UpdateRating(mangaID, srRating.SelectedStar);
      LV_Entries.SelectedItems[0].SubItems[ColRating.Index].Text =
          RatingFormat(srRating.SelectedStar);

      //set BackColor
      if (SQL.GetMangaDetail(mangaID, "Rating") == "5")
        LV_Entries.FocusedItem.BackColor = Color.FromArgb(
            Properties.Settings.Default.RowColorHighlight);
      else {
        if (LV_Entries.FocusedItem.Index % 2 == 0)
          LV_Entries.FocusedItem.BackColor = Color.FromArgb(
              Properties.Settings.Default.RowColorAlt);
        else
          LV_Entries.FocusedItem.BackColor = SystemColors.Window;
      }
    }

    /* Only enable edit when changes have been made */
    private void EntryAlt_Text(object sender, EventArgs e)
    {
      if (mangaID != -1) {
        MnTS_Edit.Visible = true;
      }
      MnTS_Clear.Visible = true;
    }
    private void EntryAlt_DtNum(object sender, EventArgs e)
    {
      if (mangaID != -1) {
        MnTS_Edit.Visible = true;
      }
      MnTS_Clear.Visible = true;
    }
    #endregion

    #region Tab_Notes
    /* Prevent loss of changes in note text   */
    private void frTxBx_Notes_TextChanged(object sender, EventArgs e)
    {
      if (bSavNotes)
        bSavNotes = false;
    }

    /* Open URL in default Browser  */
    private void frTxBx_Notes_LinkClicked(object sender, LinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(e.LinkText);
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// Refreshes the ListView and refocuses the current item
    /// </summary>
    private void AddEntries()
    {
      UpdateLV();
      LV_Entries.Select();

      if (mangaID != -1
          && (string.IsNullOrWhiteSpace(TxBx_Search.Text)
          || SQL.Search(TxBx_Search.Text, mangaID).Rows.Count > 0)) {
        ReFocus();
      }
      else {
        Reset();
      }
    }

    /// <summary>
    /// Alternate row colors in the listview
    /// </summary>
    private void Alternate()
    {
      if (Properties.Settings.Default.DefGrid)
        return;

      int iRowColorAlt = Properties.Settings.Default.RowColorAlt;
      for (int i = 0; i < LV_Entries.Items.Count; i++) {
        if (LV_Entries.Items[i].SubItems[ColRating.Index].Text.Length == 5)
          continue;
        LV_Entries.Items[i].BackColor = (i % 2 != 0) ?
            Color.FromArgb(iRowColorAlt) : SystemColors.Window;
      }
    }

    /// <summary>
    /// Solely for converting old db's in a threaded manner
    /// </summary>
    private void Database_Load(object obj)
    {
      SQL.delProgress = Database_Converting;
      SQL.Connect();
      BeginInvoke(new DelVoid(Database_Display));
    }

    /// <summary>
    /// Allows SQL.cs to send conversion progress to Main.cs
    /// </summary>
    /// <param name="i"></param>
    private void Database_Converting(int i)
    {
      BeginInvoke(new DelInt(DisplayProgress), i);
    }

    /// <summary>
    /// Bridge between the SQL and Main threads
    /// </summary>
    /// <param name="iProgress"></param>
    private void DisplayProgress(int iProgress)
    {
      Text = iProgress.ToString() + " entries converted...";
    }

    /// <summary>
    /// Sets DB contents into program once loaded
    /// </summary>
    private void Database_Display()
    {
      Cursor = Cursors.Default;
      UpdateLV();

      //set up tags
      acTxBx_Tags.KeyWords = SQL.GetTags();

      //set up artists
      CmbBx_Artist.Items.AddRange(SQL.GetArtists());

      //set up types
      CmbBx_Type.Items.AddRange(SQL.GetTypes());
    }

    /// <summary>
    /// Predict the filepath of a manga
    /// </summary>
    /// <param name="sPath">The base filepath</param>
    /// <param name="sArtist">The name of the Artist</param>
    /// <param name="sTitle">The title of the manga</param>
    /// <returns></returns>
    private static string FindPath(
        string sPath, string sArtist, string sTitle)
    {
      if (!File.Exists(sPath) && !Directory.Exists(sPath)) {
        //find base relative
        sPath = ExtString.RelativePath(sPath);

        if (!Directory.Exists(sPath) && !File.Exists(sPath)) {
          sPath = string.Format("{0}\\{1}"
              , !string.IsNullOrEmpty(Properties.Settings.Default.DefLoc) ?
                  Properties.Settings.Default.DefLoc : Environment.CurrentDirectory
              , string.Format(!string.IsNullOrEmpty(sArtist) ?
                  "[{0}] {1}" : "{1}", sArtist, sTitle)
          );

          if (!Directory.Exists(sPath)) {
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
            else
              sPath = ExtString.RelativePath(sPath);

            if (!Directory.Exists(sPath) && !File.Exists(sPath))
              sPath = null;
          }
        }
      }
      return sPath;
    }

    /// <summary>
    /// Sets the cover preview image
    /// </summary>
    /// <param name="obj">Unused</param>
    private void GetImage(Object obj)
    {
      BeginInvoke(new DelVoid(SetPicBxNull));

      //Get cover and filecount
      if (File.Exists(TxBx_Loc.Text)) {
        SetPicBxImage(TxBx_Loc.Text);
      }
      else {
        string[] sFiles = new string[0];
        if ((sFiles = ExtDir.GetFiles(TxBx_Loc.Text,
            SearchOption.TopDirectoryOnly)).Length > 0) {
          SetPicBxImage(sFiles[0]);
          Invoke(new DelInt(SetNudCount), sFiles.Length);
        }
        else {
          Invoke(new DelInt(SetOpenStatus), 0);
        }
      }
    }

    /// <summary>
    /// Adds text to a control
    /// </summary>
    /// <param name="c">The control to alter</param>
    /// <param name="sAdd">The text to add</param>
    /// <param name="iStart">The start point to insert from</param>
    /// <returns></returns>
    private static int InsertText(Control c, string sAdd, int iStart)
    {
      c.Text = c.Text.Insert(iStart, sAdd);
      return iStart + sAdd.Length;
    }

    /// <summary>
    /// Ensure file is a valid archive
    /// </summary>
    /// <param name="sPath">Path of the archive</param>
    /// <returns></returns>
    public static bool IsArchive(string sPath)
    {
      bool bArchive = false;
      if (File.Exists(sPath)) {
        try {
          switch (Path.GetExtension(sPath)) {
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

    /// <summary>
    /// Parse EH metadata into local fields
    /// </summary>
    /// <param name="sURL">URL of the EH gallery</param>
    private void LoadEH(string sURL)
    {
      this.Cursor = Cursors.WaitCursor;
      csEHSearch.gmetadata manga = null;
      lblURL.Text = sURL;
      Text = "Sending request...";

      manga = csEHSearch.LoadMetadata(sURL);
      if (manga != null) {
        Text = "Parsing metadata...";
        Tb_View.SuspendLayout();

        SetTitle(manga.title);																	//set artist/title
        CmbBx_Type.Text = manga.category;												//set entry type
        Dt_Date.Value = manga.posted;													  //set upload date
        Nud_Pages.Value = manga.filecount;											//set page count
        srRating.SelectedStar = Convert.ToInt32(manga.rating);	//set star rating
        acTxBx_Tags.Text = manga.GetTags(                       //set tags
          !string.IsNullOrWhiteSpace(acTxBx_Tags.Text) 
            ? acTxBx_Tags.Text : null
        );

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

    /// <summary>
    /// Removes all entries rated less than 5 stars
    /// </summary>
    private void OnlyFavs()
    {
      Cursor = Cursors.WaitCursor;
      List<ListViewItem> lFavs = new List<ListViewItem>(LV_Entries.Items.Count);

      for (int i = 0; i < LV_Entries.Items.Count; i++) {
        if (LV_Entries.Items[i].SubItems[ColRating.Index].Text.Length == 5)
          lFavs.Add(LV_Entries.Items[i]);
      }

      LV_Entries.BeginUpdate();
      LV_Entries.Items.Clear();
      LV_Entries.Items.AddRange(lFavs.ToArray());
      LV_Entries.EndUpdate();

      Text = string.Format("Returned: {0:n0} entries", LV_Entries.Items.Count);
      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Open image\zip with default program
    /// </summary>
    private void OpenFile()
    {
      if (PicBx_Cover.Image == null)
        return;

      string sPath = TxBx_Loc.Text;
      if (Directory.Exists(sPath)) {
        string[] sFiles = ExtDir.GetFiles(sPath);
        if (sFiles.Length > 0)
          sPath = sFiles[0];
      }

      string sProg = Properties.Settings.Default.DefProg;
      if (sProg == "")
        System.Diagnostics.Process.Start("\"" + sPath + "\"");
      else
        System.Diagnostics.Process.Start(sProg, "\"" + sPath + "\"");
    }

    /// <summary>
    /// Convert number to string of stars
    /// </summary>
    /// <param name="iRating"></param>
    /// <returns></returns>
    private static string RatingFormat(int iRating)
    {
      return new string('☆', iRating);
    }

    /// <summary>
    /// Select the current manga in the listview
    /// </summary>
    private void ReFocus()
    {
      for (int i = 0; i < LV_Entries.Items.Count; i++)
        if (LV_Entries.Items[i].SubItems[colID.Index].Text == mangaID.ToString()) {
          ScrollTo(i);
          break;
        }
    }

    /// <summary>
    /// Change inputs and variables back to their default state
    /// </summary>
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
      page = -1;
      mangaID = -1;

      //Tb_View
      TxBx_Loc.Clear();
      acTxBx_Tags.Clear();
      acTxBx_Title.Clear();
      frTxBx_Desc.Clear();
      Nud_Pages.Value = 0;
      lblURL.Text = string.Empty;
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
      MnTS_New.Visible = true;
      MnTS_Clear.Visible = false;
      Tb_View.ResumeLayout();
    }

    /// <summary>
    /// Providees weighted resizing to the listview
    /// </summary>
    private void ResizeLV()
    {
      //remaining combined column width
      int iStatic = LV_Entries.Columns[ColPages.Index].Width
        + LV_Entries.Columns[colDate.Index].Width
        + LV_Entries.Columns[ColType.Index].Width
        + LV_Entries.Columns[ColRating.Index].Width;
      int iMod = (LV_Entries.Width - iStatic) / 10;

      LV_Entries.BeginUpdate();
      LV_Entries.Columns[ColArtist.Index].Width = iMod * 2; //artist
      LV_Entries.Columns[ColTitle.Index].Width = iMod * 4; //title
      LV_Entries.Columns[ColTags.Index].Width = iMod * 4; //tags

      /* append remaining width to colTags */
      ColTags.Width += LV_Entries.DisplayRectangle.Width - iStatic
        - LV_Entries.Columns[ColArtist.Index].Width
        - LV_Entries.Columns[ColTitle.Index].Width
        - LV_Entries.Columns[ColTags.Index].Width;
      LV_Entries.EndUpdate();
    }

    /// <summary>
    /// Scroll to the indicated position in the listview
    /// </summary>
    /// <param name="iPos">The listview index to scroll to</param>
    private void ScrollTo(int iPos)
    {
      LV_Entries.FocusedItem = LV_Entries.Items[iPos];
      LV_Entries.Items[iPos].Selected = true;
      LV_Entries.TopItem = LV_Entries.Items[iPos];
    }

    /// <summary>
    /// Sets the details of the indicated manga
    /// </summary>
    /// <param name="iNewIndx"></param>
    private void SetData(int iNewIndx = -1)
    {
      if (iNewIndx != -1) {
        mangaID = iNewIndx;
      }
      if (mangaID == -1) {
        Reset();
        return;
      }

      Tb_View.SuspendLayout();
      using (DataTable dt = SQL.GetManga(mangaID)) {
        if (dt.Rows.Count > 0) {
          Text = "Selected: " + ExtString.GetFormattedTitle(
             dt.Rows[0]["Artist"].ToString(),
             dt.Rows[0]["Title"].ToString());
          acTxBx_Title.Text = dt.Rows[0]["Title"].ToString();
          CmbBx_Artist.Text = dt.Rows[0]["Artist"].ToString();
          TxBx_Loc.Text = dt.Rows[0]["Location"].ToString();
          frTxBx_Desc.Text = dt.Rows[0]["Description"].ToString();
          CmbBx_Type.Text = dt.Rows[0]["Type"].ToString();
          Dt_Date.Value = DateTime.Parse(dt.Rows[0]["PublishedDate"].ToString());
          srRating.SelectedStar = Int32.Parse(dt.Rows[0]["Rating"].ToString());
          Nud_Pages.Value = Int32.Parse(dt.Rows[0]["Pages"].ToString());
          lblURL.Text = dt.Rows[0]["GalleryURL"].ToString();
          acTxBx_Tags.Text = dt.Rows[0]["Tags"].ToString();

          acTxBx_Tags.SetScroll();
          MnTS_New.Visible = false;
          MnTS_Edit.Visible = false;
          MnTS_Del.Visible = true;
          MnTS_Clear.Visible = true;
          Tb_View.ResumeLayout();

          //check for relativity
          if (!string.IsNullOrEmpty(TxBx_Loc.Text)) {
            string sResult = FindPath(TxBx_Loc.Text, CmbBx_Artist.Text, acTxBx_Title.Text);
            if (sResult != null)
              TxBx_Loc.Text = sResult;
          }
        }
      }
    }

    /// <summary>
    /// Update the page display count
    /// </summary>
    /// <param name="iNum">The new page value</param>
    private void SetNudCount(int iNum)
    {
      Nud_Pages.Value = iNum;
      TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
      SetOpenStatus(1);
    }

    /// <summary>
    /// Sets whether the manga can be opened with an image editor
    /// </summary>
    /// <param name="iExists">Set to 1 for an open status</param>
    private void SetOpenStatus(int iExists)
    {
      MnTS_Open.Visible = (iExists == 1);
    }

    /// <summary>
    /// Sets the picturebox image
    /// </summary>
    /// <param name="sPath">The path of the image</param>
    private void SetPicBxImage(string sPath)
    {
      if (IsArchive(sPath)) {
        SCA.IArchive scArch = SCA.ArchiveFactory.Open(sPath);
        int iCount = scArch.Entries.Count();
        BeginInvoke(new DelInt(SetNudCount), iCount);

        if (iCount > 0) {
          //account for terrible default zip-sorting
          int iFirst = 0;
          SCA.IArchiveEntry[] scEntries =
              scArch.Entries.ToArray();
          List<string> lEntries = new List<string>();
          for (int i = 0; i < iCount; i++) {
            if (scEntries[i].FilePath.EndsWith("jpg")
                || scEntries[i].FilePath.EndsWith("jpeg")
                || scEntries[i].FilePath.EndsWith("png")
                || scEntries[i].FilePath.EndsWith("bmp"))
              lEntries.Add(scEntries[i].FilePath);
          }
          if (lEntries.Count == 0)
            return;
          lEntries.Sort(new TrueCompare());
          for (int i = 0; i < iCount; i++) {
            if (scEntries[i].FilePath.Equals(lEntries[0]))
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
      }
      else {
        TrySet(sPath);
      }
    }

    /// <summary>
    /// Ensure the picture being set is a valid image
    /// </summary>
    /// <param name="s"></param>
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

    /// <summary>
    /// Clear the cover image
    /// </summary>
    private void SetPicBxNull()
    {
      if (PicBx_Cover.Image != null) {
        PicBx_Cover.Image.Dispose();
        PicBx_Cover.Image = null;
        GC.Collect(0);
      }
    }

    /// <summary>
    /// Parses the input string directly into the Artist and Title fields
    /// </summary>
    /// <param name="sRaw">The string to parse</param>
    private void SetTitle(string sRaw)
    {
      Tb_View.SuspendLayout();
      string[] asProc = SplitTitle(sRaw);

      if (asProc[0] != string.Empty) {
        if (CmbBx_Artist.Text == string.Empty) {
          CmbBx_Artist.Text = asProc[0];
        }
        if (acTxBx_Title.Text == string.Empty) {
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

    /// <summary>
    /// Parses the input string into Artist and Title variables
    /// </summary>
    /// <param name="sRaw">The string to parse</param>
    /// <returns>Returns the Artist (0) and Title (1)</returns>
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
        if (sCircle != "")
          asName[1] += " " + sCircle;
      }
      else {
        asName[1] = sRaw;
      }
      return asName;
    }

    /// <summary>
    /// Refresh listview contents
    /// </summary>
    private void UpdateLV()
    {
      using (DataTable dt = (!string.IsNullOrEmpty(TxBx_Search.Text))
          ? SQL.Search(TxBx_Search.Text) : SQL.GetAllEntries()) {

        Cursor = Cursors.WaitCursor;
        Text = string.Format("Manga Organizer: {0:n0} entries", dt.Rows.Count);
        ListViewItem[] aItems = new ListViewItem[dt.Rows.Count];

        int iRating = 0;
        string[] asItems;
        int iRowColorHighlight = Properties.Settings.Default.RowColorHighlight;
        for (int i = 0; i < dt.Rows.Count; i++) {
          ListViewItem lvi = new ListViewItem(dt.Rows[i]["Artist"].ToString());

          #region Set the row properties
          asItems = new string[LV_Entries.Columns.Count];
          asItems[ColTitle.Index] = dt.Rows[i]["Title"].ToString();
          asItems[ColPages.Index] = dt.Rows[i]["Pages"].ToString();
          asItems[ColTags.Index] = dt.Rows[i]["Tags"].ToString();
          asItems[colDate.Index] = DateTime.Parse(dt.Rows[i]["PublishedDate"].ToString()).ToString("MM/dd/yy");
          asItems[ColType.Index] = dt.Rows[i]["Type"].ToString();
          iRating = Int32.Parse(dt.Rows[i]["Rating"].ToString());
          asItems[ColRating.Index] = RatingFormat(iRating);
          asItems[colID.Index] = dt.Rows[i]["mangaID"].ToString();
          #endregion

          lvi.SubItems.AddRange(asItems);
          if (iRating == 5)
            lvi.BackColor = Color.FromArgb(iRowColorHighlight);
          aItems[i] = lvi;
        }

        LV_Entries.BeginUpdate();
        LV_Entries.Items.Clear();
        LV_Entries.Items.AddRange(aItems);
        LV_Entries.Sort();
        Alternate();
        LV_Entries.EndUpdate();
        Cursor = Cursors.Default;
      }

      //prevent loss of other parameters
      if (ChkBx_ShowFav.Checked)
        OnlyFavs();
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

      if (!SQL.ContainsEntry(CmbBx_Artist.Text, acTxBx_Title.Text)) {
        if (MessageBox.Show("Are you sure you wish to add:\n\""
            + ExtString.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text) + "\"",
            Application.ProductName, MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes) {

          mangaID = SQL.SaveManga(CmbBx_Artist.Text, acTxBx_Title.Text, acTxBx_Tags.Text, TxBx_Loc.Text,
            Dt_Date.Value, Nud_Pages.Value, CmbBx_Type.Text, srRating.SelectedStar, frTxBx_Desc.Text, lblURL.Text);

          //add artist to autocomplete
          CmbBx_Artist.DataSource = SQL.GetArtists();
          acTxBx_Tags.KeyWords = SQL.GetTags();

          //update LV_Entries
          AddEntries();
        }
      }
      else
        MessageBox.Show("This item already exists in the database.",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    private void MnTS_Edit_Click(object sender, EventArgs e)
    {
      if (mangaID == -1)
        return;

      //overwrite entry properties
      SQL.SaveManga(CmbBx_Artist.Text, acTxBx_Title.Text, acTxBx_Tags.Text, TxBx_Loc.Text, Dt_Date.Value,
        Nud_Pages.Value, CmbBx_Type.Text, srRating.SelectedStar, frTxBx_Desc.Text, lblURL.Text, mangaID);
      Text = "Edited entry: " + ExtString.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text);
      acTxBx_Tags.KeyWords = SQL.GetTags();

      //check if entry should still be displayed
      if (SQL.Search(TxBx_Search.Text, mangaID).Rows.Count > 0) {
        ListViewItem lvi = LV_Entries.FocusedItem;
        if (srRating.SelectedStar == 5)
          lvi.BackColor = Color.FromArgb(Properties.Settings.Default.RowColorHighlight);
        lvi.SubItems[ColArtist.Index].Text = CmbBx_Artist.Text;
        lvi.SubItems[ColTitle.Index].Text = acTxBx_Title.Text;
        lvi.SubItems[ColPages.Index].Text = Nud_Pages.Value.ToString();
        lvi.SubItems[ColTags.Index].Text = SQL.GetMangaDetail(mangaID, "Tags");
        lvi.SubItems[colDate.Index].Text = Dt_Date.Value.ToString("MM/dd/yy");
        lvi.SubItems[ColType.Index].Text = CmbBx_Type.Text;
        LV_Entries.Sort();
        ReFocus();
      }

      Alternate();
      MnTS_Edit.Visible = false;
    }

    private void MnTS_Open_Click(object sender, EventArgs e)
    {
      if (PicBx_Cover.Image != null)
        OpenFile();
    }

    private void MnTS_Delete_Click(object sender, EventArgs e)
    {
      if (mangaID == -1)
        return;

      //ensure deletion is intentional
      string msg = "";
      string sLoc = SQL.GetMangaDetail(mangaID, "Location");
      bool bFile = false, bRst = true;
      if ((bRst = ExtDir.Accessible(sLoc)))
        msg = "Do you want to delete the source directory as well?";
      else if (bFile = File.Exists(sLoc))
        msg = "Do you want to delete the source file as well?";
      else
        msg = "Are you sure you wish to delete this entry?";
      DialogResult dResult = MessageBox.Show(msg, Application.ProductName,
          MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
      if ((bRst && !bFile) && dResult == DialogResult.No)
        dResult = DialogResult.Cancel;

      //delete source file\directory
      if (dResult == DialogResult.Yes) {
        if (bRst) {
          //warn user before deleting subdirectories
          int iNumDir = Directory.GetDirectories(sLoc).Length;
          if (iNumDir > 0) {
            dResult = MessageBox.Show(string.Format("This directory contains {0} subfolder(s),\n" +
                "are you sure you want to delete them?", iNumDir),
                Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
          }
          if (dResult == DialogResult.Yes) {
            this.Cursor = Cursors.WaitCursor;
            Directory.Delete(sLoc, true);
            this.Cursor = Cursors.Default;
          }
        }
        else if (bFile) {
          this.Cursor = Cursors.WaitCursor;
          File.Delete(sLoc);
          this.Cursor = Cursors.Default;
        }
      }

      //remove from database
      if (dResult != DialogResult.Cancel) {
        int iPos = LV_Entries.FocusedItem.Index;
        SQL.DeleteManga(mangaID);

        //remove from listview
        LV_Entries.Items.RemoveAt(LV_Entries.SelectedItems[0].Index);
        Reset();

        if (iPos < LV_Entries.Items.Count) {
          LV_Entries.TopItem = LV_Entries.Items[iPos];
          LV_Entries.TopItem = LV_Entries.Items[iPos];
          LV_Entries.TopItem = LV_Entries.Items[iPos];
        }
      }
    }

    private void MnTS_Clear_Click(object sender, EventArgs e)
    {
      Reset();
    }
    #endregion

    #region Menu: Main
    private void MnTS_CopyTitle_Click(object sender, EventArgs e)
    {
      if (acTxBx_Title.Text == "") {
        MessageBox.Show("The title field cannot be empty.",
            Application.ProductName, MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        return;
      }

      Clipboard.SetText(
        ExtString.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text));
      Text = "Name copied to clipboard";
    }

    private void MnTS_OpenSource_Click(object sender, EventArgs e)
    {
      if (Directory.Exists(TxBx_Loc.Text)) {
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
      Suggest fmSuggest = new Suggest(
        ExtString.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text));
      fmSuggest.ShowDialog();

      if (fmSuggest.DialogResult == DialogResult.OK) {
        LoadEH(fmSuggest.sChoice);
      }
      fmSuggest.Dispose();
    }

    private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
    {
      string sPath = Properties.Settings.Default.SavLoc != "" ?
          Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;

      if (Directory.Exists(sPath))
        System.Diagnostics.Process.Start(sPath);
      else
        MessageBox.Show("This directory no longer exists.", Application.ProductName,
          MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void MnTs_CleanTags_Click(object sender, EventArgs e)
    {
      MessageBox.Show(string.Format("{0} unused tags have been removed.", SQL.CleanUpTags())
        , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MnTS_Stats_Click(object sender, EventArgs e)
    {
      Stats fmStats = new Stats();
      fmStats.Show();
    }

    private void Mn_Settings_Click(object sender, EventArgs e)
    {
      string sOld = Properties.Settings.Default.SavLoc;
      Form fmSet = new Settings();
      fmSet.ShowDialog();

      if (fmSet.DialogResult == DialogResult.Yes) {
        LV_Entries.GridLines = Properties.Settings.Default.DefGrid;
        PicBx_Cover.BackColor = Properties.Settings.Default.DefColour;
        if (Properties.Settings.Default.HideDate)
          LV_Entries.Columns[4].Width = 0;
        else
          LV_Entries.Columns[4].Width = 70;
        ResizeLV();

        //Update new DB save location
        if (sOld != Properties.Settings.Default.SavLoc) {
          string sNew = Properties.Settings.Default.SavLoc + "\\MangaDatabase.bin";
          sOld += "\\MangaDatabase.bin";

          //move old save to new location
          if (File.Exists(sNew)
                  && MessageBox.Show("Open existing database at:\n" + sNew,
                  Application.ProductName, MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question) == DialogResult.Yes) {
            SQL.Connect(sNew);

            if (SQL.Connected) {
              UpdateLV();

              //set up CmbBx autocomplete
              CmbBx_Artist.Items.Clear();
              CmbBx_Artist.Items.AddRange(SQL.GetArtists());
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
    {
      this.Close();
    }
    #endregion

    #region Menu_Context
    private void Mn_TxBx_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //ensure element has focus
      if (Mn_TxBx.SourceControl.CanFocus)
        Mn_TxBx.SourceControl.Focus();
      if (Mn_TxBx.SourceControl.CanSelect)
        Mn_TxBx.SourceControl.Select();

      //check what properties are available
      bool bUndo = false, bSelect = false;
      switch (Mn_TxBx.SourceControl.GetType().Name) {
        case "FixedRichTextBox":
          FixedRichTextBox fr = ActiveControl as FixedRichTextBox;
          if (fr.CanUndo)
            bUndo = true;
          if (fr.SelectionLength > 0)
            bSelect = true;
          break;
        case "TextBox":
        case "AutoCompleteTagger":
          TextBox txt = ActiveControl as TextBox;
          if (txt.CanUndo)
            bUndo = true;
          if (txt.SelectionLength > 0)
            bSelect = true;
          break;
        case "ComboBox":
          ComboBox cb = ActiveControl as ComboBox;
          if (cb.SelectionLength > 0)
            bSelect = true;
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
            bSavNotes = true;
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
          if (TabControl.SelectedIndex == 2)
            bSavNotes = false;
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
          if (TabControl.SelectedIndex == 2)
            bSavNotes = false;
          break;
        case "TextBox":
        case "AutoCompleteTagger":
          if (ActiveControl.Name == "TxBx_Search") {
            string[] asTitle = SplitTitle(sAdd);
            sAdd = (asTitle[0] == "") ? sAdd :
                string.Format("artist:{0} title:{1}",
                asTitle[0].Replace(' ', '_'), asTitle[1].Replace(' ', '_'));
            TxBx_Search.SelectionStart = InsertText(
                TxBx_Search, sAdd, TxBx_Search.SelectionStart);
            UpdateLV();
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

          if (sAdd.Contains('['))
            SetTitle(sAdd);
          else
            cb.SelectedText = sAdd;
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
        if (TabControl.SelectedIndex == 2)
          bSavNotes = false;
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
      else
        e.Effect = DragDropEffects.None;
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
          if (sAdd.Contains('['))
            SetTitle(sAdd);
          else
            CmbBx_Artist.SelectionStart = InsertText(
              CmbBx_Artist, sAdd, CmbBx_Artist.SelectionStart);
          break;
        case "acTxBx_Title":
          if (sAdd.Contains('['))
            SetTitle(sAdd);
          else
            acTxBx_Title.SelectionStart = InsertText(
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
          UpdateLV();
          break;
      }
    }

    private void TxBx_Loc_DragDrop(object sender, DragEventArgs e)
    {
      string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
      TxBx_Loc.Text = asDir[0];

      if (Directory.Exists(asDir[0])) {
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
      else
        MessageBox.Show("The dropped text was not a valid date.",
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
      else
        MessageBox.Show("The dropped text was not a valid value.",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    /* Allow dropping of folders/zips onto LV_Entries (& TxBx_Loc) */
    private void LV_Entries_DragEnter(object sender, DragEventArgs e)
    {
      string[] sTemp = (string[])e.Data.GetData(DataFormats.FileDrop, false);
      if (sTemp == null)
        return;

      FileAttributes fa = File.GetAttributes(sTemp[0]);
      Text = fa.ToString();
      if (fa == FileAttributes.Directory || IsArchive(sTemp[0])
              || fa.ToString() == "Directory, Archive")
        e.Effect = DragDropEffects.Copy;
      else
        e.Effect = DragDropEffects.None;
    }

    /* Adds folder(s) to database when dragged over LV_Entries */
    private void LV_Entries_DragDrop(object sender, DragEventArgs e)
    {
      string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
      string sError = "";
      int iTrack = mangaID;

      //add all remaining folders
      for (int i = 0; i < asDir.Length; i++) {
        if (asDir[i] == ""
            || (!Directory.Exists(asDir[i])
                && !IsArchive(asDir[i])))
          continue;

        //add item
        csEntry en = new csEntry(asDir[i]);
        if (!SQL.ContainsEntry(en.sArtist, en.sTitle)) {
          mangaID = SQL.SaveManga(en.sArtist, en.sTitle, en.sTags, en.sLoc,
            en.dtDate, en.pages, en.sType, en.byRat, en.sDesc);

          if (en.sArtist != "" && !CmbBx_Artist.Items.Contains(en.sArtist))
            CmbBx_Artist.Items.Add(en.sArtist);
        }
        else
          sError += '\n' + asDir[i];
      }
      if (sError != "") {
        MessageBox.Show("The following path(s) already exists in the database:" + sError,
            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      //Update LV
      if (iTrack != mangaID)
        AddEntries();
    }
    #endregion

    #region Cruft

    #region Classes

    /* Holds manga metadata */
    [Serializable]
    public class csEntry : ISerializable
    {
      public string sArtist, sTitle,
          sLoc, sType, sDesc, sTags;
      public DateTime dtDate;
      public ushort pages;
      public byte byRat;

      public csEntry(string _Artist, string _Title, string _Loc, string _Desc,
          string _Tags, string _Type, DateTime _Date, decimal _Pages, int _Rating)
      {
        sArtist = _Artist;
        sTitle = _Title;
        sLoc = _Loc;
        sDesc = _Desc;
        dtDate = _Date;
        pages = (_Pages > ushort.MaxValue) ?
                ushort.MaxValue : (ushort)_Pages;
        sType = _Type;
        byRat = Convert.ToByte(_Rating);
        sTags = "";

        //trim, clean, and format tags
        if (_Tags != "") {
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
        pages = 0;

        //Get filecount
        string[] sFiles = new string[0];
        if (File.Exists(_Path))
          sFiles = new string[1] { _Path };
        else
          sFiles = ExtDir.GetFiles(_Path,
            SearchOption.TopDirectoryOnly, "*.zip|*.cbz|*.cbr|*.rar|*.7z");

        if (sFiles.Length > 0) {
          if (Main.IsArchive(sFiles[0])) {
            SCA.IArchive scArchive = SCA.ArchiveFactory.Open(sFiles[0]);
            pages = (scArchive.Entries.Count() > ushort.MaxValue) ?
                ushort.MaxValue : (ushort)Math.Abs(scArchive.Entries.Count());
            scArchive.Dispose();
          }
        }
        else
          pages = (ushort)ExtDir.GetFiles(
            _Path, SearchOption.TopDirectoryOnly).Length;
      }

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
        pages = (ushort)info.GetInt32("PG");
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
        info.AddValue("PG", pages);
        info.AddValue("TY", sType);
        info.AddValue("RT", byRat);
        info.AddValue("TG", sTags);
      }
    }

    #endregion

    #endregion
  }
}