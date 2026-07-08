namespace TpIDS
{
    partial class FormGestionPermisos
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
            this.lblTituloGestionPermisos = new System.Windows.Forms.Label();
            this.groupBoxCatalogoPermisos = new System.Windows.Forms.GroupBox();
            this.tvwCatalogoPermisos = new System.Windows.Forms.TreeView();
            this.btnEliminarGrupo = new System.Windows.Forms.Button();
            this.groupBoxEditorGrupo = new System.Windows.Forms.GroupBox();
            this.lblNombreGrupo = new System.Windows.Forms.Label();
            this.txtbNombreGrupo = new System.Windows.Forms.TextBox();
            this.btnCrearGrupo = new System.Windows.Forms.Button();
            this.groupBoxGruposDisponibles = new System.Windows.Forms.GroupBox();
            this.lstbGruposDisponibles = new System.Windows.Forms.ListBox();
            this.groupBoxPermisosDisponibles = new System.Windows.Forms.GroupBox();
            this.lstbPermisosDisponibles = new System.Windows.Forms.ListBox();
            this.btnAgregarGrupoHijo = new System.Windows.Forms.Button();
            this.btnAgregarPermisoHijo = new System.Windows.Forms.Button();
            this.groupBoxPreviewGrupo = new System.Windows.Forms.GroupBox();
            this.tvwPreviewGrupo = new System.Windows.Forms.TreeView();
            this.btnEliminarSeleccionado = new System.Windows.Forms.Button();
            this.btnGuardarGrupo = new System.Windows.Forms.Button();
            this.groupBoxCatalogoPermisos.SuspendLayout();
            this.groupBoxEditorGrupo.SuspendLayout();
            this.groupBoxGruposDisponibles.SuspendLayout();
            this.groupBoxPermisosDisponibles.SuspendLayout();
            this.groupBoxPreviewGrupo.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloGestionPermisos
            //
            this.lblTituloGestionPermisos.AutoSize = true;
            this.lblTituloGestionPermisos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionPermisos.Location = new System.Drawing.Point(12, 12);
            this.lblTituloGestionPermisos.Name = "lblTituloGestionPermisos";
            this.lblTituloGestionPermisos.Size = new System.Drawing.Size(192, 25);
            this.lblTituloGestionPermisos.TabIndex = 0;
            this.lblTituloGestionPermisos.Tag = "FormPermissionManagement.TitleLabel";
            this.lblTituloGestionPermisos.Text = "Gestion de permisos";
            //
            // groupBoxCatalogoPermisos
            //
            this.groupBoxCatalogoPermisos.Controls.Add(this.tvwCatalogoPermisos);
            this.groupBoxCatalogoPermisos.Controls.Add(this.btnEliminarGrupo);
            this.groupBoxCatalogoPermisos.Location = new System.Drawing.Point(15, 50);
            this.groupBoxCatalogoPermisos.Name = "groupBoxCatalogoPermisos";
            this.groupBoxCatalogoPermisos.Size = new System.Drawing.Size(280, 460);
            this.groupBoxCatalogoPermisos.TabIndex = 1;
            this.groupBoxCatalogoPermisos.TabStop = false;
            this.groupBoxCatalogoPermisos.Tag = "FormPermissionManagement.TreeGroup";
            //
            // tvwCatalogoPermisos
            //
            this.tvwCatalogoPermisos.HideSelection = false;
            this.tvwCatalogoPermisos.Location = new System.Drawing.Point(10, 22);
            this.tvwCatalogoPermisos.Name = "tvwCatalogoPermisos";
            this.tvwCatalogoPermisos.Size = new System.Drawing.Size(260, 395);
            this.tvwCatalogoPermisos.TabIndex = 0;
            this.tvwCatalogoPermisos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwCatalogoPermisos_AfterSelect);
            //
            // btnEliminarGrupo
            // 
            this.btnEliminarGrupo.Location = new System.Drawing.Point(10, 423);
            this.btnEliminarGrupo.Name = "btnEliminarGrupo";
            this.btnEliminarGrupo.Size = new System.Drawing.Size(260, 28);
            this.btnEliminarGrupo.TabIndex = 1;
            this.btnEliminarGrupo.Tag = "FormPermissionManagement.DeleteGroupButton";
            this.btnEliminarGrupo.UseVisualStyleBackColor = true;
            this.btnEliminarGrupo.Click += new System.EventHandler(this.btnEliminarGrupo_Click);
            // 
            // groupBoxEditorGrupo
            // 
            this.groupBoxEditorGrupo.Controls.Add(this.lblNombreGrupo);
            this.groupBoxEditorGrupo.Controls.Add(this.txtbNombreGrupo);
            this.groupBoxEditorGrupo.Controls.Add(this.btnCrearGrupo);
            this.groupBoxEditorGrupo.Controls.Add(this.groupBoxGruposDisponibles);
            this.groupBoxEditorGrupo.Controls.Add(this.groupBoxPermisosDisponibles);
            this.groupBoxEditorGrupo.Controls.Add(this.btnAgregarGrupoHijo);
            this.groupBoxEditorGrupo.Controls.Add(this.btnAgregarPermisoHijo);
            this.groupBoxEditorGrupo.Location = new System.Drawing.Point(305, 50);
            this.groupBoxEditorGrupo.Name = "groupBoxEditorGrupo";
            this.groupBoxEditorGrupo.Size = new System.Drawing.Size(420, 460);
            this.groupBoxEditorGrupo.TabIndex = 2;
            this.groupBoxEditorGrupo.TabStop = false;
            this.groupBoxEditorGrupo.Tag = "FormPermissionManagement.EditorGroup";
            // 
            // lblNombreGrupo
            // 
            this.lblNombreGrupo.AutoSize = true;
            this.lblNombreGrupo.Location = new System.Drawing.Point(12, 28);
            this.lblNombreGrupo.Name = "lblNombreGrupo";
            this.lblNombreGrupo.Size = new System.Drawing.Size(70, 13);
            this.lblNombreGrupo.TabIndex = 0;
            this.lblNombreGrupo.Tag = "FormPermissionManagement.NewGroupLabel";
            this.lblNombreGrupo.Text = "Nuevo grupo";
            // 
            // txtbNombreGrupo
            // 
            this.txtbNombreGrupo.Location = new System.Drawing.Point(15, 47);
            this.txtbNombreGrupo.Name = "txtbNombreGrupo";
            this.txtbNombreGrupo.Size = new System.Drawing.Size(290, 20);
            this.txtbNombreGrupo.TabIndex = 1;
            this.txtbNombreGrupo.TextChanged += new System.EventHandler(this.txtbNombreGrupo_TextChanged);
            // 
            // btnCrearGrupo
            // 
            this.btnCrearGrupo.Location = new System.Drawing.Point(311, 45);
            this.btnCrearGrupo.Name = "btnCrearGrupo";
            this.btnCrearGrupo.Size = new System.Drawing.Size(95, 25);
            this.btnCrearGrupo.TabIndex = 2;
            this.btnCrearGrupo.Tag = "FormPermissionManagement.CreateGroupButton";
            this.btnCrearGrupo.UseVisualStyleBackColor = true;
            this.btnCrearGrupo.Click += new System.EventHandler(this.btnCrearGrupo_Click);
            // 
            // groupBoxGruposDisponibles
            // 
            this.groupBoxGruposDisponibles.Controls.Add(this.lstbGruposDisponibles);
            this.groupBoxGruposDisponibles.Location = new System.Drawing.Point(12, 85);
            this.groupBoxGruposDisponibles.Name = "groupBoxGruposDisponibles";
            this.groupBoxGruposDisponibles.Size = new System.Drawing.Size(195, 300);
            this.groupBoxGruposDisponibles.TabIndex = 3;
            this.groupBoxGruposDisponibles.TabStop = false;
            this.groupBoxGruposDisponibles.Tag = "FormPermissionManagement.GroupsListGroup";
            // 
            // lstbGruposDisponibles
            // 
            this.lstbGruposDisponibles.FormattingEnabled = true;
            this.lstbGruposDisponibles.IntegralHeight = false;
            this.lstbGruposDisponibles.Location = new System.Drawing.Point(8, 20);
            this.lstbGruposDisponibles.Name = "lstbGruposDisponibles";
            this.lstbGruposDisponibles.Size = new System.Drawing.Size(179, 270);
            this.lstbGruposDisponibles.TabIndex = 0;
            // 
            // groupBoxPermisosDisponibles
            // 
            this.groupBoxPermisosDisponibles.Controls.Add(this.lstbPermisosDisponibles);
            this.groupBoxPermisosDisponibles.Location = new System.Drawing.Point(213, 85);
            this.groupBoxPermisosDisponibles.Name = "groupBoxPermisosDisponibles";
            this.groupBoxPermisosDisponibles.Size = new System.Drawing.Size(195, 300);
            this.groupBoxPermisosDisponibles.TabIndex = 4;
            this.groupBoxPermisosDisponibles.TabStop = false;
            this.groupBoxPermisosDisponibles.Tag = "FormPermissionManagement.PermissionsListGroup";
            // 
            // lstbPermisosDisponibles
            // 
            this.lstbPermisosDisponibles.FormattingEnabled = true;
            this.lstbPermisosDisponibles.IntegralHeight = false;
            this.lstbPermisosDisponibles.Location = new System.Drawing.Point(8, 20);
            this.lstbPermisosDisponibles.Name = "lstbPermisosDisponibles";
            this.lstbPermisosDisponibles.Size = new System.Drawing.Size(179, 270);
            this.lstbPermisosDisponibles.TabIndex = 0;
            // 
            // btnAgregarGrupoHijo
            // 
            this.btnAgregarGrupoHijo.Location = new System.Drawing.Point(12, 395);
            this.btnAgregarGrupoHijo.Name = "btnAgregarGrupoHijo";
            this.btnAgregarGrupoHijo.Size = new System.Drawing.Size(195, 35);
            this.btnAgregarGrupoHijo.TabIndex = 5;
            this.btnAgregarGrupoHijo.Tag = "FormPermissionManagement.AddGroupButton";
            this.btnAgregarGrupoHijo.UseVisualStyleBackColor = true;
            this.btnAgregarGrupoHijo.Click += new System.EventHandler(this.btnAgregarGrupoHijo_Click);
            // 
            // btnAgregarPermisoHijo
            // 
            this.btnAgregarPermisoHijo.Location = new System.Drawing.Point(213, 395);
            this.btnAgregarPermisoHijo.Name = "btnAgregarPermisoHijo";
            this.btnAgregarPermisoHijo.Size = new System.Drawing.Size(195, 35);
            this.btnAgregarPermisoHijo.TabIndex = 6;
            this.btnAgregarPermisoHijo.Tag = "FormPermissionManagement.AddPermissionButton";
            this.btnAgregarPermisoHijo.UseVisualStyleBackColor = true;
            this.btnAgregarPermisoHijo.Click += new System.EventHandler(this.btnAgregarPermisoHijo_Click);
            // 
            // groupBoxPreviewGrupo
            // 
            this.groupBoxPreviewGrupo.Controls.Add(this.tvwPreviewGrupo);
            this.groupBoxPreviewGrupo.Controls.Add(this.btnEliminarSeleccionado);
            this.groupBoxPreviewGrupo.Controls.Add(this.btnGuardarGrupo);
            this.groupBoxPreviewGrupo.Location = new System.Drawing.Point(735, 50);
            this.groupBoxPreviewGrupo.Name = "groupBoxPreviewGrupo";
            this.groupBoxPreviewGrupo.Size = new System.Drawing.Size(250, 460);
            this.groupBoxPreviewGrupo.TabIndex = 3;
            this.groupBoxPreviewGrupo.TabStop = false;
            this.groupBoxPreviewGrupo.Tag = "FormPermissionManagement.PreviewGroup";
            // 
            // tvwPreviewGrupo
            // 
            this.tvwPreviewGrupo.HideSelection = false;
            this.tvwPreviewGrupo.Location = new System.Drawing.Point(10, 22);
            this.tvwPreviewGrupo.Name = "tvwPreviewGrupo";
            this.tvwPreviewGrupo.Size = new System.Drawing.Size(230, 360);
            this.tvwPreviewGrupo.TabIndex = 0;
            //
            // btnEliminarSeleccionado
            //
            this.btnEliminarSeleccionado.Location = new System.Drawing.Point(10, 388);
            this.btnEliminarSeleccionado.Name = "btnEliminarSeleccionado";
            this.btnEliminarSeleccionado.Size = new System.Drawing.Size(230, 28);
            this.btnEliminarSeleccionado.TabIndex = 1;
            this.btnEliminarSeleccionado.Tag = "FormPermissionManagement.RemoveSelectedButton";
            this.btnEliminarSeleccionado.UseVisualStyleBackColor = true;
            this.btnEliminarSeleccionado.Click += new System.EventHandler(this.btnEliminarSeleccionado_Click);
            //
            // btnGuardarGrupo
            // 
            this.btnGuardarGrupo.Location = new System.Drawing.Point(10, 422);
            this.btnGuardarGrupo.Name = "btnGuardarGrupo";
            this.btnGuardarGrupo.Size = new System.Drawing.Size(230, 30);
            this.btnGuardarGrupo.TabIndex = 2;
            this.btnGuardarGrupo.Tag = "FormPermissionManagement.SaveGroupButton";
            this.btnGuardarGrupo.UseVisualStyleBackColor = true;
            this.btnGuardarGrupo.Click += new System.EventHandler(this.btnGuardarGrupo_Click);
            //
            // FormGestionPermisos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 525);
            this.Controls.Add(this.groupBoxPreviewGrupo);
            this.Controls.Add(this.groupBoxEditorGrupo);
            this.Controls.Add(this.groupBoxCatalogoPermisos);
            this.Controls.Add(this.lblTituloGestionPermisos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionPermisos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPermissionManagement.Title";
            this.Text = "Gestion de permisos";
            this.groupBoxCatalogoPermisos.ResumeLayout(false);
            this.groupBoxEditorGrupo.ResumeLayout(false);
            this.groupBoxEditorGrupo.PerformLayout();
            this.groupBoxGruposDisponibles.ResumeLayout(false);
            this.groupBoxPermisosDisponibles.ResumeLayout(false);
            this.groupBoxPreviewGrupo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloGestionPermisos;
        private System.Windows.Forms.GroupBox groupBoxCatalogoPermisos;
        private System.Windows.Forms.TreeView tvwCatalogoPermisos;
        private System.Windows.Forms.Button btnEliminarGrupo;
        private System.Windows.Forms.GroupBox groupBoxEditorGrupo;
        private System.Windows.Forms.Label lblNombreGrupo;
        private System.Windows.Forms.TextBox txtbNombreGrupo;
        private System.Windows.Forms.Button btnCrearGrupo;
        private System.Windows.Forms.GroupBox groupBoxGruposDisponibles;
        private System.Windows.Forms.ListBox lstbGruposDisponibles;
        private System.Windows.Forms.GroupBox groupBoxPermisosDisponibles;
        private System.Windows.Forms.ListBox lstbPermisosDisponibles;
        private System.Windows.Forms.Button btnAgregarGrupoHijo;
        private System.Windows.Forms.Button btnAgregarPermisoHijo;
        private System.Windows.Forms.GroupBox groupBoxPreviewGrupo;
        private System.Windows.Forms.TreeView tvwPreviewGrupo;
        private System.Windows.Forms.Button btnEliminarSeleccionado;
        private System.Windows.Forms.Button btnGuardarGrupo;
    }
}
