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
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.btnIngreso = new System.Windows.Forms.Button();
            this.txtbNickname = new System.Windows.Forms.TextBox();
            this.txtbPassword = new TpIDS.PasswordTextBox();
            this.lblNickname = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnRegistro = new System.Windows.Forms.Button();
            this.lblRegistro = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLogo
            // 
            this.pbLogo.Image = global::TpIDS.Properties.Resources.logo_finanzapp;
            this.pbLogo.Location = new System.Drawing.Point(38, 22);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(369, 219);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabIndex = 10;
            this.pbLogo.TabStop = false;
            // 
            // btnIngreso
            // 
            this.btnIngreso.Location = new System.Drawing.Point(38, 360);
            this.btnIngreso.Name = "btnIngreso";
            this.btnIngreso.Size = new System.Drawing.Size(369, 43);
            this.btnIngreso.TabIndex = 2;
            this.btnIngreso.Tag = "FormLogin.LoginButton";
            this.btnIngreso.Text = "Ingresar";
            this.btnIngreso.UseVisualStyleBackColor = true;
            this.btnIngreso.Click += new System.EventHandler(this.btnIngreso_Click);
            // 
            // txtbNickname
            // 
            this.txtbNickname.Location = new System.Drawing.Point(38, 271);
            this.txtbNickname.Name = "txtbNickname";
            this.txtbNickname.Size = new System.Drawing.Size(369, 20);
            this.txtbNickname.TabIndex = 0;
            // 
            // txtbPassword
            // 
            this.txtbPassword.Location = new System.Drawing.Point(38, 321);
            this.txtbPassword.Name = "txtbPassword";
            this.txtbPassword.Size = new System.Drawing.Size(369, 20);
            this.txtbPassword.TabIndex = 1;
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(35, 255);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(108, 13);
            this.lblNickname.TabIndex = 5;
            this.lblNickname.Tag = "FormLogin.NicknameLabel";
            this.lblNickname.Text = "Ingrese su nickname:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(37, 305);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(115, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Tag = "FormLogin.PasswordLabel";
            this.lblPassword.Text = "Ingrese su contrasena:";
            // 
            // btnRegistro
            // 
            this.btnRegistro.Location = new System.Drawing.Point(38, 444);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Size = new System.Drawing.Size(369, 28);
            this.btnRegistro.TabIndex = 3;
            this.btnRegistro.Tag = "FormLogin.RegisterButton";
            this.btnRegistro.Text = "Registrarme";
            this.btnRegistro.UseVisualStyleBackColor = true;
            this.btnRegistro.Click += new System.EventHandler(this.btnRegistro_Click);
            // 
            // lblRegistro
            // 
            this.lblRegistro.AutoSize = true;
            this.lblRegistro.Location = new System.Drawing.Point(37, 428);
            this.lblRegistro.Name = "lblRegistro";
            this.lblRegistro.Size = new System.Drawing.Size(93, 13);
            this.lblRegistro.TabIndex = 9;
            this.lblRegistro.Tag = "FormLogin.RegisterPrompt";
            this.lblRegistro.Text = "No tenes usuario?";
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 500);
            this.Controls.Add(this.lblRegistro);
            this.Controls.Add(this.btnRegistro);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblNickname);
            this.Controls.Add(this.txtbPassword);
            this.Controls.Add(this.txtbNickname);
            this.Controls.Add(this.btnIngreso);
            this.Controls.Add(this.pbLogo);
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormLogin.Title";
            this.Text = "Menu de ingreso";
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Button btnIngreso;
        private System.Windows.Forms.TextBox txtbNickname;
        private PasswordTextBox txtbPassword;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnRegistro;
        private System.Windows.Forms.Label lblRegistro;
    }
}
