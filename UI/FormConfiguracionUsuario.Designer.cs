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
            this.lblTituloConfiguracion = new Krypton.Toolkit.KryptonLabel();
            this.pnlUsuarioActual = new System.Windows.Forms.FlowLayoutPanel();
            this.lblUsuarioActual = new Krypton.Toolkit.KryptonLabel();
            this.lblUsuarioActualNombre = new Krypton.Toolkit.KryptonLabel();
            this.lblIdiomaSesion = new Krypton.Toolkit.KryptonLabel();
            this.cmbIdiomas = new Krypton.Toolkit.KryptonComboBox();
            this.btnGuardarConfiguracion = new Krypton.Toolkit.KryptonButton();
            this.pnlUsuarioActual.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloConfiguracion
            // 
            this.lblTituloConfiguracion.AutoSize = true;
            this.lblTituloConfiguracion.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloConfiguracion.Location = new System.Drawing.Point(20, 18);
            this.lblTituloConfiguracion.Name = "lblTituloConfiguracion";
            this.lblTituloConfiguracion.Size = new System.Drawing.Size(232, 25);
            this.lblTituloConfiguracion.Tag = "FormUserSettings.TitleLabel";
            this.lblTituloConfiguracion.TabIndex = 0;
            this.lblTituloConfiguracion.Values.Text = "Configuracion del usuario";
            //
            // pnlUsuarioActual
            //
            this.pnlUsuarioActual.AutoSize = true;
            this.pnlUsuarioActual.Controls.Add(this.lblUsuarioActual);
            this.pnlUsuarioActual.Controls.Add(this.lblUsuarioActualNombre);
            this.pnlUsuarioActual.Location = new System.Drawing.Point(22, 62);
            this.pnlUsuarioActual.Name = "pnlUsuarioActual";
            this.pnlUsuarioActual.Size = new System.Drawing.Size(95, 13);
            this.pnlUsuarioActual.TabIndex = 1;
            this.pnlUsuarioActual.WrapContents = false;
            //
            // lblUsuarioActual
            //
            this.lblUsuarioActual.AutoSize = true;
            this.lblUsuarioActual.Margin = new System.Windows.Forms.Padding(0);
            this.lblUsuarioActual.Name = "lblUsuarioActual";
            this.lblUsuarioActual.Size = new System.Drawing.Size(76, 13);
            this.lblUsuarioActual.TabIndex = 0;
            this.lblUsuarioActual.Tag = "FormUserSettings.CurrentUserPrefix";
            this.lblUsuarioActual.Values.Text = "Usuario actual:";
            //
            // lblUsuarioActualNombre
            //
            this.lblUsuarioActualNombre.AutoSize = true;
            this.lblUsuarioActualNombre.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblUsuarioActualNombre.Name = "lblUsuarioActualNombre";
            this.lblUsuarioActualNombre.Size = new System.Drawing.Size(10, 13);
            this.lblUsuarioActualNombre.TabIndex = 1;
            this.lblUsuarioActualNombre.Values.Text = "-";
            //
            // lblIdiomaSesion
            // 
            this.lblIdiomaSesion.AutoSize = true;
            this.lblIdiomaSesion.Location = new System.Drawing.Point(22, 101);
            this.lblIdiomaSesion.Name = "lblIdiomaSesion";
            this.lblIdiomaSesion.Size = new System.Drawing.Size(112, 13);
            this.lblIdiomaSesion.Tag = "FormUserSettings.LanguageLabel";
            this.lblIdiomaSesion.TabIndex = 2;
            this.lblIdiomaSesion.Values.Text = "Idioma para la sesion:";
            // 
            // cmbIdiomas
            // 
            this.cmbIdiomas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIdiomas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdiomas.FormattingEnabled = true;
            this.cmbIdiomas.Location = new System.Drawing.Point(25, 121);
            this.cmbIdiomas.Name = "cmbIdiomas";
            this.cmbIdiomas.Size = new System.Drawing.Size(290, 21);
            this.cmbIdiomas.TabIndex = 3;
            // 
            // btnGuardarConfiguracion
            // 
            this.btnGuardarConfiguracion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGuardarConfiguracion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnGuardarConfiguracion.Location = new System.Drawing.Point(25, 165);
            this.btnGuardarConfiguracion.Name = "btnGuardarConfiguracion";
            this.btnGuardarConfiguracion.Size = new System.Drawing.Size(290, 35);
            this.btnGuardarConfiguracion.Tag = "FormUserSettings.SaveButton";
            this.btnGuardarConfiguracion.TabIndex = 4;
            this.btnGuardarConfiguracion.Values.Text = "Guardar";
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
            this.Controls.Add(this.pnlUsuarioActual);
            this.Controls.Add(this.lblTituloConfiguracion);
            this.Name = "FormConfiguracionUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormUserSettings.Title";
            this.Text = "Configuracion del usuario";
            this.pnlUsuarioActual.ResumeLayout(false);
            this.pnlUsuarioActual.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblTituloConfiguracion;
        private System.Windows.Forms.FlowLayoutPanel pnlUsuarioActual;
        private Krypton.Toolkit.KryptonLabel lblUsuarioActual;
        private Krypton.Toolkit.KryptonLabel lblUsuarioActualNombre;
        private Krypton.Toolkit.KryptonLabel lblIdiomaSesion;
        private Krypton.Toolkit.KryptonComboBox cmbIdiomas;
        private Krypton.Toolkit.KryptonButton btnGuardarConfiguracion;
    }
}
