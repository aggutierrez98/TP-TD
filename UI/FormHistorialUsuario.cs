using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormHistorialUsuario : FormTraducibleBase
    {
        private readonly UsuarioBLL _usuarioBLL;
        private readonly int _usuarioId;
        private readonly string _username;

        public FormHistorialUsuario(int usuarioId, string username)
        {
            _usuarioBLL = CompositionRoot.Usuario;
            _usuarioId = usuarioId;
            _username = username;
            InitializeComponent();

            // El username es dato fijo del constructor, no depende del idioma:
            // el prefijo lo traduce el helper via Tag y aca solo seteamos el dato.
            lblUsuarioHistorialNombre.Text = string.IsNullOrWhiteSpace(_username) ? "-" : _username;

            Load += FormHistorialUsuario_Load;
        }

        private void FormHistorialUsuario_Load(object sender, EventArgs e)
        {
            dgvHistorialUsuario.CellFormatting += Grilla_CellFormatting;
            dgvCambiosEstado.CellFormatting += Grilla_CellFormatting;
            RefrescarDatos();
        }

        // Traduce el contenido de las celdas que guardan una clave cruda, al pintarse.
        private void Grilla_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            DataGridView grilla = (DataGridView)sender;
            switch (grilla.Columns[e.ColumnIndex].Name)
            {
                case "Action":
                case "Observations":
                case "Field":
                case "PreviousValue":
                case "NewValue":
                    IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
                    if (idioma != null)
                    {
                        e.Value = idioma.ObtenerTraduccion(e.Value.ToString());
                        e.FormattingApplied = true;
                    }

                    break;
            }
        }

        private void btnRefrescarHistorial_Click(object sender, EventArgs e)
        {
            RefrescarDatos();
        }

        private void btnRestaurarVersion_Click(object sender, EventArgs e)
        {
            if (dgvHistorialUsuario.SelectedRows.Count == 0)
            {
                UiDialogHelper.MostrarWarning("FormUserHistory.Warning.SelectVersion");
                return;
            }

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistoryId"].Value);
            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormUserHistory.Confirm.RestoreVersionMessage",
                "FormUserHistory.Confirm.RestoreVersionTitle",
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _usuarioBLL.RestaurarVersion(historialId);
                UiDialogHelper.MostrarInfo("FormUserHistory.Success.VersionRestored");
                RefrescarDatos();
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

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistoryId"].Value);
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

        private void RefrescarDatos()
        {
            try
            {
                List<UsuarioHistorialSnapshot> historial = _usuarioBLL.ObtenerHistorialEstados(_usuarioId);
                dgvHistorialUsuario.DataSource = null;
                dgvHistorialUsuario.DataSource = historial;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de historial de usuario");
            }
        }

    }
}
