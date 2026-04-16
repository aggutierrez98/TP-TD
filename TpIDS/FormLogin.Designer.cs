namespace TpIDS
{
    partial class FormLogin
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
            this.buttonIniciarSesion = new System.Windows.Forms.Button();
            this.textBoxNickname = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRegistro = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBoxPassword = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonIniciarSesion
            // 
            this.buttonIniciarSesion.Location = new System.Drawing.Point(38, 270);
            this.buttonIniciarSesion.Name = "buttonIniciarSesion";
            this.buttonIniciarSesion.Size = new System.Drawing.Size(200, 43);
            this.buttonIniciarSesion.TabIndex = 2;
            this.buttonIniciarSesion.Text = "Ingresar";
            this.buttonIniciarSesion.UseVisualStyleBackColor = true;
            this.buttonIniciarSesion.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxNickname
            // 
            this.textBoxNickname.Location = new System.Drawing.Point(38, 181);
            this.textBoxNickname.Name = "textBoxNickname";
            this.textBoxNickname.Size = new System.Drawing.Size(200, 20);
            this.textBoxNickname.TabIndex = 0;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(38, 231);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '●';
            this.textBoxPassword.Size = new System.Drawing.Size(174, 20);
            this.textBoxPassword.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Ingrese su nickname:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Ingrese su contraseña:";
            // 
            // buttonRegistro
            // 
            this.buttonRegistro.Location = new System.Drawing.Point(38, 354);
            this.buttonRegistro.Name = "buttonRegistro";
            this.buttonRegistro.Size = new System.Drawing.Size(200, 28);
            this.buttonRegistro.TabIndex = 3;
            this.buttonRegistro.Text = "Registrarme";
            this.buttonRegistro.UseVisualStyleBackColor = true;
            this.buttonRegistro.Click += new System.EventHandler(this.buttonRegistro_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 338);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "¿No tenes usuario?";
            // 
            // pictureBoxPassword
            // 
            this.pictureBoxPassword.Location = new System.Drawing.Point(219, 231);
            this.pictureBoxPassword.Name = "pictureBoxPassword";
            this.pictureBoxPassword.Size = new System.Drawing.Size(19, 20);
            this.pictureBoxPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPassword.TabIndex = 10;
            this.pictureBoxPassword.TabStop = false;
            this.pictureBoxPassword.Click += new System.EventHandler(this.pictureBoxPassword_Click);

            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 410);
            this.Controls.Add(this.pictureBoxPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonRegistro);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxNickname);
            this.Controls.Add(this.buttonIniciarSesion);
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu de ingreso";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button buttonIniciarSesion;
        private System.Windows.Forms.TextBox textBoxNickname;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonRegistro;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBoxPassword;
    }
}
