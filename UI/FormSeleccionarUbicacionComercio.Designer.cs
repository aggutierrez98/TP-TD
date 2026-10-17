namespace TpIDS
{
    partial class FormSeleccionarUbicacionComercio
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
            this.mapUbicacionComercio = new GMap.NET.WindowsForms.GMapControl();
            this.lblInstruccionesUbicacion = new Krypton.Toolkit.KryptonLabel();
            this.lblBusquedaUbicacion = new Krypton.Toolkit.KryptonLabel();
            this.txtbBusquedaUbicacion = new Krypton.Toolkit.KryptonTextBox();
            this.btnBuscarUbicacion = new Krypton.Toolkit.KryptonButton();
            this.cmbResultadosUbicacion = new Krypton.Toolkit.KryptonComboBox();
            this.lblEstadoBusquedaUbicacion = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblAtribucionOsmUbicacion = new Krypton.Toolkit.KryptonLinkLabel();
            this.btnAceptarUbicacion = new Krypton.Toolkit.KryptonButton();
            this.btnCancelarUbicacion = new Krypton.Toolkit.KryptonButton();
            this.btnMapaArriba = new Krypton.Toolkit.KryptonButton();
            this.btnMapaAbajo = new Krypton.Toolkit.KryptonButton();
            this.btnMapaIzquierda = new Krypton.Toolkit.KryptonButton();
            this.btnMapaDerecha = new Krypton.Toolkit.KryptonButton();
            this.btnMapaAcercar = new Krypton.Toolkit.KryptonButton();
            this.btnMapaAlejar = new Krypton.Toolkit.KryptonButton();
            this.toolTipNavegacion = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cmbResultadosUbicacion)).BeginInit();
            this.mapUbicacionComercio.SuspendLayout();
            this.SuspendLayout();
            //
            // mapUbicacionComercio
            //
            this.mapUbicacionComercio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.mapUbicacionComercio.Bearing = 0F;
            this.mapUbicacionComercio.CanDragMap = true;
            this.mapUbicacionComercio.EmptyTileColor = System.Drawing.Color.Navy;
            this.mapUbicacionComercio.GrayScaleMode = false;
            this.mapUbicacionComercio.LevelsKeepInMemory = 5;
            this.mapUbicacionComercio.Location = new System.Drawing.Point(12, 163);
            this.mapUbicacionComercio.MarkersEnabled = true;
            this.mapUbicacionComercio.MaxZoom = 18;
            this.mapUbicacionComercio.MinZoom = 2;
            this.mapUbicacionComercio.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.mapUbicacionComercio.Name = "mapUbicacionComercio";
            this.mapUbicacionComercio.NegativeMode = false;
            this.mapUbicacionComercio.PolygonsEnabled = true;
            this.mapUbicacionComercio.RetryLoadTile = 0;
            this.mapUbicacionComercio.RoutesEnabled = true;
            this.mapUbicacionComercio.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.mapUbicacionComercio.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.mapUbicacionComercio.ShowTileGridLines = false;
            this.mapUbicacionComercio.Size = new System.Drawing.Size(560, 326);
            this.mapUbicacionComercio.TabIndex = 6;
            this.mapUbicacionComercio.Zoom = 0D;
            this.mapUbicacionComercio.Controls.Add(this.btnMapaArriba);
            this.mapUbicacionComercio.Controls.Add(this.btnMapaAbajo);
            this.mapUbicacionComercio.Controls.Add(this.btnMapaIzquierda);
            this.mapUbicacionComercio.Controls.Add(this.btnMapaDerecha);
            this.mapUbicacionComercio.Controls.Add(this.btnMapaAcercar);
            this.mapUbicacionComercio.Controls.Add(this.btnMapaAlejar);
            //
            // btnMapaArriba
            //
            this.btnMapaArriba.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaArriba.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaArriba.Location = new System.Drawing.Point(430, 214);
            this.btnMapaArriba.Name = "btnMapaArriba";
            this.btnMapaArriba.Size = new System.Drawing.Size(32, 32);
            this.btnMapaArriba.TabIndex = 0;
            this.btnMapaArriba.Values.Text = "↑";
            this.btnMapaArriba.Click += new System.EventHandler(this.btnMapaArriba_Click);
            //
            // btnMapaIzquierda
            //
            this.btnMapaIzquierda.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaIzquierda.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaIzquierda.Location = new System.Drawing.Point(396, 248);
            this.btnMapaIzquierda.Name = "btnMapaIzquierda";
            this.btnMapaIzquierda.Size = new System.Drawing.Size(32, 32);
            this.btnMapaIzquierda.TabIndex = 1;
            this.btnMapaIzquierda.Values.Text = "←";
            this.btnMapaIzquierda.Click += new System.EventHandler(this.btnMapaIzquierda_Click);
            //
            // btnMapaDerecha
            //
            this.btnMapaDerecha.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaDerecha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaDerecha.Location = new System.Drawing.Point(464, 248);
            this.btnMapaDerecha.Name = "btnMapaDerecha";
            this.btnMapaDerecha.Size = new System.Drawing.Size(32, 32);
            this.btnMapaDerecha.TabIndex = 2;
            this.btnMapaDerecha.Values.Text = "→";
            this.btnMapaDerecha.Click += new System.EventHandler(this.btnMapaDerecha_Click);
            //
            // btnMapaAbajo
            //
            this.btnMapaAbajo.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaAbajo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaAbajo.Location = new System.Drawing.Point(430, 282);
            this.btnMapaAbajo.Name = "btnMapaAbajo";
            this.btnMapaAbajo.Size = new System.Drawing.Size(32, 32);
            this.btnMapaAbajo.TabIndex = 3;
            this.btnMapaAbajo.Values.Text = "↓";
            this.btnMapaAbajo.Click += new System.EventHandler(this.btnMapaAbajo_Click);
            //
            // btnMapaAcercar
            //
            this.btnMapaAcercar.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaAcercar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaAcercar.Location = new System.Drawing.Point(512, 238);
            this.btnMapaAcercar.Name = "btnMapaAcercar";
            this.btnMapaAcercar.Size = new System.Drawing.Size(36, 36);
            this.btnMapaAcercar.TabIndex = 4;
            this.btnMapaAcercar.Values.Text = "+";
            this.btnMapaAcercar.Click += new System.EventHandler(this.btnMapaAcercar_Click);
            //
            // btnMapaAlejar
            //
            this.btnMapaAlejar.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnMapaAlejar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapaAlejar.Location = new System.Drawing.Point(512, 278);
            this.btnMapaAlejar.Name = "btnMapaAlejar";
            this.btnMapaAlejar.Size = new System.Drawing.Size(36, 36);
            this.btnMapaAlejar.TabIndex = 5;
            this.btnMapaAlejar.Values.Text = "−";
            this.btnMapaAlejar.Click += new System.EventHandler(this.btnMapaAlejar_Click);
            //
            // lblInstruccionesUbicacion
            //
            this.lblInstruccionesUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstruccionesUbicacion.AutoSize = false;
            this.lblInstruccionesUbicacion.Location = new System.Drawing.Point(12, 8);
            this.lblInstruccionesUbicacion.Name = "lblInstruccionesUbicacion";
            this.lblInstruccionesUbicacion.Size = new System.Drawing.Size(560, 54);
            this.lblInstruccionesUbicacion.TabIndex = 0;
            this.lblInstruccionesUbicacion.Tag = "FormMapLocationPicker.HintLabel";
            this.lblInstruccionesUbicacion.Values.Text = "Arrastre con el botón derecho para mover el mapa.\r\nUse la rueda o los botones +/− para acercar o alejar.\r\nHaga clic izquierdo para marcar. Arrastre el marcador para ajustar.";
            //
            // lblBusquedaUbicacion
            //
            this.lblBusquedaUbicacion.AutoSize = false;
            this.lblBusquedaUbicacion.Location = new System.Drawing.Point(12, 67);
            this.lblBusquedaUbicacion.Name = "lblBusquedaUbicacion";
            this.lblBusquedaUbicacion.Size = new System.Drawing.Size(182, 22);
            this.lblBusquedaUbicacion.TabIndex = 1;
            this.lblBusquedaUbicacion.Tag = "FormMapLocationPicker.SearchLabel";
            this.lblBusquedaUbicacion.Values.Text = "Dirección, comercio o lugar";
            //
            // txtbBusquedaUbicacion
            //
            this.txtbBusquedaUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbBusquedaUbicacion.Location = new System.Drawing.Point(198, 67);
            this.txtbBusquedaUbicacion.MaxLength = 200;
            this.txtbBusquedaUbicacion.Name = "txtbBusquedaUbicacion";
            this.txtbBusquedaUbicacion.Size = new System.Drawing.Size(292, 22);
            this.txtbBusquedaUbicacion.TabIndex = 2;
            this.txtbBusquedaUbicacion.TextChanged += new System.EventHandler(this.txtbBusquedaUbicacion_TextChanged);
            this.txtbBusquedaUbicacion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbBusquedaUbicacion_KeyDown);
            //
            // btnBuscarUbicacion
            //
            this.btnBuscarUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarUbicacion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnBuscarUbicacion.Location = new System.Drawing.Point(496, 66);
            this.btnBuscarUbicacion.Name = "btnBuscarUbicacion";
            this.btnBuscarUbicacion.Size = new System.Drawing.Size(76, 24);
            this.btnBuscarUbicacion.TabIndex = 3;
            this.btnBuscarUbicacion.Tag = "FormMapLocationPicker.SearchButton";
            this.btnBuscarUbicacion.Values.Text = "Buscar";
            this.btnBuscarUbicacion.Click += new System.EventHandler(this.btnBuscarUbicacion_Click);
            //
            // cmbResultadosUbicacion
            //
            this.cmbResultadosUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbResultadosUbicacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResultadosUbicacion.Enabled = false;
            this.cmbResultadosUbicacion.FormattingEnabled = true;
            this.cmbResultadosUbicacion.Location = new System.Drawing.Point(12, 97);
            this.cmbResultadosUbicacion.Name = "cmbResultadosUbicacion";
            this.cmbResultadosUbicacion.Size = new System.Drawing.Size(560, 24);
            this.cmbResultadosUbicacion.TabIndex = 4;
            this.cmbResultadosUbicacion.DropDown += new System.EventHandler(this.cmbResultadosUbicacion_DropDown);
            this.cmbResultadosUbicacion.SelectionChangeCommitted += new System.EventHandler(this.cmbResultadosUbicacion_SelectionChangeCommitted);
            //
            // lblEstadoBusquedaUbicacion
            //
            this.lblEstadoBusquedaUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstadoBusquedaUbicacion.AutoSize = false;
            this.lblEstadoBusquedaUbicacion.Location = new System.Drawing.Point(14, 128);
            this.lblEstadoBusquedaUbicacion.Name = "lblEstadoBusquedaUbicacion";
            this.lblEstadoBusquedaUbicacion.Size = new System.Drawing.Size(556, 32);
            this.lblEstadoBusquedaUbicacion.TabIndex = 5;
            //
            // lblAtribucionOsmUbicacion
            //
            this.lblAtribucionOsmUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAtribucionOsmUbicacion.AutoSize = false;
            this.lblAtribucionOsmUbicacion.Location = new System.Drawing.Point(12, 506);
            this.lblAtribucionOsmUbicacion.Name = "lblAtribucionOsmUbicacion";
            this.lblAtribucionOsmUbicacion.Size = new System.Drawing.Size(390, 22);
            this.lblAtribucionOsmUbicacion.TabIndex = 9;
            this.lblAtribucionOsmUbicacion.Tag = "FormMapLocationPicker.SearchAttribution";
            this.lblAtribucionOsmUbicacion.Values.Text = "Datos © OpenStreetMap contributors";
            this.lblAtribucionOsmUbicacion.LinkClicked += new System.EventHandler(this.lblAtribucionOsmUbicacion_LinkClicked);
            //
            // btnAceptarUbicacion
            //
            this.btnAceptarUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAceptarUbicacion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAceptarUbicacion.Location = new System.Drawing.Point(416, 500);
            this.btnAceptarUbicacion.Name = "btnAceptarUbicacion";
            this.btnAceptarUbicacion.Size = new System.Drawing.Size(75, 30);
            this.btnAceptarUbicacion.TabIndex = 7;
            this.btnAceptarUbicacion.Tag = "FormMapLocationPicker.AcceptButton";
            this.btnAceptarUbicacion.Values.Text = "Aceptar";
            this.btnAceptarUbicacion.Click += new System.EventHandler(this.btnAceptarUbicacion_Click);
            //
            // btnCancelarUbicacion
            //
            this.btnCancelarUbicacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelarUbicacion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnCancelarUbicacion.Location = new System.Drawing.Point(497, 500);
            this.btnCancelarUbicacion.Name = "btnCancelarUbicacion";
            this.btnCancelarUbicacion.Size = new System.Drawing.Size(75, 30);
            this.btnCancelarUbicacion.TabIndex = 8;
            this.btnCancelarUbicacion.Tag = "FormMapLocationPicker.CancelButton";
            this.btnCancelarUbicacion.Values.Text = "Cancelar";
            this.btnCancelarUbicacion.Click += new System.EventHandler(this.btnCancelarUbicacion_Click);
            //
            // FormSeleccionarUbicacionComercio
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 542);
            this.Controls.Add(this.lblAtribucionOsmUbicacion);
            this.Controls.Add(this.lblEstadoBusquedaUbicacion);
            this.Controls.Add(this.cmbResultadosUbicacion);
            this.Controls.Add(this.btnBuscarUbicacion);
            this.Controls.Add(this.txtbBusquedaUbicacion);
            this.Controls.Add(this.lblBusquedaUbicacion);
            this.Controls.Add(this.btnCancelarUbicacion);
            this.Controls.Add(this.btnAceptarUbicacion);
            this.Controls.Add(this.lblInstruccionesUbicacion);
            this.Controls.Add(this.mapUbicacionComercio);
            this.MinimizeBox = false;
            this.Name = "FormSeleccionarUbicacionComercio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMapLocationPicker.Title";
            this.Text = "Seleccionar ubicacion en el mapa";
            ((System.ComponentModel.ISupportInitialize)(this.cmbResultadosUbicacion)).EndInit();
            this.mapUbicacionComercio.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private GMap.NET.WindowsForms.GMapControl mapUbicacionComercio;
        private Krypton.Toolkit.KryptonLabel lblInstruccionesUbicacion;
        private Krypton.Toolkit.KryptonLabel lblBusquedaUbicacion;
        private Krypton.Toolkit.KryptonTextBox txtbBusquedaUbicacion;
        private Krypton.Toolkit.KryptonButton btnBuscarUbicacion;
        private Krypton.Toolkit.KryptonComboBox cmbResultadosUbicacion;
        private Krypton.Toolkit.KryptonWrapLabel lblEstadoBusquedaUbicacion;
        private Krypton.Toolkit.KryptonLinkLabel lblAtribucionOsmUbicacion;
        private Krypton.Toolkit.KryptonButton btnAceptarUbicacion;
        private Krypton.Toolkit.KryptonButton btnCancelarUbicacion;
        private Krypton.Toolkit.KryptonButton btnMapaArriba;
        private Krypton.Toolkit.KryptonButton btnMapaAbajo;
        private Krypton.Toolkit.KryptonButton btnMapaIzquierda;
        private Krypton.Toolkit.KryptonButton btnMapaDerecha;
        private Krypton.Toolkit.KryptonButton btnMapaAcercar;
        private Krypton.Toolkit.KryptonButton btnMapaAlejar;
        private System.Windows.Forms.ToolTip toolTipNavegacion;
    }
}
