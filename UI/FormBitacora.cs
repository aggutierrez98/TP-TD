using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
            CargarNiveles();
            Buscar();
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
            Buscar();
        }

        private void Buscar()
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
                Idioma idioma = IdiomaService.GetInstance().IdiomaActual;

                IEnumerable<EventoBitacora> filtrados = string.IsNullOrWhiteSpace(filtroActividad)
                    ? registros
                    : registros.Where(r => UiDialogHelper.TraducirSiExiste(r.Actividad)
                        .IndexOf(filtroActividad, StringComparison.OrdinalIgnoreCase) >= 0);

                GrillaFormHelper.Recargar(dgvBitacora, filtrados, ProyectarBitacoraFila, idioma);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de bitacora");
            }
        }

        private BitacoraGridItem ProyectarBitacoraFila(EventoBitacora evento, Idioma idioma)
        {
            return new BitacoraGridItem
            {
                Id = evento.Id,
                FechaHora = evento.FechaHora,
                UsuarioId = evento.UsuarioId,
                UsernameSnapshot = evento.UsernameSnapshot,
                Actividad = UiDialogHelper.TraducirSiExiste(evento.Actividad),
                Informacion = UiDialogHelper.TraducirSiExiste(evento.Informacion),
                Nivel = ObtenerTextoNivel(idioma, evento.Nivel),
                Origen = evento.Origen,
                EsTecnica = evento.EsTecnica
            };
        }

        private string ObtenerNivelSeleccionado()
        {
            OpcionTraducible opcion = cmbNivelBitacora.SelectedItem as OpcionTraducible;
            string nivel = opcion?.Valor as string;
            return string.IsNullOrWhiteSpace(nivel) ? null : nivel;
        }

        private string ObtenerTextoNivel(Idioma idioma, string codigoNivel)
        {
            switch (codigoNivel)
            {
                case NivelInformacion:
                    return idioma == null ? NivelInformacion : idioma.ObtenerTraduccion("FormAuditLog.Level.Information");
                case NivelAdvertencia:
                    return idioma == null ? NivelAdvertencia : idioma.ObtenerTraduccion("FormAuditLog.Level.Warning");
                case NivelError:
                    return idioma == null ? NivelError : idioma.ObtenerTraduccion("FormAuditLog.Level.Error");
                default:
                    return codigoNivel;
            }
        }
    }
}
