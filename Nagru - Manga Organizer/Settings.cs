using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
  public partial class Settings : Form
  {
    #region Properties
    const string sDefProgram = "System Default";
    csSettings stCurrent;
    #endregion

    #region Settings Handler
    private class csSettings
    {
      #region Properties
      public Color BackColour;
      public int ReadInterval;
      public string[] SearchIgnore;
      public string SearchIgnoreRaw;
      public string CustomProgram;
      public string SaveLoc;
      public string RootLoc;
      public bool ShowGrid;
      public bool ShowDate;

      #region Track value changes
      public bool bBackColourChanged = false;
      public bool bReadIntervalChanged = false;
      public bool bSearchIgnoreChanged = false;
      public bool bDefaultProgramChanged = false;
      public bool bSaveLocChanged = false;
      public bool bRootLocChanged = false;
      public bool bShowGridChanged = false;
      public bool bShowDateChanged = false;
      #endregion

      #endregion

      #region Constructor

      public csSettings()
      {
        SaveLoc = Properties.Settings.Default.SavLoc != string.Empty ?
          Properties.Settings.Default.SavLoc : Environment.CurrentDirectory;
        RootLoc = ((string)SQL.GetSetting(SQL.Setting.RootPath));
        CustomProgram = ((string)SQL.GetSetting(SQL.Setting.ImageBrowser));
        if (string.IsNullOrWhiteSpace(CustomProgram)) {
          CustomProgram = sDefProgram;
        }
        ReadInterval = ((int)SQL.GetSetting(SQL.Setting.ReadInterval));
        BackColour = ((Color)SQL.GetSetting(SQL.Setting.BackgroundColour));
        ShowGrid = ((bool)SQL.GetSetting(SQL.Setting.ShowGrid));
        ShowDate = ((bool)SQL.GetSetting(SQL.Setting.ShowDate));
        SearchIgnoreRaw = ((string)SQL.GetSetting(SQL.Setting.SearchIgnore));
        SearchIgnore = Ext.Split(SearchIgnoreRaw, "|");
      }

      #endregion

      #region Setting Changed

      public bool HasSettingChanged()
      {
        return (bBackColourChanged
          || bReadIntervalChanged
          || bSearchIgnoreChanged
          || bDefaultProgramChanged
          || bSaveLocChanged
          || bRootLocChanged
          || bShowGridChanged
          || bShowDateChanged
         );
      }

      public bool BackColourChanged(Color cNew)
      {
        return bBackColourChanged = 
          BackColour != cNew;
      }

      public bool ReadIntervalChanged(int iNew)
      {
        return bReadIntervalChanged
          = ReadInterval != iNew;
      }

      public bool SearchIgnoreChanged(string sNew)
      {
        return bSearchIgnoreChanged
          = (SearchIgnoreRaw.Length != sNew.Length
              || SearchIgnoreRaw != sNew);
      }

      public bool DefaultProgramChanged(string sNew)
      {
        return bDefaultProgramChanged
          = (CustomProgram != sNew);
      }

      public bool SaveLocChanged(string sNew)
      {
        return bSaveLocChanged
          = (SaveLoc.Length != sNew.Length
              || SaveLoc != sNew);
      }

      public bool RootLocChanged(string sNew)
      {
        return bRootLocChanged
          = (RootLoc.Length != sNew.Length
              || RootLoc != sNew);
      }

      public bool ShowGridChanged(bool bNew)
      {
        return bShowGridChanged
          = ShowGrid != bNew;
      }

      public bool ShowDateChanged(bool bNew)
      {
        return bShowDateChanged
          = ShowDate != bNew;
      }

      #endregion
    }
    #endregion

    #region Events

    #region Main Events

    public Settings()
    {
      InitializeComponent();
      this.Icon = Properties.Resources.dbIcon;
    }

    private void Settings_Load(object sender, EventArgs e)
    {
      stCurrent = new csSettings();

      //initialize components
      aTxBx_Save.Text = stCurrent.SaveLoc;
      aTxBx_Root.Text = stCurrent.RootLoc;
      CmbBx_ImageViewer.Text = stCurrent.CustomProgram;
      Nud_Intv.Value = stCurrent.ReadInterval;
      picBx_Colour.BackColor = stCurrent.BackColour;
      ChkBx_Gridlines.Checked = stCurrent.ShowGrid;
      ChkBx_Date.Checked = stCurrent.ShowDate;

      ckLbx_Ign.SuspendLayout();
      for (int i = 0; i < stCurrent.SearchIgnore.Length; i++) {
        ckLbx_Ign.Items.Add(stCurrent.SearchIgnore[i], true);
      }
      ckLbx_Ign.ResumeLayout();

      Btn_Save.FlatAppearance.BorderColor = Color.Green;
    }

    private void Settings_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (stCurrent.HasSettingChanged()) {
				if(MessageBox.Show("Are you sure you want to exit without saving?",
						Application.ProductName, MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.No) {
					e.Cancel = true;
				}
      }
      else {
        this.DialogResult = DialogResult.Yes;
      }
    }

    #endregion

    #region Changed Property

    private void btnImageViewer_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog ofd = new OpenFileDialog()) {
        string sPath = CmbBx_ImageViewer.SelectedIndex == -1 ?
          CmbBx_ImageViewer.Text : Environment.CurrentDirectory;
        ofd.Filter = "Executables (*.exe)|*.exe";
        ofd.InitialDirectory = sPath;

        if (ofd.ShowDialog() == DialogResult.OK) {
          CmbBx_ImageViewer.Text = ofd.FileName;
          stCurrent.DefaultProgramChanged(ofd.FileName);
          SettingsChanged();
        }
      }
    }

    private void CmbBx_ImageViewer_SelectedIndexChanged(object sender, EventArgs e)
    {
      stCurrent.DefaultProgramChanged(CmbBx_ImageViewer.Text);
      SettingsChanged();
    }

    private void aTxBx_Root_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog fbd = new FolderBrowserDialog()) {
        string sPath = aTxBx_Root.Text != "" ?
          aTxBx_Root.Text : Environment.CurrentDirectory;
        fbd.SelectedPath = sPath;

        if (fbd.ShowDialog() == DialogResult.OK
            && Ext.Accessible(fbd.SelectedPath) != Ext.PathType.Invalid) {
          aTxBx_Root.Text = fbd.SelectedPath;
          if (aTxBx_Save.Text == string.Empty) {
            aTxBx_Save.Text = fbd.SelectedPath;
          }
          stCurrent.RootLocChanged(fbd.SelectedPath);
          SettingsChanged();
        }
      }
    }

    private void aTxBx_Save_Click(object sender, EventArgs e)
    {
      using(FolderBrowserDialog fbd = new FolderBrowserDialog()) {
        string sPath = !string.IsNullOrWhiteSpace(aTxBx_Save.Text) ?
          aTxBx_Save.Text : Environment.CurrentDirectory;
        fbd.SelectedPath = sPath;

        if (fbd.ShowDialog() == DialogResult.OK
            && Ext.Accessible(fbd.SelectedPath) != Ext.PathType.Invalid) {
          aTxBx_Save.Text = fbd.SelectedPath;
          if (string.IsNullOrWhiteSpace(aTxBx_Root.Text)) {
            aTxBx_Root.Text = fbd.SelectedPath;
          }
          stCurrent.SaveLocChanged(fbd.SelectedPath);
          SettingsChanged();
        }
      }
    }

    private void ChkBx_Defaults_CheckedChanged(object sender, EventArgs e)
    {
      stCurrent.ShowDateChanged(ChkBx_Date.Checked);
      stCurrent.ShowGridChanged(ChkBx_Gridlines.Checked);
      SettingsChanged();
    }

    private void ckLbx_Ign_SelectedIndexChanged(object sender, EventArgs e)
    {
      stCurrent.SearchIgnoreChanged(GetIgnoredPaths());
      SettingsChanged();
    }

    private void Nud_Intv_ValueChanged(object sender, EventArgs e)
    {
      stCurrent.ReadIntervalChanged(Convert.ToInt32(Nud_Intv.Value));
      SettingsChanged();
    }

    private void picBx_Colour_Click(object sender, EventArgs e)
    {
      using (ColorDialog cd = new ColorDialog()) {
        cd.CustomColors = new int[2] {
          ColorTranslator.ToOle(Color.FromArgb(39,40,34)),
          ColorTranslator.ToOle(picBx_Colour.BackColor)
        };
        cd.Color = picBx_Colour.BackColor;

        if (cd.ShowDialog() == DialogResult.OK) {
          picBx_Colour.BackColor = cd.Color;
          stCurrent.BackColourChanged(cd.Color);
          SettingsChanged();
        }
      }
    }

    #endregion

    #region Other

    private void Btn_Save_Click(object sender, EventArgs e)
    {
      //only finalize settings when `Btn_Save_Click' triggered
      if (stCurrent.bSaveLocChanged) {
        Properties.Settings.Default.SavLoc = aTxBx_Save.Text;
        Properties.Settings.Default.Save();
        stCurrent.bSaveLocChanged = false;
      }
      if (stCurrent.bDefaultProgramChanged) {
        SQL.UpdateSetting(SQL.Setting.ImageBrowser, CmbBx_ImageViewer.Text);
        stCurrent.bDefaultProgramChanged = false;
      }
      if (stCurrent.bRootLocChanged) {
        SQL.UpdateSetting(SQL.Setting.RootPath, aTxBx_Root.Text);
        stCurrent.bRootLocChanged = false;
      }
      if (stCurrent.bReadIntervalChanged) {
        SQL.UpdateSetting(SQL.Setting.ReadInterval, (int)Nud_Intv.Value);
        stCurrent.bReadIntervalChanged = false;
      }
      if (stCurrent.bBackColourChanged) {
        SQL.UpdateSetting(SQL.Setting.BackgroundColour, picBx_Colour.BackColor.ToArgb());
        stCurrent.bBackColourChanged = false;
      }
      if (stCurrent.bShowGridChanged) {
        SQL.UpdateSetting(SQL.Setting.ShowGrid, ChkBx_Gridlines.Checked ? 1 : 0);
        stCurrent.bShowGridChanged = false;
      }
      if (stCurrent.bShowDateChanged) {
        SQL.UpdateSetting(SQL.Setting.ShowDate, ChkBx_Date.Checked ? 1 : 0);
        stCurrent.bShowDateChanged = false;
      }
      if (stCurrent.bSearchIgnoreChanged) {
        SQL.UpdateSetting(SQL.Setting.SearchIgnore, GetIgnoredPaths());
        stCurrent.bSearchIgnoreChanged = false;
      }
      this.Close();
    }

    private void MnAct_Reset_Click(object sender, EventArgs e)
    {
      switch (MnAct.SourceControl.Name) {
        case "aTxBx_Save":
          aTxBx_Save.Text = stCurrent.SaveLoc;
          stCurrent.bSaveLocChanged = false;
          break;
        case "aTxBx_Root":
          aTxBx_Root.Text = stCurrent.RootLoc;
          stCurrent.bRootLocChanged = false;
          break;
        case "aTxBx_Prog":
          CmbBx_ImageViewer.Text = stCurrent.CustomProgram;
          stCurrent.bDefaultProgramChanged = false;
          break;
        case "Nud_Intv":
          Nud_Intv.Value = stCurrent.ReadInterval;
          stCurrent.bReadIntervalChanged = false;
          break;
        case "picBx_Colour":
          picBx_Colour.BackColor = stCurrent.BackColour;
          stCurrent.bBackColourChanged = false;
          break;
        case "ckLbx_Ign":
          ckLbx_Ign.Items.Clear();
          for (int i = 0; i < stCurrent.SearchIgnore.Length; i++) {
            ckLbx_Ign.Items.Add(stCurrent.SearchIgnore[i], true);
          }
          stCurrent.bSearchIgnoreChanged = false;
          break;
        case "ChkBx_Date":
          ChkBx_Date.Checked = stCurrent.ShowDate;
          stCurrent.bShowDateChanged = false;
          break;
        case "ChkBx_Gridlines":
          ChkBx_Gridlines.Checked = stCurrent.ShowGrid;
          stCurrent.bShowGridChanged = false;
          break;
      }
      SettingsChanged();
    }

    private void TxBx_KeyPress(object sender, KeyPressEventArgs e)
    {
      e.Handled = true;
    }

    #endregion

    #endregion

    #region Methods

    private string GetIgnoredPaths()
    {
      StringBuilder sIgn = new StringBuilder("");
      foreach (string svar in ckLbx_Ign.CheckedItems) {
        sIgn.AppendFormat(string.Format("{0}|", svar));
      }
      if (sIgn.Length > 1) {
        sIgn.Remove(sIgn.Length - 1, 1);
      }

      return sIgn.ToString();
    }

    private void SettingsChanged()
    {
      Btn_Save.FlatAppearance.BorderColor = 
        stCurrent.HasSettingChanged() ? Color.Red : Color.Green;
    }

    #endregion
  }
}
