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
            this.lblTituloAsistente = new System.Windows.Forms.Label();
            this.rtbHistorialAsistente = new System.Windows.Forms.RichTextBox();
            this.lblMensajeAsistente = new System.Windows.Forms.Label();
            this.txtbMensajeAsistente = new System.Windows.Forms.TextBox();
            this.btnEnviarMensajeAsistente = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTituloAsistente
            //
            this.lblTituloAsistente.AutoSize = true;
            this.lblTituloAsistente.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloAsistente.Location = new System.Drawing.Point(18, 15);
            this.lblTituloAsistente.Name = "lblTituloAsistente";
            this.lblTituloAsistente.Size = new System.Drawing.Size(216, 25);
            this.lblTituloAsistente.TabIndex = 0;
            this.lblTituloAsistente.Tag = "FormPersonalAssistant.TitleLabel";
            this.lblTituloAsistente.Text = "Asistente Personal";
            //
            // rtbHistorialAsistente
            //
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
            this.lblMensajeAsistente.Location = new System.Drawing.Point(20, 418);
            this.lblMensajeAsistente.Name = "lblMensajeAsistente";
            this.lblMensajeAsistente.Size = new System.Drawing.Size(76, 13);
            this.lblMensajeAsistente.TabIndex = 2;
            this.lblMensajeAsistente.Tag = "FormPersonalAssistant.InputLabel";
            this.lblMensajeAsistente.Text = "Tu mensaje:";
            //
            // txtbMensajeAsistente
            //
            this.txtbMensajeAsistente.Location = new System.Drawing.Point(23, 436);
            this.txtbMensajeAsistente.Name = "txtbMensajeAsistente";
            this.txtbMensajeAsistente.Size = new System.Drawing.Size(608, 20);
            this.txtbMensajeAsistente.TabIndex = 3;
            this.txtbMensajeAsistente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbMensajeAsistente_KeyDown);
            //
            // btnEnviarMensajeAsistente
            //
            this.btnEnviarMensajeAsistente.Location = new System.Drawing.Point(637, 434);
            this.btnEnviarMensajeAsistente.Name = "btnEnviarMensajeAsistente";
            this.btnEnviarMensajeAsistente.Size = new System.Drawing.Size(126, 26);
            this.btnEnviarMensajeAsistente.TabIndex = 4;
            this.btnEnviarMensajeAsistente.Tag = "FormPersonalAssistant.SendButton";
            this.btnEnviarMensajeAsistente.Text = "Enviar";
            this.btnEnviarMensajeAsistente.UseVisualStyleBackColor = true;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormAsistentePersonal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPersonalAssistant.Title";
            this.Text = "Asistente Personal";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloAsistente;
        private System.Windows.Forms.RichTextBox rtbHistorialAsistente;
        private System.Windows.Forms.Label lblMensajeAsistente;
        private System.Windows.Forms.TextBox txtbMensajeAsistente;
        private System.Windows.Forms.Button btnEnviarMensajeAsistente;
    }
}
