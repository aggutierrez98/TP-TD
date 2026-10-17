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
            this.lblTituloGestionUsuarios = new Krypton.Toolkit.KryptonLabel();
            this.pnlSesionActual = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSesionActual = new Krypton.Toolkit.KryptonLabel();
            this.lblSesionActualUsuario = new Krypton.Toolkit.KryptonLabel();
            this.dgvUsuarios = new Krypton.Toolkit.KryptonDataGridView();
            this.groupBoxGestion = new Krypton.Toolkit.KryptonGroupBox();
            this.lblPasswordUsuario = new Krypton.Toolkit.KryptonLabel();
            this.txtbPasswordUsuario = new Krypton.Toolkit.KryptonTextBox();
            this.chkbUsuarioActivo = new TpIDS.CheckBoxAccesible();
            this.lblNombreUsuario = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreUsuario = new Krypton.Toolkit.KryptonTextBox();
            this.btnAgregarUsuario = new Krypton.Toolkit.KryptonButton();
            this.btnEditarUsuario = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarUsuario = new Krypton.Toolkit.KryptonButton();
            this.btnDesbloquearUsuario = new Krypton.Toolkit.KryptonButton();
            this.groupBoxPermisosUsuario = new Krypton.Toolkit.KryptonGroupBox();
            this.tvwPermisosUsuario = new System.Windows.Forms.TreeView();
            this.btnGuardarPermisosUsuario = new Krypton.Toolkit.KryptonButton();
            this.btnHistorialUsuario = new Krypton.Toolkit.KryptonButton();
            this.colUsuariosId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuariosUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuariosActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colUsuariosBloqueado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlSesionActual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGestion.Panel)).BeginInit();
            this.groupBoxGestion.Panel.SuspendLayout();
            this.groupBoxGestion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosUsuario.Panel)).BeginInit();
            this.groupBoxPermisosUsuario.Panel.SuspendLayout();
            this.groupBoxPermisosUsuario.SuspendLayout();
            this.pnlZonaUsuarios = new System.Windows.Forms.Panel();
            this.pnlZonaUsuarios.SuspendLayout();
            this.tlpZonasUsuarios = new System.Windows.Forms.TableLayoutPanel();
            this.tlpZonasUsuarios.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloGestionUsuarios
            // 
            this.lblTituloGestionUsuarios.AutoSize = true;
            this.lblTituloGestionUsuarios.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionUsuarios.Location = new System.Drawing.Point(12, 15);
            this.lblTituloGestionUsuarios.Name = "lblTituloGestionUsuarios";
            this.lblTituloGestionUsuarios.Size = new System.Drawing.Size(189, 25);
            this.lblTituloGestionUsuarios.TabIndex = 0;
            this.lblTituloGestionUsuarios.Tag = "FormUserManagement.TitleLabel";
            this.lblTituloGestionUsuarios.Values.Text = "Gestion de Usuarios";
            // 
            // pnlSesionActual
            // 
            this.pnlSesionActual.AutoSize = true;
            this.pnlSesionActual.Controls.Add(this.lblSesionActual);
            this.pnlSesionActual.Controls.Add(this.lblSesionActualUsuario);
            this.pnlSesionActual.Location = new System.Drawing.Point(12, 42);
            this.pnlSesionActual.Name = "pnlSesionActual";
            this.pnlSesionActual.Size = new System.Drawing.Size(110, 15);
            this.pnlSesionActual.TabIndex = 1;
            this.pnlSesionActual.WrapContents = false;
            // 
            // lblSesionActual
            // 
            this.lblSesionActual.AutoSize = true;
            this.lblSesionActual.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblSesionActual.StateCommon.ShortText.Color1 = System.Drawing.Color.DimGray;
            this.lblSesionActual.Location = new System.Drawing.Point(0, 0);
            this.lblSesionActual.Margin = new System.Windows.Forms.Padding(0);
            this.lblSesionActual.Name = "lblSesionActual";
            this.lblSesionActual.Size = new System.Drawing.Size(78, 15);
            this.lblSesionActual.TabIndex = 0;
            this.lblSesionActual.Tag = "FormUserManagement.SessionLabelPrefix";
            this.lblSesionActual.Values.Text = "Sesion activa:";
            // 
            // lblSesionActualUsuario
            // 
            this.lblSesionActualUsuario.AutoSize = true;
            this.lblSesionActualUsuario.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblSesionActualUsuario.StateCommon.ShortText.Color1 = System.Drawing.Color.DimGray;
            this.lblSesionActualUsuario.Location = new System.Drawing.Point(82, 0);
            this.lblSesionActualUsuario.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblSesionActualUsuario.Name = "lblSesionActualUsuario";
            this.lblSesionActualUsuario.Size = new System.Drawing.Size(12, 15);
            this.lblSesionActualUsuario.TabIndex = 1;
            this.lblSesionActualUsuario.Values.Text = "-";
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.AutoGenerateColumns = false;
            this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUsuariosId,
            this.colUsuariosUsername,
            this.colUsuariosActive,
            this.colUsuariosBloqueado});
            this.dgvUsuarios.Location = new System.Drawing.Point(0, 0);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(530, 230);
            this.dgvUsuarios.TabIndex = 2;
            this.dgvUsuarios.Tag = "FormUserManagement.Grid";
            this.dgvUsuarios.SelectionChanged += new System.EventHandler(this.dgvUsuarios_SelectionChanged);
            // 
            // groupBoxGestion
            // 
            this.groupBoxGestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGestion.Panel.Controls.Add(this.lblPasswordUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.txtbPasswordUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.chkbUsuarioActivo);
            this.groupBoxGestion.Panel.Controls.Add(this.lblNombreUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.txtbNombreUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.btnAgregarUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.btnEditarUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.btnEliminarUsuario);
            this.groupBoxGestion.Panel.Controls.Add(this.btnDesbloquearUsuario);
            this.groupBoxGestion.Location = new System.Drawing.Point(0, 245);
            this.groupBoxGestion.Name = "groupBoxGestion";
            this.groupBoxGestion.Size = new System.Drawing.Size(530, 190);
            this.groupBoxGestion.TabIndex = 3;
            this.groupBoxGestion.TabStop = false;
            this.groupBoxGestion.Tag = "FormUserManagement.UserDataGroup";
            this.groupBoxGestion.Values.Heading = "Datos del usuario";
            // 
            // lblPasswordUsuario
            // 
            this.lblPasswordUsuario.AutoSize = true;
            this.lblPasswordUsuario.Location = new System.Drawing.Point(10, 55);
            this.lblPasswordUsuario.Name = "lblPasswordUsuario";
            this.lblPasswordUsuario.Size = new System.Drawing.Size(257, 13);
            this.lblPasswordUsuario.TabIndex = 2;
            this.lblPasswordUsuario.Tag = "FormUserManagement.PasswordLabel";
            this.lblPasswordUsuario.Values.Text = "Contrasena (obligatoria al agregar, opcional al editar):";
            // 
            // txtbPasswordUsuario
            // 
            this.txtbPasswordUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbPasswordUsuario.Location = new System.Drawing.Point(10, 76);
            this.txtbPasswordUsuario.Name = "txtbPasswordUsuario";
            this.txtbPasswordUsuario.PasswordChar = '*';
            this.txtbPasswordUsuario.Size = new System.Drawing.Size(514, 20);
            this.txtbPasswordUsuario.TabIndex = 3;
            // 
            // chkbUsuarioActivo
            // 
            this.chkbUsuarioActivo.AutoSize = true;
            this.chkbUsuarioActivo.Location = new System.Drawing.Point(10, 102);
            this.chkbUsuarioActivo.Name = "chkbUsuarioActivo";
            this.chkbUsuarioActivo.Size = new System.Drawing.Size(94, 17);
            this.chkbUsuarioActivo.TabIndex = 4;
            this.chkbUsuarioActivo.Tag = "FormUserManagement.ActiveCheckbox";
            this.chkbUsuarioActivo.Values.Text = "Usuario activo";
            // 
            // lblNombreUsuario
            // 
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.Location = new System.Drawing.Point(10, 8);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new System.Drawing.Size(99, 13);
            this.lblNombreUsuario.TabIndex = 0;
            this.lblNombreUsuario.Tag = "FormUserManagement.UsernameLabel";
            this.lblNombreUsuario.Values.Text = "Nombre de usuario:";
            // 
            // txtbNombreUsuario
            // 
            this.txtbNombreUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreUsuario.Location = new System.Drawing.Point(10, 29);
            this.txtbNombreUsuario.Name = "txtbNombreUsuario";
            this.txtbNombreUsuario.Size = new System.Drawing.Size(514, 20);
            this.txtbNombreUsuario.TabIndex = 1;
            // 
            // btnAgregarUsuario
            // 
            this.btnAgregarUsuario.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarUsuario.Location = new System.Drawing.Point(10, 129);
            this.btnAgregarUsuario.Name = "btnAgregarUsuario";
            this.btnAgregarUsuario.Size = new System.Drawing.Size(124, 30);
            this.btnAgregarUsuario.TabIndex = 5;
            this.btnAgregarUsuario.Tag = "FormUserManagement.AddButton";
            this.btnAgregarUsuario.Values.Text = "Agregar usuario";
            this.btnAgregarUsuario.Click += new System.EventHandler(this.btnAgregarUsuario_Click);
            //
            // btnEditarUsuario
            //
            this.btnEditarUsuario.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEditarUsuario.Location = new System.Drawing.Point(140, 129);
            this.btnEditarUsuario.Name = "btnEditarUsuario";
            this.btnEditarUsuario.Size = new System.Drawing.Size(124, 30);
            this.btnEditarUsuario.TabIndex = 6;
            this.btnEditarUsuario.Tag = "FormUserManagement.EditButton";
            this.btnEditarUsuario.Values.Text = "Editar usuario";
            this.btnEditarUsuario.Click += new System.EventHandler(this.btnEditarUsuario_Click);
            //
            // btnEliminarUsuario
            //
            this.btnEliminarUsuario.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarUsuario.Location = new System.Drawing.Point(270, 129);
            this.btnEliminarUsuario.Name = "btnEliminarUsuario";
            this.btnEliminarUsuario.Size = new System.Drawing.Size(124, 30);
            this.btnEliminarUsuario.TabIndex = 7;
            this.btnEliminarUsuario.Tag = "FormUserManagement.DeleteButton";
            this.btnEliminarUsuario.Values.Text = "Eliminar usuario";
            this.btnEliminarUsuario.Click += new System.EventHandler(this.btnEliminarUsuario_Click);
            //
            // btnDesbloquearUsuario
            //
            this.btnDesbloquearUsuario.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnDesbloquearUsuario.Location = new System.Drawing.Point(400, 129);
            this.btnDesbloquearUsuario.Name = "btnDesbloquearUsuario";
            this.btnDesbloquearUsuario.Size = new System.Drawing.Size(124, 30);
            this.btnDesbloquearUsuario.TabIndex = 8;
            this.btnDesbloquearUsuario.Tag = "FormUserManagement.UnlockButton";
            this.btnDesbloquearUsuario.Values.Text = "Desbloquear";
            this.btnDesbloquearUsuario.Click += new System.EventHandler(this.btnDesbloquearUsuario_Click);
            //
            // groupBoxPermisosUsuario
            // 
            this.groupBoxPermisosUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPermisosUsuario.Panel.Controls.Add(this.tvwPermisosUsuario);
            this.groupBoxPermisosUsuario.Panel.Controls.Add(this.btnGuardarPermisosUsuario);
            this.groupBoxPermisosUsuario.Location = new System.Drawing.Point(540, 0);
            this.groupBoxPermisosUsuario.Name = "groupBoxPermisosUsuario";
            this.groupBoxPermisosUsuario.Size = new System.Drawing.Size(570, 460);
            this.groupBoxPermisosUsuario.TabIndex = 6;
            this.groupBoxPermisosUsuario.TabStop = false;
            this.groupBoxPermisosUsuario.Tag = "FormUserManagement.PermissionTreeGroup";
            this.groupBoxPermisosUsuario.Values.Heading = "Permisos y grupos asignados";
            this.groupBoxPermisosUsuario.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            // 
            // tvwPermisosUsuario
            // 
            this.tvwPermisosUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwPermisosUsuario.CheckBoxes = true;
            this.tvwPermisosUsuario.HideSelection = false;
            this.tvwPermisosUsuario.Location = new System.Drawing.Point(10, 9);
            this.tvwPermisosUsuario.Name = "tvwPermisosUsuario";
            this.tvwPermisosUsuario.Size = new System.Drawing.Size(545, 385);
            this.tvwPermisosUsuario.TabIndex = 0;
            // 
            // btnGuardarPermisosUsuario
            // 
            this.btnGuardarPermisosUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardarPermisosUsuario.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnGuardarPermisosUsuario.Location = new System.Drawing.Point(10, 404);
            this.btnGuardarPermisosUsuario.Name = "btnGuardarPermisosUsuario";
            this.btnGuardarPermisosUsuario.Size = new System.Drawing.Size(545, 30);
            this.btnGuardarPermisosUsuario.TabIndex = 1;
            this.btnGuardarPermisosUsuario.Tag = "FormUserManagement.SavePermissionsButton";
            this.btnGuardarPermisosUsuario.Values.Text = "Guardar permisos del usuario";
            this.btnGuardarPermisosUsuario.Click += new System.EventHandler(this.btnGuardarPermisosUsuario_Click);
            //
            // btnHistorialUsuario
            //
            this.btnHistorialUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHistorialUsuario.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnHistorialUsuario.Location = new System.Drawing.Point(15, 540);
            this.btnHistorialUsuario.Name = "btnHistorialUsuario";
            this.btnHistorialUsuario.Size = new System.Drawing.Size(170, 30);
            this.btnHistorialUsuario.TabIndex = 7;
            this.btnHistorialUsuario.Tag = "FormUserManagement.HistoryButton";
            this.btnHistorialUsuario.Values.Text = "Historial de usuario";
            this.btnHistorialUsuario.Click += new System.EventHandler(this.btnHistorialUsuario_Click);
            //
            // colUsuariosId
            //
            this.colUsuariosId.DataPropertyName = "Id";
            this.colUsuariosId.HeaderText = "Id";
            this.colUsuariosId.Name = "Id";
            this.colUsuariosId.ReadOnly = true;
            //
            // colUsuariosUsername
            //
            this.colUsuariosUsername.DataPropertyName = "Username";
            this.colUsuariosUsername.HeaderText = "Usuario";
            this.colUsuariosUsername.Name = "Username";
            this.colUsuariosUsername.ReadOnly = true;
            //
            // colUsuariosActive
            //
            this.colUsuariosActive.DataPropertyName = "Activo";
            this.colUsuariosActive.HeaderText = "Activo";
            this.colUsuariosActive.Name = "Active";
            this.colUsuariosActive.ReadOnly = true;
            //
            // colUsuariosBloqueado
            //
            this.colUsuariosBloqueado.DataPropertyName = "Bloqueado";
            this.colUsuariosBloqueado.HeaderText = "Bloqueado";
            this.colUsuariosBloqueado.Name = "Bloqueado";
            this.colUsuariosBloqueado.ReadOnly = true;
            // 
            // pnlZonaUsuarios
            // 
            this.pnlZonaUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlZonaUsuarios.Controls.Add(this.dgvUsuarios);
            this.pnlZonaUsuarios.Controls.Add(this.groupBoxGestion);
            this.pnlZonaUsuarios.Location = new System.Drawing.Point(0, 0);
            this.pnlZonaUsuarios.Name = "pnlZonaUsuarios";
            this.pnlZonaUsuarios.Size = new System.Drawing.Size(530, 435);
            this.pnlZonaUsuarios.Margin = new System.Windows.Forms.Padding(0, 0, 10, 29);
            // 
            // tlpZonasUsuarios
            // 
            this.tlpZonasUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpZonasUsuarios.Controls.Add(this.groupBoxPermisosUsuario, 1, 0);
            this.tlpZonasUsuarios.Controls.Add(this.pnlZonaUsuarios, 0, 0);
            this.tlpZonasUsuarios.Location = new System.Drawing.Point(15, 70);
            this.tlpZonasUsuarios.Name = "tlpZonasUsuarios";
            this.tlpZonasUsuarios.Size = new System.Drawing.Size(1110, 464);
            this.tlpZonasUsuarios.ColumnCount = 2;
            this.tlpZonasUsuarios.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.65F));
            this.tlpZonasUsuarios.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.35F));
            this.tlpZonasUsuarios.RowCount = 1;
            this.tlpZonasUsuarios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpZonasUsuarios.TabIndex = 6;
            //
            // FormGestionUsuarios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 585);
            this.Controls.Add(this.btnHistorialUsuario);
            this.Controls.Add(this.tlpZonasUsuarios);
            this.Controls.Add(this.lblTituloGestionUsuarios);
            this.Controls.Add(this.pnlSesionActual);
            this.Name = "FormGestionUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormUserManagement.Title";
            this.Text = "Gestion de Usuarios";
            this.Load += new System.EventHandler(this.FormGestionUsuarios_Load);
            this.pnlSesionActual.ResumeLayout(false);
            this.pnlSesionActual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGestion.Panel)).EndInit();
            this.groupBoxGestion.Panel.ResumeLayout(false);
            this.groupBoxGestion.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGestion)).EndInit();
            this.groupBoxGestion.ResumeLayout(false);
            this.groupBoxGestion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosUsuario.Panel)).EndInit();
            this.groupBoxPermisosUsuario.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosUsuario)).EndInit();
            this.groupBoxPermisosUsuario.ResumeLayout(false);
            this.pnlZonaUsuarios.ResumeLayout(false);
            this.pnlZonaUsuarios.PerformLayout();
            this.tlpZonasUsuarios.ResumeLayout(false);
            this.tlpZonasUsuarios.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloGestionUsuarios;
        private System.Windows.Forms.FlowLayoutPanel pnlSesionActual;
        private Krypton.Toolkit.KryptonLabel lblSesionActual;
        private Krypton.Toolkit.KryptonLabel lblSesionActualUsuario;
        private Krypton.Toolkit.KryptonDataGridView dgvUsuarios;
        private Krypton.Toolkit.KryptonGroupBox groupBoxGestion;
        private Krypton.Toolkit.KryptonLabel lblPasswordUsuario;
        private Krypton.Toolkit.KryptonTextBox txtbPasswordUsuario;
        private TpIDS.CheckBoxAccesible chkbUsuarioActivo;
        private Krypton.Toolkit.KryptonLabel lblNombreUsuario;
        private Krypton.Toolkit.KryptonTextBox txtbNombreUsuario;
        private Krypton.Toolkit.KryptonButton btnAgregarUsuario;
        private Krypton.Toolkit.KryptonButton btnEditarUsuario;
        private Krypton.Toolkit.KryptonButton btnEliminarUsuario;
        private Krypton.Toolkit.KryptonButton btnDesbloquearUsuario;
        private Krypton.Toolkit.KryptonGroupBox groupBoxPermisosUsuario;
        private System.Windows.Forms.TreeView tvwPermisosUsuario;
        private Krypton.Toolkit.KryptonButton btnGuardarPermisosUsuario;
        private Krypton.Toolkit.KryptonButton btnHistorialUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuariosId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuariosUsername;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colUsuariosActive;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colUsuariosBloqueado;
        private System.Windows.Forms.Panel pnlZonaUsuarios;
        private System.Windows.Forms.TableLayoutPanel tlpZonasUsuarios;
    }
}
