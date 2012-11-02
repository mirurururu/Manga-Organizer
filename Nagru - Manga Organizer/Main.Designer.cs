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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.TabControl = new System.Windows.Forms.TabControl();
            this.Tb_Browse = new System.Windows.Forms.TabPage();
            this.ChkBx_ShowFav = new System.Windows.Forms.CheckBox();
            this.LV_Entries = new Nagru___Manga_Organizer.ListViewNF();
            this.ColArtist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColPages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColTags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_SelAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Tb_View = new System.Windows.Forms.TabPage();
            this.Btn_Loc = new System.Windows.Forms.Button();
            this.ChkBx_Fav = new System.Windows.Forms.CheckBox();
            this.TxBx_Loc = new System.Windows.Forms.TextBox();
            this.Lbl_Desc = new System.Windows.Forms.Label();
            this.CmBx_Type = new System.Windows.Forms.ComboBox();
            this.Dt_Date = new System.Windows.Forms.DateTimePicker();
            this.Nud_Pages = new System.Windows.Forms.NumericUpDown();
            this.TxBx_Tags = new System.Windows.Forms.TextBox();
            this.TxBx_Artist = new System.Windows.Forms.TextBox();
            this.Lbl_Pages = new System.Windows.Forms.Label();
            this.Lbl_Date = new System.Windows.Forms.Label();
            this.Lbl_Type = new System.Windows.Forms.Label();
            this.Lbl_Tags = new System.Windows.Forms.Label();
            this.Lbl_Artist = new System.Windows.Forms.Label();
            this.Lbl_Title = new System.Windows.Forms.Label();
            this.TxBx_Title = new System.Windows.Forms.TextBox();
            this.PicBx_Cover = new System.Windows.Forms.PictureBox();
            this.Mn_EntryOps = new System.Windows.Forms.MenuStrip();
            this.MnTS_Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_SavLoc = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_OpenDataFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTS_OpenEntryFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_DefLoc = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_CopyTitle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTS_GET = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_About = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_SecretSort = new System.Windows.Forms.ToolStripSeparator();
            this.MnTS_New = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Del = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTS_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.frTxBx_Desc = new Nagru___Manga_Organizer.FixedRichTextBox();
            this.Mn_rTxBx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnRTx_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSeperate = new System.Windows.Forms.ToolStripSeparator();
            this.MnRTx_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.MnRTx_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MnRTx_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSeperate2 = new System.Windows.Forms.ToolStripSeparator();
            this.MnRTx_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Tb_Notes = new System.Windows.Forms.TabPage();
            this.frTxBx_Notes = new Nagru___Manga_Organizer.FixedRichTextBox();
            this.Delay = new System.Windows.Forms.Timer(this.components);
            this.TabControl.SuspendLayout();
            this.Tb_Browse.SuspendLayout();
            this.Mn_TxBx.SuspendLayout();
            this.Tb_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Pages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicBx_Cover)).BeginInit();
            this.Mn_EntryOps.SuspendLayout();
            this.Mn_rTxBx.SuspendLayout();
            this.Tb_Notes.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.Tb_Browse);
            this.TabControl.Controls.Add(this.Tb_View);
            this.TabControl.Controls.Add(this.Tb_Notes);
            this.TabControl.Location = new System.Drawing.Point(0, 1);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(937, 575);
            this.TabControl.TabIndex = 0;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // Tb_Browse
            // 
            this.Tb_Browse.Controls.Add(this.ChkBx_ShowFav);
            this.Tb_Browse.Controls.Add(this.LV_Entries);
            this.Tb_Browse.Controls.Add(this.Btn_Scan);
            this.Tb_Browse.Controls.Add(this.Btn_Clear);
            this.Tb_Browse.Controls.Add(this.Lbl_Search);
            this.Tb_Browse.Controls.Add(this.TxBx_Search);
            this.Tb_Browse.Location = new System.Drawing.Point(4, 22);
            this.Tb_Browse.Name = "Tb_Browse";
            this.Tb_Browse.Padding = new System.Windows.Forms.Padding(3);
            this.Tb_Browse.Size = new System.Drawing.Size(929, 549);
            this.Tb_Browse.TabIndex = 0;
            this.Tb_Browse.Text = "Browse";
            this.Tb_Browse.UseVisualStyleBackColor = true;
            this.Tb_Browse.Click += new System.EventHandler(this.ClearSelection);
            // 
            // ChkBx_ShowFav
            // 
            this.ChkBx_ShowFav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkBx_ShowFav.AutoSize = true;
            this.ChkBx_ShowFav.Location = new System.Drawing.Point(796, 8);
            this.ChkBx_ShowFav.Name = "ChkBx_ShowFav";
            this.ChkBx_ShowFav.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ChkBx_ShowFav.Size = new System.Drawing.Size(75, 17);
            this.ChkBx_ShowFav.TabIndex = 6;
            this.ChkBx_ShowFav.Text = "Fav\'s Only";
            this.ChkBx_ShowFav.UseVisualStyleBackColor = true;
            this.ChkBx_ShowFav.CheckedChanged += new System.EventHandler(this.ChkBx_ShowFav_CheckedChanged);
            // 
            // LV_Entries
            // 
            this.LV_Entries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_Entries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColArtist,
            this.ColTitle,
            this.ColPages,
            this.ColTags,
            this.ColType});
            this.LV_Entries.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LV_Entries.FullRowSelect = true;
            this.LV_Entries.HideSelection = false;
            this.LV_Entries.Location = new System.Drawing.Point(0, 30);
            this.LV_Entries.MultiSelect = false;
            this.LV_Entries.Name = "LV_Entries";
            this.LV_Entries.Size = new System.Drawing.Size(926, 519);
            this.LV_Entries.TabIndex = 0;
            this.LV_Entries.UseCompatibleStateImageBehavior = false;
            this.LV_Entries.View = System.Windows.Forms.View.Details;
            this.LV_Entries.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LV_Entries_ColumnClick);
            this.LV_Entries.SelectedIndexChanged += new System.EventHandler(this.LV_Entries_SelectedIndexChanged);
            this.LV_Entries.DoubleClick += new System.EventHandler(this.LV_Entries_DoubleClick);
            this.LV_Entries.Resize += new System.EventHandler(this.LV_Entries_Resize);
            // 
            // ColArtist
            // 
            this.ColArtist.Text = "Artist";
            this.ColArtist.Width = 202;
            // 
            // ColTitle
            // 
            this.ColTitle.Text = "Title";
            this.ColTitle.Width = 321;
            // 
            // ColPages
            // 
            this.ColPages.Text = "Pages";
            this.ColPages.Width = 46;
            // 
            // ColTags
            // 
            this.ColTags.Text = "Tags";
            this.ColTags.Width = 261;
            // 
            // ColType
            // 
            this.ColType.Text = "Type";
            this.ColType.Width = 72;
            // 
            // Btn_Scan
            // 
            this.Btn_Scan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Scan.Location = new System.Drawing.Point(877, 5);
            this.Btn_Scan.Name = "Btn_Scan";
            this.Btn_Scan.Size = new System.Drawing.Size(44, 21);
            this.Btn_Scan.TabIndex = 5;
            this.Btn_Scan.Text = "Scan";
            this.Btn_Scan.UseVisualStyleBackColor = true;
            this.Btn_Scan.Click += new System.EventHandler(this.Btn_Scan_Click);
            // 
            // Btn_Clear
            // 
            this.Btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Clear.BackColor = System.Drawing.Color.Red;
            this.Btn_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Clear.Location = new System.Drawing.Point(760, 6);
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
            this.TxBx_Search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxBx_Search.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Search.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxBx_Search.Location = new System.Drawing.Point(59, 6);
            this.TxBx_Search.Name = "TxBx_Search";
            this.TxBx_Search.Size = new System.Drawing.Size(731, 21);
            this.TxBx_Search.TabIndex = 1;
            this.TxBx_Search.TextChanged += new System.EventHandler(this.TxBx_Search_TextChanged);
            // 
            // Mn_TxBx
            // 
            this.Mn_TxBx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnTx_Undo,
            this.toolStripSeparator2,
            this.MnTx_Cut,
            this.MnTx_Copy,
            this.MnTx_Paste,
            this.toolStripSeparator3,
            this.MnTx_SelAll});
            this.Mn_TxBx.Name = "Mn_Context";
            this.Mn_TxBx.Size = new System.Drawing.Size(116, 126);
            // 
            // MnTx_Undo
            // 
            this.MnTx_Undo.Enabled = false;
            this.MnTx_Undo.Name = "MnTx_Undo";
            this.MnTx_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.MnTx_Undo.ShowShortcutKeys = false;
            this.MnTx_Undo.Size = new System.Drawing.Size(115, 22);
            this.MnTx_Undo.Text = "Undo";
            this.MnTx_Undo.Click += new System.EventHandler(this.MnTx_Undo_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(112, 6);
            // 
            // MnTx_Cut
            // 
            this.MnTx_Cut.Name = "MnTx_Cut";
            this.MnTx_Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.MnTx_Cut.ShowShortcutKeys = false;
            this.MnTx_Cut.Size = new System.Drawing.Size(115, 22);
            this.MnTx_Cut.Text = "Cut";
            this.MnTx_Cut.Click += new System.EventHandler(this.MnTx_Cut_Click);
            // 
            // MnTx_Copy
            // 
            this.MnTx_Copy.Name = "MnTx_Copy";
            this.MnTx_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.MnTx_Copy.ShowShortcutKeys = false;
            this.MnTx_Copy.Size = new System.Drawing.Size(115, 22);
            this.MnTx_Copy.Text = "Copy";
            this.MnTx_Copy.Click += new System.EventHandler(this.MnTx_Copy_Click);
            // 
            // MnTx_Paste
            // 
            this.MnTx_Paste.Name = "MnTx_Paste";
            this.MnTx_Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.MnTx_Paste.ShowShortcutKeys = false;
            this.MnTx_Paste.Size = new System.Drawing.Size(115, 22);
            this.MnTx_Paste.Text = "Paste";
            this.MnTx_Paste.Click += new System.EventHandler(this.MnTx_Paste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(112, 6);
            // 
            // MnTx_SelAll
            // 
            this.MnTx_SelAll.Name = "MnTx_SelAll";
            this.MnTx_SelAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.MnTx_SelAll.ShowShortcutKeys = false;
            this.MnTx_SelAll.Size = new System.Drawing.Size(115, 22);
            this.MnTx_SelAll.Text = "Select All";
            this.MnTx_SelAll.Click += new System.EventHandler(this.MnTx_SelAll_Click);
            // 
            // Tb_View
            // 
            this.Tb_View.BackColor = System.Drawing.Color.Transparent;
            this.Tb_View.Controls.Add(this.Btn_Loc);
            this.Tb_View.Controls.Add(this.ChkBx_Fav);
            this.Tb_View.Controls.Add(this.TxBx_Loc);
            this.Tb_View.Controls.Add(this.Lbl_Desc);
            this.Tb_View.Controls.Add(this.CmBx_Type);
            this.Tb_View.Controls.Add(this.Dt_Date);
            this.Tb_View.Controls.Add(this.Nud_Pages);
            this.Tb_View.Controls.Add(this.TxBx_Tags);
            this.Tb_View.Controls.Add(this.TxBx_Artist);
            this.Tb_View.Controls.Add(this.Lbl_Pages);
            this.Tb_View.Controls.Add(this.Lbl_Date);
            this.Tb_View.Controls.Add(this.Lbl_Type);
            this.Tb_View.Controls.Add(this.Lbl_Tags);
            this.Tb_View.Controls.Add(this.Lbl_Artist);
            this.Tb_View.Controls.Add(this.Lbl_Title);
            this.Tb_View.Controls.Add(this.TxBx_Title);
            this.Tb_View.Controls.Add(this.PicBx_Cover);
            this.Tb_View.Controls.Add(this.Mn_EntryOps);
            this.Tb_View.Controls.Add(this.frTxBx_Desc);
            this.Tb_View.Location = new System.Drawing.Point(4, 22);
            this.Tb_View.Name = "Tb_View";
            this.Tb_View.Padding = new System.Windows.Forms.Padding(3);
            this.Tb_View.Size = new System.Drawing.Size(929, 549);
            this.Tb_View.TabIndex = 1;
            this.Tb_View.Text = "View";
            // 
            // Btn_Loc
            // 
            this.Btn_Loc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Loc.Location = new System.Drawing.Point(11, 146);
            this.Btn_Loc.Name = "Btn_Loc";
            this.Btn_Loc.Size = new System.Drawing.Size(35, 21);
            this.Btn_Loc.TabIndex = 23;
            this.Btn_Loc.Text = "Loc";
            this.Btn_Loc.UseVisualStyleBackColor = true;
            this.Btn_Loc.Click += new System.EventHandler(this.Btn_Loc_Click);
            // 
            // ChkBx_Fav
            // 
            this.ChkBx_Fav.AutoSize = true;
            this.ChkBx_Fav.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkBx_Fav.Location = new System.Drawing.Point(448, 181);
            this.ChkBx_Fav.Name = "ChkBx_Fav";
            this.ChkBx_Fav.Size = new System.Drawing.Size(44, 19);
            this.ChkBx_Fav.TabIndex = 20;
            this.ChkBx_Fav.Text = "Fav";
            this.ChkBx_Fav.UseVisualStyleBackColor = true;
            this.ChkBx_Fav.CheckStateChanged += new System.EventHandler(this.ChkBx_Fav_CheckStateChanged);
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
            this.TxBx_Loc.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.TxBx_Loc.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
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
            // CmBx_Type
            // 
            this.CmBx_Type.AutoCompleteCustomSource.AddRange(new string[] {
            "Cosplay",
            "Doujinshi",
            "Ecchi",
            "Game CG",
            "Image Set",
            "Manga",
            "Western"});
            this.CmBx_Type.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CmBx_Type.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.CmBx_Type.ContextMenuStrip = this.Mn_TxBx;
            this.CmBx_Type.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmBx_Type.FormattingEnabled = true;
            this.CmBx_Type.Items.AddRange(new object[] {
            "Cosplay",
            "Doujinshi",
            "Ecchi",
            "Game CG",
            "Image Set",
            "Manga",
            "Western"});
            this.CmBx_Type.Location = new System.Drawing.Point(52, 216);
            this.CmBx_Type.Name = "CmBx_Type";
            this.CmBx_Type.Size = new System.Drawing.Size(142, 21);
            this.CmBx_Type.TabIndex = 5;
            this.CmBx_Type.Text = "Manga";
            this.CmBx_Type.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            // 
            // Dt_Date
            // 
            this.Dt_Date.CustomFormat = "MMMM dd, yyyy";
            this.Dt_Date.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Dt_Date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Dt_Date.Location = new System.Drawing.Point(52, 181);
            this.Dt_Date.MaxDate = new System.DateTime(2100, 1, 1, 0, 0, 0, 0);
            this.Dt_Date.MinDate = new System.DateTime(1972, 4, 12, 0, 0, 0, 0);
            this.Dt_Date.Name = "Dt_Date";
            this.Dt_Date.Size = new System.Drawing.Size(200, 21);
            this.Dt_Date.TabIndex = 4;
            this.Dt_Date.ValueChanged += new System.EventHandler(this.EntryAlt_DtNum);
            // 
            // Nud_Pages
            // 
            this.Nud_Pages.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nud_Pages.Location = new System.Drawing.Point(337, 181);
            this.Nud_Pages.Maximum = new decimal(new int[] {
            2147483646,
            0,
            0,
            0});
            this.Nud_Pages.Name = "Nud_Pages";
            this.Nud_Pages.Size = new System.Drawing.Size(91, 21);
            this.Nud_Pages.TabIndex = 6;
            this.Nud_Pages.ValueChanged += new System.EventHandler(this.EntryAlt_DtNum);
            // 
            // TxBx_Tags
            // 
            this.TxBx_Tags.AllowDrop = true;
            this.TxBx_Tags.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.TxBx_Tags.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Tags.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxBx_Tags.Location = new System.Drawing.Point(52, 111);
            this.TxBx_Tags.Name = "TxBx_Tags";
            this.TxBx_Tags.Size = new System.Drawing.Size(440, 21);
            this.TxBx_Tags.TabIndex = 2;
            this.TxBx_Tags.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.TxBx_Tags.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.TxBx_Tags.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            // 
            // TxBx_Artist
            // 
            this.TxBx_Artist.AllowDrop = true;
            this.TxBx_Artist.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Artist.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxBx_Artist.Location = new System.Drawing.Point(52, 39);
            this.TxBx_Artist.Name = "TxBx_Artist";
            this.TxBx_Artist.Size = new System.Drawing.Size(440, 21);
            this.TxBx_Artist.TabIndex = 0;
            this.TxBx_Artist.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.TxBx_Artist.DragDrop += new System.Windows.Forms.DragEventHandler(this.TxBx_Artist_DragDrop);
            this.TxBx_Artist.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            // 
            // Lbl_Pages
            // 
            this.Lbl_Pages.AutoSize = true;
            this.Lbl_Pages.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Pages.Location = new System.Drawing.Point(290, 182);
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
            // TxBx_Title
            // 
            this.TxBx_Title.AllowDrop = true;
            this.TxBx_Title.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Title.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxBx_Title.Location = new System.Drawing.Point(52, 76);
            this.TxBx_Title.Name = "TxBx_Title";
            this.TxBx_Title.Size = new System.Drawing.Size(440, 21);
            this.TxBx_Title.TabIndex = 1;
            this.TxBx_Title.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            this.TxBx_Title.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropTxBx);
            this.TxBx_Title.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterTxBx);
            // 
            // PicBx_Cover
            // 
            this.PicBx_Cover.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PicBx_Cover.BackColor = System.Drawing.Color.DarkGray;
            this.PicBx_Cover.Location = new System.Drawing.Point(498, 9);
            this.PicBx_Cover.Name = "PicBx_Cover";
            this.PicBx_Cover.Size = new System.Drawing.Size(420, 533);
            this.PicBx_Cover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicBx_Cover.TabIndex = 0;
            this.PicBx_Cover.TabStop = false;
            this.PicBx_Cover.Click += new System.EventHandler(this.PicBx_Cover_Click);
            // 
            // Mn_EntryOps
            // 
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
            this.Mn_EntryOps.Location = new System.Drawing.Point(3, 3);
            this.Mn_EntryOps.Name = "Mn_EntryOps";
            this.Mn_EntryOps.Size = new System.Drawing.Size(153, 27);
            this.Mn_EntryOps.TabIndex = 21;
            this.Mn_EntryOps.Text = "menuStrip1";
            // 
            // MnTS_Menu
            // 
            this.MnTS_Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnTS_SavLoc,
            this.MnTS_OpenDataFolder,
            this.MnTS_Save,
            this.toolStripSeparator1,
            this.MnTS_OpenEntryFolder,
            this.MnTS_DefLoc,
            this.MnTS_CopyTitle,
            this.toolStripSeparator4,
            this.MnTS_GET,
            this.MnTS_About});
            this.MnTS_Menu.Name = "MnTS_Menu";
            this.MnTS_Menu.Size = new System.Drawing.Size(50, 23);
            this.MnTS_Menu.Text = "Menu";
            // 
            // MnTS_SavLoc
            // 
            this.MnTS_SavLoc.Name = "MnTS_SavLoc";
            this.MnTS_SavLoc.Size = new System.Drawing.Size(191, 22);
            this.MnTS_SavLoc.Text = "Change Save Location";
            this.MnTS_SavLoc.Click += new System.EventHandler(this.MnTS_SavLoc_Click);
            // 
            // MnTS_OpenDataFolder
            // 
            this.MnTS_OpenDataFolder.Name = "MnTS_OpenDataFolder";
            this.MnTS_OpenDataFolder.Size = new System.Drawing.Size(191, 22);
            this.MnTS_OpenDataFolder.Text = "Open Database Folder";
            this.MnTS_OpenDataFolder.Click += new System.EventHandler(this.MnTS_OpenDataFolder_Click);
            // 
            // MnTS_Save
            // 
            this.MnTS_Save.Name = "MnTS_Save";
            this.MnTS_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MnTS_Save.ShowShortcutKeys = false;
            this.MnTS_Save.Size = new System.Drawing.Size(191, 22);
            this.MnTS_Save.Text = "Save Database";
            this.MnTS_Save.Click += new System.EventHandler(this.MnTS_Save_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // MnTS_OpenEntryFolder
            // 
            this.MnTS_OpenEntryFolder.Name = "MnTS_OpenEntryFolder";
            this.MnTS_OpenEntryFolder.Size = new System.Drawing.Size(191, 22);
            this.MnTS_OpenEntryFolder.Text = "Open Entry\'s Folder";
            this.MnTS_OpenEntryFolder.Click += new System.EventHandler(this.MnTS_OpenEntryFolder_Click);
            // 
            // MnTS_DefLoc
            // 
            this.MnTS_DefLoc.Name = "MnTS_DefLoc";
            this.MnTS_DefLoc.Size = new System.Drawing.Size(191, 22);
            this.MnTS_DefLoc.Text = "Change Root Folder";
            this.MnTS_DefLoc.Click += new System.EventHandler(this.MnTS_DefLoc_Click);
            // 
            // MnTS_CopyTitle
            // 
            this.MnTS_CopyTitle.Name = "MnTS_CopyTitle";
            this.MnTS_CopyTitle.Size = new System.Drawing.Size(191, 22);
            this.MnTS_CopyTitle.Text = "Copy Formatted Title";
            this.MnTS_CopyTitle.Click += new System.EventHandler(this.MnTS_CopyTitle_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(188, 6);
            // 
            // MnTS_GET
            // 
            this.MnTS_GET.Name = "MnTS_GET";
            this.MnTS_GET.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.MnTS_GET.ShowShortcutKeys = false;
            this.MnTS_GET.Size = new System.Drawing.Size(191, 22);
            this.MnTS_GET.Text = "GET from URL";
            this.MnTS_GET.Click += new System.EventHandler(this.MnTS_GET_Click);
            // 
            // MnTS_About
            // 
            this.MnTS_About.Name = "MnTS_About";
            this.MnTS_About.Size = new System.Drawing.Size(191, 22);
            this.MnTS_About.Text = "About";
            this.MnTS_About.Click += new System.EventHandler(this.MnTS_About_Click);
            // 
            // MnTS_SecretSort
            // 
            this.MnTS_SecretSort.Name = "MnTS_SecretSort";
            this.MnTS_SecretSort.Size = new System.Drawing.Size(6, 23);
            // 
            // MnTS_New
            // 
            this.MnTS_New.Name = "MnTS_New";
            this.MnTS_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.MnTS_New.ShowShortcutKeys = false;
            this.MnTS_New.Size = new System.Drawing.Size(43, 23);
            this.MnTS_New.Text = "New";
            this.MnTS_New.Click += new System.EventHandler(this.MnTS_New_Click);
            // 
            // MnTS_Edit
            // 
            this.MnTS_Edit.Name = "MnTS_Edit";
            this.MnTS_Edit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.MnTS_Edit.ShowShortcutKeys = false;
            this.MnTS_Edit.Size = new System.Drawing.Size(39, 23);
            this.MnTS_Edit.Text = "Edit";
            this.MnTS_Edit.Visible = false;
            this.MnTS_Edit.Click += new System.EventHandler(this.MnTS_Edit_Click);
            // 
            // MnTS_Open
            // 
            this.MnTS_Open.Name = "MnTS_Open";
            this.MnTS_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MnTS_Open.ShowShortcutKeys = false;
            this.MnTS_Open.Size = new System.Drawing.Size(48, 23);
            this.MnTS_Open.Text = "Open";
            this.MnTS_Open.Visible = false;
            this.MnTS_Open.Click += new System.EventHandler(this.MnTS_Open_Click);
            // 
            // MnTS_Del
            // 
            this.MnTS_Del.Name = "MnTS_Del";
            this.MnTS_Del.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.MnTS_Del.ShowShortcutKeys = false;
            this.MnTS_Del.Size = new System.Drawing.Size(52, 23);
            this.MnTS_Del.Text = "Delete";
            this.MnTS_Del.Visible = false;
            this.MnTS_Del.Click += new System.EventHandler(this.MnTS_Delete_Click);
            // 
            // MnTS_Clear
            // 
            this.MnTS_Clear.Name = "MnTS_Clear";
            this.MnTS_Clear.Size = new System.Drawing.Size(46, 23);
            this.MnTS_Clear.Text = "Clear";
            this.MnTS_Clear.Click += new System.EventHandler(this.MnTS_Clear_Click);
            // 
            // frTxBx_Desc
            // 
            this.frTxBx_Desc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.frTxBx_Desc.ContextMenuStrip = this.Mn_rTxBx;
            this.frTxBx_Desc.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frTxBx_Desc.Location = new System.Drawing.Point(8, 278);
            this.frTxBx_Desc.Name = "frTxBx_Desc";
            this.frTxBx_Desc.Size = new System.Drawing.Size(484, 264);
            this.frTxBx_Desc.TabIndex = 22;
            this.frTxBx_Desc.Text = "";
            this.frTxBx_Desc.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.frTxBx_Desc_LinkClicked);
            this.frTxBx_Desc.TextChanged += new System.EventHandler(this.EntryAlt_Text);
            // 
            // Mn_rTxBx
            // 
            this.Mn_rTxBx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnRTx_Undo,
            this.tsSeperate,
            this.MnRTx_Cut,
            this.MnRTx_Copy,
            this.MnRTx_Paste,
            this.tsSeperate2,
            this.MnRTx_SelectAll});
            this.Mn_rTxBx.Name = "Mn_Context";
            this.Mn_rTxBx.Size = new System.Drawing.Size(116, 126);
            // 
            // MnRTx_Undo
            // 
            this.MnRTx_Undo.Enabled = false;
            this.MnRTx_Undo.Name = "MnRTx_Undo";
            this.MnRTx_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.MnRTx_Undo.ShowShortcutKeys = false;
            this.MnRTx_Undo.Size = new System.Drawing.Size(115, 22);
            this.MnRTx_Undo.Text = "Undo";
            this.MnRTx_Undo.Click += new System.EventHandler(this.MnRTx_Undo_Click);
            // 
            // tsSeperate
            // 
            this.tsSeperate.Name = "tsSeperate";
            this.tsSeperate.Size = new System.Drawing.Size(112, 6);
            // 
            // MnRTx_Cut
            // 
            this.MnRTx_Cut.Name = "MnRTx_Cut";
            this.MnRTx_Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.MnRTx_Cut.ShowShortcutKeys = false;
            this.MnRTx_Cut.Size = new System.Drawing.Size(115, 22);
            this.MnRTx_Cut.Text = "Cut";
            this.MnRTx_Cut.Click += new System.EventHandler(this.MnRTx_Cut_Click);
            // 
            // MnRTx_Copy
            // 
            this.MnRTx_Copy.Name = "MnRTx_Copy";
            this.MnRTx_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.MnRTx_Copy.ShowShortcutKeys = false;
            this.MnRTx_Copy.Size = new System.Drawing.Size(115, 22);
            this.MnRTx_Copy.Text = "Copy";
            this.MnRTx_Copy.Click += new System.EventHandler(this.MnRTx_Copy_Click);
            // 
            // MnRTx_Paste
            // 
            this.MnRTx_Paste.Name = "MnRTx_Paste";
            this.MnRTx_Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.MnRTx_Paste.ShowShortcutKeys = false;
            this.MnRTx_Paste.Size = new System.Drawing.Size(115, 22);
            this.MnRTx_Paste.Text = "Paste";
            this.MnRTx_Paste.Click += new System.EventHandler(this.MnRTx_Paste_Click);
            // 
            // tsSeperate2
            // 
            this.tsSeperate2.Name = "tsSeperate2";
            this.tsSeperate2.Size = new System.Drawing.Size(112, 6);
            // 
            // MnRTx_SelectAll
            // 
            this.MnRTx_SelectAll.Name = "MnRTx_SelectAll";
            this.MnRTx_SelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.MnRTx_SelectAll.ShowShortcutKeys = false;
            this.MnRTx_SelectAll.Size = new System.Drawing.Size(115, 22);
            this.MnRTx_SelectAll.Text = "Select All";
            this.MnRTx_SelectAll.Click += new System.EventHandler(this.MnRTx_SelectAll_Click);
            // 
            // Tb_Notes
            // 
            this.Tb_Notes.Controls.Add(this.frTxBx_Notes);
            this.Tb_Notes.Location = new System.Drawing.Point(4, 22);
            this.Tb_Notes.Name = "Tb_Notes";
            this.Tb_Notes.Size = new System.Drawing.Size(929, 549);
            this.Tb_Notes.TabIndex = 2;
            this.Tb_Notes.Text = "Notes";
            this.Tb_Notes.UseVisualStyleBackColor = true;
            // 
            // frTxBx_Notes
            // 
            this.frTxBx_Notes.AcceptsTab = true;
            this.frTxBx_Notes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.frTxBx_Notes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.frTxBx_Notes.ContextMenuStrip = this.Mn_rTxBx;
            this.frTxBx_Notes.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frTxBx_Notes.ForeColor = System.Drawing.SystemColors.Window;
            this.frTxBx_Notes.Location = new System.Drawing.Point(3, 3);
            this.frTxBx_Notes.Name = "frTxBx_Notes";
            this.frTxBx_Notes.Size = new System.Drawing.Size(923, 543);
            this.frTxBx_Notes.TabIndex = 1;
            this.frTxBx_Notes.Text = "";
            this.frTxBx_Notes.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.frTxBx_Notes_LinkClicked);
            this.frTxBx_Notes.TextChanged += new System.EventHandler(this.frTxBx_Notes_TextChanged);
            // 
            // Delay
            // 
            this.Delay.Interval = 300;
            this.Delay.Tick += new System.EventHandler(this.Delay_Tick);
            // 
            // Main
            // 
            this.AcceptButton = this.Btn_Clear;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(934, 575);
            this.Controls.Add(this.TabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.Mn_EntryOps;
            this.MinimumSize = new System.Drawing.Size(750, 150);
            this.Name = "Main";
            this.Text = "Manga Organizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.TabControl.ResumeLayout(false);
            this.Tb_Browse.ResumeLayout(false);
            this.Tb_Browse.PerformLayout();
            this.Mn_TxBx.ResumeLayout(false);
            this.Tb_View.ResumeLayout(false);
            this.Tb_View.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_Pages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicBx_Cover)).EndInit();
            this.Mn_EntryOps.ResumeLayout(false);
            this.Mn_EntryOps.PerformLayout();
            this.Mn_rTxBx.ResumeLayout(false);
            this.Tb_Notes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage Tb_Browse;
        private System.Windows.Forms.TabPage Tb_View;
        private System.Windows.Forms.TextBox TxBx_Tags;
        private System.Windows.Forms.TextBox TxBx_Artist;
        private System.Windows.Forms.Label Lbl_Pages;
        private System.Windows.Forms.Label Lbl_Date;
        private System.Windows.Forms.Label Lbl_Type;
        private System.Windows.Forms.Label Lbl_Tags;
        private System.Windows.Forms.Label Lbl_Artist;
        private System.Windows.Forms.Label Lbl_Title;
        private System.Windows.Forms.TextBox TxBx_Title;
        private System.Windows.Forms.PictureBox PicBx_Cover;
        private System.Windows.Forms.Label Lbl_Search;
        private System.Windows.Forms.TextBox TxBx_Search;
        private System.Windows.Forms.Label Lbl_Desc;
        private System.Windows.Forms.ComboBox CmBx_Type;
        private System.Windows.Forms.DateTimePicker Dt_Date;
        private System.Windows.Forms.NumericUpDown Nud_Pages;
        private System.Windows.Forms.TextBox TxBx_Loc;
        private System.Windows.Forms.ContextMenuStrip Mn_rTxBx;
        private System.Windows.Forms.ToolStripMenuItem MnRTx_Undo;
        private System.Windows.Forms.ToolStripSeparator tsSeperate;
        private System.Windows.Forms.ToolStripMenuItem MnRTx_Cut;
        private System.Windows.Forms.ToolStripMenuItem MnRTx_Copy;
        private System.Windows.Forms.ToolStripMenuItem MnRTx_Paste;
        private System.Windows.Forms.ToolStripSeparator tsSeperate2;
        private System.Windows.Forms.ToolStripMenuItem MnRTx_SelectAll;
        private System.Windows.Forms.TabPage Tb_Notes;
        private System.Windows.Forms.Button Btn_Clear;
        private System.Windows.Forms.CheckBox ChkBx_Fav;
        private System.Windows.Forms.MenuStrip Mn_EntryOps;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Menu;
        private System.Windows.Forms.ToolStripMenuItem MnTS_CopyTitle;
        private System.Windows.Forms.ToolStripMenuItem MnTS_DefLoc;
        private System.Windows.Forms.ToolStripMenuItem MnTS_New;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Edit;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Del;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Open;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MnTS_Save;
        private System.Windows.Forms.ToolStripMenuItem MnTS_OpenDataFolder;
        private System.Windows.Forms.ToolStripSeparator MnTS_SecretSort;
        private System.Windows.Forms.ToolStripMenuItem MnTS_OpenEntryFolder;
        private FixedRichTextBox frTxBx_Notes;
        private FixedRichTextBox frTxBx_Desc;
        private System.Windows.Forms.ToolStripMenuItem MnTS_SavLoc;
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
        private System.Windows.Forms.ToolStripMenuItem MnTS_GET;
        private System.Windows.Forms.ContextMenuStrip Mn_TxBx;
    }
}

