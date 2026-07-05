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
            this.btnEliminarFamilia = new System.Windows.Forms.Button();
            this.groupBoxEditorFamilia = new System.Windows.Forms.GroupBox();
            this.lblNombreFamilia = new System.Windows.Forms.Label();
            this.txtbNombreFamilia = new System.Windows.Forms.TextBox();
            this.btnCrearFamilia = new System.Windows.Forms.Button();
            this.groupBoxFamiliasDisponibles = new System.Windows.Forms.GroupBox();
            this.lstbFamiliasDisponibles = new System.Windows.Forms.ListBox();
            this.groupBoxPatentesDisponibles = new System.Windows.Forms.GroupBox();
            this.lstbPatentesDisponibles = new System.Windows.Forms.ListBox();
            this.btnAgregarFamiliaHija = new System.Windows.Forms.Button();
            this.btnAgregarPatenteHija = new System.Windows.Forms.Button();
            this.groupBoxPreviewFamilia = new System.Windows.Forms.GroupBox();
            this.tvwPreviewFamilia = new System.Windows.Forms.TreeView();
            this.btnEliminarSeleccionado = new System.Windows.Forms.Button();
            this.btnGuardarFamilia = new System.Windows.Forms.Button();
            this.groupBoxCatalogoPermisos.SuspendLayout();
            this.groupBoxEditorFamilia.SuspendLayout();
            this.groupBoxFamiliasDisponibles.SuspendLayout();
            this.groupBoxPatentesDisponibles.SuspendLayout();
            this.groupBoxPreviewFamilia.SuspendLayout();
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
            this.groupBoxCatalogoPermisos.Controls.Add(this.btnEliminarFamilia);
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
            // btnEliminarFamilia
            //
            this.btnEliminarFamilia.Location = new System.Drawing.Point(10, 423);
            this.btnEliminarFamilia.Name = "btnEliminarFamilia";
            this.btnEliminarFamilia.Size = new System.Drawing.Size(260, 28);
            this.btnEliminarFamilia.TabIndex = 1;
            this.btnEliminarFamilia.Tag = "FormPermissionManagement.DeleteFamilyButton";
            this.btnEliminarFamilia.UseVisualStyleBackColor = true;
            this.btnEliminarFamilia.Click += new System.EventHandler(this.btnEliminarFamilia_Click);
            //
            // groupBoxEditorFamilia
            //
            this.groupBoxEditorFamilia.Controls.Add(this.lblNombreFamilia);
            this.groupBoxEditorFamilia.Controls.Add(this.txtbNombreFamilia);
            this.groupBoxEditorFamilia.Controls.Add(this.btnCrearFamilia);
            this.groupBoxEditorFamilia.Controls.Add(this.groupBoxFamiliasDisponibles);
            this.groupBoxEditorFamilia.Controls.Add(this.groupBoxPatentesDisponibles);
            this.groupBoxEditorFamilia.Controls.Add(this.btnAgregarFamiliaHija);
            this.groupBoxEditorFamilia.Controls.Add(this.btnAgregarPatenteHija);
            this.groupBoxEditorFamilia.Location = new System.Drawing.Point(305, 50);
            this.groupBoxEditorFamilia.Name = "groupBoxEditorFamilia";
            this.groupBoxEditorFamilia.Size = new System.Drawing.Size(420, 460);
            this.groupBoxEditorFamilia.TabIndex = 2;
            this.groupBoxEditorFamilia.TabStop = false;
            this.groupBoxEditorFamilia.Tag = "FormPermissionManagement.EditorGroup";
            //
            // lblNombreFamilia
            //
            this.lblNombreFamilia.AutoSize = true;
            this.lblNombreFamilia.Location = new System.Drawing.Point(12, 28);
            this.lblNombreFamilia.Name = "lblNombreFamilia";
            this.lblNombreFamilia.Size = new System.Drawing.Size(72, 13);
            this.lblNombreFamilia.TabIndex = 0;
            this.lblNombreFamilia.Tag = "FormPermissionManagement.NewFamilyLabel";
            this.lblNombreFamilia.Text = "Nueva familia";
            //
            // txtbNombreFamilia
            //
            this.txtbNombreFamilia.Location = new System.Drawing.Point(15, 47);
            this.txtbNombreFamilia.Name = "txtbNombreFamilia";
            this.txtbNombreFamilia.Size = new System.Drawing.Size(290, 20);
            this.txtbNombreFamilia.TabIndex = 1;
            this.txtbNombreFamilia.TextChanged += new System.EventHandler(this.txtbNombreFamilia_TextChanged);
            //
            // btnCrearFamilia
            //
            this.btnCrearFamilia.Location = new System.Drawing.Point(311, 45);
            this.btnCrearFamilia.Name = "btnCrearFamilia";
            this.btnCrearFamilia.Size = new System.Drawing.Size(95, 25);
            this.btnCrearFamilia.TabIndex = 2;
            this.btnCrearFamilia.Tag = "FormPermissionManagement.CreateFamilyButton";
            this.btnCrearFamilia.UseVisualStyleBackColor = true;
            this.btnCrearFamilia.Click += new System.EventHandler(this.btnCrearFamilia_Click);
            //
            // groupBoxFamiliasDisponibles
            //
            this.groupBoxFamiliasDisponibles.Controls.Add(this.lstbFamiliasDisponibles);
            this.groupBoxFamiliasDisponibles.Location = new System.Drawing.Point(12, 85);
            this.groupBoxFamiliasDisponibles.Name = "groupBoxFamiliasDisponibles";
            this.groupBoxFamiliasDisponibles.Size = new System.Drawing.Size(195, 300);
            this.groupBoxFamiliasDisponibles.TabIndex = 3;
            this.groupBoxFamiliasDisponibles.TabStop = false;
            this.groupBoxFamiliasDisponibles.Tag = "FormPermissionManagement.FamiliesListGroup";
            //
            // lstbFamiliasDisponibles
            //
            this.lstbFamiliasDisponibles.FormattingEnabled = true;
            this.lstbFamiliasDisponibles.IntegralHeight = false;
            this.lstbFamiliasDisponibles.Location = new System.Drawing.Point(8, 20);
            this.lstbFamiliasDisponibles.Name = "lstbFamiliasDisponibles";
            this.lstbFamiliasDisponibles.Size = new System.Drawing.Size(179, 270);
            this.lstbFamiliasDisponibles.TabIndex = 0;
            //
            // groupBoxPatentesDisponibles
            //
            this.groupBoxPatentesDisponibles.Controls.Add(this.lstbPatentesDisponibles);
            this.groupBoxPatentesDisponibles.Location = new System.Drawing.Point(213, 85);
            this.groupBoxPatentesDisponibles.Name = "groupBoxPatentesDisponibles";
            this.groupBoxPatentesDisponibles.Size = new System.Drawing.Size(195, 300);
            this.groupBoxPatentesDisponibles.TabIndex = 4;
            this.groupBoxPatentesDisponibles.TabStop = false;
            this.groupBoxPatentesDisponibles.Tag = "FormPermissionManagement.PatentsListGroup";
            //
            // lstbPatentesDisponibles
            //
            this.lstbPatentesDisponibles.FormattingEnabled = true;
            this.lstbPatentesDisponibles.IntegralHeight = false;
            this.lstbPatentesDisponibles.Location = new System.Drawing.Point(8, 20);
            this.lstbPatentesDisponibles.Name = "lstbPatentesDisponibles";
            this.lstbPatentesDisponibles.Size = new System.Drawing.Size(179, 270);
            this.lstbPatentesDisponibles.TabIndex = 0;
            //
            // btnAgregarFamiliaHija
            //
            this.btnAgregarFamiliaHija.Location = new System.Drawing.Point(12, 395);
            this.btnAgregarFamiliaHija.Name = "btnAgregarFamiliaHija";
            this.btnAgregarFamiliaHija.Size = new System.Drawing.Size(195, 35);
            this.btnAgregarFamiliaHija.TabIndex = 5;
            this.btnAgregarFamiliaHija.Tag = "FormPermissionManagement.AddFamilyButton";
            this.btnAgregarFamiliaHija.UseVisualStyleBackColor = true;
            this.btnAgregarFamiliaHija.Click += new System.EventHandler(this.btnAgregarFamiliaHija_Click);
            //
            // btnAgregarPatenteHija
            //
            this.btnAgregarPatenteHija.Location = new System.Drawing.Point(213, 395);
            this.btnAgregarPatenteHija.Name = "btnAgregarPatenteHija";
            this.btnAgregarPatenteHija.Size = new System.Drawing.Size(195, 35);
            this.btnAgregarPatenteHija.TabIndex = 6;
            this.btnAgregarPatenteHija.Tag = "FormPermissionManagement.AddPatentButton";
            this.btnAgregarPatenteHija.UseVisualStyleBackColor = true;
            this.btnAgregarPatenteHija.Click += new System.EventHandler(this.btnAgregarPatenteHija_Click);
            //
            // groupBoxPreviewFamilia
            //
            this.groupBoxPreviewFamilia.Controls.Add(this.tvwPreviewFamilia);
            this.groupBoxPreviewFamilia.Controls.Add(this.btnEliminarSeleccionado);
            this.groupBoxPreviewFamilia.Controls.Add(this.btnGuardarFamilia);
            this.groupBoxPreviewFamilia.Location = new System.Drawing.Point(735, 50);
            this.groupBoxPreviewFamilia.Name = "groupBoxPreviewFamilia";
            this.groupBoxPreviewFamilia.Size = new System.Drawing.Size(250, 460);
            this.groupBoxPreviewFamilia.TabIndex = 3;
            this.groupBoxPreviewFamilia.TabStop = false;
            this.groupBoxPreviewFamilia.Tag = "FormPermissionManagement.PreviewGroup";
            //
            // tvwPreviewFamilia
            //
            this.tvwPreviewFamilia.HideSelection = false;
            this.tvwPreviewFamilia.Location = new System.Drawing.Point(10, 22);
            this.tvwPreviewFamilia.Name = "tvwPreviewFamilia";
            this.tvwPreviewFamilia.Size = new System.Drawing.Size(230, 360);
            this.tvwPreviewFamilia.TabIndex = 0;
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
            // btnGuardarFamilia
            //
            this.btnGuardarFamilia.Location = new System.Drawing.Point(10, 422);
            this.btnGuardarFamilia.Name = "btnGuardarFamilia";
            this.btnGuardarFamilia.Size = new System.Drawing.Size(230, 30);
            this.btnGuardarFamilia.TabIndex = 2;
            this.btnGuardarFamilia.Tag = "FormPermissionManagement.SaveFamilyButton";
            this.btnGuardarFamilia.UseVisualStyleBackColor = true;
            this.btnGuardarFamilia.Click += new System.EventHandler(this.btnGuardarFamilia_Click);
            //
            // FormGestionPermisos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 525);
            this.Controls.Add(this.groupBoxPreviewFamilia);
            this.Controls.Add(this.groupBoxEditorFamilia);
            this.Controls.Add(this.groupBoxCatalogoPermisos);
            this.Controls.Add(this.lblTituloGestionPermisos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionPermisos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPermissionManagement.Title";
            this.Text = "Gestion de permisos";
            this.groupBoxCatalogoPermisos.ResumeLayout(false);
            this.groupBoxEditorFamilia.ResumeLayout(false);
            this.groupBoxEditorFamilia.PerformLayout();
            this.groupBoxFamiliasDisponibles.ResumeLayout(false);
            this.groupBoxPatentesDisponibles.ResumeLayout(false);
            this.groupBoxPreviewFamilia.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloGestionPermisos;
        private System.Windows.Forms.GroupBox groupBoxCatalogoPermisos;
        private System.Windows.Forms.TreeView tvwCatalogoPermisos;
        private System.Windows.Forms.Button btnEliminarFamilia;
        private System.Windows.Forms.GroupBox groupBoxEditorFamilia;
        private System.Windows.Forms.Label lblNombreFamilia;
        private System.Windows.Forms.TextBox txtbNombreFamilia;
        private System.Windows.Forms.Button btnCrearFamilia;
        private System.Windows.Forms.GroupBox groupBoxFamiliasDisponibles;
        private System.Windows.Forms.ListBox lstbFamiliasDisponibles;
        private System.Windows.Forms.GroupBox groupBoxPatentesDisponibles;
        private System.Windows.Forms.ListBox lstbPatentesDisponibles;
        private System.Windows.Forms.Button btnAgregarFamiliaHija;
        private System.Windows.Forms.Button btnAgregarPatenteHija;
        private System.Windows.Forms.GroupBox groupBoxPreviewFamilia;
        private System.Windows.Forms.TreeView tvwPreviewFamilia;
        private System.Windows.Forms.Button btnEliminarSeleccionado;
        private System.Windows.Forms.Button btnGuardarFamilia;
    }
}
