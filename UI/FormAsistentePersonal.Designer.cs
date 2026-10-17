namespace TpIDS
{
    partial class FormAsistentePersonal
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
            this.lblTituloAsistente = new Krypton.Toolkit.KryptonLabel();
            this.rtbHistorialAsistente = new System.Windows.Forms.RichTextBox();
            this.lblMensajeAsistente = new Krypton.Toolkit.KryptonLabel();
            this.txtbMensajeAsistente = new Krypton.Toolkit.KryptonTextBox();
            this.btnEnviarMensajeAsistente = new Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            //
            // lblTituloAsistente
            //
            this.lblTituloAsistente.AutoSize = true;
            this.lblTituloAsistente.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloAsistente.Location = new System.Drawing.Point(18, 15);
            this.lblTituloAsistente.Name = "lblTituloAsistente";
            this.lblTituloAsistente.Size = new System.Drawing.Size(216, 25);
            this.lblTituloAsistente.TabIndex = 0;
            this.lblTituloAsistente.Tag = "FormPersonalAssistant.TitleLabel";
            this.lblTituloAsistente.Values.Text = "Asistente Personal";
            //
            // rtbHistorialAsistente
            //
            this.rtbHistorialAsistente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbHistorialAsistente.Location = new System.Drawing.Point(23, 55);
            this.rtbHistorialAsistente.Name = "rtbHistorialAsistente";
            this.rtbHistorialAsistente.ReadOnly = true;
            this.rtbHistorialAsistente.Size = new System.Drawing.Size(740, 350);
            this.rtbHistorialAsistente.TabIndex = 1;
            this.rtbHistorialAsistente.Text = "";
            //
            // lblMensajeAsistente
            //
            this.lblMensajeAsistente.AutoSize = true;
            this.lblMensajeAsistente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMensajeAsistente.Location = new System.Drawing.Point(20, 414);
            this.lblMensajeAsistente.Name = "lblMensajeAsistente";
            this.lblMensajeAsistente.Size = new System.Drawing.Size(76, 13);
            this.lblMensajeAsistente.TabIndex = 2;
            this.lblMensajeAsistente.Tag = "FormPersonalAssistant.InputLabel";
            this.lblMensajeAsistente.Values.Text = "Tu mensaje:";
            //
            // txtbMensajeAsistente
            //
            this.txtbMensajeAsistente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbMensajeAsistente.Location = new System.Drawing.Point(23, 436);
            this.txtbMensajeAsistente.Name = "txtbMensajeAsistente";
            this.txtbMensajeAsistente.Size = new System.Drawing.Size(608, 20);
            this.txtbMensajeAsistente.TabIndex = 3;
            this.txtbMensajeAsistente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbMensajeAsistente_KeyDown);
            //
            // btnEnviarMensajeAsistente
            //
            this.btnEnviarMensajeAsistente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnviarMensajeAsistente.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEnviarMensajeAsistente.Location = new System.Drawing.Point(637, 434);
            this.btnEnviarMensajeAsistente.Name = "btnEnviarMensajeAsistente";
            this.btnEnviarMensajeAsistente.Size = new System.Drawing.Size(126, 26);
            this.btnEnviarMensajeAsistente.TabIndex = 4;
            this.btnEnviarMensajeAsistente.Tag = "FormPersonalAssistant.SendButton";
            this.btnEnviarMensajeAsistente.Values.Text = "Enviar";
            this.btnEnviarMensajeAsistente.Click += new System.EventHandler(this.btnEnviarMensajeAsistente_Click);
            //
            // FormAsistentePersonal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 480);
            this.Controls.Add(this.btnEnviarMensajeAsistente);
            this.Controls.Add(this.txtbMensajeAsistente);
            this.Controls.Add(this.lblMensajeAsistente);
            this.Controls.Add(this.rtbHistorialAsistente);
            this.Controls.Add(this.lblTituloAsistente);
            this.Name = "FormAsistentePersonal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPersonalAssistant.Title";
            this.Text = "Asistente Personal";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblTituloAsistente;
        private System.Windows.Forms.RichTextBox rtbHistorialAsistente;
        private Krypton.Toolkit.KryptonLabel lblMensajeAsistente;
        private Krypton.Toolkit.KryptonTextBox txtbMensajeAsistente;
        private Krypton.Toolkit.KryptonButton btnEnviarMensajeAsistente;
    }
}
