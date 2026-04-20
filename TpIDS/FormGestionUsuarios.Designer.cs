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
            this.labelTitulo = new System.Windows.Forms.Label();
            this.labelSesionActual = new System.Windows.Forms.Label();
            this.dataGridViewUsuarios = new System.Windows.Forms.DataGridView();
            this.groupBoxGestion = new System.Windows.Forms.GroupBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.buttonAgregar = new System.Windows.Forms.Button();
            this.buttonEditar = new System.Windows.Forms.Button();
            this.buttonEliminar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsuarios)).BeginInit();
            this.groupBoxGestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitulo.Location = new System.Drawing.Point(12, 15);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(220, 25);
            this.labelTitulo.TabIndex = 0;
            this.labelTitulo.Text = "Gestión de Usuarios";
            // 
            // labelSesionActual
            // 
            this.labelSesionActual.AutoSize = true;
            this.labelSesionActual.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.labelSesionActual.ForeColor = System.Drawing.Color.DimGray;
            this.labelSesionActual.Location = new System.Drawing.Point(14, 45);
            this.labelSesionActual.Name = "labelSesionActual";
            this.labelSesionActual.Size = new System.Drawing.Size(100, 15);
            this.labelSesionActual.TabIndex = 1;
            this.labelSesionActual.Text = "Sesión activa: —";
            // 
            // dataGridViewUsuarios
            // 
            this.dataGridViewUsuarios.AllowUserToAddRows = false;
            this.dataGridViewUsuarios.AllowUserToDeleteRows = false;
            this.dataGridViewUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUsuarios.Location = new System.Drawing.Point(15, 70);
            this.dataGridViewUsuarios.MultiSelect = false;
            this.dataGridViewUsuarios.Name = "dataGridViewUsuarios";
            this.dataGridViewUsuarios.ReadOnly = true;
            this.dataGridViewUsuarios.RowHeadersVisible = false;
            this.dataGridViewUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUsuarios.Size = new System.Drawing.Size(470, 230);
            this.dataGridViewUsuarios.TabIndex = 2;
            this.dataGridViewUsuarios.SelectionChanged += new System.EventHandler(this.dataGridViewUsuarios_SelectionChanged);
            // 
            // groupBoxGestion
            // 
            this.groupBoxGestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGestion.Controls.Add(this.labelPassword);
            this.groupBoxGestion.Controls.Add(this.textBoxPassword);
            this.groupBoxGestion.Controls.Add(this.labelUsername);
            this.groupBoxGestion.Controls.Add(this.textBoxUsername);
            this.groupBoxGestion.Controls.Add(this.buttonAgregar);
            this.groupBoxGestion.Controls.Add(this.buttonEditar);
            this.groupBoxGestion.Controls.Add(this.buttonEliminar);
            this.groupBoxGestion.Location = new System.Drawing.Point(15, 315);
            this.groupBoxGestion.Name = "groupBoxGestion";
            this.groupBoxGestion.Size = new System.Drawing.Size(470, 175);
            this.groupBoxGestion.TabIndex = 3;
            this.groupBoxGestion.TabStop = false;
            this.groupBoxGestion.Text = "Datos del usuario";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(10, 28);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(108, 13);
            this.labelUsername.TabIndex = 0;
            this.labelUsername.Text = "Nombre de usuario:";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(10, 45);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(440, 20);
            this.textBoxUsername.TabIndex = 1;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(10, 75);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(195, 13);
            this.labelPassword.TabIndex = 2;
            this.labelPassword.Text = "Contraseña (solo para Agregar usuario):";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(10, 92);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '●';
            this.textBoxPassword.Size = new System.Drawing.Size(440, 20);
            this.textBoxPassword.TabIndex = 3;
            // 
            // buttonAgregar
            // 
            this.buttonAgregar.Location = new System.Drawing.Point(10, 130);
            this.buttonAgregar.Name = "buttonAgregar";
            this.buttonAgregar.Size = new System.Drawing.Size(135, 30);
            this.buttonAgregar.TabIndex = 4;
            this.buttonAgregar.Text = "Agregar usuario";
            this.buttonAgregar.UseVisualStyleBackColor = true;
            this.buttonAgregar.Click += new System.EventHandler(this.buttonAgregar_Click);
            // 
            // buttonEditar
            // 
            this.buttonEditar.Location = new System.Drawing.Point(157, 130);
            this.buttonEditar.Name = "buttonEditar";
            this.buttonEditar.Size = new System.Drawing.Size(135, 30);
            this.buttonEditar.TabIndex = 5;
            this.buttonEditar.Text = "Editar usuario";
            this.buttonEditar.UseVisualStyleBackColor = true;
            this.buttonEditar.Click += new System.EventHandler(this.buttonEditar_Click);
            // 
            // buttonEliminar
            // 
            this.buttonEliminar.Location = new System.Drawing.Point(305, 130);
            this.buttonEliminar.Name = "buttonEliminar";
            this.buttonEliminar.Size = new System.Drawing.Size(135, 30);
            this.buttonEliminar.TabIndex = 6;
            this.buttonEliminar.Text = "Eliminar usuario";
            this.buttonEliminar.UseVisualStyleBackColor = true;
            this.buttonEliminar.Click += new System.EventHandler(this.buttonEliminar_Click);
            // 
            // FormGestionUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 510);
            this.Controls.Add(this.labelTitulo);
            this.Controls.Add(this.labelSesionActual);
            this.Controls.Add(this.dataGridViewUsuarios);
            this.Controls.Add(this.groupBoxGestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Usuarios";
            this.Load += new System.EventHandler(this.FormGestionUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsuarios)).EndInit();
            this.groupBoxGestion.ResumeLayout(false);
            this.groupBoxGestion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Label labelSesionActual;
        private System.Windows.Forms.DataGridView dataGridViewUsuarios;
        private System.Windows.Forms.GroupBox groupBoxGestion;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonAgregar;
        private System.Windows.Forms.Button buttonEditar;
        private System.Windows.Forms.Button buttonEliminar;
    }
}
