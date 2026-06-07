namespace TpIDS
{
    partial class FormHistorialUsuario
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
            this.lblTituloHistorialUsuario = new System.Windows.Forms.Label();
            this.lblUsuarioHistorial = new System.Windows.Forms.Label();
            this.dgvHistorialUsuario = new System.Windows.Forms.DataGridView();
            this.dgvCambiosEstado = new System.Windows.Forms.DataGridView();
            this.btnRestaurarVersion = new System.Windows.Forms.Button();
            this.btnRefrescarHistorial = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambiosEstado)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTituloHistorialUsuario
            // 
            this.lblTituloHistorialUsuario.AutoSize = true;
            this.lblTituloHistorialUsuario.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloHistorialUsuario.Location = new System.Drawing.Point(15, 15);
            this.lblTituloHistorialUsuario.Name = "lblTituloHistorialUsuario";
            this.lblTituloHistorialUsuario.Size = new System.Drawing.Size(0, 25);
            this.lblTituloHistorialUsuario.Tag = "FormHistorialUsuario.TitleLabel";
            this.lblTituloHistorialUsuario.TabIndex = 0;
            // 
            // lblUsuarioHistorial
            // 
            this.lblUsuarioHistorial.AutoSize = true;
            this.lblUsuarioHistorial.Location = new System.Drawing.Point(17, 48);
            this.lblUsuarioHistorial.Name = "lblUsuarioHistorial";
            this.lblUsuarioHistorial.Size = new System.Drawing.Size(0, 13);
            this.lblUsuarioHistorial.TabIndex = 1;
            // 
            // dgvHistorialUsuario
            // 
            this.dgvHistorialUsuario.AllowUserToAddRows = false;
            this.dgvHistorialUsuario.AllowUserToDeleteRows = false;
            this.dgvHistorialUsuario.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorialUsuario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialUsuario.Location = new System.Drawing.Point(20, 75);
            this.dgvHistorialUsuario.MultiSelect = false;
            this.dgvHistorialUsuario.Name = "dgvHistorialUsuario";
            this.dgvHistorialUsuario.ReadOnly = true;
            this.dgvHistorialUsuario.RowHeadersVisible = false;
            this.dgvHistorialUsuario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorialUsuario.Size = new System.Drawing.Size(760, 240);
            this.dgvHistorialUsuario.TabIndex = 2;
            this.dgvHistorialUsuario.SelectionChanged += new System.EventHandler(this.dgvHistorialUsuario_SelectionChanged);
            // 
            // dgvCambiosEstado
            // 
            this.dgvCambiosEstado.AllowUserToAddRows = false;
            this.dgvCambiosEstado.AllowUserToDeleteRows = false;
            this.dgvCambiosEstado.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCambiosEstado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCambiosEstado.Location = new System.Drawing.Point(20, 330);
            this.dgvCambiosEstado.MultiSelect = false;
            this.dgvCambiosEstado.Name = "dgvCambiosEstado";
            this.dgvCambiosEstado.ReadOnly = true;
            this.dgvCambiosEstado.RowHeadersVisible = false;
            this.dgvCambiosEstado.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCambiosEstado.Size = new System.Drawing.Size(760, 180);
            this.dgvCambiosEstado.TabIndex = 3;
            // 
            // btnRestaurarVersion
            // 
            this.btnRestaurarVersion.Location = new System.Drawing.Point(570, 525);
            this.btnRestaurarVersion.Name = "btnRestaurarVersion";
            this.btnRestaurarVersion.Size = new System.Drawing.Size(210, 30);
            this.btnRestaurarVersion.Tag = "FormHistorialUsuario.RestoreButton";
            this.btnRestaurarVersion.TabIndex = 5;
            this.btnRestaurarVersion.UseVisualStyleBackColor = true;
            this.btnRestaurarVersion.Click += new System.EventHandler(this.btnRestaurarVersion_Click);
            // 
            // btnRefrescarHistorial
            // 
            this.btnRefrescarHistorial.Location = new System.Drawing.Point(20, 525);
            this.btnRefrescarHistorial.Name = "btnRefrescarHistorial";
            this.btnRefrescarHistorial.Size = new System.Drawing.Size(210, 30);
            this.btnRefrescarHistorial.Tag = "FormHistorialUsuario.RefreshButton";
            this.btnRefrescarHistorial.TabIndex = 4;
            this.btnRefrescarHistorial.UseVisualStyleBackColor = true;
            this.btnRefrescarHistorial.Click += new System.EventHandler(this.btnRefrescarHistorial_Click);
            // 
            // FormHistorialUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 575);
            this.Controls.Add(this.btnRefrescarHistorial);
            this.Controls.Add(this.btnRestaurarVersion);
            this.Controls.Add(this.dgvCambiosEstado);
            this.Controls.Add(this.dgvHistorialUsuario);
            this.Controls.Add(this.lblUsuarioHistorial);
            this.Controls.Add(this.lblTituloHistorialUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormHistorialUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormHistorialUsuario.Title";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialUsuario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambiosEstado)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloHistorialUsuario;
        private System.Windows.Forms.Label lblUsuarioHistorial;
        private System.Windows.Forms.DataGridView dgvHistorialUsuario;
        private System.Windows.Forms.DataGridView dgvCambiosEstado;
        private System.Windows.Forms.Button btnRestaurarVersion;
        private System.Windows.Forms.Button btnRefrescarHistorial;
    }
}
