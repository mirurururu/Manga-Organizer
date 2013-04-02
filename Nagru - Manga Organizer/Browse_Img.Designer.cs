namespace Nagru___Manga_Organizer
{
    partial class Browse_Img
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
            this.picBx_Right = new System.Windows.Forms.PictureBox();
            this.picBx_Left = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Right)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Left)).BeginInit();
            this.SuspendLayout();
            // 
            // picBx_Right
            // 
            this.picBx_Right.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBx_Right.BackColor = System.Drawing.Color.DarkGray;
            this.picBx_Right.Location = new System.Drawing.Point(250, 0);
            this.picBx_Right.Name = "picBx_Right";
            this.picBx_Right.Size = new System.Drawing.Size(250, 390);
            this.picBx_Right.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBx_Right.TabIndex = 0;
            this.picBx_Right.TabStop = false;
            this.picBx_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Browse_MouseUp);
            // 
            // picBx_Left
            // 
            this.picBx_Left.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBx_Left.BackColor = System.Drawing.Color.DarkGray;
            this.picBx_Left.Location = new System.Drawing.Point(0, 0);
            this.picBx_Left.Name = "picBx_Left";
            this.picBx_Left.Size = new System.Drawing.Size(244, 390);
            this.picBx_Left.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBx_Left.TabIndex = 1;
            this.picBx_Left.TabStop = false;
            this.picBx_Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Browse_MouseUp);
            // 
            // Browse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(500, 389);
            this.Controls.Add(this.picBx_Left);
            this.Controls.Add(this.picBx_Right);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Browse";
            this.ShowInTaskbar = false;
            this.Text = "Browse";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Browse_FormClosing);
            this.Load += new System.EventHandler(this.Browse_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Browse_KeyDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Browse_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Right)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBx_Left)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBx_Right;
        private System.Windows.Forms.PictureBox picBx_Left;
    }
}