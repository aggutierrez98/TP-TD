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
            this.btnIngreso = new System.Windows.Forms.Button();
            this.txtbNickname = new System.Windows.Forms.TextBox();
            this.txtbPassword = new System.Windows.Forms.TextBox();
            this.lblNickname = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnRegistro = new System.Windows.Forms.Button();
            this.lblRegistro = new System.Windows.Forms.Label();
            this.pbPassword = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // btnIngreso
            // 
            this.btnIngreso.Location = new System.Drawing.Point(38, 270);
            this.btnIngreso.Name = "btnIngreso";
            this.btnIngreso.Size = new System.Drawing.Size(200, 43);
            this.btnIngreso.TabIndex = 2;
            this.btnIngreso.Text = "Ingresar";
            this.btnIngreso.UseVisualStyleBackColor = true;
            this.btnIngreso.Click += new System.EventHandler(this.btnIngreso_Click);
            // 
            // txtbNickname
            // 
            this.txtbNickname.Location = new System.Drawing.Point(38, 181);
            this.txtbNickname.Name = "txtbNickname";
            this.txtbNickname.Size = new System.Drawing.Size(200, 20);
            this.txtbNickname.TabIndex = 0;
            // 
            // txtbPassword
            // 
            this.txtbPassword.Location = new System.Drawing.Point(38, 231);
            this.txtbPassword.Name = "txtbPassword";
            this.txtbPassword.PasswordChar = '●';
            this.txtbPassword.Size = new System.Drawing.Size(174, 20);
            this.txtbPassword.TabIndex = 1;
            // 
            // lblNickname
            // 
<<<<<<< HEAD
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(35, 165);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(108, 13);
            this.lblNickname.TabIndex = 5;
            this.lblNickname.Text = "Ingrese su nickname:";
=======
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Ingrese su nombre de usuario:";
>>>>>>> main
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(37, 215);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(115, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Ingrese su contrasena:";
            // 
            // btnRegistro
            // 
            this.btnRegistro.Location = new System.Drawing.Point(38, 354);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Size = new System.Drawing.Size(200, 28);
            this.btnRegistro.TabIndex = 3;
            this.btnRegistro.Text = "Registrarme";
            this.btnRegistro.UseVisualStyleBackColor = true;
            this.btnRegistro.Click += new System.EventHandler(this.btnRegistro_Click);
            // 
            // lblRegistro
            // 
            this.lblRegistro.AutoSize = true;
            this.lblRegistro.Location = new System.Drawing.Point(37, 338);
            this.lblRegistro.Name = "lblRegistro";
            this.lblRegistro.Size = new System.Drawing.Size(97, 13);
            this.lblRegistro.TabIndex = 9;
            this.lblRegistro.Text = "No tenes usuario?";
            // 
            // pbPassword
            // 
<<<<<<< HEAD
            this.pbPassword.Location = new System.Drawing.Point(219, 231);
            this.pbPassword.Name = "pbPassword";
            this.pbPassword.Size = new System.Drawing.Size(19, 20);
            this.pbPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPassword.TabIndex = 10;
            this.pbPassword.TabStop = false;
            this.pbPassword.Click += new System.EventHandler(this.pbPassword_Click);
=======
            this.pictureBoxPassword.Location = new System.Drawing.Point(219, 231);
            this.pictureBoxPassword.Name = "pictureBoxPassword";
            this.pictureBoxPassword.Size = new System.Drawing.Size(19, 20);
            this.pictureBoxPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPassword.TabIndex = 10;
            this.pictureBoxPassword.TabStop = false;
            this.pictureBoxPassword.Click += new System.EventHandler(this.pictureBoxPassword_Click);
>>>>>>> main
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 410);
            this.Controls.Add(this.pbPassword);
            this.Controls.Add(this.lblRegistro);
            this.Controls.Add(this.btnRegistro);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblNickname);
            this.Controls.Add(this.txtbPassword);
            this.Controls.Add(this.txtbNickname);
            this.Controls.Add(this.btnIngreso);
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu de ingreso";
            ((System.ComponentModel.ISupportInitialize)(this.pbPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnIngreso;
        private System.Windows.Forms.TextBox txtbNickname;
        private System.Windows.Forms.TextBox txtbPassword;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnRegistro;
        private System.Windows.Forms.Label lblRegistro;
        private System.Windows.Forms.PictureBox pbPassword;
    }
}
