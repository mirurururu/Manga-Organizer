using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  public partial class Suggest : Form
  {
    #region Properties
    public string SearchText
    {
      set
      {
        sTrySearch = value;
      }
    }
    public string SearchResult
    {
      get
      {
        return sChoice;
      }
    }

    delegate void DelClassVoid(csEHSearch csPass);
    DelClassVoid delResults = null;
    csEHSearch csSearch = new csEHSearch();
    string sTrySearch, sChoice;
    #endregion

    #region Form

    public Suggest()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Set up form page according to saved settings
    /// </summary>
    private void Suggest_Load(object sender, EventArgs e)
    {
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
      string memberID = SQL.GetSetting(SQL.Setting.member_id);
      txbxPass.Text = SQL.GetSetting(SQL.Setting.pass_hash);
      if(memberID != "-1") {
        txbxID.Text = memberID;
      }

      //auto-format search terms where applicable
      if (sTrySearch.Contains("[")) {
        StringBuilder sb = new StringBuilder("");
        string[] asSplit = Ext.ParseGalleryTitle(sTrySearch);

        //check for artist/title fields and set formatting
        if (!string.IsNullOrWhiteSpace(asSplit[0])) {
          sb.AppendFormat("{0}{1}", (asSplit[0].Contains("("))
            ? "" : "artist:", asSplit[0]);

          sb.AppendFormat(" {0}", asSplit[1]);
          txbxSearch.Text = sb.ToString();
        }
      }
      txbxSearch.Select();
      txbxSearch.SelectionStart = txbxSearch.Text.Length;
    }

    /// <summary>
    /// Save current settings
    /// </summary>
    private void Suggest_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(txbxID.Text)) {
        SQL.UpdateSetting(SQL.Setting.member_id, txbxID.Text);
        SQL.UpdateSetting(SQL.Setting.pass_hash, txbxPass.Text);
      }

      csSearch.SaveOptions();
    }

    #endregion

    #region Events

    #region listview methods

    private void lvDetails_SelectedIndexChanged(object sender, EventArgs e)
    {
      //en/disable button depending on whether an item is selected
      ToggleButtonEnabled(ref btnOK, lvDetails.SelectedItems.Count > 0);
    }

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

    private void lvDetails_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
    {
      e.Cancel = true;
      e.NewWidth = lvDetails.Columns[e.ColumnIndex].Width;
    }

    private void lvDetails_DoubleClick(object sender, EventArgs e)
    {
      if (lvDetails.SelectedItems.Count > 0) {
        System.Diagnostics.Process.Start(
            lvDetails.SelectedItems[0].SubItems[0].Text);
      }
    }
    #endregion

    #region credential handling

    private void tsbtn_Help_Clicked(object sender, EventArgs e)
    {
      tsbtnHelp.BackColor = SystemColors.ControlLightLight;
      MessageBox.Show("These credentials are the 'content' parameters of the exhentai cookies. "
        + "If not provided, this program will use g.e-hentai instead.",
        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void txbxID_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) {
        tsbtnHelp.BackColor = Color.PaleVioletRed;
        e.Handled = true;
      }
      else {
        tsbtnHelp.BackColor = SystemColors.ControlLightLight;
      }
    }

    private void txbxID_TextChanged(object sender, EventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(txbxID.Text)) {
        int val;
        if (!int.TryParse(txbxID.Text, out val)) {
          txbxID.Text = string.Empty;
          MessageBox.Show("This value must be an integer. Please see the help button.", Application.ProductName,
            MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
      }
    }

    #endregion

    private void btnOK_Click(object sender, EventArgs e)
    {
      if (lvDetails.SelectedItems.Count > 0) {
        sChoice = lvDetails.SelectedItems[0].SubItems[0].Text;
        this.DialogResult = DialogResult.OK;
      }

      this.Close();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(txbxID.Text)) {
        SQL.UpdateSetting(SQL.Setting.member_id, txbxID.Text);
        SQL.UpdateSetting(SQL.Setting.pass_hash, txbxPass.Text);
      }

      ToggleButtonEnabled(ref btnSearch);
      ThreadPool.QueueUserWorkItem(Search, txbxSearch.Text);
      this.Cursor = Cursors.WaitCursor;
    }

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

    #endregion

    #region Methods

    /// <summary>
    /// Display the results of the search
    /// </summary>
    /// <param name="csResults">Object that holds the search details</param>
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

        lvDetails.Alternate();
        lvDetails.ResumeLayout();
      }

      this.Cursor = Cursors.Default;
      ToggleButtonEnabled(ref btnSearch);
      this.Text = "Search found " + csResults.Count + " possible matchess";
    }

    /// <summary>
    /// Starts a search using the text passed in
    /// </summary>
    /// <param name="obj">The string to search EH for</param>
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

    /// <summary>
    /// Toggles whether the passed in button is enabled. 
    /// Used to prevent returning an empty selection.
    /// </summary>
    /// <param name="btn">A reference to the desired button</param>
    /// <param name="bEnabled">An optional override to control button state</param>
    private void ToggleButtonEnabled(ref Button btn, bool? bEnabled = null)
    {
      btn.Enabled = (bEnabled == null) ?
        !btn.Enabled : (bool)bEnabled;
      btn.BackColor = btn.Enabled ?
        SystemColors.ButtonFace : SystemColors.ScrollBar;
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
