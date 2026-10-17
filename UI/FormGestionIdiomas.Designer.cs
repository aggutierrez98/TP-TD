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
            this.lblTituloGestionIdiomas = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxIdioma = new Krypton.Toolkit.KryptonGroupBox();
            this.btnIdiomaDefault = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarIdioma = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarIdioma = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarIdioma = new Krypton.Toolkit.KryptonButton();
            this.txtbNombreIdioma = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreIdioma = new Krypton.Toolkit.KryptonLabel();
            this.chkbIdiomaDefault = new TpIDS.CheckBoxAccesible();
            this.lblIdiomaSeleccionado = new Krypton.Toolkit.KryptonLabel();
            this.cmbIdiomas = new Krypton.Toolkit.KryptonComboBox();
            this.btnImportarJson = new Krypton.Toolkit.KryptonButton();
            this.btnExportarJson = new Krypton.Toolkit.KryptonButton();
            this.groupBoxTraducciones = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarTraduccion = new Krypton.Toolkit.KryptonButton();
            this.btnGuardarTraduccion = new Krypton.Toolkit.KryptonButton();
            this.txtbValorManual = new Krypton.Toolkit.KryptonTextBox();
            this.lblValorManual = new Krypton.Toolkit.KryptonLabel();
            this.txtbKeyManual = new Krypton.Toolkit.KryptonTextBox();
            this.lblKeyManual = new Krypton.Toolkit.KryptonLabel();
            this.dgvTraducciones = new Krypton.Toolkit.KryptonDataGridView();
            this.colTraduccionesKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTraduccionesValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxIdioma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxIdioma.Panel)).BeginInit();
            this.groupBoxIdioma.Panel.SuspendLayout();
            this.groupBoxIdioma.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTraducciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTraducciones.Panel)).BeginInit();
            this.groupBoxTraducciones.Panel.SuspendLayout();
            this.groupBoxTraducciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).BeginInit();
            this.tlpNombreIdioma = new System.Windows.Forms.TableLayoutPanel();
            this.tlpNombreIdioma.SuspendLayout();
            this.tlpTraduccionManual = new System.Windows.Forms.TableLayoutPanel();
            this.tlpTraduccionManual.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloGestionIdiomas
            // 
            this.lblTituloGestionIdiomas.AutoSize = true;
            this.lblTituloGestionIdiomas.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionIdiomas.Location = new System.Drawing.Point(18, 15);
            this.lblTituloGestionIdiomas.Name = "lblTituloGestionIdiomas";
            this.lblTituloGestionIdiomas.Size = new System.Drawing.Size(181, 25);
            this.lblTituloGestionIdiomas.TabIndex = 0;
            this.lblTituloGestionIdiomas.Tag = "FormLanguageManagement.TitleLabel";
            this.lblTituloGestionIdiomas.Values.Text = "Gestion de idiomas";
            // 
            // groupBoxIdioma
            // 
            this.groupBoxIdioma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxIdioma.Panel.Controls.Add(this.tlpNombreIdioma);
            this.groupBoxIdioma.Panel.Controls.Add(this.btnIdiomaDefault);
            this.groupBoxIdioma.Panel.Controls.Add(this.btnEliminarIdioma);
            this.groupBoxIdioma.Panel.Controls.Add(this.btnActualizarIdioma);
            this.groupBoxIdioma.Panel.Controls.Add(this.btnAgregarIdioma);
            this.groupBoxIdioma.Location = new System.Drawing.Point(23, 52);
            this.groupBoxIdioma.Name = "groupBoxIdioma";
            this.groupBoxIdioma.Size = new System.Drawing.Size(714, 116);
            this.groupBoxIdioma.TabIndex = 1;
            this.groupBoxIdioma.TabStop = false;
            this.groupBoxIdioma.Tag = "FormLanguageManagement.LanguageGroup";
            this.groupBoxIdioma.Values.Heading = "Datos del idioma";
            // 
            // btnIdiomaDefault
            // 
            this.btnIdiomaDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIdiomaDefault.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnIdiomaDefault.Location = new System.Drawing.Point(474, 9);
            this.btnIdiomaDefault.Name = "btnIdiomaDefault";
            this.btnIdiomaDefault.Size = new System.Drawing.Size(210, 30);
            this.btnIdiomaDefault.TabIndex = 6;
            this.btnIdiomaDefault.Tag = "FormLanguageManagement.MakeDefault";
            this.btnIdiomaDefault.Values.Text = "Marcar como default";
            this.btnIdiomaDefault.Click += new System.EventHandler(this.btnIdiomaDefault_Click);
            // 
            // btnEliminarIdioma
            // 
            this.btnEliminarIdioma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarIdioma.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarIdioma.Location = new System.Drawing.Point(474, 53);
            this.btnEliminarIdioma.Name = "btnEliminarIdioma";
            this.btnEliminarIdioma.Size = new System.Drawing.Size(210, 30);
            this.btnEliminarIdioma.TabIndex = 5;
            this.btnEliminarIdioma.Tag = "FormLanguageManagement.DeleteButton";
            this.btnEliminarIdioma.Values.Text = "Eliminar idioma";
            this.btnEliminarIdioma.Click += new System.EventHandler(this.btnEliminarIdioma_Click);
            // 
            // btnActualizarIdioma
            // 
            this.btnActualizarIdioma.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarIdioma.Location = new System.Drawing.Point(242, 53);
            this.btnActualizarIdioma.Name = "btnActualizarIdioma";
            this.btnActualizarIdioma.Size = new System.Drawing.Size(210, 30);
            this.btnActualizarIdioma.TabIndex = 4;
            this.btnActualizarIdioma.Tag = "FormLanguageManagement.UpdateButton";
            this.btnActualizarIdioma.Values.Text = "Actualizar idioma";
            this.btnActualizarIdioma.Click += new System.EventHandler(this.btnActualizarIdioma_Click);
            // 
            // btnAgregarIdioma
            // 
            this.btnAgregarIdioma.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarIdioma.Location = new System.Drawing.Point(14, 53);
            this.btnAgregarIdioma.Name = "btnAgregarIdioma";
            this.btnAgregarIdioma.Size = new System.Drawing.Size(210, 30);
            this.btnAgregarIdioma.TabIndex = 3;
            this.btnAgregarIdioma.Tag = "FormLanguageManagement.AddButton";
            this.btnAgregarIdioma.Values.Text = "Agregar idioma";
            this.btnAgregarIdioma.Click += new System.EventHandler(this.btnAgregarIdioma_Click);
            // 
            // txtbNombreIdioma
            // 
            this.txtbNombreIdioma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreIdioma.Location = new System.Drawing.Point(3, 15);
            this.txtbNombreIdioma.Name = "txtbNombreIdioma";
            this.txtbNombreIdioma.Size = new System.Drawing.Size(320, 20);
            this.txtbNombreIdioma.TabIndex = 1;
            this.txtbNombreIdioma.Margin = new System.Windows.Forms.Padding(3, 0, 16, 0);
            // 
            // lblNombreIdioma
            // 
            this.lblNombreIdioma.AutoSize = true;
            this.lblNombreIdioma.Location = new System.Drawing.Point(0, 0);
            this.lblNombreIdioma.Name = "lblNombreIdioma";
            this.lblNombreIdioma.Size = new System.Drawing.Size(97, 13);
            this.lblNombreIdioma.TabIndex = 0;
            this.lblNombreIdioma.Tag = "FormLanguageManagement.NameLabel";
            this.lblNombreIdioma.Values.Text = "Nombre del idioma:";
            this.lblNombreIdioma.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // chkbIdiomaDefault
            // 
            this.chkbIdiomaDefault.AutoCheck = false;
            this.chkbIdiomaDefault.AutoSize = true;
            this.chkbIdiomaDefault.Location = new System.Drawing.Point(339, 17);
            this.chkbIdiomaDefault.Name = "chkbIdiomaDefault";
            this.chkbIdiomaDefault.Size = new System.Drawing.Size(92, 17);
            this.chkbIdiomaDefault.TabIndex = 2;
            this.chkbIdiomaDefault.Tag = "FormLanguageManagement.DefaultCheck";
            this.chkbIdiomaDefault.Values.Text = "Idioma default";
            this.chkbIdiomaDefault.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            // 
            // lblIdiomaSeleccionado
            // 
            this.lblIdiomaSeleccionado.AutoSize = true;
            this.lblIdiomaSeleccionado.Location = new System.Drawing.Point(20, 179);
            this.lblIdiomaSeleccionado.Name = "lblIdiomaSeleccionado";
            this.lblIdiomaSeleccionado.Size = new System.Drawing.Size(107, 13);
            this.lblIdiomaSeleccionado.TabIndex = 2;
            this.lblIdiomaSeleccionado.Tag = "FormLanguageManagement.SelectLanguageLabel";
            this.lblIdiomaSeleccionado.Values.Text = "Idioma seleccionado:";
            // 
            // cmbIdiomas
            // 
            this.cmbIdiomas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnImportarJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportarJson.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnImportarJson.Location = new System.Drawing.Point(375, 197);
            this.btnImportarJson.Name = "btnImportarJson";
            this.btnImportarJson.Size = new System.Drawing.Size(171, 25);
            this.btnImportarJson.TabIndex = 4;
            this.btnImportarJson.Tag = "FormLanguageManagement.ImportButton";
            this.btnImportarJson.Values.Text = "Importar JSON";
            this.btnImportarJson.Click += new System.EventHandler(this.btnImportarJson_Click);
            // 
            // btnExportarJson
            // 
            this.btnExportarJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportarJson.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnExportarJson.Location = new System.Drawing.Point(566, 197);
            this.btnExportarJson.Name = "btnExportarJson";
            this.btnExportarJson.Size = new System.Drawing.Size(171, 25);
            this.btnExportarJson.TabIndex = 5;
            this.btnExportarJson.Tag = "FormLanguageManagement.ExportButton";
            this.btnExportarJson.Values.Text = "Exportar JSON";
            this.btnExportarJson.Click += new System.EventHandler(this.btnExportarJson_Click);
            // 
            // groupBoxTraducciones
            // 
            this.groupBoxTraducciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTraducciones.Panel.Controls.Add(this.btnEliminarTraduccion);
            this.groupBoxTraducciones.Panel.Controls.Add(this.btnGuardarTraduccion);
            this.groupBoxTraducciones.Panel.Controls.Add(this.tlpTraduccionManual);
            this.groupBoxTraducciones.Panel.Controls.Add(this.dgvTraducciones);
            this.groupBoxTraducciones.Location = new System.Drawing.Point(23, 239);
            this.groupBoxTraducciones.Name = "groupBoxTraducciones";
            this.groupBoxTraducciones.Size = new System.Drawing.Size(714, 319);
            this.groupBoxTraducciones.TabIndex = 6;
            this.groupBoxTraducciones.TabStop = false;
            this.groupBoxTraducciones.Tag = "FormLanguageManagement.TranslationsGroup";
            this.groupBoxTraducciones.Values.Heading = "Traducciones";
            // 
            // btnEliminarTraduccion
            // 
            this.btnEliminarTraduccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarTraduccion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarTraduccion.Location = new System.Drawing.Point(489, 262);
            this.btnEliminarTraduccion.Name = "btnEliminarTraduccion";
            this.btnEliminarTraduccion.Size = new System.Drawing.Size(195, 28);
            this.btnEliminarTraduccion.TabIndex = 6;
            this.btnEliminarTraduccion.Tag = "FormLanguageManagement.DeleteTranslationButton";
            this.btnEliminarTraduccion.Values.Text = "Eliminar traduccion";
            this.btnEliminarTraduccion.Click += new System.EventHandler(this.btnEliminarTraduccion_Click);
            // 
            // btnGuardarTraduccion
            // 
            this.btnGuardarTraduccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardarTraduccion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnGuardarTraduccion.Location = new System.Drawing.Point(489, 223);
            this.btnGuardarTraduccion.Name = "btnGuardarTraduccion";
            this.btnGuardarTraduccion.Size = new System.Drawing.Size(195, 28);
            this.btnGuardarTraduccion.TabIndex = 5;
            this.btnGuardarTraduccion.Tag = "FormLanguageManagement.SaveTranslationButton";
            this.btnGuardarTraduccion.Values.Text = "Guardar traduccion";
            this.btnGuardarTraduccion.Click += new System.EventHandler(this.btnGuardarTraduccion_Click);
            // 
            // txtbValorManual
            // 
            this.txtbValorManual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbValorManual.Location = new System.Drawing.Point(181, 20);
            this.txtbValorManual.Name = "txtbValorManual";
            this.txtbValorManual.Size = new System.Drawing.Size(277, 20);
            this.txtbValorManual.TabIndex = 4;
            this.txtbValorManual.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // lblValorManual
            // 
            this.lblValorManual.AutoSize = true;
            this.lblValorManual.Location = new System.Drawing.Point(178, 0);
            this.lblValorManual.Name = "lblValorManual";
            this.lblValorManual.Size = new System.Drawing.Size(64, 13);
            this.lblValorManual.TabIndex = 3;
            this.lblValorManual.Tag = "FormLanguageManagement.ManualValueLabel";
            this.lblValorManual.Values.Text = "Traduccion:";
            this.lblValorManual.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbKeyManual
            // 
            this.txtbKeyManual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbKeyManual.Location = new System.Drawing.Point(3, 20);
            this.txtbKeyManual.Name = "txtbKeyManual";
            this.txtbKeyManual.ReadOnly = true;
            this.txtbKeyManual.Size = new System.Drawing.Size(161, 20);
            this.txtbKeyManual.TabIndex = 2;
            this.txtbKeyManual.Margin = new System.Windows.Forms.Padding(3, 0, 14, 0);
            // 
            // lblKeyManual
            // 
            this.lblKeyManual.AutoSize = true;
            this.lblKeyManual.Location = new System.Drawing.Point(0, 0);
            this.lblKeyManual.Name = "lblKeyManual";
            this.lblKeyManual.Size = new System.Drawing.Size(28, 13);
            this.lblKeyManual.TabIndex = 1;
            this.lblKeyManual.Tag = "FormLanguageManagement.ManualKeyLabel";
            this.lblKeyManual.Values.Text = "Key:";
            this.lblKeyManual.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // dgvTraducciones
            // 
            this.dgvTraducciones.AllowUserToAddRows = false;
            this.dgvTraducciones.AllowUserToDeleteRows = false;
            this.dgvTraducciones.AutoGenerateColumns = false;
            this.dgvTraducciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTraducciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTraducciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTraducciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTraduccionesKey,
            this.colTraduccionesValue});
            this.dgvTraducciones.Location = new System.Drawing.Point(14, 8);
            this.dgvTraducciones.MultiSelect = false;
            this.dgvTraducciones.Name = "dgvTraducciones";
            this.dgvTraducciones.ReadOnly = true;
            this.dgvTraducciones.RowHeadersVisible = false;
            this.dgvTraducciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTraducciones.Size = new System.Drawing.Size(670, 207);
            this.dgvTraducciones.TabIndex = 0;
            this.dgvTraducciones.Tag = "FormLanguageManagement.Grid";
            this.dgvTraducciones.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTraducciones_CellClick);
            //
            // colTraduccionesKey
            //
            this.colTraduccionesKey.DataPropertyName = "Key";
            this.colTraduccionesKey.HeaderText = "Key";
            this.colTraduccionesKey.Name = "Key";
            this.colTraduccionesKey.ReadOnly = true;
            //
            // colTraduccionesValue
            //
            this.colTraduccionesValue.DataPropertyName = "Valor";
            this.colTraduccionesValue.HeaderText = "Valor";
            this.colTraduccionesValue.Name = "Value";
            this.colTraduccionesValue.ReadOnly = true;
            // 
            // tlpNombreIdioma
            // 
            this.tlpNombreIdioma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpNombreIdioma.Controls.Add(this.chkbIdiomaDefault, 1, 1);
            this.tlpNombreIdioma.Controls.Add(this.txtbNombreIdioma, 0, 1);
            this.tlpNombreIdioma.Controls.Add(this.lblNombreIdioma, 0, 0);
            this.tlpNombreIdioma.Location = new System.Drawing.Point(11, 0);
            this.tlpNombreIdioma.Name = "tlpNombreIdioma";
            this.tlpNombreIdioma.Size = new System.Drawing.Size(455, 39);
            this.tlpNombreIdioma.ColumnCount = 2;
            this.tlpNombreIdioma.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpNombreIdioma.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tlpNombreIdioma.RowCount = 2;
            this.tlpNombreIdioma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpNombreIdioma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpNombreIdioma.TabIndex = 0;
            // 
            // tlpTraduccionManual
            // 
            this.tlpTraduccionManual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpTraduccionManual.Controls.Add(this.txtbValorManual, 1, 1);
            this.tlpTraduccionManual.Controls.Add(this.lblValorManual, 1, 0);
            this.tlpTraduccionManual.Controls.Add(this.txtbKeyManual, 0, 1);
            this.tlpTraduccionManual.Controls.Add(this.lblKeyManual, 0, 0);
            this.tlpTraduccionManual.Location = new System.Drawing.Point(11, 247);
            this.tlpTraduccionManual.Name = "tlpTraduccionManual";
            this.tlpTraduccionManual.Size = new System.Drawing.Size(458, 44);
            this.tlpTraduccionManual.ColumnCount = 2;
            this.tlpTraduccionManual.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.86F));
            this.tlpTraduccionManual.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.14F));
            this.tlpTraduccionManual.RowCount = 2;
            this.tlpTraduccionManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpTraduccionManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpTraduccionManual.TabIndex = 1;
            //
            // FormGestionIdiomas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 579);
            this.Controls.Add(this.groupBoxTraducciones);
            this.Controls.Add(this.btnExportarJson);
            this.Controls.Add(this.btnImportarJson);
            this.Controls.Add(this.cmbIdiomas);
            this.Controls.Add(this.lblIdiomaSeleccionado);
            this.Controls.Add(this.groupBoxIdioma);
            this.Controls.Add(this.lblTituloGestionIdiomas);
            this.Name = "FormGestionIdiomas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormLanguageManagement.Title";
            this.Text = "Gestion de idiomas";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxIdioma.Panel)).EndInit();
            this.groupBoxIdioma.Panel.ResumeLayout(false);
            this.groupBoxIdioma.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxIdioma)).EndInit();
            this.groupBoxIdioma.ResumeLayout(false);
            this.groupBoxIdioma.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTraducciones.Panel)).EndInit();
            this.groupBoxTraducciones.Panel.ResumeLayout(false);
            this.groupBoxTraducciones.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTraducciones)).EndInit();
            this.groupBoxTraducciones.ResumeLayout(false);
            this.groupBoxTraducciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).EndInit();
            this.tlpNombreIdioma.ResumeLayout(false);
            this.tlpNombreIdioma.PerformLayout();
            this.tlpTraduccionManual.ResumeLayout(false);
            this.tlpTraduccionManual.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloGestionIdiomas;
        private Krypton.Toolkit.KryptonGroupBox groupBoxIdioma;
        private Krypton.Toolkit.KryptonButton btnEliminarIdioma;
        private Krypton.Toolkit.KryptonButton btnActualizarIdioma;
        private Krypton.Toolkit.KryptonButton btnAgregarIdioma;
        private TpIDS.CheckBoxAccesible chkbIdiomaDefault;
        private Krypton.Toolkit.KryptonTextBox txtbNombreIdioma;
        private Krypton.Toolkit.KryptonLabel lblNombreIdioma;
        private Krypton.Toolkit.KryptonLabel lblIdiomaSeleccionado;
        private Krypton.Toolkit.KryptonComboBox cmbIdiomas;
        private Krypton.Toolkit.KryptonButton btnImportarJson;
        private Krypton.Toolkit.KryptonButton btnExportarJson;
        private Krypton.Toolkit.KryptonGroupBox groupBoxTraducciones;
        private Krypton.Toolkit.KryptonButton btnEliminarTraduccion;
        private Krypton.Toolkit.KryptonButton btnGuardarTraduccion;
        private Krypton.Toolkit.KryptonTextBox txtbValorManual;
        private Krypton.Toolkit.KryptonLabel lblValorManual;
        private Krypton.Toolkit.KryptonTextBox txtbKeyManual;
        private Krypton.Toolkit.KryptonLabel lblKeyManual;
        private Krypton.Toolkit.KryptonDataGridView dgvTraducciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTraduccionesKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTraduccionesValue;
        private Krypton.Toolkit.KryptonButton btnIdiomaDefault;
        private System.Windows.Forms.TableLayoutPanel tlpNombreIdioma;
        private System.Windows.Forms.TableLayoutPanel tlpTraduccionManual;
    }
}
