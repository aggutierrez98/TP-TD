namespace TpIDS
{
    partial class FormGestionComercios
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
            this.lblTituloComercios = new System.Windows.Forms.Label();
            this.groupBoxComercio = new System.Windows.Forms.GroupBox();
            this.btnVerDetalleComercio = new System.Windows.Forms.Button();
            this.btnEliminarComercio = new System.Windows.Forms.Button();
            this.btnActualizarComercio = new System.Windows.Forms.Button();
            this.btnAgregarComercio = new System.Windows.Forms.Button();
            this.txtbUbicacionComercio = new System.Windows.Forms.TextBox();
            this.lblUbicacionComercio = new System.Windows.Forms.Label();
            this.cmbRubroComercio = new System.Windows.Forms.ComboBox();
            this.lblRubroComercio = new System.Windows.Forms.Label();
            this.txtbNombreComercio = new System.Windows.Forms.TextBox();
            this.lblNombreComercio = new System.Windows.Forms.Label();
            this.dgvComercios = new System.Windows.Forms.DataGridView();
            this.colComercioNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComercioRubro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxRubro = new System.Windows.Forms.GroupBox();
            this.btnEliminarRubro = new System.Windows.Forms.Button();
            this.btnActualizarRubro = new System.Windows.Forms.Button();
            this.btnAgregarRubro = new System.Windows.Forms.Button();
            this.txtbNombreRubro = new System.Windows.Forms.TextBox();
            this.lblNombreRubro = new System.Windows.Forms.Label();
            this.dgvRubros = new System.Windows.Forms.DataGridView();
            this.colRubroNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxComercio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComercios)).BeginInit();
            this.groupBoxRubro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRubros)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloComercios
            //
            this.lblTituloComercios.AutoSize = true;
            this.lblTituloComercios.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloComercios.Location = new System.Drawing.Point(18, 15);
            this.lblTituloComercios.Name = "lblTituloComercios";
            this.lblTituloComercios.Size = new System.Drawing.Size(260, 25);
            this.lblTituloComercios.TabIndex = 0;
            this.lblTituloComercios.Tag = "FormMerchantManagement.TitleLabel";
            this.lblTituloComercios.Text = "Gestion de Comercios";
            //
            // groupBoxComercio
            //
            this.groupBoxComercio.Controls.Add(this.btnVerDetalleComercio);
            this.groupBoxComercio.Controls.Add(this.btnEliminarComercio);
            this.groupBoxComercio.Controls.Add(this.btnActualizarComercio);
            this.groupBoxComercio.Controls.Add(this.btnAgregarComercio);
            this.groupBoxComercio.Controls.Add(this.txtbUbicacionComercio);
            this.groupBoxComercio.Controls.Add(this.lblUbicacionComercio);
            this.groupBoxComercio.Controls.Add(this.cmbRubroComercio);
            this.groupBoxComercio.Controls.Add(this.lblRubroComercio);
            this.groupBoxComercio.Controls.Add(this.txtbNombreComercio);
            this.groupBoxComercio.Controls.Add(this.lblNombreComercio);
            this.groupBoxComercio.Location = new System.Drawing.Point(23, 52);
            this.groupBoxComercio.Name = "groupBoxComercio";
            this.groupBoxComercio.Size = new System.Drawing.Size(594, 150);
            this.groupBoxComercio.TabIndex = 1;
            this.groupBoxComercio.TabStop = false;
            this.groupBoxComercio.Tag = "FormMerchantManagement.ComercioGroup";
            this.groupBoxComercio.Text = "Comercios";
            //
            // btnVerDetalleComercio
            //
            this.btnVerDetalleComercio.Location = new System.Drawing.Point(374, 108);
            this.btnVerDetalleComercio.Name = "btnVerDetalleComercio";
            this.btnVerDetalleComercio.Size = new System.Drawing.Size(200, 30);
            this.btnVerDetalleComercio.TabIndex = 9;
            this.btnVerDetalleComercio.Tag = "FormMerchantManagement.DetailButton";
            this.btnVerDetalleComercio.Text = "Ver detalle";
            this.btnVerDetalleComercio.UseVisualStyleBackColor = true;
            this.btnVerDetalleComercio.Click += new System.EventHandler(this.btnVerDetalleComercio_Click);
            //
            // btnEliminarComercio
            //
            this.btnEliminarComercio.Location = new System.Drawing.Point(254, 108);
            this.btnEliminarComercio.Name = "btnEliminarComercio";
            this.btnEliminarComercio.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarComercio.TabIndex = 8;
            this.btnEliminarComercio.Tag = "FormMerchantManagement.DeleteComercioButton";
            this.btnEliminarComercio.Text = "Eliminar";
            this.btnEliminarComercio.UseVisualStyleBackColor = true;
            this.btnEliminarComercio.Click += new System.EventHandler(this.btnEliminarComercio_Click);
            //
            // btnActualizarComercio
            //
            this.btnActualizarComercio.Location = new System.Drawing.Point(134, 108);
            this.btnActualizarComercio.Name = "btnActualizarComercio";
            this.btnActualizarComercio.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarComercio.TabIndex = 7;
            this.btnActualizarComercio.Tag = "FormMerchantManagement.UpdateComercioButton";
            this.btnActualizarComercio.Text = "Actualizar";
            this.btnActualizarComercio.UseVisualStyleBackColor = true;
            this.btnActualizarComercio.Click += new System.EventHandler(this.btnActualizarComercio_Click);
            //
            // btnAgregarComercio
            //
            this.btnAgregarComercio.Location = new System.Drawing.Point(14, 108);
            this.btnAgregarComercio.Name = "btnAgregarComercio";
            this.btnAgregarComercio.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarComercio.TabIndex = 6;
            this.btnAgregarComercio.Tag = "FormMerchantManagement.AddComercioButton";
            this.btnAgregarComercio.Text = "Agregar";
            this.btnAgregarComercio.UseVisualStyleBackColor = true;
            this.btnAgregarComercio.Click += new System.EventHandler(this.btnAgregarComercio_Click);
            //
            // txtbUbicacionComercio
            //
            this.txtbUbicacionComercio.Location = new System.Drawing.Point(385, 44);
            this.txtbUbicacionComercio.Name = "txtbUbicacionComercio";
            this.txtbUbicacionComercio.Size = new System.Drawing.Size(190, 20);
            this.txtbUbicacionComercio.TabIndex = 5;
            //
            // lblUbicacionComercio
            //
            this.lblUbicacionComercio.AutoSize = true;
            this.lblUbicacionComercio.Location = new System.Drawing.Point(382, 28);
            this.lblUbicacionComercio.Name = "lblUbicacionComercio";
            this.lblUbicacionComercio.Size = new System.Drawing.Size(59, 13);
            this.lblUbicacionComercio.TabIndex = 4;
            this.lblUbicacionComercio.Tag = "FormMerchantManagement.UbicacionLabel";
            this.lblUbicacionComercio.Text = "Ubicacion:";
            //
            // cmbRubroComercio
            //
            this.cmbRubroComercio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRubroComercio.FormattingEnabled = true;
            this.cmbRubroComercio.Location = new System.Drawing.Point(210, 44);
            this.cmbRubroComercio.Name = "cmbRubroComercio";
            this.cmbRubroComercio.Size = new System.Drawing.Size(160, 21);
            this.cmbRubroComercio.TabIndex = 3;
            //
            // lblRubroComercio
            //
            this.lblRubroComercio.AutoSize = true;
            this.lblRubroComercio.Location = new System.Drawing.Point(207, 28);
            this.lblRubroComercio.Name = "lblRubroComercio";
            this.lblRubroComercio.Size = new System.Drawing.Size(41, 13);
            this.lblRubroComercio.TabIndex = 2;
            this.lblRubroComercio.Tag = "FormMerchantManagement.RubroComboLabel";
            this.lblRubroComercio.Text = "Rubro:";
            //
            // txtbNombreComercio
            //
            this.txtbNombreComercio.Location = new System.Drawing.Point(14, 44);
            this.txtbNombreComercio.Name = "txtbNombreComercio";
            this.txtbNombreComercio.Size = new System.Drawing.Size(180, 20);
            this.txtbNombreComercio.TabIndex = 1;
            //
            // lblNombreComercio
            //
            this.lblNombreComercio.AutoSize = true;
            this.lblNombreComercio.Location = new System.Drawing.Point(11, 28);
            this.lblNombreComercio.Name = "lblNombreComercio";
            this.lblNombreComercio.Size = new System.Drawing.Size(53, 13);
            this.lblNombreComercio.TabIndex = 0;
            this.lblNombreComercio.Tag = "FormMerchantManagement.NombreComercioLabel";
            this.lblNombreComercio.Text = "Nombre:";
            //
            // dgvComercios
            //
            this.dgvComercios.AllowUserToAddRows = false;
            this.dgvComercios.AllowUserToDeleteRows = false;
            this.dgvComercios.AutoGenerateColumns = false;
            this.dgvComercios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvComercios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComercios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colComercioNombre,
            this.colComercioRubro});
            this.dgvComercios.Location = new System.Drawing.Point(23, 215);
            this.dgvComercios.MultiSelect = false;
            this.dgvComercios.Name = "dgvComercios";
            this.dgvComercios.RowHeadersVisible = false;
            this.dgvComercios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComercios.Size = new System.Drawing.Size(594, 180);
            this.dgvComercios.TabIndex = 2;
            this.dgvComercios.Tag = "FormMerchantManagement.ComercioGrid";
            this.dgvComercios.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComercios_CellClick);
            //
            // colComercioNombre
            //
            this.colComercioNombre.DataPropertyName = "Nombre";
            this.colComercioNombre.HeaderText = "Nombre";
            this.colComercioNombre.Name = "Nombre";
            this.colComercioNombre.ReadOnly = true;
            //
            // colComercioRubro
            //
            this.colComercioRubro.DataPropertyName = "Rubro";
            this.colComercioRubro.HeaderText = "Rubro";
            this.colComercioRubro.Name = "Rubro";
            this.colComercioRubro.ReadOnly = true;
            //
            // groupBoxRubro
            //
            this.groupBoxRubro.Controls.Add(this.btnEliminarRubro);
            this.groupBoxRubro.Controls.Add(this.btnActualizarRubro);
            this.groupBoxRubro.Controls.Add(this.btnAgregarRubro);
            this.groupBoxRubro.Controls.Add(this.txtbNombreRubro);
            this.groupBoxRubro.Controls.Add(this.lblNombreRubro);
            this.groupBoxRubro.Location = new System.Drawing.Point(23, 410);
            this.groupBoxRubro.Name = "groupBoxRubro";
            this.groupBoxRubro.Size = new System.Drawing.Size(594, 110);
            this.groupBoxRubro.TabIndex = 3;
            this.groupBoxRubro.TabStop = false;
            this.groupBoxRubro.Tag = "FormMerchantManagement.RubroGroup";
            this.groupBoxRubro.Text = "Rubros de comercio";
            //
            // btnEliminarRubro
            //
            this.btnEliminarRubro.Location = new System.Drawing.Point(254, 68);
            this.btnEliminarRubro.Name = "btnEliminarRubro";
            this.btnEliminarRubro.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarRubro.TabIndex = 4;
            this.btnEliminarRubro.Tag = "FormMerchantManagement.DeleteRubroButton";
            this.btnEliminarRubro.Text = "Eliminar";
            this.btnEliminarRubro.UseVisualStyleBackColor = true;
            this.btnEliminarRubro.Click += new System.EventHandler(this.btnEliminarRubro_Click);
            //
            // btnActualizarRubro
            //
            this.btnActualizarRubro.Location = new System.Drawing.Point(134, 68);
            this.btnActualizarRubro.Name = "btnActualizarRubro";
            this.btnActualizarRubro.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarRubro.TabIndex = 3;
            this.btnActualizarRubro.Tag = "FormMerchantManagement.UpdateRubroButton";
            this.btnActualizarRubro.Text = "Actualizar";
            this.btnActualizarRubro.UseVisualStyleBackColor = true;
            this.btnActualizarRubro.Click += new System.EventHandler(this.btnActualizarRubro_Click);
            //
            // btnAgregarRubro
            //
            this.btnAgregarRubro.Location = new System.Drawing.Point(14, 68);
            this.btnAgregarRubro.Name = "btnAgregarRubro";
            this.btnAgregarRubro.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarRubro.TabIndex = 2;
            this.btnAgregarRubro.Tag = "FormMerchantManagement.AddRubroButton";
            this.btnAgregarRubro.Text = "Agregar";
            this.btnAgregarRubro.UseVisualStyleBackColor = true;
            this.btnAgregarRubro.Click += new System.EventHandler(this.btnAgregarRubro_Click);
            //
            // txtbNombreRubro
            //
            this.txtbNombreRubro.Location = new System.Drawing.Point(14, 44);
            this.txtbNombreRubro.Name = "txtbNombreRubro";
            this.txtbNombreRubro.Size = new System.Drawing.Size(300, 20);
            this.txtbNombreRubro.TabIndex = 1;
            //
            // lblNombreRubro
            //
            this.lblNombreRubro.AutoSize = true;
            this.lblNombreRubro.Location = new System.Drawing.Point(11, 28);
            this.lblNombreRubro.Name = "lblNombreRubro";
            this.lblNombreRubro.Size = new System.Drawing.Size(53, 13);
            this.lblNombreRubro.TabIndex = 0;
            this.lblNombreRubro.Tag = "FormMerchantManagement.NombreRubroLabel";
            this.lblNombreRubro.Text = "Nombre:";
            //
            // dgvRubros
            //
            this.dgvRubros.AllowUserToAddRows = false;
            this.dgvRubros.AllowUserToDeleteRows = false;
            this.dgvRubros.AutoGenerateColumns = false;
            this.dgvRubros.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRubros.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRubros.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRubroNombre});
            this.dgvRubros.Location = new System.Drawing.Point(23, 526);
            this.dgvRubros.MultiSelect = false;
            this.dgvRubros.Name = "dgvRubros";
            this.dgvRubros.RowHeadersVisible = false;
            this.dgvRubros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRubros.Size = new System.Drawing.Size(594, 150);
            this.dgvRubros.TabIndex = 4;
            this.dgvRubros.Tag = "FormMerchantManagement.RubroGrid";
            this.dgvRubros.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRubros_CellClick);
            //
            // colRubroNombre
            //
            this.colRubroNombre.DataPropertyName = "Nombre";
            this.colRubroNombre.HeaderText = "Nombre";
            this.colRubroNombre.Name = "Nombre";
            this.colRubroNombre.ReadOnly = true;
            //
            // FormGestionComercios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 700);
            this.Controls.Add(this.dgvRubros);
            this.Controls.Add(this.groupBoxRubro);
            this.Controls.Add(this.dgvComercios);
            this.Controls.Add(this.groupBoxComercio);
            this.Controls.Add(this.lblTituloComercios);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionComercios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMerchantManagement.Title";
            this.Text = "Gestion de Comercios";
            this.groupBoxComercio.ResumeLayout(false);
            this.groupBoxComercio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComercios)).EndInit();
            this.groupBoxRubro.ResumeLayout(false);
            this.groupBoxRubro.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRubros)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloComercios;
        private System.Windows.Forms.GroupBox groupBoxComercio;
        private System.Windows.Forms.Button btnVerDetalleComercio;
        private System.Windows.Forms.Button btnEliminarComercio;
        private System.Windows.Forms.Button btnActualizarComercio;
        private System.Windows.Forms.Button btnAgregarComercio;
        private System.Windows.Forms.TextBox txtbUbicacionComercio;
        private System.Windows.Forms.Label lblUbicacionComercio;
        private System.Windows.Forms.ComboBox cmbRubroComercio;
        private System.Windows.Forms.Label lblRubroComercio;
        private System.Windows.Forms.TextBox txtbNombreComercio;
        private System.Windows.Forms.Label lblNombreComercio;
        private System.Windows.Forms.DataGridView dgvComercios;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComercioNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComercioRubro;
        private System.Windows.Forms.GroupBox groupBoxRubro;
        private System.Windows.Forms.Button btnEliminarRubro;
        private System.Windows.Forms.Button btnActualizarRubro;
        private System.Windows.Forms.Button btnAgregarRubro;
        private System.Windows.Forms.TextBox txtbNombreRubro;
        private System.Windows.Forms.Label lblNombreRubro;
        private System.Windows.Forms.DataGridView dgvRubros;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRubroNombre;
    }
}
