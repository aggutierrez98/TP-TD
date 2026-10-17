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
            this.btnIngreso = new Krypton.Toolkit.KryptonButton();
            this.txtbNickname = new Krypton.Toolkit.KryptonTextBox();
            this.txtbPassword = new TpIDS.PasswordTextBox();
            this.lblNickname = new Krypton.Toolkit.KryptonLabel();
            this.lblPassword = new Krypton.Toolkit.KryptonLabel();
            this.btnRegistro = new Krypton.Toolkit.KryptonButton();
            this.lblRegistro = new Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.pnlBloqueLogin = new System.Windows.Forms.Panel();
            this.pnlBloqueLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbLogo
            // 
            this.pbLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLogo.Image = global::TpIDS.Properties.Resources.logo_finanzapp;
            this.pbLogo.Location = new System.Drawing.Point(3, 0);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(369, 219);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabIndex = 10;
            this.pbLogo.TabStop = false;
            // 
            // btnIngreso
            // 
            this.btnIngreso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIngreso.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnIngreso.Location = new System.Drawing.Point(3, 338);
            this.btnIngreso.Name = "btnIngreso";
            this.btnIngreso.Size = new System.Drawing.Size(369, 43);
            this.btnIngreso.TabIndex = 2;
            this.btnIngreso.Tag = "FormLogin.LoginButton";
            this.btnIngreso.Values.Text = "Ingresar";
            this.btnIngreso.Click += new System.EventHandler(this.btnIngreso_Click);
            // 
            // txtbNickname
            // 
            this.txtbNickname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNickname.Location = new System.Drawing.Point(3, 249);
            this.txtbNickname.Name = "txtbNickname";
            this.txtbNickname.Size = new System.Drawing.Size(369, 20);
            this.txtbNickname.TabIndex = 0;
            // 
            // txtbPassword
            // 
            this.txtbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbPassword.Location = new System.Drawing.Point(3, 299);
            this.txtbPassword.Name = "txtbPassword";
            this.txtbPassword.Size = new System.Drawing.Size(369, 24);
            this.txtbPassword.TabIndex = 1;
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(0, 229);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(108, 13);
            this.lblNickname.TabIndex = 5;
            this.lblNickname.Tag = "FormLogin.NicknameLabel";
            this.lblNickname.Values.Text = "Ingrese su nickname:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(2, 279);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(115, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Tag = "FormLogin.PasswordLabel";
            this.lblPassword.Values.Text = "Ingrese su contrasena:";
            // 
            // btnRegistro
            // 
            this.btnRegistro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegistro.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnRegistro.Location = new System.Drawing.Point(3, 422);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Size = new System.Drawing.Size(369, 28);
            this.btnRegistro.TabIndex = 3;
            this.btnRegistro.Tag = "FormLogin.RegisterButton";
            this.btnRegistro.Values.Text = "Registrarme";
            this.btnRegistro.Click += new System.EventHandler(this.btnRegistro_Click);
            // 
            // lblRegistro
            // 
            this.lblRegistro.AutoSize = true;
            this.lblRegistro.Location = new System.Drawing.Point(2, 402);
            this.lblRegistro.Name = "lblRegistro";
            this.lblRegistro.Size = new System.Drawing.Size(93, 13);
            this.lblRegistro.TabIndex = 9;
            this.lblRegistro.Tag = "FormLogin.RegisterPrompt";
            this.lblRegistro.Values.Text = "No tenes usuario?";
            // 
            // pnlBloqueLogin
            // 
            this.pnlBloqueLogin.Controls.Add(this.pbLogo);
            this.pnlBloqueLogin.Controls.Add(this.lblNickname);
            this.pnlBloqueLogin.Controls.Add(this.txtbNickname);
            this.pnlBloqueLogin.Controls.Add(this.lblPassword);
            this.pnlBloqueLogin.Controls.Add(this.txtbPassword);
            this.pnlBloqueLogin.Controls.Add(this.btnIngreso);
            this.pnlBloqueLogin.Controls.Add(this.lblRegistro);
            this.pnlBloqueLogin.Controls.Add(this.btnRegistro);
            this.pnlBloqueLogin.Location = new System.Drawing.Point(35, 22);
            this.pnlBloqueLogin.Name = "pnlBloqueLogin";
            this.pnlBloqueLogin.Size = new System.Drawing.Size(372, 450);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 500);
            this.Controls.Add(this.pnlBloqueLogin);
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormLogin.Title";
            this.Text = "Menu de ingreso";
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.pnlBloqueLogin.ResumeLayout(false);
            this.pnlBloqueLogin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.PictureBox pbLogo;
        private Krypton.Toolkit.KryptonButton btnIngreso;
        private Krypton.Toolkit.KryptonTextBox txtbNickname;
        private PasswordTextBox txtbPassword;
        private Krypton.Toolkit.KryptonLabel lblNickname;
        private Krypton.Toolkit.KryptonLabel lblPassword;
        private Krypton.Toolkit.KryptonButton btnRegistro;
        private Krypton.Toolkit.KryptonLabel lblRegistro;
        private System.Windows.Forms.Panel pnlBloqueLogin;
    }
}
