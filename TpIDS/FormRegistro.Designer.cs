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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxNickname = new System.Windows.Forms.TextBox();
            this.buttonRegistro = new System.Windows.Forms.Button();
            this.pictureBoxPassword = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Ingrese su contraseña:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Ingrese su nickname:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(39, 130);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '●';
            this.textBoxPassword.Size = new System.Drawing.Size(176, 20);
            this.textBoxPassword.TabIndex = 9;
            // 
            // textBoxNickname
            // 
            this.textBoxNickname.Location = new System.Drawing.Point(39, 60);
            this.textBoxNickname.Name = "textBoxNickname";
            this.textBoxNickname.Size = new System.Drawing.Size(200, 20);
            this.textBoxNickname.TabIndex = 8;
            // 
            // buttonRegistro
            // 
            this.buttonRegistro.Location = new System.Drawing.Point(39, 194);
            this.buttonRegistro.Name = "buttonRegistro";
            this.buttonRegistro.Size = new System.Drawing.Size(200, 43);
            this.buttonRegistro.TabIndex = 12;
            this.buttonRegistro.Text = "Registrarse";
            this.buttonRegistro.UseVisualStyleBackColor = true;
            this.buttonRegistro.Click += new System.EventHandler(this.buttonRegistro_Click);
            // 
            // pictureBoxPassword
            // 
            this.pictureBoxPassword.Location = new System.Drawing.Point(220, 130);
            this.pictureBoxPassword.Name = "pictureBoxPassword";
            this.pictureBoxPassword.Size = new System.Drawing.Size(19, 20);
            this.pictureBoxPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPassword.TabIndex = 13;
            this.pictureBoxPassword.TabStop = false;
            this.pictureBoxPassword.Click += new System.EventHandler(this.pictureBoxPassword_Click);
            // 
            // FormRegistro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 285);
            this.Controls.Add(this.pictureBoxPassword);
            this.Controls.Add(this.buttonRegistro);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxNickname);
            this.Name = "FormRegistro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registro";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxNickname;
        private System.Windows.Forms.Button buttonRegistro;
        private System.Windows.Forms.PictureBox pictureBoxPassword;
    }
}
