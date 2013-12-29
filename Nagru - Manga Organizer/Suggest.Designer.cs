namespace Nagru___Manga_Organizer
{
    partial class Suggest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Suggest));
            this.lblID = new System.Windows.Forms.ToolStrip();
            this.tslbMemberID = new System.Windows.Forms.ToolStripLabel();
            this.txbxID = new System.Windows.Forms.ToolStripTextBox();
            this.tslblPass = new System.Windows.Forms.ToolStripLabel();
            this.txbxPass = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnHelp = new System.Windows.Forms.ToolStripButton();
            this.txbxSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lvDetails = new Nagru___Manga_Organizer.ListViewNF();
            this.colURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblID.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblID
            // 
            this.lblID.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblID.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.lblID.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbMemberID,
            this.txbxID,
            this.tslblPass,
            this.txbxPass,
            this.toolStripSeparator2,
            this.tsbtnHelp});
            this.lblID.Location = new System.Drawing.Point(0, 365);
            this.lblID.Name = "lblID";
            this.lblID.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.lblID.Size = new System.Drawing.Size(584, 25);
            this.lblID.TabIndex = 9;
            this.lblID.Text = "toolStrip1";
            // 
            // tslbMemberID
            // 
            this.tslbMemberID.Name = "tslbMemberID";
            this.tslbMemberID.Size = new System.Drawing.Size(69, 22);
            this.tslbMemberID.Text = "memberID: ";
            // 
            // txbxID
            // 
            this.txbxID.Name = "txbxID";
            this.txbxID.Size = new System.Drawing.Size(100, 25);
            this.txbxID.TextChanged += new System.EventHandler(this.txbxID_TextChanged);
            // 
            // tslblPass
            // 
            this.tslblPass.Name = "tslblPass";
            this.tslblPass.Size = new System.Drawing.Size(63, 22);
            this.tslblPass.Text = "passHash: ";
            // 
            // txbxPass
            // 
            this.txbxPass.Name = "txbxPass";
            this.txbxPass.Size = new System.Drawing.Size(200, 25);
            this.txbxPass.TextChanged += new System.EventHandler(this.txbxPass_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnHelp
            // 
            this.tsbtnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnHelp.Image")));
            this.tsbtnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnHelp.Name = "tsbtnHelp";
            this.tsbtnHelp.Size = new System.Drawing.Size(23, 22);
            this.tsbtnHelp.Text = "Help";
            this.tsbtnHelp.Click += new System.EventHandler(this.tsbtn_Help_Clicked);
            // 
            // txbxSearch
            // 
            this.txbxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbxSearch.Location = new System.Drawing.Point(93, 14);
            this.txbxSearch.Name = "txbxSearch";
            this.txbxSearch.Size = new System.Drawing.Size(317, 20);
            this.txbxSearch.TabIndex = 11;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(416, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(497, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(75, 13);
            this.lblSearch.TabIndex = 15;
            this.lblSearch.Text = "Search string: ";
            // 
            // lvDetails
            // 
            this.lvDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colURL,
            this.colTitle});
            this.lvDetails.FullRowSelect = true;
            this.lvDetails.Location = new System.Drawing.Point(12, 40);
            this.lvDetails.MultiSelect = false;
            this.lvDetails.Name = "lvDetails";
            this.lvDetails.Size = new System.Drawing.Size(560, 322);
            this.lvDetails.TabIndex = 10;
            this.lvDetails.UseCompatibleStateImageBehavior = false;
            this.lvDetails.View = System.Windows.Forms.View.Details;
            this.lvDetails.DoubleClick += new System.EventHandler(this.lvDetails_DoubleClick);
            this.lvDetails.Resize += new System.EventHandler(this.lvDetails_Resize);
            // 
            // colURL
            // 
            this.colURL.Text = "URL";
            this.colURL.Width = 220;
            // 
            // colTitle
            // 
            this.colTitle.Text = "Title";
            this.colTitle.Width = 260;
            // 
            // Suggest
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(584, 390);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txbxSearch);
            this.Controls.Add(this.lvDetails);
            this.Controls.Add(this.lblID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Suggest";
            this.ShowIcon = false;
            this.Text = "Suggest";
            this.Load += new System.EventHandler(this.Suggest_Load);
            this.lblID.ResumeLayout(false);
            this.lblID.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip lblID;
        private System.Windows.Forms.ToolStripLabel tslblPass;
        private System.Windows.Forms.ToolStripTextBox txbxPass;
        private System.Windows.Forms.ToolStripLabel tslbMemberID;
        private System.Windows.Forms.ToolStripTextBox txbxID;
        private System.Windows.Forms.ToolStripButton tsbtnHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private ListViewNF lvDetails;
        private System.Windows.Forms.TextBox txbxSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.ColumnHeader colURL;
        private System.Windows.Forms.ColumnHeader colTitle;

    }
}