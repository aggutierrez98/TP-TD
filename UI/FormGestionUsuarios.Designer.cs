namespace TpIDS
{
    partial class FormGestionUsuarios
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
            this.lblTituloGestionUsuarios = new System.Windows.Forms.Label();
            this.lblSesionActual = new System.Windows.Forms.Label();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.colUsuarioId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuarioUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuarioActivo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBoxGestion = new System.Windows.Forms.GroupBox();
            this.lblPasswordUsuario = new System.Windows.Forms.Label();
            this.txtbPasswordUsuario = new System.Windows.Forms.TextBox();
            this.chkbUsuarioActivo = new System.Windows.Forms.CheckBox();
            this.lblNombreUsuario = new System.Windows.Forms.Label();
            this.txtbNombreUsuario = new System.Windows.Forms.TextBox();
            this.btnAgregarUsuario = new System.Windows.Forms.Button();
            this.btnEditarUsuario = new System.Windows.Forms.Button();
            this.btnEliminarUsuario = new System.Windows.Forms.Button();
            this.btnConfiguracion = new System.Windows.Forms.Button();
            this.btnGestionIdiomas = new System.Windows.Forms.Button();
            this.groupBoxPermisosUsuario = new System.Windows.Forms.GroupBox();
            this.tvwPermisosUsuario = new System.Windows.Forms.TreeView();
            this.btnGuardarPermisosUsuario = new System.Windows.Forms.Button();
            this.btnGestionPermisos = new System.Windows.Forms.Button();
            this.btnHistorialUsuario = new System.Windows.Forms.Button();
            this.btnBitacora = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.groupBoxGestion.SuspendLayout();
            this.groupBoxPermisosUsuario.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloGestionUsuarios
            // 
            this.lblTituloGestionUsuarios.AutoSize = true;
            this.lblTituloGestionUsuarios.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionUsuarios.Location = new System.Drawing.Point(12, 15);
            this.lblTituloGestionUsuarios.Name = "lblTituloGestionUsuarios";
            this.lblTituloGestionUsuarios.Size = new System.Drawing.Size(189, 25);
            this.lblTituloGestionUsuarios.TabIndex = 0;
            this.lblTituloGestionUsuarios.Tag = "FormGestionUsuarios.TitleLabel";
            this.lblTituloGestionUsuarios.Text = "Gestion de Usuarios";
            // 
            // lblSesionActual
            // 
            this.lblSesionActual.AutoSize = true;
            this.lblSesionActual.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblSesionActual.ForeColor = System.Drawing.Color.DimGray;
            this.lblSesionActual.Location = new System.Drawing.Point(14, 45);
            this.lblSesionActual.Name = "lblSesionActual";
            this.lblSesionActual.Size = new System.Drawing.Size(86, 15);
            this.lblSesionActual.TabIndex = 1;
            this.lblSesionActual.Text = "Sesion activa: -";
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.AutoGenerateColumns = false;
            this.dgvUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUsuarioId,
            this.colUsuarioUsername,
            this.colUsuarioActivo});
            this.dgvUsuarios.Location = new System.Drawing.Point(15, 70);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(530, 230);
            this.dgvUsuarios.TabIndex = 2;
            this.dgvUsuarios.Tag = "FormGestionUsuarios.Grid";
            this.dgvUsuarios.SelectionChanged += new System.EventHandler(this.dgvUsuarios_SelectionChanged);
            // 
            // colUsuarioId
            // 
            this.colUsuarioId.DataPropertyName = "Id";
            this.colUsuarioId.FillWeight = 50F;
            this.colUsuarioId.HeaderText = "FormGestionUsuarios.Grid.Id";
            this.colUsuarioId.Name = "Id";
            this.colUsuarioId.ReadOnly = true;
            this.colUsuarioId.Tag = "FormGestionUsuarios.Grid.Id";
            this.colUsuarioId.Width = 50;
            // 
            // colUsuarioUsername
            // 
            this.colUsuarioUsername.DataPropertyName = "Username";
            this.colUsuarioUsername.HeaderText = "FormGestionUsuarios.Grid.Username";
            this.colUsuarioUsername.Name = "Username";
            this.colUsuarioUsername.ReadOnly = true;
            this.colUsuarioUsername.Tag = "FormGestionUsuarios.Grid.Username";
            // 
            // colUsuarioActivo
            // 
            this.colUsuarioActivo.DataPropertyName = "Activo";
            this.colUsuarioActivo.FillWeight = 65F;
            this.colUsuarioActivo.HeaderText = "FormGestionUsuarios.Grid.Active";
            this.colUsuarioActivo.Name = "Activo";
            this.colUsuarioActivo.ReadOnly = true;
            this.colUsuarioActivo.Tag = "FormGestionUsuarios.Grid.Active";
            this.colUsuarioActivo.Width = 65;
            // 
            // groupBoxGestion
            // 
            this.groupBoxGestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGestion.Controls.Add(this.lblPasswordUsuario);
            this.groupBoxGestion.Controls.Add(this.txtbPasswordUsuario);
            this.groupBoxGestion.Controls.Add(this.chkbUsuarioActivo);
            this.groupBoxGestion.Controls.Add(this.lblNombreUsuario);
            this.groupBoxGestion.Controls.Add(this.txtbNombreUsuario);
            this.groupBoxGestion.Controls.Add(this.btnAgregarUsuario);
            this.groupBoxGestion.Controls.Add(this.btnEditarUsuario);
            this.groupBoxGestion.Controls.Add(this.btnEliminarUsuario);
            this.groupBoxGestion.Location = new System.Drawing.Point(15, 315);
            this.groupBoxGestion.Name = "groupBoxGestion";
            this.groupBoxGestion.Size = new System.Drawing.Size(530, 190);
            this.groupBoxGestion.TabIndex = 3;
            this.groupBoxGestion.TabStop = false;
            this.groupBoxGestion.Tag = "FormGestionUsuarios.UserDataGroup";
            this.groupBoxGestion.Text = "Datos del usuario";
            // 
            // lblPasswordUsuario
            // 
            this.lblPasswordUsuario.AutoSize = true;
            this.lblPasswordUsuario.Location = new System.Drawing.Point(10, 75);
            this.lblPasswordUsuario.Name = "lblPasswordUsuario";
            this.lblPasswordUsuario.Size = new System.Drawing.Size(257, 13);
            this.lblPasswordUsuario.TabIndex = 2;
            this.lblPasswordUsuario.Tag = "FormGestionUsuarios.PasswordLabel";
            this.lblPasswordUsuario.Text = "Contrasena (obligatoria al agregar, opcional al editar):";
            // 
            // txtbPasswordUsuario
            // 
            this.txtbPasswordUsuario.Location = new System.Drawing.Point(10, 92);
            this.txtbPasswordUsuario.Name = "txtbPasswordUsuario";
            this.txtbPasswordUsuario.PasswordChar = '●';
            this.txtbPasswordUsuario.Size = new System.Drawing.Size(514, 20);
            this.txtbPasswordUsuario.TabIndex = 3;
            // 
            // chkbUsuarioActivo
            // 
            this.chkbUsuarioActivo.AutoSize = true;
            this.chkbUsuarioActivo.Location = new System.Drawing.Point(10, 118);
            this.chkbUsuarioActivo.Name = "chkbUsuarioActivo";
            this.chkbUsuarioActivo.Size = new System.Drawing.Size(94, 17);
            this.chkbUsuarioActivo.TabIndex = 4;
            this.chkbUsuarioActivo.Tag = "FormGestionUsuarios.ActiveCheckbox";
            this.chkbUsuarioActivo.Text = "Usuario activo";
            this.chkbUsuarioActivo.UseVisualStyleBackColor = true;
            // 
            // lblNombreUsuario
            // 
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.Location = new System.Drawing.Point(10, 28);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new System.Drawing.Size(99, 13);
            this.lblNombreUsuario.TabIndex = 0;
            this.lblNombreUsuario.Tag = "FormGestionUsuarios.UsernameLabel";
            this.lblNombreUsuario.Text = "Nombre de usuario:";
            // 
            // txtbNombreUsuario
            // 
            this.txtbNombreUsuario.Location = new System.Drawing.Point(10, 45);
            this.txtbNombreUsuario.Name = "txtbNombreUsuario";
            this.txtbNombreUsuario.Size = new System.Drawing.Size(514, 20);
            this.txtbNombreUsuario.TabIndex = 1;
            // 
            // btnAgregarUsuario
            // 
            this.btnAgregarUsuario.Location = new System.Drawing.Point(10, 145);
            this.btnAgregarUsuario.Name = "btnAgregarUsuario";
            this.btnAgregarUsuario.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarUsuario.TabIndex = 5;
            this.btnAgregarUsuario.Tag = "FormGestionUsuarios.AddButton";
            this.btnAgregarUsuario.Text = "Agregar usuario";
            this.btnAgregarUsuario.UseVisualStyleBackColor = true;
            this.btnAgregarUsuario.Click += new System.EventHandler(this.btnAgregarUsuario_Click);
            // 
            // btnEditarUsuario
            // 
            this.btnEditarUsuario.Location = new System.Drawing.Point(157, 145);
            this.btnEditarUsuario.Name = "btnEditarUsuario";
            this.btnEditarUsuario.Size = new System.Drawing.Size(135, 30);
            this.btnEditarUsuario.TabIndex = 6;
            this.btnEditarUsuario.Tag = "FormGestionUsuarios.EditButton";
            this.btnEditarUsuario.Text = "Editar usuario";
            this.btnEditarUsuario.UseVisualStyleBackColor = true;
            this.btnEditarUsuario.Click += new System.EventHandler(this.btnEditarUsuario_Click);
            // 
            // btnEliminarUsuario
            // 
            this.btnEliminarUsuario.Location = new System.Drawing.Point(305, 145);
            this.btnEliminarUsuario.Name = "btnEliminarUsuario";
            this.btnEliminarUsuario.Size = new System.Drawing.Size(135, 30);
            this.btnEliminarUsuario.TabIndex = 7;
            this.btnEliminarUsuario.Tag = "FormGestionUsuarios.DeleteButton";
            this.btnEliminarUsuario.Text = "Eliminar usuario";
            this.btnEliminarUsuario.UseVisualStyleBackColor = true;
            this.btnEliminarUsuario.Click += new System.EventHandler(this.btnEliminarUsuario_Click);
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.Location = new System.Drawing.Point(15, 540);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(170, 30);
            this.btnConfiguracion.TabIndex = 4;
            this.btnConfiguracion.Tag = "FormGestionUsuarios.ConfigurationButton";
            this.btnConfiguracion.Text = "Configuracion";
            this.btnConfiguracion.UseVisualStyleBackColor = true;
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            // 
            // btnGestionIdiomas
            // 
            this.btnGestionIdiomas.Location = new System.Drawing.Point(195, 540);
            this.btnGestionIdiomas.Name = "btnGestionIdiomas";
            this.btnGestionIdiomas.Size = new System.Drawing.Size(170, 30);
            this.btnGestionIdiomas.TabIndex = 5;
            this.btnGestionIdiomas.Tag = "FormGestionUsuarios.LanguageManagementButton";
            this.btnGestionIdiomas.Text = "Gestion de idiomas";
            this.btnGestionIdiomas.UseVisualStyleBackColor = true;
            this.btnGestionIdiomas.Click += new System.EventHandler(this.btnGestionIdiomas_Click);
            // 
            // groupBoxPermisosUsuario
            // 
            this.groupBoxPermisosUsuario.Controls.Add(this.tvwPermisosUsuario);
            this.groupBoxPermisosUsuario.Controls.Add(this.btnGuardarPermisosUsuario);
            this.groupBoxPermisosUsuario.Location = new System.Drawing.Point(555, 70);
            this.groupBoxPermisosUsuario.Name = "groupBoxPermisosUsuario";
            this.groupBoxPermisosUsuario.Size = new System.Drawing.Size(465, 460);
            this.groupBoxPermisosUsuario.TabIndex = 6;
            this.groupBoxPermisosUsuario.TabStop = false;
            this.groupBoxPermisosUsuario.Tag = "FormGestionUsuarios.PermissionTreeGroup";
            this.groupBoxPermisosUsuario.Text = "Permisos y grupos asignados";
            // 
            // tvwPermisosUsuario
            // 
            this.tvwPermisosUsuario.CheckBoxes = true;
            this.tvwPermisosUsuario.HideSelection = false;
            this.tvwPermisosUsuario.Location = new System.Drawing.Point(10, 25);
            this.tvwPermisosUsuario.Name = "tvwPermisosUsuario";
            this.tvwPermisosUsuario.Size = new System.Drawing.Size(445, 385);
            this.tvwPermisosUsuario.TabIndex = 0;
            // 
            // btnGuardarPermisosUsuario
            // 
            this.btnGuardarPermisosUsuario.Location = new System.Drawing.Point(10, 420);
            this.btnGuardarPermisosUsuario.Name = "btnGuardarPermisosUsuario";
            this.btnGuardarPermisosUsuario.Size = new System.Drawing.Size(445, 30);
            this.btnGuardarPermisosUsuario.TabIndex = 1;
            this.btnGuardarPermisosUsuario.Tag = "FormGestionUsuarios.SavePermissionsButton";
            this.btnGuardarPermisosUsuario.Text = "Guardar permisos del usuario";
            this.btnGuardarPermisosUsuario.UseVisualStyleBackColor = true;
            this.btnGuardarPermisosUsuario.Click += new System.EventHandler(this.btnGuardarPermisosUsuario_Click);
            // 
            // btnGestionPermisos
            // 
            this.btnGestionPermisos.Location = new System.Drawing.Point(375, 540);
            this.btnGestionPermisos.Name = "btnGestionPermisos";
            this.btnGestionPermisos.Size = new System.Drawing.Size(170, 30);
            this.btnGestionPermisos.TabIndex = 7;
            this.btnGestionPermisos.Tag = "FormGestionUsuarios.PermissionManagementButton";
            this.btnGestionPermisos.Text = "Gestion de permisos";
            this.btnGestionPermisos.UseVisualStyleBackColor = true;
            this.btnGestionPermisos.Click += new System.EventHandler(this.btnGestionPermisos_Click);
            // 
            // btnHistorialUsuario
            // 
            this.btnHistorialUsuario.Location = new System.Drawing.Point(555, 540);
            this.btnHistorialUsuario.Name = "btnHistorialUsuario";
            this.btnHistorialUsuario.Size = new System.Drawing.Size(170, 30);
            this.btnHistorialUsuario.TabIndex = 8;
            this.btnHistorialUsuario.Tag = "FormGestionUsuarios.HistoryButton";
            this.btnHistorialUsuario.Text = "Historial de usuario";
            this.btnHistorialUsuario.UseVisualStyleBackColor = true;
            this.btnHistorialUsuario.Click += new System.EventHandler(this.btnHistorialUsuario_Click);
            // 
            // btnBitacora
            // 
            this.btnBitacora.Location = new System.Drawing.Point(735, 540);
            this.btnBitacora.Name = "btnBitacora";
            this.btnBitacora.Size = new System.Drawing.Size(170, 30);
            this.btnBitacora.TabIndex = 9;
            this.btnBitacora.Tag = "FormGestionUsuarios.BitacoraButton";
            this.btnBitacora.Text = "Bitacora";
            this.btnBitacora.UseVisualStyleBackColor = true;
            this.btnBitacora.Click += new System.EventHandler(this.btnBitacora_Click);
            // 
            // FormGestionUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 585);
            this.Controls.Add(this.btnBitacora);
            this.Controls.Add(this.btnHistorialUsuario);
            this.Controls.Add(this.btnGestionPermisos);
            this.Controls.Add(this.groupBoxPermisosUsuario);
            this.Controls.Add(this.btnGestionIdiomas);
            this.Controls.Add(this.btnConfiguracion);
            this.Controls.Add(this.lblTituloGestionUsuarios);
            this.Controls.Add(this.lblSesionActual);
            this.Controls.Add(this.dgvUsuarios);
            this.Controls.Add(this.groupBoxGestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormGestionUsuarios.Title";
            this.Text = "Gestion de Usuarios";
            this.Load += new System.EventHandler(this.FormGestionUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.groupBoxGestion.ResumeLayout(false);
            this.groupBoxGestion.PerformLayout();
            this.groupBoxPermisosUsuario.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloGestionUsuarios;
        private System.Windows.Forms.Label lblSesionActual;
        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuarioId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuarioUsername;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colUsuarioActivo;
        private System.Windows.Forms.GroupBox groupBoxGestion;
        private System.Windows.Forms.Label lblPasswordUsuario;
        private System.Windows.Forms.TextBox txtbPasswordUsuario;
        private System.Windows.Forms.CheckBox chkbUsuarioActivo;
        private System.Windows.Forms.Label lblNombreUsuario;
        private System.Windows.Forms.TextBox txtbNombreUsuario;
        private System.Windows.Forms.Button btnAgregarUsuario;
        private System.Windows.Forms.Button btnEditarUsuario;
        private System.Windows.Forms.Button btnEliminarUsuario;
        private System.Windows.Forms.Button btnConfiguracion;
        private System.Windows.Forms.Button btnGestionIdiomas;
        private System.Windows.Forms.GroupBox groupBoxPermisosUsuario;
        private System.Windows.Forms.TreeView tvwPermisosUsuario;
        private System.Windows.Forms.Button btnGuardarPermisosUsuario;
        private System.Windows.Forms.Button btnGestionPermisos;
        private System.Windows.Forms.Button btnHistorialUsuario;
        private System.Windows.Forms.Button btnBitacora;
    }
}
