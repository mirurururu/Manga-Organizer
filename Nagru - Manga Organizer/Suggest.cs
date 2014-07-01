using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Nagru___Manga_Organizer
{
  public partial class Suggest : Form
  {
    delegate void DelClassVoid(csEHSearch csPass);
    public string sChoice = "";

    DelClassVoid delResults = null;
    LVsorter lvSortObj = new LVsorter();
    csEHSearch csSearch = new csEHSearch();
    string sTrySearch = string.Empty;

    public Suggest(string sRaw)
    {
      InitializeComponent();
      if (!string.IsNullOrEmpty(sRaw))
        sTrySearch = sRaw;
      else
        sTrySearch = Clipboard.GetText();
    }

    private void Suggest_Load(object sender, EventArgs e)
    {
      lvDetails.ListViewItemSorter = lvSortObj;
      delResults = DisplayResults;
      ResizeLV();

      //remove active border on form acceptbutton
      this.AcceptButton.NotifyDefault(false);

      //set-up user choices
      lvDetails.GridLines = SQL.GetSetting(SQL.Setting.ShowGrid) == "1";
      for (int i = 0; i < csSearch.Options.Length; i++) {
        (ddmGallery.DropDownItems[i] as ToolStripMenuItem).Checked = csSearch.Options[i];
      }

      //get system icon for help button
      tsbtnHelp.Image = SystemIcons.Information.ToBitmap();

      //input user credentials
      txbxPass.Text = SQL.GetSetting(SQL.Setting.pass_hash);
      txbxID.Text = SQL.GetSetting(SQL.Setting.member_id);

      //auto-format search terms where applicable
      if (sTrySearch.Contains("[")) {
        StringBuilder sb = new StringBuilder("");
        string[] asSplit = ExtString.ParseGalleryTitle(sTrySearch);

        //check for artist/title fields and set formatting
        if (!string.IsNullOrEmpty(asSplit[0])) {
          sb.AppendFormat("{0}{1}", (asSplit[0].Contains("("))
            ? "" : "artist:", asSplit[0]);

          sb.AppendFormat(" {0}", asSplit[1]);
          txbxSearch.Text = sb.ToString();
        }
      }
      txbxSearch.Select();
      txbxSearch.SelectionStart = txbxSearch.Text.Length;
    }

    private void tsbtn_Help_Clicked(object sender, EventArgs e)
    {
      MessageBox.Show("These credentials are the 'content' parameters of the exhentai cookies. "
        + "If not provided, this program will use g.e-hentai instead.",
        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    //update searched gallery types
    private void GalleryCheckedChanged(object sender, EventArgs e)
    {
      csSearch.Options = new bool[10] {
				ckbxDoujin.Checked,		ckbxManga.Checked,
				ckbxArtist.Checked,		ckbxGame.Checked,
				ckbxWestern.Checked,	ckbxNonH.Checked,
				ckbxImage.Checked,		ckbxCosplay.Checked,
				ckbxAsian.Checked,		ckbxMisc.Checked
      };
      ddmGallery.ShowDropDown();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      ToggleButtonEnabled(ref btnSearch);
      ThreadPool.QueueUserWorkItem(Search, txbxSearch.Text);
      this.Cursor = Cursors.WaitCursor;
    }

    private void Search(object obj)
    {
      if (!(obj is string))
        return;

      csSearch.Search((string)obj);
      if (delResults != null) {
        this.Invoke(delResults, csSearch);
      }
      else {
        MessageBox.Show("Search was invoked before the delegate was bound.",
          Application.ProductName, MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }

    private void DisplayResults(csEHSearch csResults)
    {
      if (csResults.Count > 0) {
        lvDetails.SuspendLayout();
        lvDetails.Items.Clear();

        for (int i = 0; i < csResults.Count; i++) {
          lvDetails.Items.Add(new ListViewItem(new string[2] {
            csResults.URL(i),
            csResults.Title(i)
          }));
        }

        Alternate();
        lvDetails.ResumeLayout();
      }

      this.Cursor = Cursors.Default;
      ToggleButtonEnabled(ref btnSearch);
      this.Text = "Search found " + csResults.Count + " possible matchess";
    }

    private void lvDetails_SelectedIndexChanged(object sender, EventArgs e)
    {
      //en/disable button depending on whether an item is selected
      ToggleButtonEnabled(ref btnOK, lvDetails.SelectedItems.Count > 0);
    }

    private void ToggleButtonEnabled(ref Button btn, bool? bEnabled = null)
    {
      btn.Enabled = (bEnabled == null) ?
        !btn.Enabled : (bool)bEnabled;
      btn.BackColor = btn.Enabled ?
        SystemColors.ButtonFace : SystemColors.ScrollBar;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      if (lvDetails.SelectedItems.Count > 0) {
        sChoice = lvDetails.SelectedItems[0].SubItems[0].Text;
        this.DialogResult = DialogResult.OK;
      }

      this.Close();
    }

    private void Suggest_FormClosing(object sender, FormClosingEventArgs e)
    {
      csSearch.SaveOptions();
    }

    #region listview methods
    private void lvDetails_Resize(object sender, EventArgs e)
    {
      ResizeLV();
    }

    private void ResizeLV()
    {
      lvDetails.BeginUpdate();
      colTitle.Width = lvDetails.DisplayRectangle.Width - colURL.Width;
      lvDetails.EndUpdate();
    }

    /* Prevent user changing column sizes */
    private void lvDetails_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
      e.NewWidth = lvDetails.Columns[e.ColumnIndex].Width;
    }

    private void lvDetails_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (e.Column != lvSortObj.SortingColumn)
        lvSortObj.NewColumn(e.Column, SortOrder.Ascending);
      else
        lvSortObj.SwapOrder();

      lvDetails.Sort();
      Alternate();
    }

    //open selected url in browser
    private void lvDetails_DoubleClick(object sender, EventArgs e)
    {
      if (lvDetails.SelectedItems.Count > 0) {
        System.Diagnostics.Process.Start(
            lvDetails.SelectedItems[0].SubItems[0].Text);
      }
    }

    private void Alternate(int iStart = 0)
    {
      if (SQL.GetSetting(SQL.Setting.ShowGrid) == "1")
        return;
      for (int i = iStart; i < lvDetails.Items.Count; i++) {
        lvDetails.Items[i].BackColor = (i % 2 != 0) ?
          Color.FromArgb(245, 245, 245) : SystemColors.Window;
      }
    }
    #endregion

    #region credential handling
    private void tsbtnHelp_Click(object sender, EventArgs e)
    {
      MessageBox.Show("", Application.ProductName,
          MessageBoxButtons.OK, MessageBoxIcon.Question);
    }
    private void txbxPass_TextChanged(object sender, EventArgs e)
    {
      SQL.UpdateSetting("pass_hash", txbxPass.Text);
    }

    private void txbxID_TextChanged(object sender, EventArgs e)
    {
      SQL.UpdateSetting("member_id", txbxID.Text);
    }
    #endregion

    #region Menu_Text
    private void Mn_TxBx_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      txbxSearch.Select();
    }

    private void MnTx_Undo_Click(object sender, EventArgs e)
    {
      if (txbxSearch.CanUndo)
        txbxSearch.Undo();
    }

    private void MnTx_Cut_Click(object sender, EventArgs e)
    {
      txbxSearch.Cut();
    }

    private void MnTx_Copy_Click(object sender, EventArgs e)
    {
      txbxSearch.Copy();
    }

    private void MnTx_Paste_Click(object sender, EventArgs e)
    {
      txbxSearch.Paste();
    }

    private void MnTx_Delete_Click(object sender, EventArgs e)
    {
      txbxSearch.SelectedText = "";
    }

    private void MnTx_SelAll_Click(object sender, EventArgs e)
    {
      txbxSearch.SelectAll();
    }
    #endregion
  }
}
