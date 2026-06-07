namespace TpIDS
{
    partial class FormRegistro
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
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblNickname = new System.Windows.Forms.Label();
            this.txtbPassword = new System.Windows.Forms.TextBox();
            this.txtbNickname = new System.Windows.Forms.TextBox();
            this.btnRegistro = new System.Windows.Forms.Button();
            this.pbPassword = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(36, 114);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(115, 13);
            this.lblPassword.Tag = "FormRegistro.PasswordLabel";
            this.lblPassword.TabIndex = 11;
            this.lblPassword.Text = "Ingrese su contrasena:";
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(36, 44);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(108, 13);
            this.lblNickname.Tag = "FormRegistro.NicknameLabel";
            this.lblNickname.TabIndex = 10;
            this.lblNickname.Text = "Ingrese su nickname:";
            // 
            // txtbPassword
            // 
            this.txtbPassword.Location = new System.Drawing.Point(39, 130);
            this.txtbPassword.Name = "txtbPassword";
            this.txtbPassword.PasswordChar = '●';
            this.txtbPassword.Size = new System.Drawing.Size(176, 20);
            this.txtbPassword.TabIndex = 9;
            // 
            // txtbNickname
            // 
            this.txtbNickname.Location = new System.Drawing.Point(39, 60);
            this.txtbNickname.Name = "txtbNickname";
            this.txtbNickname.Size = new System.Drawing.Size(200, 20);
            this.txtbNickname.TabIndex = 8;
            // 
            // btnRegistro
            // 
            this.btnRegistro.Location = new System.Drawing.Point(39, 194);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Size = new System.Drawing.Size(200, 43);
            this.btnRegistro.Tag = "FormRegistro.RegisterButton";
            this.btnRegistro.TabIndex = 12;
            this.btnRegistro.Text = "Registrarse";
            this.btnRegistro.UseVisualStyleBackColor = true;
            this.btnRegistro.Click += new System.EventHandler(this.btnRegistro_Click);
            // 
            // pbPassword
            // 
            this.pbPassword.Location = new System.Drawing.Point(220, 130);
            this.pbPassword.Name = "pbPassword";
            this.pbPassword.Size = new System.Drawing.Size(19, 20);
            this.pbPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPassword.TabIndex = 13;
            this.pbPassword.TabStop = false;
            this.pbPassword.Click += new System.EventHandler(this.pbPassword_Click);
            // 
            // FormRegistro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 285);
            this.Controls.Add(this.pbPassword);
            this.Controls.Add(this.btnRegistro);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblNickname);
            this.Controls.Add(this.txtbPassword);
            this.Controls.Add(this.txtbNickname);
            this.Name = "FormRegistro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormRegistro.Title";
            this.Text = "Registro";
            ((System.ComponentModel.ISupportInitialize)(this.pbPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.TextBox txtbPassword;
        private System.Windows.Forms.TextBox txtbNickname;
        private System.Windows.Forms.Button btnRegistro;
        private System.Windows.Forms.PictureBox pbPassword;
    }
}
