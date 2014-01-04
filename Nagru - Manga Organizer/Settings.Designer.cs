namespace Nagru___Manga_Organizer
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Nud_Intv = new System.Windows.Forms.NumericUpDown();
            this.MnAct = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnAct_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.picBx_Colour = new System.Windows.Forms.PictureBox();
            this.lblColour = new System.Windows.Forms.Label();
            this.lblSave = new System.Windows.Forms.Label();
            this.lblRoot = new System.Windows.Forms.Label();
            this.lblIntv = new System.Windows.Forms.Label();
            this.lblIgnored = new System.Windows.Forms.Label();
            this.grBxToggle = new System.Windows.Forms.GroupBox();
            this.ChkBx_Date = new System.Windows.Forms.CheckBox();
            this.ChkBx_Gridlines = new System.Windows.Forms.CheckBox();
            this.ckLbx_Ign = new System.Windows.Forms.CheckedListBox();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.lblProg = new System.Windows.Forms.Label();
            this.aTxBx_Save = new Nagru___Manga_Organizer.Classes.AutoCompleteTagger();
            this.aTxBx_Prog = new Nagru___Manga_Organizer.Classes.AutoCompleteTagger();
            this.aTxBx_Root = new Nagru___Manga_Organizer.Classes.AutoCompleteTagger();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Intv)).BeginInit();
            this.MnAct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Colour)).BeginInit();
            this.grBxToggle.SuspendLayout();
            this.SuspendLayout();
            // 
            // Nud_Intv
            // 
            this.Nud_Intv.ContextMenuStrip = this.MnAct;
            this.Nud_Intv.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.Nud_Intv.Location = new System.Drawing.Point(12, 145);
            this.Nud_Intv.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.Nud_Intv.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Nud_Intv.Name = "Nud_Intv";
            this.Nud_Intv.Size = new System.Drawing.Size(146, 20);
            this.Nud_Intv.TabIndex = 1;
            this.Nud_Intv.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Nud_Intv.ValueChanged += new System.EventHandler(this.Nud_Intv_ValueChanged);
            // 
            // MnAct
            // 
            this.MnAct.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnAct_Reset});
            this.MnAct.Name = "Mn_Context";
            this.MnAct.ShowImageMargin = false;
            this.MnAct.Size = new System.Drawing.Size(71, 26);
            // 
            // MnAct_Reset
            // 
            this.MnAct_Reset.Name = "MnAct_Reset";
            this.MnAct_Reset.ShowShortcutKeys = false;
            this.MnAct_Reset.Size = new System.Drawing.Size(70, 22);
            this.MnAct_Reset.Text = "Reset";
            this.MnAct_Reset.Click += new System.EventHandler(this.MnAct_Reset_Click);
            // 
            // picBx_Colour
            // 
            this.picBx_Colour.ContextMenuStrip = this.MnAct;
            this.picBx_Colour.Location = new System.Drawing.Point(179, 145);
            this.picBx_Colour.Name = "picBx_Colour";
            this.picBx_Colour.Size = new System.Drawing.Size(139, 20);
            this.picBx_Colour.TabIndex = 3;
            this.picBx_Colour.TabStop = false;
            this.picBx_Colour.Click += new System.EventHandler(this.picBx_Colour_Click);
            // 
            // lblColour
            // 
            this.lblColour.AutoSize = true;
            this.lblColour.Location = new System.Drawing.Point(217, 129);
            this.lblColour.Name = "lblColour";
            this.lblColour.Size = new System.Drawing.Size(101, 13);
            this.lblColour.TabIndex = 4;
            this.lblColour.Text = "Background Colour:";
            // 
            // lblSave
            // 
            this.lblSave.AutoSize = true;
            this.lblSave.Location = new System.Drawing.Point(9, 9);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(75, 13);
            this.lblSave.TabIndex = 5;
            this.lblSave.Text = "Save location:";
            // 
            // lblRoot
            // 
            this.lblRoot.AutoSize = true;
            this.lblRoot.Location = new System.Drawing.Point(9, 48);
            this.lblRoot.Name = "lblRoot";
            this.lblRoot.Size = new System.Drawing.Size(62, 13);
            this.lblRoot.TabIndex = 7;
            this.lblRoot.Text = "Root folder:";
            // 
            // lblIntv
            // 
            this.lblIntv.AutoSize = true;
            this.lblIntv.Location = new System.Drawing.Point(9, 129);
            this.lblIntv.Name = "lblIntv";
            this.lblIntv.Size = new System.Drawing.Size(128, 13);
            this.lblIntv.TabIndex = 12;
            this.lblIntv.Text = "Auto-browse interval (ms):";
            // 
            // lblIgnored
            // 
            this.lblIgnored.AutoSize = true;
            this.lblIgnored.Location = new System.Drawing.Point(9, 180);
            this.lblIgnored.Name = "lblIgnored";
            this.lblIgnored.Size = new System.Drawing.Size(81, 13);
            this.lblIgnored.TabIndex = 14;
            this.lblIgnored.Text = "Ignored manga:";
            // 
            // grBxToggle
            // 
            this.grBxToggle.Controls.Add(this.ChkBx_Date);
            this.grBxToggle.Controls.Add(this.ChkBx_Gridlines);
            this.grBxToggle.Location = new System.Drawing.Point(179, 196);
            this.grBxToggle.Name = "grBxToggle";
            this.grBxToggle.Size = new System.Drawing.Size(139, 84);
            this.grBxToggle.TabIndex = 15;
            this.grBxToggle.TabStop = false;
            this.grBxToggle.Text = "Defaults";
            // 
            // ChkBx_Date
            // 
            this.ChkBx_Date.AutoSize = true;
            this.ChkBx_Date.ContextMenuStrip = this.MnAct;
            this.ChkBx_Date.Location = new System.Drawing.Point(15, 51);
            this.ChkBx_Date.Name = "ChkBx_Date";
            this.ChkBx_Date.Size = new System.Drawing.Size(122, 17);
            this.ChkBx_Date.TabIndex = 2;
            this.ChkBx_Date.Text = "Show Date column?";
            this.ChkBx_Date.UseVisualStyleBackColor = true;
            this.ChkBx_Date.CheckedChanged += new System.EventHandler(this.ChkBx_Defaults_CheckedChanged);
            // 
            // ChkBx_Gridlines
            // 
            this.ChkBx_Gridlines.AutoSize = true;
            this.ChkBx_Gridlines.ContextMenuStrip = this.MnAct;
            this.ChkBx_Gridlines.Location = new System.Drawing.Point(15, 28);
            this.ChkBx_Gridlines.Name = "ChkBx_Gridlines";
            this.ChkBx_Gridlines.Size = new System.Drawing.Size(101, 17);
            this.ChkBx_Gridlines.TabIndex = 1;
            this.ChkBx_Gridlines.Text = "Draw grid lines?";
            this.ChkBx_Gridlines.UseVisualStyleBackColor = true;
            this.ChkBx_Gridlines.CheckedChanged += new System.EventHandler(this.ChkBx_Defaults_CheckedChanged);
            // 
            // ckLbx_Ign
            // 
            this.ckLbx_Ign.CheckOnClick = true;
            this.ckLbx_Ign.ContextMenuStrip = this.MnAct;
            this.ckLbx_Ign.FormattingEnabled = true;
            this.ckLbx_Ign.Location = new System.Drawing.Point(12, 196);
            this.ckLbx_Ign.Name = "ckLbx_Ign";
            this.ckLbx_Ign.Size = new System.Drawing.Size(146, 154);
            this.ckLbx_Ign.TabIndex = 16;
            this.ckLbx_Ign.SelectedIndexChanged += new System.EventHandler(this.ckLbx_Ign_SelectedIndexChanged);
            // 
            // Btn_Save
            // 
            this.Btn_Save.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Save.Location = new System.Drawing.Point(204, 306);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(91, 23);
            this.Btn_Save.TabIndex = 17;
            this.Btn_Save.Text = "Save Settings";
            this.Btn_Save.UseVisualStyleBackColor = false;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // lblProg
            // 
            this.lblProg.AutoSize = true;
            this.lblProg.Location = new System.Drawing.Point(9, 88);
            this.lblProg.Name = "lblProg";
            this.lblProg.Size = new System.Drawing.Size(73, 13);
            this.lblProg.TabIndex = 20;
            this.lblProg.Text = "Image viewer:";
            // 
            // aTxBx_Save
            // 
            this.aTxBx_Save.ContextMenuStrip = this.MnAct;
            this.aTxBx_Save.KeyWords = new string[0];
            this.aTxBx_Save.Location = new System.Drawing.Point(88, 6);
            this.aTxBx_Save.Name = "aTxBx_Save";
            this.aTxBx_Save.Seperator = '\0';
            this.aTxBx_Save.Size = new System.Drawing.Size(230, 20);
            this.aTxBx_Save.TabIndex = 24;
            this.aTxBx_Save.Click += new System.EventHandler(this.aTxBx_Save_Click);
            this.aTxBx_Save.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxBx_KeyPress);
            // 
            // aTxBx_Prog
            // 
            this.aTxBx_Prog.ContextMenuStrip = this.MnAct;
            this.aTxBx_Prog.KeyWords = new string[0];
            this.aTxBx_Prog.Location = new System.Drawing.Point(88, 85);
            this.aTxBx_Prog.Name = "aTxBx_Prog";
            this.aTxBx_Prog.Seperator = '\0';
            this.aTxBx_Prog.Size = new System.Drawing.Size(230, 20);
            this.aTxBx_Prog.TabIndex = 29;
            this.aTxBx_Prog.Click += new System.EventHandler(this.aTxBx_Prog_Click);
            this.aTxBx_Prog.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxBx_KeyPress);
            // 
            // aTxBx_Root
            // 
            this.aTxBx_Root.ContextMenuStrip = this.MnAct;
            this.aTxBx_Root.KeyWords = new string[0];
            this.aTxBx_Root.Location = new System.Drawing.Point(88, 45);
            this.aTxBx_Root.Name = "aTxBx_Root";
            this.aTxBx_Root.Seperator = '\0';
            this.aTxBx_Root.Size = new System.Drawing.Size(230, 20);
            this.aTxBx_Root.TabIndex = 32;
            this.aTxBx_Root.Click += new System.EventHandler(this.aTxBx_Root_Click);
            this.aTxBx_Root.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxBx_KeyPress);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(331, 362);
            this.Controls.Add(this.aTxBx_Root);
            this.Controls.Add(this.aTxBx_Prog);
            this.Controls.Add(this.aTxBx_Save);
            this.Controls.Add(this.lblProg);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.ckLbx_Ign);
            this.Controls.Add(this.grBxToggle);
            this.Controls.Add(this.lblIgnored);
            this.Controls.Add(this.lblIntv);
            this.Controls.Add(this.lblRoot);
            this.Controls.Add(this.lblSave);
            this.Controls.Add(this.lblColour);
            this.Controls.Add(this.picBx_Colour);
            this.Controls.Add(this.Nud_Intv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.Load += new System.EventHandler(this.Settings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Intv)).EndInit();
            this.MnAct.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Colour)).EndInit();
            this.grBxToggle.ResumeLayout(false);
            this.grBxToggle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown Nud_Intv;
        private System.Windows.Forms.PictureBox picBx_Colour;
        private System.Windows.Forms.Label lblColour;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.Label lblRoot;
        private System.Windows.Forms.Label lblIntv;
        private System.Windows.Forms.Label lblIgnored;
        private System.Windows.Forms.GroupBox grBxToggle;
        private System.Windows.Forms.CheckBox ChkBx_Gridlines;
        private System.Windows.Forms.CheckedListBox ckLbx_Ign;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.CheckBox ChkBx_Date;
        private System.Windows.Forms.Label lblProg;
        private Classes.AutoCompleteTagger aTxBx_Save;
        private Classes.AutoCompleteTagger aTxBx_Prog;
        private Classes.AutoCompleteTagger aTxBx_Root;
        private System.Windows.Forms.ContextMenuStrip MnAct;
        private System.Windows.Forms.ToolStripMenuItem MnAct_Reset;
    }
}