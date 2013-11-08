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
        const string sDefProg = "System Default";
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
            aTxBx_Save.Text = Properties.Settings.Default.SavLoc;
            aTxBx_Root.Text = Properties.Settings.Default.DefLoc;
            aTxBx_Prog.Text = Properties.Settings.Default.DefProg;
            if (aTxBx_Prog.Text == "") aTxBx_Prog.Text = sDefProg;
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
        }
        
        private void aTxBx_Save_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            string sPath = aTxBx_Save.Text != "" ?
                aTxBx_Save.Text : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK 
                    && !ExtDir.Restricted(fbd.SelectedPath)) {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                aTxBx_Save.Text = fbd.SelectedPath;
                bNew = true;
            }
            fbd.Dispose();
        }

        private void aTxBx_Root_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            string sPath = aTxBx_Root.Text != "" ?
                aTxBx_Root.Text : Environment.CurrentDirectory;
            fbd.SelectedPath = sPath;

            if (fbd.ShowDialog() == DialogResult.OK
                    && !ExtDir.Restricted(fbd.SelectedPath)) {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                aTxBx_Root.Text = fbd.SelectedPath;
                bNew = true;
            }
            fbd.Dispose();
        }

        private void aTxBx_Prog_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string sPath = aTxBx_Prog.Text != sDefProg ?
                aTxBx_Prog.Text : Environment.CurrentDirectory;
            ofd.Filter = "Executables (*.exe)|*.exe";
            ofd.InitialDirectory = sPath;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Btn_Save.FlatAppearance.BorderColor = Color.Red;
                aTxBx_Prog.Text = ofd.FileName;
                bNew = true;
            }
            ofd.Dispose();
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
        
        private void MnAct_Reset_Click(object sender, EventArgs e)
        {
            switch (MnAct.SourceControl.Name) {
                case "aTxBx_Save":
                    aTxBx_Save.Text = Environment.CurrentDirectory;
                    break;
                case "aTxBx_Root":
                    aTxBx_Root.Text = Environment.CurrentDirectory;
                    break;
                case "aTxBx_Prog":
                    aTxBx_Prog.Text = sDefProg;
                    break;
                case "Nud_Intv":
                    Nud_Intv.Value = 20000;
                    break;
                case "picBx_Colour":
                    picBx_Colour.BackColor = System.Drawing.Color.FromArgb(39, 40, 34);
                    break;
                case "ckLbx_Ign":
                    ckLbx_Ign.Items.Clear();
                    break;
                case "ChkBx_Date":
                    ChkBx_Date.Checked = true;
                    break;
                case "ChkBx_Gridlines":
                    ChkBx_Gridlines.Checked = false;
                    break;
            }

            Btn_Save.FlatAppearance.BorderColor = Color.Red;
            bNew = true;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            //only finalize settings when `Btn_Save_Click' triggered
            if (aTxBx_Prog.Text != sDefProg)
                Properties.Settings.Default.DefProg = aTxBx_Prog.Text;
            else Properties.Settings.Default.DefProg = "";
            Properties.Settings.Default.SavLoc = aTxBx_Save.Text;
            Properties.Settings.Default.DefLoc = aTxBx_Root.Text;
            Properties.Settings.Default.Interval = (int)Nud_Intv.Value;
            Properties.Settings.Default.DefColour = picBx_Colour.BackColor;
            Properties.Settings.Default.DefGrid = ChkBx_Gridlines.Checked;
            Properties.Settings.Default.HideDate = !ChkBx_Date.Checked;
            Properties.Settings.Default.Ignore = sIgnored;
            Properties.Settings.Default.Save();
            bNew = false; bSave = true;
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
