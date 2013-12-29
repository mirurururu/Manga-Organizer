namespace Nagru___Manga_Organizer
{
    partial class Explorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Explorer));
            this.scTop = new System.Windows.Forms.SplitContainer();
            this.tvDir = new System.Windows.Forms.TreeView();
            this.imgFiles = new System.Windows.Forms.ImageList(this.components);
            this.lvCurr = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.scBottom = new System.Windows.Forms.SplitContainer();
            this.TxBx_Path = new System.Windows.Forms.TextBox();
            this.lbl_Path = new System.Windows.Forms.Label();
            this.CmbBx_Filters = new System.Windows.Forms.ComboBox();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_Open = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scTop)).BeginInit();
            this.scTop.Panel1.SuspendLayout();
            this.scTop.Panel2.SuspendLayout();
            this.scTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scBottom)).BeginInit();
            this.scBottom.Panel1.SuspendLayout();
            this.scBottom.Panel2.SuspendLayout();
            this.scBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // scTop
            // 
            this.scTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scTop.Location = new System.Drawing.Point(0, 0);
            this.scTop.Name = "scTop";
            // 
            // scTop.Panel1
            // 
            this.scTop.Panel1.Controls.Add(this.tvDir);
            // 
            // scTop.Panel2
            // 
            this.scTop.Panel2.Controls.Add(this.lvCurr);
            this.scTop.Size = new System.Drawing.Size(684, 286);
            this.scTop.SplitterDistance = 182;
            this.scTop.TabIndex = 0;
            // 
            // tvDir
            // 
            this.tvDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDir.ImageIndex = 0;
            this.tvDir.ImageList = this.imgFiles;
            this.tvDir.Location = new System.Drawing.Point(0, 0);
            this.tvDir.Name = "tvDir";
            this.tvDir.SelectedImageIndex = 0;
            this.tvDir.Size = new System.Drawing.Size(182, 286);
            this.tvDir.TabIndex = 0;
            this.tvDir.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvDir_NodeMouseClick);
            this.tvDir.MouseHover += new System.EventHandler(this.tvDir_MouseHover);
            // 
            // imgFiles
            // 
            this.imgFiles.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgFiles.ImageStream")));
            this.imgFiles.TransparentColor = System.Drawing.Color.Transparent;
            this.imgFiles.Images.SetKeyName(0, "Folder.ico");
            this.imgFiles.Images.SetKeyName(1, "Zip.ico");
            // 
            // lvCurr
            // 
            this.lvCurr.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colMod});
            this.lvCurr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvCurr.FullRowSelect = true;
            this.lvCurr.Location = new System.Drawing.Point(0, 0);
            this.lvCurr.MultiSelect = false;
            this.lvCurr.Name = "lvCurr";
            this.lvCurr.Size = new System.Drawing.Size(498, 286);
            this.lvCurr.SmallImageList = this.imgFiles;
            this.lvCurr.TabIndex = 0;
            this.lvCurr.UseCompatibleStateImageBehavior = false;
            this.lvCurr.View = System.Windows.Forms.View.Details;
            this.lvCurr.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvCurr_ItemSelectionChanged);
            this.lvCurr.DoubleClick += new System.EventHandler(this.lvCurr_DoubleClick);
            this.lvCurr.MouseHover += new System.EventHandler(this.lvCurr_MouseHover);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 92;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 88;
            // 
            // colMod
            // 
            this.colMod.Text = "Last Modified";
            this.colMod.Width = 123;
            // 
            // scBottom
            // 
            this.scBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scBottom.Location = new System.Drawing.Point(0, 0);
            this.scBottom.Name = "scBottom";
            this.scBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scBottom.Panel1
            // 
            this.scBottom.Panel1.Controls.Add(this.scTop);
            // 
            // scBottom.Panel2
            // 
            this.scBottom.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.scBottom.Panel2.Controls.Add(this.TxBx_Path);
            this.scBottom.Panel2.Controls.Add(this.lbl_Path);
            this.scBottom.Panel2.Controls.Add(this.CmbBx_Filters);
            this.scBottom.Panel2.Controls.Add(this.Btn_Cancel);
            this.scBottom.Panel2.Controls.Add(this.Btn_Open);
            this.scBottom.Size = new System.Drawing.Size(684, 362);
            this.scBottom.SplitterDistance = 286;
            this.scBottom.TabIndex = 1;
            // 
            // TxBx_Path
            // 
            this.TxBx_Path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxBx_Path.Location = new System.Drawing.Point(72, 10);
            this.TxBx_Path.Name = "TxBx_Path";
            this.TxBx_Path.Size = new System.Drawing.Size(438, 20);
            this.TxBx_Path.TabIndex = 5;
            // 
            // lbl_Path
            // 
            this.lbl_Path.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_Path.AutoSize = true;
            this.lbl_Path.Location = new System.Drawing.Point(12, 13);
            this.lbl_Path.Name = "lbl_Path";
            this.lbl_Path.Size = new System.Drawing.Size(54, 13);
            this.lbl_Path.TabIndex = 4;
            this.lbl_Path.Text = "File Path: ";
            // 
            // CmbBx_Filters
            // 
            this.CmbBx_Filters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbBx_Filters.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.CmbBx_Filters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbBx_Filters.FormattingEnabled = true;
            this.CmbBx_Filters.Location = new System.Drawing.Point(516, 10);
            this.CmbBx_Filters.Name = "CmbBx_Filters";
            this.CmbBx_Filters.Size = new System.Drawing.Size(156, 21);
            this.CmbBx_Filters.TabIndex = 3;
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Cancel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Cancel.Location = new System.Drawing.Point(597, 37);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Btn_Cancel.TabIndex = 2;
            this.Btn_Cancel.Text = "Cancel";
            this.Btn_Cancel.UseVisualStyleBackColor = false;
            // 
            // Btn_Open
            // 
            this.Btn_Open.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Open.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Open.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Btn_Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Open.Location = new System.Drawing.Point(516, 37);
            this.Btn_Open.Name = "Btn_Open";
            this.Btn_Open.Size = new System.Drawing.Size(75, 23);
            this.Btn_Open.TabIndex = 1;
            this.Btn_Open.Text = "Open";
            this.Btn_Open.UseVisualStyleBackColor = false;
            this.Btn_Open.Click += new System.EventHandler(this.Btn_Open_Click);
            // 
            // Explorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 362);
            this.Controls.Add(this.scBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Explorer";
            this.Text = "Explorer";
            this.Load += new System.EventHandler(this.Explorer_Load);
            this.scTop.Panel1.ResumeLayout(false);
            this.scTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTop)).EndInit();
            this.scTop.ResumeLayout(false);
            this.scBottom.Panel1.ResumeLayout(false);
            this.scBottom.Panel2.ResumeLayout(false);
            this.scBottom.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scBottom)).EndInit();
            this.scBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scTop;
        private System.Windows.Forms.TreeView tvDir;
        private System.Windows.Forms.ImageList imgFiles;
        private System.Windows.Forms.ListView lvCurr;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colMod;
        private System.Windows.Forms.SplitContainer scBottom;
        private System.Windows.Forms.ComboBox CmbBx_Filters;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_Open;
        private System.Windows.Forms.Label lbl_Path;
        private System.Windows.Forms.TextBox TxBx_Path;
    }
}