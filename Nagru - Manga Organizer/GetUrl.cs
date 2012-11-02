using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    public partial class GetUrl : Form
    {
        Uri uri;
        public string Url
        { get { return uri.AbsoluteUri; } }

        public GetUrl()
        { InitializeComponent(); }

        private void Btn_Get_Click(object sender, EventArgs e)
        {
            //ensure user only enters proper values
            if (TxBx_Url.Text != string.Empty && 
                Uri.TryCreate(TxBx_Url.Text, UriKind.Absolute, out uri) &&
                Url.StartsWith(@"http://g.e-hentai.org/g/"))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("URL was invalid. Please make sure it comes from an g.e-hentai gallery page.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region Menu_Text
        private void MnTx_Undo_Click(object sender, EventArgs e)
        { if(TxBx_Url.CanUndo) TxBx_Url.Undo(); }

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
