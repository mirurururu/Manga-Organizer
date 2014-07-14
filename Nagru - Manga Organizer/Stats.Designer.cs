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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chtTags = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ChkBx_FavsOnly = new System.Windows.Forms.CheckBox();
            this.pnlView0 = new System.Windows.Forms.Panel();
            this.BtnSwitch = new System.Windows.Forms.Button();
            this.pnlView1 = new System.Windows.Forms.Panel();
            this.lvStats = new Nagru___Manga_Organizer.ListViewNF();
            this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblChartNote = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chtTags)).BeginInit();
            this.pnlView0.SuspendLayout();
            this.pnlView1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chtTags
            // 
            chartArea2.Name = "ChartArea1";
            this.chtTags.ChartAreas.Add(chartArea2);
            this.chtTags.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chtTags.Legends.Add(legend2);
            this.chtTags.Location = new System.Drawing.Point(0, 0);
            this.chtTags.Name = "chtTags";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series2.Legend = "Legend1";
            series2.Name = "Tags";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            this.chtTags.Series.Add(series2);
            this.chtTags.Size = new System.Drawing.Size(647, 571);
            this.chtTags.TabIndex = 2;
            this.chtTags.Text = "chart1";
            this.chtTags.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            // 
            // ChkBx_FavsOnly
            // 
            this.ChkBx_FavsOnly.Appearance = System.Windows.Forms.Appearance.Button;
            this.ChkBx_FavsOnly.AutoSize = true;
            this.ChkBx_FavsOnly.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ChkBx_FavsOnly.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ChkBx_FavsOnly.Location = new System.Drawing.Point(12, 12);
            this.ChkBx_FavsOnly.Name = "ChkBx_FavsOnly";
            this.ChkBx_FavsOnly.Size = new System.Drawing.Size(64, 23);
            this.ChkBx_FavsOnly.TabIndex = 4;
            this.ChkBx_FavsOnly.Text = "Favs Only";
            this.ChkBx_FavsOnly.UseVisualStyleBackColor = false;
            this.ChkBx_FavsOnly.CheckedChanged += new System.EventHandler(this.ChkBx_FavsOnly0_CheckedChanged);
            // 
            // pnlView0
            // 
            this.pnlView0.Controls.Add(this.lblChartNote);
            this.pnlView0.Controls.Add(this.BtnSwitch);
            this.pnlView0.Controls.Add(this.ChkBx_FavsOnly);
            this.pnlView0.Controls.Add(this.chtTags);
            this.pnlView0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlView0.Location = new System.Drawing.Point(0, 0);
            this.pnlView0.Name = "pnlView0";
            this.pnlView0.Size = new System.Drawing.Size(647, 571);
            this.pnlView0.TabIndex = 5;
            // 
            // BtnSwitch
            // 
            this.BtnSwitch.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.BtnSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSwitch.Location = new System.Drawing.Point(12, 41);
            this.BtnSwitch.Name = "BtnSwitch";
            this.BtnSwitch.Size = new System.Drawing.Size(64, 23);
            this.BtnSwitch.TabIndex = 30;
            this.BtnSwitch.Text = "Switch";
            this.BtnSwitch.UseVisualStyleBackColor = false;
            this.BtnSwitch.Click += new System.EventHandler(this.BtnSwitch_Click);
            // 
            // pnlView1
            // 
            this.pnlView1.Controls.Add(this.lvStats);
            this.pnlView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlView1.Location = new System.Drawing.Point(0, 0);
            this.pnlView1.Name = "pnlView1";
            this.pnlView1.Size = new System.Drawing.Size(647, 571);
            this.pnlView1.TabIndex = 6;
            // 
            // lvStats
            // 
            this.lvStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvStats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTag,
            this.colCount,
            this.colPer});
            this.lvStats.FullRowSelect = true;
            this.lvStats.Location = new System.Drawing.Point(12, 41);
            this.lvStats.MultiSelect = false;
            this.lvStats.Name = "lvStats";
            this.lvStats.Size = new System.Drawing.Size(623, 518);
            this.lvStats.TabIndex = 0;
            this.lvStats.UseCompatibleStateImageBehavior = false;
            this.lvStats.View = System.Windows.Forms.View.Details;
            this.lvStats.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lvStats_ColumnWidthChanging);
            this.lvStats.Resize += new System.EventHandler(this.lvStats_Resize);
            // 
            // colTag
            // 
            this.colTag.Text = "Tag";
            this.colTag.Width = 169;
            // 
            // colCount
            // 
            this.colCount.Text = "Count";
            this.colCount.Width = 79;
            // 
            // colPer
            // 
            this.colPer.Text = "Percent";
            // 
            // lblChartNote
            // 
            this.lblChartNote.Location = new System.Drawing.Point(12, 545);
            this.lblChartNote.Name = "lblChartNote";
            this.lblChartNote.Size = new System.Drawing.Size(623, 26);
            this.lblChartNote.TabIndex = 31;
            this.lblChartNote.Text = "Tags with less than 5% frequency are excluded here";
            this.lblChartNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Stats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(647, 571);
            this.Controls.Add(this.pnlView0);
            this.Controls.Add(this.pnlView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "Stats";
            this.ShowIcon = false;
            this.Text = "Stats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Stats_FormClosing);
            this.Load += new System.EventHandler(this.Stats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chtTags)).EndInit();
            this.pnlView0.ResumeLayout(false);
            this.pnlView0.PerformLayout();
            this.pnlView1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chtTags;
        private System.Windows.Forms.CheckBox ChkBx_FavsOnly;
        private System.Windows.Forms.Panel pnlView0;
        private System.Windows.Forms.Panel pnlView1;
        private ListViewNF lvStats;
        private System.Windows.Forms.Button BtnSwitch;
        private System.Windows.Forms.ColumnHeader colTag;
        private System.Windows.Forms.ColumnHeader colCount;
        private System.Windows.Forms.ColumnHeader colPer;
        private System.Windows.Forms.Label lblChartNote;
    }
}