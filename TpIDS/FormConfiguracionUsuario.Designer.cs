namespace TpIDS
{
    partial class FormConfiguracionUsuario
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
            this.lblTituloConfiguracion = new System.Windows.Forms.Label();
            this.lblUsuarioActual = new System.Windows.Forms.Label();
            this.lblIdiomaSesion = new System.Windows.Forms.Label();
            this.cmbIdiomas = new System.Windows.Forms.ComboBox();
            this.btnGuardarConfiguracion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTituloConfiguracion
            // 
            this.lblTituloConfiguracion.AutoSize = true;
            this.lblTituloConfiguracion.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloConfiguracion.Location = new System.Drawing.Point(20, 18);
            this.lblTituloConfiguracion.Name = "lblTituloConfiguracion";
            this.lblTituloConfiguracion.Size = new System.Drawing.Size(232, 25);
            this.lblTituloConfiguracion.TabIndex = 0;
            this.lblTituloConfiguracion.Text = "Configuracion del usuario";
            // 
            // lblUsuarioActual
            // 
            this.lblUsuarioActual.AutoSize = true;
            this.lblUsuarioActual.Location = new System.Drawing.Point(22, 65);
            this.lblUsuarioActual.Name = "lblUsuarioActual";
            this.lblUsuarioActual.Size = new System.Drawing.Size(87, 13);
            this.lblUsuarioActual.TabIndex = 1;
            this.lblUsuarioActual.Text = "Usuario actual: -";
            // 
            // lblIdiomaSesion
            // 
            this.lblIdiomaSesion.AutoSize = true;
            this.lblIdiomaSesion.Location = new System.Drawing.Point(22, 105);
            this.lblIdiomaSesion.Name = "lblIdiomaSesion";
            this.lblIdiomaSesion.Size = new System.Drawing.Size(112, 13);
            this.lblIdiomaSesion.TabIndex = 2;
            this.lblIdiomaSesion.Text = "Idioma para la sesion:";
            // 
            // cmbIdiomas
            // 
            this.cmbIdiomas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdiomas.FormattingEnabled = true;
            this.cmbIdiomas.Location = new System.Drawing.Point(25, 121);
            this.cmbIdiomas.Name = "cmbIdiomas";
            this.cmbIdiomas.Size = new System.Drawing.Size(290, 21);
            this.cmbIdiomas.TabIndex = 3;
            // 
            // btnGuardarConfiguracion
            // 
            this.btnGuardarConfiguracion.Location = new System.Drawing.Point(25, 165);
            this.btnGuardarConfiguracion.Name = "btnGuardarConfiguracion";
            this.btnGuardarConfiguracion.Size = new System.Drawing.Size(290, 35);
            this.btnGuardarConfiguracion.TabIndex = 4;
            this.btnGuardarConfiguracion.Text = "Guardar";
            this.btnGuardarConfiguracion.UseVisualStyleBackColor = true;
            this.btnGuardarConfiguracion.Click += new System.EventHandler(this.btnGuardarConfiguracion_Click);
            // 
            // FormConfiguracionUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 225);
            this.Controls.Add(this.btnGuardarConfiguracion);
            this.Controls.Add(this.cmbIdiomas);
            this.Controls.Add(this.lblIdiomaSesion);
            this.Controls.Add(this.lblUsuarioActual);
            this.Controls.Add(this.lblTituloConfiguracion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormConfiguracionUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuracion del usuario";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloConfiguracion;
        private System.Windows.Forms.Label lblUsuarioActual;
        private System.Windows.Forms.Label lblIdiomaSesion;
        private System.Windows.Forms.ComboBox cmbIdiomas;
        private System.Windows.Forms.Button btnGuardarConfiguracion;
    }
}
