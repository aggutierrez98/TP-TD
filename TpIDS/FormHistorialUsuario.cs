using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public class FormHistorialUsuario : Form
    {
        private readonly UsuarioBLL _usuarioBLL;
        private readonly Action<Idioma> _observadorIdioma;
        private readonly int _usuarioId;
        private readonly string _username;

        private Label lblTituloHistorialUsuario;
        private Label lblUsuarioHistorial;
        private DataGridView dgvHistorialUsuario;
        private DataGridView dgvCambiosEstado;
        private Button btnRestaurarVersion;
        private Button btnRefrescarHistorial;

        public FormHistorialUsuario(int usuarioId, string username)
        {
            _usuarioBLL = new UsuarioBLL();
            _observadorIdioma = Actualizar;
            _usuarioId = usuarioId;
            _username = username;
            InitializeComponent();
            ConfigurarTags();
            Load += FormHistorialUsuario_Load;
            FormClosed += FormHistorialUsuario_FormClosed;
        }

        private void InitializeComponent()
        {
            lblTituloHistorialUsuario = new Label();
            lblUsuarioHistorial = new Label();
            dgvHistorialUsuario = new DataGridView();
            dgvCambiosEstado = new DataGridView();
            btnRestaurarVersion = new Button();
            btnRefrescarHistorial = new Button();

            SuspendLayout();

            lblTituloHistorialUsuario.AutoSize = true;
            lblTituloHistorialUsuario.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTituloHistorialUsuario.Location = new Point(15, 15);
            lblTituloHistorialUsuario.Name = "lblTituloHistorialUsuario";

            lblUsuarioHistorial.AutoSize = true;
            lblUsuarioHistorial.Location = new Point(17, 48);
            lblUsuarioHistorial.Name = "lblUsuarioHistorial";
            lblUsuarioHistorial.Size = new Size(0, 13);

            dgvHistorialUsuario.AllowUserToAddRows = false;
            dgvHistorialUsuario.AllowUserToDeleteRows = false;
            dgvHistorialUsuario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorialUsuario.Location = new Point(20, 75);
            dgvHistorialUsuario.MultiSelect = false;
            dgvHistorialUsuario.Name = "dgvHistorialUsuario";
            dgvHistorialUsuario.ReadOnly = true;
            dgvHistorialUsuario.RowHeadersVisible = false;
            dgvHistorialUsuario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorialUsuario.Size = new Size(760, 240);
            dgvHistorialUsuario.SelectionChanged += dgvHistorialUsuario_SelectionChanged;

            dgvCambiosEstado.AllowUserToAddRows = false;
            dgvCambiosEstado.AllowUserToDeleteRows = false;
            dgvCambiosEstado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCambiosEstado.Location = new Point(20, 330);
            dgvCambiosEstado.MultiSelect = false;
            dgvCambiosEstado.Name = "dgvCambiosEstado";
            dgvCambiosEstado.ReadOnly = true;
            dgvCambiosEstado.RowHeadersVisible = false;
            dgvCambiosEstado.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCambiosEstado.Size = new Size(760, 180);

            btnRefrescarHistorial.Location = new Point(20, 525);
            btnRefrescarHistorial.Name = "btnRefrescarHistorial";
            btnRefrescarHistorial.Size = new Size(210, 30);
            btnRefrescarHistorial.Click += btnRefrescarHistorial_Click;

            btnRestaurarVersion.Location = new Point(570, 525);
            btnRestaurarVersion.Name = "btnRestaurarVersion";
            btnRestaurarVersion.Size = new Size(210, 30);
            btnRestaurarVersion.Click += btnRestaurarVersion_Click;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 575);
            Controls.Add(lblTituloHistorialUsuario);
            Controls.Add(lblUsuarioHistorial);
            Controls.Add(dgvHistorialUsuario);
            Controls.Add(dgvCambiosEstado);
            Controls.Add(btnRefrescarHistorial);
            Controls.Add(btnRestaurarVersion);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormHistorialUsuario";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        private void FormHistorialUsuario_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(_observadorIdioma);
            lblUsuarioHistorial.Text = "Usuario: " + _username;
            CargarHistorial();
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
        }

        private void btnRefrescarHistorial_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnRestaurarVersion_Click(object sender, EventArgs e)
        {
            if (dgvHistorialUsuario.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar una version.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistorialId"].Value);
            DialogResult confirmacion = MessageBox.Show(
                "Se restaurara la version seleccionada del usuario. Desea continuar?",
                "Confirmar restauracion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _usuarioBLL.RestaurarVersion(historialId);
                MessageBox.Show("La version se restauro correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHistorial();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Restauracion de usuario");
            }
        }

        private void dgvHistorialUsuario_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHistorialUsuario.SelectedRows.Count == 0)
            {
                dgvCambiosEstado.DataSource = null;
                return;
            }

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistorialId"].Value);
            try
            {
                List<ResultadoComparacionEstado> diferencias = _usuarioBLL.ObtenerDiferenciasConVersionAnterior(historialId);
                dgvCambiosEstado.DataSource = null;
                dgvCambiosEstado.DataSource = diferencias;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de diferencias de historial");
            }
        }

        private void CargarHistorial()
        {
            try
            {
                List<UsuarioHistorialSnapshot> historial = _usuarioBLL.ObtenerHistorialEstados(_usuarioId);
                dgvHistorialUsuario.DataSource = null;
                dgvHistorialUsuario.DataSource = historial;

                if (dgvHistorialUsuario.Columns.Contains("PasswordHash"))
                {
                    dgvHistorialUsuario.Columns["PasswordHash"].Visible = false;
                }

                if (dgvHistorialUsuario.Columns.Contains("PermisosIds"))
                {
                    dgvHistorialUsuario.Columns["PermisosIds"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de historial de usuario");
            }
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormHistorialUsuario.Title";
            lblTituloHistorialUsuario.Tag = "FormHistorialUsuario.TitleLabel";
            btnRefrescarHistorial.Tag = "FormHistorialUsuario.RefreshButton";
            btnRestaurarVersion.Tag = "FormHistorialUsuario.RestoreButton";
        }

        private void FormHistorialUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(_observadorIdioma);
        }
    }
}
