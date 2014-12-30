using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  public partial class Scan : Form
  {
    private delegate void DelVoidEntry(Main.csEntry en);
    public delegate void DelVoid();
    public DelVoid delNewEntry, delDone;
    bool bIsClosing = false;

    HashSet<string> hsPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    HashSet<string> hsIgnore = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    List<Main.csEntry> lFound = new List<Main.csEntry>();

    #region ScanOperation
    public Scan()
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;
    }

    private void Scan_Load(object sender, EventArgs e)
    {
      LV_Found.GridLines = SQL.GetSetting(SQL.Setting.ShowGrid) == "1";
      LV_Found_Resize(sender, e);

      string[] sRaw = SQL.GetSetting(SQL.Setting.SearchIgnore).Split('|');
      for (int i = 0; i < sRaw.Length - 1; i++)
        hsIgnore.Add(sRaw[i]);

      using (DataTable dt = SQL.GetAllManga()) {
        for (int i = 0; i < dt.Rows.Count; i++)
          hsPaths.Add(dt.Rows[i]["Location"].ToString());
      }

      //auto-scan at load
      string sPath = SQL.GetSetting(SQL.Setting.RootPath);
      if (sPath == string.Empty || !Directory.Exists(sPath))
        sPath = Environment.CurrentDirectory;
      TxBx_Loc.Text = sPath;
      TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
      TryScan();
    }

    /* Scan selected directory */
    private void TxBx_Loc_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      fbd.RootFolder = Environment.SpecialFolder.Desktop;
      fbd.Description = "Select the directory you want to scan.";

      if (string.IsNullOrWhiteSpace(TxBx_Loc.Text) || !Directory.Exists(TxBx_Loc.Text))
        fbd.SelectedPath = Environment.CurrentDirectory;
      else
        fbd.SelectedPath = TxBx_Loc.Text;

      if (fbd.ShowDialog() == DialogResult.OK) {
        TxBx_Loc.Text = fbd.SelectedPath;
        TxBx_Loc.SelectionStart = TxBx_Loc.Text.Length;
        TryScan();
      }
      fbd.Dispose();
      LV_Found.Select();
    }

    /* Start scan op in new thread */
    private void TryScan()
    {
      if (Ext.Accessible(TxBx_Loc.Text)) {
        Cursor = Cursors.WaitCursor;

        lFound.Clear();
        LV_Found.Items.Clear();
        LV_Found.ListViewItemSorter = null;
        this.Text = "Scan";

        Thread trdScan = new Thread(ScanDir);
        trdScan.IsBackground = true;
        trdScan.Start(TxBx_Loc.Text);
      }
      else
        MessageBox.Show("Cannot read from the selected folder path.",
          "Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /* Scan passed directory for entries */
    private void ScanDir(Object obj)
    {
      List<string> lEns = new List<string>();

      try {
        lEns.AddRange(Ext.GetFiles(obj as string,
          SearchOption.TopDirectoryOnly, "*.zip|*.cbz|*.rar|*.cbr|*.7z"));
        lEns.AddRange(Directory.EnumerateDirectories(
          obj as string, "*", SearchOption.TopDirectoryOnly));
      } catch (ArgumentException) {
        MessageBox.Show("An invalid object got passed through!",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      } catch (UnauthorizedAccessException) {
        MessageBox.Show("User does not have access to one of the folders/files scanned!",
          Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      } catch (Exception ex) {
        MessageBox.Show(ex.Message, Application.ProductName,
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      int i = 0;
      while (i < lEns.Count && !bIsClosing) {
        if (!hsPaths.Contains(lEns[i])) {
          try {
            Main.csEntry en = new Main.csEntry(lEns[i]);
            BeginInvoke(new DelVoidEntry(AddItem), en);
          } catch (Exception) {
            MessageBox.Show(string.Format("{0}:\n{1}\n\n{2}"
              , "An unexpected error occurred while trying to parse the entry at"
              , lEns[i]
              , "Please contact the developer at nagru@live.ca with the details."),
              Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
        i++;
      }
      if (!bIsClosing) {
        BeginInvoke(new DelVoid(SetFoundItems));
      }
    }

    /* Add new item to listview */
    private void AddItem(Main.csEntry en)
    {
      if (en.pages == 0)
        return;
      ListViewItem lvi = new ListViewItem(en.sArtist);
      lvi.SubItems.AddRange(new string[] { 
          en.sTitle, 
          en.pages.ToString(),
          lFound.Count.ToString()
      });
      lFound.Add(en);

      if (hsIgnore.Contains(en.sArtist + en.sTitle)) {
        if (ChkBx_All.Checked) {
          lvi.BackColor = Color.MistyRose;
          LV_Found.Items.Add(lvi);
        }
      }
      else
        LV_Found.Items.Add(lvi);

      this.SuspendLayout();
      Text = "Scan: Found " + LV_Found.Items.Count + " possible entries";
      this.ResumeLayout();
    }

    /* Signal finished scan */
    private void SetFoundItems()
    {
      LV_Found.SortRows();

      Cursor = Cursors.Default;
    }
    #endregion

    #region DisplayMethods
    /*Update listview with all 'missing' entries */
    private void UpdateLV()
    {
      Cursor = Cursors.WaitCursor;
      List<ListViewItem> lItems = new List<ListViewItem>(LV_Found.Items.Count + 1);

      for (int i = 0; i < lFound.Count; i++) {
        ListViewItem lvi = new ListViewItem(lFound[i].sArtist);
        lvi.SubItems.AddRange(new string[] {
          lFound[i].sTitle,
          lFound[i].pages.ToString(),
          i.ToString()
        });

        if (hsIgnore.Contains(lFound[i].sArtist + lFound[i].sTitle)) {
          if (ChkBx_All.Checked) {
            lvi.BackColor = Color.MistyRose;
            lItems.Add(lvi);
          }
        }
        else {
          lItems.Add(lvi);
        }
      }

      LV_Found.BeginUpdate();
      LV_Found.Items.Clear();
      LV_Found.Items.AddRange(lItems.ToArray());
      LV_Found.SortRows();
      LV_Found.EndUpdate();

      LV_Found.Select();
      Cursor = Cursors.Default;
      Text = "Found " + LV_Found.Items.Count + " possible entries";
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

    private void LV_Found_MouseHover(object sender, EventArgs e)
    {
      if (!LV_Found.Focused)
        LV_Found.Select();
    }

    private void LV_Found_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
      e.NewWidth = LV_Found.Columns[e.ColumnIndex].Width;
    }

    /* Toggle whether LV shows ignored items */
    private void ChkBx_All_CheckedChanged(object sender, EventArgs e)
    {
      UpdateLV();
    }
    #endregion

    #region ManipulateItems
    /*Open folder of double-clicked item */
    private void LV_Found_DoubleClick(object sender, EventArgs e)
    {
      int iPos = int.Parse(LV_Found.FocusedItem.SubItems[3].Text);
      Process.Start(lFound[iPos].sLoc);
    }

    /* Sends selected item(s) back to Main */
    private void Btn_Add_Click(object sender, EventArgs e)
    {
      if (LV_Found.SelectedItems.Count == 0)
        return;

      this.Cursor = Cursors.WaitCursor;
      int[] irmList = new int[LV_Found.SelectedItems.Count];
      int[] irmView = new int[LV_Found.SelectedItems.Count];
      SQL.BeginTransaction();
      for (int i = 0; i < LV_Found.SelectedItems.Count; i++) {
        //save entry and remove from list
        int iPos = Int32.Parse(LV_Found.SelectedItems[i].SubItems[3].Text);
        SQL.SaveManga(lFound[iPos].sArtist, lFound[iPos].sTitle, lFound[iPos].dtDate,
          lFound[iPos].sTags, lFound[iPos].sLoc, lFound[iPos].pages,
          lFound[iPos].sType, lFound[iPos].byRat, lFound[iPos].sDesc);
        irmView[i] = LV_Found.SelectedItems[i].Index;
        irmList[i] = iPos;
      }
      SQL.CommitTransaction();
      
      Array.Sort(irmList);
      Array.Sort(irmView);
      LV_Found.BeginUpdate();
      for (int i = LV_Found.SelectedItems.Count - 1; i > -1; i--) {
        LV_Found.Items.RemoveAt(irmView[i]);
        lFound.RemoveAt(irmList[i]);
      }
      LV_Found.EndUpdate();

      if (delNewEntry != null)
        delNewEntry.Invoke();
      this.Cursor = Cursors.Default;
    }

    /* Add or remove item from ignored list based on context */
    private void Btn_Ignore_Click(object sender, EventArgs e)
    {
      if (LV_Found.SelectedItems.Count == 0)
        return;

      LV_Found.BeginUpdate();
      for (int i = 0; i < LV_Found.SelectedItems.Count; i++) {
        string sItem = LV_Found.SelectedItems[i].SubItems[0].Text +
                LV_Found.SelectedItems[i].SubItems[1].Text;

        if (hsIgnore.Contains(sItem)) {
          hsIgnore.Remove(sItem);
          LV_Found.SelectedItems[i].BackColor = SystemColors.Window;
        }
        else {
          hsIgnore.Add(sItem);
          if (!ChkBx_All.Checked)
            LV_Found.SelectedItems[i--].Remove();
          else
            LV_Found.SelectedItems[i].BackColor = Color.MistyRose;
        }
      }
      LV_Found.EndUpdate();
    }

    private void Scan_FormClosing(object sender, FormClosingEventArgs e)
    {
      bIsClosing = true;

      //preserve ignored items
      string sNew = "";
      foreach (string svar in hsIgnore)
        if (svar != "")
          sNew += svar + '|';

      SQL.UpdateSetting(SQL.Setting.SearchIgnore, sNew);
      hsIgnore.Clear();
      hsPaths.Clear();
      lFound.Clear();

      //update main form
      if (delDone != null)
        delDone.Invoke();
    }
    #endregion

    #region Menu_Text
    private void MnTx_Undo_Click(object sender, EventArgs e)
    {
      if (TxBx_Loc.CanUndo)
        TxBx_Loc.Undo();
    }

    private void MnTx_Cut_Click(object sender, EventArgs e)
    {
      TxBx_Loc.Cut();
    }

    private void MnTx_Copy_Click(object sender, EventArgs e)
    {
      TxBx_Loc.Copy();
    }

    private void MnTx_Paste_Click(object sender, EventArgs e)
    {
      TxBx_Loc.Paste();
    }

    private void MnTx_SelAll_Click(object sender, EventArgs e)
    {
      TxBx_Loc.SelectAll();
    }
    #endregion
  }
}
