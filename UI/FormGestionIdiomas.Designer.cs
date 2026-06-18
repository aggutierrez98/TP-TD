namespace TpIDS
{
    partial class FormGestionIdiomas
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
            this.lblTituloGestionIdiomas = new System.Windows.Forms.Label();
            this.groupBoxIdioma = new System.Windows.Forms.GroupBox();
            this.btnIdiomaDefault = new System.Windows.Forms.Button();
            this.btnEliminarIdioma = new System.Windows.Forms.Button();
            this.btnActualizarIdioma = new System.Windows.Forms.Button();
            this.btnAgregarIdioma = new System.Windows.Forms.Button();
            this.txtbNombreIdioma = new System.Windows.Forms.TextBox();
            this.lblNombreIdioma = new System.Windows.Forms.Label();
            this.chkbIdiomaDefault = new System.Windows.Forms.CheckBox();
            this.lblIdiomaSeleccionado = new System.Windows.Forms.Label();
            this.cmbIdiomas = new System.Windows.Forms.ComboBox();
            this.btnImportarJson = new System.Windows.Forms.Button();
            this.btnExportarJson = new System.Windows.Forms.Button();
            this.groupBoxTraducciones = new System.Windows.Forms.GroupBox();
            this.btnEliminarTraduccion = new System.Windows.Forms.Button();
            this.btnGuardarTraduccion = new System.Windows.Forms.Button();
            this.txtbValorManual = new System.Windows.Forms.TextBox();
            this.lblValorManual = new System.Windows.Forms.Label();
            this.txtbKeyManual = new System.Windows.Forms.TextBox();
            this.lblKeyManual = new System.Windows.Forms.Label();
            this.dgvTraducciones = new System.Windows.Forms.DataGridView();
            this.groupBoxIdioma.SuspendLayout();
            this.groupBoxTraducciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTituloGestionIdiomas
            // 
            this.lblTituloGestionIdiomas.AutoSize = true;
            this.lblTituloGestionIdiomas.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionIdiomas.Location = new System.Drawing.Point(18, 15);
            this.lblTituloGestionIdiomas.Name = "lblTituloGestionIdiomas";
            this.lblTituloGestionIdiomas.Size = new System.Drawing.Size(181, 25);
            this.lblTituloGestionIdiomas.TabIndex = 0;
            this.lblTituloGestionIdiomas.Tag = "FormLanguageManagement.TitleLabel";
            this.lblTituloGestionIdiomas.Text = "Gestion de idiomas";
            // 
            // groupBoxIdioma
            // 
            this.groupBoxIdioma.Controls.Add(this.btnIdiomaDefault);
            this.groupBoxIdioma.Controls.Add(this.btnEliminarIdioma);
            this.groupBoxIdioma.Controls.Add(this.btnActualizarIdioma);
            this.groupBoxIdioma.Controls.Add(this.btnAgregarIdioma);
            this.groupBoxIdioma.Controls.Add(this.txtbNombreIdioma);
            this.groupBoxIdioma.Controls.Add(this.lblNombreIdioma);
            this.groupBoxIdioma.Location = new System.Drawing.Point(23, 52);
            this.groupBoxIdioma.Name = "groupBoxIdioma";
            this.groupBoxIdioma.Size = new System.Drawing.Size(714, 116);
            this.groupBoxIdioma.TabIndex = 1;
            this.groupBoxIdioma.TabStop = false;
            this.groupBoxIdioma.Tag = "FormLanguageManagement.LanguageGroup";
            this.groupBoxIdioma.Text = "Datos del idioma";
            // 
            // btnIdiomaDefault
            // 
            this.btnIdiomaDefault.Location = new System.Drawing.Point(474, 25);
            this.btnIdiomaDefault.Name = "btnIdiomaDefault";
            this.btnIdiomaDefault.Size = new System.Drawing.Size(210, 30);
            this.btnIdiomaDefault.TabIndex = 6;
            this.btnIdiomaDefault.Tag = "FormLanguageManagement.MakeDefault";
            this.btnIdiomaDefault.Text = "Eliminar idioma";
            this.btnIdiomaDefault.UseVisualStyleBackColor = true;
            this.btnIdiomaDefault.Click += new System.EventHandler(this.btnIdiomaDefault_Click);
            // 
            // btnEliminarIdioma
            // 
            this.btnEliminarIdioma.Location = new System.Drawing.Point(474, 69);
            this.btnEliminarIdioma.Name = "btnEliminarIdioma";
            this.btnEliminarIdioma.Size = new System.Drawing.Size(210, 30);
            this.btnEliminarIdioma.TabIndex = 5;
            this.btnEliminarIdioma.Tag = "FormLanguageManagement.DeleteButton";
            this.btnEliminarIdioma.Text = "Eliminar idioma";
            this.btnEliminarIdioma.UseVisualStyleBackColor = true;
            this.btnEliminarIdioma.Click += new System.EventHandler(this.btnEliminarIdioma_Click);
            // 
            // btnActualizarIdioma
            // 
            this.btnActualizarIdioma.Location = new System.Drawing.Point(242, 69);
            this.btnActualizarIdioma.Name = "btnActualizarIdioma";
            this.btnActualizarIdioma.Size = new System.Drawing.Size(210, 30);
            this.btnActualizarIdioma.TabIndex = 4;
            this.btnActualizarIdioma.Tag = "FormLanguageManagement.UpdateButton";
            this.btnActualizarIdioma.Text = "Actualizar idioma";
            this.btnActualizarIdioma.UseVisualStyleBackColor = true;
            this.btnActualizarIdioma.Click += new System.EventHandler(this.btnActualizarIdioma_Click);
            // 
            // btnAgregarIdioma
            // 
            this.btnAgregarIdioma.Location = new System.Drawing.Point(14, 69);
            this.btnAgregarIdioma.Name = "btnAgregarIdioma";
            this.btnAgregarIdioma.Size = new System.Drawing.Size(210, 30);
            this.btnAgregarIdioma.TabIndex = 3;
            this.btnAgregarIdioma.Tag = "FormLanguageManagement.AddButton";
            this.btnAgregarIdioma.Text = "Agregar idioma";
            this.btnAgregarIdioma.UseVisualStyleBackColor = true;
            this.btnAgregarIdioma.Click += new System.EventHandler(this.btnAgregarIdioma_Click);
            // 
            // txtbNombreIdioma
            // 
            this.txtbNombreIdioma.Location = new System.Drawing.Point(14, 31);
            this.txtbNombreIdioma.Name = "txtbNombreIdioma";
            this.txtbNombreIdioma.Size = new System.Drawing.Size(438, 20);
            this.txtbNombreIdioma.TabIndex = 1;
            // 
            // lblNombreIdioma
            // 
            this.lblNombreIdioma.AutoSize = true;
            this.lblNombreIdioma.Location = new System.Drawing.Point(11, 15);
            this.lblNombreIdioma.Name = "lblNombreIdioma";
            this.lblNombreIdioma.Size = new System.Drawing.Size(97, 13);
            this.lblNombreIdioma.TabIndex = 0;
            this.lblNombreIdioma.Tag = "FormLanguageManagement.NameLabel";
            this.lblNombreIdioma.Text = "Nombre del idioma:";
            // 
            // chkbIdiomaDefault
            // 
            this.chkbIdiomaDefault.AutoCheck = false;
            this.chkbIdiomaDefault.AutoSize = true;
            this.chkbIdiomaDefault.Location = new System.Drawing.Point(512, 52);
            this.chkbIdiomaDefault.Name = "chkbIdiomaDefault";
            this.chkbIdiomaDefault.Size = new System.Drawing.Size(92, 17);
            this.chkbIdiomaDefault.TabIndex = 2;
            this.chkbIdiomaDefault.Tag = "FormLanguageManagement.DefaultCheck";
            this.chkbIdiomaDefault.Text = "Idioma default";
            this.chkbIdiomaDefault.UseVisualStyleBackColor = true;
            // 
            // lblIdiomaSeleccionado
            // 
            this.lblIdiomaSeleccionado.AutoSize = true;
            this.lblIdiomaSeleccionado.Location = new System.Drawing.Point(20, 183);
            this.lblIdiomaSeleccionado.Name = "lblIdiomaSeleccionado";
            this.lblIdiomaSeleccionado.Size = new System.Drawing.Size(107, 13);
            this.lblIdiomaSeleccionado.TabIndex = 2;
            this.lblIdiomaSeleccionado.Tag = "FormLanguageManagement.SelectLanguageLabel";
            this.lblIdiomaSeleccionado.Text = "Idioma seleccionado:";
            // 
            // cmbIdiomas
            // 
            this.cmbIdiomas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdiomas.FormattingEnabled = true;
            this.cmbIdiomas.Location = new System.Drawing.Point(23, 199);
            this.cmbIdiomas.Name = "cmbIdiomas";
            this.cmbIdiomas.Size = new System.Drawing.Size(329, 21);
            this.cmbIdiomas.TabIndex = 3;
            this.cmbIdiomas.SelectedIndexChanged += new System.EventHandler(this.cmbIdiomas_SelectedIndexChanged);
            // 
            // btnImportarJson
            // 
            this.btnImportarJson.Location = new System.Drawing.Point(375, 197);
            this.btnImportarJson.Name = "btnImportarJson";
            this.btnImportarJson.Size = new System.Drawing.Size(171, 25);
            this.btnImportarJson.TabIndex = 4;
            this.btnImportarJson.Tag = "FormLanguageManagement.ImportButton";
            this.btnImportarJson.Text = "Importar JSON";
            this.btnImportarJson.UseVisualStyleBackColor = true;
            this.btnImportarJson.Click += new System.EventHandler(this.btnImportarJson_Click);
            // 
            // btnExportarJson
            // 
            this.btnExportarJson.Location = new System.Drawing.Point(566, 197);
            this.btnExportarJson.Name = "btnExportarJson";
            this.btnExportarJson.Size = new System.Drawing.Size(171, 25);
            this.btnExportarJson.TabIndex = 5;
            this.btnExportarJson.Tag = "FormLanguageManagement.ExportButton";
            this.btnExportarJson.Text = "Exportar JSON";
            this.btnExportarJson.UseVisualStyleBackColor = true;
            this.btnExportarJson.Click += new System.EventHandler(this.btnExportarJson_Click);
            // 
            // groupBoxTraducciones
            // 
            this.groupBoxTraducciones.Controls.Add(this.btnEliminarTraduccion);
            this.groupBoxTraducciones.Controls.Add(this.btnGuardarTraduccion);
            this.groupBoxTraducciones.Controls.Add(this.txtbValorManual);
            this.groupBoxTraducciones.Controls.Add(this.lblValorManual);
            this.groupBoxTraducciones.Controls.Add(this.txtbKeyManual);
            this.groupBoxTraducciones.Controls.Add(this.lblKeyManual);
            this.groupBoxTraducciones.Controls.Add(this.dgvTraducciones);
            this.groupBoxTraducciones.Location = new System.Drawing.Point(23, 239);
            this.groupBoxTraducciones.Name = "groupBoxTraducciones";
            this.groupBoxTraducciones.Size = new System.Drawing.Size(714, 319);
            this.groupBoxTraducciones.TabIndex = 6;
            this.groupBoxTraducciones.TabStop = false;
            this.groupBoxTraducciones.Tag = "FormLanguageManagement.TranslationsGroup";
            this.groupBoxTraducciones.Text = "Traducciones";
            // 
            // btnEliminarTraduccion
            // 
            this.btnEliminarTraduccion.Location = new System.Drawing.Point(489, 278);
            this.btnEliminarTraduccion.Name = "btnEliminarTraduccion";
            this.btnEliminarTraduccion.Size = new System.Drawing.Size(195, 28);
            this.btnEliminarTraduccion.TabIndex = 6;
            this.btnEliminarTraduccion.Tag = "FormLanguageManagement.DeleteTranslationButton";
            this.btnEliminarTraduccion.Text = "Eliminar traduccion";
            this.btnEliminarTraduccion.UseVisualStyleBackColor = true;
            this.btnEliminarTraduccion.Click += new System.EventHandler(this.btnEliminarTraduccion_Click);
            // 
            // btnGuardarTraduccion
            // 
            this.btnGuardarTraduccion.Location = new System.Drawing.Point(489, 239);
            this.btnGuardarTraduccion.Name = "btnGuardarTraduccion";
            this.btnGuardarTraduccion.Size = new System.Drawing.Size(195, 28);
            this.btnGuardarTraduccion.TabIndex = 5;
            this.btnGuardarTraduccion.Tag = "FormLanguageManagement.SaveTranslationButton";
            this.btnGuardarTraduccion.Text = "Guardar traduccion";
            this.btnGuardarTraduccion.UseVisualStyleBackColor = true;
            this.btnGuardarTraduccion.Click += new System.EventHandler(this.btnGuardarTraduccion_Click);
            // 
            // txtbValorManual
            // 
            this.txtbValorManual.Location = new System.Drawing.Point(192, 283);
            this.txtbValorManual.Name = "txtbValorManual";
            this.txtbValorManual.Size = new System.Drawing.Size(277, 20);
            this.txtbValorManual.TabIndex = 4;
            // 
            // lblValorManual
            // 
            this.lblValorManual.AutoSize = true;
            this.lblValorManual.Location = new System.Drawing.Point(189, 267);
            this.lblValorManual.Name = "lblValorManual";
            this.lblValorManual.Size = new System.Drawing.Size(64, 13);
            this.lblValorManual.TabIndex = 3;
            this.lblValorManual.Tag = "FormLanguageManagement.ManualValueLabel";
            this.lblValorManual.Text = "Traduccion:";
            // 
            // txtbKeyManual
            // 
            this.txtbKeyManual.Location = new System.Drawing.Point(14, 283);
            this.txtbKeyManual.Name = "txtbKeyManual";
            this.txtbKeyManual.Size = new System.Drawing.Size(161, 20);
            this.txtbKeyManual.TabIndex = 2;
            // 
            // lblKeyManual
            // 
            this.lblKeyManual.AutoSize = true;
            this.lblKeyManual.Location = new System.Drawing.Point(11, 267);
            this.lblKeyManual.Name = "lblKeyManual";
            this.lblKeyManual.Size = new System.Drawing.Size(28, 13);
            this.lblKeyManual.TabIndex = 1;
            this.lblKeyManual.Tag = "FormLanguageManagement.ManualKeyLabel";
            this.lblKeyManual.Text = "Key:";
            // 
            // dgvTraducciones
            // 
            this.dgvTraducciones.AllowUserToAddRows = false;
            this.dgvTraducciones.AllowUserToDeleteRows = false;
            this.dgvTraducciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTraducciones.Location = new System.Drawing.Point(14, 24);
            this.dgvTraducciones.MultiSelect = false;
            this.dgvTraducciones.Name = "dgvTraducciones";
            this.dgvTraducciones.ReadOnly = true;
            this.dgvTraducciones.RowHeadersVisible = false;
            this.dgvTraducciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTraducciones.Size = new System.Drawing.Size(670, 230);
            this.dgvTraducciones.TabIndex = 0;
            this.dgvTraducciones.Tag = "FormLanguageManagement.Grid";
            this.dgvTraducciones.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTraducciones_CellClick);
            // 
            // FormGestionIdiomas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 579);
            this.Controls.Add(this.groupBoxTraducciones);
            this.Controls.Add(this.btnExportarJson);
            this.Controls.Add(this.chkbIdiomaDefault);
            this.Controls.Add(this.btnImportarJson);
            this.Controls.Add(this.cmbIdiomas);
            this.Controls.Add(this.lblIdiomaSeleccionado);
            this.Controls.Add(this.groupBoxIdioma);
            this.Controls.Add(this.lblTituloGestionIdiomas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionIdiomas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormLanguageManagement.Title";
            this.Text = "Gestion de idiomas";
            this.groupBoxIdioma.ResumeLayout(false);
            this.groupBoxIdioma.PerformLayout();
            this.groupBoxTraducciones.ResumeLayout(false);
            this.groupBoxTraducciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloGestionIdiomas;
        private System.Windows.Forms.GroupBox groupBoxIdioma;
        private System.Windows.Forms.Button btnEliminarIdioma;
        private System.Windows.Forms.Button btnActualizarIdioma;
        private System.Windows.Forms.Button btnAgregarIdioma;
        private System.Windows.Forms.CheckBox chkbIdiomaDefault;
        private System.Windows.Forms.TextBox txtbNombreIdioma;
        private System.Windows.Forms.Label lblNombreIdioma;
        private System.Windows.Forms.Label lblIdiomaSeleccionado;
        private System.Windows.Forms.ComboBox cmbIdiomas;
        private System.Windows.Forms.Button btnImportarJson;
        private System.Windows.Forms.Button btnExportarJson;
        private System.Windows.Forms.GroupBox groupBoxTraducciones;
        private System.Windows.Forms.Button btnEliminarTraduccion;
        private System.Windows.Forms.Button btnGuardarTraduccion;
        private System.Windows.Forms.TextBox txtbValorManual;
        private System.Windows.Forms.Label lblValorManual;
        private System.Windows.Forms.TextBox txtbKeyManual;
        private System.Windows.Forms.Label lblKeyManual;
        private System.Windows.Forms.DataGridView dgvTraducciones;
        private System.Windows.Forms.Button btnIdiomaDefault;
    }
}
