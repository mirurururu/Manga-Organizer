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
    public partial class Settings : Form
    {
        string sIgnored = Properties.Settings.Default.Ignore;
        bool bNew, bSave = false;

        public Settings()
        { 
            InitializeComponent();
            this.Icon = Properties.Resources.dbIcon;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            //initialize components
            TxBx_Save.Text = Properties.Settings.Default.SavLoc;
            TxBx_Root.Text = Properties.Settings.Default.DefLoc;
            Nud_Intv.Value = Properties.Settings.Default.Interval;
            picBx_Colour.BackColor = Properties.Settings.Default.DefColour;
            ChkBx_Gridlines.Checked = Properties.Settings.Default.DefGrid;
            ChkBx_Date.Checked = !Properties.Settings.Default.HideDate;
            string[] asIgn = Properties.Settings.Default.Ignore.Split('|');
            for (int i = 0; i < asIgn.Length; i++) {
                if (asIgn[i] == "") continue;
                ckLbx_Ign.Items.Add(asIgn[i], true);
            }
            Btn_Save.FlatAppearance.BorderColor = Color.Green;
            bNew = false;

            //Disable context menus and setup hScrollbars
            TxBx_Save.ContextMenu = new ContextMenu();
            TxBx_Root.ContextMenu = new ContextMenu();
            SetScroll(TxBx_Save, ScrSave);
            SetScroll(TxBx_Root, ScrRoot);
        }

        private void TxBx_Save_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            string sPath = Properties.Settings.Default.SavLoc != "" ?
                Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK 
                    && !ExtDir.Restricted(fbd.SelectedPath)) {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                TxBx_Save.Text = fbd.SelectedPath;
                SetScroll(TxBx_Save, ScrSave);
                bNew = true;
            }
            fbd.Dispose();
        }
        private void Scr_Save_Scroll(object sender, ScrollEventArgs e)
        {
            TxBx_Save.Select(ScrSave.Value, 0);
            TxBx_Save.ScrollToCaret();
        }

        private void TxBx_Root_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            string sPath = Properties.Settings.Default.DefLoc != "" ?
                Properties.Settings.Default.DefLoc : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK
                    && !ExtDir.Restricted(fbd.SelectedPath)) {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                TxBx_Root.Text = fbd.SelectedPath;
                SetScroll(TxBx_Root, ScrRoot);
                bNew = true;
            }
            fbd.Dispose();
        }
        private void ScrRoot_Scroll(object sender, ScrollEventArgs e)
        {
            TxBx_Root.Select(ScrRoot.Value, 0);
            TxBx_Root.ScrollToCaret();
        }

        private void SetScroll(TextBox cnt, HScrollBar hs)
        {
            int iWidth = TextRenderer.MeasureText(cnt.Text, cnt.Font).Width;
            if (iWidth > cnt.Width) {
                hs.Maximum = iWidth / 5;
                hs.Value = cnt.SelectionStart;
                hs.Visible = true;
            }
            else hs.Visible = false;
        }

        private void TxBx_KeyPress(object sender, KeyPressEventArgs e)
        { e.Handled = true; }

        private void Nud_Intv_ValueChanged(object sender, EventArgs e)
        {
            Btn_Save.FlatAppearance.BorderColor = Color.Red;
            bNew = true;
        }

        private void picBx_Colour_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.CustomColors = new int[2] {
                ColorTranslator.ToOle(Color.FromArgb(39,40,34)),
                ColorTranslator.ToOle(picBx_Colour.BackColor)
            };
            cd.Color = picBx_Colour.BackColor;

            if (cd.ShowDialog() == DialogResult.OK) {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                picBx_Colour.BackColor = cd.Color;
                bNew = true;
            }
        }
        
        private void ckLbx_Ign_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckListbox();
        }
        private void CheckListbox()
        {
            if (ckLbx_Ign.CheckedItems.Count == ckLbx_Ign.Items.Count)
                return;

            StringBuilder sIgn = new StringBuilder("");
            foreach (string svar in ckLbx_Ign.CheckedItems) {
                sIgn.AppendFormat(string.Format("{0}|", svar));
            }
            if (sIgn.Length > 1)
                sIgn.Remove(sIgn.Length - 1, 1);
            if(sIgnored != sIgn.ToString()) {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                sIgnored = sIgn.ToString();
                bNew = true;
            }
        }

        private void ChkBx_Defaults_CheckedChanged(object sender, EventArgs e)
        {
            Btn_Save.FlatAppearance.BorderColor = Color.Red;
            bNew = true;
        }

        private void ChkBx_Date_CheckedChanged(object sender, EventArgs e)
        {
            Btn_Save.FlatAppearance.BorderColor = Color.Red;
            bNew = true;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            //only finalize settings when `Btn_Save_Click' triggered
            Properties.Settings.Default.SavLoc = TxBx_Save.Text;
            Properties.Settings.Default.DefLoc = TxBx_Root.Text;
            Properties.Settings.Default.Interval = (int)Nud_Intv.Value;
            Properties.Settings.Default.DefColour = picBx_Colour.BackColor;
            Properties.Settings.Default.DefGrid = ChkBx_Gridlines.Checked;
            Properties.Settings.Default.HideDate = !ChkBx_Date.Checked;
            Properties.Settings.Default.Ignore = sIgnored;
            Properties.Settings.Default.Save();
            bNew = false; bSave = true;
            Btn_Save.FlatAppearance.BorderColor = Color.Green;
            this.Close();
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(bNew && MessageBox.Show(
                    "Are you sure you want to exit wthout saving?",
                    Application.ProductName, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No) {
                e.Cancel = true;
            }
            else if(bSave)
                this.DialogResult = DialogResult.Yes;
        }
    }
}
