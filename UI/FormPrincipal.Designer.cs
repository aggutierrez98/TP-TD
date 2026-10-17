namespace TpIDS
{
    partial class FormPrincipal
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
            this.menuStripPrincipal = new System.Windows.Forms.MenuStrip();
            this.mnuSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMiConfiguracion = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorSesion = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCerrarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFinanzas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMovimientos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCategorias = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReglasCategorizacion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuListaCompras = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdministracion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPermisos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIdiomas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBitacora = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOperador = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNotificaciones = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComercios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMonedas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOperadorMediosPago = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOperadorEntidadesFinancieras = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFinanzasCuentasFinancieras = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAsistentePersonal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAsistentePersonalAbrir = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnaliticas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnaliticasDashboard = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnaliticasBalanceGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripPrincipal.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStripPrincipal
            //
            this.menuStripPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSesion,
            this.mnuFinanzas,
            this.mnuAdministracion,
            this.mnuOperador,
            this.mnuAsistentePersonal,
            this.mnuAnaliticas});
            this.menuStripPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuStripPrincipal.Name = "menuStripPrincipal";
            this.menuStripPrincipal.Size = new System.Drawing.Size(1000, 24);
            this.menuStripPrincipal.TabIndex = 0;
            //
            // mnuSesion
            //
            this.mnuSesion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMiConfiguracion,
            this.toolStripSeparatorSesion,
            this.mnuCerrarSesion});
            this.mnuSesion.Name = "mnuSesion";
            this.mnuSesion.Size = new System.Drawing.Size(58, 20);
            this.mnuSesion.Tag = "FormMainShell.Menu.Session";
            this.mnuSesion.Text = "Sesion";
            //
            // mnuMiConfiguracion
            //
            this.mnuMiConfiguracion.Name = "mnuMiConfiguracion";
            this.mnuMiConfiguracion.Size = new System.Drawing.Size(180, 22);
            this.mnuMiConfiguracion.Tag = "FormMainShell.Menu.MyConfiguration";
            this.mnuMiConfiguracion.Text = "Mi configuracion";
            this.mnuMiConfiguracion.Click += new System.EventHandler(this.mnuMiConfiguracion_Click);
            //
            // toolStripSeparatorSesion
            //
            this.toolStripSeparatorSesion.Name = "toolStripSeparatorSesion";
            this.toolStripSeparatorSesion.Size = new System.Drawing.Size(177, 6);
            //
            // mnuCerrarSesion
            //
            this.mnuCerrarSesion.Name = "mnuCerrarSesion";
            this.mnuCerrarSesion.Size = new System.Drawing.Size(180, 22);
            this.mnuCerrarSesion.Tag = "FormMainShell.Menu.Logout";
            this.mnuCerrarSesion.Text = "Cerrar sesion";
            this.mnuCerrarSesion.Click += new System.EventHandler(this.mnuCerrarSesion_Click);
            //
            // mnuFinanzas
            //
            this.mnuFinanzas.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMovimientos,
            this.mnuCategorias,
            this.mnuReglasCategorizacion,
            this.mnuListaCompras,
            this.mnuFinanzasCuentasFinancieras});
            this.mnuFinanzas.Name = "mnuFinanzas";
            this.mnuFinanzas.Size = new System.Drawing.Size(65, 20);
            this.mnuFinanzas.Tag = "FormMainShell.Menu.Finance";
            this.mnuFinanzas.Text = "Finanzas";
            // 
            // mnuMovimientos
            // 
            this.mnuMovimientos.Name = "mnuMovimientos";
            this.mnuMovimientos.Size = new System.Drawing.Size(200, 22);
            this.mnuMovimientos.Tag = "FormMainShell.Menu.Movements";
            this.mnuMovimientos.Text = "Movimientos";
            this.mnuMovimientos.Click += new System.EventHandler(this.mnuMovimientos_Click);
            // 
            // mnuCategorias
            // 
            this.mnuCategorias.Name = "mnuCategorias";
            this.mnuCategorias.Size = new System.Drawing.Size(200, 22);
            this.mnuCategorias.Tag = "FormMainShell.Menu.Categories";
            this.mnuCategorias.Text = "Categorias";
            this.mnuCategorias.Click += new System.EventHandler(this.mnuCategorias_Click);
            // 
            // mnuReglasCategorizacion
            // 
            this.mnuReglasCategorizacion.Name = "mnuReglasCategorizacion";
            this.mnuReglasCategorizacion.Size = new System.Drawing.Size(200, 22);
            this.mnuReglasCategorizacion.Tag = "FormMainShell.Menu.CategorizationRules";
            this.mnuReglasCategorizacion.Text = "Reglas de categorizacion";
            this.mnuReglasCategorizacion.Click += new System.EventHandler(this.mnuReglasCategorizacion_Click);
            //
            // mnuListaCompras
            //
            this.mnuListaCompras.Name = "mnuListaCompras";
            this.mnuListaCompras.Size = new System.Drawing.Size(180, 22);
            this.mnuListaCompras.Tag = "FormMainShell.Menu.ShoppingList";
            this.mnuListaCompras.Text = "Lista de compras";
            this.mnuListaCompras.Click += new System.EventHandler(this.mnuListaCompras_Click);
            //
            // mnuFinanzasCuentasFinancieras
            //
            this.mnuFinanzasCuentasFinancieras.Name = "mnuFinanzasCuentasFinancieras";
            this.mnuFinanzasCuentasFinancieras.Size = new System.Drawing.Size(200, 22);
            this.mnuFinanzasCuentasFinancieras.Tag = "FormMainShell.Menu.FinancialAccounts";
            this.mnuFinanzasCuentasFinancieras.Text = "Mis cuentas financieras";
            this.mnuFinanzasCuentasFinancieras.Click += new System.EventHandler(this.mnuFinanzasCuentasFinancieras_Click);
            //
            // mnuAdministracion
            //
            this.mnuAdministracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUsuarios,
            this.mnuPermisos,
            this.mnuIdiomas,
            this.mnuBitacora,
            this.mnuBackup});
            this.mnuAdministracion.Name = "mnuAdministracion";
            this.mnuAdministracion.Size = new System.Drawing.Size(107, 20);
            this.mnuAdministracion.Tag = "FormMainShell.Menu.Administration";
            this.mnuAdministracion.Text = "Administracion";
            //
            // mnuUsuarios
            //
            this.mnuUsuarios.Name = "mnuUsuarios";
            this.mnuUsuarios.Size = new System.Drawing.Size(180, 22);
            this.mnuUsuarios.Tag = "FormMainShell.Menu.Users";
            this.mnuUsuarios.Text = "Usuarios";
            this.mnuUsuarios.Click += new System.EventHandler(this.mnuUsuarios_Click);
            //
            // mnuPermisos
            //
            this.mnuPermisos.Name = "mnuPermisos";
            this.mnuPermisos.Size = new System.Drawing.Size(180, 22);
            this.mnuPermisos.Tag = "FormMainShell.Menu.Permissions";
            this.mnuPermisos.Text = "Permisos";
            this.mnuPermisos.Click += new System.EventHandler(this.mnuPermisos_Click);
            //
            // mnuIdiomas
            //
            this.mnuIdiomas.Name = "mnuIdiomas";
            this.mnuIdiomas.Size = new System.Drawing.Size(180, 22);
            this.mnuIdiomas.Tag = "FormMainShell.Menu.Languages";
            this.mnuIdiomas.Text = "Idiomas";
            this.mnuIdiomas.Click += new System.EventHandler(this.mnuIdiomas_Click);
            //
            // mnuBitacora
            //
            this.mnuBitacora.Name = "mnuBitacora";
            this.mnuBitacora.Size = new System.Drawing.Size(180, 22);
            this.mnuBitacora.Tag = "FormMainShell.Menu.AuditLog";
            this.mnuBitacora.Text = "Bitacora";
            this.mnuBitacora.Click += new System.EventHandler(this.mnuBitacora_Click);
            //
            // mnuBackup
            //
            this.mnuBackup.Name = "mnuBackup";
            this.mnuBackup.Size = new System.Drawing.Size(180, 22);
            this.mnuBackup.Tag = "FormMainShell.Menu.Backup";
            this.mnuBackup.Text = "Backup";
            this.mnuBackup.Click += new System.EventHandler(this.mnuBackup_Click);
            //
            // mnuOperador
            //
            this.mnuOperador.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNotificaciones,
            this.mnuComercios,
            this.mnuMonedas,
            this.mnuOperadorMediosPago,
            this.mnuOperadorEntidadesFinancieras});
            this.mnuOperador.Name = "mnuOperador";
            this.mnuOperador.Size = new System.Drawing.Size(76, 20);
            this.mnuOperador.Tag = "FormMainShell.Menu.Operator";
            this.mnuOperador.Text = "Operador";
            //
            // mnuNotificaciones
            //
            this.mnuNotificaciones.Name = "mnuNotificaciones";
            this.mnuNotificaciones.Size = new System.Drawing.Size(180, 22);
            this.mnuNotificaciones.Tag = "FormMainShell.Menu.Notifications";
            this.mnuNotificaciones.Text = "Notificaciones";
            this.mnuNotificaciones.Click += new System.EventHandler(this.mnuNotificaciones_Click);
            //
            // mnuComercios
            //
            this.mnuComercios.Name = "mnuComercios";
            this.mnuComercios.Size = new System.Drawing.Size(180, 22);
            this.mnuComercios.Tag = "FormMainShell.Menu.Merchants";
            this.mnuComercios.Text = "Comercios";
            this.mnuComercios.Click += new System.EventHandler(this.mnuComercios_Click);
            //
            // mnuMonedas
            //
            this.mnuMonedas.Name = "mnuMonedas";
            this.mnuMonedas.Size = new System.Drawing.Size(180, 22);
            this.mnuMonedas.Tag = "FormMainShell.Menu.Currencies";
            this.mnuMonedas.Text = "Monedas";
            this.mnuMonedas.Click += new System.EventHandler(this.mnuMonedas_Click);
            //
            // mnuOperadorMediosPago
            //
            this.mnuOperadorMediosPago.Name = "mnuOperadorMediosPago";
            this.mnuOperadorMediosPago.Size = new System.Drawing.Size(200, 22);
            this.mnuOperadorMediosPago.Tag = "FormMainShell.Menu.PaymentMethods";
            this.mnuOperadorMediosPago.Text = "Medios de pago";
            this.mnuOperadorMediosPago.Click += new System.EventHandler(this.mnuOperadorMediosPago_Click);
            //
            // mnuOperadorEntidadesFinancieras
            //
            this.mnuOperadorEntidadesFinancieras.Name = "mnuOperadorEntidadesFinancieras";
            this.mnuOperadorEntidadesFinancieras.Size = new System.Drawing.Size(200, 22);
            this.mnuOperadorEntidadesFinancieras.Tag = "FormMainShell.Menu.FinancialEntities";
            this.mnuOperadorEntidadesFinancieras.Text = "Entidades financieras";
            this.mnuOperadorEntidadesFinancieras.Click += new System.EventHandler(this.mnuOperadorEntidadesFinancieras_Click);
            //
            // mnuAsistentePersonal
            //
            this.mnuAsistentePersonal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAsistentePersonalAbrir});
            this.mnuAsistentePersonal.Name = "mnuAsistentePersonal";
            this.mnuAsistentePersonal.Size = new System.Drawing.Size(126, 20);
            this.mnuAsistentePersonal.Tag = "FormMainShell.Menu.PersonalAssistant";
            this.mnuAsistentePersonal.Text = "Asistente Personal";
            //
            // mnuAsistentePersonalAbrir
            //
            this.mnuAsistentePersonalAbrir.Name = "mnuAsistentePersonalAbrir";
            this.mnuAsistentePersonalAbrir.Size = new System.Drawing.Size(180, 22);
            this.mnuAsistentePersonalAbrir.Tag = "FormMainShell.Menu.PersonalAssistantChat";
            this.mnuAsistentePersonalAbrir.Text = "Chatear con el asistente";
            this.mnuAsistentePersonalAbrir.Click += new System.EventHandler(this.mnuAsistentePersonalAbrir_Click);
            //
            // mnuAnaliticas
            //
            this.mnuAnaliticas.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAnaliticasDashboard,
            this.mnuAnaliticasBalanceGeneral});
            this.mnuAnaliticas.Name = "mnuAnaliticas";
            this.mnuAnaliticas.Size = new System.Drawing.Size(76, 20);
            this.mnuAnaliticas.Tag = "FormMainShell.Menu.Analytics";
            this.mnuAnaliticas.Text = "Analiticas";
            //
            // mnuAnaliticasDashboard
            //
            this.mnuAnaliticasDashboard.Name = "mnuAnaliticasDashboard";
            this.mnuAnaliticasDashboard.Size = new System.Drawing.Size(180, 22);
            this.mnuAnaliticasDashboard.Tag = "FormMainShell.Menu.AnalyticsDashboard";
            this.mnuAnaliticasDashboard.Text = "Dashboard";
            this.mnuAnaliticasDashboard.Click += new System.EventHandler(this.mnuAnaliticasDashboard_Click);
            //
            // mnuAnaliticasBalanceGeneral
            //
            this.mnuAnaliticasBalanceGeneral.Name = "mnuAnaliticasBalanceGeneral";
            this.mnuAnaliticasBalanceGeneral.Size = new System.Drawing.Size(180, 22);
            this.mnuAnaliticasBalanceGeneral.Tag = "FormMainShell.Menu.AnalyticsBalanceSheet";
            this.mnuAnaliticasBalanceGeneral.Text = "Balance General";
            this.mnuAnaliticasBalanceGeneral.Click += new System.EventHandler(this.mnuAnaliticasBalanceGeneral_Click);
            //
            // FormPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.menuStripPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStripPrincipal;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "FormMainShell.Title";
            this.Text = "FINAZAPP";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStripPrincipal.ResumeLayout(false);
            this.menuStripPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.MenuStrip menuStripPrincipal;
        private System.Windows.Forms.ToolStripMenuItem mnuSesion;
        private System.Windows.Forms.ToolStripMenuItem mnuMiConfiguracion;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorSesion;
        private System.Windows.Forms.ToolStripMenuItem mnuCerrarSesion;
        private System.Windows.Forms.ToolStripMenuItem mnuFinanzas;
        private System.Windows.Forms.ToolStripMenuItem mnuMovimientos;
        private System.Windows.Forms.ToolStripMenuItem mnuCategorias;
        private System.Windows.Forms.ToolStripMenuItem mnuReglasCategorizacion;
        private System.Windows.Forms.ToolStripMenuItem mnuListaCompras;
        private System.Windows.Forms.ToolStripMenuItem mnuAdministracion;
        private System.Windows.Forms.ToolStripMenuItem mnuUsuarios;
        private System.Windows.Forms.ToolStripMenuItem mnuPermisos;
        private System.Windows.Forms.ToolStripMenuItem mnuIdiomas;
        private System.Windows.Forms.ToolStripMenuItem mnuBitacora;
        private System.Windows.Forms.ToolStripMenuItem mnuBackup;
        private System.Windows.Forms.ToolStripMenuItem mnuOperador;
        private System.Windows.Forms.ToolStripMenuItem mnuNotificaciones;
        private System.Windows.Forms.ToolStripMenuItem mnuComercios;
        private System.Windows.Forms.ToolStripMenuItem mnuMonedas;
        private System.Windows.Forms.ToolStripMenuItem mnuOperadorMediosPago;
        private System.Windows.Forms.ToolStripMenuItem mnuOperadorEntidadesFinancieras;
        private System.Windows.Forms.ToolStripMenuItem mnuFinanzasCuentasFinancieras;
        private System.Windows.Forms.ToolStripMenuItem mnuAsistentePersonal;
        private System.Windows.Forms.ToolStripMenuItem mnuAsistentePersonalAbrir;
        private System.Windows.Forms.ToolStripMenuItem mnuAnaliticas;
        private System.Windows.Forms.ToolStripMenuItem mnuAnaliticasDashboard;
        private System.Windows.Forms.ToolStripMenuItem mnuAnaliticasBalanceGeneral;
    }
}
