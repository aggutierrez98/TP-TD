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
            this.lblTituloBalance = new System.Windows.Forms.Label();
            this.groupBoxFiltrosBalance = new System.Windows.Forms.GroupBox();
            this.btnBuscarBalance = new System.Windows.Forms.Button();
            this.txtbBuscarBalance = new System.Windows.Forms.TextBox();
            this.lblBuscarBalance = new System.Windows.Forms.Label();
            this.cmbTipoBalance = new System.Windows.Forms.ComboBox();
            this.lblTipoBalance = new System.Windows.Forms.Label();
            this.dtpHastaBalance = new System.Windows.Forms.DateTimePicker();
            this.lblHastaBalance = new System.Windows.Forms.Label();
            this.dtpDesdeBalance = new System.Windows.Forms.DateTimePicker();
            this.lblDesdeBalance = new System.Windows.Forms.Label();
            this.lblIngresosBalance = new System.Windows.Forms.Label();
            this.lblIngresosMontoBalance = new System.Windows.Forms.Label();
            this.lblEgresosBalance = new System.Windows.Forms.Label();
            this.lblEgresosMontoBalance = new System.Windows.Forms.Label();
            this.lblSaldoBalance = new System.Windows.Forms.Label();
            this.lblSaldoMontoBalance = new System.Windows.Forms.Label();
            this.dgvBalance = new System.Windows.Forms.DataGridView();
            this.colBalanceFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceComercio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceMonto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBalanceDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnPaginaAnteriorBalance = new System.Windows.Forms.Button();
            this.btnPaginaSiguienteBalance = new System.Windows.Forms.Button();
            this.lblPaginaBalance = new System.Windows.Forms.Label();
            this.groupBoxFiltrosBalance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalance)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloBalance
            //
            this.lblTituloBalance.AutoSize = true;
            this.lblTituloBalance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloBalance.Location = new System.Drawing.Point(18, 15);
            this.lblTituloBalance.Name = "lblTituloBalance";
            this.lblTituloBalance.Size = new System.Drawing.Size(180, 25);
            this.lblTituloBalance.TabIndex = 0;
            this.lblTituloBalance.Tag = "FormBalanceSheet.TitleLabel";
            this.lblTituloBalance.Text = "Balance General";
            //
            // groupBoxFiltrosBalance
            //
            this.groupBoxFiltrosBalance.Controls.Add(this.btnBuscarBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.txtbBuscarBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.lblBuscarBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.cmbTipoBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.lblTipoBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.dtpHastaBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.lblHastaBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.dtpDesdeBalance);
            this.groupBoxFiltrosBalance.Controls.Add(this.lblDesdeBalance);
            this.groupBoxFiltrosBalance.Location = new System.Drawing.Point(23, 52);
            this.groupBoxFiltrosBalance.Name = "groupBoxFiltrosBalance";
            this.groupBoxFiltrosBalance.Size = new System.Drawing.Size(900, 76);
            this.groupBoxFiltrosBalance.TabIndex = 1;
            this.groupBoxFiltrosBalance.TabStop = false;
            this.groupBoxFiltrosBalance.Tag = "FormBalanceSheet.FiltersGroup";
            this.groupBoxFiltrosBalance.Text = "Filtros";
            //
            // btnBuscarBalance
            //
            this.btnBuscarBalance.Location = new System.Drawing.Point(748, 29);
            this.btnBuscarBalance.Name = "btnBuscarBalance";
            this.btnBuscarBalance.Size = new System.Drawing.Size(110, 27);
            this.btnBuscarBalance.TabIndex = 8;
            this.btnBuscarBalance.Tag = "FormBalanceSheet.SearchButton";
            this.btnBuscarBalance.Text = "Buscar";
            this.btnBuscarBalance.UseVisualStyleBackColor = true;
            this.btnBuscarBalance.Click += new System.EventHandler(this.btnBuscarBalance_Click);
            //
            // txtbBuscarBalance
            //
            this.txtbBuscarBalance.Location = new System.Drawing.Point(482, 31);
            this.txtbBuscarBalance.Name = "txtbBuscarBalance";
            this.txtbBuscarBalance.Size = new System.Drawing.Size(250, 20);
            this.txtbBuscarBalance.TabIndex = 7;
            //
            // lblBuscarBalance
            //
            this.lblBuscarBalance.AutoSize = true;
            this.lblBuscarBalance.Location = new System.Drawing.Point(479, 15);
            this.lblBuscarBalance.Name = "lblBuscarBalance";
            this.lblBuscarBalance.Size = new System.Drawing.Size(50, 13);
            this.lblBuscarBalance.TabIndex = 6;
            this.lblBuscarBalance.Tag = "FormBalanceSheet.SearchLabel";
            this.lblBuscarBalance.Text = "Buscar:";
            //
            // cmbTipoBalance
            //
            this.cmbTipoBalance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoBalance.FormattingEnabled = true;
            this.cmbTipoBalance.Location = new System.Drawing.Point(326, 31);
            this.cmbTipoBalance.Name = "cmbTipoBalance";
            this.cmbTipoBalance.Size = new System.Drawing.Size(140, 21);
            this.cmbTipoBalance.TabIndex = 5;
            //
            // lblTipoBalance
            //
            this.lblTipoBalance.AutoSize = true;
            this.lblTipoBalance.Location = new System.Drawing.Point(323, 15);
            this.lblTipoBalance.Name = "lblTipoBalance";
            this.lblTipoBalance.Size = new System.Drawing.Size(33, 13);
            this.lblTipoBalance.TabIndex = 4;
            this.lblTipoBalance.Tag = "FormBalanceSheet.TypeLabel";
            this.lblTipoBalance.Text = "Tipo:";
            //
            // dtpHastaBalance
            //
            this.dtpHastaBalance.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHastaBalance.Location = new System.Drawing.Point(170, 31);
            this.dtpHastaBalance.Name = "dtpHastaBalance";
            this.dtpHastaBalance.Size = new System.Drawing.Size(140, 20);
            this.dtpHastaBalance.TabIndex = 3;
            //
            // lblHastaBalance
            //
            this.lblHastaBalance.AutoSize = true;
            this.lblHastaBalance.Location = new System.Drawing.Point(167, 15);
            this.lblHastaBalance.Name = "lblHastaBalance";
            this.lblHastaBalance.Size = new System.Drawing.Size(41, 13);
            this.lblHastaBalance.TabIndex = 2;
            this.lblHastaBalance.Tag = "FormBalanceSheet.DateToLabel";
            this.lblHastaBalance.Text = "Hasta:";
            //
            // dtpDesdeBalance
            //
            this.dtpDesdeBalance.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesdeBalance.Location = new System.Drawing.Point(14, 31);
            this.dtpDesdeBalance.Name = "dtpDesdeBalance";
            this.dtpDesdeBalance.Size = new System.Drawing.Size(140, 20);
            this.dtpDesdeBalance.TabIndex = 1;
            //
            // lblDesdeBalance
            //
            this.lblDesdeBalance.AutoSize = true;
            this.lblDesdeBalance.Location = new System.Drawing.Point(11, 15);
            this.lblDesdeBalance.Name = "lblDesdeBalance";
            this.lblDesdeBalance.Size = new System.Drawing.Size(45, 13);
            this.lblDesdeBalance.TabIndex = 0;
            this.lblDesdeBalance.Tag = "FormBalanceSheet.DateFromLabel";
            this.lblDesdeBalance.Text = "Desde:";
            //
            // lblIngresosBalance
            //
            this.lblIngresosBalance.AutoSize = true;
            this.lblIngresosBalance.Location = new System.Drawing.Point(23, 148);
            this.lblIngresosBalance.Name = "lblIngresosBalance";
            this.lblIngresosBalance.Size = new System.Drawing.Size(115, 13);
            this.lblIngresosBalance.TabIndex = 2;
            this.lblIngresosBalance.Tag = "FormBalanceSheet.IncomeLabel";
            this.lblIngresosBalance.Text = "Ingresos del periodo:";
            //
            // lblIngresosMontoBalance
            //
            this.lblIngresosMontoBalance.AutoSize = true;
            this.lblIngresosMontoBalance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblIngresosMontoBalance.Location = new System.Drawing.Point(200, 148);
            this.lblIngresosMontoBalance.Name = "lblIngresosMontoBalance";
            this.lblIngresosMontoBalance.Size = new System.Drawing.Size(28, 15);
            this.lblIngresosMontoBalance.TabIndex = 3;
            this.lblIngresosMontoBalance.Text = "0,00";
            //
            // lblEgresosBalance
            //
            this.lblEgresosBalance.AutoSize = true;
            this.lblEgresosBalance.Location = new System.Drawing.Point(360, 148);
            this.lblEgresosBalance.Name = "lblEgresosBalance";
            this.lblEgresosBalance.Size = new System.Drawing.Size(113, 13);
            this.lblEgresosBalance.TabIndex = 4;
            this.lblEgresosBalance.Tag = "FormBalanceSheet.ExpenseLabel";
            this.lblEgresosBalance.Text = "Egresos del periodo:";
            //
            // lblEgresosMontoBalance
            //
            this.lblEgresosMontoBalance.AutoSize = true;
            this.lblEgresosMontoBalance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEgresosMontoBalance.Location = new System.Drawing.Point(535, 148);
            this.lblEgresosMontoBalance.Name = "lblEgresosMontoBalance";
            this.lblEgresosMontoBalance.Size = new System.Drawing.Size(28, 15);
            this.lblEgresosMontoBalance.TabIndex = 5;
            this.lblEgresosMontoBalance.Text = "0,00";
            //
            // lblSaldoBalance
            //
            this.lblSaldoBalance.AutoSize = true;
            this.lblSaldoBalance.Location = new System.Drawing.Point(700, 148);
            this.lblSaldoBalance.Name = "lblSaldoBalance";
            this.lblSaldoBalance.Size = new System.Drawing.Size(113, 13);
            this.lblSaldoBalance.TabIndex = 6;
            this.lblSaldoBalance.Tag = "FormBalanceSheet.BalanceLabel";
            this.lblSaldoBalance.Text = "Balance del periodo:";
            //
            // lblSaldoMontoBalance
            //
            this.lblSaldoMontoBalance.AutoSize = true;
            this.lblSaldoMontoBalance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSaldoMontoBalance.Location = new System.Drawing.Point(875, 148);
            this.lblSaldoMontoBalance.Name = "lblSaldoMontoBalance";
            this.lblSaldoMontoBalance.Size = new System.Drawing.Size(28, 15);
            this.lblSaldoMontoBalance.TabIndex = 7;
            this.lblSaldoMontoBalance.Text = "0,00";
            //
            // dgvBalance
            //
            this.dgvBalance.AllowUserToAddRows = false;
            this.dgvBalance.AllowUserToDeleteRows = false;
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
            this.btnPaginaAnteriorBalance.Location = new System.Drawing.Point(23, 568);
            this.btnPaginaAnteriorBalance.Name = "btnPaginaAnteriorBalance";
            this.btnPaginaAnteriorBalance.Size = new System.Drawing.Size(110, 27);
            this.btnPaginaAnteriorBalance.TabIndex = 9;
            this.btnPaginaAnteriorBalance.Tag = "FormBalanceSheet.PreviousPageButton";
            this.btnPaginaAnteriorBalance.Text = "Anterior";
            this.btnPaginaAnteriorBalance.UseVisualStyleBackColor = true;
            this.btnPaginaAnteriorBalance.Click += new System.EventHandler(this.btnPaginaAnteriorBalance_Click);
            //
            // btnPaginaSiguienteBalance
            //
            this.btnPaginaSiguienteBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaginaSiguienteBalance.Location = new System.Drawing.Point(320, 568);
            this.btnPaginaSiguienteBalance.Name = "btnPaginaSiguienteBalance";
            this.btnPaginaSiguienteBalance.Size = new System.Drawing.Size(110, 27);
            this.btnPaginaSiguienteBalance.TabIndex = 10;
            this.btnPaginaSiguienteBalance.Tag = "FormBalanceSheet.NextPageButton";
            this.btnPaginaSiguienteBalance.Text = "Siguiente";
            this.btnPaginaSiguienteBalance.UseVisualStyleBackColor = true;
            this.btnPaginaSiguienteBalance.Click += new System.EventHandler(this.btnPaginaSiguienteBalance_Click);
            //
            // lblPaginaBalance
            //
            this.lblPaginaBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPaginaBalance.AutoSize = true;
            this.lblPaginaBalance.Location = new System.Drawing.Point(145, 576);
            this.lblPaginaBalance.Name = "lblPaginaBalance";
            this.lblPaginaBalance.Size = new System.Drawing.Size(90, 13);
            this.lblPaginaBalance.TabIndex = 11;
            this.lblPaginaBalance.Text = "Pagina 1 de 1";
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
            this.groupBoxFiltrosBalance.ResumeLayout(false);
            this.groupBoxFiltrosBalance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloBalance;
        private System.Windows.Forms.GroupBox groupBoxFiltrosBalance;
        private System.Windows.Forms.Label lblDesdeBalance;
        private System.Windows.Forms.DateTimePicker dtpDesdeBalance;
        private System.Windows.Forms.Label lblHastaBalance;
        private System.Windows.Forms.DateTimePicker dtpHastaBalance;
        private System.Windows.Forms.Label lblTipoBalance;
        private System.Windows.Forms.ComboBox cmbTipoBalance;
        private System.Windows.Forms.Label lblBuscarBalance;
        private System.Windows.Forms.TextBox txtbBuscarBalance;
        private System.Windows.Forms.Button btnBuscarBalance;
        private System.Windows.Forms.Label lblIngresosBalance;
        private System.Windows.Forms.Label lblIngresosMontoBalance;
        private System.Windows.Forms.Label lblEgresosBalance;
        private System.Windows.Forms.Label lblEgresosMontoBalance;
        private System.Windows.Forms.Label lblSaldoBalance;
        private System.Windows.Forms.Label lblSaldoMontoBalance;
        private System.Windows.Forms.DataGridView dgvBalance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceComercio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceMonto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceDescripcion;
        private System.Windows.Forms.Button btnPaginaAnteriorBalance;
        private System.Windows.Forms.Button btnPaginaSiguienteBalance;
        private System.Windows.Forms.Label lblPaginaBalance;
    }
}
