namespace TpIDS
{
    partial class FormGestionNotificaciones
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
            this.lblTituloNotificaciones = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxNotificacion = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarNotificacion = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarNotificacion = new Krypton.Toolkit.KryptonButton();
            this.txtbMensajeNotificacion = new Krypton.Toolkit.KryptonTextBox();
            this.lblMensajeNotificacion = new Krypton.Toolkit.KryptonLabel();
            this.cmbUsuarioDestino = new Krypton.Toolkit.KryptonComboBox();
            this.lblUsuarioDestino = new Krypton.Toolkit.KryptonLabel();
            this.dgvNotificaciones = new Krypton.Toolkit.KryptonDataGridView();
            this.colNotifUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifMensaje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifLeida = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNotificacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNotificacion.Panel)).BeginInit();
            this.groupBoxNotificacion.Panel.SuspendLayout();
            this.groupBoxNotificacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotificaciones)).BeginInit();
            this.tlpCamposNotificacion = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposNotificacion.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloNotificaciones
            //
            this.lblTituloNotificaciones.AutoSize = true;
            this.lblTituloNotificaciones.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloNotificaciones.Location = new System.Drawing.Point(18, 15);
            this.lblTituloNotificaciones.Name = "lblTituloNotificaciones";
            this.lblTituloNotificaciones.Size = new System.Drawing.Size(283, 25);
            this.lblTituloNotificaciones.TabIndex = 0;
            this.lblTituloNotificaciones.Tag = "FormNotificationManagement.TitleLabel";
            this.lblTituloNotificaciones.Values.Text = "Gestion de Notificaciones";
            //
            // groupBoxNotificacion
            //
            this.groupBoxNotificacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxNotificacion.Panel.Controls.Add(this.btnEliminarNotificacion);
            this.groupBoxNotificacion.Panel.Controls.Add(this.btnAgregarNotificacion);
            this.groupBoxNotificacion.Panel.Controls.Add(this.tlpCamposNotificacion);
            this.groupBoxNotificacion.Location = new System.Drawing.Point(23, 52);
            this.groupBoxNotificacion.Name = "groupBoxNotificacion";
            this.groupBoxNotificacion.Size = new System.Drawing.Size(554, 116);
            this.groupBoxNotificacion.TabIndex = 1;
            this.groupBoxNotificacion.TabStop = false;
            this.groupBoxNotificacion.Tag = "FormNotificationManagement.NotificationGroup";
            this.groupBoxNotificacion.Values.Heading = "Nueva notificacion";
            //
            // btnEliminarNotificacion
            //
            this.btnEliminarNotificacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarNotificacion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarNotificacion.Location = new System.Drawing.Point(394, 63);
            this.btnEliminarNotificacion.Name = "btnEliminarNotificacion";
            this.btnEliminarNotificacion.Size = new System.Drawing.Size(145, 30);
            this.btnEliminarNotificacion.TabIndex = 5;
            this.btnEliminarNotificacion.Tag = "FormNotificationManagement.DeleteButton";
            this.btnEliminarNotificacion.Values.Text = "Eliminar seleccionada";
            this.btnEliminarNotificacion.Click += new System.EventHandler(this.btnEliminarNotificacion_Click);
            //
            // btnAgregarNotificacion
            //
            this.btnAgregarNotificacion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarNotificacion.Location = new System.Drawing.Point(14, 63);
            this.btnAgregarNotificacion.Name = "btnAgregarNotificacion";
            this.btnAgregarNotificacion.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarNotificacion.TabIndex = 4;
            this.btnAgregarNotificacion.Tag = "FormNotificationManagement.AddButton";
            this.btnAgregarNotificacion.Values.Text = "Agregar";
            this.btnAgregarNotificacion.Click += new System.EventHandler(this.btnAgregarNotificacion_Click);
            //
            // txtbMensajeNotificacion
            //
            this.txtbMensajeNotificacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbMensajeNotificacion.Location = new System.Drawing.Point(229, 20);
            this.txtbMensajeNotificacion.Name = "txtbMensajeNotificacion";
            this.txtbMensajeNotificacion.Size = new System.Drawing.Size(299, 20);
            this.txtbMensajeNotificacion.TabIndex = 3;
            this.txtbMensajeNotificacion.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblMensajeNotificacion
            //
            this.lblMensajeNotificacion.AutoSize = true;
            this.lblMensajeNotificacion.Location = new System.Drawing.Point(226, 0);
            this.lblMensajeNotificacion.Name = "lblMensajeNotificacion";
            this.lblMensajeNotificacion.Size = new System.Drawing.Size(68, 13);
            this.lblMensajeNotificacion.TabIndex = 2;
            this.lblMensajeNotificacion.Tag = "FormNotificationManagement.MessageLabel";
            this.lblMensajeNotificacion.Values.Text = "Mensaje:";
            this.lblMensajeNotificacion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbUsuarioDestino
            //
            this.cmbUsuarioDestino.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbUsuarioDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuarioDestino.FormattingEnabled = true;
            this.cmbUsuarioDestino.Location = new System.Drawing.Point(3, 20);
            this.cmbUsuarioDestino.Name = "cmbUsuarioDestino";
            this.cmbUsuarioDestino.Size = new System.Drawing.Size(210, 21);
            this.cmbUsuarioDestino.TabIndex = 1;
            this.cmbUsuarioDestino.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            //
            // lblUsuarioDestino
            //
            this.lblUsuarioDestino.AutoSize = true;
            this.lblUsuarioDestino.Location = new System.Drawing.Point(0, 0);
            this.lblUsuarioDestino.Name = "lblUsuarioDestino";
            this.lblUsuarioDestino.Size = new System.Drawing.Size(94, 13);
            this.lblUsuarioDestino.TabIndex = 0;
            this.lblUsuarioDestino.Tag = "FormNotificationManagement.UserLabel";
            this.lblUsuarioDestino.Values.Text = "Usuario destino:";
            this.lblUsuarioDestino.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvNotificaciones
            //
            this.dgvNotificaciones.AllowUserToAddRows = false;
            this.dgvNotificaciones.AllowUserToDeleteRows = false;
            this.dgvNotificaciones.AutoGenerateColumns = false;
            this.dgvNotificaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNotificaciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvNotificaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotificaciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNotifUsuario,
            this.colNotifTipo,
            this.colNotifMensaje,
            this.colNotifFecha,
            this.colNotifLeida});
            this.dgvNotificaciones.Location = new System.Drawing.Point(23, 185);
            this.dgvNotificaciones.MultiSelect = false;
            this.dgvNotificaciones.Name = "dgvNotificaciones";
            this.dgvNotificaciones.RowHeadersVisible = false;
            this.dgvNotificaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNotificaciones.Size = new System.Drawing.Size(554, 320);
            this.dgvNotificaciones.TabIndex = 2;
            this.dgvNotificaciones.Tag = "FormNotificationManagement.Grid";
            //
            // colNotifUsuario
            //
            this.colNotifUsuario.DataPropertyName = "Usuario";
            this.colNotifUsuario.HeaderText = "Usuario";
            this.colNotifUsuario.Name = "Usuario";
            this.colNotifUsuario.ReadOnly = true;
            //
            // colNotifTipo
            //
            this.colNotifTipo.DataPropertyName = "Tipo";
            this.colNotifTipo.HeaderText = "Tipo";
            this.colNotifTipo.Name = "Tipo";
            this.colNotifTipo.ReadOnly = true;
            //
            // colNotifMensaje
            //
            this.colNotifMensaje.DataPropertyName = "Mensaje";
            this.colNotifMensaje.HeaderText = "Mensaje";
            this.colNotifMensaje.Name = "Mensaje";
            this.colNotifMensaje.ReadOnly = true;
            //
            // colNotifFecha
            //
            this.colNotifFecha.DataPropertyName = "Fecha";
            this.colNotifFecha.HeaderText = "Fecha";
            this.colNotifFecha.Name = "Fecha";
            this.colNotifFecha.ReadOnly = true;
            //
            // colNotifLeida
            //
            this.colNotifLeida.DataPropertyName = "Leida";
            this.colNotifLeida.HeaderText = "Leida";
            this.colNotifLeida.Name = "Leida";
            // 
            // tlpCamposNotificacion
            // 
            this.tlpCamposNotificacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposNotificacion.Controls.Add(this.txtbMensajeNotificacion, 1, 1);
            this.tlpCamposNotificacion.Controls.Add(this.lblMensajeNotificacion, 1, 0);
            this.tlpCamposNotificacion.Controls.Add(this.cmbUsuarioDestino, 0, 1);
            this.tlpCamposNotificacion.Controls.Add(this.lblUsuarioDestino, 0, 0);
            this.tlpCamposNotificacion.Location = new System.Drawing.Point(11, 8);
            this.tlpCamposNotificacion.Name = "tlpCamposNotificacion";
            this.tlpCamposNotificacion.Size = new System.Drawing.Size(528, 45);
            this.tlpCamposNotificacion.ColumnCount = 2;
            this.tlpCamposNotificacion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.8F));
            this.tlpCamposNotificacion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.2F));
            this.tlpCamposNotificacion.RowCount = 2;
            this.tlpCamposNotificacion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposNotificacion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposNotificacion.TabIndex = 0;
            //
            // FormGestionNotificaciones
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 525);
            this.Controls.Add(this.dgvNotificaciones);
            this.Controls.Add(this.groupBoxNotificacion);
            this.Controls.Add(this.lblTituloNotificaciones);
            this.Name = "FormGestionNotificaciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormNotificationManagement.Title";
            this.Text = "Gestion de Notificaciones";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNotificacion.Panel)).EndInit();
            this.groupBoxNotificacion.Panel.ResumeLayout(false);
            this.groupBoxNotificacion.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNotificacion)).EndInit();
            this.groupBoxNotificacion.ResumeLayout(false);
            this.groupBoxNotificacion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotificaciones)).EndInit();
            this.tlpCamposNotificacion.ResumeLayout(false);
            this.tlpCamposNotificacion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloNotificaciones;
        private Krypton.Toolkit.KryptonGroupBox groupBoxNotificacion;
        private Krypton.Toolkit.KryptonButton btnEliminarNotificacion;
        private Krypton.Toolkit.KryptonButton btnAgregarNotificacion;
        private Krypton.Toolkit.KryptonTextBox txtbMensajeNotificacion;
        private Krypton.Toolkit.KryptonLabel lblMensajeNotificacion;
        private Krypton.Toolkit.KryptonComboBox cmbUsuarioDestino;
        private Krypton.Toolkit.KryptonLabel lblUsuarioDestino;
        private Krypton.Toolkit.KryptonDataGridView dgvNotificaciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifMensaje;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifFecha;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNotifLeida;
        private System.Windows.Forms.TableLayoutPanel tlpCamposNotificacion;
    }
}
