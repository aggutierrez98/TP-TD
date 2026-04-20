using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionUsuarios : Form
    {
        private readonly UsuarioBLL usuarioBLL = new UsuarioBLL();

        /// <summary>Id del usuario actualmente seleccionado en el DataGridView. -1 si ninguno.</summary>
        private int _idSeleccionado = -1;

        public FormGestionUsuarios()
        {
            InitializeComponent();
        }

        // ─── Carga inicial ───────────────────────────────────────────────────────

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            // Mostrar quién está logueado en el título
            Usuario sesion = Sesion.GetInstance().GetUsuarioActualUsuario();
            if (sesion != null)
            {
                labelSesionActual.Text = $"Sesión activa: {sesion.Username}";
            }

            CargarGrilla();
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private void CargarGrilla()
        {
            try
            {
                List<Usuario> usuarios = usuarioBLL.ObtenerTodos();
                dataGridViewUsuarios.DataSource = null;
                dataGridViewUsuarios.DataSource = usuarios;

                // Mostrar solo Id y Username; ocultar PasswordHash
                if (dataGridViewUsuarios.Columns.Contains("PasswordHash"))
                    dataGridViewUsuarios.Columns["PasswordHash"].Visible = false;

                if (dataGridViewUsuarios.Columns.Contains("Id"))
                {
                    dataGridViewUsuarios.Columns["Id"].HeaderText = "ID";
                    dataGridViewUsuarios.Columns["Id"].Width = 50;
                }
                if (dataGridViewUsuarios.Columns.Contains("Username"))
                {
                    dataGridViewUsuarios.Columns["Username"].HeaderText = "Nombre de usuario";
                    dataGridViewUsuarios.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            textBoxUsername.Clear();
            textBoxPassword.Clear();
            dataGridViewUsuarios.ClearSelection();
        }

        // ─── Selección en grilla ─────────────────────────────────────────────────

        private void dataGridViewUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUsuarios.SelectedRows.Count == 0)
            {
                _idSeleccionado = -1;
                return;
            }

            DataGridViewRow fila = dataGridViewUsuarios.SelectedRows[0];
            _idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            textBoxUsername.Text = fila.Cells["Username"].Value?.ToString();
            // Al seleccionar, limpiamos la contraseña (solo aplica para Agregar)
            textBoxPassword.Clear();
        }

        // ─── Botón Agregar ────────────────────────────────────────────────────────

        private void buttonAgregar_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Para agregar un usuario debe completar el Nombre de usuario y la Contraseña.",
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
                    MessageBox.Show($"Usuario '{username}' creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Botón Editar ─────────────────────────────────────────────────────────

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                MessageBox.Show(
                    "No hay ningún usuario seleccionado. Seleccioná uno de la lista para editarlo.",
                    "Sin selección",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string nuevoUsername = textBoxUsername.Text.Trim();

            if (string.IsNullOrWhiteSpace(nuevoUsername))
            {
                MessageBox.Show(
                    "El nombre de usuario no puede estar vacío.",
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
                    // Si el usuario editado es el que está en sesión, actualizamos la sesión también
                    Usuario sesionActual = Sesion.GetInstance().GetUsuarioActualUsuario();
                    if (sesionActual != null && sesionActual.Id == _idSeleccionado)
                    {
                        sesionActual.Username = nuevoUsername;
                        labelSesionActual.Text = $"Sesión activa: {nuevoUsername}";
                    }

                    MessageBox.Show($"Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Botón Eliminar ───────────────────────────────────────────────────────

        private void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == -1)
            {
                MessageBox.Show(
                    "No hay ningún usuario seleccionado. Seleccioná uno de la lista para eliminarlo.",
                    "Sin selección",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Protección: no se puede eliminar al usuario en sesión
            Usuario sesionActual = Sesion.GetInstance().GetUsuarioActualUsuario();
            if (sesionActual != null && sesionActual.Id == _idSeleccionado)
            {
                MessageBox.Show(
                    "No podés eliminar tu propio usuario mientras estás con la sesión activa.",
                    "Operación no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                return;
            }

            // Confirmación de seguridad
            string usernameSeleccionado = dataGridViewUsuarios.SelectedRows[0].Cells["Username"].Value?.ToString();
            DialogResult confirmacion = MessageBox.Show(
                $"¿Estás seguro de que querés eliminar al usuario '{usernameSeleccionado}'?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                bool exito = usuarioBLL.Eliminar(_idSeleccionado);
                if (exito)
                {
                    MessageBox.Show($"Usuario '{usernameSeleccionado}' eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarSeleccion();
                    CargarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
