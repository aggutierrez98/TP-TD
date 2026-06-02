using System;
using System.Collections.Generic;
using System.Drawing;
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

        private GroupBox groupBoxPermisosUsuario;
        private TreeView tvwPermisosUsuario;
        private Button btnGuardarPermisosUsuario;
        private Button btnGestionPermisos;
        private Button btnHistorialUsuario;
        private Button btnBitacora;

        public FormGestionUsuarios()
        {
            InitializeComponent();
            observadorIdioma = Actualizar;
            InicializarControlesDinamicos();
            ConfigurarLayout();
            ConfigurarTags();
            FormClosed += FormGestionUsuarios_FormClosed;
        }

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(observadorIdioma);
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
            CargarGrilla();
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
        }

        private void InicializarControlesDinamicos()
        {
            groupBoxPermisosUsuario = new GroupBox
            {
                Name = "groupBoxPermisosUsuario",
                Location = new Point(500, 70),
                Size = new Size(465, 460)
            };

            tvwPermisosUsuario = new TreeView
            {
                Name = "tvwPermisosUsuario",
                CheckBoxes = true,
                HideSelection = false,
                Location = new Point(10, 25),
                Size = new Size(445, 385)
            };

            btnGuardarPermisosUsuario = new Button
            {
                Name = "btnGuardarPermisosUsuario",
                Location = new Point(10, 420),
                Size = new Size(445, 30)
            };
            btnGuardarPermisosUsuario.Click += btnGuardarPermisosUsuario_Click;

            btnGestionPermisos = new Button
            {
                Name = "btnGestionPermisos",
                Size = new Size(170, 30)
            };
            btnGestionPermisos.Click += btnGestionPermisos_Click;

            btnHistorialUsuario = new Button
            {
                Name = "btnHistorialUsuario",
                Size = new Size(170, 30)
            };
            btnHistorialUsuario.Click += btnHistorialUsuario_Click;

            btnBitacora = new Button
            {
                Name = "btnBitacora",
                Size = new Size(170, 30)
            };
            btnBitacora.Click += btnBitacora_Click;

            groupBoxPermisosUsuario.Controls.Add(tvwPermisosUsuario);
            groupBoxPermisosUsuario.Controls.Add(btnGuardarPermisosUsuario);
            Controls.Add(groupBoxPermisosUsuario);
            Controls.Add(btnGestionPermisos);
            Controls.Add(btnHistorialUsuario);
            Controls.Add(btnBitacora);
        }

        private void ConfigurarLayout()
        {
            ClientSize = new Size(1140, 585);
            dgvUsuarios.Size = new Size(470, 230);
            groupBoxGestion.Size = new Size(470, 175);
            btnConfiguracion.Location = new Point(15, 540);
            btnConfiguracion.Size = new Size(170, 30);
            btnGestionIdiomas.Location = new Point(195, 540);
            btnGestionIdiomas.Size = new Size(170, 30);
            btnGestionPermisos.Location = new Point(375, 540);
            btnHistorialUsuario.Location = new Point(555, 540);
            btnBitacora.Location = new Point(735, 540);
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
            dgvUsuarios.ClearSelection();
            tvwPermisosUsuario.Nodes.Clear();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                _idSeleccionado = -1;
                tvwPermisosUsuario.Nodes.Clear();
                return;
            }

            DataGridViewRow fila = dgvUsuarios.SelectedRows[0];
            _idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtbNombreUsuario.Text = fila.Cells["Username"].Value == null ? string.Empty : fila.Cells["Username"].Value.ToString();
            txtbPasswordUsuario.Clear();
            CargarPermisosUsuario();
        }

        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            string username = txtbNombreUsuario.Text.Trim();
            string password = txtbPasswordUsuario.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Para agregar un usuario debe completar el Nombre de usuario y la Contrasena.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (usuarioBLL.Registrar(username, password))
                {
                    MessageBox.Show("Usuario creado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(
                    "No hay ningun usuario seleccionado. Selecciona uno de la lista para editarlo.",
                    "Sin seleccion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string nuevoUsername = txtbNombreUsuario.Text.Trim();
            if (string.IsNullOrWhiteSpace(nuevoUsername))
            {
                MessageBox.Show(
                    "El nombre de usuario no puede estar vacio.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (usuarioBLL.Actualizar(_idSeleccionado, nuevoUsername))
                {
                    ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualNombre());
                    MessageBox.Show("Usuario actualizado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(
                    "No hay ningun usuario seleccionado. Selecciona uno de la lista para desactivarlo.",
                    "Sin seleccion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "Se desactivara logicamente el usuario seleccionado. Desea continuar?",
                "Confirmar baja logica",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (usuarioBLL.Eliminar(_idSeleccionado))
                {
                    MessageBox.Show("Usuario desactivado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja logica de usuario");
            }
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
                MessageBox.Show("Debe seleccionar un usuario para guardar sus permisos.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<int> permisosSeleccionados = ObtenerPermisosSeleccionados(tvwPermisosUsuario.Nodes)
                    .Distinct()
                    .ToList();

                permisoBLL.GuardarPermisosUsuario(_idSeleccionado, permisosSeleccionados);
                MessageBox.Show("Permisos guardados correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            RecargarTextosPermiso(idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormGestionUsuarios.Title";
            lblTituloGestionUsuarios.Tag = "FormGestionUsuarios.TitleLabel";
            groupBoxGestion.Tag = "FormGestionUsuarios.UserDataGroup";
            lblNombreUsuario.Tag = "FormGestionUsuarios.UsernameLabel";
            lblPasswordUsuario.Tag = "FormGestionUsuarios.PasswordLabel";
            btnAgregarUsuario.Tag = "FormGestionUsuarios.AddButton";
            btnEditarUsuario.Tag = "FormGestionUsuarios.EditButton";
            btnEliminarUsuario.Tag = "FormGestionUsuarios.DeleteButton";
            btnConfiguracion.Tag = "FormGestionUsuarios.ConfigurationButton";
            btnGestionIdiomas.Tag = "FormGestionUsuarios.LanguageManagementButton";
            groupBoxPermisosUsuario.Tag = "FormGestionUsuarios.PermissionTreeGroup";
            btnGuardarPermisosUsuario.Tag = "FormGestionUsuarios.SavePermissionsButton";
            btnGestionPermisos.Tag = "FormGestionUsuarios.PermissionManagementButton";
            btnHistorialUsuario.Tag = "FormGestionUsuarios.HistoryButton";
            btnBitacora.Tag = "FormGestionUsuarios.BitacoraButton";
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
                dgvUsuarios.Columns["Id"].HeaderText = idioma.ObtenerTexto("FormGestionUsuarios.Grid.Id");
            }

            if (dgvUsuarios.Columns.Contains("Username"))
            {
                dgvUsuarios.Columns["Username"].HeaderText = idioma.ObtenerTexto("FormGestionUsuarios.Grid.Username");
            }

            if (dgvUsuarios.Columns.Contains("Activo"))
            {
                dgvUsuarios.Columns["Activo"].HeaderText = idioma.ObtenerTexto("FormGestionUsuarios.Grid.Active");
            }
        }

        private void RecargarTextosPermiso(Idioma idioma)
        {
            if (idioma == null)
            {
                return;
            }

            btnGestionPermisos.Text = idioma.ObtenerTexto("FormGestionUsuarios.PermissionManagementButton");
            btnGuardarPermisosUsuario.Text = idioma.ObtenerTexto("FormGestionUsuarios.SavePermissionsButton");
            btnHistorialUsuario.Text = idioma.ObtenerTexto("FormGestionUsuarios.HistoryButton");
            btnBitacora.Text = idioma.ObtenerTexto("FormGestionUsuarios.BitacoraButton");
            groupBoxPermisosUsuario.Text = idioma.ObtenerTexto("FormGestionUsuarios.PermissionTreeGroup");
        }

        private string ObtenerTextoActual(string key)
        {
            Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTexto(key);
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
                MessageBox.Show("Debe seleccionar un usuario para ver su historial.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
