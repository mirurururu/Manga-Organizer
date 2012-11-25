namespace Nagru___Manga_Organizer
{
    partial class Fullscreen
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
            this.PicBx_Entry = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicBx_Entry)).BeginInit();
            this.SuspendLayout();
            // 
            // PicBx_Entry
            // 
            this.PicBx_Entry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PicBx_Entry.BackColor = System.Drawing.Color.DarkGray;
            this.PicBx_Entry.Location = new System.Drawing.Point(0, -1);
            this.PicBx_Entry.Name = "PicBx_Entry";
            this.PicBx_Entry.Size = new System.Drawing.Size(284, 264);
            this.PicBx_Entry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicBx_Entry.TabIndex = 0;
            this.PicBx_Entry.TabStop = false;
            this.PicBx_Entry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PicBx_Entry_MouseUp);
            // 
            // Fullscreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.PicBx_Entry);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Fullscreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Fullscreen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fullscreen_FormClosing);
            this.Load += new System.EventHandler(this.Fullscreen_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Fullscreen_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.PicBx_Entry)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PicBx_Entry;
    }
}