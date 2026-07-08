using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionUsuarios : FormTraducibleBase
    {
        private const string PermisoBitacora = "verbitacora";
        private const string PermisoConfiguracion = "configuracionusuario";
        private const string PermisoGestionIdiomas = "gestionidiomas";
        private const string PermisoGestionPermisos = "gestionpermisos";
        private const string PermisoGestionUsuarios = "gestionusuarios";
        private const string PermisoHistorialUsuarios = "historialusuarios";

        private readonly UsuarioBLL usuarioBLL = new UsuarioBLL();
        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private Usuario _usuarioSeleccionado;
        private bool _cargandoGrillaUsuarios;

        public FormGestionUsuarios()
        {
            InitializeComponent();
        }

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            chkbUsuarioActivo.Checked = true;
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
            AplicarPermisosPantalla();
            RefrescarDatos();
        }

        private void RefrescarDatos()
        {
            int idAnterior = _usuarioSeleccionado?.Id ?? -1;
            _cargandoGrillaUsuarios = true;

            try
            {
                List<Usuario> usuarios = usuarioBLL.ObtenerTodos();
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = usuarios;
                ReseleccionarUsuario(dgvUsuarios, idAnterior);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de usuarios");
            }
            finally
            {
                _cargandoGrillaUsuarios = false;
                AplicarSeleccionActualDesdeGrilla();
            }
        }

        private void LimpiarSeleccion()
        {
            _usuarioSeleccionado = null;
            txtbNombreUsuario.Clear();
            txtbPasswordUsuario.Clear();
            chkbUsuarioActivo.Checked = true;
            dgvUsuarios.ClearSelection();
            tvwPermisosUsuario.Nodes.Clear();
        }

        private void AplicarPermisosPantalla()
        {
            bool puedeGestionarUsuarios = AutorizacionService.GetInstance().TienePermiso(PermisoGestionUsuarios);
            bool puedeGestionarPermisos = AutorizacionService.GetInstance().TienePermiso(PermisoGestionPermisos);
            bool puedeVerHistorial = AutorizacionService.GetInstance().TienePermiso(PermisoHistorialUsuarios);
            bool puedeVerBitacora = AutorizacionService.GetInstance().TienePermiso(PermisoBitacora);
            bool puedeConfigurarUsuario = AutorizacionService.GetInstance().TienePermiso(PermisoConfiguracion);
            bool puedeGestionarIdiomas = AutorizacionService.GetInstance().TienePermiso(PermisoGestionIdiomas);

            btnAgregarUsuario.Enabled = puedeGestionarUsuarios;
            btnEditarUsuario.Enabled = puedeGestionarUsuarios;
            btnEliminarUsuario.Enabled = puedeGestionarUsuarios;
            groupBoxPermisosUsuario.Enabled = puedeGestionarUsuarios;
            btnGuardarPermisosUsuario.Enabled = puedeGestionarUsuarios;
            btnGestionPermisos.Enabled = puedeGestionarPermisos;
            btnHistorialUsuario.Enabled = puedeVerHistorial;
            btnBitacora.Enabled = puedeVerBitacora;
            btnConfiguracion.Enabled = puedeConfigurarUsuario;
            btnGestionIdiomas.Enabled = puedeGestionarIdiomas;
        }

        private bool ValidarPermiso(string permiso)
        {
            if (AutorizacionService.GetInstance().TienePermiso(permiso))
            {
                return true;
            }

            UiDialogHelper.MostrarWarning("FormUserManagement.Warning.NotPermitted");
            return false;
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            // Durante el rebinding la grilla dispara eventos intermedios antes de terminar de reconstruir columnas y filas.
            if (_cargandoGrillaUsuarios)
            {
                return;
            }

            AplicarSeleccionActualDesdeGrilla();
        }

        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoGestionUsuarios))
            {
                return;
            }

            string username = txtbNombreUsuario.Text.Trim();
            string password = txtbPasswordUsuario.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormUserManagement.Warning.CreateIncompleteData",
                    "FormUserManagement.Warning.CreateIncompleteDataTitle");
                return;
            }

            try
            {
                if (usuarioBLL.Registrar(username, password))
                {
                    UiDialogHelper.MostrarInfo("FormUserManagement.Success.UserCreated");
                    LimpiarSeleccion();
                    RefrescarDatos();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de usuario");
            }
        }

        private void btnEditarUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoGestionUsuarios))
            {
                return;
            }

            if (_usuarioSeleccionado == null)
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormUserManagement.Warning.NoUserSelectedEdit",
                    "FormUserManagement.Warning.NoUserSelectedTitle");
                return;
            }

            string nuevoUsername = txtbNombreUsuario.Text.Trim();
            string nuevaPassword = txtbPasswordUsuario.Text.Trim();
            if (string.IsNullOrWhiteSpace(nuevoUsername))
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormUserManagement.Warning.UsernameRequired",
                    "FormUserManagement.Warning.UsernameRequiredTitle");
                return;
            }

            try
            {
                bool usuarioEstabaActivo = _usuarioSeleccionado.Activo;
                bool usuarioActivo = chkbUsuarioActivo.Checked;

                if (usuarioEstabaActivo && !usuarioActivo)
                {
                    DialogResult confirmacionDesactivacion = UiDialogHelper.Confirmar(
                        "FormUserManagement.Confirm.DeactivateUserMessage",
                        "FormUserManagement.Confirm.DeactivateUserTitle",
                        MessageBoxIcon.Warning);

                    if (confirmacionDesactivacion != DialogResult.Yes)
                    {
                        return;
                    }
                }

                Usuario usuarioEditado = new Usuario
                {
                    Id = _usuarioSeleccionado.Id,
                    Username = nuevoUsername,
                    Activo = usuarioActivo
                };

                if (usuarioBLL.Actualizar(usuarioEditado, nuevaPassword))
                {
                    ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
                    string successKey = !usuarioEstabaActivo && usuarioActivo
                        ? "FormUserManagement.Success.UserActivated"
                        : usuarioEstabaActivo && !usuarioActivo
                            ? "FormUserManagement.Success.UserDeactivated"
                            : "FormUserManagement.Success.UserUpdated";
                    UiDialogHelper.MostrarInfo(successKey);
                    LimpiarSeleccion();
                    RefrescarDatos();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de usuario");
            }
        }

        private void btnEliminarUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoGestionUsuarios))
            {
                return;
            }

            if (_usuarioSeleccionado == null)
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormUserManagement.Warning.NoUserSelectedDelete",
                    "FormUserManagement.Warning.NoUserSelectedTitle");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormUserManagement.Confirm.PhysicalDeleteMessage",
                "FormUserManagement.Confirm.PhysicalDeleteTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (usuarioBLL.Eliminar(_usuarioSeleccionado))
                {
                    UiDialogHelper.MostrarInfo("FormUserManagement.Success.UserDeleted");
                    LimpiarSeleccion();
                    RefrescarDatos();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Borrado fisico de usuario");
            }
        }

        private void CargarPermisosUsuario()
        {
            tvwPermisosUsuario.Nodes.Clear();
            if (_usuarioSeleccionado == null || !AutorizacionService.GetInstance().TienePermiso(PermisoGestionUsuarios))
            {
                return;
            }

            try
            {
                List<IPermiso> catalogo = permisoBLL.ObtenerCatalogo();
                HashSet<int> permisosAsignados = new HashSet<int>(permisoBLL.ObtenerPermisoIdsDeUsuario(_usuarioSeleccionado.Id));

                foreach (IPermiso raiz in catalogo)
                {
                    tvwPermisosUsuario.Nodes.Add(CrearNodoPermiso(raiz, permisosAsignados));
                }

                tvwPermisosUsuario.ExpandAll();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de permisos del usuario");
            }
        }

        private TreeNode CrearNodoPermiso(IPermiso permiso, ISet<int> permisosAsignados)
        {
            TreeNode nodo = new TreeNode(permiso.Nombre)
            {
                Tag = permiso,
                Checked = permisosAsignados.Contains(permiso.Id)
            };

            foreach (IPermiso hijo in permiso.Hijos)
            {
                nodo.Nodes.Add(CrearNodoPermiso(hijo, permisosAsignados));
            }

            return nodo;
        }

        private void btnGuardarPermisosUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoGestionUsuarios))
            {
                return;
            }

            if (_usuarioSeleccionado == null)
            {
                UiDialogHelper.MostrarWarning("FormUserManagement.Warning.SelectUserPermissions");
                return;
            }

            try
            {
                List<IPermiso> permisosSeleccionados = ObtenerPermisosSeleccionados(tvwPermisosUsuario.Nodes)
                    .Distinct()
                    .ToList();

                permisoBLL.GuardarPermisosUsuario(_usuarioSeleccionado, permisosSeleccionados);
                UiDialogHelper.MostrarInfo("FormUserManagement.Success.UserPermissionsSaved");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de permisos de usuario");
            }
        }

        private List<IPermiso> ObtenerPermisosSeleccionados(TreeNodeCollection nodos)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            foreach (TreeNode nodo in nodos)
            {
                IPermiso permiso = nodo.Tag as IPermiso;
                if (nodo.Checked && permiso != null)
                {
                    permisos.Add(permiso);
                }

                permisos.AddRange(ObtenerPermisosSeleccionados(nodo.Nodes));
            }

            return permisos;
        }

        private void AplicarSeleccionActualDesdeGrilla()
        {
            if (dgvUsuarios.SelectedRows.Count == 0 || !(dgvUsuarios.SelectedRows[0].DataBoundItem is Usuario usuario))
            {
                _usuarioSeleccionado = null;
                txtbNombreUsuario.Clear();
                txtbPasswordUsuario.Clear();
                chkbUsuarioActivo.Checked = true;
                tvwPermisosUsuario.Nodes.Clear();
                return;
            }

            _usuarioSeleccionado = usuario;
            txtbNombreUsuario.Text = usuario.Username ?? string.Empty;
            txtbPasswordUsuario.Clear();
            chkbUsuarioActivo.Checked = usuario.Activo;
            CargarPermisosUsuario();
        }

        private void ReseleccionarUsuario(DataGridView grilla, int usuarioId)
        {
            if (grilla == null || usuarioId <= 0)
            {
                return;
            }

            DataGridViewRow fila = grilla.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item =>
                {
                    Usuario usuario = item.DataBoundItem as Usuario;
                    return usuario != null && usuario.Id == usuarioId;
                });

            if (fila == null)
            {
                return;
            }

            grilla.ClearSelection();
            fila.Selected = true;

            if (fila.Cells.Count > 0)
            {
                grilla.CurrentCell = fila.Cells[0];
            }
        }

        // El username es estado de sesion, no de idioma: el prefijo lo traduce el helper via Tag
        // y aca solo refrescamos el dato cuando la sesion cambia.
        private void ActualizarSesionActualLabel(string username)
        {
            lblSesionActualUsuario.Text = string.IsNullOrWhiteSpace(username) ? "-" : username;
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoConfiguracion))
            {
                return;
            }

            new FormConfiguracionUsuario().ShowDialog(this);
        }

        private void btnGestionIdiomas_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoGestionIdiomas))
            {
                return;
            }

            new FormGestionIdiomas().Show(this);
        }

        private void btnGestionPermisos_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoGestionPermisos))
            {
                return;
            }

            using (FormGestionPermisos formGestionPermisos = new FormGestionPermisos())
            {
                formGestionPermisos.ShowDialog(this);
            }

            CargarPermisosUsuario();
        }

        private void btnHistorialUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoHistorialUsuarios))
            {
                return;
            }

            if (_usuarioSeleccionado == null)
            {
                UiDialogHelper.MostrarWarning("FormUserManagement.Warning.SelectUserHistory");
                return;
            }

            using (FormHistorialUsuario formHistorialUsuario = new FormHistorialUsuario(_usuarioSeleccionado.Id, txtbNombreUsuario.Text.Trim()))
            {
                formHistorialUsuario.ShowDialog(this);
            }

            RefrescarDatos();
            CargarPermisosUsuario();
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
        }

        private void btnBitacora_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisoBitacora))
            {
                return;
            }

            using (FormBitacora formBitacora = new FormBitacora())
            {
                formBitacora.ShowDialog(this);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormUserManagement.Confirm.LogoutMessage",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            usuarioBLL.CerrarSesionActual();
            Close();
        }

    }
}
