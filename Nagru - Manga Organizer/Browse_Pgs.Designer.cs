namespace Nagru___Manga_Organizer
{
    partial class BrowseTo
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
            this.LV_Pages = new Nagru___Manga_Organizer.ListViewNF();
            this.Col_Page = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // LV_Pages
            // 
            this.LV_Pages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Col_Page});
            this.LV_Pages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_Pages.FullRowSelect = true;
            this.LV_Pages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.LV_Pages.HideSelection = false;
            this.LV_Pages.Location = new System.Drawing.Point(0, 0);
            this.LV_Pages.Name = "LV_Pages";
            this.LV_Pages.Size = new System.Drawing.Size(184, 216);
            this.LV_Pages.TabIndex = 0;
            this.LV_Pages.UseCompatibleStateImageBehavior = false;
            this.LV_Pages.View = System.Windows.Forms.View.Details;
            this.LV_Pages.Click += new System.EventHandler(this.LV_Pages_Click);
            this.LV_Pages.DoubleClick += new System.EventHandler(this.LV_Pages_DoubleClick);
            this.LV_Pages.Resize += new System.EventHandler(this.LV_Pages_Resize);
            // 
            // Col_Page
            // 
            this.Col_Page.Text = "Page";
            this.Col_Page.Width = 400;
            // 
            // BrowseTo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 216);
            this.Controls.Add(this.LV_Pages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(150, 100);
            this.Name = "BrowseTo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select pages: ";
            this.Load += new System.EventHandler(this.Browse_GoTo_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrowseTo_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNF LV_Pages;
        private System.Windows.Forms.ColumnHeader Col_Page;
    }
}