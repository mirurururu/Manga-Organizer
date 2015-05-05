#region Assemblies
using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SCA = SharpCompress.Archive;
#endregion

namespace Nagru___Manga_Organizer
{
  public partial class Main : Form
  {
    #region Properties
    const string DATE_FORMAT = "MM/dd/yyyy";

    delegate void DelVoid();
    delegate void DelInt(int iNum);
    delegate void DelString(string sMsg);
    delegate void DelBitmap(Bitmap bmp);
    delegate void DelMangaCover(MangaCover mangaCover);

    Thread thLoadImages;
    Queue<ListViewItem> qManga = new Queue<ListViewItem>();
    bool bLoadImages = false;

    bool bSavNotes = true, bResize = false;
    int mangaID = -1, page = -1;
    int[] aiShuffle = null;
    #endregion

    #region Main Form
    /// <summary>
    /// Initializes the project
    /// </summary>
    /// <param name="sFile">The path passed in if the user opens the DB with the shell</param>
    public Main(string[] sFile)
    {
      SQL.Connect();
      InitializeComponent();
    }

    /// <summary>
    /// Perform some runtime modifications to behavior
    /// </summary>
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

      //set-up listview sorting & sizing
      lvManga.LargeImageList = new ImageList();
      lvManga.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
      lvManga.LargeImageList.ImageSize = new System.Drawing.Size(ListViewNF.ICON_SIZE, ListViewNF.ICON_SIZE);
      lvManga.staticColumns.Add(ColTags.Index);
      lvManga.RatingColumn = ColRating.Index;
      lvManga.Select();

      //set WindowState to what it was the last time
      this.WindowState = Properties.Settings.Default.LastWindowState;

      thLoadImages = new Thread(PopulateIcons);
      thLoadImages.IsBackground = true;
      thLoadImages.Start();
    }

    /// <summary>
    /// Start loading the DB asynchronously
    /// </summary>
    private void Main_Shown(object sender, EventArgs e)
    {
      Cursor = Cursors.WaitCursor;
      Text = "Loading Database...";
      LoadDB();
    }

    /// <summary>
    /// When the program is closed, save the last form position and close the DB connection
    /// </summary>
    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.Visible = false;
      if (SQL.IsConnected) {
        //save changes to text automatically
        if (!bSavNotes) {
          SQL.UpdateSetting(SQL.Setting.Notes, frTxBx_Notes.Text);
          bSavNotes = true;
        }

        //save Form's last position
        SQL.UpdateSetting(SQL.Setting.FormPosition, string.Format("{0},{1},{2},{3}"
          , this.Location.X > -1 ? this.Location.X : 0
          , this.Location.Y > -1 ? this.Location.Y : 0
          , this.Size.Width
          , this.Size.Height)
        );

        SQL.Disconnect();

        //save form's last WindowState
        Properties.Settings.Default.LastWindowState = this.WindowState;
        Properties.Settings.Default.Save();
      }
    }

    /// <summary>
    /// Change the title of the Form based on which tab we're on
    /// </summary>
    private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SuspendLayout();
      switch (TabControl.SelectedIndex) {
        case 0:
          if (mangaID == -1) {
            Text = string.Format("{0}: {1:n0} entries",
                (string.IsNullOrWhiteSpace(TxBx_Search.Text) ?
                "Manga Organizer" : "Returned"), lvManga.Items.Count);
          }
          this.AcceptButton = Btn_Clear;
          lvManga.Focus();
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

    /// <summary>
    /// Switch between tabs with ctrl+# shortcuts
    /// </summary>
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
      else if (e.Alt && e.KeyCode == Keys.E) {
        OpenSourceLocation(TxBx_Loc.Text);
      }
    }

    /// <summary>
    /// Switch the view mode of the listview when clicked
    /// </summary>
    private void chkbxViewMode_CheckedChanged(object sender, EventArgs e)
    {
      bLoadImages = chkbxViewMode.Checked;
      if (chkbxViewMode.Checked) {
        lvManga.View = View.LargeIcon;
      }
      else {
        lvManga.View = View.Details;
      }

      UpdateLV();
    }
    #endregion

    #region Tab_Browse
    /// <summary>
    /// When clicking on Form whitespace, unselect anything in the ListView
    /// </summary>
    private void ClearSelection(object sender, EventArgs e)
    {
      if (mangaID != -1)
        Reset();
    }

    /// <summary>
    /// Load up the form for scanning for manga
    /// </summary>
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

    /// <summary>
    /// Insert delay before Search() to account for Human input speed
    /// </summary>
    private void TxBx_Search_TextChanged(object sender, EventArgs e)
    {
      int offsetWidth = Btn_Clear.Size.Width;
      Reset();
      Delay.Stop();

      if (string.IsNullOrWhiteSpace(TxBx_Search.Text)) {
        TxBx_Search.Width += offsetWidth;
        Btn_Clear.Visible = false;
        UpdateLV();
      }
      else if (!Btn_Clear.Visible) {
        TxBx_Search.Width -= offsetWidth;
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

    /// <summary>
    /// Clear the search form and refresh the ListView
    /// </summary>
    private void Btn_Clear_Click(object sender, EventArgs e)
    {
      TxBx_Search.Focus();
      TxBx_Search.Clear();
    }

    /// <summary>
    /// Updates tab two of the form with the selected manga
    /// </summary>
    private void LV_Entries_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (lvManga.SelectedItems.Count > 0) {
        if (lvManga.Items[0].SubItems.Count == 3) {
          SetData(Int32.Parse(lvManga.FocusedItem.SubItems[2].Text));
        }
        else {
          SetData(Int32.Parse(lvManga.FocusedItem.SubItems[colID.Index].Text));
        }
      }
      else
        Reset();
    }

    /// <summary>
    /// Proportionally-resizes columns on ListView resizes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LV_Entries_Resize(object sender, EventArgs e)
    {
      ResizeLV();
    }

    /// <summary>
    /// Prevent user from changing column widths
    /// </summary>
    private void LV_Entries_ColumnWidthChanging(
        object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
      e.NewWidth = lvManga.Columns[e.ColumnIndex].Width;
    }

    /// <summary>
    /// Display basic manga options when right-clicking on an item
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void lvManga_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right) {
        if (lvManga.FocusedItem.Bounds.Contains(e.Location)) {
          Mn_ListItem.Show(Cursor.Position);
        }
      }
    }

    /// <summary>
    /// Opens the selected manga in the default image browser
    /// </summary>
    private void LV_Entries_DoubleClick(object sender, EventArgs e)
    {
      OpenFile();
    }

    /// <summary>
    /// Auto-focus on the ListView when hovered over
    /// </summary>
    private void LV_Entries_MouseHover(object sender, EventArgs e)
    {
      if (!lvManga.Focused && !Delay.Enabled)
        lvManga.Focus();
    }
    #endregion

    #region Tab_View
    /// <summary>
    /// Lets the user select where the manga is located
    /// </summary>
    private void Btn_Loc_Click(object sender, EventArgs e)
    {
      //try to auto-magically grab folder\file path
      string sPath = Ext.FindPath(CmbBx_Artist.Text, acTxBx_Title.Text, TxBx_Loc.Text)
        ?? ((string)SQL.GetSetting(SQL.Setting.RootPath));

      using (ExtFolderBrowserDialog xfbd = new ExtFolderBrowserDialog()) {
        xfbd.ShowBothFilesAndFolders = true;
        xfbd.RootFolder = Environment.SpecialFolder.MyComputer;
        xfbd.SelectedPath = sPath;

        if (xfbd.ShowDialog() == DialogResult.OK) {
          TxBx_Loc.Text = xfbd.SelectedPath;
          ThreadPool.QueueUserWorkItem(GetImage, TxBx_Loc.Text);

          if (string.IsNullOrWhiteSpace(CmbBx_Artist.Text) && string.IsNullOrWhiteSpace(acTxBx_Title.Text))
            SetTitle(Ext.GetNameSansExtension(xfbd.SelectedPath));
        }
      }
    }

    /// <summary>
    /// Open URL in default Browser
    /// </summary>
    private void frTxBx_Desc_LinkClicked(object sender, LinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(e.LinkText);
    }

    /// <summary>
    /// Opens the manga using the built-in image browser
    /// </summary>
    private void PicBx_Cover_Click(object sender, EventArgs e)
    {
      OpenImageBrowser();
    }

    /// <summary>
    /// Redraw cover image if form size has changed
    /// </summary>
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
        ThreadPool.QueueUserWorkItem(GetImage, TxBx_Loc.Text);
        bResize = false;
      }
    }
    private void Main_Resize(object sender, EventArgs e)
    {
      if (this.WindowState == FormWindowState.Maximized
          && PicBx_Cover.Image != null) {
        ThreadPool.QueueUserWorkItem(GetImage, TxBx_Loc.Text);
      }
    }

    /// <summary>
    /// Dynamically update PicBx when user manually alters path
    /// </summary>
    private void TxBx_Loc_TextChanged(object sender, EventArgs e)
    {
      if (mangaID != -1) {
        MnTS_Edit.Visible = true;
      }
      MnTS_Clear.Visible = true;

      if (Directory.Exists(TxBx_Loc.Text)
          || File.Exists(TxBx_Loc.Text)) {
        page = -1;
        ThreadPool.QueueUserWorkItem(GetImage, TxBx_Loc.Text);
      }
      else {
        SetPicBxNull();
        SetOpenStatus(0);
      }
    }

    /// <summary>
    /// Programmatically select items in LV_Entries
    /// </summary>
    private void Btn_GoDn_Click(object sender, EventArgs e)
    {
      if (lvManga.Items.Count == 0
          || (lvManga.Items.Count == 1 && mangaID != -1))
        return;
      int iPos = lvManga.TopItem.Index;

      if (lvManga.SelectedItems.Count == 1) {
        iPos = lvManga.SelectedItems[0].Index;
        if (++iPos >= lvManga.Items.Count)
          iPos = 0;
      }

      ScrollTo(iPos);
    }
    private void Btn_GoUp_Click(object sender, EventArgs e)
    {
      if (lvManga.Items.Count == 0
          || (lvManga.Items.Count == 1 && mangaID != -1))
        return;
      int iPos = lvManga.Items.Count - 1;

      if (lvManga.SelectedItems.Count == 1) {
        iPos = lvManga.SelectedItems[0].Index;
        if (--iPos < 0)
          iPos = lvManga.Items.Count - 1;
      }

      ScrollTo(iPos);
    }
    private void Btn_Rand_Click(object sender, EventArgs e)
    {
      /* if there are no items to select, or we have already
       * selected the only one, skip operation */
      if (lvManga.Items.Count == 0
          || (lvManga.Items.Count == 1 && mangaID != -1)) {
        return;
      }

      /* if the shuffling process hasn't happened yet, do it here */
      if (aiShuffle == null || aiShuffle.Length != lvManga.Items.Count) {
        Random rnd = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
        aiShuffle = Enumerable.Range(0, lvManga.Items.Count).ToArray()
          .OrderBy(x => rnd.Next()).ToArray();
      }

      /* Find the next manga in the random sequence to go to */
      int iPos = lvManga.SelectedItems.Count == 1 ?
        lvManga.SelectedItems[0].Index : aiShuffle[0];

      for (int i = 0; i < aiShuffle.Length; i++) {
        if (aiShuffle[i] == iPos) {
          iPos = aiShuffle[i + 1 < aiShuffle.Length ? i + 1 : 0];
          break;
        }
      }
      ScrollTo(iPos > -1 ? iPos : 0);
    }

    /// <summary>
    /// Update the rating of the selected manga
    /// </summary>
    private void srRating_Click(object sender, EventArgs e)
    {
      if (mangaID != -1) {
        MnTS_Edit.Visible = true;
      }
      MnTS_Clear.Visible = true;
    }

    /// <summary>
    /// Only enable edit when changes have been made
    /// </summary>
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
    /// <summary>
    /// Prevent loss of changes in note text
    /// </summary>
    private void frTxBx_Notes_TextChanged(object sender, EventArgs e)
    {
      if (bSavNotes)
        bSavNotes = false;
    }

    /// <summary>
    /// Open URL in default Browser
    /// </summary>
    private void frTxBx_Notes_LinkClicked(object sender, LinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(e.LinkText);
    }
    #endregion

    #region Custom Methods

    #region Database Conversion Progress

    /// <summary>
    /// Sets DB contents into program once loaded
    /// </summary>
    private void LoadDB()
    {
      //run tutorial on first run
      if ((bool)SQL.GetSetting(SQL.Setting.NewUser)) {
        //Run tutorial on first execution
        SQL.UpdateSetting(SQL.Setting.NewUser, 0);
        Tutorial fmTut = new Tutorial();
        fmTut.Show();

        //set runtime sensitive default locations
        SQL.UpdateSetting(SQL.Setting.RootPath, Environment.CurrentDirectory);
        Properties.Settings.Default.SavLoc = Environment.CurrentDirectory;
        Properties.Settings.Default.Save();
      }

      //check user settings
      #region Set Form Position
      object objPos = (SQL.GetSetting(SQL.Setting.FormPosition));
      if (objPos != null) {
        this.Width = ((Rectangle)objPos).Width;
        this.Height = ((Rectangle)objPos).Height;
      }

      //ensure it's displayed
      bool bDisplayed = false;
      foreach (Screen sc in Screen.AllScreens) {
        if (sc.WorkingArea.Contains(this.Location)) {
          bDisplayed = true;
          break;
        }
      }
      if (!bDisplayed) {
        this.Location = new Point(
          (Screen.PrimaryScreen.WorkingArea.Width / 2) - (this.Width / 2)
          , (Screen.PrimaryScreen.WorkingArea.Height / 2) - (this.Height / 2)
        );
      }
      #endregion

      frTxBx_Notes.Text = ((string)SQL.GetSetting(SQL.Setting.Notes));
      lvManga.GridLines = ((bool)SQL.GetSetting(SQL.Setting.ShowGrid));
      PicBx_Cover.BackColor = ((Color)SQL.GetSetting(SQL.Setting.BackgroundColour));
      if ((bool)SQL.GetSetting(SQL.Setting.ShowDate))
        lvManga.Columns[4].Width = 0;

      //set up tags, types, and artists
      RefreshAutocomplete();

      Reset();
      UpdateLV();
      ResizeLV();
      Cursor = Cursors.Default;
    }
    #endregion

    /// <summary>
    /// Refreshes the ListView and refocuses the current item
    /// </summary>
    private void AddEntries()
    {
      UpdateLV();
      lvManga.Select();

      if (mangaID != -1
          && (string.IsNullOrWhiteSpace(TxBx_Search.Text)
          || SQL.GetAllManga(TxBx_Search.Text, mangaID).Rows.Count > 0)) {
        ReFocus();
      }
      else {
        Reset();
      }
    }

    /// <summary>
    /// Handles deleting files or folders
    /// Helps ensure deletion is intentional
    /// </summary>
    /// <param name="sLoc"></param>
    /// <returns></returns>
    private bool DeleteLocation(string sLoc)
    {
      string msg;
      //bool bFile = false, bFolder = true;
      const int iMaxFolder = 2, iMaxFile = 50;

      Ext.PathType pathType = Ext.Accessible(sLoc);
      if (pathType == Ext.PathType.ValidDirectory) {
        msg = "Do you want to delete the source directory as well?";
      }
      else if (pathType == Ext.PathType.ValidFile) {
        msg = "Do you want to delete the source file as well?";
      }
      else {
        msg = "Are you sure you wish to delete this entry?";
      }
      DialogResult dResult = MessageBox.Show(
          string.Format("{0}{1}", (pathType != Ext.PathType.Invalid) ? "\"" + sLoc + "\"\n\n" : "", msg),
        Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
      if (pathType == Ext.PathType.Invalid && dResult == DialogResult.No) {
        dResult = DialogResult.Cancel;
      }

      //delete source file\directory
      if (dResult == DialogResult.Yes) {
        this.Cursor = Cursors.WaitCursor;
        if (pathType == Ext.PathType.ValidDirectory) {
          //warn user before deleting subdirectories
          int iNumDir = Directory.GetDirectories(sLoc, "*", SearchOption.AllDirectories).Length;
          int iNumFile = Directory.GetFiles(sLoc, "*", SearchOption.AllDirectories).Length;
          if (iNumDir > 0 || iNumFile > 0) {
            if (iNumDir < iMaxFolder && iNumFile < iMaxFile) {
              dResult = MessageBox.Show(string.Format("This directory contains:\n\t- {0} file(s)\n\t- {1} subfolder(s)\n\nAre you sure you want to delete them?", iNumFile, iNumDir)
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }
            else {
              MessageBox.Show(string.Format("This directory contains more than {0} subdirectories or more than {1} files. In the interest of safety,"
                + " please delete this folder manually. Press {2} to open this location in Windows Explorer.", iMaxFolder, iMaxFile, MnTS_OpenSource.ShortcutKeyDisplayString)
                , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
              dResult = DialogResult.Cancel;
            }
          }
          if (dResult == DialogResult.Yes) {
            Directory.Delete(sLoc, true);
          }
        }
        else if (pathType == Ext.PathType.ValidFile) {
          File.Delete(sLoc);
        }
        this.Cursor = Cursors.Default;
      }

      return (dResult != DialogResult.Cancel);
    }

    /// <summary>
    /// Sets the cover preview image
    /// </summary>
    /// <param name="obj">Unused</param>
    private void GetImage(Object obj)
    {
      //Get cover and filecount
      if (File.Exists(obj.ToString())) {
        BeginInvoke(new DelBitmap(SetPreviewImage), LoadImage(obj.ToString()));
      }
      else {
        string[] sFiles = new string[0];
        if ((sFiles = Ext.GetFiles(obj.ToString(),
            SearchOption.TopDirectoryOnly)).Length > 0) {
          BeginInvoke(new DelBitmap(SetPreviewImage), LoadImage(sFiles[0]));
          BeginInvoke(new DelInt(SetNudCount), sFiles.Length);
          BeginInvoke(new DelVoid(SetZipSourceStatus));
        }
        else {
          BeginInvoke(new DelInt(SetOpenStatus), 0);
        }
      }
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
    /// Ensures the archive can be accessed.
    /// Since SharpCompress cannot check for encryption, we simply check for an access exception.
    /// </summary>
    /// <param name="Archive">A SharpCompress Archive object to validate</param>
    /// <returns>Whether the file can be accessed</returns>
    public static bool IsArchiveAccessible(SCA.IArchive scArch)
    {
      bool bAccessible = false;

      try {
        if (scArch.Entries.Count() >= 0) {
          bAccessible = true;
        }
      } catch {
        Console.WriteLine("Archive was inaccessible");
      }
      return bAccessible;
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
      if (manga != null && !manga.APIError) {
        Text = "Parsing metadata...";
        Tb_View.SuspendLayout();

        SetTitle(manga.title);																	//set artist/title
        CmbBx_Type.Text = manga.category;												//set entry type
        Dt_Date.Value = manga.posted;													  //set upload date
        Nud_Pages.Value = manga.filecount;											//set page count
        if (srRating.SelectedStar == 0) {                       //set star rating
          srRating.SelectedStar = Convert.ToInt32(manga.rating);
        }
        acTxBx_Tags.Text = manga.GetTags(acTxBx_Tags.Text);			//set tags

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
    /// Selects the passed in file/directory with Windows Explorer
    /// </summary>
    /// <param name="sLoc">The location to try and open</param>
    private void OpenSourceLocation(string sLoc)
    {
      if (!string.IsNullOrWhiteSpace(sLoc)) {
        if (Directory.Exists(sLoc)) {
          System.Diagnostics.Process.Start("explorer.exe", "\"" + sLoc + "\"");
        }
        else if (File.Exists(sLoc)) {
          System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + sLoc + "\"");
        }
      }
    }

    /// <summary>
    /// Inserts new tags and re-sorts them alphabetically
    /// </summary>
    /// <param name="NewTags">The new tags to be inserted</param>
    private void UpdateTags(string NewTags)
    {
      List<string> lTags = new List<string>(20);
      lTags.AddRange(Ext.Split(NewTags, "\r\n", ","));
      lTags.AddRange(Ext.Split(acTxBx_Tags.Text, ","));
      lTags = lTags.Select(s => s.Trim())
        .Where(s => !(s[s.Length - 1] == ':'))
        .Distinct<string>()
        .OrderBy(s => s)
        .ToList<string>();
      acTxBx_Tags.Text = string.Join(", ", lTags);
    }

    /// <summary>
    /// Open image\zip with default program
    /// </summary>
    private void OpenFile()
    {
      if (PicBx_Cover.Image == null) {
        MessageBox.Show("The manga location could not be opened:\n" + TxBx_Loc.Text,
          Application.ProductName, MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      string sPath = TxBx_Loc.Text;
      if (Directory.Exists(sPath)) {
        string[] sFiles = Ext.GetFiles(sPath);
        if (sFiles.Length > 0)
          sPath = sFiles[0];
      }

      string sProg = ((string)SQL.GetSetting(SQL.Setting.ImageBrowser));
      if (string.IsNullOrWhiteSpace(sProg) || sProg == "System Default") {
        System.Diagnostics.Process.Start("\"" + sPath + "\"");
      }
      else if (sProg == "Built-In Viewer") {
        OpenImageBrowser();
      }
      else {
        System.Diagnostics.Process.Start(sProg, "\"" + sPath + "\"");
      }
      UpdateReadProgress(Convert.ToInt32(Nud_Pages.Value), SaveStatus: true);
    }

    /// <summary>
    /// Open the internal image browser
    /// </summary>
    private void OpenImageBrowser()
    {
      if (PicBx_Cover.Image == null)
        return;

      Browse_Img fmBrowse = new Browse_Img();
      fmBrowse.Page = page;

      if (Directory.Exists(TxBx_Loc.Text)) {
        //process 'loose' images
        string[] sFiles = new string[0];
        if ((sFiles = Ext.GetFiles(TxBx_Loc.Text,
            SearchOption.TopDirectoryOnly)).Length > 0) {
          fmBrowse.Files = new List<string>(sFiles.Length);
          fmBrowse.Files.AddRange(sFiles);
          fmBrowse.ShowDialog();
          UpdateReadProgress(Math.Abs(fmBrowse.Page), SaveStatus: true);
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
          UpdateReadProgress(Math.Abs(fmBrowse.Page), SaveStatus: true);
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

    /// <summary>
    /// Select the current manga in the listview
    /// </summary>
    private void ReFocus()
    {
      for (int i = 0; i < lvManga.Items.Count; i++)
        if (lvManga.Items[i].SubItems[colID.Index].Text == mangaID.ToString()) {
          ScrollTo(i);
          break;
        }
    }

    /// <summary>
    /// Re-set the values in the auto-complete controls
    /// </summary>
    private void RefreshAutocomplete()
    {
      CmbBx_Artist.Items.Clear();
      CmbBx_Artist.Items.AddRange(SQL.GetArtists());
      CmbBx_Type.Items.Clear();
      CmbBx_Type.Items.AddRange(SQL.GetTypes());
      acTxBx_Tags.KeyWords = SQL.GetTags();
    }

    /// <summary>
    /// Change inputs and variables back to their default state
    /// </summary>
    private void Reset()
    {
      //reset Form title
      Tb_View.SuspendLayout();
      Text = string.Format("{0}: {1:n0} entries",
          (string.IsNullOrWhiteSpace(TxBx_Search.Text) ?
          "Manga Organizer" : "Returned"), lvManga.Items.Count);

      //Tb_Browse
      lvManga.FocusedItem = null;
      lvManga.SelectedItems.Clear();
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
      progReadCount.Value = 0;

      //Mn_EntryOps
      acTxBx_Tags.SetScroll();
      MnTS_ZipSource.Enabled = false;
      MnTS_New.Visible = true;
      MnTS_Del.Visible = false;
      MnTS_Edit.Visible = false;
      MnTS_Open.Visible = false;
      MnTS_New.Visible = true;
      MnTS_Clear.Visible = false;
      Tb_View.ResumeLayout();
    }

    /// <summary>
    /// Provides weighted resizing to the listview
    /// </summary>
    private void ResizeLV()
    {
      //remaining combined column width
      int iStatic = lvManga.Columns[ColPages.Index].Width
        + lvManga.Columns[colDate.Index].Width
        + lvManga.Columns[ColType.Index].Width
        + lvManga.Columns[ColRating.Index].Width;
      int iMod = (lvManga.Width - iStatic) / 10;

      lvManga.BeginUpdate();
      lvManga.Columns[ColArtist.Index].Width = iMod * 2; //artist
      lvManga.Columns[ColTitle.Index].Width = iMod * 4; //title
      lvManga.Columns[ColTags.Index].Width = iMod * 4; //tags

      /* append remaining width to colTags */
      ColTags.Width += lvManga.DisplayRectangle.Width - iStatic
        - lvManga.Columns[ColArtist.Index].Width
        - lvManga.Columns[ColTitle.Index].Width
        - lvManga.Columns[ColTags.Index].Width;
      lvManga.EndUpdate();
    }

    /// <summary>
    /// Scroll to the indicated position in the listview
    /// </summary>
    /// <param name="iPos">The listview index to scroll to</param>
    private void ScrollTo(int iPos)
    {
      if (lvManga.View == View.Details
          && iPos > -1 
          && iPos < lvManga.Items.Count) {
        lvManga.FocusedItem = lvManga.Items[iPos];
        lvManga.Items[iPos].Selected = true;
        lvManga.TopItem = lvManga.Items[iPos];
      }
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
      using (DataTable dt = SQL.GetMangaDetails(mangaID)) {
        if (dt.Rows.Count > 0) {
          Text = "Selected: " + Ext.GetFormattedTitle(
            dt.Rows[0]["Artists"].ToString(),
            dt.Rows[0]["Title"].ToString()
          );
          acTxBx_Title.Text = dt.Rows[0]["Title"].ToString();
          CmbBx_Artist.Text = dt.Rows[0]["Artists"].ToString();
          TxBx_Loc.Text = dt.Rows[0]["Location"].ToString();
          frTxBx_Desc.Text = dt.Rows[0]["Description"].ToString();
          CmbBx_Type.Text = dt.Rows[0]["Type"].ToString();
          Dt_Date.Value = DateTime.Parse(dt.Rows[0]["PublishedDate"].ToString());
          srRating.SelectedStar = Int32.Parse(dt.Rows[0]["Rating"].ToString());
          Nud_Pages.Value = Int32.Parse(dt.Rows[0]["PageCount"].ToString());
          lblURL.Text = dt.Rows[0]["GalleryURL"].ToString();
          acTxBx_Tags.Text = dt.Rows[0]["Tags"].ToString();
          UpdateReadProgress(Convert.ToInt32(dt.Rows[0]["PageReadCount"].ToString()));

          acTxBx_Tags.SetScroll();
          MnTS_New.Visible = false;
          MnTS_Edit.Visible = false;
          MnTS_Del.Visible = true;
          MnTS_Clear.Visible = true;
          Tb_View.ResumeLayout();

          //check for relativity
          if (!string.IsNullOrWhiteSpace(TxBx_Loc.Text)) {
            string sResult = Ext.FindPath(CmbBx_Artist.Text, acTxBx_Title.Text, TxBx_Loc.Text);
            if (sResult != null)
              TxBx_Loc.Text = sResult;
          }
        }
      }
    }

    private void UpdateReadProgress(int NewCount, bool SaveStatus = false)
    {
      this.page = NewCount;
      int iPageCount = Convert.ToInt32(Nud_Pages.Value);
      if (this.page <= iPageCount && iPageCount != 0) {
        progReadCount.Value = Convert.ToInt32(
          this.page == -1 ? 0 : this.page / (double)iPageCount * 100);
      }
      if (SaveStatus && mangaID != -1) {
        SQL.SaveReadProgress(mangaID, NewCount);
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
      try {
        MnTS_Open.Visible = (iExists == 1);
      } catch {
      }
    }

    private Bitmap LoadImage(string sPath)
    {
      Bitmap bmp = null;

      if (IsArchive(sPath)) {
        using (SCA.IArchive scArch = SCA.ArchiveFactory.Open(sPath)) {
          if (!IsArchiveAccessible(scArch)) {
            Console.WriteLine("The archive is password-protected and cannot be opened here.");
            return bmp;
          }

          int iCount = scArch.Entries.Count();
          //BeginInvoke(new DelInt(SetNudCount), iCount);

          if (iCount > 0) {
            //account for terrible default zip-sorting
            int iFirst = 0;
            SCA.IArchiveEntry[] scEntries = scArch.Entries.ToArray();
            string[] asFileTypes = new string[4] { ".jpg", ".jpeg", ".png", ".bmp" };

            List<string> lEntries = new List<string>(iCount);
            for (int i = 0; i < iCount; i++) {
              if (asFileTypes.Contains(Path.GetExtension(scEntries[i].FilePath).ToLower())) {
                lEntries.Add(scEntries[i].FilePath);
              }
            }
            if (lEntries.Count > 0) {
              lEntries.Sort(new TrueCompare());
              for (; iFirst < iCount; iFirst++) {
                if (scEntries[iFirst].FilePath.Length == lEntries[0].Length) {
                  if (scEntries[iFirst].FilePath.Equals(lEntries[0])) {
                    break;
                  }
                }
              }

              //load image
              try {
                using (MemoryStream ms = new MemoryStream()) {
                  scEntries[iFirst].WriteTo(ms);
                  bmp = new Bitmap(ms);
                }
              } catch {
                //MessageBox.Show("The following file could not be loaded:\n" + scEntries[iFirst].FilePath,
                //  Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //BeginInvoke(new DelInt(SetOpenStatus), 0);
              }
            }
          }
        }
      }
      else {
        try {
          bmp = new Bitmap(sPath);
        } catch {
          //MessageBox.Show("The following file could not be loaded:\n" + sPath,
          //  Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          //BeginInvoke(new DelInt(SetOpenStatus), 0);
        }
      }

      return bmp;
    }

    /// <summary>
    /// Enables the 'zip source' option
    /// </summary>
    private void SetZipSourceStatus()
    {
      MnTS_ZipSource.Enabled = true;
    }

    private void SetPreviewImage(Bitmap bmp)
    {
      if (bmp != null) {
        PicBx_Cover.Image = Ext.ScaleImage(bmp,
          PicBx_Cover.Width, PicBx_Cover.Height);
        SetOpenStatus(1);
        GC.Collect();
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
      string[] asProc = Ext.ParseGalleryTitle(sRaw);

      if (!string.IsNullOrWhiteSpace(asProc[0])) {
        if (string.IsNullOrWhiteSpace(CmbBx_Artist.Text)) {
          CmbBx_Artist.Text = asProc[0];
        }
        if (string.IsNullOrWhiteSpace(acTxBx_Title.Text)) {
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
    /// Refresh listview contents
    /// </summary>
    private void UpdateLV()
    {
      using (DataTable dt = SQL.GetAllManga(TxBx_Search.Text)) {
        Cursor = Cursors.WaitCursor;
        ListViewItem[] aItems = new ListViewItem[dt.Rows.Count];

        Color cRowColorHighlight = ((Color)SQL.GetSetting(SQL.Setting.RowColourHighlight));
        for (int i = 0; i < dt.Rows.Count; i++) {
          int iRating = Int32.Parse(dt.Rows[i]["Rating"].ToString());
          ListViewItem lvi = null;

          #region Set the row properties
          if (chkbxViewMode.Checked) {
            lvi = new ListViewItem(new string[3] {
              TrimLength(Ext.GetFormattedTitle(dt.Rows[i]["Artists"].ToString(), dt.Rows[i]["Title"].ToString()))
              , dt.Rows[i]["Location"].ToString()
              , dt.Rows[i]["mangaID"].ToString()
            });
            lvi.ImageKey = dt.Rows[i]["mangaID"].ToString();

            if (!lvManga.LargeImageList.Images.Keys.Contains(dt.Rows[i]["mangaID"].ToString())) {
              qManga.Enqueue(lvi);
            }
          }
          else {
            lvi = new ListViewItem(new string[8] {
              dt.Rows[i]["Artists"].ToString()
              ,dt.Rows[i]["Title"].ToString()
              ,dt.Rows[i]["PageCount"].ToString()
              ,dt.Rows[i]["Tags"].ToString()
              ,DateTime.Parse(dt.Rows[i]["PublishedDate"].ToString()).ToString(DATE_FORMAT)
              ,dt.Rows[i]["Type"].ToString()
              ,Ext.RatingFormat(iRating)
              ,dt.Rows[i]["mangaID"].ToString()
            });
          }

          if (iRating == 5) {
            lvi.BackColor = cRowColorHighlight;
          }
          #endregion

          aItems[i] = lvi;
        }

        lvManga.SuspendLayout();
        lvManga.Items.Clear();
        lvManga.Items.AddRange(aItems);
        lvManga.SortRows();
        lvManga.ResumeLayout();

        aiShuffle = null;
        Text = string.Format("{0}: {1:n0} entries",
          (!string.IsNullOrWhiteSpace(TxBx_Search.Text) ? "Returned" : "Manga Organizer")
          , dt.Rows.Count);
        Cursor = Cursors.Default;
      }
    }

    private string TrimLength(string sOrig)
    {
      if (sOrig.Length > 25) {
        sOrig = sOrig.Substring(0, 22) + "...";
      }
      return sOrig;
    }

    private void PopulateIcons(object obj)
    {
      do {
        if (bLoadImages && qManga.Count > 0) {
          ListViewItem aItem = qManga.Dequeue();

          Image bmp = null;
          string sLocation = aItem.SubItems[1].Text;
          int iMangaID = int.Parse(aItem.SubItems[2].Text);

          //get image
          bmp = SQL.GetThumbnail(iMangaID);
          if (bmp == null) {
            if (File.Exists(sLocation)) {
              bmp = LoadImage(sLocation);
            }
            else if (Directory.Exists(sLocation)) {
              string[] sFiles = new string[0];
              if ((sFiles = Ext.GetFiles(sLocation,
                  SearchOption.TopDirectoryOnly)).Length > 0) {
                bmp = LoadImage(sFiles[0]);
              }
            }
          }
          //ALSO CHECK BITMAP ERROR STATE
          if (bmp != null) {
            BeginInvoke(new DelMangaCover(UpdateIcon), new MangaCover(bmp, sLocation, iMangaID));
            bmp.Dispose();
          }
        }
        Thread.Sleep(bLoadImages ? 500 : 2000);
      } while (true);
    }

    public struct MangaCover
    {
      public ImageFormat imageFormat;
      public Image image;
      public string key;

      public MangaCover(Image bmp, string imagePath, int mangaID)
      {
        image = FormatIcon(bmp);
        key = mangaID.ToString();
        imageFormat = ImageFormat.Bmp;
        imageFormat = GetImageType(imagePath);
      }

      private ImageFormat GetImageType(string sPath)
      {
        switch (Path.GetExtension(sPath).ToLower()) {
          default:
          case "bmp":
            return ImageFormat.Bmp;
          case "jpeg":
          case "jpg":
            return ImageFormat.Jpeg;
          case "png":
            return ImageFormat.Png;
        }
      }
    }

    private static Image FormatIcon(Image bmp)
    {
      bmp = Ext.ScaleImage(bmp, ListViewNF.ICON_SIZE, ListViewNF.ICON_SIZE);
      Image baseImg = new Bitmap(ListViewNF.ICON_SIZE, ListViewNF.ICON_SIZE);
      using (Graphics g = Graphics.FromImage(baseImg)) {
        g.Clear(Color.White);
        int x = (baseImg.Width - bmp.Width) / 2;
        int y = (baseImg.Height - bmp.Height) / 2;
        g.DrawImage(bmp, x, y);
      }
      return baseImg;
    }

    private void UpdateIcon(MangaCover newCover)
    {
      //update listview
      for (int i = 0; i < lvManga.Items.Count; i++) {
        if (lvManga.Items[i].SubItems[2].Text == newCover.key) {
          lvManga.Items[i].ImageKey = newCover.key;
          break;
        }
      }

      try {
        lvManga.LargeImageList.Images.Add(newCover.key, newCover.image);
        SQL.SaveThumbnail(int.Parse(newCover.key), newCover.image, newCover.imageFormat);
      } catch (Exception) {
        Console.WriteLine(newCover.image);
      }

      GC.Collect(0);
    }

    #endregion

    #region Menu: Entry Operations
    /// <summary>
    /// Insert a new manga into the DB
    /// </summary>
    private void MnTS_New_Click(object sender, EventArgs e)
    {
      //reject when title is unfilled
      if (string.IsNullOrWhiteSpace(acTxBx_Title.Text)) {
        MessageBox.Show("Title cannot be empty.", Application.ProductName,
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (!SQL.ContainsEntry(CmbBx_Artist.Text, acTxBx_Title.Text)) {
        if (MessageBox.Show("Are you sure you wish to add:\n\""
            + Ext.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text) + "\"",
            Application.ProductName, MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes) {

          mangaID = SQL.SaveManga(CmbBx_Artist.Text, acTxBx_Title.Text, Dt_Date.Value,
            acTxBx_Tags.Text, TxBx_Loc.Text, Nud_Pages.Value, this.page, CmbBx_Type.Text,
            srRating.SelectedStar, frTxBx_Desc.Text, lblURL.Text);

          //refresh
          RefreshAutocomplete();
          AddEntries();
        }
      }
      else
        MessageBox.Show("This item already exists in the database.",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    /// <summary>
    /// Update the details of a current manga
    /// </summary>
    private void MnTS_Edit_Click(object sender, EventArgs e)
    {
      if (mangaID == -1)
        return;

      //overwrite entry properties
      string sEditNotice = Ext.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text);
      SQL.SaveManga(CmbBx_Artist.Text, acTxBx_Title.Text, Dt_Date.Value, acTxBx_Tags.Text, TxBx_Loc.Text,
        Nud_Pages.Value, this.page, CmbBx_Type.Text, srRating.SelectedStar, frTxBx_Desc.Text, lblURL.Text, mangaID);
      acTxBx_Tags.KeyWords = SQL.GetTags();

      //update auto-complete controls
      RefreshAutocomplete();

      //check if entry should still be displayed
      if (SQL.GetAllManga(TxBx_Search.Text, mangaID).Rows.Count > 0) {
        ListViewItem lvi = lvManga.FocusedItem;
        if (srRating.SelectedStar == 5) {
          lvi.BackColor = ((Color)SQL.GetSetting(SQL.Setting.RowColourHighlight));
        }
        else {
          lvManga.Alternate();
        }
        acTxBx_Tags.Text = SQL.GetMangaDetail(mangaID, SQL.Manga.Tags);
        lvi.SubItems[ColRating.Index].Text = Ext.RatingFormat(srRating.SelectedStar);
        lvi.SubItems[ColArtist.Index].Text = CmbBx_Artist.Text;
        lvi.SubItems[ColTitle.Index].Text = acTxBx_Title.Text;
        lvi.SubItems[ColPages.Index].Text = Nud_Pages.Value.ToString();
        lvi.SubItems[ColTags.Index].Text = acTxBx_Tags.Text;
        lvi.SubItems[colDate.Index].Text = Dt_Date.Value.ToString(DATE_FORMAT);
        lvi.SubItems[ColType.Index].Text = CmbBx_Type.Text;
        ReFocus();
      }
      else {
        lvManga.Items.RemoveAt(lvManga.FocusedItem.Index);
      }

      lvManga.SortRows();
      MnTS_Edit.Visible = false;
      Text = "Edited entry: " + sEditNotice;
    }

    /// <summary>
    /// Open the manag with the default image viewer
    /// </summary>
    private void MnTS_Open_Click(object sender, EventArgs e)
    {
      OpenFile();
    }

    /// <summary>
    /// Delete the selected manga
    /// </summary>
    private void MnTS_Delete_Click(object sender, EventArgs e)
    {
      if (mangaID == -1)
        return;

      //remove from database
      if (DeleteLocation(SQL.GetMangaDetail(mangaID, SQL.Manga.Location))) {
        int iPos = lvManga.FocusedItem.Index;
        SQL.DeleteManga(mangaID);

        //remove from listview
        lvManga.Items.RemoveAt(lvManga.SelectedItems[0].Index);
        Reset();
        ScrollTo(iPos);
      }
    }

    /// <summary>
    /// Reset the second form tab to default
    /// </summary>
    private void MnTS_Clear_Click(object sender, EventArgs e)
    {
      Reset();
    }
    #endregion

    #region Menu: Main
    /// <summary>
    /// Format the manga title as [Artist] Title
    /// </summary>
    private void MnTS_CopyTitle_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(acTxBx_Title.Text)) {
        MessageBox.Show("The title field cannot be empty.",
            Application.ProductName, MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        return;
      }

      Clipboard.SetText(
        Ext.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text));
      Text = "Name copied to clipboard";
    }

    /// <summary>
    /// Open the source directory/file of the manga
    /// </summary>
    private void MnTS_OpenSource_Click(object sender, EventArgs e)
    {
      OpenSourceLocation(TxBx_Loc.Text);
    }

    /// <summary>
    /// Converts a folder into a new cbz file
    /// </summary>
    private void MnTS_ZipSource_Click(object sender, EventArgs e)
    {
      string sBaseLoc = TxBx_Loc.Text.Trim();

      if (!File.Exists(sBaseLoc + ".cbz")) {
        SharpCompress.Common.CompressionInfo cmp = new SharpCompress.Common.CompressionInfo();
        this.Cursor = Cursors.WaitCursor;

        //zip the folder contents into a .cbz
        this.Text = "Compressing...";
        using (var archive = SCA.Zip.ZipArchive.Create()) {
          string[] asFiles = Directory.GetFiles(sBaseLoc);
          for (int x = 0; x < asFiles.Length; x++) {
            this.Text = string.Format("Compressing...{0}/{1}", x + 1, asFiles.Length);
            FileInfo fi = new FileInfo(asFiles[x]);
            archive.AddEntry(fi.Name, fi);
          }

          this.Text = "Saving to disk...";
          using (FileStream fs = new FileStream(sBaseLoc + ".cbz", FileMode.CreateNew)) {
            archive.SaveTo(fs, cmp);
          }
        }
        this.Text = "Compression complete";

        //Update the manga location and delete the old folder
        TxBx_Loc.Text = sBaseLoc + ".cbz";
        if (Ext.Accessible(sBaseLoc) != Ext.PathType.Invalid) {
          DeleteLocation(sBaseLoc);
        }
        MnTS_ZipSource.Enabled = false;
        this.Cursor = Cursors.Default;
      }
      else {
        MessageBox.Show("The file \"" + sBaseLoc + ".cbz\" already exists.",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    /// <summary>
    /// Uses EH API to get metadata from gallery URL
    /// </summary>
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

    /// <summary>
    /// Manually search and scrape EH results
    /// </summary>
    private void MnTs_SearchEH_Click(object sender, EventArgs e)
    {
      string sAutosearch = Ext.GetFormattedTitle(CmbBx_Artist.Text, acTxBx_Title.Text);

      using (Suggest fmSuggest = new Suggest()) {
        fmSuggest.SearchText = string.IsNullOrWhiteSpace(sAutosearch) ? Clipboard.GetText() : sAutosearch;
        fmSuggest.ShowDialog();

        if (fmSuggest.DialogResult == DialogResult.OK) {
          LoadEH(fmSuggest.SearchResult);
        }
      }
    }

    /// <summary>
    /// Open the database folder
    /// </summary>
    private void MnTS_OpenDataFolder_Click(object sender, EventArgs e)
    {
      string sPath = !string.IsNullOrWhiteSpace(Properties.Settings.Default.SavLoc) ?
        Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;

      if (Directory.Exists(sPath))
        System.Diagnostics.Process.Start(sPath);
      else
        MessageBox.Show("This directory no longer exists.", Application.ProductName,
          MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Remove unused tags
    /// </summary>
    private void MnTs_CleanTags_Click(object sender, EventArgs e)
    {
      int iUnused = SQL.CleanUpTags();
      MessageBox.Show(iUnused > 0 ? iUnused + " unused tags have been removed." : "No unused tags exist."
        , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      acTxBx_Tags.KeyWords = SQL.GetTags();
    }

    /// <summary>
    /// Remove unused artists
    /// </summary>
    private void MnTS_CleanArtists_Click(object sender, EventArgs e)
    {
      int iUnused = SQL.CleanUpArtists();
      MessageBox.Show(iUnused > 0 ? iUnused + " unused artists have been removed." : "No unused artists exist."
        , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      CmbBx_Artist.Items.Clear();
      CmbBx_Artist.Items.AddRange(SQL.GetArtists());
    }

    /// <summary>
    /// Performs DB cleanup tasks that don't fall into a user-friendly category
    /// </summary>
    private void MnTS_DBMaintenance_Click(object sender, EventArgs e)
    {
      int iAltered = SQL.CleanUpReferences();
      MessageBox.Show("Refreshed tag references; " + iAltered + " records affected."
        , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Displays all entries whose source file cannot be found
    /// </summary>
    private void MnTS_missingSource_Click(object sender, EventArgs e)
    {
      //add missing entries
      using (DataTable dt = SQL.GetAllManga()) {
        Cursor = Cursors.WaitCursor;
        List<ListViewItem> lItems = new List<ListViewItem>(dt.Rows.Count);

        Color cRowColorHighlight = ((Color)SQL.GetSetting(SQL.Setting.RowColourHighlight));
        for (int i = 0; i < dt.Rows.Count; i++) {
          string sPath = dt.Rows[i]["Location"].ToString();
          if (!(File.Exists(sPath) || Directory.Exists(sPath))) {
            int iRating = Int32.Parse(dt.Rows[i]["Rating"].ToString());

            #region Set the row properties
            ListViewItem lvi = new ListViewItem(new string[8] {
              dt.Rows[i]["Artist"].ToString()
              ,dt.Rows[i]["Title"].ToString()
              ,dt.Rows[i]["Pages"].ToString()
              ,dt.Rows[i]["Tags"].ToString()
              ,DateTime.Parse(dt.Rows[i]["PublishedDate"].ToString()).ToString(DATE_FORMAT)
              ,dt.Rows[i]["Type"].ToString()
              ,Ext.RatingFormat(iRating)
              ,dt.Rows[i]["mangaID"].ToString()
            });

            if (iRating == 5) {
              lvi.BackColor = cRowColorHighlight;
            }
            #endregion

            lItems.Add(lvi);
          }
        }

        string sMsg = lItems.Count + " entries were found missing their source file or directory.";
        if (lItems.Count > 0) {

          //remove search parameters
          TxBx_Search.Text = "MISSING_SOURCE";
          aiShuffle = null;

          lvManga.SuspendLayout();
          lvManga.Items.Clear();
          lvManga.Items.AddRange(lItems.ToArray());
          lvManga.SortRows();
          lvManga.ResumeLayout();

          Text = string.Format("Returned: {0:n0} entries", lvManga.Items.Count);
          sMsg += "\nPlease go back to the 'Browse' tab to view them.";
        }

        MessageBox.Show(sMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        Cursor = Cursors.Default;
      }
    }

    /// <summary>
    /// Show the tag statistics
    /// </summary>
    private void MnTS_Stats_Click(object sender, EventArgs e)
    {
      Stats fmStats = new Stats();
      fmStats.Show();
    }

    /// <summary>
    /// Allow users to alter their settings
    /// </summary>
    private void Mn_Settings_Click(object sender, EventArgs e)
    {
      string sOld = Properties.Settings.Default.SavLoc;
      Form fmSet = new Settings();
      fmSet.ShowDialog();

      if (fmSet.DialogResult == DialogResult.Yes) {
        lvManga.GridLines = ((bool)SQL.GetSetting(SQL.Setting.ShowGrid));
        PicBx_Cover.BackColor = ((Color)SQL.GetSetting(SQL.Setting.BackgroundColour));
        lvManga.Columns[4].Width = ((bool)SQL.GetSetting(SQL.Setting.ShowDate)) ? Convert.ToInt32(colDate.Tag) : 0;
        ResizeLV();

        //Update new DB save location
        if (sOld != Properties.Settings.Default.SavLoc) {
          string sNew = Properties.Settings.Default.SavLoc + "\\MangaDB.sqlite";
          sOld += "\\MangaDB.sqlite";

          //move current DB
          if (!File.Exists(sNew) && File.Exists(sNew)) {
            File.Move(sOld, sNew);
          }
          else if (File.Exists(sNew) && lvManga.Items.Count > 0) {
            MessageBox.Show("A database already exists at this location. We'll load it and leave your old DB in place.",
              Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
          }

          //reconnect & repopulate
          SQL.Disconnect();
          SQL.Connect();
          if (SQL.IsConnected) {
            UpdateLV();

            //set up CmbBx autocomplete
            CmbBx_Type.Items.Clear();
            CmbBx_Artist.Items.Clear();
            CmbBx_Artist.Items.AddRange(SQL.GetArtists());
            CmbBx_Type.Items.AddRange(SQL.GetTypes());
            acTxBx_Tags.KeyWords = SQL.GetTags();

            Text = "Default save location changed";
          }
          else {
            MessageBox.Show("Could not establish a connection with the database.",
              Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
        }
      }
      fmSet.Dispose();
    }

    /// <summary>
    /// Show the tutorial form
    /// </summary>
    private void MnTS_Tutorial_Click(object sender, EventArgs e)
    {
      Tutorial fmTut = new Tutorial();
      fmTut.ShowDialog();
      fmTut.Dispose();
    }

    /// <summary>
    /// Show the About form
    /// </summary>
    private void MnTS_About_Click(object sender, EventArgs e)
    {
      About fmAbout = new About();
      fmAbout.ShowDialog();
      fmAbout.Dispose();
    }

    /// <summary>
    /// Close the program
    /// </summary>
    private void MnTs_Quit_Click(object sender, EventArgs e)
    {
      this.Close();
    }
    #endregion

    #region Menu_Context
    /// <summary>
    /// Control the options available in the context menu
    /// </summary>
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

    /// <summary>
    /// Run the undo action on the selected field
    /// </summary>
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

    /// <summary>
    /// Run the cut action on the selected field
    /// </summary>
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

    /// <summary>
    /// Run the copy action on the selected field
    /// </summary>
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

    /// <summary>
    /// Run the paste action on the selected field
    /// </summary>
    private void MnTx_Paste_Click(object sender, EventArgs e)
    {
      string sAdd = Clipboard.GetText();
      switch (Mn_TxBx.SourceControl.GetType().Name) {
        case "FixedRichTextBox":
          FixedRichTextBox fr = ((FixedRichTextBox)ActiveControl);
          fr.SelectedText = sAdd;
          if (TabControl.SelectedIndex == 2) {
            bSavNotes = false;
          }
          break;
        case "TextBox":
        case "AutoCompleteTagger":
          if (ActiveControl.Name == "TxBx_Search") {
            string[] asTitle = Ext.ParseGalleryTitle(sAdd);
            TxBx_Search.SelectedText = (string.IsNullOrWhiteSpace(asTitle[0])) ? sAdd :
              string.Format("artist:{0} title:{1}"
                , asTitle[0].Replace(' ', '_')
                , asTitle[1].Replace(' ', '_')
              );
            UpdateLV();
            break;
          }
          else {
            TextBox txt = (TextBox)ActiveControl;
            if (txt.Name == "acTxBx_Tags" && sAdd.Contains("\r\n")) {
              UpdateTags(sAdd);
            }
            else {
              txt.SelectedText = sAdd;
            }
          }
          break;
        case "ComboBox":
          if (sAdd.Contains('[')) {
            SetTitle(sAdd);
          }
          else {
            ((ComboBox)ActiveControl).SelectedText = sAdd;
          }
          break;
      }
    }

    /// <summary>
    /// Run the delete action on the selected field
    /// </summary>
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

    /// <summary>
    /// Run the select all action on the selected field
    /// </summary>
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

    /// <summary>
    /// Small improvements to the textboxes to strip out text 
    /// formatting and stop console beeps
    /// </summary>
    private void frTxBx_KeyDown(object sender, KeyEventArgs e)
    {
      /* Prevent mixed font types when c/p  */
      if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
        FixedRichTextBox frTxBx = sender as FixedRichTextBox;
        frTxBx.SelectionStart = Ext.InsertText(
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
    /// <summary>
    /// Show proper cursor when text is dragged over TxBx
    /// </summary>
    private void DragEnterTxBx(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.Text))
        e.Effect = DragDropEffects.Copy;
      else
        e.Effect = DragDropEffects.None;
    }

    /// <summary>
    /// Parse dropped text into TxBx(s)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragDropTxBx(object sender, DragEventArgs e)
    {
      string sAdd = (string)e.Data.GetData(DataFormats.Text);

      switch ((sender as Control).Name) {
        case "frTxBx_Desc":
          frTxBx_Desc.SelectionStart = Ext.InsertText(
              frTxBx_Desc, sAdd, frTxBx_Desc.SelectionStart);
          break;
        case "frTxBx_Notes":
          frTxBx_Notes.SelectionStart = Ext.InsertText(
              frTxBx_Notes, sAdd, frTxBx_Notes.SelectionStart);
          break;
        case "CmbBx_Artist":
          if (sAdd.Contains('[') && sAdd.Contains(']'))
            SetTitle(sAdd);
          else
            CmbBx_Artist.SelectionStart = Ext.InsertText(
              CmbBx_Artist, sAdd, CmbBx_Artist.SelectionStart);
          break;
        case "acTxBx_Title":
          if (sAdd.Contains('[') && sAdd.Contains(']'))
            SetTitle(sAdd);
          else
            acTxBx_Title.SelectionStart = Ext.InsertText(
              acTxBx_Title, sAdd, acTxBx_Title.SelectionStart);
          break;
        case "acTxBx_Tags":
          if (sAdd.Contains("\r\n")) {
            UpdateTags(sAdd);
          }
          else {
            acTxBx_Tags.SelectionStart = Ext.InsertText(
              acTxBx_Tags, sAdd, acTxBx_Tags.SelectionStart);
          }
          break;
        case "TxBx_Search":
          string[] asTitle = Ext.ParseGalleryTitle(sAdd);
          sAdd = (string.IsNullOrWhiteSpace(asTitle[0])) ? sAdd :
              string.Format("artist:{0} title:{1}",
              asTitle[0].Replace(' ', '_'), asTitle[1].Replace(' ', '_'));
          TxBx_Search.SelectionStart = Ext.InsertText(
              TxBx_Search, sAdd, TxBx_Search.SelectionStart);
          UpdateLV();
          break;
      }
    }

    /// <summary>
    /// Try to auto-get the artist/title and cover from the location
    /// </summary>
    private void TxBx_Loc_DragDrop(object sender, DragEventArgs e)
    {
      string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
      TxBx_Loc.Text = asDir[0];

      if (Directory.Exists(asDir[0])
          || (File.Exists(asDir[0]) && IsArchive(asDir[0]))) {
        if (string.IsNullOrWhiteSpace(CmbBx_Artist.Text) && string.IsNullOrWhiteSpace(acTxBx_Title.Text)) {
          SetTitle(Ext.GetNameSansExtension(asDir[0]));
          ThreadPool.QueueUserWorkItem(GetImage, TxBx_Loc.Text);
        }
      }
    }

    /// <summary>
    /// If it's valid, set the date 
    /// </summary>
    private void Dt_Date_DragDrop(object sender, DragEventArgs e)
    {
      DateTime dtDrop;
      if (DateTime.TryParse(
          (string)e.Data.GetData(DataFormats.Text),
          out dtDrop)) {
        Dt_Date.Value = dtDrop;
      }
      else {
        MessageBox.Show("The dropped text was not a valid date.",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    /// <summary>
    /// If it's valid, set the page count
    /// </summary>
    private void Nud_Pages_DragDrop(object sender, DragEventArgs e)
    {
      decimal dcDrop;
      if (decimal.TryParse(
          (string)e.Data.GetData(DataFormats.Text),
          out dcDrop)) {
        Nud_Pages.Value = dcDrop;
      }
      else {
        MessageBox.Show("The dropped text was not a valid value.",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    /// <summary>
    /// Allow dropping of folders/zips onto LV_Entries (& TxBx_Loc)
    /// </summary>
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

    /// <summary>
    /// Adds folder(s) to database when dragged over LV_Entries
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LV_Entries_DragDrop(object sender, DragEventArgs e)
    {
      string[] asDir = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
      string sError = "";
      int iTrack = mangaID;

      //add all remaining folders
      for (int i = 0; i < asDir.Length; i++) {
        if (string.IsNullOrWhiteSpace(asDir[i])
            || (!Directory.Exists(asDir[i])
                && !IsArchive(asDir[i])))
          continue;

        //add item
        csEntry en = new csEntry(asDir[i]);
        if (!SQL.ContainsEntry(en.sArtist, en.sTitle)) {
          mangaID = SQL.SaveManga(en.sArtist, en.sTitle, en.dtDate, en.sTags,
            en.sLoc, en.pages, 0, en.sType, en.byRat, en.sDesc);

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

    #region Classes

    /* Holds manga metadata */
    public class csEntry
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
              x => !string.IsNullOrWhiteSpace(x)).ToArray<string>();
          sTags = String.Join(", ", sRaw.OrderBy(x => x));
        }
      }

      public csEntry(string _Path)
      {
        //Try to format raw title string
        string[] asTitle = Ext.ParseGalleryTitle(
            Ext.GetNameSansExtension(_Path));
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
        if (File.Exists(_Path)) {
          sFiles = new string[1] { _Path };
        }
        else {
          sFiles = Ext.GetFiles(_Path,
            SearchOption.TopDirectoryOnly, "*.zip|*.cbz|*.cbr|*.rar|*.7z");
        }

        if (sFiles.Length > 0) {
          if (Main.IsArchive(sFiles[0])) {
            using (SCA.IArchive scArchive = SCA.ArchiveFactory.Open(sFiles[0])) {
              if (Main.IsArchiveAccessible(scArchive)) {
                pages = (scArchive.Entries.Count() > ushort.MaxValue) ?
                  ushort.MaxValue : (ushort)Math.Abs(scArchive.Entries.Count());
              }
            }
          }
        }
        else {
          pages = (ushort)Ext.GetFiles(
            _Path, SearchOption.TopDirectoryOnly).Length;
        }
      }
    }

    #endregion
  }
}