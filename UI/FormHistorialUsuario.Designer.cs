namespace TpIDS
{
    partial class FormHistorialUsuario
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
            this.lblTituloHistorialUsuario = new System.Windows.Forms.Label();
            this.pnlUsuarioHistorial = new System.Windows.Forms.FlowLayoutPanel();
            this.lblUsuarioHistorial = new System.Windows.Forms.Label();
            this.lblUsuarioHistorialNombre = new System.Windows.Forms.Label();
            this.dgvHistorialUsuario = new System.Windows.Forms.DataGridView();
            this.dgvCambiosEstado = new System.Windows.Forms.DataGridView();
            this.colHistorialHistoryId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialActorUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialActorUsernameSnapshot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialObservations = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colHistorialLanguageId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambiosField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambiosPreviousValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambiosNewValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRestaurarVersion = new System.Windows.Forms.Button();
            this.btnRefrescarHistorial = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambiosEstado)).BeginInit();
            this.pnlUsuarioHistorial.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloHistorialUsuario
            // 
            this.lblTituloHistorialUsuario.AutoSize = true;
            this.lblTituloHistorialUsuario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloHistorialUsuario.Location = new System.Drawing.Point(15, 15);
            this.lblTituloHistorialUsuario.Name = "lblTituloHistorialUsuario";
            this.lblTituloHistorialUsuario.Size = new System.Drawing.Size(0, 25);
            this.lblTituloHistorialUsuario.Tag = "FormUserHistory.TitleLabel";
            this.lblTituloHistorialUsuario.TabIndex = 0;
            //
            // pnlUsuarioHistorial
            //
            this.pnlUsuarioHistorial.AutoSize = true;
            this.pnlUsuarioHistorial.Controls.Add(this.lblUsuarioHistorial);
            this.pnlUsuarioHistorial.Controls.Add(this.lblUsuarioHistorialNombre);
            this.pnlUsuarioHistorial.Location = new System.Drawing.Point(17, 45);
            this.pnlUsuarioHistorial.Name = "pnlUsuarioHistorial";
            this.pnlUsuarioHistorial.Size = new System.Drawing.Size(60, 13);
            this.pnlUsuarioHistorial.TabIndex = 1;
            this.pnlUsuarioHistorial.WrapContents = false;
            //
            // lblUsuarioHistorial
            //
            this.lblUsuarioHistorial.AutoSize = true;
            this.lblUsuarioHistorial.Margin = new System.Windows.Forms.Padding(0);
            this.lblUsuarioHistorial.Name = "lblUsuarioHistorial";
            this.lblUsuarioHistorial.Size = new System.Drawing.Size(49, 13);
            this.lblUsuarioHistorial.TabIndex = 0;
            this.lblUsuarioHistorial.Tag = "FormUserHistory.UserPrefix";
            this.lblUsuarioHistorial.Text = "Usuario:";
            //
            // lblUsuarioHistorialNombre
            //
            this.lblUsuarioHistorialNombre.AutoSize = true;
            this.lblUsuarioHistorialNombre.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblUsuarioHistorialNombre.Name = "lblUsuarioHistorialNombre";
            this.lblUsuarioHistorialNombre.Size = new System.Drawing.Size(10, 13);
            this.lblUsuarioHistorialNombre.TabIndex = 1;
            this.lblUsuarioHistorialNombre.Text = "-";
            //
            // dgvHistorialUsuario
            // 
            this.dgvHistorialUsuario.AllowUserToAddRows = false;
            this.dgvHistorialUsuario.AllowUserToDeleteRows = false;
            this.dgvHistorialUsuario.AutoGenerateColumns = false;
            this.dgvHistorialUsuario.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorialUsuario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialUsuario.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHistorialHistoryId,
            this.colHistorialUserId,
            this.colHistorialDateTime,
            this.colHistorialActorUserId,
            this.colHistorialActorUsernameSnapshot,
            this.colHistorialAction,
            this.colHistorialObservations,
            this.colHistorialUsername,
            this.colHistorialActive,
            this.colHistorialLanguageId});
            this.dgvHistorialUsuario.Location = new System.Drawing.Point(20, 75);
            this.dgvHistorialUsuario.MultiSelect = false;
            this.dgvHistorialUsuario.Name = "dgvHistorialUsuario";
            this.dgvHistorialUsuario.ReadOnly = true;
            this.dgvHistorialUsuario.RowHeadersVisible = false;
            this.dgvHistorialUsuario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorialUsuario.Size = new System.Drawing.Size(760, 240);
            this.dgvHistorialUsuario.Tag = "FormUserHistory.Grid";
            this.dgvHistorialUsuario.TabIndex = 2;
            this.dgvHistorialUsuario.SelectionChanged += new System.EventHandler(this.dgvHistorialUsuario_SelectionChanged);
            // 
            // dgvCambiosEstado
            // 
            this.dgvCambiosEstado.AllowUserToAddRows = false;
            this.dgvCambiosEstado.AllowUserToDeleteRows = false;
            this.dgvCambiosEstado.AutoGenerateColumns = false;
            this.dgvCambiosEstado.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCambiosEstado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCambiosEstado.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCambiosField,
            this.colCambiosPreviousValue,
            this.colCambiosNewValue});
            this.dgvCambiosEstado.Location = new System.Drawing.Point(20, 330);
            this.dgvCambiosEstado.MultiSelect = false;
            this.dgvCambiosEstado.Name = "dgvCambiosEstado";
            this.dgvCambiosEstado.ReadOnly = true;
            this.dgvCambiosEstado.RowHeadersVisible = false;
            this.dgvCambiosEstado.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCambiosEstado.Size = new System.Drawing.Size(760, 180);
            this.dgvCambiosEstado.Tag = "FormUserHistory.DiffGrid";
            this.dgvCambiosEstado.TabIndex = 3;
            // 
            // btnRestaurarVersion
            // 
            this.btnRestaurarVersion.Location = new System.Drawing.Point(570, 525);
            this.btnRestaurarVersion.Name = "btnRestaurarVersion";
            this.btnRestaurarVersion.Size = new System.Drawing.Size(210, 30);
            this.btnRestaurarVersion.Tag = "FormUserHistory.RestoreButton";
            this.btnRestaurarVersion.TabIndex = 5;
            this.btnRestaurarVersion.UseVisualStyleBackColor = true;
            this.btnRestaurarVersion.Click += new System.EventHandler(this.btnRestaurarVersion_Click);
            // 
            // btnRefrescarHistorial
            // 
            this.btnRefrescarHistorial.Location = new System.Drawing.Point(20, 525);
            this.btnRefrescarHistorial.Name = "btnRefrescarHistorial";
            this.btnRefrescarHistorial.Size = new System.Drawing.Size(210, 30);
            this.btnRefrescarHistorial.Tag = "FormUserHistory.RefreshButton";
            this.btnRefrescarHistorial.TabIndex = 4;
            this.btnRefrescarHistorial.UseVisualStyleBackColor = true;
            this.btnRefrescarHistorial.Click += new System.EventHandler(this.btnRefrescarHistorial_Click);
            //
            // colHistorialHistoryId
            //
            this.colHistorialHistoryId.DataPropertyName = "HistorialId";
            this.colHistorialHistoryId.HeaderText = "Id historial";
            this.colHistorialHistoryId.Name = "HistoryId";
            this.colHistorialHistoryId.ReadOnly = true;
            //
            // colHistorialUserId
            //
            this.colHistorialUserId.DataPropertyName = "UsuarioId";
            this.colHistorialUserId.HeaderText = "Id usuario";
            this.colHistorialUserId.Name = "UserId";
            this.colHistorialUserId.ReadOnly = true;
            //
            // colHistorialDateTime
            //
            this.colHistorialDateTime.DataPropertyName = "FechaHora";
            this.colHistorialDateTime.HeaderText = "Fecha y hora";
            this.colHistorialDateTime.Name = "DateTime";
            this.colHistorialDateTime.ReadOnly = true;
            //
            // colHistorialActorUserId
            //
            this.colHistorialActorUserId.DataPropertyName = "ActorUsuarioId";
            this.colHistorialActorUserId.HeaderText = "Id actor";
            this.colHistorialActorUserId.Name = "ActorUserId";
            this.colHistorialActorUserId.ReadOnly = true;
            //
            // colHistorialActorUsernameSnapshot
            //
            this.colHistorialActorUsernameSnapshot.DataPropertyName = "ActorUsernameSnapshot";
            this.colHistorialActorUsernameSnapshot.HeaderText = "Actor";
            this.colHistorialActorUsernameSnapshot.Name = "ActorUsernameSnapshot";
            this.colHistorialActorUsernameSnapshot.ReadOnly = true;
            //
            // colHistorialAction
            //
            this.colHistorialAction.DataPropertyName = "Accion";
            this.colHistorialAction.HeaderText = "Accion";
            this.colHistorialAction.Name = "Action";
            this.colHistorialAction.ReadOnly = true;
            //
            // colHistorialObservations
            //
            this.colHistorialObservations.DataPropertyName = "Observaciones";
            this.colHistorialObservations.HeaderText = "Observaciones";
            this.colHistorialObservations.Name = "Observations";
            this.colHistorialObservations.ReadOnly = true;
            //
            // colHistorialUsername
            //
            this.colHistorialUsername.DataPropertyName = "Username";
            this.colHistorialUsername.HeaderText = "Usuario";
            this.colHistorialUsername.Name = "Username";
            this.colHistorialUsername.ReadOnly = true;
            //
            // colHistorialActive
            //
            this.colHistorialActive.DataPropertyName = "Activo";
            this.colHistorialActive.HeaderText = "Activo";
            this.colHistorialActive.Name = "Active";
            this.colHistorialActive.ReadOnly = true;
            //
            // colHistorialLanguageId
            //
            this.colHistorialLanguageId.DataPropertyName = "IdiomaId";
            this.colHistorialLanguageId.HeaderText = "Id idioma";
            this.colHistorialLanguageId.Name = "LanguageId";
            this.colHistorialLanguageId.ReadOnly = true;
            //
            // colCambiosField
            //
            this.colCambiosField.DataPropertyName = "Campo";
            this.colCambiosField.HeaderText = "Campo";
            this.colCambiosField.Name = "Field";
            this.colCambiosField.ReadOnly = true;
            //
            // colCambiosPreviousValue
            //
            this.colCambiosPreviousValue.DataPropertyName = "ValorAnterior";
            this.colCambiosPreviousValue.HeaderText = "Valor anterior";
            this.colCambiosPreviousValue.Name = "PreviousValue";
            this.colCambiosPreviousValue.ReadOnly = true;
            //
            // colCambiosNewValue
            //
            this.colCambiosNewValue.DataPropertyName = "ValorNuevo";
            this.colCambiosNewValue.HeaderText = "Valor nuevo";
            this.colCambiosNewValue.Name = "NewValue";
            this.colCambiosNewValue.ReadOnly = true;
            //
            // FormHistorialUsuario
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 575);
            this.Controls.Add(this.btnRefrescarHistorial);
            this.Controls.Add(this.btnRestaurarVersion);
            this.Controls.Add(this.dgvCambiosEstado);
            this.Controls.Add(this.dgvHistorialUsuario);
            this.Controls.Add(this.pnlUsuarioHistorial);
            this.Controls.Add(this.lblTituloHistorialUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormHistorialUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormUserHistory.Title";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialUsuario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambiosEstado)).EndInit();
            this.pnlUsuarioHistorial.ResumeLayout(false);
            this.pnlUsuarioHistorial.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloHistorialUsuario;
        private System.Windows.Forms.FlowLayoutPanel pnlUsuarioHistorial;
        private System.Windows.Forms.Label lblUsuarioHistorial;
        private System.Windows.Forms.Label lblUsuarioHistorialNombre;
        private System.Windows.Forms.DataGridView dgvHistorialUsuario;
        private System.Windows.Forms.DataGridView dgvCambiosEstado;
        private System.Windows.Forms.Button btnRestaurarVersion;
        private System.Windows.Forms.Button btnRefrescarHistorial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialHistoryId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialActorUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialActorUsernameSnapshot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialObservations;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialUsername;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colHistorialActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialLanguageId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambiosField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambiosPreviousValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambiosNewValue;
    }
}
