namespace Nagru___Manga_Organizer
{
    partial class Suggest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Suggest));
            this.lblID = new System.Windows.Forms.ToolStrip();
            this.lblPass = new System.Windows.Forms.ToolStripLabel();
            this.txbxPass = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txbxID = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOK = new System.Windows.Forms.ToolStripButton();
            this.progLoad = new System.Windows.Forms.ToolStripProgressBar();
            this.lblID.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblID
            // 
            this.lblID.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblID.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.lblID.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.txbxID,
            this.lblPass,
            this.txbxPass,
            this.toolStripSeparator1,
            this.progLoad,
            this.btnOK});
            this.lblID.Location = new System.Drawing.Point(0, 362);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(518, 25);
            this.lblID.TabIndex = 9;
            this.lblID.Text = "toolStrip1";
            // 
            // lblPass
            // 
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(57, 22);
            this.lblPass.Text = "passHash";
            // 
            // txbxPass
            // 
            this.txbxPass.Name = "txbxPass";
            this.txbxPass.Size = new System.Drawing.Size(100, 25);
            this.txbxPass.TextChanged += new System.EventHandler(this.txbxPass_TextChanged);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(63, 22);
            this.toolStripLabel2.Text = "memberID";
            // 
            // txbxID
            // 
            this.txbxID.Name = "txbxID";
            this.txbxID.Size = new System.Drawing.Size(100, 25);
            this.txbxID.TextChanged += new System.EventHandler(this.txbxID_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOK
            // 
            this.btnOK.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(27, 22);
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // progLoad
            // 
            this.progLoad.Name = "progLoad";
            this.progLoad.Size = new System.Drawing.Size(140, 22);
            // 
            // Suggest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 387);
            this.Controls.Add(this.lblID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Suggest";
            this.ShowIcon = false;
            this.Text = "Suggest";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Suggest_FormClosing);
            this.lblID.ResumeLayout(false);
            this.lblID.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip lblID;
        private System.Windows.Forms.ToolStripLabel lblPass;
        private System.Windows.Forms.ToolStripTextBox txbxPass;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox txbxID;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripProgressBar progLoad;
        private System.Windows.Forms.ToolStripButton btnOK;

    }
}