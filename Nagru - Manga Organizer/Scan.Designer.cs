namespace Nagru___Manga_Organizer
{
    partial class Scan
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
            this.Btn_Add = new System.Windows.Forms.Button();
            this.TxBx_Loc = new System.Windows.Forms.TextBox();
            this.Mn_TxBx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnTx_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_SelAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Btn_Ignore = new System.Windows.Forms.Button();
            this.ChkBx_All = new System.Windows.Forms.CheckBox();
            this.LV_Found = new Nagru___Manga_Organizer.ListViewNF();
            this.ColArtist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColPages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColPos = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Mn_TxBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_Add
            // 
            this.Btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Add.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Add.Location = new System.Drawing.Point(572, 8);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(72, 23);
            this.Btn_Add.TabIndex = 2;
            this.Btn_Add.Text = "Add Item(s)";
            this.Btn_Add.UseVisualStyleBackColor = false;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // TxBx_Loc
            // 
            this.TxBx_Loc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxBx_Loc.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Loc.Location = new System.Drawing.Point(12, 10);
            this.TxBx_Loc.Name = "TxBx_Loc";
            this.TxBx_Loc.Size = new System.Drawing.Size(481, 20);
            this.TxBx_Loc.TabIndex = 4;
            this.TxBx_Loc.Click += new System.EventHandler(this.TxBx_Loc_Click);
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
            this.MnTx_Undo.Name = "MnTx_Undo";
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
            this.MnTx_Cut.ShowShortcutKeys = false;
            this.MnTx_Cut.Size = new System.Drawing.Size(115, 22);
            this.MnTx_Cut.Text = "Cut";
            this.MnTx_Cut.Click += new System.EventHandler(this.MnTx_Cut_Click);
            // 
            // MnTx_Copy
            // 
            this.MnTx_Copy.Name = "MnTx_Copy";
            this.MnTx_Copy.ShowShortcutKeys = false;
            this.MnTx_Copy.Size = new System.Drawing.Size(115, 22);
            this.MnTx_Copy.Text = "Copy";
            this.MnTx_Copy.Click += new System.EventHandler(this.MnTx_Copy_Click);
            // 
            // MnTx_Paste
            // 
            this.MnTx_Paste.Name = "MnTx_Paste";
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
            this.MnTx_SelAll.ShowShortcutKeys = false;
            this.MnTx_SelAll.Size = new System.Drawing.Size(115, 22);
            this.MnTx_SelAll.Text = "Select All";
            this.MnTx_SelAll.Click += new System.EventHandler(this.MnTx_SelAll_Click);
            // 
            // Btn_Ignore
            // 
            this.Btn_Ignore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Ignore.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Ignore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Ignore.Location = new System.Drawing.Point(650, 8);
            this.Btn_Ignore.Name = "Btn_Ignore";
            this.Btn_Ignore.Size = new System.Drawing.Size(79, 23);
            this.Btn_Ignore.TabIndex = 25;
            this.Btn_Ignore.Text = "Ignore Item(s)";
            this.Btn_Ignore.UseVisualStyleBackColor = false;
            this.Btn_Ignore.Click += new System.EventHandler(this.Btn_Ignore_Click);
            // 
            // ChkBx_All
            // 
            this.ChkBx_All.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkBx_All.Appearance = System.Windows.Forms.Appearance.Button;
            this.ChkBx_All.AutoSize = true;
            this.ChkBx_All.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ChkBx_All.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ChkBx_All.Location = new System.Drawing.Point(508, 8);
            this.ChkBx_All.Name = "ChkBx_All";
            this.ChkBx_All.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ChkBx_All.Size = new System.Drawing.Size(58, 23);
            this.ChkBx_All.TabIndex = 26;
            this.ChkBx_All.Text = "Show All";
            this.ChkBx_All.UseVisualStyleBackColor = false;
            this.ChkBx_All.CheckedChanged += new System.EventHandler(this.ChkBx_All_CheckedChanged);
            // 
            // LV_Found
            // 
            this.LV_Found.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_Found.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColArtist,
            this.ColTitle,
            this.ColPages,
            this.ColPos});
            this.LV_Found.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LV_Found.FullRowSelect = true;
            this.LV_Found.HideSelection = false;
            this.LV_Found.Location = new System.Drawing.Point(0, 36);
            this.LV_Found.Name = "LV_Found";
            this.LV_Found.Size = new System.Drawing.Size(741, 307);
            this.LV_Found.TabIndex = 7;
            this.LV_Found.UseCompatibleStateImageBehavior = false;
            this.LV_Found.View = System.Windows.Forms.View.Details;
            this.LV_Found.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LV_Found_ColumnClick);
            this.LV_Found.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.LV_Found_ColumnWidthChanging);
            this.LV_Found.SelectedIndexChanged += new System.EventHandler(this.LV_Found_SelectedIndexChanged);
            this.LV_Found.DoubleClick += new System.EventHandler(this.LV_Found_DoubleClick);
            this.LV_Found.MouseHover += new System.EventHandler(this.LV_Found_MouseHover);
            this.LV_Found.Resize += new System.EventHandler(this.LV_Found_Resize);
            // 
            // ColArtist
            // 
            this.ColArtist.Text = "Artist";
            this.ColArtist.Width = 256;
            // 
            // ColTitle
            // 
            this.ColTitle.Text = "Title";
            this.ColTitle.Width = 386;
            // 
            // ColPages
            // 
            this.ColPages.Text = "Pages";
            this.ColPages.Width = 67;
            // 
            // ColPos
            // 
            this.ColPos.Width = 0;
            // 
            // Scan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(741, 343);
            this.Controls.Add(this.ChkBx_All);
            this.Controls.Add(this.Btn_Ignore);
            this.Controls.Add(this.LV_Found);
            this.Controls.Add(this.TxBx_Loc);
            this.Controls.Add(this.Btn_Add);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(300, 150);
            this.Name = "Scan";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Scan";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Scan_FormClosing);
            this.Load += new System.EventHandler(this.Scan_Load);
            this.Mn_TxBx.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.TextBox TxBx_Loc;
        private ListViewNF LV_Found;
        private System.Windows.Forms.ColumnHeader ColArtist;
        private System.Windows.Forms.ColumnHeader ColTitle;
        private System.Windows.Forms.ColumnHeader ColPages;
        private System.Windows.Forms.Button Btn_Ignore;
        private System.Windows.Forms.CheckBox ChkBx_All;
        private System.Windows.Forms.ContextMenuStrip Mn_TxBx;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Undo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Cut;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Copy;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Paste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem MnTx_SelAll;
        private System.Windows.Forms.ColumnHeader ColPos;
    }
}