namespace Nagru___Manga_Organizer
{
    partial class Stats
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
            this.LV_Stats = new Nagru___Manga_Organizer.ListViewNF();
            this.colTags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // LV_Stats
            // 
            this.LV_Stats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTags,
            this.colCount,
            this.colPer});
            this.LV_Stats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_Stats.FullRowSelect = true;
            this.LV_Stats.GridLines = true;
            this.LV_Stats.Location = new System.Drawing.Point(0, 0);
            this.LV_Stats.MultiSelect = false;
            this.LV_Stats.Name = "LV_Stats";
            this.LV_Stats.Size = new System.Drawing.Size(217, 387);
            this.LV_Stats.TabIndex = 0;
            this.LV_Stats.UseCompatibleStateImageBehavior = false;
            this.LV_Stats.View = System.Windows.Forms.View.Details;
            this.LV_Stats.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LV_Stats_ColumnClick);
            this.LV_Stats.Resize += new System.EventHandler(this.LV_Stats_Resize);
            // 
            // colTags
            // 
            this.colTags.Text = "Tag";
            this.colTags.Width = 93;
            // 
            // colCount
            // 
            this.colCount.Text = "Count";
            // 
            // colPer
            // 
            this.colPer.Text = "Percent";
            // 
            // Stats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 387);
            this.Controls.Add(this.LV_Stats);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Stats";
            this.Text = "Stats";
            this.Load += new System.EventHandler(this.Stats_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNF LV_Stats;
        private System.Windows.Forms.ColumnHeader colTags;
        private System.Windows.Forms.ColumnHeader colCount;
        private System.Windows.Forms.ColumnHeader colPer;
    }
}