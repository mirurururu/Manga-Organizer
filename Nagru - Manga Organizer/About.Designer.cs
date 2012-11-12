namespace Nagru___Manga_Organizer
{
    partial class About
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
            this.Lbl_P1 = new System.Windows.Forms.Label();
            this.LnkLbl_Gpl = new System.Windows.Forms.LinkLabel();
            this.Lbl_P2 = new System.Windows.Forms.Label();
            this.LnkLbl_Git = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Lbl_P1
            // 
            this.Lbl_P1.Location = new System.Drawing.Point(12, 20);
            this.Lbl_P1.Name = "Lbl_P1";
            this.Lbl_P1.Size = new System.Drawing.Size(371, 196);
            this.Lbl_P1.TabIndex = 0;
            this.Lbl_P1.Text = "<Error>";
            this.Lbl_P1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LnkLbl_Gpl
            // 
            this.LnkLbl_Gpl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LnkLbl_Gpl.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.LnkLbl_Gpl.Location = new System.Drawing.Point(12, 220);
            this.LnkLbl_Gpl.Name = "LnkLbl_Gpl";
            this.LnkLbl_Gpl.Size = new System.Drawing.Size(371, 23);
            this.LnkLbl_Gpl.TabIndex = 1;
            this.LnkLbl_Gpl.TabStop = true;
            this.LnkLbl_Gpl.Text = "http://www.gnu.org/licenses/gpl.html";
            this.LnkLbl_Gpl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LnkLbl_Gpl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            // 
            // Lbl_P2
            // 
            this.Lbl_P2.Location = new System.Drawing.Point(12, 252);
            this.Lbl_P2.Name = "Lbl_P2";
            this.Lbl_P2.Size = new System.Drawing.Size(371, 17);
            this.Lbl_P2.TabIndex = 2;
            this.Lbl_P2.Text = "<Error>";
            this.Lbl_P2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LnkLbl_Git
            // 
            this.LnkLbl_Git.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LnkLbl_Git.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.LnkLbl_Git.Location = new System.Drawing.Point(12, 269);
            this.LnkLbl_Git.Name = "LnkLbl_Git";
            this.LnkLbl_Git.Size = new System.Drawing.Size(371, 23);
            this.LnkLbl_Git.TabIndex = 3;
            this.LnkLbl_Git.TabStop = true;
            this.LnkLbl_Git.Text = "http://nagru.github.com/Manga-Organizer";
            this.LnkLbl_Git.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LnkLbl_Git.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 310);
            this.Controls.Add(this.LnkLbl_Git);
            this.Controls.Add(this.Lbl_P2);
            this.Controls.Add(this.LnkLbl_Gpl);
            this.Controls.Add(this.Lbl_P1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Text = "About <Error>";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Lbl_P1;
        private System.Windows.Forms.LinkLabel LnkLbl_Gpl;
        private System.Windows.Forms.Label Lbl_P2;
        private System.Windows.Forms.LinkLabel LnkLbl_Git;


    }
}