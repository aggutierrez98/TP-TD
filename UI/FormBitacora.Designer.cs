namespace TpIDS
{
    partial class FormBitacora
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
            this.lblTituloBitacora = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxFiltrosBitacora = new Krypton.Toolkit.KryptonGroupBox();
            this.chkbFechaDesdeBitacora = new TpIDS.CheckBoxAccesible();
            this.dtpFechaDesdeBitacora = new TpIDS.DateTimePickerAccesible();
            this.chkbFechaHastaBitacora = new TpIDS.CheckBoxAccesible();
            this.dtpFechaHastaBitacora = new TpIDS.DateTimePickerAccesible();
            this.lblUsuarioBitacora = new Krypton.Toolkit.KryptonLabel();
            this.txtbUsuarioBitacora = new Krypton.Toolkit.KryptonTextBox();
            this.lblActividadBitacora = new Krypton.Toolkit.KryptonLabel();
            this.txtbActividadBitacora = new Krypton.Toolkit.KryptonTextBox();
            this.lblNivelBitacora = new Krypton.Toolkit.KryptonLabel();
            this.cmbNivelBitacora = new Krypton.Toolkit.KryptonComboBox();
            this.chkbSoloTecnicaBitacora = new TpIDS.CheckBoxAccesible();
            this.btnBuscarBitacora = new Krypton.Toolkit.KryptonButton();
            this.dgvBitacora = new Krypton.Toolkit.KryptonDataGridView();
            this.colBitacoraId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraUsernameSnapshot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraActivity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraInformation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraIsTechnical = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBitacora)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBitacora.Panel)).BeginInit();
            this.groupBoxFiltrosBitacora.Panel.SuspendLayout();
            this.groupBoxFiltrosBitacora.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).BeginInit();
            this.tlpFechasBitacora = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFechasBitacora.SuspendLayout();
            this.tlpCamposBitacora = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposBitacora.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloBitacora
            // 
            this.lblTituloBitacora.AutoSize = true;
            this.lblTituloBitacora.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloBitacora.Location = new System.Drawing.Point(15, 15);
            this.lblTituloBitacora.Name = "lblTituloBitacora";
            this.lblTituloBitacora.Size = new System.Drawing.Size(0, 25);
            this.lblTituloBitacora.Tag = "FormAuditLog.TitleLabel";
            this.lblTituloBitacora.TabIndex = 0;
            // 
            // groupBoxFiltrosBitacora
            // 
            this.groupBoxFiltrosBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFiltrosBitacora.Panel.Controls.Add(this.tlpFechasBitacora);
            this.groupBoxFiltrosBitacora.Panel.Controls.Add(this.tlpCamposBitacora);
            this.groupBoxFiltrosBitacora.Panel.Controls.Add(this.btnBuscarBitacora);
            this.groupBoxFiltrosBitacora.Location = new System.Drawing.Point(18, 50);
            this.groupBoxFiltrosBitacora.Name = "groupBoxFiltrosBitacora";
            this.groupBoxFiltrosBitacora.Size = new System.Drawing.Size(944, 110);
            this.groupBoxFiltrosBitacora.Tag = "FormAuditLog.FiltersGroup";
            this.groupBoxFiltrosBitacora.TabIndex = 1;
            this.groupBoxFiltrosBitacora.TabStop = false;
            // 
            // chkbFechaDesdeBitacora
            // 
            this.chkbFechaDesdeBitacora.Location = new System.Drawing.Point(0, 2);
            this.chkbFechaDesdeBitacora.Name = "chkbFechaDesdeBitacora";
            this.chkbFechaDesdeBitacora.Size = new System.Drawing.Size(110, 20);
            this.chkbFechaDesdeBitacora.Tag = "FormAuditLog.FromCheck";
            this.chkbFechaDesdeBitacora.TabIndex = 0;
            this.chkbFechaDesdeBitacora.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            // 
            // dtpFechaDesdeBitacora
            // 
            this.dtpFechaDesdeBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFechaDesdeBitacora.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFechaDesdeBitacora.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaDesdeBitacora.Location = new System.Drawing.Point(115, 0);
            this.dtpFechaDesdeBitacora.Name = "dtpFechaDesdeBitacora";
            this.dtpFechaDesdeBitacora.Size = new System.Drawing.Size(185, 20);
            this.dtpFechaDesdeBitacora.TabIndex = 1;
            this.dtpFechaDesdeBitacora.Margin = new System.Windows.Forms.Padding(0, 0, 25, 0);
            // 
            // chkbFechaHastaBitacora
            // 
            this.chkbFechaHastaBitacora.Location = new System.Drawing.Point(325, 2);
            this.chkbFechaHastaBitacora.Name = "chkbFechaHastaBitacora";
            this.chkbFechaHastaBitacora.Size = new System.Drawing.Size(110, 20);
            this.chkbFechaHastaBitacora.Tag = "FormAuditLog.ToCheck";
            this.chkbFechaHastaBitacora.TabIndex = 2;
            this.chkbFechaHastaBitacora.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            // 
            // dtpFechaHastaBitacora
            // 
            this.dtpFechaHastaBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFechaHastaBitacora.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFechaHastaBitacora.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaHastaBitacora.Location = new System.Drawing.Point(440, 0);
            this.dtpFechaHastaBitacora.Name = "dtpFechaHastaBitacora";
            this.dtpFechaHastaBitacora.Size = new System.Drawing.Size(185, 20);
            this.dtpFechaHastaBitacora.TabIndex = 3;
            this.dtpFechaHastaBitacora.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // lblUsuarioBitacora
            // 
            this.lblUsuarioBitacora.AutoSize = true;
            this.lblUsuarioBitacora.Location = new System.Drawing.Point(0, 0);
            this.lblUsuarioBitacora.Name = "lblUsuarioBitacora";
            this.lblUsuarioBitacora.Size = new System.Drawing.Size(0, 13);
            this.lblUsuarioBitacora.Tag = "FormAuditLog.UserLabel";
            this.lblUsuarioBitacora.TabIndex = 4;
            this.lblUsuarioBitacora.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbUsuarioBitacora
            // 
            this.txtbUsuarioBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbUsuarioBitacora.Location = new System.Drawing.Point(3, 20);
            this.txtbUsuarioBitacora.Name = "txtbUsuarioBitacora";
            this.txtbUsuarioBitacora.Size = new System.Drawing.Size(180, 20);
            this.txtbUsuarioBitacora.TabIndex = 5;
            this.txtbUsuarioBitacora.Margin = new System.Windows.Forms.Padding(3, 0, 22, 0);
            // 
            // lblActividadBitacora
            // 
            this.lblActividadBitacora.AutoSize = true;
            this.lblActividadBitacora.Location = new System.Drawing.Point(205, 0);
            this.lblActividadBitacora.Name = "lblActividadBitacora";
            this.lblActividadBitacora.Size = new System.Drawing.Size(0, 13);
            this.lblActividadBitacora.Tag = "FormAuditLog.ActivityLabel";
            this.lblActividadBitacora.TabIndex = 6;
            this.lblActividadBitacora.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbActividadBitacora
            // 
            this.txtbActividadBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbActividadBitacora.Location = new System.Drawing.Point(208, 20);
            this.txtbActividadBitacora.Name = "txtbActividadBitacora";
            this.txtbActividadBitacora.Size = new System.Drawing.Size(220, 20);
            this.txtbActividadBitacora.TabIndex = 7;
            this.txtbActividadBitacora.Margin = new System.Windows.Forms.Padding(3, 0, 22, 0);
            // 
            // lblNivelBitacora
            // 
            this.lblNivelBitacora.AutoSize = true;
            this.lblNivelBitacora.Location = new System.Drawing.Point(450, 0);
            this.lblNivelBitacora.Name = "lblNivelBitacora";
            this.lblNivelBitacora.Size = new System.Drawing.Size(0, 13);
            this.lblNivelBitacora.Tag = "FormAuditLog.LevelLabel";
            this.lblNivelBitacora.TabIndex = 8;
            this.lblNivelBitacora.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // cmbNivelBitacora
            // 
            this.cmbNivelBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbNivelBitacora.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNivelBitacora.FormattingEnabled = true;
            this.cmbNivelBitacora.Location = new System.Drawing.Point(453, 20);
            this.cmbNivelBitacora.Name = "cmbNivelBitacora";
            this.cmbNivelBitacora.Size = new System.Drawing.Size(160, 21);
            this.cmbNivelBitacora.TabIndex = 9;
            this.cmbNivelBitacora.Margin = new System.Windows.Forms.Padding(3, 0, 22, 0);
            // 
            // chkbSoloTecnicaBitacora
            // 
            this.chkbSoloTecnicaBitacora.Location = new System.Drawing.Point(635, 20);
            this.chkbSoloTecnicaBitacora.Name = "chkbSoloTecnicaBitacora";
            this.chkbSoloTecnicaBitacora.Size = new System.Drawing.Size(140, 20);
            this.chkbSoloTecnicaBitacora.Tag = "FormAuditLog.OnlyTechnicalCheck";
            this.chkbSoloTecnicaBitacora.TabIndex = 10;
            this.chkbSoloTecnicaBitacora.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // btnBuscarBitacora
            // 
            this.btnBuscarBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarBitacora.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnBuscarBitacora.Location = new System.Drawing.Point(800, 56);
            this.btnBuscarBitacora.Name = "btnBuscarBitacora";
            this.btnBuscarBitacora.Size = new System.Drawing.Size(125, 30);
            this.btnBuscarBitacora.Tag = "FormAuditLog.SearchButton";
            this.btnBuscarBitacora.TabIndex = 11;
            this.btnBuscarBitacora.Click += new System.EventHandler(this.btnBuscarBitacora_Click);
            // 
            // dgvBitacora
            // 
            this.dgvBitacora.AllowUserToAddRows = false;
            this.dgvBitacora.AllowUserToDeleteRows = false;
            this.dgvBitacora.AutoGenerateColumns = false;
            this.dgvBitacora.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBitacora.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBitacoraId,
            this.colBitacoraDateTime,
            this.colBitacoraUserId,
            this.colBitacoraUsernameSnapshot,
            this.colBitacoraActivity,
            this.colBitacoraInformation,
            this.colBitacoraLevel,
            this.colBitacoraSource,
            this.colBitacoraIsTechnical});
            this.dgvBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBitacora.Location = new System.Drawing.Point(18, 175);
            this.dgvBitacora.MultiSelect = false;
            this.dgvBitacora.Name = "dgvBitacora";
            this.dgvBitacora.ReadOnly = true;
            this.dgvBitacora.RowHeadersVisible = false;
            this.dgvBitacora.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBitacora.Size = new System.Drawing.Size(944, 360);
            this.dgvBitacora.Tag = "FormAuditLog.Grid";
            this.dgvBitacora.TabIndex = 2;
            //
            // colBitacoraId
            //
            this.colBitacoraId.DataPropertyName = "Id";
            this.colBitacoraId.HeaderText = "Id";
            this.colBitacoraId.Name = "Id";
            this.colBitacoraId.ReadOnly = true;
            //
            // colBitacoraDateTime
            //
            this.colBitacoraDateTime.DataPropertyName = "FechaHora";
            this.colBitacoraDateTime.HeaderText = "Fecha y hora";
            this.colBitacoraDateTime.Name = "DateTime";
            this.colBitacoraDateTime.ReadOnly = true;
            //
            // colBitacoraUserId
            //
            this.colBitacoraUserId.DataPropertyName = "UsuarioId";
            this.colBitacoraUserId.HeaderText = "Id usuario";
            this.colBitacoraUserId.Name = "UserId";
            this.colBitacoraUserId.ReadOnly = true;
            //
            // colBitacoraUsernameSnapshot
            //
            this.colBitacoraUsernameSnapshot.DataPropertyName = "UsernameSnapshot";
            this.colBitacoraUsernameSnapshot.HeaderText = "Usuario";
            this.colBitacoraUsernameSnapshot.Name = "UsernameSnapshot";
            this.colBitacoraUsernameSnapshot.ReadOnly = true;
            //
            // colBitacoraActivity
            //
            this.colBitacoraActivity.DataPropertyName = "Actividad";
            this.colBitacoraActivity.HeaderText = "Actividad";
            this.colBitacoraActivity.Name = "Activity";
            this.colBitacoraActivity.ReadOnly = true;
            //
            // colBitacoraInformation
            //
            this.colBitacoraInformation.DataPropertyName = "Informacion";
            this.colBitacoraInformation.HeaderText = "Informacion";
            this.colBitacoraInformation.Name = "Information";
            this.colBitacoraInformation.ReadOnly = true;
            //
            // colBitacoraLevel
            //
            this.colBitacoraLevel.DataPropertyName = "Nivel";
            this.colBitacoraLevel.HeaderText = "Nivel";
            this.colBitacoraLevel.Name = "Level";
            this.colBitacoraLevel.ReadOnly = true;
            //
            // colBitacoraSource
            //
            this.colBitacoraSource.DataPropertyName = "Origen";
            this.colBitacoraSource.HeaderText = "Origen";
            this.colBitacoraSource.Name = "Source";
            this.colBitacoraSource.ReadOnly = true;
            //
            // colBitacoraIsTechnical
            //
            this.colBitacoraIsTechnical.DataPropertyName = "EsTecnica";
            this.colBitacoraIsTechnical.HeaderText = "Tecnica";
            this.colBitacoraIsTechnical.Name = "IsTechnical";
            this.colBitacoraIsTechnical.ReadOnly = true;
            // 
            // tlpFechasBitacora
            // 
            this.tlpFechasBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpFechasBitacora.Controls.Add(this.chkbFechaDesdeBitacora, 0, 0);
            this.tlpFechasBitacora.Controls.Add(this.dtpFechaDesdeBitacora, 1, 0);
            this.tlpFechasBitacora.Controls.Add(this.chkbFechaHastaBitacora, 2, 0);
            this.tlpFechasBitacora.Controls.Add(this.dtpFechaHastaBitacora, 3, 0);
            this.tlpFechasBitacora.Location = new System.Drawing.Point(15, 10);
            this.tlpFechasBitacora.Name = "tlpFechasBitacora";
            this.tlpFechasBitacora.Size = new System.Drawing.Size(625, 26);
            this.tlpFechasBitacora.ColumnCount = 4;
            this.tlpFechasBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tlpFechasBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.16F));
            this.tlpFechasBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tlpFechasBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.84F));
            this.tlpFechasBitacora.RowCount = 1;
            this.tlpFechasBitacora.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tlpFechasBitacora.TabIndex = 0;
            // 
            // tlpCamposBitacora
            // 
            this.tlpCamposBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposBitacora.Controls.Add(this.lblUsuarioBitacora, 0, 0);
            this.tlpCamposBitacora.Controls.Add(this.txtbUsuarioBitacora, 0, 1);
            this.tlpCamposBitacora.Controls.Add(this.lblActividadBitacora, 1, 0);
            this.tlpCamposBitacora.Controls.Add(this.txtbActividadBitacora, 1, 1);
            this.tlpCamposBitacora.Controls.Add(this.lblNivelBitacora, 2, 0);
            this.tlpCamposBitacora.Controls.Add(this.cmbNivelBitacora, 2, 1);
            this.tlpCamposBitacora.Controls.Add(this.chkbSoloTecnicaBitacora, 3, 1);
            this.tlpCamposBitacora.Location = new System.Drawing.Point(15, 43);
            this.tlpCamposBitacora.Name = "tlpCamposBitacora";
            this.tlpCamposBitacora.Size = new System.Drawing.Size(775, 45);
            this.tlpCamposBitacora.ColumnCount = 4;
            this.tlpCamposBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.28F));
            this.tlpCamposBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.58F));
            this.tlpCamposBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.13F));
            this.tlpCamposBitacora.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlpCamposBitacora.RowCount = 2;
            this.tlpCamposBitacora.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposBitacora.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposBitacora.TabIndex = 4;
            //
            // FormBitacora
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 555);
            this.Controls.Add(this.dgvBitacora);
            this.Controls.Add(this.groupBoxFiltrosBitacora);
            this.Controls.Add(this.lblTituloBitacora);
            this.Name = "FormBitacora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormAuditLog.Title";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBitacora.Panel)).EndInit();
            this.groupBoxFiltrosBitacora.Panel.ResumeLayout(false);
            this.groupBoxFiltrosBitacora.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltrosBitacora)).EndInit();
            this.groupBoxFiltrosBitacora.ResumeLayout(false);
            this.groupBoxFiltrosBitacora.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).EndInit();
            this.tlpFechasBitacora.ResumeLayout(false);
            this.tlpFechasBitacora.PerformLayout();
            this.tlpCamposBitacora.ResumeLayout(false);
            this.tlpCamposBitacora.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblTituloBitacora;
        private Krypton.Toolkit.KryptonGroupBox groupBoxFiltrosBitacora;
        private TpIDS.CheckBoxAccesible chkbFechaDesdeBitacora;
        private TpIDS.DateTimePickerAccesible dtpFechaDesdeBitacora;
        private TpIDS.CheckBoxAccesible chkbFechaHastaBitacora;
        private TpIDS.DateTimePickerAccesible dtpFechaHastaBitacora;
        private Krypton.Toolkit.KryptonLabel lblUsuarioBitacora;
        private Krypton.Toolkit.KryptonTextBox txtbUsuarioBitacora;
        private Krypton.Toolkit.KryptonLabel lblActividadBitacora;
        private Krypton.Toolkit.KryptonTextBox txtbActividadBitacora;
        private Krypton.Toolkit.KryptonLabel lblNivelBitacora;
        private Krypton.Toolkit.KryptonComboBox cmbNivelBitacora;
        private TpIDS.CheckBoxAccesible chkbSoloTecnicaBitacora;
        private Krypton.Toolkit.KryptonButton btnBuscarBitacora;
        private Krypton.Toolkit.KryptonDataGridView dgvBitacora;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraUsernameSnapshot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraActivity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraInformation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraSource;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colBitacoraIsTechnical;
        private System.Windows.Forms.TableLayoutPanel tlpFechasBitacora;
        private System.Windows.Forms.TableLayoutPanel tlpCamposBitacora;
    }
}
