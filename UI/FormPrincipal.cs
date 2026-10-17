using System;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;

namespace TpIDS
{
    public partial class FormPrincipal : FormTraducibleBase
    {
        private const string PermisoConfiguracion = "configuracionusuario";
        private const string PermisoGestionMovimientos = "gestionmovimientos";
        private const string PermisoGestionCategorias = "gestioncategorias";
        private const string PermisoGestionListaCompras = "gestionlistacompras";
        private const string PermisoGestionUsuarios = "gestionusuarios";
        private const string PermisoGestionPermisos = "gestionpermisos";
        private const string PermisoGestionIdiomas = "gestionidiomas";
        private const string PermisoBitacora = "verbitacora";
        private const string PermisoGestionBackup = "gestionbackup";
        private const string PermisoGestionNotificaciones = "gestionnotificaciones";
        private const string PermisoGestionComercios = "gestioncomercios";
        private const string PermisoGestionMonedas = "gestionmonedas";
        private const string PermisoGestionReglasCategorizacion = "gestionreglascategorizacion";
        private const string PermisoUsarAsistentePersonal = "usarasistentepersonal";
        private const string PermisoVerDashboardAnaliticas = "verdashboardanaliticas";
        private const string PermisoVerBalanceGeneral = "verbalancegeneral";
        private const string PermisoGestionMediosPago = "gestionmediospago";
        private const string PermisoGestionEntidadesFinancieras = "gestionentidadesfinancieras";
        private const string PermisoGestionCuentasFinancieras = "gestioncuentasfinancieras";

        private readonly IAutorizacion autorizacion = CompositionRoot.Autorizacion;
        private readonly UsuarioBLL usuarioBLL = CompositionRoot.Usuario;

        public FormPrincipal()
        {
            InitializeComponent();
            Load += FormPrincipal_Load;
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            AplicarPermisosMenu();
            AbrirPantallaInicial();
        }

        private void AplicarPermisosMenu()
        {
            bool puedeConfigurar = autorizacion.TienePermiso(PermisoConfiguracion);
            bool puedeMovimientos = autorizacion.TienePermiso(PermisoGestionMovimientos);
            bool puedeCategorias = autorizacion.TienePermiso(PermisoGestionCategorias);
            bool puedeReglas = autorizacion.TienePermiso(PermisoGestionReglasCategorizacion);
            bool puedeListaCompras = autorizacion.TienePermiso(PermisoGestionListaCompras);
            bool puedeUsuarios = autorizacion.TienePermiso(PermisoGestionUsuarios);
            bool puedePermisos = autorizacion.TienePermiso(PermisoGestionPermisos);
            bool puedeIdiomas = autorizacion.TienePermiso(PermisoGestionIdiomas);
            bool puedeBitacora = autorizacion.TienePermiso(PermisoBitacora);
            bool puedeBackup = autorizacion.TienePermiso(PermisoGestionBackup);
            bool puedeNotificaciones = autorizacion.TienePermiso(PermisoGestionNotificaciones);
            bool puedeComercios = autorizacion.TienePermiso(PermisoGestionComercios);
            bool puedeMonedas = autorizacion.TienePermiso(PermisoGestionMonedas);
            bool puedeAsistentePersonal = autorizacion.TienePermiso(PermisoUsarAsistentePersonal);
            bool puedeDashboard = autorizacion.TienePermiso(PermisoVerDashboardAnaliticas);
            bool puedeBalanceGeneral = autorizacion.TienePermiso(PermisoVerBalanceGeneral);
            bool puedeMediosPago = autorizacion.TienePermiso(PermisoGestionMediosPago);
            bool puedeEntidadesFinancieras = autorizacion.TienePermiso(PermisoGestionEntidadesFinancieras);
            bool puedeCuentasFinancieras = autorizacion.TienePermiso(PermisoGestionCuentasFinancieras);

            mnuMiConfiguracion.Enabled = puedeConfigurar;

            mnuMovimientos.Enabled = puedeMovimientos;
            mnuCategorias.Enabled = puedeCategorias;
            mnuReglasCategorizacion.Enabled = puedeReglas;
            mnuListaCompras.Enabled = puedeListaCompras;
            mnuFinanzasCuentasFinancieras.Enabled = puedeCuentasFinancieras;
            mnuFinanzas.Enabled = puedeMovimientos || puedeCategorias || puedeReglas || puedeListaCompras || puedeCuentasFinancieras;

            mnuUsuarios.Enabled = puedeUsuarios;
            mnuPermisos.Enabled = puedePermisos;
            mnuIdiomas.Enabled = puedeIdiomas;
            mnuBitacora.Enabled = puedeBitacora;
            mnuBackup.Enabled = puedeBackup;
            mnuAdministracion.Enabled = puedeUsuarios || puedePermisos || puedeIdiomas || puedeBitacora || puedeBackup;

            mnuNotificaciones.Enabled = puedeNotificaciones;
            mnuComercios.Enabled = puedeComercios;
            mnuMonedas.Enabled = puedeMonedas;
            mnuOperadorMediosPago.Enabled = puedeMediosPago;
            mnuOperadorEntidadesFinancieras.Enabled = puedeEntidadesFinancieras;
            mnuOperador.Enabled = puedeNotificaciones || puedeComercios || puedeMonedas || puedeMediosPago || puedeEntidadesFinancieras;

            mnuAsistentePersonalAbrir.Enabled = puedeAsistentePersonal;
            mnuAsistentePersonal.Enabled = puedeAsistentePersonal;

            mnuAnaliticas.Enabled = puedeDashboard || puedeBalanceGeneral;
            mnuAnaliticasDashboard.Enabled = puedeDashboard;
            mnuAnaliticasBalanceGeneral.Enabled = puedeBalanceGeneral;
        }

        // Pantalla de aterrizaje: el nucleo de negocio (movimientos) si el usuario tiene acceso.
        private void AbrirPantallaInicial()
        {
            if (mnuMovimientos.Enabled)
            {
                AbrirHijoMdiUnico<FormMovimientos>();
            }
        }

        private void mnuMiConfiguracion_Click(object sender, EventArgs e)
        {
            new FormConfiguracionUsuario().ShowDialog(this);
        }

        private void mnuCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMainShell.Confirm.LogoutMessage",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            usuarioBLL.CerrarSesionActual();
            Close();
        }

        private void mnuMovimientos_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormMovimientos>();
        }

        private void mnuCategorias_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionCategorias>();
        }

        private void mnuReglasCategorizacion_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormReglasCategorizacion>();
        }

        private void mnuListaCompras_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormListaDeCompras>();
        }

        private void mnuUsuarios_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionUsuarios>();
        }

        private void mnuPermisos_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionPermisos>();
        }

        private void mnuIdiomas_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionIdiomas>();
        }

        private void mnuBitacora_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormBitacora>();
        }

        private void mnuBackup_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormBackup>();
        }

        private void mnuNotificaciones_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionNotificaciones>();
        }

        private void mnuComercios_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionComercios>();
        }

        private void mnuMonedas_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionMonedas>();
        }

        private void mnuOperadorMediosPago_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionMediosPago>();
        }

        private void mnuOperadorEntidadesFinancieras_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionEntidadesFinancieras>();
        }

        private void mnuFinanzasCuentasFinancieras_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormGestionCuentasFinancieras>();
        }

        private void mnuAsistentePersonalAbrir_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormAsistentePersonal>();
        }

        private void mnuAnaliticasDashboard_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormDashboardAnaliticas>();
        }

        private void mnuAnaliticasBalanceGeneral_Click(object sender, EventArgs e)
        {
            AbrirHijoMdiUnico<FormBalanceGeneral>();
        }

        // Si ya hay un hijo MDI del tipo T abierto, lo enfoca en vez de abrir uno nuevo
        // (evita duplicar pantallas al hacer doble click en el menu).
        private void AbrirHijoMdiUnico<T>() where T : Form, new()
        {
            foreach (Form hijo in MdiChildren)
            {
                if (hijo is T existente)
                {
                    if (existente.WindowState == FormWindowState.Minimized)
                    {
                        existente.WindowState = FormWindowState.Normal;
                    }

                    existente.Activate();
                    return;
                }
            }

            T nuevo = new T
            {
                MdiParent = this
            };
            nuevo.Show();
        }
    }
}
