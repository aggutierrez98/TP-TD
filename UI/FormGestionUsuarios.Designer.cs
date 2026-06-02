namespace TpIDS
{
    partial class FormGestionUsuarios
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
            this.lblTituloGestionUsuarios = new System.Windows.Forms.Label();
            this.lblSesionActual = new System.Windows.Forms.Label();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.groupBoxGestion = new System.Windows.Forms.GroupBox();
            this.lblPasswordUsuario = new System.Windows.Forms.Label();
            this.txtbPasswordUsuario = new System.Windows.Forms.TextBox();
            this.lblNombreUsuario = new System.Windows.Forms.Label();
            this.txtbNombreUsuario = new System.Windows.Forms.TextBox();
            this.btnAgregarUsuario = new System.Windows.Forms.Button();
            this.btnEditarUsuario = new System.Windows.Forms.Button();
            this.btnEliminarUsuario = new System.Windows.Forms.Button();
            this.btnConfiguracion = new System.Windows.Forms.Button();
            this.btnGestionIdiomas = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.groupBoxGestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloGestionUsuarios
            // 
            this.lblTituloGestionUsuarios.AutoSize = true;
            this.lblTituloGestionUsuarios.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionUsuarios.Location = new System.Drawing.Point(12, 15);
            this.lblTituloGestionUsuarios.Name = "lblTituloGestionUsuarios";
            this.lblTituloGestionUsuarios.Size = new System.Drawing.Size(216, 25);
            this.lblTituloGestionUsuarios.TabIndex = 0;
            this.lblTituloGestionUsuarios.Text = "Gestion de Usuarios";
            // 
            // lblSesionActual
            // 
            this.lblSesionActual.AutoSize = true;
            this.lblSesionActual.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblSesionActual.ForeColor = System.Drawing.Color.DimGray;
            this.lblSesionActual.Location = new System.Drawing.Point(14, 45);
            this.lblSesionActual.Name = "lblSesionActual";
            this.lblSesionActual.Size = new System.Drawing.Size(93, 15);
            this.lblSesionActual.TabIndex = 1;
            this.lblSesionActual.Text = "Sesion activa: -";
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Location = new System.Drawing.Point(15, 70);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(470, 230);
            this.dgvUsuarios.TabIndex = 2;
            this.dgvUsuarios.SelectionChanged += new System.EventHandler(this.dgvUsuarios_SelectionChanged);
            // 
            // groupBoxGestion
            // 
            this.groupBoxGestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGestion.Controls.Add(this.lblPasswordUsuario);
            this.groupBoxGestion.Controls.Add(this.txtbPasswordUsuario);
            this.groupBoxGestion.Controls.Add(this.lblNombreUsuario);
            this.groupBoxGestion.Controls.Add(this.txtbNombreUsuario);
            this.groupBoxGestion.Controls.Add(this.btnAgregarUsuario);
            this.groupBoxGestion.Controls.Add(this.btnEditarUsuario);
            this.groupBoxGestion.Controls.Add(this.btnEliminarUsuario);
            this.groupBoxGestion.Location = new System.Drawing.Point(15, 315);
            this.groupBoxGestion.Name = "groupBoxGestion";
            this.groupBoxGestion.Size = new System.Drawing.Size(470, 175);
            this.groupBoxGestion.TabIndex = 3;
            this.groupBoxGestion.TabStop = false;
            this.groupBoxGestion.Text = "Datos del usuario";
            // 
            // lblPasswordUsuario
            // 
            this.lblPasswordUsuario.AutoSize = true;
            this.lblPasswordUsuario.Location = new System.Drawing.Point(10, 75);
            this.lblPasswordUsuario.Name = "lblPasswordUsuario";
            this.lblPasswordUsuario.Size = new System.Drawing.Size(193, 13);
            this.lblPasswordUsuario.TabIndex = 2;
            this.lblPasswordUsuario.Text = "Contrasena (solo para agregar usuario):";
            // 
            // txtbPasswordUsuario
            // 
            this.txtbPasswordUsuario.Location = new System.Drawing.Point(10, 92);
            this.txtbPasswordUsuario.Name = "txtbPasswordUsuario";
            this.txtbPasswordUsuario.PasswordChar = '●';
            this.txtbPasswordUsuario.Size = new System.Drawing.Size(440, 20);
            this.txtbPasswordUsuario.TabIndex = 3;
            // 
            // lblNombreUsuario
            // 
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.Location = new System.Drawing.Point(10, 28);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new System.Drawing.Size(108, 13);
            this.lblNombreUsuario.TabIndex = 0;
            this.lblNombreUsuario.Text = "Nombre de usuario:";
            // 
            // txtbNombreUsuario
            // 
            this.txtbNombreUsuario.Location = new System.Drawing.Point(10, 45);
            this.txtbNombreUsuario.Name = "txtbNombreUsuario";
            this.txtbNombreUsuario.Size = new System.Drawing.Size(440, 20);
            this.txtbNombreUsuario.TabIndex = 1;
            // 
            // btnAgregarUsuario
            // 
            this.btnAgregarUsuario.Location = new System.Drawing.Point(10, 130);
            this.btnAgregarUsuario.Name = "btnAgregarUsuario";
            this.btnAgregarUsuario.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarUsuario.TabIndex = 4;
            this.btnAgregarUsuario.Text = "Agregar usuario";
            this.btnAgregarUsuario.UseVisualStyleBackColor = true;
            this.btnAgregarUsuario.Click += new System.EventHandler(this.btnAgregarUsuario_Click);
            // 
            // btnEditarUsuario
            // 
            this.btnEditarUsuario.Location = new System.Drawing.Point(157, 130);
            this.btnEditarUsuario.Name = "btnEditarUsuario";
            this.btnEditarUsuario.Size = new System.Drawing.Size(135, 30);
            this.btnEditarUsuario.TabIndex = 5;
            this.btnEditarUsuario.Text = "Editar usuario";
            this.btnEditarUsuario.UseVisualStyleBackColor = true;
            this.btnEditarUsuario.Click += new System.EventHandler(this.btnEditarUsuario_Click);
            // 
            // btnEliminarUsuario
            // 
            this.btnEliminarUsuario.Location = new System.Drawing.Point(305, 130);
            this.btnEliminarUsuario.Name = "btnEliminarUsuario";
            this.btnEliminarUsuario.Size = new System.Drawing.Size(135, 30);
            this.btnEliminarUsuario.TabIndex = 6;
            this.btnEliminarUsuario.Text = "Eliminar usuario";
            this.btnEliminarUsuario.UseVisualStyleBackColor = true;
            this.btnEliminarUsuario.Click += new System.EventHandler(this.btnEliminarUsuario_Click);
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.Location = new System.Drawing.Point(15, 500);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(220, 30);
            this.btnConfiguracion.TabIndex = 4;
            this.btnConfiguracion.Text = "Configuracion";
            this.btnConfiguracion.UseVisualStyleBackColor = true;
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            // 
            // btnGestionIdiomas
            // 
            this.btnGestionIdiomas.Location = new System.Drawing.Point(265, 500);
            this.btnGestionIdiomas.Name = "btnGestionIdiomas";
            this.btnGestionIdiomas.Size = new System.Drawing.Size(220, 30);
            this.btnGestionIdiomas.TabIndex = 5;
            this.btnGestionIdiomas.Text = "Gestion de idiomas";
            this.btnGestionIdiomas.UseVisualStyleBackColor = true;
            this.btnGestionIdiomas.Click += new System.EventHandler(this.btnGestionIdiomas_Click);
            // 
            // FormGestionUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 545);
            this.Controls.Add(this.btnGestionIdiomas);
            this.Controls.Add(this.btnConfiguracion);
            this.Controls.Add(this.lblTituloGestionUsuarios);
            this.Controls.Add(this.lblSesionActual);
            this.Controls.Add(this.dgvUsuarios);
            this.Controls.Add(this.groupBoxGestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestion de Usuarios";
            this.Load += new System.EventHandler(this.FormGestionUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.groupBoxGestion.ResumeLayout(false);
            this.groupBoxGestion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloGestionUsuarios;
        private System.Windows.Forms.Label lblSesionActual;
        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.GroupBox groupBoxGestion;
        private System.Windows.Forms.Label lblPasswordUsuario;
        private System.Windows.Forms.TextBox txtbPasswordUsuario;
        private System.Windows.Forms.Label lblNombreUsuario;
        private System.Windows.Forms.TextBox txtbNombreUsuario;
        private System.Windows.Forms.Button btnAgregarUsuario;
        private System.Windows.Forms.Button btnEditarUsuario;
        private System.Windows.Forms.Button btnEliminarUsuario;
        private System.Windows.Forms.Button btnConfiguracion;
        private System.Windows.Forms.Button btnGestionIdiomas;
    }
}
