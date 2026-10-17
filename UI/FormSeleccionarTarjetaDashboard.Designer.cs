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
            this.lstbTiposTarjeta = new System.Windows.Forms.ListBox();
            this.btnAceptarTarjeta = new System.Windows.Forms.Button();
            this.btnCancelarTarjeta = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lstbTiposTarjeta
            //
            this.lstbTiposTarjeta.FormattingEnabled = true;
            this.lstbTiposTarjeta.Location = new System.Drawing.Point(12, 12);
            this.lstbTiposTarjeta.Name = "lstbTiposTarjeta";
            this.lstbTiposTarjeta.Size = new System.Drawing.Size(300, 160);
            this.lstbTiposTarjeta.TabIndex = 0;
            this.lstbTiposTarjeta.SelectedIndexChanged += new System.EventHandler(this.lstbTiposTarjeta_SelectedIndexChanged);
            //
            // btnAceptarTarjeta
            //
            this.btnAceptarTarjeta.Enabled = false;
            this.btnAceptarTarjeta.Location = new System.Drawing.Point(156, 184);
            this.btnAceptarTarjeta.Name = "btnAceptarTarjeta";
            this.btnAceptarTarjeta.Size = new System.Drawing.Size(75, 30);
            this.btnAceptarTarjeta.TabIndex = 1;
            this.btnAceptarTarjeta.Tag = "FormSelectCardType.AcceptButton";
            this.btnAceptarTarjeta.Text = "Aceptar";
            this.btnAceptarTarjeta.UseVisualStyleBackColor = true;
            this.btnAceptarTarjeta.Click += new System.EventHandler(this.btnAceptarTarjeta_Click);
            //
            // btnCancelarTarjeta
            //
            this.btnCancelarTarjeta.Location = new System.Drawing.Point(237, 184);
            this.btnCancelarTarjeta.Name = "btnCancelarTarjeta";
            this.btnCancelarTarjeta.Size = new System.Drawing.Size(75, 30);
            this.btnCancelarTarjeta.TabIndex = 2;
            this.btnCancelarTarjeta.Tag = "FormSelectCardType.CancelButton";
            this.btnCancelarTarjeta.Text = "Cancelar";
            this.btnCancelarTarjeta.UseVisualStyleBackColor = true;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSeleccionarTarjetaDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormSelectCardType.Title";
            this.Text = "Agregar tarjeta";
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ListBox lstbTiposTarjeta;
        private System.Windows.Forms.Button btnAceptarTarjeta;
        private System.Windows.Forms.Button btnCancelarTarjeta;
    }
}
