namespace Nagru___Manga_Organizer
{
    partial class Main
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
            this.TabControl = new System.Windows.Forms.TabControl();
            this.Tb_Browse = new System.Windows.Forms.TabPage();
            this.ChkBx_ShowFav = new System.Windows.Forms.CheckBox();
            this.Btn_Scan = new System.Windows.Forms.Button();
            this.Btn_Clear = new System.Windows.Forms.Button();
            this.Lbl_Search = new System.Windows.Forms.Label();
            this.TxBx_Search = new System.Windows.Forms.TextBox();
            this.Mn_TxBx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnTx_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_SelAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Tb_View = new System.Windows.Forms.TabPage();
            this.Btn_Rand = new System.Windows.Forms.Button();
            this.CmbBx_Artist = new System.Windows.Forms.ComboBox();
            this.Btn_GoUp = new System.Windows.Forms.Button();
            this.Btn_GoDn = new System.Windows.Forms.Button();
            this.Btn_Loc = new System.Windows.Forms.Button();
            this.TxBx_Loc = new System.Windows.Forms.TextBox();
            this.Lbl_Desc = new System.Windows.Forms.Label();
            this.CmbBx_Type = new System.Windows.Forms.ComboBox();
            this.Dt_Date = new System.Windows.Forms.DateTimePicker();
            this.Nud_Pages = new System.Windows.Forms.NumericUpDown();
            this.Lbl_Pages = new System.Windows.Forms.Label();
            this.Lbl_Date = new System.Windows.Forms.Label();
            this.Lbl_Type = new System.Windows.Forms.Label();
            this.Lbl_Tags = new System.Windows.Forms.Label();
            this.Lbl_Artist = new System.Windows.Forms.Label();
            this.Lbl_Title = new System.Windows.Forms.Label();
            this.Mn_EntryOps = new System.Windows.Forms.MenuStrip();
            this.MnTS_Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_CopyTitle = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_OpenSource = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_LoadUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTS_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_OpenDataFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTS_Stats = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Settings = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Tutorial = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_About = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTs_Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_SecretSort = new System.Windows.Forms.ToolStripSeparator();
            this.MnTS_New = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Del = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.PicBx_Cover = new System.Windows.Forms.PictureBox();
            this.Tb_Notes = new System.Windows.Forms.TabPage();
            this.Delay = new System.Windows.Forms.Timer(this.components);
            this.LV_Entries = new Nagru___Manga_Organizer.ListViewNF();
            this.ColArtist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColPages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColTags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColRating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColPos = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.acTxBx_Title = new Nagru___Manga_Organizer.Classes.AutoCompleteTagger();
            this.acTxBx_Tags = new Nagru___Manga_Organizer.Classes.AutoCompleteTagger();
            this.srRating = new Nagru___Manga_Organizer.StarRatingControl();
            this.frTxBx_Desc = new Nagru___Manga_Organizer.FixedRichTextBox();
            this.frTxBx_Notes = new Nagru___Manga_Organizer.FixedRichTextBox();
            this.TabControl.SuspendLayout();
            this.Tb_Browse.SuspendLayout();
            this.Mn_TxBx.SuspendLayout();
            this.Tb_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Pages)).BeginInit();
            this.Mn_EntryOps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBx_Cover)).BeginInit();
            this.Tb_Notes.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.Tb_Browse);
            this.TabControl.Controls.Add(this.Tb_View);
            this.TabControl.Controls.Add(this.Tb_Notes);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.HotTrack = true;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(934, 575);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.TabControl.TabIndex = 0;
            this.TabControl.TabStop = false;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // Tb_Browse
            // 
            this.Tb_Browse.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Tb_Browse.Controls.Add(this.ChkBx_ShowFav);
            this.Tb_Browse.Controls.Add(this.LV_Entries);
            this.Tb_Browse.Controls.Add(this.Btn_Scan);
            this.Tb_Browse.Controls.Add(this.Btn_Clear);
            this.Tb_Browse.Controls.Add(this.Lbl_Search);
            this.Tb_Browse.Controls.Add(this.TxBx_Search);
            this.Tb_Browse.Location = new System.Drawing.Point(4, 22);
            this.Tb_Browse.Name = "Tb_Browse";
            this.Tb_Browse.Padding = new System.Windows.Forms.Padding(3);
            this.Tb_Browse.Size = new System.Drawing.Size(926, 549);
            this.Tb_Browse.TabIndex = 0;
            this.Tb_Browse.Text = "Browse";
            this.Tb_Browse.Click += new System.EventHandler(this.ClearSelection);
            // 
            // ChkBx_ShowFav
            // 
            this.ChkBx_ShowFav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkBx_ShowFav.Appearance = System.Windows.Forms.Appearance.Button;
            this.ChkBx_ShowFav.AutoSize = true;
            this.ChkBx_ShowFav.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ChkBx_ShowFav.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ChkBx_ShowFav.Location = new System.Drawing.Point(802, 5);
            this.ChkBx_ShowFav.Name = "ChkBx_ShowFav";
            this.ChkBx_ShowFav.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ChkBx_ShowFav.Size = new System.Drawing.Size(66, 23);
            this.ChkBx_ShowFav.TabIndex = 6;
            this.ChkBx_ShowFav.Text = "Favs Only";
            this.ChkBx_ShowFav.UseVisualStyleBackColor = false;
            this.ChkBx_ShowFav.CheckedChanged += new System.EventHandler(this.ChkBx_ShowFav_CheckedChanged);
            // 
            // Btn_Scan
            // 
            this.Btn_Scan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Scan.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Scan.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Scan.Location = new System.Drawing.Point(874, 5);
            this.Btn_Scan.Name = "Btn_Scan";
            this.Btn_Scan.Size = new System.Drawing.Size(44, 23);
            this.Btn_Scan.TabIndex = 5;
            this.Btn_Scan.Text = "Scan";
            this.Btn_Scan.UseVisualStyleBackColor = false;
            this.Btn_Scan.Click += new System.EventHandler(this.Btn_Scan_Click);
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.BackColor = System.Drawing.Color.Red;
            this.Btn_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Clear.Location = new System.Drawing.Point(757, 6);
            this.Btn_Clear.Name = "Btn_Clear";
            this.Btn_Clear.Size = new System.Drawing.Size(30, 20);
            this.Btn_Clear.TabIndex = 4;
            this.Btn_Clear.Text = "X";
            this.Btn_Clear.UseVisualStyleBackColor = false;
            this.Btn_Clear.Visible = false;
            this.Btn_Clear.Click += new System.EventHandler(this.Btn_Clear_Click);
            // 
            // Lbl_Search
            // 
            this.Lbl_Search.AutoSize = true;
            this.Lbl_Search.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Search.Location = new System.Drawing.Point(8, 8);
            this.Lbl_Search.Name = "Lbl_Search";
            this.Lbl_Search.Size = new System.Drawing.Size(45, 15);
            this.Lbl_Search.TabIndex = 2;
            this.Lbl_Search.Text = "Search:";
            this.Lbl_Search.Click += new System.EventHandler(this.ClearSelection);
            // 
            // TxBx_Search
            // 
            this.TxBx_Search.AllowDrop = true;
            this.TxBx_Search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxBx_Search.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.TxBx_Search.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Search.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxBx_Search.Location = new System.Drawing.Point(59, 6);
            this.TxBx_Search.Name = "TxBx_Search";
            this.TxBx_Search.Size = new System.Drawing.Size(728, 21);
            this.TxBx_Search.TabIndex = 1;
            this.TxBx_Search.TextChanged += new System.EventHandler(this.TxBx_Search_TextChanged);
            this.TxBx_Search.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.TxBx_Search.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            this.TxBx_Search.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // Mn_TxBx
            // 
            this.Mn_TxBx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnTx_Undo,
            this.toolStripSeparator2,
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(87, 6);
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
            // Tb_View
            // 
            this.Tb_View.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Tb_View.Controls.Add(this.acTxBx_Title);
            this.Tb_View.Controls.Add(this.acTxBx_Tags);
            this.Tb_View.Controls.Add(this.srRating);
            this.Tb_View.Controls.Add(this.Btn_Rand);
            this.Tb_View.Controls.Add(this.CmbBx_Artist);
            this.Tb_View.Controls.Add(this.Btn_GoUp);
            this.Tb_View.Controls.Add(this.Btn_GoDn);
            this.Tb_View.Controls.Add(this.Btn_Loc);
            this.Tb_View.Controls.Add(this.TxBx_Loc);
            this.Tb_View.Controls.Add(this.Lbl_Desc);
            this.Tb_View.Controls.Add(this.CmbBx_Type);
            this.Tb_View.Controls.Add(this.Dt_Date);
            this.Tb_View.Controls.Add(this.Nud_Pages);
            this.Tb_View.Controls.Add(this.Lbl_Pages);
            this.Tb_View.Controls.Add(this.Lbl_Date);
            this.Tb_View.Controls.Add(this.Lbl_Type);
            this.Tb_View.Controls.Add(this.Lbl_Tags);
            this.Tb_View.Controls.Add(this.Lbl_Artist);
            this.Tb_View.Controls.Add(this.Lbl_Title);
            this.Tb_View.Controls.Add(this.Mn_EntryOps);
            this.Tb_View.Controls.Add(this.PicBx_Cover);
            this.Tb_View.Controls.Add(this.frTxBx_Desc);
            this.Tb_View.Location = new System.Drawing.Point(4, 22);
            this.Tb_View.Name = "Tb_View";
            this.Tb_View.Padding = new System.Windows.Forms.Padding(3);
            this.Tb_View.Size = new System.Drawing.Size(926, 549);
            this.Tb_View.TabIndex = 1;
            this.Tb_View.Text = "View";
            // 
            // Btn_Rand
            // 
            this.Btn_Rand.FlatAppearance.BorderSize = 0;
            this.Btn_Rand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Rand.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Rand.Location = new System.Drawing.Point(411, 9);
            this.Btn_Rand.Name = "Btn_Rand";
            this.Btn_Rand.Size = new System.Drawing.Size(23, 23);
            this.Btn_Rand.TabIndex = 27;
            this.Btn_Rand.Text = "♣";
            this.Btn_Rand.UseVisualStyleBackColor = true;
            this.Btn_Rand.Click += new System.EventHandler(this.Btn_Rand_Click);
            // 
            // CmbBx_Artist
            // 
            this.CmbBx_Artist.AllowDrop = true;
            this.CmbBx_Artist.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CmbBx_Artist.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CmbBx_Artist.ContextMenuStrip = this.Mn_TxBx;
            this.CmbBx_Artist.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmbBx_Artist.FormattingEnabled = true;
            this.CmbBx_Artist.Location = new System.Drawing.Point(52, 39);
            this.CmbBx_Artist.Name = "CmbBx_Artist";
            this.CmbBx_Artist.Size = new System.Drawing.Size(440, 21);
            this.CmbBx_Artist.TabIndex = 0;
            this.CmbBx_Artist.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.CmbBx_Artist.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.CmbBx_Artist.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            this.CmbBx_Artist.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // Btn_GoUp
            // 
            this.Btn_GoUp.FlatAppearance.BorderSize = 0;
            this.Btn_GoUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_GoUp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_GoUp.Location = new System.Drawing.Point(469, 9);
            this.Btn_GoUp.Name = "Btn_GoUp";
            this.Btn_GoUp.Size = new System.Drawing.Size(23, 23);
            this.Btn_GoUp.TabIndex = 25;
            this.Btn_GoUp.Text = "▲";
            this.Btn_GoUp.UseVisualStyleBackColor = true;
            this.Btn_GoUp.Click += new System.EventHandler(this.Btn_GoUp_Click);
            // 
            // Btn_GoDn
            // 
            this.Btn_GoDn.FlatAppearance.BorderSize = 0;
            this.Btn_GoDn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_GoDn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_GoDn.Location = new System.Drawing.Point(440, 9);
            this.Btn_GoDn.Name = "Btn_GoDn";
            this.Btn_GoDn.Size = new System.Drawing.Size(23, 23);
            this.Btn_GoDn.TabIndex = 24;
            this.Btn_GoDn.Text = "▼";
            this.Btn_GoDn.UseVisualStyleBackColor = true;
            this.Btn_GoDn.Click += new System.EventHandler(this.Btn_GoDn_Click);
            // 
            // Btn_Loc
            // 
            this.Btn_Loc.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Loc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Loc.Location = new System.Drawing.Point(11, 146);
            this.Btn_Loc.Name = "Btn_Loc";
            this.Btn_Loc.Size = new System.Drawing.Size(35, 21);
            this.Btn_Loc.TabIndex = 23;
            this.Btn_Loc.Text = "Loc";
            this.Btn_Loc.UseVisualStyleBackColor = false;
            this.Btn_Loc.Click += new System.EventHandler(this.Btn_Loc_Click);
            // 
            // TxBx_Loc
            // 
            this.TxBx_Loc.AllowDrop = true;
            this.TxBx_Loc.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Loc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxBx_Loc.Location = new System.Drawing.Point(52, 146);
            this.TxBx_Loc.Name = "TxBx_Loc";
            this.TxBx_Loc.Size = new System.Drawing.Size(440, 21);
            this.TxBx_Loc.TabIndex = 3;
            this.TxBx_Loc.TextChanged += new System.EventHandler(this.TxBx_Loc_TextChanged);
            this.TxBx_Loc.DragDrop += new System.Windows.Forms.DragEventHandler(this.TxBx_Loc_DragDrop);
            this.TxBx_Loc.DragEnter += new System.Windows.Forms.DragEventHandler(this.LV_Entries_DragEnter);
            this.TxBx_Loc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // Lbl_Desc
            // 
            this.Lbl_Desc.AutoSize = true;
            this.Lbl_Desc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Desc.Location = new System.Drawing.Point(8, 260);
            this.Lbl_Desc.Name = "Lbl_Desc";
            this.Lbl_Desc.Size = new System.Drawing.Size(70, 15);
            this.Lbl_Desc.TabIndex = 14;
            this.Lbl_Desc.Text = "Description:";
            // 
            // CmbBx_Type
            // 
            this.CmbBx_Type.AutoCompleteCustomSource.AddRange(new string[] {
            "Cosplay",
            "Doujinshi",
            "Ecchi",
            "Game CG",
            "Image Set",
            "Manga",
            "Western"});
            this.CmbBx_Type.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CmbBx_Type.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.CmbBx_Type.ContextMenuStrip = this.Mn_TxBx;
            this.CmbBx_Type.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmbBx_Type.FormattingEnabled = true;
            this.CmbBx_Type.Items.AddRange(new object[] {
            "Cosplay",
            "Doujinshi",
            "Ecchi",
            "Game CG",
            "Image Set",
            "Manga",
            "Western"});
            this.CmbBx_Type.Location = new System.Drawing.Point(52, 216);
            this.CmbBx_Type.Name = "CmbBx_Type";
            this.CmbBx_Type.Size = new System.Drawing.Size(142, 21);
            this.CmbBx_Type.TabIndex = 7;
            this.CmbBx_Type.Text = "Manga";
            this.CmbBx_Type.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.CmbBx_Type.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // Dt_Date
            // 
            this.Dt_Date.AllowDrop = true;
            this.Dt_Date.CustomFormat = "MMMM dd, yyyy";
            this.Dt_Date.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Dt_Date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Dt_Date.Location = new System.Drawing.Point(52, 181);
            this.Dt_Date.MinDate = new System.DateTime(1972, 4, 12, 0, 0, 0, 0);
            this.Dt_Date.Name = "Dt_Date";
            this.Dt_Date.Size = new System.Drawing.Size(169, 21);
            this.Dt_Date.TabIndex = 4;
            this.Dt_Date.ValueChanged += new System.EventHandler(this.EntryAlt_DtNum);
            this.Dt_Date.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dt_Date_DragDrop);
            this.Dt_Date.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            // 
            // Nud_Pages
            // 
            this.Nud_Pages.AllowDrop = true;
            this.Nud_Pages.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nud_Pages.Location = new System.Drawing.Point(401, 181);
            this.Nud_Pages.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.Nud_Pages.Name = "Nud_Pages";
            this.Nud_Pages.Size = new System.Drawing.Size(91, 21);
            this.Nud_Pages.TabIndex = 5;
            this.Nud_Pages.ThousandsSeparator = true;
            this.Nud_Pages.ValueChanged += new System.EventHandler(this.EntryAlt_DtNum);
            this.Nud_Pages.DragDrop += new System.Windows.Forms.DragEventHandler(this.Nud_Pages_DragDrop);
            this.Nud_Pages.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            // 
            // Lbl_Pages
            // 
            this.Lbl_Pages.AutoSize = true;
            this.Lbl_Pages.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Pages.Location = new System.Drawing.Point(354, 184);
            this.Lbl_Pages.Name = "Lbl_Pages";
            this.Lbl_Pages.Size = new System.Drawing.Size(41, 15);
            this.Lbl_Pages.TabIndex = 7;
            this.Lbl_Pages.Text = "Pages:";
            // 
            // Lbl_Date
            // 
            this.Lbl_Date.AutoSize = true;
            this.Lbl_Date.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Date.Location = new System.Drawing.Point(12, 184);
            this.Lbl_Date.Name = "Lbl_Date";
            this.Lbl_Date.Size = new System.Drawing.Size(34, 15);
            this.Lbl_Date.TabIndex = 6;
            this.Lbl_Date.Text = "Date:";
            // 
            // Lbl_Type
            // 
            this.Lbl_Type.AutoSize = true;
            this.Lbl_Type.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Type.Location = new System.Drawing.Point(10, 218);
            this.Lbl_Type.Name = "Lbl_Type";
            this.Lbl_Type.Size = new System.Drawing.Size(36, 15);
            this.Lbl_Type.TabIndex = 5;
            this.Lbl_Type.Text = "Type:";
            // 
            // Lbl_Tags
            // 
            this.Lbl_Tags.AutoSize = true;
            this.Lbl_Tags.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Tags.Location = new System.Drawing.Point(11, 113);
            this.Lbl_Tags.Name = "Lbl_Tags";
            this.Lbl_Tags.Size = new System.Drawing.Size(35, 15);
            this.Lbl_Tags.TabIndex = 4;
            this.Lbl_Tags.Text = "Tags:";
            // 
            // Lbl_Artist
            // 
            this.Lbl_Artist.AutoSize = true;
            this.Lbl_Artist.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Artist.Location = new System.Drawing.Point(8, 41);
            this.Lbl_Artist.Name = "Lbl_Artist";
            this.Lbl_Artist.Size = new System.Drawing.Size(38, 15);
            this.Lbl_Artist.TabIndex = 3;
            this.Lbl_Artist.Text = "Artist:";
            // 
            // Lbl_Title
            // 
            this.Lbl_Title.AutoSize = true;
            this.Lbl_Title.BackColor = System.Drawing.Color.Transparent;
            this.Lbl_Title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Title.Location = new System.Drawing.Point(13, 78);
            this.Lbl_Title.Name = "Lbl_Title";
            this.Lbl_Title.Size = new System.Drawing.Size(33, 15);
            this.Lbl_Title.TabIndex = 2;
            this.Lbl_Title.Text = "Title:";
            // 
            // Mn_EntryOps
            // 
            this.Mn_EntryOps.AllowMerge = false;
            this.Mn_EntryOps.BackColor = System.Drawing.Color.Transparent;
            this.Mn_EntryOps.Dock = System.Windows.Forms.DockStyle.None;
            this.Mn_EntryOps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnTS_Menu,
            this.MnTS_SecretSort,
            this.MnTS_New,
            this.MnTS_Edit,
            this.MnTS_Open,
            this.MnTS_Del,
            this.MnTS_Clear});
            this.Mn_EntryOps.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.Mn_EntryOps.Location = new System.Drawing.Point(3, 3);
            this.Mn_EntryOps.Name = "Mn_EntryOps";
            this.Mn_EntryOps.Size = new System.Drawing.Size(388, 27);
            this.Mn_EntryOps.TabIndex = 21;
            this.Mn_EntryOps.Text = "menuStrip";
            // 
            // MnTS_Menu
            // 
            this.MnTS_Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnTS_CopyTitle,
            this.MnTS_OpenSource,
            this.MnTS_LoadUrl,
            this.toolStripSeparator5,
            this.MnTS_Save,
            this.MnTS_OpenDataFolder,
            this.toolStripSeparator4,
            this.MnTS_Stats,
            this.MnTS_Settings,
            this.MnTS_Tutorial,
            this.MnTS_About,
            this.MnTs_Quit});
            this.MnTS_Menu.Name = "MnTS_Menu";
            this.MnTS_Menu.Size = new System.Drawing.Size(50, 23);
            this.MnTS_Menu.Text = "Menu";
            // 
            // MnTS_CopyTitle
            // 
            this.MnTS_CopyTitle.Name = "MnTS_CopyTitle";
            this.MnTS_CopyTitle.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.MnTS_CopyTitle.ShowShortcutKeys = false;
            this.MnTS_CopyTitle.Size = new System.Drawing.Size(183, 22);
            this.MnTS_CopyTitle.Text = "Copy Formatted Title";
            this.MnTS_CopyTitle.ToolTipText = "Alt+F";
            this.MnTS_CopyTitle.Click += new System.EventHandler(this.MnTS_CopyTitle_Click);
            // 
            // MnTS_OpenSource
            // 
            this.MnTS_OpenSource.Name = "MnTS_OpenSource";
            this.MnTS_OpenSource.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.MnTS_OpenSource.ShowShortcutKeys = false;
            this.MnTS_OpenSource.Size = new System.Drawing.Size(183, 22);
            this.MnTS_OpenSource.Text = "Open Entry\'s Folder";
            this.MnTS_OpenSource.ToolTipText = "Alt+E";
            this.MnTS_OpenSource.Click += new System.EventHandler(this.MnTS_OpenSource_Click);
            // 
            // MnTS_LoadUrl
            // 
            this.MnTS_LoadUrl.Name = "MnTS_LoadUrl";
            this.MnTS_LoadUrl.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.U)));
            this.MnTS_LoadUrl.ShowShortcutKeys = false;
            this.MnTS_LoadUrl.Size = new System.Drawing.Size(183, 22);
            this.MnTS_LoadUrl.Text = "GET from URL";
            this.MnTS_LoadUrl.ToolTipText = "Alt+U";
            this.MnTS_LoadUrl.Click += new System.EventHandler(this.MnTS_LoadUrl_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(180, 6);
            // 
            // MnTS_Save
            // 
            this.MnTS_Save.Name = "MnTS_Save";
            this.MnTS_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.MnTS_Save.ShowShortcutKeys = false;
            this.MnTS_Save.Size = new System.Drawing.Size(183, 22);
            this.MnTS_Save.Text = "Save Database";
            this.MnTS_Save.ToolTipText = "Alt+A";
            this.MnTS_Save.Click += new System.EventHandler(this.MnTS_Save_Click);
            // 
            // MnTS_OpenDataFolder
            // 
            this.MnTS_OpenDataFolder.Name = "MnTS_OpenDataFolder";
            this.MnTS_OpenDataFolder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.MnTS_OpenDataFolder.ShowShortcutKeys = false;
            this.MnTS_OpenDataFolder.Size = new System.Drawing.Size(183, 22);
            this.MnTS_OpenDataFolder.Text = "Open Database Folder";
            this.MnTS_OpenDataFolder.ToolTipText = "Alt+L";
            this.MnTS_OpenDataFolder.Click += new System.EventHandler(this.MnTS_OpenDataFolder_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(180, 6);
            // 
            // MnTS_Stats
            // 
            this.MnTS_Stats.Name = "MnTS_Stats";
            this.MnTS_Stats.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.MnTS_Stats.ShowShortcutKeys = false;
            this.MnTS_Stats.Size = new System.Drawing.Size(183, 22);
            this.MnTS_Stats.Text = "Show Tag Stats";
            this.MnTS_Stats.ToolTipText = "Alt+T";
            this.MnTS_Stats.Click += new System.EventHandler(this.MnTS_Stats_Click);
            // 
            // MnTS_Settings
            // 
            this.MnTS_Settings.Name = "MnTS_Settings";
            this.MnTS_Settings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.MnTS_Settings.ShowShortcutKeys = false;
            this.MnTS_Settings.Size = new System.Drawing.Size(183, 22);
            this.MnTS_Settings.Text = "Settings";
            this.MnTS_Settings.ToolTipText = "Alt+P";
            this.MnTS_Settings.Click += new System.EventHandler(this.Mn_Settings_Click);
            // 
            // MnTS_Tutorial
            // 
            this.MnTS_Tutorial.Name = "MnTS_Tutorial";
            this.MnTS_Tutorial.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.MnTS_Tutorial.ShowShortcutKeys = false;
            this.MnTS_Tutorial.Size = new System.Drawing.Size(183, 22);
            this.MnTS_Tutorial.Text = "Tutorial";
            this.MnTS_Tutorial.ToolTipText = "Alt+T";
            this.MnTS_Tutorial.Click += new System.EventHandler(this.MnTS_Tutorial_Click);
            // 
            // MnTS_About
            // 
            this.MnTS_About.Name = "MnTS_About";
            this.MnTS_About.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
            this.MnTS_About.ShowShortcutKeys = false;
            this.MnTS_About.Size = new System.Drawing.Size(183, 22);
            this.MnTS_About.Text = "About";
            this.MnTS_About.ToolTipText = "Alt+B";
            this.MnTS_About.Click += new System.EventHandler(this.MnTS_About_Click);
            // 
            // MnTs_Quit
            // 
            this.MnTs_Quit.Name = "MnTs_Quit";
            this.MnTs_Quit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.MnTs_Quit.ShowShortcutKeys = false;
            this.MnTs_Quit.Size = new System.Drawing.Size(183, 22);
            this.MnTs_Quit.Text = "Quit";
            this.MnTs_Quit.ToolTipText = "Alt+Q";
            this.MnTs_Quit.Click += new System.EventHandler(this.MnTs_Quit_Click);
            // 
            // MnTS_SecretSort
            // 
            this.MnTS_SecretSort.Name = "MnTS_SecretSort";
            this.MnTS_SecretSort.Size = new System.Drawing.Size(6, 23);
            // 
            // MnTS_New
            // 
            this.MnTS_New.Name = "MnTS_New";
            this.MnTS_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N)));
            this.MnTS_New.ShowShortcutKeys = false;
            this.MnTS_New.Size = new System.Drawing.Size(43, 23);
            this.MnTS_New.Text = "New";
            this.MnTS_New.ToolTipText = "Alt+N";
            this.MnTS_New.Click += new System.EventHandler(this.MnTS_New_Click);
            // 
            // MnTS_Edit
            // 
            this.MnTS_Edit.Name = "MnTS_Edit";
            this.MnTS_Edit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.MnTS_Edit.ShowShortcutKeys = false;
            this.MnTS_Edit.Size = new System.Drawing.Size(43, 23);
            this.MnTS_Edit.Text = "Save";
            this.MnTS_Edit.ToolTipText = "Alt+S";
            this.MnTS_Edit.Visible = false;
            this.MnTS_Edit.Click += new System.EventHandler(this.MnTS_Edit_Click);
            // 
            // MnTS_Open
            // 
            this.MnTS_Open.Name = "MnTS_Open";
            this.MnTS_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.MnTS_Open.ShowShortcutKeys = false;
            this.MnTS_Open.Size = new System.Drawing.Size(48, 23);
            this.MnTS_Open.Text = "Open";
            this.MnTS_Open.ToolTipText = "Alt+O";
            this.MnTS_Open.Visible = false;
            this.MnTS_Open.Click += new System.EventHandler(this.MnTS_Open_Click);
            // 
            // MnTS_Del
            // 
            this.MnTS_Del.Name = "MnTS_Del";
            this.MnTS_Del.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D)));
            this.MnTS_Del.ShowShortcutKeys = false;
            this.MnTS_Del.Size = new System.Drawing.Size(52, 23);
            this.MnTS_Del.Text = "Delete";
            this.MnTS_Del.ToolTipText = "Alt+D";
            this.MnTS_Del.Visible = false;
            this.MnTS_Del.Click += new System.EventHandler(this.MnTS_Delete_Click);
            // 
            // MnTS_Clear
            // 
            this.MnTS_Clear.Name = "MnTS_Clear";
            this.MnTS_Clear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.MnTS_Clear.ShowShortcutKeys = false;
            this.MnTS_Clear.Size = new System.Drawing.Size(46, 23);
            this.MnTS_Clear.Text = "Clear";
            this.MnTS_Clear.ToolTipText = "Alt+C";
            this.MnTS_Clear.Click += new System.EventHandler(this.MnTS_Clear_Click);
            // 
            // PicBx_Cover
            // 
            this.PicBx_Cover.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PicBx_Cover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.PicBx_Cover.Location = new System.Drawing.Point(498, 9);
            this.PicBx_Cover.Name = "PicBx_Cover";
            this.PicBx_Cover.Size = new System.Drawing.Size(420, 533);
            this.PicBx_Cover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicBx_Cover.TabIndex = 0;
            this.PicBx_Cover.TabStop = false;
            this.PicBx_Cover.Click += new System.EventHandler(this.PicBx_Cover_Click);
            this.PicBx_Cover.Resize += new System.EventHandler(this.PicBx_Cover_Resize);
            // 
            // Tb_Notes
            // 
            this.Tb_Notes.Controls.Add(this.frTxBx_Notes);
            this.Tb_Notes.Location = new System.Drawing.Point(4, 22);
            this.Tb_Notes.Name = "Tb_Notes";
            this.Tb_Notes.Size = new System.Drawing.Size(926, 549);
            this.Tb_Notes.TabIndex = 2;
            this.Tb_Notes.Text = "Notes";
            this.Tb_Notes.UseVisualStyleBackColor = true;
            // 
            // Delay
            // 
            this.Delay.Interval = 400;
            this.Delay.Tick += new System.EventHandler(this.Delay_Tick);
            // 
            // LV_Entries
            // 
            this.LV_Entries.AllowDrop = true;
            this.LV_Entries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_Entries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LV_Entries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColArtist,
            this.ColTitle,
            this.ColPages,
            this.ColTags,
            this.colDate,
            this.ColType,
            this.ColRating,
            this.ColPos});
            this.LV_Entries.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LV_Entries.FullRowSelect = true;
            this.LV_Entries.HideSelection = false;
            this.LV_Entries.LabelWrap = false;
            this.LV_Entries.Location = new System.Drawing.Point(0, 32);
            this.LV_Entries.MultiSelect = false;
            this.LV_Entries.Name = "LV_Entries";
            this.LV_Entries.Size = new System.Drawing.Size(923, 517);
            this.LV_Entries.TabIndex = 0;
            this.LV_Entries.UseCompatibleStateImageBehavior = false;
            this.LV_Entries.View = System.Windows.Forms.View.Details;
            this.LV_Entries.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LV_Entries_ColumnClick);
            this.LV_Entries.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.LV_Entries_ColumnWidthChanging);
            this.LV_Entries.SelectedIndexChanged += new System.EventHandler(this.LV_Entries_SelectedIndexChanged);
            this.LV_Entries.DragDrop += new System.Windows.Forms.DragEventHandler(this.LV_Entries_DragDrop);
            this.LV_Entries.DragEnter += new System.Windows.Forms.DragEventHandler(this.LV_Entries_DragEnter);
            this.LV_Entries.DoubleClick += new System.EventHandler(this.LV_Entries_DoubleClick);
            this.LV_Entries.MouseHover += new System.EventHandler(this.LV_Entries_MouseHover);
            this.LV_Entries.Resize += new System.EventHandler(this.LV_Entries_Resize);
            // 
            // ColArtist
            // 
            this.ColArtist.Text = "Artist";
            this.ColArtist.Width = 190;
            // 
            // ColTitle
            // 
            this.ColTitle.Text = "Title";
            this.ColTitle.Width = 240;
            // 
            // ColPages
            // 
            this.ColPages.Text = "Pages";
            this.ColPages.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColPages.Width = 50;
            // 
            // ColTags
            // 
            this.ColTags.Text = "Tags";
            this.ColTags.Width = 210;
            // 
            // colDate
            // 
            this.colDate.Text = "Date";
            this.colDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colDate.Width = 70;
            // 
            // ColType
            // 
            this.ColType.Text = "Type";
            this.ColType.Width = 80;
            // 
            // ColRating
            // 
            this.ColRating.Text = "Rating";
            this.ColRating.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColRating.Width = 70;
            // 
            // ColPos
            // 
            this.ColPos.Width = 0;
            // 
            // acTxBx_Title
            // 
            this.acTxBx_Title.AllowDrop = true;
            this.acTxBx_Title.ContextMenuStrip = this.Mn_TxBx;
            this.acTxBx_Title.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acTxBx_Title.KeyWords = new string[0];
            this.acTxBx_Title.Location = new System.Drawing.Point(52, 75);
            this.acTxBx_Title.Name = "acTxBx_Title";
            this.acTxBx_Title.Seperator = '\0';
            this.acTxBx_Title.Size = new System.Drawing.Size(440, 21);
            this.acTxBx_Title.TabIndex = 1;
            this.acTxBx_Title.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.acTxBx_Title.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.acTxBx_Title.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            this.acTxBx_Title.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // acTxBx_Tags
            // 
            this.acTxBx_Tags.AllowDrop = true;
            this.acTxBx_Tags.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.acTxBx_Tags.ContextMenuStrip = this.Mn_TxBx;
            this.acTxBx_Tags.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acTxBx_Tags.KeyWords = new string[0];
            this.acTxBx_Tags.Location = new System.Drawing.Point(52, 111);
            this.acTxBx_Tags.Name = "acTxBx_Tags";
            this.acTxBx_Tags.Seperator = ',';
            this.acTxBx_Tags.Size = new System.Drawing.Size(440, 21);
            this.acTxBx_Tags.TabIndex = 2;
            this.acTxBx_Tags.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.acTxBx_Tags.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.acTxBx_Tags.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            this.acTxBx_Tags.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // srRating
            // 
            this.srRating.Location = new System.Drawing.Point(372, 216);
            this.srRating.Name = "srRating";
            this.srRating.Size = new System.Drawing.Size(120, 18);
            this.srRating.TabIndex = 28;
            this.srRating.Text = "starRatingControl1";
            this.srRating.Click += new System.EventHandler(this.srRating_Click);
            // 
            // frTxBx_Desc
            // 
            this.frTxBx_Desc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.frTxBx_Desc.ContextMenuStrip = this.Mn_TxBx;
            this.frTxBx_Desc.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frTxBx_Desc.Location = new System.Drawing.Point(8, 278);
            this.frTxBx_Desc.Name = "frTxBx_Desc";
            this.frTxBx_Desc.Size = new System.Drawing.Size(484, 264);
            this.frTxBx_Desc.TabIndex = 8;
            this.frTxBx_Desc.Text = "";
            this.frTxBx_Desc.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.frTxBx_Desc_LinkClicked);
            this.frTxBx_Desc.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.frTxBx_Desc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frTxBx_KeyDown);
            this.frTxBx_Desc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // frTxBx_Notes
            // 
            this.frTxBx_Notes.AcceptsTab = true;
            this.frTxBx_Notes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.frTxBx_Notes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frTxBx_Notes.ContextMenuStrip = this.Mn_TxBx;
            this.frTxBx_Notes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frTxBx_Notes.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frTxBx_Notes.ForeColor = System.Drawing.SystemColors.Window;
            this.frTxBx_Notes.Location = new System.Drawing.Point(0, 0);
            this.frTxBx_Notes.Name = "frTxBx_Notes";
            this.frTxBx_Notes.Size = new System.Drawing.Size(926, 549);
            this.frTxBx_Notes.TabIndex = 1;
            this.frTxBx_Notes.Text = "";
            this.frTxBx_Notes.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.frTxBx_Notes_LinkClicked);
            this.frTxBx_Notes.TextChanged += new System.EventHandler(this.frTxBx_Notes_TextChanged);
            this.frTxBx_Notes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frTxBx_KeyDown);
            this.frTxBx_Notes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TbPg_Click);
            // 
            // Main
            // 
            this.AcceptButton = this.Btn_Clear;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(934, 575);
            this.Controls.Add(this.TabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.MainMenuStrip = this.Mn_EntryOps;
            this.MinimumSize = new System.Drawing.Size(350, 150);
            this.Name = "Main";
            this.Text = "Manga Organizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResizeEnd += new System.EventHandler(this.Main_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.TabControl.ResumeLayout(false);
            this.Tb_Browse.ResumeLayout(false);
            this.Tb_Browse.PerformLayout();
            this.Mn_TxBx.ResumeLayout(false);
            this.Tb_View.ResumeLayout(false);
            this.Tb_View.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Pages)).EndInit();
            this.Mn_EntryOps.ResumeLayout(false);
            this.Mn_EntryOps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBx_Cover)).EndInit();
            this.Tb_Notes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage Tb_Browse;
        private System.Windows.Forms.TabPage Tb_View;
        private System.Windows.Forms.Label Lbl_Pages;
        private System.Windows.Forms.Label Lbl_Date;
        private System.Windows.Forms.Label Lbl_Type;
        private System.Windows.Forms.Label Lbl_Tags;
        private System.Windows.Forms.Label Lbl_Artist;
        private System.Windows.Forms.Label Lbl_Title;
        private System.Windows.Forms.PictureBox PicBx_Cover;
        private System.Windows.Forms.Label Lbl_Search;
        private System.Windows.Forms.TextBox TxBx_Search;
        private System.Windows.Forms.Label Lbl_Desc;
        private System.Windows.Forms.ComboBox CmbBx_Type;
        private System.Windows.Forms.DateTimePicker Dt_Date;
        private System.Windows.Forms.NumericUpDown Nud_Pages;
        private System.Windows.Forms.TextBox TxBx_Loc;
        private System.Windows.Forms.TabPage Tb_Notes;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.MenuStrip Mn_EntryOps;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Menu;
        private System.Windows.Forms.ToolStripMenuItem MnTS_CopyTitle;
        private System.Windows.Forms.ToolStripMenuItem MnTS_New;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Edit;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Del;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Open;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Save;
        private System.Windows.Forms.ToolStripMenuItem MnTS_OpenDataFolder;
        private System.Windows.Forms.ToolStripSeparator MnTS_SecretSort;
        private System.Windows.Forms.ToolStripMenuItem MnTS_OpenSource;
        private FixedRichTextBox frTxBx_Notes;
        private FixedRichTextBox frTxBx_Desc;
        private System.Windows.Forms.Button Btn_Scan;
        private System.Windows.Forms.Timer Delay;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Clear;
        private ListViewNF LV_Entries;
        private System.Windows.Forms.ColumnHeader ColArtist;
        private System.Windows.Forms.ColumnHeader ColTitle;
        private System.Windows.Forms.ColumnHeader ColPages;
        private System.Windows.Forms.ColumnHeader ColTags;
        private System.Windows.Forms.ColumnHeader ColType;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Undo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Cut;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Copy;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Paste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem MnTx_SelAll;
        private System.Windows.Forms.Button Btn_Loc;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem MnTS_About;
        private System.Windows.Forms.CheckBox ChkBx_ShowFav;
        private System.Windows.Forms.ToolStripMenuItem MnTS_LoadUrl;
        private System.Windows.Forms.ToolStripMenuItem MnTs_Quit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.Button Btn_GoUp;
        private System.Windows.Forms.Button Btn_GoDn;
        private System.Windows.Forms.ComboBox CmbBx_Artist;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Stats;
        private System.Windows.Forms.Button Btn_Rand;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Settings;
        private System.Windows.Forms.ContextMenuStrip Mn_TxBx;
        private System.Windows.Forms.ColumnHeader ColPos;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Delete;
        private StarRatingControl srRating;
        private System.Windows.Forms.ColumnHeader ColRating;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Tutorial;
        private Classes.AutoCompleteTagger acTxBx_Tags;
        private System.Windows.Forms.ColumnHeader colDate;
        private Classes.AutoCompleteTagger acTxBx_Title;
    }
}

