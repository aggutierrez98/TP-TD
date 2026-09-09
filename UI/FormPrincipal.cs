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
        private const string PermisoGestionUsuarios = "gestionusuarios";
        private const string PermisoGestionPermisos = "gestionpermisos";
        private const string PermisoGestionIdiomas = "gestionidiomas";
        private const string PermisoBitacora = "verbitacora";
        private const string PermisoGestionBackup = "gestionbackup";

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
            bool puedeUsuarios = autorizacion.TienePermiso(PermisoGestionUsuarios);
            bool puedePermisos = autorizacion.TienePermiso(PermisoGestionPermisos);
            bool puedeIdiomas = autorizacion.TienePermiso(PermisoGestionIdiomas);
            bool puedeBitacora = autorizacion.TienePermiso(PermisoBitacora);
            bool puedeBackup = autorizacion.TienePermiso(PermisoGestionBackup);

            mnuMiConfiguracion.Enabled = puedeConfigurar;

            mnuMovimientos.Enabled = puedeMovimientos;
            mnuCategorias.Enabled = puedeCategorias;
            mnuFinanzas.Enabled = puedeMovimientos || puedeCategorias;

            mnuUsuarios.Enabled = puedeUsuarios;
            mnuPermisos.Enabled = puedePermisos;
            mnuIdiomas.Enabled = puedeIdiomas;
            mnuBitacora.Enabled = puedeBitacora;
            mnuBackup.Enabled = puedeBackup;
            mnuAdministracion.Enabled = puedeUsuarios || puedePermisos || puedeIdiomas || puedeBitacora || puedeBackup;
        }

        // Pantalla de aterrizaje: el nucleo de negocio (movimientos) si el usuario tiene acceso.
        private void AbrirPantallaInicial()
        {
            if (mnuMovimientos.Enabled)
            {
                AbrirHijoMdi(new FormMovimientos());
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
            AbrirHijoMdi(new FormMovimientos());
        }

        private void mnuCategorias_Click(object sender, EventArgs e)
        {
            AbrirHijoMdi(new FormGestionCategorias());
        }

        private void mnuUsuarios_Click(object sender, EventArgs e)
        {
            AbrirHijoMdi(new FormGestionUsuarios());
        }

        private void mnuPermisos_Click(object sender, EventArgs e)
        {
            AbrirHijoMdi(new FormGestionPermisos());
        }

        private void mnuIdiomas_Click(object sender, EventArgs e)
        {
            AbrirHijoMdi(new FormGestionIdiomas());
        }

        private void mnuBitacora_Click(object sender, EventArgs e)
        {
            AbrirHijoMdi(new FormBitacora());
        }

        private void mnuBackup_Click(object sender, EventArgs e)
        {
            AbrirHijoMdi(new FormBackup());
        }

        private void AbrirHijoMdi(Form hijo)
        {
            hijo.MdiParent = this;
            hijo.Show();
        }
    }
}
