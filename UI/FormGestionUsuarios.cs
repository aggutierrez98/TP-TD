using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        private int _idSeleccionado = -1;
        private bool _cargandoGrillaUsuarios;

        public FormGestionUsuarios()
        {
            InitializeComponent();
        }

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            GrillaColumnasBuilder.Configurar<UsuarioGridItem>(dgvUsuarios);
            chkbUsuarioActivo.Checked = true;
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
            AplicarPermisosPantalla();
            RefrescarDatos();
        }

        private void RefrescarDatos()
        {
            int idAnterior = _idSeleccionado;
            _cargandoGrillaUsuarios = true;

            try
            {
                List<Usuario> usuarios = usuarioBLL.ObtenerTodos();
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = usuarios.Select(ProyectarUsuarioFila).ToList();
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
            _idSeleccionado = -1;
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

            if (_idSeleccionado == -1)
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
                bool usuarioEstabaActivo = ObtenerEstadoActivoSeleccionado();
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

                if (usuarioBLL.Actualizar(_idSeleccionado, nuevoUsername, nuevaPassword, usuarioActivo))
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

            if (_idSeleccionado == -1)
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
                if (usuarioBLL.Eliminar(_idSeleccionado))
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

        private bool ObtenerEstadoActivoSeleccionado()
        {
            if (dgvUsuarios.SelectedRows.Count == 0 || !dgvUsuarios.Columns.Contains("Activo"))
            {
                return true;
            }

            object valorActivo = dgvUsuarios.SelectedRows[0].Cells["Activo"].Value;
            return valorActivo != null && valorActivo != DBNull.Value && Convert.ToBoolean(valorActivo);
        }

        private void CargarPermisosUsuario()
        {
            tvwPermisosUsuario.Nodes.Clear();
            if (_idSeleccionado == -1 || !AutorizacionService.GetInstance().TienePermiso(PermisoGestionUsuarios))
            {
                return;
            }

            try
            {
                List<object> catalogo = permisoBLL.ObtenerCatalogo();
                HashSet<int> permisosAsignados = new HashSet<int>(permisoBLL.ObtenerPermisoIdsDeUsuario(_idSeleccionado));

                foreach (object raiz in catalogo)
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

        private TreeNode CrearNodoPermiso(object permiso, ISet<int> permisosAsignados)
        {
            dynamic permisoActual = permiso;

            TreeNode nodo = new TreeNode((string)permisoActual.Nombre)
            {
                Tag = permiso,
                Checked = permisosAsignados.Contains((int)permisoActual.Id)
            };

            foreach (object hijo in permisoActual.Hijos)
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

            if (_idSeleccionado == -1)
            {
                UiDialogHelper.MostrarWarning("FormUserManagement.Warning.SelectUserPermissions");
                return;
            }

            try
            {
                List<int> permisosSeleccionados = ObtenerPermisosSeleccionados(tvwPermisosUsuario.Nodes)
                    .Distinct()
                    .ToList();

                permisoBLL.GuardarPermisosUsuario(_idSeleccionado, permisosSeleccionados);
                UiDialogHelper.MostrarInfo("FormUserManagement.Success.UserPermissionsSaved");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de permisos de usuario");
            }
        }

        private List<int> ObtenerPermisosSeleccionados(TreeNodeCollection nodos)
        {
            List<int> ids = new List<int>();
            foreach (TreeNode nodo in nodos)
            {
                object permiso = nodo.Tag;
                if (nodo.Checked && permiso != null)
                {
                    ids.Add((int)((dynamic)permiso).Id);
                }

                ids.AddRange(ObtenerPermisosSeleccionados(nodo.Nodes));
            }

            return ids;
        }

        private UsuarioGridItem ProyectarUsuarioFila(Usuario usuario)
        {
            return new UsuarioGridItem
            {
                Id = usuario.Id,
                Username = usuario.Username,
                Activo = usuario.Activo
            };
        }

        private void AplicarSeleccionActualDesdeGrilla()
        {
            if (dgvUsuarios.SelectedRows.Count == 0 ||
                !dgvUsuarios.Columns.Contains("Id") ||
                !dgvUsuarios.Columns.Contains("Username") ||
                !dgvUsuarios.Columns.Contains("Activo"))
            {
                _idSeleccionado = -1;
                txtbNombreUsuario.Clear();
                txtbPasswordUsuario.Clear();
                chkbUsuarioActivo.Checked = true;
                tvwPermisosUsuario.Nodes.Clear();
                return;
            }

            DataGridViewRow fila = dgvUsuarios.SelectedRows[0];
            _idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtbNombreUsuario.Text = fila.Cells["Username"].Value == null ? string.Empty : fila.Cells["Username"].Value.ToString();
            txtbPasswordUsuario.Clear();
            object valorActivo = fila.Cells["Activo"].Value;
            chkbUsuarioActivo.Checked = valorActivo != null && valorActivo != DBNull.Value && Convert.ToBoolean(valorActivo);
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
                    UsuarioGridItem usuario = item.DataBoundItem as UsuarioGridItem;
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

            if (_idSeleccionado == -1)
            {
                UiDialogHelper.MostrarWarning("FormUserManagement.Warning.SelectUserHistory");
                return;
            }

            using (FormHistorialUsuario formHistorialUsuario = new FormHistorialUsuario(_idSeleccionado, txtbNombreUsuario.Text.Trim()))
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
    }
}
