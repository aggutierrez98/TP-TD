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
            this.lblTituloBitacora = new System.Windows.Forms.Label();
            this.groupBoxFiltrosBitacora = new System.Windows.Forms.GroupBox();
            this.chkbFechaDesdeBitacora = new System.Windows.Forms.CheckBox();
            this.dtpFechaDesdeBitacora = new System.Windows.Forms.DateTimePicker();
            this.chkbFechaHastaBitacora = new System.Windows.Forms.CheckBox();
            this.dtpFechaHastaBitacora = new System.Windows.Forms.DateTimePicker();
            this.lblUsuarioBitacora = new System.Windows.Forms.Label();
            this.txtbUsuarioBitacora = new System.Windows.Forms.TextBox();
            this.lblActividadBitacora = new System.Windows.Forms.Label();
            this.txtbActividadBitacora = new System.Windows.Forms.TextBox();
            this.lblNivelBitacora = new System.Windows.Forms.Label();
            this.cmbNivelBitacora = new System.Windows.Forms.ComboBox();
            this.chkbSoloTecnicaBitacora = new System.Windows.Forms.CheckBox();
            this.btnBuscarBitacora = new System.Windows.Forms.Button();
            this.dgvBitacora = new System.Windows.Forms.DataGridView();
            this.colBitacoraId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraFechaHora = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraUsuarioId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraUsernameSnapshot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraActividad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraInformacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraNivel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraOrigen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitacoraEsTecnica = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBoxFiltrosBitacora.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTituloBitacora
            // 
            this.lblTituloBitacora.AutoSize = true;
            this.lblTituloBitacora.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloBitacora.Location = new System.Drawing.Point(15, 15);
            this.lblTituloBitacora.Name = "lblTituloBitacora";
            this.lblTituloBitacora.Size = new System.Drawing.Size(0, 25);
            this.lblTituloBitacora.Tag = "FormBitacora.TitleLabel";
            this.lblTituloBitacora.TabIndex = 0;
            // 
            // groupBoxFiltrosBitacora
            // 
            this.groupBoxFiltrosBitacora.Controls.Add(this.chkbFechaDesdeBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.dtpFechaDesdeBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.chkbFechaHastaBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.dtpFechaHastaBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.lblUsuarioBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.txtbUsuarioBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.lblActividadBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.txtbActividadBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.lblNivelBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.cmbNivelBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.chkbSoloTecnicaBitacora);
            this.groupBoxFiltrosBitacora.Controls.Add(this.btnBuscarBitacora);
            this.groupBoxFiltrosBitacora.Location = new System.Drawing.Point(18, 50);
            this.groupBoxFiltrosBitacora.Name = "groupBoxFiltrosBitacora";
            this.groupBoxFiltrosBitacora.Size = new System.Drawing.Size(944, 110);
            this.groupBoxFiltrosBitacora.Tag = "FormBitacora.FiltersGroup";
            this.groupBoxFiltrosBitacora.TabIndex = 1;
            this.groupBoxFiltrosBitacora.TabStop = false;
            // 
            // chkbFechaDesdeBitacora
            // 
            this.chkbFechaDesdeBitacora.Location = new System.Drawing.Point(15, 28);
            this.chkbFechaDesdeBitacora.Name = "chkbFechaDesdeBitacora";
            this.chkbFechaDesdeBitacora.Size = new System.Drawing.Size(110, 20);
            this.chkbFechaDesdeBitacora.Tag = "FormBitacora.FromCheck";
            this.chkbFechaDesdeBitacora.TabIndex = 0;
            this.chkbFechaDesdeBitacora.UseVisualStyleBackColor = true;
            // 
            // dtpFechaDesdeBitacora
            // 
            this.dtpFechaDesdeBitacora.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFechaDesdeBitacora.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaDesdeBitacora.Location = new System.Drawing.Point(130, 26);
            this.dtpFechaDesdeBitacora.Name = "dtpFechaDesdeBitacora";
            this.dtpFechaDesdeBitacora.Size = new System.Drawing.Size(185, 20);
            this.dtpFechaDesdeBitacora.TabIndex = 1;
            // 
            // chkbFechaHastaBitacora
            // 
            this.chkbFechaHastaBitacora.Location = new System.Drawing.Point(340, 28);
            this.chkbFechaHastaBitacora.Name = "chkbFechaHastaBitacora";
            this.chkbFechaHastaBitacora.Size = new System.Drawing.Size(110, 20);
            this.chkbFechaHastaBitacora.Tag = "FormBitacora.ToCheck";
            this.chkbFechaHastaBitacora.TabIndex = 2;
            this.chkbFechaHastaBitacora.UseVisualStyleBackColor = true;
            // 
            // dtpFechaHastaBitacora
            // 
            this.dtpFechaHastaBitacora.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFechaHastaBitacora.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaHastaBitacora.Location = new System.Drawing.Point(455, 26);
            this.dtpFechaHastaBitacora.Name = "dtpFechaHastaBitacora";
            this.dtpFechaHastaBitacora.Size = new System.Drawing.Size(185, 20);
            this.dtpFechaHastaBitacora.TabIndex = 3;
            // 
            // lblUsuarioBitacora
            // 
            this.lblUsuarioBitacora.AutoSize = true;
            this.lblUsuarioBitacora.Location = new System.Drawing.Point(15, 63);
            this.lblUsuarioBitacora.Name = "lblUsuarioBitacora";
            this.lblUsuarioBitacora.Size = new System.Drawing.Size(0, 13);
            this.lblUsuarioBitacora.Tag = "FormBitacora.UserLabel";
            this.lblUsuarioBitacora.TabIndex = 4;
            // 
            // txtbUsuarioBitacora
            // 
            this.txtbUsuarioBitacora.Location = new System.Drawing.Point(18, 79);
            this.txtbUsuarioBitacora.Name = "txtbUsuarioBitacora";
            this.txtbUsuarioBitacora.Size = new System.Drawing.Size(180, 20);
            this.txtbUsuarioBitacora.TabIndex = 5;
            // 
            // lblActividadBitacora
            // 
            this.lblActividadBitacora.AutoSize = true;
            this.lblActividadBitacora.Location = new System.Drawing.Point(220, 63);
            this.lblActividadBitacora.Name = "lblActividadBitacora";
            this.lblActividadBitacora.Size = new System.Drawing.Size(0, 13);
            this.lblActividadBitacora.Tag = "FormBitacora.ActivityLabel";
            this.lblActividadBitacora.TabIndex = 6;
            // 
            // txtbActividadBitacora
            // 
            this.txtbActividadBitacora.Location = new System.Drawing.Point(223, 79);
            this.txtbActividadBitacora.Name = "txtbActividadBitacora";
            this.txtbActividadBitacora.Size = new System.Drawing.Size(220, 20);
            this.txtbActividadBitacora.TabIndex = 7;
            // 
            // lblNivelBitacora
            // 
            this.lblNivelBitacora.AutoSize = true;
            this.lblNivelBitacora.Location = new System.Drawing.Point(465, 63);
            this.lblNivelBitacora.Name = "lblNivelBitacora";
            this.lblNivelBitacora.Size = new System.Drawing.Size(0, 13);
            this.lblNivelBitacora.Tag = "FormBitacora.LevelLabel";
            this.lblNivelBitacora.TabIndex = 8;
            // 
            // cmbNivelBitacora
            // 
            this.cmbNivelBitacora.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNivelBitacora.FormattingEnabled = true;
            this.cmbNivelBitacora.Location = new System.Drawing.Point(468, 79);
            this.cmbNivelBitacora.Name = "cmbNivelBitacora";
            this.cmbNivelBitacora.Size = new System.Drawing.Size(160, 21);
            this.cmbNivelBitacora.TabIndex = 9;
            // 
            // chkbSoloTecnicaBitacora
            // 
            this.chkbSoloTecnicaBitacora.Location = new System.Drawing.Point(650, 79);
            this.chkbSoloTecnicaBitacora.Name = "chkbSoloTecnicaBitacora";
            this.chkbSoloTecnicaBitacora.Size = new System.Drawing.Size(140, 20);
            this.chkbSoloTecnicaBitacora.Tag = "FormBitacora.OnlyTechnicalCheck";
            this.chkbSoloTecnicaBitacora.TabIndex = 10;
            this.chkbSoloTecnicaBitacora.UseVisualStyleBackColor = true;
            // 
            // btnBuscarBitacora
            // 
            this.btnBuscarBitacora.Location = new System.Drawing.Point(800, 72);
            this.btnBuscarBitacora.Name = "btnBuscarBitacora";
            this.btnBuscarBitacora.Size = new System.Drawing.Size(125, 30);
            this.btnBuscarBitacora.Tag = "FormBitacora.SearchButton";
            this.btnBuscarBitacora.TabIndex = 11;
            this.btnBuscarBitacora.UseVisualStyleBackColor = true;
            this.btnBuscarBitacora.Click += new System.EventHandler(this.btnBuscarBitacora_Click);
            // 
            // dgvBitacora
            // 
            this.dgvBitacora.AllowUserToAddRows = false;
            this.dgvBitacora.AllowUserToDeleteRows = false;
            this.dgvBitacora.AutoGenerateColumns = false;
            this.dgvBitacora.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBitacora.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBitacoraId,
            this.colBitacoraFechaHora,
            this.colBitacoraUsuarioId,
            this.colBitacoraUsernameSnapshot,
            this.colBitacoraActividad,
            this.colBitacoraInformacion,
            this.colBitacoraNivel,
            this.colBitacoraOrigen,
            this.colBitacoraEsTecnica});
            this.dgvBitacora.Location = new System.Drawing.Point(18, 175);
            this.dgvBitacora.MultiSelect = false;
            this.dgvBitacora.Name = "dgvBitacora";
            this.dgvBitacora.ReadOnly = true;
            this.dgvBitacora.RowHeadersVisible = false;
            this.dgvBitacora.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBitacora.Size = new System.Drawing.Size(944, 360);
            this.dgvBitacora.Tag = "FormBitacora.Grid";
            this.dgvBitacora.TabIndex = 2;
            // 
            // colBitacoraId
            // 
            this.colBitacoraId.DataPropertyName = "Id";
            this.colBitacoraId.FillWeight = 60F;
            this.colBitacoraId.HeaderText = "FormBitacora.Grid.Id";
            this.colBitacoraId.Name = "Id";
            this.colBitacoraId.ReadOnly = true;
            this.colBitacoraId.Tag = "FormBitacora.Grid.Id";
            this.colBitacoraId.Width = 60;
            // 
            // colBitacoraFechaHora
            // 
            this.colBitacoraFechaHora.DataPropertyName = "FechaHora";
            this.colBitacoraFechaHora.HeaderText = "FormBitacora.Grid.FechaHora";
            this.colBitacoraFechaHora.Name = "FechaHora";
            this.colBitacoraFechaHora.ReadOnly = true;
            this.colBitacoraFechaHora.Tag = "FormBitacora.Grid.FechaHora";
            // 
            // colBitacoraUsuarioId
            // 
            this.colBitacoraUsuarioId.DataPropertyName = "UsuarioId";
            this.colBitacoraUsuarioId.HeaderText = "FormBitacora.Grid.UsuarioId";
            this.colBitacoraUsuarioId.Name = "UsuarioId";
            this.colBitacoraUsuarioId.ReadOnly = true;
            this.colBitacoraUsuarioId.Tag = "FormBitacora.Grid.UsuarioId";
            // 
            // colBitacoraUsernameSnapshot
            // 
            this.colBitacoraUsernameSnapshot.DataPropertyName = "UsernameSnapshot";
            this.colBitacoraUsernameSnapshot.HeaderText = "FormBitacora.Grid.UsernameSnapshot";
            this.colBitacoraUsernameSnapshot.Name = "UsernameSnapshot";
            this.colBitacoraUsernameSnapshot.ReadOnly = true;
            this.colBitacoraUsernameSnapshot.Tag = "FormBitacora.Grid.UsernameSnapshot";
            // 
            // colBitacoraActividad
            // 
            this.colBitacoraActividad.DataPropertyName = "Actividad";
            this.colBitacoraActividad.HeaderText = "FormBitacora.Grid.Actividad";
            this.colBitacoraActividad.Name = "Actividad";
            this.colBitacoraActividad.ReadOnly = true;
            this.colBitacoraActividad.Tag = "FormBitacora.Grid.Actividad";
            // 
            // colBitacoraInformacion
            // 
            this.colBitacoraInformacion.DataPropertyName = "Informacion";
            this.colBitacoraInformacion.FillWeight = 180F;
            this.colBitacoraInformacion.HeaderText = "FormBitacora.Grid.Informacion";
            this.colBitacoraInformacion.Name = "Informacion";
            this.colBitacoraInformacion.ReadOnly = true;
            this.colBitacoraInformacion.Tag = "FormBitacora.Grid.Informacion";
            // 
            // colBitacoraNivel
            // 
            this.colBitacoraNivel.DataPropertyName = "Nivel";
            this.colBitacoraNivel.HeaderText = "FormBitacora.Grid.Nivel";
            this.colBitacoraNivel.Name = "Nivel";
            this.colBitacoraNivel.ReadOnly = true;
            this.colBitacoraNivel.Tag = "FormBitacora.Grid.Nivel";
            // 
            // colBitacoraOrigen
            // 
            this.colBitacoraOrigen.DataPropertyName = "Origen";
            this.colBitacoraOrigen.HeaderText = "FormBitacora.Grid.Origen";
            this.colBitacoraOrigen.Name = "Origen";
            this.colBitacoraOrigen.ReadOnly = true;
            this.colBitacoraOrigen.Tag = "FormBitacora.Grid.Origen";
            // 
            // colBitacoraEsTecnica
            // 
            this.colBitacoraEsTecnica.DataPropertyName = "EsTecnica";
            this.colBitacoraEsTecnica.HeaderText = "FormBitacora.Grid.EsTecnica";
            this.colBitacoraEsTecnica.Name = "EsTecnica";
            this.colBitacoraEsTecnica.ReadOnly = true;
            this.colBitacoraEsTecnica.Tag = "FormBitacora.Grid.EsTecnica";
            //
            // FormBitacora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 555);
            this.Controls.Add(this.dgvBitacora);
            this.Controls.Add(this.groupBoxFiltrosBitacora);
            this.Controls.Add(this.lblTituloBitacora);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormBitacora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormBitacora.Title";
            this.groupBoxFiltrosBitacora.ResumeLayout(false);
            this.groupBoxFiltrosBitacora.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloBitacora;
        private System.Windows.Forms.GroupBox groupBoxFiltrosBitacora;
        private System.Windows.Forms.CheckBox chkbFechaDesdeBitacora;
        private System.Windows.Forms.DateTimePicker dtpFechaDesdeBitacora;
        private System.Windows.Forms.CheckBox chkbFechaHastaBitacora;
        private System.Windows.Forms.DateTimePicker dtpFechaHastaBitacora;
        private System.Windows.Forms.Label lblUsuarioBitacora;
        private System.Windows.Forms.TextBox txtbUsuarioBitacora;
        private System.Windows.Forms.Label lblActividadBitacora;
        private System.Windows.Forms.TextBox txtbActividadBitacora;
        private System.Windows.Forms.Label lblNivelBitacora;
        private System.Windows.Forms.ComboBox cmbNivelBitacora;
        private System.Windows.Forms.CheckBox chkbSoloTecnicaBitacora;
        private System.Windows.Forms.Button btnBuscarBitacora;
        private System.Windows.Forms.DataGridView dgvBitacora;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraFechaHora;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraUsuarioId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraUsernameSnapshot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraActividad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraInformacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraNivel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitacoraOrigen;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colBitacoraEsTecnica;
    }
}
