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
            this.Nud_Intv = new System.Windows.Forms.NumericUpDown();
            this.picBx_Colour = new System.Windows.Forms.PictureBox();
            this.lblColour = new System.Windows.Forms.Label();
            this.lblSave = new System.Windows.Forms.Label();
            this.TxBx_Save = new System.Windows.Forms.TextBox();
            this.lblRoot = new System.Windows.Forms.Label();
            this.ScrSave = new System.Windows.Forms.HScrollBar();
            this.ScrRoot = new System.Windows.Forms.HScrollBar();
            this.TxBx_Root = new System.Windows.Forms.TextBox();
            this.lblIntv = new System.Windows.Forms.Label();
            this.lblIgnored = new System.Windows.Forms.Label();
            this.grBxToggle = new System.Windows.Forms.GroupBox();
            this.ChkBx_Gridlines = new System.Windows.Forms.CheckBox();
            this.ckLbx_Ign = new System.Windows.Forms.CheckedListBox();
            this.Btn_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Intv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Colour)).BeginInit();
            this.grBxToggle.SuspendLayout();
            this.SuspendLayout();
            // 
            // Nud_Intv
            // 
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
            // picBx_Colour
            // 
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
            this.lblSave.Size = new System.Drawing.Size(122, 13);
            this.lblSave.TabIndex = 5;
            this.lblSave.Text = "Database save location:";
            // 
            // TxBx_Save
            // 
            this.TxBx_Save.Location = new System.Drawing.Point(12, 25);
            this.TxBx_Save.Name = "TxBx_Save";
            this.TxBx_Save.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxBx_Save.Size = new System.Drawing.Size(306, 20);
            this.TxBx_Save.TabIndex = 6;
            this.TxBx_Save.Click += new System.EventHandler(this.TxBx_Save_Click);
            this.TxBx_Save.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxBx_KeyPress);
            // 
            // lblRoot
            // 
            this.lblRoot.AutoSize = true;
            this.lblRoot.Location = new System.Drawing.Point(9, 65);
            this.lblRoot.Name = "lblRoot";
            this.lblRoot.Size = new System.Drawing.Size(93, 13);
            this.lblRoot.TabIndex = 7;
            this.lblRoot.Text = "Manga root folder:";
            // 
            // ScrSave
            // 
            this.ScrSave.Location = new System.Drawing.Point(12, 44);
            this.ScrSave.Name = "ScrSave";
            this.ScrSave.Size = new System.Drawing.Size(306, 17);
            this.ScrSave.TabIndex = 9;
            this.ScrSave.Visible = false;
            this.ScrSave.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Scr_Save_Scroll);
            // 
            // ScrRoot
            // 
            this.ScrRoot.Location = new System.Drawing.Point(12, 100);
            this.ScrRoot.Name = "ScrRoot";
            this.ScrRoot.Size = new System.Drawing.Size(306, 17);
            this.ScrRoot.TabIndex = 11;
            this.ScrRoot.Visible = false;
            this.ScrRoot.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrRoot_Scroll);
            // 
            // TxBx_Root
            // 
            this.TxBx_Root.Location = new System.Drawing.Point(12, 81);
            this.TxBx_Root.Name = "TxBx_Root";
            this.TxBx_Root.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxBx_Root.Size = new System.Drawing.Size(306, 20);
            this.TxBx_Root.TabIndex = 10;
            this.TxBx_Root.Click += new System.EventHandler(this.TxBx_Root_Click);
            this.TxBx_Root.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxBx_KeyPress);
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
            this.grBxToggle.Controls.Add(this.ChkBx_Gridlines);
            this.grBxToggle.Location = new System.Drawing.Point(179, 196);
            this.grBxToggle.Name = "grBxToggle";
            this.grBxToggle.Size = new System.Drawing.Size(139, 84);
            this.grBxToggle.TabIndex = 15;
            this.grBxToggle.TabStop = false;
            this.grBxToggle.Text = "Defaults";
            // 
            // ChkBx_Gridlines
            // 
            this.ChkBx_Gridlines.AutoSize = true;
            this.ChkBx_Gridlines.Location = new System.Drawing.Point(15, 36);
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
            this.Btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Save.Location = new System.Drawing.Point(204, 306);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(91, 23);
            this.Btn_Save.TabIndex = 17;
            this.Btn_Save.Text = "Save Settings";
            this.Btn_Save.UseVisualStyleBackColor = false;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(331, 362);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.ckLbx_Ign);
            this.Controls.Add(this.grBxToggle);
            this.Controls.Add(this.lblIgnored);
            this.Controls.Add(this.lblIntv);
            this.Controls.Add(this.ScrRoot);
            this.Controls.Add(this.TxBx_Root);
            this.Controls.Add(this.ScrSave);
            this.Controls.Add(this.lblRoot);
            this.Controls.Add(this.TxBx_Save);
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
        private System.Windows.Forms.TextBox TxBx_Save;
        private System.Windows.Forms.Label lblRoot;
        private System.Windows.Forms.HScrollBar ScrSave;
        private System.Windows.Forms.HScrollBar ScrRoot;
        private System.Windows.Forms.TextBox TxBx_Root;
        private System.Windows.Forms.Label lblIntv;
        private System.Windows.Forms.Label lblIgnored;
        private System.Windows.Forms.GroupBox grBxToggle;
        private System.Windows.Forms.CheckBox ChkBx_Gridlines;
        private System.Windows.Forms.CheckedListBox ckLbx_Ign;
        private System.Windows.Forms.Button Btn_Save;
    }
}