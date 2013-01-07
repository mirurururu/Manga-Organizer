namespace Nagru___Manga_Organizer
{
    partial class GetUrl
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
            this.TxBx_Url = new System.Windows.Forms.TextBox();
            this.Mn_TxBx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MnTx_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MnTx_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MnTx_SelAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Btn_Get = new System.Windows.Forms.Button();
            this.Mn_TxBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // TxBx_Url
            // 
            this.TxBx_Url.ContextMenuStrip = this.Mn_TxBx;
            this.TxBx_Url.Location = new System.Drawing.Point(12, 12);
            this.TxBx_Url.Name = "TxBx_Url";
            this.TxBx_Url.Size = new System.Drawing.Size(288, 20);
            this.TxBx_Url.TabIndex = 0;
            this.TxBx_Url.Text = "Input EH gallery page URL...";
            this.TxBx_Url.Click += new System.EventHandler(this.TxBx_Url_Click);
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
            this.Mn_TxBx.Size = new System.Drawing.Size(153, 148);
            // 
            // MnTx_Undo
            // 
            this.MnTx_Undo.Name = "MnTx_Undo";
            this.MnTx_Undo.ShowShortcutKeys = false;
            this.MnTx_Undo.Size = new System.Drawing.Size(152, 22);
            this.MnTx_Undo.Text = "Undo";
            this.MnTx_Undo.Click += new System.EventHandler(this.MnTx_Undo_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // MnTx_Cut
            // 
            this.MnTx_Cut.Name = "MnTx_Cut";
            this.MnTx_Cut.ShowShortcutKeys = false;
            this.MnTx_Cut.Size = new System.Drawing.Size(152, 22);
            this.MnTx_Cut.Text = "Cut";
            this.MnTx_Cut.Click += new System.EventHandler(this.MnTx_Cut_Click);
            // 
            // MnTx_Copy
            // 
            this.MnTx_Copy.Name = "MnTx_Copy";
            this.MnTx_Copy.ShowShortcutKeys = false;
            this.MnTx_Copy.Size = new System.Drawing.Size(152, 22);
            this.MnTx_Copy.Text = "Copy";
            this.MnTx_Copy.Click += new System.EventHandler(this.MnTx_Copy_Click);
            // 
            // MnTx_Paste
            // 
            this.MnTx_Paste.Name = "MnTx_Paste";
            this.MnTx_Paste.ShowShortcutKeys = false;
            this.MnTx_Paste.Size = new System.Drawing.Size(152, 22);
            this.MnTx_Paste.Text = "Paste";
            this.MnTx_Paste.Click += new System.EventHandler(this.MnTx_Paste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // MnTx_SelAll
            // 
            this.MnTx_SelAll.Name = "MnTx_SelAll";
            this.MnTx_SelAll.ShowShortcutKeys = false;
            this.MnTx_SelAll.Size = new System.Drawing.Size(152, 22);
            this.MnTx_SelAll.Text = "Select All";
            this.MnTx_SelAll.Click += new System.EventHandler(this.MnTx_SelAll_Click);
            // 
            // Btn_Get
            // 
            this.Btn_Get.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_Get.Location = new System.Drawing.Point(306, 10);
            this.Btn_Get.Name = "Btn_Get";
            this.Btn_Get.Size = new System.Drawing.Size(75, 23);
            this.Btn_Get.TabIndex = 1;
            this.Btn_Get.Text = "GET";
            this.Btn_Get.UseVisualStyleBackColor = true;
            this.Btn_Get.Click += new System.EventHandler(this.Btn_Get_Click);
            // 
            // GetUrl
            // 
            this.AcceptButton = this.Btn_Get;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 42);
            this.Controls.Add(this.TxBx_Url);
            this.Controls.Add(this.Btn_Get);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GetUrl";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Grab Data";
            this.TopMost = true;
            this.Mn_TxBx.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxBx_Url;
        private System.Windows.Forms.Button Btn_Get;
        private System.Windows.Forms.ContextMenuStrip Mn_TxBx;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Undo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Cut;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Copy;
        private System.Windows.Forms.ToolStripMenuItem MnTx_Paste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem MnTx_SelAll;
    }
}