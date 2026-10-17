namespace TpIDS
{
    partial class FormBalanceGeneral
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
            this.lblTituloBalance = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxFiltrosBalance = new Krypton.Toolkit.KryptonGroupBox();
            this.btnBuscarBalance = new Krypton.Toolkit.KryptonButton();
            this.txtbBuscarBalance = new Krypton.Toolkit.KryptonTextBox();
            this.lblBuscarBalance = new Krypton.Toolkit.KryptonLabel();
            this.cmbTipoBalance = new Krypton.Toolkit.KryptonComboBox();
            this.lblTipoBalance = new Krypton.Toolkit.KryptonLabel();
            this.dtpHastaBalance = new TpIDS.DateTimePickerAccesible();
            this.lblHastaBalance = new Krypton.Toolkit.KryptonLabel();
            this.dtpDesdeBalance = new TpIDS.DateTimePickerAccesible();
            this.lblDesdeBalance = new Krypton.Toolkit.KryptonLabel();
            this.lblIngresosBalance = new Krypton.Toolkit.KryptonLabel();
            this.lblIngresosMontoBalance = new Krypton.Toolkit.KryptonLabel();
            this.lblEgresosBalance = new Krypton.Toolkit.KryptonLabel();
            this.lblEgresosMontoBalance = new Krypton.Toolkit.KryptonLabel();
            this.lblSaldoBalance = new Krypton.Toolkit.KryptonLabel();
            this.lblSaldoMontoBalance = new Krypton.Toolkit.KryptonLabel();
            this.dgvBalance = new Krypton.Toolkit.KryptonDataGridView();
            this.colBalanceFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceComercio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceMonto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnPaginaAnteriorBalance = new Krypton.Toolkit.KryptonButton();
            this.btnPaginaSiguienteBalance = new Krypton.Toolkit.KryptonButton();
            this.lblPaginaBalance = new Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBalance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBalance.Panel)).BeginInit();
            this.groupBoxFiltrosBalance.Panel.SuspendLayout();
            this.groupBoxFiltrosBalance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalance)).BeginInit();
            this.tlpFiltrosBalance = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFiltrosBalance.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloBalance
            //
            this.lblTituloBalance.AutoSize = true;
            this.lblTituloBalance.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloBalance.Location = new System.Drawing.Point(18, 15);
            this.lblTituloBalance.Name = "lblTituloBalance";
            this.lblTituloBalance.Size = new System.Drawing.Size(180, 25);
            this.lblTituloBalance.TabIndex = 0;
            this.lblTituloBalance.Tag = "FormBalanceSheet.TitleLabel";
            this.lblTituloBalance.Values.Text = "Balance General";
            //
            // groupBoxFiltrosBalance
            //
            this.groupBoxFiltrosBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFiltrosBalance.Panel.Controls.Add(this.btnBuscarBalance);
            this.groupBoxFiltrosBalance.Panel.Controls.Add(this.tlpFiltrosBalance);
            this.groupBoxFiltrosBalance.Location = new System.Drawing.Point(23, 52);
            this.groupBoxFiltrosBalance.Name = "groupBoxFiltrosBalance";
            this.groupBoxFiltrosBalance.Size = new System.Drawing.Size(900, 76);
            this.groupBoxFiltrosBalance.TabIndex = 1;
            this.groupBoxFiltrosBalance.TabStop = false;
            this.groupBoxFiltrosBalance.Tag = "FormBalanceSheet.FiltersGroup";
            this.groupBoxFiltrosBalance.Values.Heading = "Filtros";
            //
            // btnBuscarBalance
            //
            this.btnBuscarBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarBalance.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnBuscarBalance.Location = new System.Drawing.Point(748, 13);
            this.btnBuscarBalance.Name = "btnBuscarBalance";
            this.btnBuscarBalance.Size = new System.Drawing.Size(110, 27);
            this.btnBuscarBalance.TabIndex = 8;
            this.btnBuscarBalance.Tag = "FormBalanceSheet.SearchButton";
            this.btnBuscarBalance.Values.Text = "Buscar";
            this.btnBuscarBalance.Click += new System.EventHandler(this.btnBuscarBalance_Click);
            //
            // txtbBuscarBalance
            //
            this.txtbBuscarBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbBuscarBalance.Location = new System.Drawing.Point(471, 15);
            this.txtbBuscarBalance.Name = "txtbBuscarBalance";
            this.txtbBuscarBalance.Size = new System.Drawing.Size(250, 20);
            this.txtbBuscarBalance.TabIndex = 7;
            this.txtbBuscarBalance.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblBuscarBalance
            //
            this.lblBuscarBalance.AutoSize = true;
            this.lblBuscarBalance.Location = new System.Drawing.Point(468, 0);
            this.lblBuscarBalance.Name = "lblBuscarBalance";
            this.lblBuscarBalance.Size = new System.Drawing.Size(50, 13);
            this.lblBuscarBalance.TabIndex = 6;
            this.lblBuscarBalance.Tag = "FormBalanceSheet.SearchLabel";
            this.lblBuscarBalance.Values.Text = "Buscar:";
            this.lblBuscarBalance.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbTipoBalance
            //
            this.cmbTipoBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTipoBalance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoBalance.FormattingEnabled = true;
            this.cmbTipoBalance.Location = new System.Drawing.Point(315, 15);
            this.cmbTipoBalance.Name = "cmbTipoBalance";
            this.cmbTipoBalance.Size = new System.Drawing.Size(140, 21);
            this.cmbTipoBalance.TabIndex = 5;
            this.cmbTipoBalance.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            //
            // lblTipoBalance
            //
            this.lblTipoBalance.AutoSize = true;
            this.lblTipoBalance.Location = new System.Drawing.Point(312, 0);
            this.lblTipoBalance.Name = "lblTipoBalance";
            this.lblTipoBalance.Size = new System.Drawing.Size(33, 13);
            this.lblTipoBalance.TabIndex = 4;
            this.lblTipoBalance.Tag = "FormBalanceSheet.TypeLabel";
            this.lblTipoBalance.Values.Text = "Tipo:";
            this.lblTipoBalance.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dtpHastaBalance
            //
            this.dtpHastaBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpHastaBalance.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHastaBalance.Location = new System.Drawing.Point(159, 15);
            this.dtpHastaBalance.Name = "dtpHastaBalance";
            this.dtpHastaBalance.Size = new System.Drawing.Size(140, 20);
            this.dtpHastaBalance.TabIndex = 3;
            this.dtpHastaBalance.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            //
            // lblHastaBalance
            //
            this.lblHastaBalance.AutoSize = true;
            this.lblHastaBalance.Location = new System.Drawing.Point(156, 0);
            this.lblHastaBalance.Name = "lblHastaBalance";
            this.lblHastaBalance.Size = new System.Drawing.Size(41, 13);
            this.lblHastaBalance.TabIndex = 2;
            this.lblHastaBalance.Tag = "FormBalanceSheet.DateToLabel";
            this.lblHastaBalance.Values.Text = "Hasta:";
            this.lblHastaBalance.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dtpDesdeBalance
            //
            this.dtpDesdeBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDesdeBalance.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesdeBalance.Location = new System.Drawing.Point(3, 15);
            this.dtpDesdeBalance.Name = "dtpDesdeBalance";
            this.dtpDesdeBalance.Size = new System.Drawing.Size(140, 20);
            this.dtpDesdeBalance.TabIndex = 1;
            this.dtpDesdeBalance.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            //
            // lblDesdeBalance
            //
            this.lblDesdeBalance.AutoSize = true;
            this.lblDesdeBalance.Location = new System.Drawing.Point(0, 0);
            this.lblDesdeBalance.Name = "lblDesdeBalance";
            this.lblDesdeBalance.Size = new System.Drawing.Size(45, 13);
            this.lblDesdeBalance.TabIndex = 0;
            this.lblDesdeBalance.Tag = "FormBalanceSheet.DateFromLabel";
            this.lblDesdeBalance.Values.Text = "Desde:";
            this.lblDesdeBalance.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // lblIngresosBalance
            //
            this.lblIngresosBalance.AutoSize = true;
            this.lblIngresosBalance.Location = new System.Drawing.Point(23, 144);
            this.lblIngresosBalance.Name = "lblIngresosBalance";
            this.lblIngresosBalance.Size = new System.Drawing.Size(115, 13);
            this.lblIngresosBalance.TabIndex = 2;
            this.lblIngresosBalance.Tag = "FormBalanceSheet.IncomeLabel";
            this.lblIngresosBalance.Values.Text = "Ingresos del periodo:";
            //
            // lblIngresosMontoBalance
            //
            this.lblIngresosMontoBalance.AutoSize = true;
            this.lblIngresosMontoBalance.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblIngresosMontoBalance.Location = new System.Drawing.Point(200, 144);
            this.lblIngresosMontoBalance.Name = "lblIngresosMontoBalance";
            this.lblIngresosMontoBalance.Size = new System.Drawing.Size(28, 15);
            this.lblIngresosMontoBalance.TabIndex = 3;
            this.lblIngresosMontoBalance.Values.Text = "0,00";
            //
            // lblEgresosBalance
            //
            this.lblEgresosBalance.AutoSize = true;
            this.lblEgresosBalance.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblEgresosBalance.Location = new System.Drawing.Point(360, 144);
            this.lblEgresosBalance.Name = "lblEgresosBalance";
            this.lblEgresosBalance.Size = new System.Drawing.Size(113, 13);
            this.lblEgresosBalance.TabIndex = 4;
            this.lblEgresosBalance.Tag = "FormBalanceSheet.ExpenseLabel";
            this.lblEgresosBalance.Values.Text = "Egresos del periodo:";
            //
            // lblEgresosMontoBalance
            //
            this.lblEgresosMontoBalance.AutoSize = true;
            this.lblEgresosMontoBalance.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblEgresosMontoBalance.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEgresosMontoBalance.Location = new System.Drawing.Point(535, 144);
            this.lblEgresosMontoBalance.Name = "lblEgresosMontoBalance";
            this.lblEgresosMontoBalance.Size = new System.Drawing.Size(28, 15);
            this.lblEgresosMontoBalance.TabIndex = 5;
            this.lblEgresosMontoBalance.Values.Text = "0,00";
            //
            // lblSaldoBalance
            //
            this.lblSaldoBalance.AutoSize = true;
            this.lblSaldoBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSaldoBalance.Location = new System.Drawing.Point(700, 144);
            this.lblSaldoBalance.Name = "lblSaldoBalance";
            this.lblSaldoBalance.Size = new System.Drawing.Size(113, 13);
            this.lblSaldoBalance.TabIndex = 6;
            this.lblSaldoBalance.Tag = "FormBalanceSheet.BalanceLabel";
            this.lblSaldoBalance.Values.Text = "Balance del periodo:";
            //
            // lblSaldoMontoBalance
            //
            this.lblSaldoMontoBalance.AutoSize = true;
            this.lblSaldoMontoBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSaldoMontoBalance.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSaldoMontoBalance.Location = new System.Drawing.Point(875, 144);
            this.lblSaldoMontoBalance.Name = "lblSaldoMontoBalance";
            this.lblSaldoMontoBalance.Size = new System.Drawing.Size(28, 15);
            this.lblSaldoMontoBalance.TabIndex = 7;
            this.lblSaldoMontoBalance.Values.Text = "0,00";
            //
            // dgvBalance
            //
            this.dgvBalance.AllowUserToAddRows = false;
            this.dgvBalance.AllowUserToDeleteRows = false;
            this.dgvBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBalance.AutoGenerateColumns = false;
            this.dgvBalance.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBalance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBalance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBalanceFecha,
            this.colBalanceTipo,
            this.colBalanceCategoria,
            this.colBalanceComercio,
            this.colBalanceMonto,
            this.colBalanceDescripcion});
            this.dgvBalance.Location = new System.Drawing.Point(23, 178);
            this.dgvBalance.MultiSelect = false;
            this.dgvBalance.Name = "dgvBalance";
            this.dgvBalance.ReadOnly = true;
            this.dgvBalance.RowHeadersVisible = false;
            this.dgvBalance.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBalance.Size = new System.Drawing.Size(900, 380);
            this.dgvBalance.TabIndex = 8;
            this.dgvBalance.Tag = "FormBalanceSheet.Grid";
            //
            // colBalanceFecha
            //
            this.colBalanceFecha.DataPropertyName = "Fecha";
            this.colBalanceFecha.HeaderText = "Fecha";
            this.colBalanceFecha.Name = "Fecha";
            this.colBalanceFecha.ReadOnly = true;
            //
            // colBalanceTipo
            //
            this.colBalanceTipo.DataPropertyName = "Tipo";
            this.colBalanceTipo.HeaderText = "Tipo";
            this.colBalanceTipo.Name = "Tipo";
            this.colBalanceTipo.ReadOnly = true;
            //
            // colBalanceCategoria
            //
            this.colBalanceCategoria.DataPropertyName = "Categoria";
            this.colBalanceCategoria.HeaderText = "Categoria";
            this.colBalanceCategoria.Name = "Categoria";
            this.colBalanceCategoria.ReadOnly = true;
            //
            // colBalanceComercio
            //
            this.colBalanceComercio.DataPropertyName = "Comercio";
            this.colBalanceComercio.HeaderText = "Comercio";
            this.colBalanceComercio.Name = "Comercio";
            this.colBalanceComercio.ReadOnly = true;
            //
            // colBalanceMonto
            //
            this.colBalanceMonto.DataPropertyName = "Monto";
            this.colBalanceMonto.HeaderText = "Monto";
            this.colBalanceMonto.Name = "Monto";
            this.colBalanceMonto.ReadOnly = true;
            //
            // colBalanceDescripcion
            //
            this.colBalanceDescripcion.DataPropertyName = "Descripcion";
            this.colBalanceDescripcion.HeaderText = "Descripcion";
            this.colBalanceDescripcion.Name = "Descripcion";
            this.colBalanceDescripcion.ReadOnly = true;
            //
            // btnPaginaAnteriorBalance
            //
            this.btnPaginaAnteriorBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaginaAnteriorBalance.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnPaginaAnteriorBalance.Location = new System.Drawing.Point(23, 568);
            this.btnPaginaAnteriorBalance.Name = "btnPaginaAnteriorBalance";
            this.btnPaginaAnteriorBalance.Size = new System.Drawing.Size(110, 27);
            this.btnPaginaAnteriorBalance.TabIndex = 9;
            this.btnPaginaAnteriorBalance.Tag = "FormBalanceSheet.PreviousPageButton";
            this.btnPaginaAnteriorBalance.Values.Text = "Anterior";
            this.btnPaginaAnteriorBalance.Click += new System.EventHandler(this.btnPaginaAnteriorBalance_Click);
            //
            // btnPaginaSiguienteBalance
            //
            this.btnPaginaSiguienteBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaginaSiguienteBalance.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnPaginaSiguienteBalance.Location = new System.Drawing.Point(320, 568);
            this.btnPaginaSiguienteBalance.Name = "btnPaginaSiguienteBalance";
            this.btnPaginaSiguienteBalance.Size = new System.Drawing.Size(110, 27);
            this.btnPaginaSiguienteBalance.TabIndex = 10;
            this.btnPaginaSiguienteBalance.Tag = "FormBalanceSheet.NextPageButton";
            this.btnPaginaSiguienteBalance.Values.Text = "Siguiente";
            this.btnPaginaSiguienteBalance.Click += new System.EventHandler(this.btnPaginaSiguienteBalance_Click);
            //
            // lblPaginaBalance
            //
            this.lblPaginaBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPaginaBalance.AutoSize = true;
            this.lblPaginaBalance.Location = new System.Drawing.Point(145, 572);
            this.lblPaginaBalance.Name = "lblPaginaBalance";
            this.lblPaginaBalance.Size = new System.Drawing.Size(90, 13);
            this.lblPaginaBalance.TabIndex = 11;
            this.lblPaginaBalance.Values.Text = "Pagina 1 de 1";
            // 
            // tlpFiltrosBalance
            // 
            this.tlpFiltrosBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpFiltrosBalance.Controls.Add(this.txtbBuscarBalance, 3, 1);
            this.tlpFiltrosBalance.Controls.Add(this.lblBuscarBalance, 3, 0);
            this.tlpFiltrosBalance.Controls.Add(this.cmbTipoBalance, 2, 1);
            this.tlpFiltrosBalance.Controls.Add(this.lblTipoBalance, 2, 0);
            this.tlpFiltrosBalance.Controls.Add(this.dtpHastaBalance, 1, 1);
            this.tlpFiltrosBalance.Controls.Add(this.lblHastaBalance, 1, 0);
            this.tlpFiltrosBalance.Controls.Add(this.dtpDesdeBalance, 0, 1);
            this.tlpFiltrosBalance.Controls.Add(this.lblDesdeBalance, 0, 0);
            this.tlpFiltrosBalance.Location = new System.Drawing.Point(11, 0);
            this.tlpFiltrosBalance.Name = "tlpFiltrosBalance";
            this.tlpFiltrosBalance.Size = new System.Drawing.Size(721, 40);
            this.tlpFiltrosBalance.ColumnCount = 4;
            this.tlpFiltrosBalance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.64F));
            this.tlpFiltrosBalance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.64F));
            this.tlpFiltrosBalance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.64F));
            this.tlpFiltrosBalance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.09F));
            this.tlpFiltrosBalance.RowCount = 2;
            this.tlpFiltrosBalance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpFiltrosBalance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpFiltrosBalance.TabIndex = 0;
            //
            // FormBalanceGeneral
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 620);
            this.Controls.Add(this.lblPaginaBalance);
            this.Controls.Add(this.btnPaginaSiguienteBalance);
            this.Controls.Add(this.btnPaginaAnteriorBalance);
            this.Controls.Add(this.dgvBalance);
            this.Controls.Add(this.lblSaldoMontoBalance);
            this.Controls.Add(this.lblSaldoBalance);
            this.Controls.Add(this.lblEgresosMontoBalance);
            this.Controls.Add(this.lblEgresosBalance);
            this.Controls.Add(this.lblIngresosMontoBalance);
            this.Controls.Add(this.lblIngresosBalance);
            this.Controls.Add(this.groupBoxFiltrosBalance);
            this.Controls.Add(this.lblTituloBalance);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "FormBalanceGeneral";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormBalanceSheet.Title";
            this.Text = "Balance General";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBalance.Panel)).EndInit();
            this.groupBoxFiltrosBalance.Panel.ResumeLayout(false);
            this.groupBoxFiltrosBalance.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBalance)).EndInit();
            this.groupBoxFiltrosBalance.ResumeLayout(false);
            this.groupBoxFiltrosBalance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalance)).EndInit();
            this.tlpFiltrosBalance.ResumeLayout(false);
            this.tlpFiltrosBalance.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloBalance;
        private Krypton.Toolkit.KryptonGroupBox groupBoxFiltrosBalance;
        private Krypton.Toolkit.KryptonLabel lblDesdeBalance;
        private TpIDS.DateTimePickerAccesible dtpDesdeBalance;
        private Krypton.Toolkit.KryptonLabel lblHastaBalance;
        private TpIDS.DateTimePickerAccesible dtpHastaBalance;
        private Krypton.Toolkit.KryptonLabel lblTipoBalance;
        private Krypton.Toolkit.KryptonComboBox cmbTipoBalance;
        private Krypton.Toolkit.KryptonLabel lblBuscarBalance;
        private Krypton.Toolkit.KryptonTextBox txtbBuscarBalance;
        private Krypton.Toolkit.KryptonButton btnBuscarBalance;
        private Krypton.Toolkit.KryptonLabel lblIngresosBalance;
        private Krypton.Toolkit.KryptonLabel lblIngresosMontoBalance;
        private Krypton.Toolkit.KryptonLabel lblEgresosBalance;
        private Krypton.Toolkit.KryptonLabel lblEgresosMontoBalance;
        private Krypton.Toolkit.KryptonLabel lblSaldoBalance;
        private Krypton.Toolkit.KryptonLabel lblSaldoMontoBalance;
        private Krypton.Toolkit.KryptonDataGridView dgvBalance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceComercio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceMonto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceDescripcion;
        private Krypton.Toolkit.KryptonButton btnPaginaAnteriorBalance;
        private Krypton.Toolkit.KryptonButton btnPaginaSiguienteBalance;
        private Krypton.Toolkit.KryptonLabel lblPaginaBalance;
        private System.Windows.Forms.TableLayoutPanel tlpFiltrosBalance;
    }
}
