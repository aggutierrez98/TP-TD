namespace TpIDS
{
    partial class FormBackup
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
            this.lblTituloBackup = new System.Windows.Forms.Label();
            this.groupBoxCrearBackup = new System.Windows.Forms.GroupBox();
            this.btnCrearBackup = new System.Windows.Forms.Button();
            this.btnSeleccionarRutaCrear = new System.Windows.Forms.Button();
            this.txtbRutaCrearBackup = new System.Windows.Forms.TextBox();
            this.lblRutaCrearBackup = new System.Windows.Forms.Label();
            this.groupBoxRestaurarBackup = new System.Windows.Forms.GroupBox();
            this.btnRestaurarBackup = new System.Windows.Forms.Button();
            this.btnSeleccionarRutaRestaurar = new System.Windows.Forms.Button();
            this.txtbRutaRestaurarBackup = new System.Windows.Forms.TextBox();
            this.lblRutaRestaurarBackup = new System.Windows.Forms.Label();
            this.groupBoxCrearBackup.SuspendLayout();
            this.groupBoxRestaurarBackup.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloBackup
            //
            this.lblTituloBackup.AutoSize = true;
            this.lblTituloBackup.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloBackup.Location = new System.Drawing.Point(15, 15);
            this.lblTituloBackup.Name = "lblTituloBackup";
            this.lblTituloBackup.Size = new System.Drawing.Size(0, 25);
            this.lblTituloBackup.TabIndex = 0;
            this.lblTituloBackup.Tag = "FormBackup.TitleLabel";
            //
            // groupBoxCrearBackup
            //
            this.groupBoxCrearBackup.Controls.Add(this.btnCrearBackup);
            this.groupBoxCrearBackup.Controls.Add(this.btnSeleccionarRutaCrear);
            this.groupBoxCrearBackup.Controls.Add(this.txtbRutaCrearBackup);
            this.groupBoxCrearBackup.Controls.Add(this.lblRutaCrearBackup);
            this.groupBoxCrearBackup.Location = new System.Drawing.Point(18, 55);
            this.groupBoxCrearBackup.Name = "groupBoxCrearBackup";
            this.groupBoxCrearBackup.Size = new System.Drawing.Size(560, 110);
            this.groupBoxCrearBackup.TabIndex = 1;
            this.groupBoxCrearBackup.TabStop = false;
            this.groupBoxCrearBackup.Tag = "FormBackup.CreateGroup";
            //
            // lblRutaCrearBackup
            //
            this.lblRutaCrearBackup.AutoSize = true;
            this.lblRutaCrearBackup.Location = new System.Drawing.Point(15, 28);
            this.lblRutaCrearBackup.Name = "lblRutaCrearBackup";
            this.lblRutaCrearBackup.Size = new System.Drawing.Size(0, 13);
            this.lblRutaCrearBackup.TabIndex = 0;
            this.lblRutaCrearBackup.Tag = "FormBackup.CreatePathLabel";
            //
            // txtbRutaCrearBackup
            //
            this.txtbRutaCrearBackup.Location = new System.Drawing.Point(18, 48);
            this.txtbRutaCrearBackup.Name = "txtbRutaCrearBackup";
            this.txtbRutaCrearBackup.ReadOnly = true;
            this.txtbRutaCrearBackup.Size = new System.Drawing.Size(400, 20);
            this.txtbRutaCrearBackup.TabIndex = 1;
            //
            // btnSeleccionarRutaCrear
            //
            this.btnSeleccionarRutaCrear.Location = new System.Drawing.Point(424, 46);
            this.btnSeleccionarRutaCrear.Name = "btnSeleccionarRutaCrear";
            this.btnSeleccionarRutaCrear.Size = new System.Drawing.Size(120, 25);
            this.btnSeleccionarRutaCrear.TabIndex = 2;
            this.btnSeleccionarRutaCrear.Tag = "FormBackup.BrowseCreateButton";
            this.btnSeleccionarRutaCrear.UseVisualStyleBackColor = true;
            this.btnSeleccionarRutaCrear.Click += new System.EventHandler(this.btnSeleccionarRutaCrear_Click);
            //
            // btnCrearBackup
            //
            this.btnCrearBackup.Enabled = false;
            this.btnCrearBackup.Location = new System.Drawing.Point(18, 78);
            this.btnCrearBackup.Name = "btnCrearBackup";
            this.btnCrearBackup.Size = new System.Drawing.Size(160, 25);
            this.btnCrearBackup.TabIndex = 3;
            this.btnCrearBackup.Tag = "FormBackup.CreateButton";
            this.btnCrearBackup.UseVisualStyleBackColor = true;
            this.btnCrearBackup.Click += new System.EventHandler(this.btnCrearBackup_Click);
            //
            // groupBoxRestaurarBackup
            //
            this.groupBoxRestaurarBackup.Controls.Add(this.btnRestaurarBackup);
            this.groupBoxRestaurarBackup.Controls.Add(this.btnSeleccionarRutaRestaurar);
            this.groupBoxRestaurarBackup.Controls.Add(this.txtbRutaRestaurarBackup);
            this.groupBoxRestaurarBackup.Controls.Add(this.lblRutaRestaurarBackup);
            this.groupBoxRestaurarBackup.Location = new System.Drawing.Point(18, 180);
            this.groupBoxRestaurarBackup.Name = "groupBoxRestaurarBackup";
            this.groupBoxRestaurarBackup.Size = new System.Drawing.Size(560, 110);
            this.groupBoxRestaurarBackup.TabIndex = 2;
            this.groupBoxRestaurarBackup.TabStop = false;
            this.groupBoxRestaurarBackup.Tag = "FormBackup.RestoreGroup";
            //
            // lblRutaRestaurarBackup
            //
            this.lblRutaRestaurarBackup.AutoSize = true;
            this.lblRutaRestaurarBackup.Location = new System.Drawing.Point(15, 28);
            this.lblRutaRestaurarBackup.Name = "lblRutaRestaurarBackup";
            this.lblRutaRestaurarBackup.Size = new System.Drawing.Size(0, 13);
            this.lblRutaRestaurarBackup.TabIndex = 0;
            this.lblRutaRestaurarBackup.Tag = "FormBackup.RestorePathLabel";
            //
            // txtbRutaRestaurarBackup
            //
            this.txtbRutaRestaurarBackup.Location = new System.Drawing.Point(18, 48);
            this.txtbRutaRestaurarBackup.Name = "txtbRutaRestaurarBackup";
            this.txtbRutaRestaurarBackup.ReadOnly = true;
            this.txtbRutaRestaurarBackup.Size = new System.Drawing.Size(400, 20);
            this.txtbRutaRestaurarBackup.TabIndex = 1;
            //
            // btnSeleccionarRutaRestaurar
            //
            this.btnSeleccionarRutaRestaurar.Location = new System.Drawing.Point(424, 46);
            this.btnSeleccionarRutaRestaurar.Name = "btnSeleccionarRutaRestaurar";
            this.btnSeleccionarRutaRestaurar.Size = new System.Drawing.Size(120, 25);
            this.btnSeleccionarRutaRestaurar.TabIndex = 2;
            this.btnSeleccionarRutaRestaurar.Tag = "FormBackup.BrowseRestoreButton";
            this.btnSeleccionarRutaRestaurar.UseVisualStyleBackColor = true;
            this.btnSeleccionarRutaRestaurar.Click += new System.EventHandler(this.btnSeleccionarRutaRestaurar_Click);
            //
            // btnRestaurarBackup
            //
            this.btnRestaurarBackup.Enabled = false;
            this.btnRestaurarBackup.Location = new System.Drawing.Point(18, 78);
            this.btnRestaurarBackup.Name = "btnRestaurarBackup";
            this.btnRestaurarBackup.Size = new System.Drawing.Size(160, 25);
            this.btnRestaurarBackup.TabIndex = 3;
            this.btnRestaurarBackup.Tag = "FormBackup.RestoreButton";
            this.btnRestaurarBackup.UseVisualStyleBackColor = true;
            this.btnRestaurarBackup.Click += new System.EventHandler(this.btnRestaurarBackup_Click);
            //
            // FormBackup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 310);
            this.Controls.Add(this.groupBoxRestaurarBackup);
            this.Controls.Add(this.groupBoxCrearBackup);
            this.Controls.Add(this.lblTituloBackup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormBackup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormBackup.Title";
            this.groupBoxCrearBackup.ResumeLayout(false);
            this.groupBoxCrearBackup.PerformLayout();
            this.groupBoxRestaurarBackup.ResumeLayout(false);
            this.groupBoxRestaurarBackup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloBackup;
        private System.Windows.Forms.GroupBox groupBoxCrearBackup;
        private System.Windows.Forms.Label lblRutaCrearBackup;
        private System.Windows.Forms.TextBox txtbRutaCrearBackup;
        private System.Windows.Forms.Button btnSeleccionarRutaCrear;
        private System.Windows.Forms.Button btnCrearBackup;
        private System.Windows.Forms.GroupBox groupBoxRestaurarBackup;
        private System.Windows.Forms.Label lblRutaRestaurarBackup;
        private System.Windows.Forms.TextBox txtbRutaRestaurarBackup;
        private System.Windows.Forms.Button btnSeleccionarRutaRestaurar;
        private System.Windows.Forms.Button btnRestaurarBackup;
    }
}
