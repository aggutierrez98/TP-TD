namespace TpIDS
{
    partial class FormMisZonas
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
            this.components = new System.ComponentModel.Container();
            this.lblTituloMisZonas = new Krypton.Toolkit.KryptonLabel();
            this.dgvZonasUsuario = new Krypton.Toolkit.KryptonDataGridView();
            this.colZonaNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZonaRadio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnQuitarZona = new Krypton.Toolkit.KryptonButton();
            this.groupBoxNuevaZona = new Krypton.Toolkit.KryptonGroupBox();
            this.lblNombreZona = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreZona = new Krypton.Toolkit.KryptonTextBox();
            this.lblBusquedaZona = new Krypton.Toolkit.KryptonLabel();
            this.txtbBusquedaZona = new Krypton.Toolkit.KryptonTextBox();
            this.btnBuscarZona = new Krypton.Toolkit.KryptonButton();
            this.cmbResultadosZona = new Krypton.Toolkit.KryptonComboBox();
            this.lblEstadoBusquedaZona = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblRadioZona = new Krypton.Toolkit.KryptonLabel();
            this.nudRadioZona = new System.Windows.Forms.NumericUpDown();
            this.lblPuntoZona = new Krypton.Toolkit.KryptonLabel();
            this.lblCoordenadasZona = new Krypton.Toolkit.KryptonLabel();
            this.btnAgregarZona = new Krypton.Toolkit.KryptonButton();
            this.lblInstruccionesMapaZonas = new Krypton.Toolkit.KryptonLabel();
            this.btnMapaAcercar = new Krypton.Toolkit.KryptonButton();
            this.btnMapaAlejar = new Krypton.Toolkit.KryptonButton();
            this.mapZonasUsuario = new GMap.NET.WindowsForms.GMapControl();
            this.lblAtribucionOsmZonas = new Krypton.Toolkit.KryptonLinkLabel();
            this.toolTipMapaZonas = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvZonasUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNuevaZona)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNuevaZona.Panel)).BeginInit();
            this.groupBoxNuevaZona.Panel.SuspendLayout();
            this.groupBoxNuevaZona.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbResultadosZona)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadioZona)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloMisZonas
            //
            this.lblTituloMisZonas.AutoSize = true;
            this.lblTituloMisZonas.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMisZonas.Location = new System.Drawing.Point(12, 10);
            this.lblTituloMisZonas.Name = "lblTituloMisZonas";
            this.lblTituloMisZonas.Size = new System.Drawing.Size(120, 25);
            this.lblTituloMisZonas.TabIndex = 0;
            this.lblTituloMisZonas.Tag = "FormMyZones.TitleLabel";
            this.lblTituloMisZonas.Values.Text = "Mis zonas";
            //
            // dgvZonasUsuario
            //
            this.dgvZonasUsuario.AllowUserToAddRows = false;
            this.dgvZonasUsuario.AllowUserToDeleteRows = false;
            this.dgvZonasUsuario.AutoGenerateColumns = false;
            this.dgvZonasUsuario.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvZonasUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvZonasUsuario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvZonasUsuario.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colZonaNombre,
            this.colZonaRadio});
            this.dgvZonasUsuario.Location = new System.Drawing.Point(12, 44);
            this.dgvZonasUsuario.MultiSelect = false;
            this.dgvZonasUsuario.Name = "dgvZonasUsuario";
            this.dgvZonasUsuario.ReadOnly = true;
            this.dgvZonasUsuario.RowHeadersVisible = false;
            this.dgvZonasUsuario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvZonasUsuario.Size = new System.Drawing.Size(360, 200);
            this.dgvZonasUsuario.TabIndex = 1;
            this.dgvZonasUsuario.Tag = "FormMyZones.ZoneGrid";
            this.dgvZonasUsuario.SelectionChanged += new System.EventHandler(this.dgvZonasUsuario_SelectionChanged);
            //
            // colZonaNombre
            //
            this.colZonaNombre.DataPropertyName = "Nombre";
            this.colZonaNombre.FillWeight = 200F;
            this.colZonaNombre.HeaderText = "Nombre";
            this.colZonaNombre.Name = "Nombre";
            this.colZonaNombre.ReadOnly = true;
            //
            // colZonaRadio
            //
            this.colZonaRadio.DataPropertyName = "RadioMetros";
            this.colZonaRadio.HeaderText = "Radio (m)";
            this.colZonaRadio.Name = "RadioMetros";
            this.colZonaRadio.ReadOnly = true;
            //
            // btnQuitarZona
            //
            this.btnQuitarZona.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuitarZona.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnQuitarZona.Location = new System.Drawing.Point(12, 252);
            this.btnQuitarZona.Name = "btnQuitarZona";
            this.btnQuitarZona.Size = new System.Drawing.Size(160, 30);
            this.btnQuitarZona.TabIndex = 2;
            this.btnQuitarZona.Tag = "FormMyZones.RemoveButton";
            this.btnQuitarZona.Values.Text = "Quitar seleccionada";
            this.btnQuitarZona.Click += new System.EventHandler(this.btnQuitarZona_Click);
            //
            // groupBoxNuevaZona
            //
            this.groupBoxNuevaZona.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxNuevaZona.Location = new System.Drawing.Point(12, 290);
            this.groupBoxNuevaZona.Name = "groupBoxNuevaZona";
            this.groupBoxNuevaZona.Panel.Controls.Add(this.lblNombreZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.txtbNombreZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.lblBusquedaZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.txtbBusquedaZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.btnBuscarZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.cmbResultadosZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.lblEstadoBusquedaZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.lblRadioZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.nudRadioZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.lblPuntoZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.lblCoordenadasZona);
            this.groupBoxNuevaZona.Panel.Controls.Add(this.btnAgregarZona);
            this.groupBoxNuevaZona.Size = new System.Drawing.Size(360, 298);
            this.groupBoxNuevaZona.TabIndex = 3;
            this.groupBoxNuevaZona.TabStop = false;
            this.groupBoxNuevaZona.Tag = "FormMyZones.NewZoneGroup";
            this.groupBoxNuevaZona.Values.Heading = "Nueva zona";
            //
            // lblNombreZona
            //
            this.lblNombreZona.AutoSize = false;
            this.lblNombreZona.Location = new System.Drawing.Point(8, 8);
            this.lblNombreZona.Name = "lblNombreZona";
            this.lblNombreZona.Size = new System.Drawing.Size(88, 22);
            this.lblNombreZona.TabIndex = 0;
            this.lblNombreZona.Tag = "FormMyZones.NameLabel";
            this.lblNombreZona.Values.Text = "Nombre";
            //
            // txtbNombreZona
            //
            this.txtbNombreZona.Location = new System.Drawing.Point(100, 8);
            this.txtbNombreZona.MaxLength = 50;
            this.txtbNombreZona.Name = "txtbNombreZona";
            this.txtbNombreZona.Size = new System.Drawing.Size(242, 22);
            this.txtbNombreZona.TabIndex = 1;
            //
            // lblBusquedaZona
            //
            this.lblBusquedaZona.AutoSize = false;
            this.lblBusquedaZona.Location = new System.Drawing.Point(8, 40);
            this.lblBusquedaZona.Name = "lblBusquedaZona";
            this.lblBusquedaZona.Size = new System.Drawing.Size(88, 22);
            this.lblBusquedaZona.TabIndex = 2;
            this.lblBusquedaZona.Tag = "FormMyZones.SearchLabel";
            this.lblBusquedaZona.Values.Text = "Dirección";
            //
            // txtbBusquedaZona
            //
            this.txtbBusquedaZona.Location = new System.Drawing.Point(100, 40);
            this.txtbBusquedaZona.MaxLength = 200;
            this.txtbBusquedaZona.Name = "txtbBusquedaZona";
            this.txtbBusquedaZona.Size = new System.Drawing.Size(162, 22);
            this.txtbBusquedaZona.TabIndex = 3;
            this.txtbBusquedaZona.TextChanged += new System.EventHandler(this.txtbBusquedaZona_TextChanged);
            this.txtbBusquedaZona.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbBusquedaZona_KeyDown);
            //
            // btnBuscarZona
            //
            this.btnBuscarZona.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnBuscarZona.Location = new System.Drawing.Point(268, 39);
            this.btnBuscarZona.Name = "btnBuscarZona";
            this.btnBuscarZona.Size = new System.Drawing.Size(74, 24);
            this.btnBuscarZona.TabIndex = 4;
            this.btnBuscarZona.Tag = "FormMapLocationPicker.SearchButton";
            this.btnBuscarZona.Values.Text = "Buscar";
            this.btnBuscarZona.Click += new System.EventHandler(this.btnBuscarZona_Click);
            //
            // cmbResultadosZona
            //
            this.cmbResultadosZona.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResultadosZona.Enabled = false;
            this.cmbResultadosZona.FormattingEnabled = true;
            this.cmbResultadosZona.Location = new System.Drawing.Point(8, 72);
            this.cmbResultadosZona.Name = "cmbResultadosZona";
            this.cmbResultadosZona.Size = new System.Drawing.Size(334, 24);
            this.cmbResultadosZona.TabIndex = 5;
            this.cmbResultadosZona.DropDown += new System.EventHandler(this.cmbResultadosZona_DropDown);
            this.cmbResultadosZona.SelectionChangeCommitted += new System.EventHandler(this.cmbResultadosZona_SelectionChangeCommitted);
            //
            // lblEstadoBusquedaZona
            //
            this.lblEstadoBusquedaZona.AutoSize = false;
            this.lblEstadoBusquedaZona.Location = new System.Drawing.Point(10, 102);
            this.lblEstadoBusquedaZona.Name = "lblEstadoBusquedaZona";
            this.lblEstadoBusquedaZona.Size = new System.Drawing.Size(332, 46);
            this.lblEstadoBusquedaZona.TabIndex = 6;
            //
            // lblRadioZona
            //
            this.lblRadioZona.AutoSize = false;
            this.lblRadioZona.Location = new System.Drawing.Point(8, 156);
            this.lblRadioZona.Name = "lblRadioZona";
            this.lblRadioZona.Size = new System.Drawing.Size(88, 22);
            this.lblRadioZona.TabIndex = 7;
            this.lblRadioZona.Tag = "FormMyZones.RadiusLabel";
            this.lblRadioZona.Values.Text = "Radio (m)";
            //
            // nudRadioZona
            //
            this.nudRadioZona.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudRadioZona.Location = new System.Drawing.Point(100, 156);
            this.nudRadioZona.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudRadioZona.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudRadioZona.Name = "nudRadioZona";
            this.nudRadioZona.Size = new System.Drawing.Size(100, 22);
            this.nudRadioZona.TabIndex = 8;
            this.nudRadioZona.ThousandsSeparator = true;
            this.nudRadioZona.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRadioZona.ValueChanged += new System.EventHandler(this.nudRadioZona_ValueChanged);
            //
            // lblPuntoZona
            //
            this.lblPuntoZona.AutoSize = false;
            this.lblPuntoZona.Location = new System.Drawing.Point(8, 188);
            this.lblPuntoZona.Name = "lblPuntoZona";
            this.lblPuntoZona.Size = new System.Drawing.Size(88, 22);
            this.lblPuntoZona.TabIndex = 9;
            this.lblPuntoZona.Tag = "FormMyZones.PointLabel";
            this.lblPuntoZona.Values.Text = "Punto";
            //
            // lblCoordenadasZona
            //
            this.lblCoordenadasZona.AutoSize = false;
            this.lblCoordenadasZona.Location = new System.Drawing.Point(100, 188);
            this.lblCoordenadasZona.Name = "lblCoordenadasZona";
            this.lblCoordenadasZona.Size = new System.Drawing.Size(242, 22);
            this.lblCoordenadasZona.TabIndex = 10;
            this.lblCoordenadasZona.Values.Text = "Sin elegir";
            //
            // btnAgregarZona
            //
            this.btnAgregarZona.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarZona.Location = new System.Drawing.Point(8, 224);
            this.btnAgregarZona.Name = "btnAgregarZona";
            this.btnAgregarZona.Size = new System.Drawing.Size(120, 30);
            this.btnAgregarZona.TabIndex = 11;
            this.btnAgregarZona.Tag = "FormMyZones.AddButton";
            this.btnAgregarZona.Values.Text = "Agregar";
            this.btnAgregarZona.Click += new System.EventHandler(this.btnAgregarZona_Click);
            //
            // lblInstruccionesMapaZonas
            //
            this.lblInstruccionesMapaZonas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstruccionesMapaZonas.AutoSize = false;
            this.lblInstruccionesMapaZonas.Location = new System.Drawing.Point(384, 10);
            this.lblInstruccionesMapaZonas.Name = "lblInstruccionesMapaZonas";
            this.lblInstruccionesMapaZonas.Size = new System.Drawing.Size(420, 40);
            this.lblInstruccionesMapaZonas.TabIndex = 4;
            this.lblInstruccionesMapaZonas.Tag = "FormMyZones.HintLabel";
            this.lblInstruccionesMapaZonas.Values.Text = "Haga clic izquierdo en el mapa para elegir el punto de la zona.\r\nArrastre con el botón derecho para mover el mapa y use la rueda para acercar o alejar.";
            //
            // btnMapaAcercar
            //
            this.btnMapaAcercar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaAcercar.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaAcercar.Location = new System.Drawing.Point(812, 12);
            this.btnMapaAcercar.Name = "btnMapaAcercar";
            this.btnMapaAcercar.Size = new System.Drawing.Size(36, 36);
            this.btnMapaAcercar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.btnMapaAcercar.TabIndex = 5;
            this.btnMapaAcercar.Values.Text = "+";
            this.btnMapaAcercar.Click += new System.EventHandler(this.btnMapaAcercar_Click);
            //
            // btnMapaAlejar
            //
            this.btnMapaAlejar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaAlejar.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaAlejar.Location = new System.Drawing.Point(852, 12);
            this.btnMapaAlejar.Name = "btnMapaAlejar";
            this.btnMapaAlejar.Size = new System.Drawing.Size(36, 36);
            this.btnMapaAlejar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.btnMapaAlejar.TabIndex = 6;
            this.btnMapaAlejar.Values.Text = "−";
            this.btnMapaAlejar.Click += new System.EventHandler(this.btnMapaAlejar_Click);
            //
            // mapZonasUsuario
            //
            this.mapZonasUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.mapZonasUsuario.Bearing = 0F;
            this.mapZonasUsuario.CanDragMap = true;
            this.mapZonasUsuario.EmptyTileColor = System.Drawing.Color.Navy;
            this.mapZonasUsuario.GrayScaleMode = false;
            this.mapZonasUsuario.LevelsKeepInMemory = 5;
            this.mapZonasUsuario.Location = new System.Drawing.Point(384, 56);
            this.mapZonasUsuario.MarkersEnabled = true;
            this.mapZonasUsuario.MaxZoom = 18;
            this.mapZonasUsuario.MinZoom = 2;
            this.mapZonasUsuario.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.mapZonasUsuario.Name = "mapZonasUsuario";
            this.mapZonasUsuario.NegativeMode = false;
            this.mapZonasUsuario.PolygonsEnabled = true;
            this.mapZonasUsuario.RetryLoadTile = 0;
            this.mapZonasUsuario.RoutesEnabled = true;
            this.mapZonasUsuario.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.mapZonasUsuario.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.mapZonasUsuario.ShowTileGridLines = false;
            this.mapZonasUsuario.Size = new System.Drawing.Size(504, 506);
            this.mapZonasUsuario.TabIndex = 7;
            this.mapZonasUsuario.Zoom = 0D;
            //
            // lblAtribucionOsmZonas
            //
            this.lblAtribucionOsmZonas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAtribucionOsmZonas.AutoSize = false;
            this.lblAtribucionOsmZonas.Location = new System.Drawing.Point(384, 568);
            this.lblAtribucionOsmZonas.Name = "lblAtribucionOsmZonas";
            this.lblAtribucionOsmZonas.Size = new System.Drawing.Size(390, 22);
            this.lblAtribucionOsmZonas.TabIndex = 8;
            this.lblAtribucionOsmZonas.Tag = "FormMapLocationPicker.SearchAttribution";
            this.lblAtribucionOsmZonas.Values.Text = "Datos © OpenStreetMap contributors";
            this.lblAtribucionOsmZonas.LinkClicked += new System.EventHandler(this.lblAtribucionOsmZonas_LinkClicked);
            //
            // FormMisZonas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.lblAtribucionOsmZonas);
            this.Controls.Add(this.mapZonasUsuario);
            this.Controls.Add(this.btnMapaAlejar);
            this.Controls.Add(this.btnMapaAcercar);
            this.Controls.Add(this.lblInstruccionesMapaZonas);
            this.Controls.Add(this.groupBoxNuevaZona);
            this.Controls.Add(this.btnQuitarZona);
            this.Controls.Add(this.dgvZonasUsuario);
            this.Controls.Add(this.lblTituloMisZonas);
            this.Name = "FormMisZonas";
            this.Tag = "FormMyZones.Title";
            this.Text = "Mis zonas";
            ((System.ComponentModel.ISupportInitialize)(this.dgvZonasUsuario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNuevaZona.Panel)).EndInit();
            this.groupBoxNuevaZona.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxNuevaZona)).EndInit();
            this.groupBoxNuevaZona.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbResultadosZona)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadioZona)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloMisZonas;
        private Krypton.Toolkit.KryptonDataGridView dgvZonasUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZonaNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZonaRadio;
        private Krypton.Toolkit.KryptonButton btnQuitarZona;
        private Krypton.Toolkit.KryptonGroupBox groupBoxNuevaZona;
        private Krypton.Toolkit.KryptonLabel lblNombreZona;
        private Krypton.Toolkit.KryptonTextBox txtbNombreZona;
        private Krypton.Toolkit.KryptonLabel lblBusquedaZona;
        private Krypton.Toolkit.KryptonTextBox txtbBusquedaZona;
        private Krypton.Toolkit.KryptonButton btnBuscarZona;
        private Krypton.Toolkit.KryptonComboBox cmbResultadosZona;
        private Krypton.Toolkit.KryptonWrapLabel lblEstadoBusquedaZona;
        private Krypton.Toolkit.KryptonLabel lblRadioZona;
        private System.Windows.Forms.NumericUpDown nudRadioZona;
        private Krypton.Toolkit.KryptonLabel lblPuntoZona;
        private Krypton.Toolkit.KryptonLabel lblCoordenadasZona;
        private Krypton.Toolkit.KryptonButton btnAgregarZona;
        private Krypton.Toolkit.KryptonLabel lblInstruccionesMapaZonas;
        private Krypton.Toolkit.KryptonButton btnMapaAcercar;
        private Krypton.Toolkit.KryptonButton btnMapaAlejar;
        private GMap.NET.WindowsForms.GMapControl mapZonasUsuario;
        private Krypton.Toolkit.KryptonLinkLabel lblAtribucionOsmZonas;
        private System.Windows.Forms.ToolTip toolTipMapaZonas;
    }
}
