using System;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class GetUrl : Form
    {
        Uri uri;
        public string Url {
            get { return uri.AbsoluteUri; }
        }

        public GetUrl()
        {
            InitializeComponent();
            
            if(Clipboard.GetText().Contains("hentai.org/g/"))
                TxBx_Url.Text = Clipboard.GetText();
            else TxBx_Url.SelectAll();
        }

        private void Btn_Get_Click(object sender, EventArgs e)
        {
            if (TxBx_Url.Text != string.Empty &&
                TxBx_Url.Text != "Input EH gallery page URL...") {
                TestText();
            }
        }

        private void TestText()
        {
            string sUrl = TxBx_Url.Text;
            if (!sUrl.StartsWith("http://"))
                sUrl = sUrl.Insert(0, "http://");

            //ensure user only enters proper values
            if ((sUrl.StartsWith("http://g.e-hentai.org/g/") || sUrl.StartsWith("http://exhentai.org/g/")) &&
                Uri.TryCreate(sUrl, UriKind.Absolute, out uri)) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("URL was invalid. Please make sure it comes from an EH gallery page.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* remove initial message on focus */
        private void TxBx_Url_Click(object sender, EventArgs e)
        {
            if (TxBx_Url.Text == "Input EH gallery page URL...")
                TxBx_Url.Clear();
        }

        #region Menu_Text
        private void MnTx_Undo_Click(object sender, EventArgs e)
        { if (TxBx_Url.CanUndo) TxBx_Url.Undo(); }

        private void MnTx_Cut_Click(object sender, EventArgs e)
        { TxBx_Url.Cut(); }

        private void MnTx_Copy_Click(object sender, EventArgs e)
        { TxBx_Url.Copy(); }

        private void MnTx_Paste_Click(object sender, EventArgs e)
        { TxBx_Url.Paste(); }

        private void MnTx_SelAll_Click(object sender, EventArgs e)
        { TxBx_Url.SelectAll(); }
        #endregion
    }
}
