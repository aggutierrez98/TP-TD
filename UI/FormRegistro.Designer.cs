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
            this.lblPassword = new Krypton.Toolkit.KryptonLabel();
            this.lblNickname = new Krypton.Toolkit.KryptonLabel();
            this.txtbPassword = new TpIDS.PasswordTextBox();
            this.txtbNickname = new Krypton.Toolkit.KryptonTextBox();
            this.btnRegistro = new Krypton.Toolkit.KryptonButton();
            this.pnlBloqueRegistro = new System.Windows.Forms.Panel();
            this.pnlBloqueRegistro.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(0, 70);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(115, 13);
            this.lblPassword.Tag = "FormRegistration.PasswordLabel";
            this.lblPassword.TabIndex = 11;
            this.lblPassword.Values.Text = "Ingrese su contrasena:";
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(0, 0);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(108, 13);
            this.lblNickname.Tag = "FormRegistration.NicknameLabel";
            this.lblNickname.TabIndex = 10;
            this.lblNickname.Values.Text = "Ingrese su nickname:";
            // 
            // txtbPassword
            // 
            this.txtbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbPassword.Location = new System.Drawing.Point(3, 90);
            this.txtbPassword.Name = "txtbPassword";
            this.txtbPassword.Size = new System.Drawing.Size(200, 24);
            this.txtbPassword.TabIndex = 9;
            // 
            // txtbNickname
            // 
            this.txtbNickname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNickname.Location = new System.Drawing.Point(3, 20);
            this.txtbNickname.Name = "txtbNickname";
            this.txtbNickname.Size = new System.Drawing.Size(200, 20);
            this.txtbNickname.TabIndex = 8;
            // 
            // btnRegistro
            // 
            this.btnRegistro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegistro.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnRegistro.Location = new System.Drawing.Point(3, 154);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Size = new System.Drawing.Size(200, 43);
            this.btnRegistro.Tag = "FormRegistration.RegisterButton";
            this.btnRegistro.TabIndex = 12;
            this.btnRegistro.Values.Text = "Registrarse";
            this.btnRegistro.Click += new System.EventHandler(this.btnRegistro_Click);
            // 
            // pnlBloqueRegistro
            // 
            this.pnlBloqueRegistro.Controls.Add(this.lblNickname);
            this.pnlBloqueRegistro.Controls.Add(this.txtbNickname);
            this.pnlBloqueRegistro.Controls.Add(this.lblPassword);
            this.pnlBloqueRegistro.Controls.Add(this.txtbPassword);
            this.pnlBloqueRegistro.Controls.Add(this.btnRegistro);
            this.pnlBloqueRegistro.Location = new System.Drawing.Point(36, 40);
            this.pnlBloqueRegistro.Name = "pnlBloqueRegistro";
            this.pnlBloqueRegistro.Size = new System.Drawing.Size(203, 197);
            // 
            // FormRegistro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 285);
            this.Controls.Add(this.pnlBloqueRegistro);
            this.Name = "FormRegistro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormRegistration.Title";
            this.Text = "Registro";
            this.pnlBloqueRegistro.ResumeLayout(false);
            this.pnlBloqueRegistro.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblPassword;
        private Krypton.Toolkit.KryptonLabel lblNickname;
        private PasswordTextBox txtbPassword;
        private Krypton.Toolkit.KryptonTextBox txtbNickname;
        private Krypton.Toolkit.KryptonButton btnRegistro;
        private System.Windows.Forms.Panel pnlBloqueRegistro;
    }
}
