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
            this.lblEncabezado = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.lblTablaAfectada = new System.Windows.Forms.Label();
            this.lblTablaValor = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblEncabezado
            //
            this.lblEncabezado.AutoSize = true;
            this.lblEncabezado.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblEncabezado.ForeColor = System.Drawing.Color.Firebrick;
            this.lblEncabezado.Location = new System.Drawing.Point(22, 22);
            this.lblEncabezado.Name = "lblEncabezado";
            this.lblEncabezado.Size = new System.Drawing.Size(289, 20);
            this.lblEncabezado.TabIndex = 0;
            this.lblEncabezado.Tag = "FormContingencia.Heading";
            this.lblEncabezado.Text = "Error de integridad de datos";
            //
            // lblMensaje
            //
            this.lblMensaje.Location = new System.Drawing.Point(24, 58);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(446, 86);
            this.lblMensaje.TabIndex = 1;
            //
            // lblTablaAfectada
            //
            this.lblTablaAfectada.AutoSize = true;
            this.lblTablaAfectada.Location = new System.Drawing.Point(24, 158);
            this.lblTablaAfectada.Name = "lblTablaAfectada";
            this.lblTablaAfectada.Size = new System.Drawing.Size(78, 13);
            this.lblTablaAfectada.TabIndex = 2;
            this.lblTablaAfectada.Tag = "FormContingencia.Label.TablaAfectada";
            this.lblTablaAfectada.Text = "Tabla afectada:";
            //
            // lblTablaValor
            //
            this.lblTablaValor.AutoSize = true;
            this.lblTablaValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTablaValor.Location = new System.Drawing.Point(108, 158);
            this.lblTablaValor.Name = "lblTablaValor";
            this.lblTablaValor.Size = new System.Drawing.Size(0, 13);
            this.lblTablaValor.TabIndex = 3;
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(184, 196);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(126, 38);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Tag = "FormContingencia.Button.Salir";
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // FormContingenciaIntegridad
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 261);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblTablaValor);
            this.Controls.Add(this.lblTablaAfectada);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.lblEncabezado);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormContingenciaIntegridad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormContingencia.Title";
            this.Text = "Error de integridad";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblEncabezado;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Label lblTablaAfectada;
        private System.Windows.Forms.Label lblTablaValor;
        private System.Windows.Forms.Button btnSalir;
    }
}
