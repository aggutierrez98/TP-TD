namespace TpIDS
{
    partial class PasswordTextBox
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtbInput = new Krypton.Toolkit.KryptonTextBox();
            this.pbTogglePassword = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbTogglePassword)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbInput
            // 
            this.txtbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbInput.Location = new System.Drawing.Point(0, 0);
            this.txtbInput.Name = "txtbInput";
            this.txtbInput.Size = new System.Drawing.Size(174, 24);
            this.txtbInput.TabIndex = 0;
            // 
            // pbTogglePassword
            // 
            this.pbTogglePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbTogglePassword.Location = new System.Drawing.Point(181, 0);
            this.pbTogglePassword.Name = "pbTogglePassword";
            this.pbTogglePassword.Size = new System.Drawing.Size(19, 24);
            this.pbTogglePassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbTogglePassword.TabIndex = 1;
            this.pbTogglePassword.TabStop = false;
            this.pbTogglePassword.Click += new System.EventHandler(this.pbTogglePassword_Click);
            // 
            // PasswordTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbTogglePassword);
            this.Controls.Add(this.txtbInput);
            this.Name = "PasswordTextBox";
            this.Size = new System.Drawing.Size(200, 24);
            ((System.ComponentModel.ISupportInitialize)(this.pbTogglePassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonTextBox txtbInput;
        private System.Windows.Forms.PictureBox pbTogglePassword;
    }
}
