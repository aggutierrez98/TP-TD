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
            this.groupBoxEdicionPermiso = new System.Windows.Forms.GroupBox();
            this.lblNombrePermiso = new System.Windows.Forms.Label();
            this.txtbNombrePermiso = new System.Windows.Forms.TextBox();
            this.lblTipoPermiso = new System.Windows.Forms.Label();
            this.cmbTipoPermiso = new System.Windows.Forms.ComboBox();
            this.btnAgregarRaizPermiso = new System.Windows.Forms.Button();
            this.btnAgregarHijoPermiso = new System.Windows.Forms.Button();
            this.btnRenombrarPermiso = new System.Windows.Forms.Button();
            this.btnEliminarPermiso = new System.Windows.Forms.Button();
            this.btnGuardarCatalogoPermiso = new System.Windows.Forms.Button();
            this.groupBoxCatalogoPermisos.SuspendLayout();
            this.groupBoxEdicionPermiso.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloGestionPermisos
            // 
            this.lblTituloGestionPermisos.AutoSize = true;
            this.lblTituloGestionPermisos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionPermisos.Location = new System.Drawing.Point(12, 12);
            this.lblTituloGestionPermisos.Name = "lblTituloGestionPermisos";
            this.lblTituloGestionPermisos.Size = new System.Drawing.Size(192, 25);
            this.lblTituloGestionPermisos.Tag = "FormPermissionManagement.TitleLabel";
            this.lblTituloGestionPermisos.TabIndex = 0;
            this.lblTituloGestionPermisos.Text = "Gestion de permisos";
            // 
            // groupBoxCatalogoPermisos
            // 
            this.groupBoxCatalogoPermisos.Controls.Add(this.tvwCatalogoPermisos);
            this.groupBoxCatalogoPermisos.Location = new System.Drawing.Point(15, 55);
            this.groupBoxCatalogoPermisos.Name = "groupBoxCatalogoPermisos";
            this.groupBoxCatalogoPermisos.Size = new System.Drawing.Size(420, 480);
            this.groupBoxCatalogoPermisos.Tag = "FormPermissionManagement.TreeGroup";
            this.groupBoxCatalogoPermisos.TabIndex = 1;
            this.groupBoxCatalogoPermisos.TabStop = false;
            // 
            // tvwCatalogoPermisos
            // 
            this.tvwCatalogoPermisos.HideSelection = false;
            this.tvwCatalogoPermisos.Location = new System.Drawing.Point(10, 25);
            this.tvwCatalogoPermisos.Name = "tvwCatalogoPermisos";
            this.tvwCatalogoPermisos.Size = new System.Drawing.Size(400, 445);
            this.tvwCatalogoPermisos.TabIndex = 0;
            this.tvwCatalogoPermisos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwCatalogoPermisos_AfterSelect);
            // 
            // groupBoxEdicionPermiso
            // 
            this.groupBoxEdicionPermiso.Controls.Add(this.lblNombrePermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.txtbNombrePermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.lblTipoPermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.cmbTipoPermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.btnAgregarRaizPermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.btnAgregarHijoPermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.btnRenombrarPermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.btnEliminarPermiso);
            this.groupBoxEdicionPermiso.Controls.Add(this.btnGuardarCatalogoPermiso);
            this.groupBoxEdicionPermiso.Location = new System.Drawing.Point(450, 55);
            this.groupBoxEdicionPermiso.Name = "groupBoxEdicionPermiso";
            this.groupBoxEdicionPermiso.Size = new System.Drawing.Size(320, 480);
            this.groupBoxEdicionPermiso.Tag = "FormPermissionManagement.EditionGroup";
            this.groupBoxEdicionPermiso.TabIndex = 2;
            this.groupBoxEdicionPermiso.TabStop = false;
            // 
            // lblNombrePermiso
            // 
            this.lblNombrePermiso.AutoSize = true;
            this.lblNombrePermiso.Location = new System.Drawing.Point(12, 35);
            this.lblNombrePermiso.Name = "lblNombrePermiso";
            this.lblNombrePermiso.Size = new System.Drawing.Size(27, 13);
            this.lblNombrePermiso.Tag = "FormPermissionManagement.NameLabel";
            this.lblNombrePermiso.TabIndex = 0;
            this.lblNombrePermiso.Text = "Nombre:";
            // 
            // txtbNombrePermiso
            // 
            this.txtbNombrePermiso.Location = new System.Drawing.Point(15, 52);
            this.txtbNombrePermiso.Name = "txtbNombrePermiso";
            this.txtbNombrePermiso.Size = new System.Drawing.Size(285, 20);
            this.txtbNombrePermiso.TabIndex = 1;
            // 
            // lblTipoPermiso
            // 
            this.lblTipoPermiso.AutoSize = true;
            this.lblTipoPermiso.Location = new System.Drawing.Point(12, 92);
            this.lblTipoPermiso.Name = "lblTipoPermiso";
            this.lblTipoPermiso.Size = new System.Drawing.Size(0, 13);
            this.lblTipoPermiso.Tag = "FormPermissionManagement.TypeLabel";
            this.lblTipoPermiso.TabIndex = 2;
            // 
            // cmbTipoPermiso
            // 
            this.cmbTipoPermiso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoPermiso.FormattingEnabled = true;
            this.cmbTipoPermiso.Location = new System.Drawing.Point(15, 109);
            this.cmbTipoPermiso.Name = "cmbTipoPermiso";
            this.cmbTipoPermiso.Size = new System.Drawing.Size(285, 21);
            this.cmbTipoPermiso.TabIndex = 3;
            // 
            // btnAgregarRaizPermiso
            // 
            this.btnAgregarRaizPermiso.Location = new System.Drawing.Point(15, 155);
            this.btnAgregarRaizPermiso.Name = "btnAgregarRaizPermiso";
            this.btnAgregarRaizPermiso.Size = new System.Drawing.Size(285, 30);
            this.btnAgregarRaizPermiso.Tag = "FormPermissionManagement.RootButton";
            this.btnAgregarRaizPermiso.TabIndex = 4;
            this.btnAgregarRaizPermiso.UseVisualStyleBackColor = true;
            this.btnAgregarRaizPermiso.Click += new System.EventHandler(this.btnAgregarRaizPermiso_Click);
            // 
            // btnAgregarHijoPermiso
            // 
            this.btnAgregarHijoPermiso.Location = new System.Drawing.Point(15, 195);
            this.btnAgregarHijoPermiso.Name = "btnAgregarHijoPermiso";
            this.btnAgregarHijoPermiso.Size = new System.Drawing.Size(285, 30);
            this.btnAgregarHijoPermiso.Tag = "FormPermissionManagement.ChildButton";
            this.btnAgregarHijoPermiso.TabIndex = 5;
            this.btnAgregarHijoPermiso.UseVisualStyleBackColor = true;
            this.btnAgregarHijoPermiso.Click += new System.EventHandler(this.btnAgregarHijoPermiso_Click);
            // 
            // btnRenombrarPermiso
            // 
            this.btnRenombrarPermiso.Location = new System.Drawing.Point(15, 235);
            this.btnRenombrarPermiso.Name = "btnRenombrarPermiso";
            this.btnRenombrarPermiso.Size = new System.Drawing.Size(285, 30);
            this.btnRenombrarPermiso.Tag = "FormPermissionManagement.RenameButton";
            this.btnRenombrarPermiso.TabIndex = 6;
            this.btnRenombrarPermiso.UseVisualStyleBackColor = true;
            this.btnRenombrarPermiso.Click += new System.EventHandler(this.btnRenombrarPermiso_Click);
            // 
            // btnEliminarPermiso
            // 
            this.btnEliminarPermiso.Location = new System.Drawing.Point(15, 275);
            this.btnEliminarPermiso.Name = "btnEliminarPermiso";
            this.btnEliminarPermiso.Size = new System.Drawing.Size(285, 30);
            this.btnEliminarPermiso.Tag = "FormPermissionManagement.DeleteButton";
            this.btnEliminarPermiso.TabIndex = 7;
            this.btnEliminarPermiso.UseVisualStyleBackColor = true;
            this.btnEliminarPermiso.Click += new System.EventHandler(this.btnEliminarPermiso_Click);
            // 
            // btnGuardarCatalogoPermiso
            // 
            this.btnGuardarCatalogoPermiso.Location = new System.Drawing.Point(15, 430);
            this.btnGuardarCatalogoPermiso.Name = "btnGuardarCatalogoPermiso";
            this.btnGuardarCatalogoPermiso.Size = new System.Drawing.Size(285, 30);
            this.btnGuardarCatalogoPermiso.Tag = "FormPermissionManagement.SaveButton";
            this.btnGuardarCatalogoPermiso.TabIndex = 8;
            this.btnGuardarCatalogoPermiso.UseVisualStyleBackColor = true;
            this.btnGuardarCatalogoPermiso.Click += new System.EventHandler(this.btnGuardarCatalogoPermiso_Click);
            // 
            // FormGestionPermisos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 550);
            this.Controls.Add(this.groupBoxEdicionPermiso);
            this.Controls.Add(this.groupBoxCatalogoPermisos);
            this.Controls.Add(this.lblTituloGestionPermisos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionPermisos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPermissionManagement.Title";
            this.Text = "Gestion de permisos";
            this.groupBoxCatalogoPermisos.ResumeLayout(false);
            this.groupBoxEdicionPermiso.ResumeLayout(false);
            this.groupBoxEdicionPermiso.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloGestionPermisos;
        private System.Windows.Forms.GroupBox groupBoxCatalogoPermisos;
        private System.Windows.Forms.GroupBox groupBoxEdicionPermiso;
        private System.Windows.Forms.TreeView tvwCatalogoPermisos;
        private System.Windows.Forms.Label lblNombrePermiso;
        private System.Windows.Forms.TextBox txtbNombrePermiso;
        private System.Windows.Forms.Label lblTipoPermiso;
        private System.Windows.Forms.ComboBox cmbTipoPermiso;
        private System.Windows.Forms.Button btnAgregarRaizPermiso;
        private System.Windows.Forms.Button btnAgregarHijoPermiso;
        private System.Windows.Forms.Button btnRenombrarPermiso;
        private System.Windows.Forms.Button btnEliminarPermiso;
        private System.Windows.Forms.Button btnGuardarCatalogoPermiso;
    }
}
