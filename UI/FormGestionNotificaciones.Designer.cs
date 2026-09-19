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
            this.lblTituloNotificaciones = new System.Windows.Forms.Label();
            this.groupBoxNotificacion = new System.Windows.Forms.GroupBox();
            this.btnEliminarNotificacion = new System.Windows.Forms.Button();
            this.btnAgregarNotificacion = new System.Windows.Forms.Button();
            this.txtbMensajeNotificacion = new System.Windows.Forms.TextBox();
            this.lblMensajeNotificacion = new System.Windows.Forms.Label();
            this.cmbUsuarioDestino = new System.Windows.Forms.ComboBox();
            this.lblUsuarioDestino = new System.Windows.Forms.Label();
            this.dgvNotificaciones = new System.Windows.Forms.DataGridView();
            this.colNotifUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifMensaje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotifLeida = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBoxNotificacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotificaciones)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloNotificaciones
            //
            this.lblTituloNotificaciones.AutoSize = true;
            this.lblTituloNotificaciones.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloNotificaciones.Location = new System.Drawing.Point(18, 15);
            this.lblTituloNotificaciones.Name = "lblTituloNotificaciones";
            this.lblTituloNotificaciones.Size = new System.Drawing.Size(283, 25);
            this.lblTituloNotificaciones.TabIndex = 0;
            this.lblTituloNotificaciones.Tag = "FormNotificationManagement.TitleLabel";
            this.lblTituloNotificaciones.Text = "Gestion de Notificaciones";
            //
            // groupBoxNotificacion
            //
            this.groupBoxNotificacion.Controls.Add(this.btnEliminarNotificacion);
            this.groupBoxNotificacion.Controls.Add(this.btnAgregarNotificacion);
            this.groupBoxNotificacion.Controls.Add(this.txtbMensajeNotificacion);
            this.groupBoxNotificacion.Controls.Add(this.lblMensajeNotificacion);
            this.groupBoxNotificacion.Controls.Add(this.cmbUsuarioDestino);
            this.groupBoxNotificacion.Controls.Add(this.lblUsuarioDestino);
            this.groupBoxNotificacion.Location = new System.Drawing.Point(23, 52);
            this.groupBoxNotificacion.Name = "groupBoxNotificacion";
            this.groupBoxNotificacion.Size = new System.Drawing.Size(554, 116);
            this.groupBoxNotificacion.TabIndex = 1;
            this.groupBoxNotificacion.TabStop = false;
            this.groupBoxNotificacion.Tag = "FormNotificationManagement.NotificationGroup";
            this.groupBoxNotificacion.Text = "Nueva notificacion";
            //
            // btnEliminarNotificacion
            //
            this.btnEliminarNotificacion.Location = new System.Drawing.Point(394, 79);
            this.btnEliminarNotificacion.Name = "btnEliminarNotificacion";
            this.btnEliminarNotificacion.Size = new System.Drawing.Size(145, 30);
            this.btnEliminarNotificacion.TabIndex = 5;
            this.btnEliminarNotificacion.Tag = "FormNotificationManagement.DeleteButton";
            this.btnEliminarNotificacion.Text = "Eliminar seleccionada";
            this.btnEliminarNotificacion.UseVisualStyleBackColor = true;
            this.btnEliminarNotificacion.Click += new System.EventHandler(this.btnEliminarNotificacion_Click);
            //
            // btnAgregarNotificacion
            //
            this.btnAgregarNotificacion.Location = new System.Drawing.Point(14, 79);
            this.btnAgregarNotificacion.Name = "btnAgregarNotificacion";
            this.btnAgregarNotificacion.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarNotificacion.TabIndex = 4;
            this.btnAgregarNotificacion.Tag = "FormNotificationManagement.AddButton";
            this.btnAgregarNotificacion.Text = "Agregar";
            this.btnAgregarNotificacion.UseVisualStyleBackColor = true;
            this.btnAgregarNotificacion.Click += new System.EventHandler(this.btnAgregarNotificacion_Click);
            //
            // txtbMensajeNotificacion
            //
            this.txtbMensajeNotificacion.Location = new System.Drawing.Point(240, 44);
            this.txtbMensajeNotificacion.Name = "txtbMensajeNotificacion";
            this.txtbMensajeNotificacion.Size = new System.Drawing.Size(299, 20);
            this.txtbMensajeNotificacion.TabIndex = 3;
            //
            // lblMensajeNotificacion
            //
            this.lblMensajeNotificacion.AutoSize = true;
            this.lblMensajeNotificacion.Location = new System.Drawing.Point(237, 28);
            this.lblMensajeNotificacion.Name = "lblMensajeNotificacion";
            this.lblMensajeNotificacion.Size = new System.Drawing.Size(68, 13);
            this.lblMensajeNotificacion.TabIndex = 2;
            this.lblMensajeNotificacion.Tag = "FormNotificationManagement.MessageLabel";
            this.lblMensajeNotificacion.Text = "Mensaje:";
            //
            // cmbUsuarioDestino
            //
            this.cmbUsuarioDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuarioDestino.FormattingEnabled = true;
            this.cmbUsuarioDestino.Location = new System.Drawing.Point(14, 44);
            this.cmbUsuarioDestino.Name = "cmbUsuarioDestino";
            this.cmbUsuarioDestino.Size = new System.Drawing.Size(210, 21);
            this.cmbUsuarioDestino.TabIndex = 1;
            //
            // lblUsuarioDestino
            //
            this.lblUsuarioDestino.AutoSize = true;
            this.lblUsuarioDestino.Location = new System.Drawing.Point(11, 28);
            this.lblUsuarioDestino.Name = "lblUsuarioDestino";
            this.lblUsuarioDestino.Size = new System.Drawing.Size(94, 13);
            this.lblUsuarioDestino.TabIndex = 0;
            this.lblUsuarioDestino.Tag = "FormNotificationManagement.UserLabel";
            this.lblUsuarioDestino.Text = "Usuario destino:";
            //
            // dgvNotificaciones
            //
            this.dgvNotificaciones.AllowUserToAddRows = false;
            this.dgvNotificaciones.AllowUserToDeleteRows = false;
            this.dgvNotificaciones.AutoGenerateColumns = false;
            this.dgvNotificaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
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
            // FormGestionNotificaciones
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 525);
            this.Controls.Add(this.dgvNotificaciones);
            this.Controls.Add(this.groupBoxNotificacion);
            this.Controls.Add(this.lblTituloNotificaciones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionNotificaciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormNotificationManagement.Title";
            this.Text = "Gestion de Notificaciones";
            this.groupBoxNotificacion.ResumeLayout(false);
            this.groupBoxNotificacion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotificaciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloNotificaciones;
        private System.Windows.Forms.GroupBox groupBoxNotificacion;
        private System.Windows.Forms.Button btnEliminarNotificacion;
        private System.Windows.Forms.Button btnAgregarNotificacion;
        private System.Windows.Forms.TextBox txtbMensajeNotificacion;
        private System.Windows.Forms.Label lblMensajeNotificacion;
        private System.Windows.Forms.ComboBox cmbUsuarioDestino;
        private System.Windows.Forms.Label lblUsuarioDestino;
        private System.Windows.Forms.DataGridView dgvNotificaciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifMensaje;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotifFecha;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNotifLeida;
    }
}
