namespace TpIDS
{
    partial class FormSeleccionarTarjetaDashboard
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
            this.lstbTiposTarjeta = new Krypton.Toolkit.KryptonListBox();
            this.btnAceptarTarjeta = new Krypton.Toolkit.KryptonButton();
            this.btnCancelarTarjeta = new Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            //
            // lstbTiposTarjeta
            //
            this.lstbTiposTarjeta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lstbTiposTarjeta.FormattingEnabled = true;
            this.lstbTiposTarjeta.Location = new System.Drawing.Point(12, 12);
            this.lstbTiposTarjeta.Name = "lstbTiposTarjeta";
            this.lstbTiposTarjeta.Size = new System.Drawing.Size(300, 160);
            this.lstbTiposTarjeta.TabIndex = 0;
            this.lstbTiposTarjeta.SelectedIndexChanged += new System.EventHandler(this.lstbTiposTarjeta_SelectedIndexChanged);
            //
            // btnAceptarTarjeta
            //
            this.btnAceptarTarjeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAceptarTarjeta.Enabled = false;
            this.btnAceptarTarjeta.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAceptarTarjeta.Location = new System.Drawing.Point(156, 184);
            this.btnAceptarTarjeta.Name = "btnAceptarTarjeta";
            this.btnAceptarTarjeta.Size = new System.Drawing.Size(75, 30);
            this.btnAceptarTarjeta.TabIndex = 1;
            this.btnAceptarTarjeta.Tag = "FormSelectCardType.AcceptButton";
            this.btnAceptarTarjeta.Values.Text = "Aceptar";
            this.btnAceptarTarjeta.Click += new System.EventHandler(this.btnAceptarTarjeta_Click);
            //
            // btnCancelarTarjeta
            //
            this.btnCancelarTarjeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelarTarjeta.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnCancelarTarjeta.Location = new System.Drawing.Point(237, 184);
            this.btnCancelarTarjeta.Name = "btnCancelarTarjeta";
            this.btnCancelarTarjeta.Size = new System.Drawing.Size(75, 30);
            this.btnCancelarTarjeta.TabIndex = 2;
            this.btnCancelarTarjeta.Tag = "FormSelectCardType.CancelButton";
            this.btnCancelarTarjeta.Values.Text = "Cancelar";
            this.btnCancelarTarjeta.Click += new System.EventHandler(this.btnCancelarTarjeta_Click);
            //
            // FormSeleccionarTarjetaDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 226);
            this.Controls.Add(this.btnCancelarTarjeta);
            this.Controls.Add(this.btnAceptarTarjeta);
            this.Controls.Add(this.lstbTiposTarjeta);
            this.MinimizeBox = false;
            this.Name = "FormSeleccionarTarjetaDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormSelectCardType.Title";
            this.Text = "Agregar tarjeta";
            this.ResumeLayout(false);

        }

        private Krypton.Toolkit.KryptonListBox lstbTiposTarjeta;
        private Krypton.Toolkit.KryptonButton btnAceptarTarjeta;
        private Krypton.Toolkit.KryptonButton btnCancelarTarjeta;
    }
}
