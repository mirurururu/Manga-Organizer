using System;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  public partial class GetUrl : Form
  {
    #region Properties

    private Uri uri;

    /// <summary>
    /// The address of the EH gallery
    /// </summary>
    public string Url
    {
      get
      {
        return uri.AbsoluteUri;
      }
    }

    #endregion

    #region Events

    /// <summary>
    /// Constructor of the page
    /// </summary>
    public GetUrl()
    {
      InitializeComponent();

      if (Ext.Contains(Clipboard.GetText(), "hentai.org/g/")){
        TxBx_Url.Text = Clipboard.GetText();
      }
      else {
        TxBx_Url.SelectAll();
      }
    }

    /// <summary>
    /// Clean the URL, test it's valid, then send to Main
    /// </summary>
    private void Btn_Get_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(TxBx_Url.Text) &&
          TxBx_Url.Text != "Input EH gallery page URL...") {
        if (TestText()) {
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
        else {
          MessageBox.Show("URL was invalid. Please make sure it comes from an EH gallery page.",
            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    /// <summary>
    /// Remove initial message on focus
    /// </summary>
    private void TxBx_Url_Click(object sender, EventArgs e)
    {
      if (TxBx_Url.Text == "Input EH gallery page URL...")
        TxBx_Url.Clear();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove any query string parameters
    /// </summary>
    /// <param name="sRaw">The raw URL</param>
    /// <returns>The stripped URL</returns>
    private static string RemovePage(string sRaw)
    {
      int iPos = sRaw.LastIndexOf('?');
      if (iPos != -1) {
        return sRaw.Substring(0, iPos);
      }
      else {
        return sRaw;
      }
    }

    /// <summary>
    /// Tests if the URL is a valid
    /// </summary>
    private bool TestText()
    {
      bool bValid = false;
      string sUrl = RemovePage(TxBx_Url.Text);
      if (!sUrl.StartsWith("http://"))
        sUrl = sUrl.Insert(0, "http://");

      //ensure user only enters proper values
      if ((sUrl.StartsWith("http://g.e-hentai.org/g/")
            || sUrl.StartsWith("http://exhentai.org/g/"))
           && Uri.TryCreate(sUrl, UriKind.Absolute, out uri)) {
        bValid = true;
      }

      return bValid;
    }

    #endregion

    #region Menu_Text
    private void Mn_TxBx_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      TxBx_Url.Select();
    }

    private void MnTx_Undo_Click(object sender, EventArgs e)
    {
      if (TxBx_Url.CanUndo)
        TxBx_Url.Undo();
    }

    private void MnTx_Cut_Click(object sender, EventArgs e)
    {
      TxBx_Url.Cut();
    }

    private void MnTx_Copy_Click(object sender, EventArgs e)
    {
      TxBx_Url.Copy();
    }

    private void MnTx_Paste_Click(object sender, EventArgs e)
    {
      TxBx_Url.Paste();
    }

    private void MnTx_Delete_Click(object sender, EventArgs e)
    {
      TxBx_Url.SelectedText = "";
    }

    private void MnTx_SelAll_Click(object sender, EventArgs e)
    {
      TxBx_Url.SelectAll();
    }
    #endregion
  }
}
