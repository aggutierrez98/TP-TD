using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionUsuarios : Form
    {
        private readonly UsuarioBLL usuarioBLL = new UsuarioBLL();
        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private readonly Action<Idioma> observadorIdioma;
        private int _idSeleccionado = -1;

        public FormGestionUsuarios()
        {
            InitializeComponent();
            observadorIdioma = Actualizar;
            FormClosed += FormGestionUsuarios_FormClosed;
        }

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(observadorIdioma);
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
            chkbUsuarioActivo.Checked = true;
            CargarGrilla();
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
        }

        private void CargarGrilla()
        {
            try
            {
                List<Usuario> usuarios = usuarioBLL.ObtenerTodos();
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = usuarios;

                if (dgvUsuarios.Columns.Contains("PasswordHash"))
                {
                    dgvUsuarios.Columns["PasswordHash"].Visible = false;
                }

                if (dgvUsuarios.Columns.Contains("Permisos"))
                {
                    dgvUsuarios.Columns["Permisos"].Visible = false;
                }

                if (dgvUsuarios.Columns.Contains("Id"))
                {
                    dgvUsuarios.Columns["Id"].HeaderText = ObtenerTextoActual("FormGestionUsuarios.Grid.Id");
                    dgvUsuarios.Columns["Id"].Width = 50;
                }

                if (dgvUsuarios.Columns.Contains("Username"))
                {
                    dgvUsuarios.Columns["Username"].HeaderText = ObtenerTextoActual("FormGestionUsuarios.Grid.Username");
                }

                if (dgvUsuarios.Columns.Contains("Activo"))
                {
                    dgvUsuarios.Columns["Activo"].HeaderText = ObtenerTextoActual("FormGestionUsuarios.Grid.Active");
                    dgvUsuarios.Columns["Activo"].Width = 65;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de usuarios");
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

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                _idSeleccionado = -1;
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

        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            string username = txtbNombreUsuario.Text.Trim();
            string password = txtbPasswordUsuario.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormGestionUsuarios.Warning.CreateIncompleteData",
                    "FormGestionUsuarios.Warning.CreateIncompleteDataTitle");
                return;
            }

            try
            {
                if (usuarioBLL.Registrar(username, password))
                {
                    UiDialogHelper.MostrarInfo("FormGestionUsuarios.Success.UserCreated");
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de usuario");
            }
        }

        private void btnEditarUsuario_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormGestionUsuarios.Warning.NoUserSelectedEdit",
                    "FormGestionUsuarios.Warning.NoUserSelectedTitle");
                return;
            }

            string nuevoUsername = txtbNombreUsuario.Text.Trim();
            string nuevaPassword = txtbPasswordUsuario.Text.Trim();
            if (string.IsNullOrWhiteSpace(nuevoUsername))
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormGestionUsuarios.Warning.UsernameRequired",
                    "FormGestionUsuarios.Warning.UsernameRequiredTitle");
                return;
            }

            try
            {
                bool usuarioEstabaActivo = ObtenerEstadoActivoSeleccionado();
                bool usuarioActivo = chkbUsuarioActivo.Checked;

                if (usuarioEstabaActivo && !usuarioActivo)
                {
                    DialogResult confirmacionDesactivacion = UiDialogHelper.Confirmar(
                        "FormGestionUsuarios.Confirm.DeactivateUserMessage",
                        "FormGestionUsuarios.Confirm.DeactivateUserTitle",
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
                        ? "FormGestionUsuarios.Success.UserActivated"
                        : usuarioEstabaActivo && !usuarioActivo
                            ? "FormGestionUsuarios.Success.UserDeactivated"
                            : "FormGestionUsuarios.Success.UserUpdated";
                    UiDialogHelper.MostrarInfo(successKey);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de usuario");
            }
        }

        private void btnEliminarUsuario_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                UiDialogHelper.MostrarWarningConTitulo(
                    "FormGestionUsuarios.Warning.NoUserSelectedDelete",
                    "FormGestionUsuarios.Warning.NoUserSelectedTitle");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormGestionUsuarios.Confirm.PhysicalDeleteMessage",
                "FormGestionUsuarios.Confirm.PhysicalDeleteTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (usuarioBLL.Eliminar(_idSeleccionado))
                {
                    UiDialogHelper.MostrarInfo("FormGestionUsuarios.Success.UserDeleted");
                    LimpiarSeleccion();
                    CargarGrilla();
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
            if (_idSeleccionado == -1)
            {
                return;
            }

            try
            {
                List<PermisoNodo> catalogo = permisoBLL.ObtenerCatalogo();
                HashSet<int> permisosAsignados = new HashSet<int>(permisoBLL.ObtenerPermisoIdsDeUsuario(_idSeleccionado));

                foreach (PermisoNodo raiz in catalogo)
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

        private TreeNode CrearNodoPermiso(PermisoNodo permiso, ISet<int> permisosAsignados)
        {
            TreeNode nodo = new TreeNode(permiso.Nombre)
            {
                Tag = permiso,
                Checked = permisosAsignados.Contains(permiso.Id)
            };

            foreach (PermisoNodo hijo in permiso.HijosNodos)
            {
                nodo.Nodes.Add(CrearNodoPermiso(hijo, permisosAsignados));
            }

            return nodo;
        }

        private void btnGuardarPermisosUsuario_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                UiDialogHelper.MostrarWarning("FormGestionUsuarios.Warning.SelectUserPermissions");
                return;
            }

            try
            {
                List<int> permisosSeleccionados = ObtenerPermisosSeleccionados(tvwPermisosUsuario.Nodes)
                    .Distinct()
                    .ToList();

                permisoBLL.GuardarPermisosUsuario(_idSeleccionado, permisosSeleccionados);
                UiDialogHelper.MostrarInfo("FormGestionUsuarios.Success.UserPermissionsSaved");
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
                PermisoNodo permiso = nodo.Tag as PermisoNodo;
                if (nodo.Checked && permiso != null)
                {
                    ids.Add(permiso.Id);
                }

                ids.AddRange(ObtenerPermisosSeleccionados(nodo.Nodes));
            }

            return ids;
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
            ActualizarEncabezadosGrilla(idioma);
        }

        private void ActualizarSesionActualLabel(string username)
        {
            string prefijo = ObtenerTextoActual("FormGestionUsuarios.SessionLabelPrefix");
            string nombreUsuario = string.IsNullOrWhiteSpace(username) ? "-" : username;
            lblSesionActual.Text = string.Format("{0} {1}", prefijo, nombreUsuario);
        }

        private void ActualizarEncabezadosGrilla(Idioma idioma)
        {
            if (idioma == null || dgvUsuarios.Columns.Count == 0)
            {
                return;
            }

            if (dgvUsuarios.Columns.Contains("Id"))
            {
                dgvUsuarios.Columns["Id"].HeaderText = idioma.ObtenerTraduccion("FormGestionUsuarios.Grid.Id");
            }

            if (dgvUsuarios.Columns.Contains("Username"))
            {
                dgvUsuarios.Columns["Username"].HeaderText = idioma.ObtenerTraduccion("FormGestionUsuarios.Grid.Username");
            }

            if (dgvUsuarios.Columns.Contains("Activo"))
            {
                dgvUsuarios.Columns["Activo"].HeaderText = idioma.ObtenerTraduccion("FormGestionUsuarios.Grid.Active");
            }
        }

        private string ObtenerTextoActual(string key)
        {
            Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTraduccion(key);
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            new FormConfiguracionUsuario().ShowDialog(this);
        }

        private void btnGestionIdiomas_Click(object sender, EventArgs e)
        {
            new FormGestionIdiomas().Show(this);
        }

        private void btnGestionPermisos_Click(object sender, EventArgs e)
        {
            using (FormGestionPermisos formGestionPermisos = new FormGestionPermisos())
            {
                formGestionPermisos.ShowDialog(this);
            }

            CargarPermisosUsuario();
        }

        private void btnHistorialUsuario_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                UiDialogHelper.MostrarWarning("FormGestionUsuarios.Warning.SelectUserHistory");
                return;
            }

            using (FormHistorialUsuario formHistorialUsuario = new FormHistorialUsuario(_idSeleccionado, txtbNombreUsuario.Text.Trim()))
            {
                formHistorialUsuario.ShowDialog(this);
            }

            CargarGrilla();
            CargarPermisosUsuario();
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
        }

        private void btnBitacora_Click(object sender, EventArgs e)
        {
            using (FormBitacora formBitacora = new FormBitacora())
            {
                formBitacora.ShowDialog(this);
            }
        }

        private void FormGestionUsuarios_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
