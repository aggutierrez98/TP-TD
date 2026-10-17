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
            this.lblTituloGestionPermisos = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxCatalogoPermisos = new Krypton.Toolkit.KryptonGroupBox();
            this.tvwCatalogoPermisos = new System.Windows.Forms.TreeView();
            this.btnEliminarGrupo = new Krypton.Toolkit.KryptonButton();
            this.groupBoxEditorGrupo = new Krypton.Toolkit.KryptonGroupBox();
            this.lblNombreGrupo = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreGrupo = new Krypton.Toolkit.KryptonTextBox();
            this.btnCrearGrupo = new Krypton.Toolkit.KryptonButton();
            this.groupBoxGruposDisponibles = new Krypton.Toolkit.KryptonGroupBox();
            this.lstbGruposDisponibles = new Krypton.Toolkit.KryptonListBox();
            this.groupBoxPermisosDisponibles = new Krypton.Toolkit.KryptonGroupBox();
            this.lstbPermisosDisponibles = new Krypton.Toolkit.KryptonListBox();
            this.btnAgregarGrupoHijo = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarPermisoHijo = new Krypton.Toolkit.KryptonButton();
            this.groupBoxPreviewGrupo = new Krypton.Toolkit.KryptonGroupBox();
            this.tvwPreviewGrupo = new System.Windows.Forms.TreeView();
            this.btnEliminarSeleccionado = new Krypton.Toolkit.KryptonButton();
            this.btnGuardarGrupo = new Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCatalogoPermisos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCatalogoPermisos.Panel)).BeginInit();
            this.groupBoxCatalogoPermisos.Panel.SuspendLayout();
            this.groupBoxCatalogoPermisos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorGrupo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorGrupo.Panel)).BeginInit();
            this.groupBoxEditorGrupo.Panel.SuspendLayout();
            this.groupBoxEditorGrupo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGruposDisponibles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGruposDisponibles.Panel)).BeginInit();
            this.groupBoxGruposDisponibles.Panel.SuspendLayout();
            this.groupBoxGruposDisponibles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosDisponibles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosDisponibles.Panel)).BeginInit();
            this.groupBoxPermisosDisponibles.Panel.SuspendLayout();
            this.groupBoxPermisosDisponibles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPreviewGrupo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPreviewGrupo.Panel)).BeginInit();
            this.groupBoxPreviewGrupo.Panel.SuspendLayout();
            this.groupBoxPreviewGrupo.SuspendLayout();
            this.tlpListasEditorGrupo = new System.Windows.Forms.TableLayoutPanel();
            this.tlpListasEditorGrupo.SuspendLayout();
            this.tlpZonasPermisos = new System.Windows.Forms.TableLayoutPanel();
            this.tlpZonasPermisos.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloGestionPermisos
            //
            this.lblTituloGestionPermisos.AutoSize = true;
            this.lblTituloGestionPermisos.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionPermisos.Location = new System.Drawing.Point(12, 12);
            this.lblTituloGestionPermisos.Name = "lblTituloGestionPermisos";
            this.lblTituloGestionPermisos.Size = new System.Drawing.Size(192, 25);
            this.lblTituloGestionPermisos.TabIndex = 0;
            this.lblTituloGestionPermisos.Tag = "FormPermissionManagement.TitleLabel";
            this.lblTituloGestionPermisos.Values.Text = "Gestion de permisos";
            //
            // groupBoxCatalogoPermisos
            //
            this.groupBoxCatalogoPermisos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCatalogoPermisos.Panel.Controls.Add(this.tvwCatalogoPermisos);
            this.groupBoxCatalogoPermisos.Panel.Controls.Add(this.btnEliminarGrupo);
            this.groupBoxCatalogoPermisos.Location = new System.Drawing.Point(0, 0);
            this.groupBoxCatalogoPermisos.Name = "groupBoxCatalogoPermisos";
            this.groupBoxCatalogoPermisos.Size = new System.Drawing.Size(280, 460);
            this.groupBoxCatalogoPermisos.TabIndex = 1;
            this.groupBoxCatalogoPermisos.TabStop = false;
            this.groupBoxCatalogoPermisos.Tag = "FormPermissionManagement.TreeGroup";
            this.groupBoxCatalogoPermisos.Margin = new System.Windows.Forms.Padding(0, 0, 10, 4);
            //
            // tvwCatalogoPermisos
            //
            this.tvwCatalogoPermisos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwCatalogoPermisos.HideSelection = false;
            this.tvwCatalogoPermisos.Location = new System.Drawing.Point(10, 6);
            this.tvwCatalogoPermisos.Name = "tvwCatalogoPermisos";
            this.tvwCatalogoPermisos.Size = new System.Drawing.Size(260, 395);
            this.tvwCatalogoPermisos.TabIndex = 0;
            this.tvwCatalogoPermisos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwCatalogoPermisos_AfterSelect);
            //
            // btnEliminarGrupo
            // 
            this.btnEliminarGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarGrupo.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarGrupo.Location = new System.Drawing.Point(10, 407);
            this.btnEliminarGrupo.Name = "btnEliminarGrupo";
            this.btnEliminarGrupo.Size = new System.Drawing.Size(260, 28);
            this.btnEliminarGrupo.TabIndex = 1;
            this.btnEliminarGrupo.Tag = "FormPermissionManagement.DeleteGroupButton";
            this.btnEliminarGrupo.Click += new System.EventHandler(this.btnEliminarGrupo_Click);
            // 
            // groupBoxEditorGrupo
            // 
            this.groupBoxEditorGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEditorGrupo.Panel.Controls.Add(this.lblNombreGrupo);
            this.groupBoxEditorGrupo.Panel.Controls.Add(this.txtbNombreGrupo);
            this.groupBoxEditorGrupo.Panel.Controls.Add(this.btnCrearGrupo);
            this.groupBoxEditorGrupo.Panel.Controls.Add(this.tlpListasEditorGrupo);
            this.groupBoxEditorGrupo.Location = new System.Drawing.Point(290, 0);
            this.groupBoxEditorGrupo.Name = "groupBoxEditorGrupo";
            this.groupBoxEditorGrupo.Size = new System.Drawing.Size(420, 460);
            this.groupBoxEditorGrupo.TabIndex = 2;
            this.groupBoxEditorGrupo.TabStop = false;
            this.groupBoxEditorGrupo.Tag = "FormPermissionManagement.EditorGroup";
            this.groupBoxEditorGrupo.Margin = new System.Windows.Forms.Padding(0, 0, 10, 4);
            // 
            // lblNombreGrupo
            // 
            this.lblNombreGrupo.AutoSize = true;
            this.lblNombreGrupo.Location = new System.Drawing.Point(12, 8);
            this.lblNombreGrupo.Name = "lblNombreGrupo";
            this.lblNombreGrupo.Size = new System.Drawing.Size(70, 13);
            this.lblNombreGrupo.TabIndex = 0;
            this.lblNombreGrupo.Tag = "FormPermissionManagement.NewGroupLabel";
            this.lblNombreGrupo.Values.Text = "Nuevo grupo";
            // 
            // txtbNombreGrupo
            // 
            this.txtbNombreGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreGrupo.Location = new System.Drawing.Point(15, 31);
            this.txtbNombreGrupo.Name = "txtbNombreGrupo";
            this.txtbNombreGrupo.Size = new System.Drawing.Size(290, 20);
            this.txtbNombreGrupo.TabIndex = 1;
            this.txtbNombreGrupo.TextChanged += new System.EventHandler(this.txtbNombreGrupo_TextChanged);
            // 
            // btnCrearGrupo
            // 
            this.btnCrearGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrearGrupo.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnCrearGrupo.Location = new System.Drawing.Point(311, 29);
            this.btnCrearGrupo.Name = "btnCrearGrupo";
            this.btnCrearGrupo.Size = new System.Drawing.Size(95, 25);
            this.btnCrearGrupo.TabIndex = 2;
            this.btnCrearGrupo.Tag = "FormPermissionManagement.CreateGroupButton";
            this.btnCrearGrupo.Click += new System.EventHandler(this.btnCrearGrupo_Click);
            // 
            // groupBoxGruposDisponibles
            // 
            this.groupBoxGruposDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGruposDisponibles.Panel.Controls.Add(this.lstbGruposDisponibles);
            this.groupBoxGruposDisponibles.Location = new System.Drawing.Point(0, 0);
            this.groupBoxGruposDisponibles.Name = "groupBoxGruposDisponibles";
            this.groupBoxGruposDisponibles.Size = new System.Drawing.Size(195, 300);
            this.groupBoxGruposDisponibles.TabIndex = 3;
            this.groupBoxGruposDisponibles.TabStop = false;
            this.groupBoxGruposDisponibles.Tag = "FormPermissionManagement.GroupsListGroup";
            this.groupBoxGruposDisponibles.Margin = new System.Windows.Forms.Padding(0, 0, 6, 10);
            // 
            // lstbGruposDisponibles
            // 
            this.lstbGruposDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lstbGruposDisponibles.FormattingEnabled = true;
            this.lstbGruposDisponibles.Location = new System.Drawing.Point(8, 4);
            this.lstbGruposDisponibles.Name = "lstbGruposDisponibles";
            this.lstbGruposDisponibles.Size = new System.Drawing.Size(179, 270);
            this.lstbGruposDisponibles.TabIndex = 0;
            // 
            // groupBoxPermisosDisponibles
            // 
            this.groupBoxPermisosDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPermisosDisponibles.Panel.Controls.Add(this.lstbPermisosDisponibles);
            this.groupBoxPermisosDisponibles.Location = new System.Drawing.Point(201, 0);
            this.groupBoxPermisosDisponibles.Name = "groupBoxPermisosDisponibles";
            this.groupBoxPermisosDisponibles.Size = new System.Drawing.Size(195, 300);
            this.groupBoxPermisosDisponibles.TabIndex = 4;
            this.groupBoxPermisosDisponibles.TabStop = false;
            this.groupBoxPermisosDisponibles.Tag = "FormPermissionManagement.PermissionsListGroup";
            this.groupBoxPermisosDisponibles.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            // 
            // lstbPermisosDisponibles
            // 
            this.lstbPermisosDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lstbPermisosDisponibles.FormattingEnabled = true;
            this.lstbPermisosDisponibles.Location = new System.Drawing.Point(8, 4);
            this.lstbPermisosDisponibles.Name = "lstbPermisosDisponibles";
            this.lstbPermisosDisponibles.Size = new System.Drawing.Size(179, 270);
            this.lstbPermisosDisponibles.TabIndex = 0;
            // 
            // btnAgregarGrupoHijo
            // 
            this.btnAgregarGrupoHijo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarGrupoHijo.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarGrupoHijo.Location = new System.Drawing.Point(0, 310);
            this.btnAgregarGrupoHijo.Name = "btnAgregarGrupoHijo";
            this.btnAgregarGrupoHijo.Size = new System.Drawing.Size(195, 35);
            this.btnAgregarGrupoHijo.TabIndex = 5;
            this.btnAgregarGrupoHijo.Tag = "FormPermissionManagement.AddGroupButton";
            this.btnAgregarGrupoHijo.Click += new System.EventHandler(this.btnAgregarGrupoHijo_Click);
            this.btnAgregarGrupoHijo.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            // 
            // btnAgregarPermisoHijo
            // 
            this.btnAgregarPermisoHijo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarPermisoHijo.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarPermisoHijo.Location = new System.Drawing.Point(201, 310);
            this.btnAgregarPermisoHijo.Name = "btnAgregarPermisoHijo";
            this.btnAgregarPermisoHijo.Size = new System.Drawing.Size(195, 35);
            this.btnAgregarPermisoHijo.TabIndex = 6;
            this.btnAgregarPermisoHijo.Tag = "FormPermissionManagement.AddPermissionButton";
            this.btnAgregarPermisoHijo.Click += new System.EventHandler(this.btnAgregarPermisoHijo_Click);
            this.btnAgregarPermisoHijo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // groupBoxPreviewGrupo
            // 
            this.groupBoxPreviewGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPreviewGrupo.Panel.Controls.Add(this.tvwPreviewGrupo);
            this.groupBoxPreviewGrupo.Panel.Controls.Add(this.btnEliminarSeleccionado);
            this.groupBoxPreviewGrupo.Panel.Controls.Add(this.btnGuardarGrupo);
            this.groupBoxPreviewGrupo.Location = new System.Drawing.Point(720, 0);
            this.groupBoxPreviewGrupo.Name = "groupBoxPreviewGrupo";
            this.groupBoxPreviewGrupo.Size = new System.Drawing.Size(250, 460);
            this.groupBoxPreviewGrupo.TabIndex = 3;
            this.groupBoxPreviewGrupo.TabStop = false;
            this.groupBoxPreviewGrupo.Tag = "FormPermissionManagement.PreviewGroup";
            this.groupBoxPreviewGrupo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            // 
            // tvwPreviewGrupo
            // 
            this.tvwPreviewGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwPreviewGrupo.HideSelection = false;
            this.tvwPreviewGrupo.Location = new System.Drawing.Point(10, 6);
            this.tvwPreviewGrupo.Name = "tvwPreviewGrupo";
            this.tvwPreviewGrupo.Size = new System.Drawing.Size(230, 360);
            this.tvwPreviewGrupo.TabIndex = 0;
            //
            // btnEliminarSeleccionado
            //
            this.btnEliminarSeleccionado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarSeleccionado.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarSeleccionado.Location = new System.Drawing.Point(10, 372);
            this.btnEliminarSeleccionado.Name = "btnEliminarSeleccionado";
            this.btnEliminarSeleccionado.Size = new System.Drawing.Size(230, 28);
            this.btnEliminarSeleccionado.TabIndex = 1;
            this.btnEliminarSeleccionado.Tag = "FormPermissionManagement.RemoveSelectedButton";
            this.btnEliminarSeleccionado.Click += new System.EventHandler(this.btnEliminarSeleccionado_Click);
            //
            // btnGuardarGrupo
            // 
            this.btnGuardarGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardarGrupo.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnGuardarGrupo.Location = new System.Drawing.Point(10, 406);
            this.btnGuardarGrupo.Name = "btnGuardarGrupo";
            this.btnGuardarGrupo.Size = new System.Drawing.Size(230, 30);
            this.btnGuardarGrupo.TabIndex = 2;
            this.btnGuardarGrupo.Tag = "FormPermissionManagement.SaveGroupButton";
            this.btnGuardarGrupo.Click += new System.EventHandler(this.btnGuardarGrupo_Click);
            // 
            // tlpListasEditorGrupo
            // 
            this.tlpListasEditorGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpListasEditorGrupo.Controls.Add(this.groupBoxGruposDisponibles, 0, 0);
            this.tlpListasEditorGrupo.Controls.Add(this.groupBoxPermisosDisponibles, 1, 0);
            this.tlpListasEditorGrupo.Controls.Add(this.btnAgregarGrupoHijo, 0, 1);
            this.tlpListasEditorGrupo.Controls.Add(this.btnAgregarPermisoHijo, 1, 1);
            this.tlpListasEditorGrupo.Location = new System.Drawing.Point(12, 69);
            this.tlpListasEditorGrupo.Name = "tlpListasEditorGrupo";
            this.tlpListasEditorGrupo.Size = new System.Drawing.Size(396, 349);
            this.tlpListasEditorGrupo.ColumnCount = 2;
            this.tlpListasEditorGrupo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.76F));
            this.tlpListasEditorGrupo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.24F));
            this.tlpListasEditorGrupo.RowCount = 2;
            this.tlpListasEditorGrupo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpListasEditorGrupo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tlpListasEditorGrupo.TabIndex = 3;
            // 
            // tlpZonasPermisos
            // 
            this.tlpZonasPermisos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpZonasPermisos.Controls.Add(this.groupBoxPreviewGrupo, 2, 0);
            this.tlpZonasPermisos.Controls.Add(this.groupBoxEditorGrupo, 1, 0);
            this.tlpZonasPermisos.Controls.Add(this.groupBoxCatalogoPermisos, 0, 0);
            this.tlpZonasPermisos.Location = new System.Drawing.Point(15, 50);
            this.tlpZonasPermisos.Name = "tlpZonasPermisos";
            this.tlpZonasPermisos.Size = new System.Drawing.Size(970, 464);
            this.tlpZonasPermisos.ColumnCount = 3;
            this.tlpZonasPermisos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.9F));
            this.tlpZonasPermisos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.33F));
            this.tlpZonasPermisos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.77F));
            this.tlpZonasPermisos.RowCount = 1;
            this.tlpZonasPermisos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpZonasPermisos.TabIndex = 1;
            //
            // FormGestionPermisos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 525);
            this.Controls.Add(this.tlpZonasPermisos);
            this.Controls.Add(this.lblTituloGestionPermisos);
            this.Name = "FormGestionPermisos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPermissionManagement.Title";
            this.Text = "Gestion de permisos";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCatalogoPermisos.Panel)).EndInit();
            this.groupBoxCatalogoPermisos.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCatalogoPermisos)).EndInit();
            this.groupBoxCatalogoPermisos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorGrupo.Panel)).EndInit();
            this.groupBoxEditorGrupo.Panel.ResumeLayout(false);
            this.groupBoxEditorGrupo.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorGrupo)).EndInit();
            this.groupBoxEditorGrupo.ResumeLayout(false);
            this.groupBoxEditorGrupo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGruposDisponibles.Panel)).EndInit();
            this.groupBoxGruposDisponibles.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxGruposDisponibles)).EndInit();
            this.groupBoxGruposDisponibles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosDisponibles.Panel)).EndInit();
            this.groupBoxPermisosDisponibles.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPermisosDisponibles)).EndInit();
            this.groupBoxPermisosDisponibles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPreviewGrupo.Panel)).EndInit();
            this.groupBoxPreviewGrupo.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPreviewGrupo)).EndInit();
            this.groupBoxPreviewGrupo.ResumeLayout(false);
            this.tlpListasEditorGrupo.ResumeLayout(false);
            this.tlpListasEditorGrupo.PerformLayout();
            this.tlpZonasPermisos.ResumeLayout(false);
            this.tlpZonasPermisos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloGestionPermisos;
        private Krypton.Toolkit.KryptonGroupBox groupBoxCatalogoPermisos;
        private System.Windows.Forms.TreeView tvwCatalogoPermisos;
        private Krypton.Toolkit.KryptonButton btnEliminarGrupo;
        private Krypton.Toolkit.KryptonGroupBox groupBoxEditorGrupo;
        private Krypton.Toolkit.KryptonLabel lblNombreGrupo;
        private Krypton.Toolkit.KryptonTextBox txtbNombreGrupo;
        private Krypton.Toolkit.KryptonButton btnCrearGrupo;
        private Krypton.Toolkit.KryptonGroupBox groupBoxGruposDisponibles;
        private Krypton.Toolkit.KryptonListBox lstbGruposDisponibles;
        private Krypton.Toolkit.KryptonGroupBox groupBoxPermisosDisponibles;
        private Krypton.Toolkit.KryptonListBox lstbPermisosDisponibles;
        private Krypton.Toolkit.KryptonButton btnAgregarGrupoHijo;
        private Krypton.Toolkit.KryptonButton btnAgregarPermisoHijo;
        private Krypton.Toolkit.KryptonGroupBox groupBoxPreviewGrupo;
        private System.Windows.Forms.TreeView tvwPreviewGrupo;
        private Krypton.Toolkit.KryptonButton btnEliminarSeleccionado;
        private Krypton.Toolkit.KryptonButton btnGuardarGrupo;
        private System.Windows.Forms.TableLayoutPanel tlpListasEditorGrupo;
        private System.Windows.Forms.TableLayoutPanel tlpZonasPermisos;
    }
}
