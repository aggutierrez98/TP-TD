using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionUsuarios : Form, IObservador
    {
        private readonly UsuarioBLL usuarioBLL = new UsuarioBLL();
        private int _idSeleccionado = -1;

        public FormGestionUsuarios()
        {
            InitializeComponent();
            ConfigurarTags();
            FormClosed += FormGestionUsuarios_FormClosed;
        }

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            Sesion.GetInstance().RegistrarObservador(this);
            Usuario sesion = Sesion.GetInstance().GetUsuarioActualUsuario();
            ActualizarSesionActualLabel(sesion);
            CargarGrilla();
            Actualizar(Sesion.GetInstance().IdiomaActual);
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

                if (dgvUsuarios.Columns.Contains("Id"))
                {
                    dgvUsuarios.Columns["Id"].HeaderText = ObtenerTextoActual("FormGestionUsuarios.Grid.Id");
                    dgvUsuarios.Columns["Id"].Width = 50;
                }

                if (dgvUsuarios.Columns.Contains("Username"))
                {
                    dgvUsuarios.Columns["Username"].HeaderText = ObtenerTextoActual("FormGestionUsuarios.Grid.Username");
                    dgvUsuarios.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarSeleccion()
        {
            _idSeleccionado = -1;
            txtbNombreUsuario.Clear();
            txtbPasswordUsuario.Clear();
            dgvUsuarios.ClearSelection();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                _idSeleccionado = -1;
                return;
            }

            DataGridViewRow fila = dgvUsuarios.SelectedRows[0];
            _idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtbNombreUsuario.Text = fila.Cells["Username"].Value?.ToString();
            txtbPasswordUsuario.Clear();
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
                bool exito = usuarioBLL.Registrar(username, password);
                if (exito)
                {
                    MessageBox.Show($"Usuario '{username}' creado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                bool exito = usuarioBLL.Actualizar(_idSeleccionado, nuevoUsername);
                if (exito)
                {
                    Usuario sesionActual = Sesion.GetInstance().GetUsuarioActualUsuario();
                    if (sesionActual != null && sesionActual.Id == _idSeleccionado)
                    {
                        sesionActual.Username = nuevoUsername;
                        lblSesionActual.Text = $"Sesion activa: {nuevoUsername}";
                    }

                    MessageBox.Show("Usuario actualizado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarUsuario_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                MessageBox.Show(
                    "No hay ningun usuario seleccionado. Selecciona uno de la lista para eliminarlo.",
                    "Sin seleccion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string usernameSeleccionado = dgvUsuarios.SelectedRows[0].Cells["Username"].Value?.ToString();
            DialogResult confirmacion = MessageBox.Show(
                $"Estas seguro de que queres eliminar al usuario '{usernameSeleccionado}'?\n\nEsta accion no se puede deshacer.",
                "Confirmar eliminacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool exito = usuarioBLL.Eliminar(_idSeleccionado);
                if (exito)
                {
                    MessageBox.Show($"Usuario '{usernameSeleccionado}' eliminado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Actualizar(IIdioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            ActualizarSesionActualLabel(Sesion.GetInstance().GetUsuarioActualUsuario());
            ActualizarEncabezadosGrilla(idioma);
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
        }

        private void ActualizarSesionActualLabel(Usuario usuario)
        {
            string prefijo = ObtenerTextoActual("FormGestionUsuarios.SessionLabelPrefix");
            string nombreUsuario = usuario == null ? "-" : usuario.Username;
            lblSesionActual.Text = string.Format("{0} {1}", prefijo, nombreUsuario);
        }

        private void ActualizarEncabezadosGrilla(IIdioma idioma)
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
        }

        private string ObtenerTextoActual(string key)
        {
            IIdioma idioma = Sesion.GetInstance().IdiomaActual;
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

        private void FormGestionUsuarios_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(this);
        }
    }
}
