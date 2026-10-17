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
            this.mapUbicacionComercio = new GMap.NET.WindowsForms.GMapControl();
            this.lblInstruccionesUbicacion = new System.Windows.Forms.Label();
            this.btnAceptarUbicacion = new System.Windows.Forms.Button();
            this.btnCancelarUbicacion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // mapUbicacionComercio
            //
            this.mapUbicacionComercio.Bearing = 0F;
            this.mapUbicacionComercio.CanDragMap = true;
            this.mapUbicacionComercio.EmptyTileColor = System.Drawing.Color.Navy;
            this.mapUbicacionComercio.GrayScaleMode = false;
            this.mapUbicacionComercio.LevelsKeepInMemory = 5;
            this.mapUbicacionComercio.Location = new System.Drawing.Point(12, 41);
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
            this.mapUbicacionComercio.Size = new System.Drawing.Size(560, 400);
            this.mapUbicacionComercio.TabIndex = 0;
            this.mapUbicacionComercio.Zoom = 0D;
            //
            // lblInstruccionesUbicacion
            //
            this.lblInstruccionesUbicacion.AutoSize = true;
            this.lblInstruccionesUbicacion.Location = new System.Drawing.Point(12, 15);
            this.lblInstruccionesUbicacion.Name = "lblInstruccionesUbicacion";
            this.lblInstruccionesUbicacion.Size = new System.Drawing.Size(350, 13);
            this.lblInstruccionesUbicacion.TabIndex = 1;
            this.lblInstruccionesUbicacion.Tag = "FormMapLocationPicker.HintLabel";
            this.lblInstruccionesUbicacion.Text = "Haga click en el mapa para marcar la ubicacion. Arrastre el marcador para ajust" +
    "arla.";
            //
            // btnAceptarUbicacion
            //
            this.btnAceptarUbicacion.Location = new System.Drawing.Point(416, 452);
            this.btnAceptarUbicacion.Name = "btnAceptarUbicacion";
            this.btnAceptarUbicacion.Size = new System.Drawing.Size(75, 30);
            this.btnAceptarUbicacion.TabIndex = 2;
            this.btnAceptarUbicacion.Tag = "FormMapLocationPicker.AcceptButton";
            this.btnAceptarUbicacion.Text = "Aceptar";
            this.btnAceptarUbicacion.UseVisualStyleBackColor = true;
            this.btnAceptarUbicacion.Click += new System.EventHandler(this.btnAceptarUbicacion_Click);
            //
            // btnCancelarUbicacion
            //
            this.btnCancelarUbicacion.Location = new System.Drawing.Point(497, 452);
            this.btnCancelarUbicacion.Name = "btnCancelarUbicacion";
            this.btnCancelarUbicacion.Size = new System.Drawing.Size(75, 30);
            this.btnCancelarUbicacion.TabIndex = 3;
            this.btnCancelarUbicacion.Tag = "FormMapLocationPicker.CancelButton";
            this.btnCancelarUbicacion.Text = "Cancelar";
            this.btnCancelarUbicacion.UseVisualStyleBackColor = true;
            this.btnCancelarUbicacion.Click += new System.EventHandler(this.btnCancelarUbicacion_Click);
            //
            // FormSeleccionarUbicacionComercio
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 494);
            this.Controls.Add(this.btnCancelarUbicacion);
            this.Controls.Add(this.btnAceptarUbicacion);
            this.Controls.Add(this.lblInstruccionesUbicacion);
            this.Controls.Add(this.mapUbicacionComercio);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSeleccionarUbicacionComercio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMapLocationPicker.Title";
            this.Text = "Seleccionar ubicacion en el mapa";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private GMap.NET.WindowsForms.GMapControl mapUbicacionComercio;
        private System.Windows.Forms.Label lblInstruccionesUbicacion;
        private System.Windows.Forms.Button btnAceptarUbicacion;
        private System.Windows.Forms.Button btnCancelarUbicacion;
    }
}
