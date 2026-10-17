namespace TpIDS
{
    partial class FormContingenciaIntegridad
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
            this.lblEncabezado = new Krypton.Toolkit.KryptonLabel();
            this.lblMensaje = new Krypton.Toolkit.KryptonWrapLabel();
            this.btnSalir = new Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            //
            // lblEncabezado
            //
            this.lblEncabezado.AutoSize = false;
            this.lblEncabezado.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblEncabezado.StateCommon.ShortText.Color1 = System.Drawing.Color.Firebrick;
            this.lblEncabezado.Location = new System.Drawing.Point(22, 22);
            this.lblEncabezado.Name = "lblEncabezado";
            this.lblEncabezado.Size = new System.Drawing.Size(446, 24);
            this.lblEncabezado.TabIndex = 0;
            this.lblEncabezado.Tag = "FormContingencia.Heading";
            this.lblEncabezado.Values.Text = "Error de integridad de datos";
            //
            // lblMensaje
            //
            this.lblMensaje.AutoSize = false;
            this.lblMensaje.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMensaje.Location = new System.Drawing.Point(24, 54);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(446, 86);
            this.lblMensaje.TabIndex = 1;
            //
            // btnSalir
            //
            this.btnSalir.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSalir.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnSalir.Location = new System.Drawing.Point(184, 158);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(126, 38);
            this.btnSalir.TabIndex = 2;
            this.btnSalir.Tag = "FormContingencia.Button.Salir";
            this.btnSalir.Values.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // FormContingenciaIntegridad
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 220);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.lblEncabezado);
            this.MinimizeBox = false;
            this.Name = "FormContingenciaIntegridad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormContingencia.Title";
            this.Text = "Error de integridad";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblEncabezado;
        private Krypton.Toolkit.KryptonWrapLabel lblMensaje;
        private Krypton.Toolkit.KryptonButton btnSalir;
    }
}
