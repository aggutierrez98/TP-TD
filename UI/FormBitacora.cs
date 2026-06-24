using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormBitacora : FormTraducibleBase
    {
        private const string NivelInformacion = "Informacion";
        private const string NivelAdvertencia = "Advertencia";
        private const string NivelError = "Error";

        private readonly BitacoraBLL _bitacoraBLL;

        public FormBitacora()
        {
            _bitacoraBLL = new BitacoraBLL();
            InitializeComponent();
            Load += FormBitacora_Load;
        }

        private void FormBitacora_Load(object sender, EventArgs e)
        {
            dgvBitacora.CellFormatting += dgvBitacora_CellFormatting;
            CargarNiveles();
            RefrescarDatos();
        }

        // Traduce el contenido de las celdas al pintarse: Actividad/Informacion guardan la clave
        // cruda; Nivel guarda el codigo de dominio y se mapea a su clave antes de traducir.
        private void dgvBitacora_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma == null)
            {
                return;
            }

            switch (dgvBitacora.Columns[e.ColumnIndex].Name)
            {
                case "Activity":
                case "Information":
                    e.Value = idioma.ObtenerTraduccion(e.Value.ToString());
                    e.FormattingApplied = true;
                    break;
                case "Level":
                    e.Value = idioma.ObtenerTraduccion(ObtenerKeyNivel(e.Value.ToString()));
                    e.FormattingApplied = true;
                    break;
            }
        }

        // El combo se puebla una sola vez con los codigos de nivel (dominio); su texto lo traduce
        // IdiomaFormHelper al cambiar el idioma (ver OpcionTraducible), sin hooks por-form.
        private void CargarNiveles()
        {
            cmbNivelBitacora.DisplayMember = "Texto";
            cmbNivelBitacora.Items.Clear();
            cmbNivelBitacora.Items.Add(new OpcionTraducible(string.Empty, string.Empty));
            cmbNivelBitacora.Items.Add(new OpcionTraducible("FormAuditLog.Level.Information", NivelInformacion));
            cmbNivelBitacora.Items.Add(new OpcionTraducible("FormAuditLog.Level.Warning", NivelAdvertencia));
            cmbNivelBitacora.Items.Add(new OpcionTraducible("FormAuditLog.Level.Error", NivelError));
            cmbNivelBitacora.SelectedIndex = 0;
        }

        private void btnBuscarBitacora_Click(object sender, EventArgs e)
        {
            RefrescarDatos();
        }

        private void RefrescarDatos()
        {
            try
            {
                FiltroBitacora filtro = new FiltroBitacora
                {
                    FechaDesde = chkbFechaDesdeBitacora.Checked ? (DateTime?)dtpFechaDesdeBitacora.Value : null,
                    FechaHasta = chkbFechaHastaBitacora.Checked ? (DateTime?)dtpFechaHastaBitacora.Value : null,
                    Usuario = txtbUsuarioBitacora.Text.Trim(),
                    Nivel = ObtenerNivelSeleccionado(),
                    EsTecnica = chkbSoloTecnicaBitacora.Checked ? (bool?)true : null
                };

                List<EventoBitacora> registros = _bitacoraBLL.Buscar(filtro);
                string filtroActividad = txtbActividadBitacora.Text.Trim();

                IEnumerable<EventoBitacora> filtrados = string.IsNullOrWhiteSpace(filtroActividad)
                    ? registros
                    : registros.Where(r => UiDialogHelper.TraducirSiExiste(r.Actividad)
                        .IndexOf(filtroActividad, StringComparison.OrdinalIgnoreCase) >= 0);

                dgvBitacora.DataSource = null;
                dgvBitacora.DataSource = filtrados.ToList();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de bitacora");
            }
        }

        private string ObtenerNivelSeleccionado()
        {
            OpcionTraducible opcion = cmbNivelBitacora.SelectedItem as OpcionTraducible;
            string nivel = opcion?.Valor as string;
            return string.IsNullOrWhiteSpace(nivel) ? null : nivel;
        }

        private static string ObtenerKeyNivel(string codigoNivel)
        {
            switch (codigoNivel)
            {
                case NivelInformacion:
                    return "FormAuditLog.Level.Information";
                case NivelAdvertencia:
                    return "FormAuditLog.Level.Warning";
                case NivelError:
                    return "FormAuditLog.Level.Error";
                default:
                    return codigoNivel;
            }
        }
    }
}
