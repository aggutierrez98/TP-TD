namespace TpIDS
{
    partial class FormDashboardAnaliticas
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
            this.lblTituloDashboard = new Krypton.Toolkit.KryptonLabel();
            this.btnAgregarTarjeta = new Krypton.Toolkit.KryptonButton();
            this.lblSinTarjetasDisponibles = new Krypton.Toolkit.KryptonLabel();
            this.flpTarjetasDashboard = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            //
            // lblTituloDashboard
            //
            this.lblTituloDashboard.AutoSize = true;
            this.lblTituloDashboard.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloDashboard.Location = new System.Drawing.Point(18, 15);
            this.lblTituloDashboard.Name = "lblTituloDashboard";
            this.lblTituloDashboard.Size = new System.Drawing.Size(97, 25);
            this.lblTituloDashboard.TabIndex = 0;
            this.lblTituloDashboard.Tag = "FormDashboard.TitleLabel";
            this.lblTituloDashboard.Values.Text = "Dashboard";
            //
            // btnAgregarTarjeta
            //
            this.btnAgregarTarjeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarTarjeta.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarTarjeta.Location = new System.Drawing.Point(700, 18);
            this.btnAgregarTarjeta.Name = "btnAgregarTarjeta";
            this.btnAgregarTarjeta.Size = new System.Drawing.Size(160, 26);
            this.btnAgregarTarjeta.TabIndex = 1;
            this.btnAgregarTarjeta.Tag = "FormDashboard.AddCardButton";
            this.btnAgregarTarjeta.Values.Text = "Agregar tarjeta";
            this.btnAgregarTarjeta.Click += new System.EventHandler(this.btnAgregarTarjeta_Click);
            //
            // lblSinTarjetasDisponibles
            //
            this.lblSinTarjetasDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSinTarjetasDisponibles.AutoSize = true;
            this.lblSinTarjetasDisponibles.Location = new System.Drawing.Point(560, 48);
            this.lblSinTarjetasDisponibles.MaximumSize = new System.Drawing.Size(300, 0);
            this.lblSinTarjetasDisponibles.Name = "lblSinTarjetasDisponibles";
            this.lblSinTarjetasDisponibles.Size = new System.Drawing.Size(0, 13);
            this.lblSinTarjetasDisponibles.TabIndex = 2;
            this.lblSinTarjetasDisponibles.Tag = "FormDashboard.NoCardsAvailable";
            this.lblSinTarjetasDisponibles.Values.Text = "Ya agrego todas las tarjetas disponibles.";
            this.lblSinTarjetasDisponibles.StateCommon.ShortText.TextH = Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.lblSinTarjetasDisponibles.Visible = false;
            //
            // flpTarjetasDashboard
            //
            this.flpTarjetasDashboard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flpTarjetasDashboard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpTarjetasDashboard.AutoScroll = true;
            this.flpTarjetasDashboard.Location = new System.Drawing.Point(18, 78);
            this.flpTarjetasDashboard.Name = "flpTarjetasDashboard";
            this.flpTarjetasDashboard.Size = new System.Drawing.Size(864, 557);
            this.flpTarjetasDashboard.TabIndex = 3;
            this.flpTarjetasDashboard.WrapContents = true;
            //
            // FormDashboardAnaliticas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 650);
            this.Controls.Add(this.flpTarjetasDashboard);
            this.Controls.Add(this.lblSinTarjetasDisponibles);
            this.Controls.Add(this.btnAgregarTarjeta);
            this.Controls.Add(this.lblTituloDashboard);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "FormDashboardAnaliticas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormDashboard.Title";
            this.Text = "Dashboard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloDashboard;
        private Krypton.Toolkit.KryptonButton btnAgregarTarjeta;
        private Krypton.Toolkit.KryptonLabel lblSinTarjetasDisponibles;
        private System.Windows.Forms.FlowLayoutPanel flpTarjetasDashboard;
    }
}
