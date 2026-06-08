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
            this.lblUsuarioHistorial = new System.Windows.Forms.Label();
            this.dgvHistorialUsuario = new System.Windows.Forms.DataGridView();
            this.dgvCambiosEstado = new System.Windows.Forms.DataGridView();
            this.colHistorialId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialUsuarioId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialFechaHora = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialActorUsuarioId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialActorUsernameSnapshot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialAccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialObservaciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistorialActivo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colHistorialIdiomaId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambioCampo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambioValorAnterior = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambioValorNuevo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRestaurarVersion = new System.Windows.Forms.Button();
            this.btnRefrescarHistorial = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambiosEstado)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTituloHistorialUsuario
            // 
            this.lblTituloHistorialUsuario.AutoSize = true;
            this.lblTituloHistorialUsuario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloHistorialUsuario.Location = new System.Drawing.Point(15, 15);
            this.lblTituloHistorialUsuario.Name = "lblTituloHistorialUsuario";
            this.lblTituloHistorialUsuario.Size = new System.Drawing.Size(0, 25);
            this.lblTituloHistorialUsuario.Tag = "FormHistorialUsuario.TitleLabel";
            this.lblTituloHistorialUsuario.TabIndex = 0;
            // 
            // lblUsuarioHistorial
            // 
            this.lblUsuarioHistorial.AutoSize = true;
            this.lblUsuarioHistorial.Location = new System.Drawing.Point(17, 48);
            this.lblUsuarioHistorial.Name = "lblUsuarioHistorial";
            this.lblUsuarioHistorial.Size = new System.Drawing.Size(0, 13);
            this.lblUsuarioHistorial.TabIndex = 1;
            // 
            // dgvHistorialUsuario
            // 
            this.dgvHistorialUsuario.AllowUserToAddRows = false;
            this.dgvHistorialUsuario.AllowUserToDeleteRows = false;
            this.dgvHistorialUsuario.AutoGenerateColumns = false;
            this.dgvHistorialUsuario.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorialUsuario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialUsuario.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHistorialId,
            this.colHistorialUsuarioId,
            this.colHistorialFechaHora,
            this.colHistorialActorUsuarioId,
            this.colHistorialActorUsernameSnapshot,
            this.colHistorialAccion,
            this.colHistorialObservaciones,
            this.colHistorialUsername,
            this.colHistorialActivo,
            this.colHistorialIdiomaId});
            this.dgvHistorialUsuario.Location = new System.Drawing.Point(20, 75);
            this.dgvHistorialUsuario.MultiSelect = false;
            this.dgvHistorialUsuario.Name = "dgvHistorialUsuario";
            this.dgvHistorialUsuario.ReadOnly = true;
            this.dgvHistorialUsuario.RowHeadersVisible = false;
            this.dgvHistorialUsuario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorialUsuario.Size = new System.Drawing.Size(760, 240);
            this.dgvHistorialUsuario.Tag = "FormHistorialUsuario.Grid";
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
            this.colCambioCampo,
            this.colCambioValorAnterior,
            this.colCambioValorNuevo});
            this.dgvCambiosEstado.Location = new System.Drawing.Point(20, 330);
            this.dgvCambiosEstado.MultiSelect = false;
            this.dgvCambiosEstado.Name = "dgvCambiosEstado";
            this.dgvCambiosEstado.ReadOnly = true;
            this.dgvCambiosEstado.RowHeadersVisible = false;
            this.dgvCambiosEstado.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCambiosEstado.Size = new System.Drawing.Size(760, 180);
            this.dgvCambiosEstado.Tag = "FormHistorialUsuario.DiffGrid";
            this.dgvCambiosEstado.TabIndex = 3;
            // 
            // colHistorialId
            // 
            this.colHistorialId.DataPropertyName = "HistorialId";
            this.colHistorialId.HeaderText = "FormHistorialUsuario.Grid.HistorialId";
            this.colHistorialId.Name = "HistorialId";
            this.colHistorialId.ReadOnly = true;
            this.colHistorialId.Tag = "FormHistorialUsuario.Grid.HistorialId";
            // 
            // colHistorialUsuarioId
            // 
            this.colHistorialUsuarioId.DataPropertyName = "UsuarioId";
            this.colHistorialUsuarioId.HeaderText = "FormHistorialUsuario.Grid.UsuarioId";
            this.colHistorialUsuarioId.Name = "UsuarioId";
            this.colHistorialUsuarioId.ReadOnly = true;
            this.colHistorialUsuarioId.Tag = "FormHistorialUsuario.Grid.UsuarioId";
            // 
            // colHistorialFechaHora
            // 
            this.colHistorialFechaHora.DataPropertyName = "FechaHora";
            this.colHistorialFechaHora.HeaderText = "FormHistorialUsuario.Grid.FechaHora";
            this.colHistorialFechaHora.Name = "FechaHora";
            this.colHistorialFechaHora.ReadOnly = true;
            this.colHistorialFechaHora.Tag = "FormHistorialUsuario.Grid.FechaHora";
            // 
            // colHistorialActorUsuarioId
            // 
            this.colHistorialActorUsuarioId.DataPropertyName = "ActorUsuarioId";
            this.colHistorialActorUsuarioId.HeaderText = "FormHistorialUsuario.Grid.ActorUsuarioId";
            this.colHistorialActorUsuarioId.Name = "ActorUsuarioId";
            this.colHistorialActorUsuarioId.ReadOnly = true;
            this.colHistorialActorUsuarioId.Tag = "FormHistorialUsuario.Grid.ActorUsuarioId";
            // 
            // colHistorialActorUsernameSnapshot
            // 
            this.colHistorialActorUsernameSnapshot.DataPropertyName = "ActorUsernameSnapshot";
            this.colHistorialActorUsernameSnapshot.HeaderText = "FormHistorialUsuario.Grid.ActorUsernameSnapshot";
            this.colHistorialActorUsernameSnapshot.Name = "ActorUsernameSnapshot";
            this.colHistorialActorUsernameSnapshot.ReadOnly = true;
            this.colHistorialActorUsernameSnapshot.Tag = "FormHistorialUsuario.Grid.ActorUsernameSnapshot";
            // 
            // colHistorialAccion
            // 
            this.colHistorialAccion.DataPropertyName = "Accion";
            this.colHistorialAccion.HeaderText = "FormHistorialUsuario.Grid.Accion";
            this.colHistorialAccion.Name = "Accion";
            this.colHistorialAccion.ReadOnly = true;
            this.colHistorialAccion.Tag = "FormHistorialUsuario.Grid.Accion";
            // 
            // colHistorialObservaciones
            // 
            this.colHistorialObservaciones.DataPropertyName = "Observaciones";
            this.colHistorialObservaciones.HeaderText = "FormHistorialUsuario.Grid.Observaciones";
            this.colHistorialObservaciones.Name = "Observaciones";
            this.colHistorialObservaciones.ReadOnly = true;
            this.colHistorialObservaciones.Tag = "FormHistorialUsuario.Grid.Observaciones";
            // 
            // colHistorialUsername
            // 
            this.colHistorialUsername.DataPropertyName = "Username";
            this.colHistorialUsername.HeaderText = "FormHistorialUsuario.Grid.Username";
            this.colHistorialUsername.Name = "Username";
            this.colHistorialUsername.ReadOnly = true;
            this.colHistorialUsername.Tag = "FormHistorialUsuario.Grid.Username";
            // 
            // colHistorialActivo
            // 
            this.colHistorialActivo.DataPropertyName = "Activo";
            this.colHistorialActivo.HeaderText = "FormHistorialUsuario.Grid.Activo";
            this.colHistorialActivo.Name = "Activo";
            this.colHistorialActivo.ReadOnly = true;
            this.colHistorialActivo.Tag = "FormHistorialUsuario.Grid.Activo";
            // 
            // colHistorialIdiomaId
            // 
            this.colHistorialIdiomaId.DataPropertyName = "IdiomaId";
            this.colHistorialIdiomaId.HeaderText = "FormHistorialUsuario.Grid.IdiomaId";
            this.colHistorialIdiomaId.Name = "IdiomaId";
            this.colHistorialIdiomaId.ReadOnly = true;
            this.colHistorialIdiomaId.Tag = "FormHistorialUsuario.Grid.IdiomaId";
            // 
            // colCambioCampo
            // 
            this.colCambioCampo.DataPropertyName = "Campo";
            this.colCambioCampo.HeaderText = "FormHistorialUsuario.DiffGrid.Campo";
            this.colCambioCampo.Name = "Campo";
            this.colCambioCampo.ReadOnly = true;
            this.colCambioCampo.Tag = "FormHistorialUsuario.DiffGrid.Campo";
            // 
            // colCambioValorAnterior
            // 
            this.colCambioValorAnterior.DataPropertyName = "ValorAnterior";
            this.colCambioValorAnterior.HeaderText = "FormHistorialUsuario.DiffGrid.ValorAnterior";
            this.colCambioValorAnterior.Name = "ValorAnterior";
            this.colCambioValorAnterior.ReadOnly = true;
            this.colCambioValorAnterior.Tag = "FormHistorialUsuario.DiffGrid.ValorAnterior";
            // 
            // colCambioValorNuevo
            // 
            this.colCambioValorNuevo.DataPropertyName = "ValorNuevo";
            this.colCambioValorNuevo.HeaderText = "FormHistorialUsuario.DiffGrid.ValorNuevo";
            this.colCambioValorNuevo.Name = "ValorNuevo";
            this.colCambioValorNuevo.ReadOnly = true;
            this.colCambioValorNuevo.Tag = "FormHistorialUsuario.DiffGrid.ValorNuevo";
            // 
            // btnRestaurarVersion
            // 
            this.btnRestaurarVersion.Location = new System.Drawing.Point(570, 525);
            this.btnRestaurarVersion.Name = "btnRestaurarVersion";
            this.btnRestaurarVersion.Size = new System.Drawing.Size(210, 30);
            this.btnRestaurarVersion.Tag = "FormHistorialUsuario.RestoreButton";
            this.btnRestaurarVersion.TabIndex = 5;
            this.btnRestaurarVersion.UseVisualStyleBackColor = true;
            this.btnRestaurarVersion.Click += new System.EventHandler(this.btnRestaurarVersion_Click);
            // 
            // btnRefrescarHistorial
            // 
            this.btnRefrescarHistorial.Location = new System.Drawing.Point(20, 525);
            this.btnRefrescarHistorial.Name = "btnRefrescarHistorial";
            this.btnRefrescarHistorial.Size = new System.Drawing.Size(210, 30);
            this.btnRefrescarHistorial.Tag = "FormHistorialUsuario.RefreshButton";
            this.btnRefrescarHistorial.TabIndex = 4;
            this.btnRefrescarHistorial.UseVisualStyleBackColor = true;
            this.btnRefrescarHistorial.Click += new System.EventHandler(this.btnRefrescarHistorial_Click);
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
            this.Controls.Add(this.lblUsuarioHistorial);
            this.Controls.Add(this.lblTituloHistorialUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormHistorialUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormHistorialUsuario.Title";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialUsuario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambiosEstado)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloHistorialUsuario;
        private System.Windows.Forms.Label lblUsuarioHistorial;
        private System.Windows.Forms.DataGridView dgvHistorialUsuario;
        private System.Windows.Forms.DataGridView dgvCambiosEstado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialUsuarioId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialFechaHora;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialActorUsuarioId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialActorUsernameSnapshot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialAccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialObservaciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialUsername;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colHistorialActivo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistorialIdiomaId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambioCampo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambioValorAnterior;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambioValorNuevo;
        private System.Windows.Forms.Button btnRestaurarVersion;
        private System.Windows.Forms.Button btnRefrescarHistorial;
    }
}
