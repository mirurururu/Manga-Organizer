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
    const string sDefProgram = "System Default";
    string sIgnored = SQL.GetSetting(SQL.Setting.SearchIgnore);
    bool bSave = true;

    public Settings()
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;
    }

    private void Settings_Load(object sender, EventArgs e)
    {
      //initialize components
      aTxBx_Save.Text = SQL.GetSetting(SQL.Setting.SavePath);
      aTxBx_Root.Text = SQL.GetSetting(SQL.Setting.RootPath);
      aTxBx_Prog.Text = SQL.GetSetting(SQL.Setting.ImageBrowser);
      if (!string.IsNullOrEmpty(aTxBx_Prog.Text)) aTxBx_Prog.Text = sDefProgram;
      Nud_Intv.Value = Int32.Parse(SQL.GetSetting(SQL.Setting.ReadInterval));
      picBx_Colour.BackColor = Color.FromArgb(Int32.Parse(SQL.GetSetting(SQL.Setting.BackgroundColour)));
      ChkBx_Gridlines.Checked = SQL.GetSetting(SQL.Setting.ShowGrid) == "1";
      ChkBx_Date.Checked = SQL.GetSetting(SQL.Setting.ShowDate) == "1";
      ChkBx_Email.Checked = SQL.GetSetting(SQL.Setting.SendReports) == "1";

      string[] asIgn = ExtString.Split(SQL.GetSetting(SQL.Setting.SearchIgnore), "|");
      for (int i = 0; i < asIgn.Length; i++) {
        ckLbx_Ign.Items.Add(asIgn[i], true);
      }

      Btn_Save.FlatAppearance.BorderColor = Color.Green;
    }

    private void aTxBx_Save_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      string sPath = aTxBx_Save.Text != "" ?
        aTxBx_Save.Text : Environment.CurrentDirectory;
      fbd.SelectedPath = sPath;

      if (fbd.ShowDialog() == DialogResult.OK
          && ExtDir.Accessible(fbd.SelectedPath)) {
        Btn_Save.FlatAppearance.BorderColor = Color.Red;
        aTxBx_Save.Text = fbd.SelectedPath;
        bSave = false;

        if (aTxBx_Root.Text == string.Empty)
          aTxBx_Root.Text = fbd.SelectedPath;
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
          && ExtDir.Accessible(fbd.SelectedPath)) {
        Btn_Save.FlatAppearance.BorderColor = Color.Red;
        aTxBx_Root.Text = fbd.SelectedPath;
        bSave = false;

        if (aTxBx_Save.Text == string.Empty)
          aTxBx_Save.Text = fbd.SelectedPath;
      }
      fbd.Dispose();
    }

    private void aTxBx_Prog_Click(object sender, EventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
      string sPath = aTxBx_Prog.Text != sDefProgram ?
        aTxBx_Prog.Text : Environment.CurrentDirectory;
      ofd.Filter = "Executables (*.exe)|*.exe";
      ofd.InitialDirectory = sPath;

      if (ofd.ShowDialog() == DialogResult.OK) {
        Btn_Save.FlatAppearance.BorderColor = Color.Red;
        aTxBx_Prog.Text = ofd.FileName;
        bSave = false;
      }
      ofd.Dispose();
    }

    private void TxBx_KeyPress(object sender, KeyPressEventArgs e)
    {
      e.Handled = true;
    }

    private void Nud_Intv_ValueChanged(object sender, EventArgs e)
    {
      Btn_Save.FlatAppearance.BorderColor = Color.Red;
      bSave = false;
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
        bSave = false;
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
      if (sIgn.Length > 1) {
        sIgn.Remove(sIgn.Length - 1, 1);
      }
      if (sIgnored != sIgn.ToString()) {
        Btn_Save.FlatAppearance.BorderColor = Color.Red;
        sIgnored = sIgn.ToString();
        bSave = false;
      }
    }

    private void ChkBx_Defaults_CheckedChanged(object sender, EventArgs e)
    {
      Btn_Save.FlatAppearance.BorderColor = Color.Red;
      bSave = false;
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
          aTxBx_Prog.Text = sDefProgram;
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
      bSave = false;
    }

    private void Btn_Save_Click(object sender, EventArgs e)
    {
      //only finalize settings when `Btn_Save_Click' triggered
      if (aTxBx_Prog.Text != sDefProgram) {
        SQL.UpdateSetting("ImageBrowser", aTxBx_Prog.Text);
        Properties.Settings.Default.DefLoc = aTxBx_Prog.Text;
        Properties.Settings.Default.Save();
      }
      SQL.UpdateSetting("SavePath", aTxBx_Save.Text);
      SQL.UpdateSetting("RootPath", aTxBx_Root.Text);
      SQL.UpdateSetting("ReadInterval", (int)Nud_Intv.Value);
      SQL.UpdateSetting("BackgroundColour", picBx_Colour.BackColor.ToArgb());
      SQL.UpdateSetting("ShowGrid", ChkBx_Gridlines.Checked ? 1 : 0);
      SQL.UpdateSetting("ShowDate", ChkBx_Date.Checked ? 1 : 0);
      SQL.UpdateSetting("SendReports", ChkBx_Email.Checked ? 1 : 0);
      SQL.UpdateSetting("SearchIgnored", sIgnored);
      bSave = true;
      this.Close();
    }

    private void Settings_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!bSave && MessageBox.Show(
          "Are you sure you want to exit wthout saving?",
          Application.ProductName, MessageBoxButtons.YesNo,
          MessageBoxIcon.Question) == DialogResult.No) {
        e.Cancel = true;
      }
      else {
        this.DialogResult = DialogResult.Yes;
      }
    }
  }
}
