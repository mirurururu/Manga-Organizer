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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Suggest));
            this.tsCredentials = new System.Windows.Forms.ToolStrip();
            this.tslbMemberID = new System.Windows.Forms.ToolStripLabel();
            this.txbxID = new System.Windows.Forms.ToolStripTextBox();
            this.tslblPass = new System.Windows.Forms.ToolStripLabel();
            this.txbxPass = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnHelp = new System.Windows.Forms.ToolStripButton();
            this.ddmGallery = new System.Windows.Forms.ToolStripDropDownButton();
            this.ckbxDoujin = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxManga = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxArtist = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxGame = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxWestern = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxNonH = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxImage = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxCosplay = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxAsian = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbxMisc = new System.Windows.Forms.ToolStripMenuItem();
            this.Mn_TxBx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnTx_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_SelAll = new System.Windows.Forms.ToolStripMenuItem();
            this.txbxSearch = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lvDetails = new Nagru___Manga_Organizer.ListViewNF();
            this.colURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tsCredentials.SuspendLayout();
            this.Mn_TxBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsCredentials
            // 
            this.tsCredentials.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsCredentials.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsCredentials.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbMemberID,
            this.txbxID,
            this.tslblPass,
            this.txbxPass,
            this.toolStripSeparator2,
            this.tsbtnHelp,
            this.ddmGallery});
            this.tsCredentials.Location = new System.Drawing.Point(0, 365);
            this.tsCredentials.Name = "tsCredentials";
            this.tsCredentials.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsCredentials.Size = new System.Drawing.Size(584, 25);
            this.tsCredentials.TabIndex = 9;
            this.tsCredentials.Text = "toolStrip1";
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
            // ddmGallery
            // 
            this.ddmGallery.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ddmGallery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ddmGallery.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ckbxDoujin,
            this.ckbxManga,
            this.ckbxArtist,
            this.ckbxGame,
            this.ckbxWestern,
            this.ckbxNonH,
            this.ckbxImage,
            this.ckbxCosplay,
            this.ckbxAsian,
            this.ckbxMisc});
            this.ddmGallery.Image = ((System.Drawing.Image)(resources.GetObject("ddmGallery.Image")));
            this.ddmGallery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddmGallery.Name = "ddmGallery";
            this.ddmGallery.Size = new System.Drawing.Size(90, 22);
            this.ddmGallery.Text = "Gallery Types";
            // 
            // ckbxDoujin
            // 
            this.ckbxDoujin.Checked = true;
            this.ckbxDoujin.CheckOnClick = true;
            this.ckbxDoujin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxDoujin.Name = "ckbxDoujin";
            this.ckbxDoujin.Size = new System.Drawing.Size(152, 22);
            this.ckbxDoujin.Text = "Doujinshi";
            this.ckbxDoujin.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxManga
            // 
            this.ckbxManga.Checked = true;
            this.ckbxManga.CheckOnClick = true;
            this.ckbxManga.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxManga.Name = "ckbxManga";
            this.ckbxManga.Size = new System.Drawing.Size(152, 22);
            this.ckbxManga.Text = "Manga";
            this.ckbxManga.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxArtist
            // 
            this.ckbxArtist.CheckOnClick = true;
            this.ckbxArtist.Name = "ckbxArtist";
            this.ckbxArtist.Size = new System.Drawing.Size(152, 22);
            this.ckbxArtist.Text = "Artist CG";
            this.ckbxArtist.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxGame
            // 
            this.ckbxGame.CheckOnClick = true;
            this.ckbxGame.Name = "ckbxGame";
            this.ckbxGame.Size = new System.Drawing.Size(152, 22);
            this.ckbxGame.Text = "Game CG";
            this.ckbxGame.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxWestern
            // 
            this.ckbxWestern.CheckOnClick = true;
            this.ckbxWestern.Name = "ckbxWestern";
            this.ckbxWestern.Size = new System.Drawing.Size(152, 22);
            this.ckbxWestern.Text = "Western";
            this.ckbxWestern.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxNonH
            // 
            this.ckbxNonH.CheckOnClick = true;
            this.ckbxNonH.Name = "ckbxNonH";
            this.ckbxNonH.Size = new System.Drawing.Size(152, 22);
            this.ckbxNonH.Text = "Non-H";
            this.ckbxNonH.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxImage
            // 
            this.ckbxImage.CheckOnClick = true;
            this.ckbxImage.Name = "ckbxImage";
            this.ckbxImage.Size = new System.Drawing.Size(152, 22);
            this.ckbxImage.Text = "Image Set";
            this.ckbxImage.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxCosplay
            // 
            this.ckbxCosplay.CheckOnClick = true;
            this.ckbxCosplay.Name = "ckbxCosplay";
            this.ckbxCosplay.Size = new System.Drawing.Size(152, 22);
            this.ckbxCosplay.Text = "Cosplay";
            this.ckbxCosplay.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxAsian
            // 
            this.ckbxAsian.CheckOnClick = true;
            this.ckbxAsian.Name = "ckbxAsian";
            this.ckbxAsian.Size = new System.Drawing.Size(152, 22);
            this.ckbxAsian.Text = "Asian Porn";
            this.ckbxAsian.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // ckbxMisc
            // 
            this.ckbxMisc.CheckOnClick = true;
            this.ckbxMisc.Name = "ckbxMisc";
            this.ckbxMisc.Size = new System.Drawing.Size(152, 22);
            this.ckbxMisc.Text = "Miscellaneous";
            this.ckbxMisc.CheckedChanged += new System.EventHandler(this.GalleryCheckedChanged);
            // 
            // Mn_TxBx
            // 
            this.Mn_TxBx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnTx_Undo,
            this.toolStripSeparator1,
            this.MnTx_Cut,
            this.MnTx_Copy,
            this.MnTx_Paste,
            this.MnTx_Delete,
            this.toolStripSeparator3,
            this.MnTx_SelAll});
            this.Mn_TxBx.Name = "Mn_Context";
            this.Mn_TxBx.ShowImageMargin = false;
            this.Mn_TxBx.Size = new System.Drawing.Size(91, 148);
            this.Mn_TxBx.Opening += new System.ComponentModel.CancelEventHandler(this.Mn_TxBx_Opening);
            // 
            // MnTx_Undo
            // 
            this.MnTx_Undo.Name = "MnTx_Undo";
            this.MnTx_Undo.ShowShortcutKeys = false;
            this.MnTx_Undo.Size = new System.Drawing.Size(90, 22);
            this.MnTx_Undo.Text = "Undo";
            this.MnTx_Undo.Click += new System.EventHandler(this.MnTx_Undo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(87, 6);
            // 
            // MnTx_Cut
            // 
            this.MnTx_Cut.Name = "MnTx_Cut";
            this.MnTx_Cut.ShowShortcutKeys = false;
            this.MnTx_Cut.Size = new System.Drawing.Size(90, 22);
            this.MnTx_Cut.Text = "Cut";
            this.MnTx_Cut.Click += new System.EventHandler(this.MnTx_Cut_Click);
            // 
            // MnTx_Copy
            // 
            this.MnTx_Copy.Name = "MnTx_Copy";
            this.MnTx_Copy.ShowShortcutKeys = false;
            this.MnTx_Copy.Size = new System.Drawing.Size(90, 22);
            this.MnTx_Copy.Text = "Copy";
            this.MnTx_Copy.Click += new System.EventHandler(this.MnTx_Copy_Click);
            // 
            // MnTx_Paste
            // 
            this.MnTx_Paste.Name = "MnTx_Paste";
            this.MnTx_Paste.ShowShortcutKeys = false;
            this.MnTx_Paste.Size = new System.Drawing.Size(90, 22);
            this.MnTx_Paste.Text = "Paste";
            this.MnTx_Paste.Click += new System.EventHandler(this.MnTx_Paste_Click);
            // 
            // MnTx_Delete
            // 
            this.MnTx_Delete.Name = "MnTx_Delete";
            this.MnTx_Delete.Size = new System.Drawing.Size(90, 22);
            this.MnTx_Delete.Text = "Delete";
            this.MnTx_Delete.Click += new System.EventHandler(this.MnTx_Delete_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(87, 6);
            // 
            // MnTx_SelAll
            // 
            this.MnTx_SelAll.Name = "MnTx_SelAll";
            this.MnTx_SelAll.ShowShortcutKeys = false;
            this.MnTx_SelAll.Size = new System.Drawing.Size(90, 22);
            this.MnTx_SelAll.Text = "Select All";
            this.MnTx_SelAll.Click += new System.EventHandler(this.MnTx_SelAll_Click);
            // 
            // txbxSearch
            // 
            this.txbxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbxSearch.ContextMenuStrip = this.Mn_TxBx;
            this.txbxSearch.Location = new System.Drawing.Point(93, 14);
            this.txbxSearch.Name = "txbxSearch";
            this.txbxSearch.Size = new System.Drawing.Size(317, 20);
            this.txbxSearch.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Location = new System.Drawing.Point(497, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "Select";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(9, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(79, 13);
            this.lblSearch.TabIndex = 15;
            this.lblSearch.Text = "Search Terms: ";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Location = new System.Drawing.Point(416, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 16;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
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
            this.lvDetails.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lvDetails_ColumnWidthChanging);
            this.lvDetails.SelectedIndexChanged += new System.EventHandler(this.lvDetails_SelectedIndexChanged);
            this.lvDetails.DoubleClick += new System.EventHandler(this.lvDetails_DoubleClick);
            this.lvDetails.Resize += new System.EventHandler(this.lvDetails_Resize);
            // 
            // colURL
            // 
            this.colURL.Text = "URL";
            this.colURL.Width = 225;
            // 
            // colTitle
            // 
            this.colTitle.Text = "Title";
            this.colTitle.Width = 255;
            // 
            // Suggest
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(584, 390);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txbxSearch);
            this.Controls.Add(this.lvDetails);
            this.Controls.Add(this.tsCredentials);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Suggest";
            this.ShowIcon = false;
            this.Text = "Suggest";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Suggest_FormClosing);
            this.Load += new System.EventHandler(this.Suggest_Load);
            this.tsCredentials.ResumeLayout(false);
            this.tsCredentials.PerformLayout();
            this.Mn_TxBx.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsCredentials;
        private System.Windows.Forms.ToolStripLabel tslblPass;
        private System.Windows.Forms.ToolStripTextBox txbxPass;
        private System.Windows.Forms.ToolStripLabel tslbMemberID;
        private System.Windows.Forms.ToolStripTextBox txbxID;
        private System.Windows.Forms.ToolStripButton tsbtnHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private ListViewNF lvDetails;
        private System.Windows.Forms.TextBox txbxSearch;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ColumnHeader colURL;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.ContextMenuStrip Mn_TxBx;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Undo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Cut;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Copy;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Paste;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Delete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem MnTx_SelAll;
        private System.Windows.Forms.ToolStripDropDownButton ddmGallery;
        private System.Windows.Forms.ToolStripMenuItem ckbxDoujin;
        private System.Windows.Forms.ToolStripMenuItem ckbxManga;
        private System.Windows.Forms.ToolStripMenuItem ckbxArtist;
        private System.Windows.Forms.ToolStripMenuItem ckbxGame;
        private System.Windows.Forms.ToolStripMenuItem ckbxWestern;
        private System.Windows.Forms.ToolStripMenuItem ckbxNonH;
        private System.Windows.Forms.ToolStripMenuItem ckbxImage;
        private System.Windows.Forms.ToolStripMenuItem ckbxCosplay;
        private System.Windows.Forms.ToolStripMenuItem ckbxAsian;
        private System.Windows.Forms.ToolStripMenuItem ckbxMisc;

    }
}