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
        }

        private void CargarNiveles(Idioma idioma)
        {
            string nivelSeleccionado = cmbNivelBitacora.SelectedValue == null
                ? string.Empty
                : cmbNivelBitacora.SelectedValue.ToString();

            List<NivelBitacoraItem> niveles = new List<NivelBitacoraItem>
            {
                new NivelBitacoraItem { Codigo = string.Empty, Texto = string.Empty },
                new NivelBitacoraItem { Codigo = NivelInformacion, Texto = ObtenerTextoNivel(idioma, NivelInformacion) },
                new NivelBitacoraItem { Codigo = NivelAdvertencia, Texto = ObtenerTextoNivel(idioma, NivelAdvertencia) },
                new NivelBitacoraItem { Codigo = NivelError, Texto = ObtenerTextoNivel(idioma, NivelError) }
            };

            cmbNivelBitacora.DataSource = null;
            cmbNivelBitacora.DisplayMember = "Texto";
            cmbNivelBitacora.ValueMember = "Codigo";
            cmbNivelBitacora.DataSource = niveles;

            if (niveles.Any(item => string.Equals(item.Codigo, nivelSeleccionado, StringComparison.Ordinal)))
            {
                cmbNivelBitacora.SelectedValue = nivelSeleccionado;
            }
            else
            {
                cmbNivelBitacora.SelectedIndex = 0;
            }
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

        protected override void OnIdiomaActualizado(Idioma idioma)
        {
            base.OnIdiomaActualizado(idioma);
            CargarNiveles(idioma);
            Buscar();
        }

        private string ObtenerNivelSeleccionado()
        {
            string nivel = cmbNivelBitacora.SelectedValue == null
                ? null
                : cmbNivelBitacora.SelectedValue.ToString();

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

        private sealed class NivelBitacoraItem
        {
            public string Codigo { get; set; }

            public string Texto { get; set; }
        }
    }
}
