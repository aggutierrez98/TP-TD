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
            this.lblTituloBackup = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxCrearBackup = new Krypton.Toolkit.KryptonGroupBox();
            this.btnCrearBackup = new Krypton.Toolkit.KryptonButton();
            this.btnSeleccionarRutaCrear = new Krypton.Toolkit.KryptonButton();
            this.txtbRutaCrearBackup = new Krypton.Toolkit.KryptonTextBox();
            this.lblRutaCrearBackup = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxRestaurarBackup = new Krypton.Toolkit.KryptonGroupBox();
            this.btnRestaurarBackup = new Krypton.Toolkit.KryptonButton();
            this.btnSeleccionarRutaRestaurar = new Krypton.Toolkit.KryptonButton();
            this.txtbRutaRestaurarBackup = new Krypton.Toolkit.KryptonTextBox();
            this.lblRutaRestaurarBackup = new Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCrearBackup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCrearBackup.Panel)).BeginInit();
            this.groupBoxCrearBackup.Panel.SuspendLayout();
            this.groupBoxCrearBackup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRestaurarBackup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRestaurarBackup.Panel)).BeginInit();
            this.groupBoxRestaurarBackup.Panel.SuspendLayout();
            this.groupBoxRestaurarBackup.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloBackup
            //
            this.lblTituloBackup.AutoSize = true;
            this.lblTituloBackup.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloBackup.Location = new System.Drawing.Point(15, 15);
            this.lblTituloBackup.Name = "lblTituloBackup";
            this.lblTituloBackup.Size = new System.Drawing.Size(0, 25);
            this.lblTituloBackup.TabIndex = 0;
            this.lblTituloBackup.Tag = "FormBackup.TitleLabel";
            //
            // groupBoxCrearBackup
            //
            this.groupBoxCrearBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCrearBackup.Panel.Controls.Add(this.btnCrearBackup);
            this.groupBoxCrearBackup.Panel.Controls.Add(this.btnSeleccionarRutaCrear);
            this.groupBoxCrearBackup.Panel.Controls.Add(this.txtbRutaCrearBackup);
            this.groupBoxCrearBackup.Panel.Controls.Add(this.lblRutaCrearBackup);
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
            this.lblRutaCrearBackup.Location = new System.Drawing.Point(15, 8);
            this.lblRutaCrearBackup.Name = "lblRutaCrearBackup";
            this.lblRutaCrearBackup.Size = new System.Drawing.Size(0, 13);
            this.lblRutaCrearBackup.TabIndex = 0;
            this.lblRutaCrearBackup.Tag = "FormBackup.CreatePathLabel";
            //
            // txtbRutaCrearBackup
            //
            this.txtbRutaCrearBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbRutaCrearBackup.Location = new System.Drawing.Point(18, 32);
            this.txtbRutaCrearBackup.Name = "txtbRutaCrearBackup";
            this.txtbRutaCrearBackup.ReadOnly = true;
            this.txtbRutaCrearBackup.Size = new System.Drawing.Size(370, 20);
            this.txtbRutaCrearBackup.TabIndex = 1;
            //
            // btnSeleccionarRutaCrear
            //
            this.btnSeleccionarRutaCrear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSeleccionarRutaCrear.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnSeleccionarRutaCrear.Location = new System.Drawing.Point(394, 30);
            this.btnSeleccionarRutaCrear.Name = "btnSeleccionarRutaCrear";
            this.btnSeleccionarRutaCrear.Size = new System.Drawing.Size(150, 25);
            this.btnSeleccionarRutaCrear.TabIndex = 2;
            this.btnSeleccionarRutaCrear.Tag = "FormBackup.BrowseCreateButton";
            this.btnSeleccionarRutaCrear.Click += new System.EventHandler(this.btnSeleccionarRutaCrear_Click);
            //
            // btnCrearBackup
            //
            this.btnCrearBackup.Enabled = false;
            this.btnCrearBackup.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnCrearBackup.Location = new System.Drawing.Point(18, 62);
            this.btnCrearBackup.Name = "btnCrearBackup";
            this.btnCrearBackup.Size = new System.Drawing.Size(160, 25);
            this.btnCrearBackup.TabIndex = 3;
            this.btnCrearBackup.Tag = "FormBackup.CreateButton";
            this.btnCrearBackup.Click += new System.EventHandler(this.btnCrearBackup_Click);
            //
            // groupBoxRestaurarBackup
            //
            this.groupBoxRestaurarBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRestaurarBackup.Panel.Controls.Add(this.btnRestaurarBackup);
            this.groupBoxRestaurarBackup.Panel.Controls.Add(this.btnSeleccionarRutaRestaurar);
            this.groupBoxRestaurarBackup.Panel.Controls.Add(this.txtbRutaRestaurarBackup);
            this.groupBoxRestaurarBackup.Panel.Controls.Add(this.lblRutaRestaurarBackup);
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
            this.lblRutaRestaurarBackup.Location = new System.Drawing.Point(15, 8);
            this.lblRutaRestaurarBackup.Name = "lblRutaRestaurarBackup";
            this.lblRutaRestaurarBackup.Size = new System.Drawing.Size(0, 13);
            this.lblRutaRestaurarBackup.TabIndex = 0;
            this.lblRutaRestaurarBackup.Tag = "FormBackup.RestorePathLabel";
            //
            // txtbRutaRestaurarBackup
            //
            this.txtbRutaRestaurarBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbRutaRestaurarBackup.Location = new System.Drawing.Point(18, 32);
            this.txtbRutaRestaurarBackup.Name = "txtbRutaRestaurarBackup";
            this.txtbRutaRestaurarBackup.ReadOnly = true;
            this.txtbRutaRestaurarBackup.Size = new System.Drawing.Size(370, 20);
            this.txtbRutaRestaurarBackup.TabIndex = 1;
            //
            // btnSeleccionarRutaRestaurar
            //
            this.btnSeleccionarRutaRestaurar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSeleccionarRutaRestaurar.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnSeleccionarRutaRestaurar.Location = new System.Drawing.Point(394, 30);
            this.btnSeleccionarRutaRestaurar.Name = "btnSeleccionarRutaRestaurar";
            this.btnSeleccionarRutaRestaurar.Size = new System.Drawing.Size(150, 25);
            this.btnSeleccionarRutaRestaurar.TabIndex = 2;
            this.btnSeleccionarRutaRestaurar.Tag = "FormBackup.BrowseRestoreButton";
            this.btnSeleccionarRutaRestaurar.Click += new System.EventHandler(this.btnSeleccionarRutaRestaurar_Click);
            //
            // btnRestaurarBackup
            //
            this.btnRestaurarBackup.Enabled = false;
            this.btnRestaurarBackup.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnRestaurarBackup.Location = new System.Drawing.Point(18, 62);
            this.btnRestaurarBackup.Name = "btnRestaurarBackup";
            this.btnRestaurarBackup.Size = new System.Drawing.Size(160, 25);
            this.btnRestaurarBackup.TabIndex = 3;
            this.btnRestaurarBackup.Tag = "FormBackup.RestoreButton";
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
            this.Name = "FormBackup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormBackup.Title";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCrearBackup.Panel)).EndInit();
            this.groupBoxCrearBackup.Panel.ResumeLayout(false);
            this.groupBoxCrearBackup.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCrearBackup)).EndInit();
            this.groupBoxCrearBackup.ResumeLayout(false);
            this.groupBoxCrearBackup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRestaurarBackup.Panel)).EndInit();
            this.groupBoxRestaurarBackup.Panel.ResumeLayout(false);
            this.groupBoxRestaurarBackup.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRestaurarBackup)).EndInit();
            this.groupBoxRestaurarBackup.ResumeLayout(false);
            this.groupBoxRestaurarBackup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblTituloBackup;
        private Krypton.Toolkit.KryptonGroupBox groupBoxCrearBackup;
        private Krypton.Toolkit.KryptonLabel lblRutaCrearBackup;
        private Krypton.Toolkit.KryptonTextBox txtbRutaCrearBackup;
        private Krypton.Toolkit.KryptonButton btnSeleccionarRutaCrear;
        private Krypton.Toolkit.KryptonButton btnCrearBackup;
        private Krypton.Toolkit.KryptonGroupBox groupBoxRestaurarBackup;
        private Krypton.Toolkit.KryptonLabel lblRutaRestaurarBackup;
        private Krypton.Toolkit.KryptonTextBox txtbRutaRestaurarBackup;
        private Krypton.Toolkit.KryptonButton btnSeleccionarRutaRestaurar;
        private Krypton.Toolkit.KryptonButton btnRestaurarBackup;
    }
}
