using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormBitacora : Form
    {
        private const string NivelInformacion = "Informacion";
        private const string NivelAdvertencia = "Advertencia";
        private const string NivelError = "Error";

        private readonly BitacoraBLL _bitacoraBLL;
        private readonly Action<Idioma> _observadorIdioma;

        private static readonly Dictionary<string, string> ActividadKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Login fallido",                        "FormBitacora.Value.Activity.LoginFallido" },
            { "Login exitoso",                        "FormBitacora.Value.Activity.LoginExitoso" },
            { "Alta de usuario rechazada",            "FormBitacora.Value.Activity.AltaUsuarioRechazada" },
            { "Alta de usuario",                      "FormBitacora.Value.Activity.AltaUsuario" },
            { "Edicion de usuario rechazada",         "FormBitacora.Value.Activity.EdicionUsuarioRechazada" },
            { "Edicion de usuario",                   "FormBitacora.Value.Activity.EdicionUsuario" },
            { "Baja logica de usuario",               "FormBitacora.Value.Activity.BajaLogicaUsuario" },
            { "Reactivacion de usuario",              "FormBitacora.Value.Activity.ReactivacionUsuario" },
            { "Borrado fisico de usuario rechazado",  "FormBitacora.Value.Activity.BorradoFisicoUsuarioRechazado" },
            { "Borrado fisico de usuario",            "FormBitacora.Value.Activity.BorradoFisicoUsuario" },
            { "Restauracion de usuario",              "FormBitacora.Value.Activity.RestauracionUsuario" },
            { "Actualizacion preferencia idioma",     "FormBitacora.Value.Activity.ActualizacionPreferenciaIdioma" },
            { "Alta de idioma",                       "FormBitacora.Value.Activity.AltaIdioma" },
            { "Edicion de idioma",                    "FormBitacora.Value.Activity.EdicionIdioma" },
            { "Baja de idioma",                       "FormBitacora.Value.Activity.BajaIdioma" },
            { "Actualizacion traduccion",             "FormBitacora.Value.Activity.ActualizacionTraduccion" },
            { "Importacion de traducciones",          "FormBitacora.Value.Activity.ImportacionTraducciones" },
            { "Exportacion de traducciones",          "FormBitacora.Value.Activity.ExportacionTraducciones" },
            { "Baja de traduccion",                   "FormBitacora.Value.Activity.BajaTraduccion" }
        };

        private static readonly Dictionary<string, string> InformacionKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Intento de login con campos vacios.",                             "FormBitacora.Value.Info.LoginCamposVacios" },
            { "Intento de login con usuario inexistente.",                       "FormBitacora.Value.Info.LoginUsuarioInexistente" },
            { "Intento de acceso sobre usuario inactivo.",                       "FormBitacora.Value.Info.LoginUsuarioInactivo" },
            { "Credenciales invalidas.",                                         "FormBitacora.Value.Info.LoginCredencialesInvalidas" },
            { "Ingreso de usuario.",                                             "FormBitacora.Value.Info.LoginExitoso" },
            { "Se intento registrar un usuario con datos incompletos.",          "FormBitacora.Value.Info.AltaUsuarioDatosIncompletos" },
            { "El nombre de usuario ya existe.",                                 "FormBitacora.Value.Info.NombreUsuarioYaExiste" },
            { "Se creo el usuario.",                                             "FormBitacora.Value.Info.AltaUsuario" },
            { "Se intento guardar un nombre vacio.",                             "FormBitacora.Value.Info.EdicionUsuarioNombreVacio" },
            { "Se actualizo el usuario.",                                        "FormBitacora.Value.Info.EdicionUsuario" },
            { "Se intento desactivar al usuario en sesion.",                     "FormBitacora.Value.Info.EdicionDesactivarSesion" },
            { "Se desactivo el usuario.",                                        "FormBitacora.Value.Info.BajaLogicaUsuario" },
            { "Se reactivo el usuario.",                                         "FormBitacora.Value.Info.ReactivacionUsuario" },
            { "Se elimino definitivamente el usuario y su historial asociado.",  "FormBitacora.Value.Info.BorradoFisicoUsuario" },
            { "Se intento eliminar al usuario en sesion.",                       "FormBitacora.Value.Info.BorradoFisicoSesion" },
            { "Se restauro el usuario desde el historial.",                      "FormBitacora.Value.Info.RestauracionUsuario" },
            { "Se actualizo el idioma preferido del usuario.",                   "FormBitacora.Value.Info.ActualizacionPreferenciaIdioma" },
            { "Se creo el idioma.",                                              "FormBitacora.Value.Info.AltaIdioma" },
            { "Se actualizo el idioma.",                                         "FormBitacora.Value.Info.EdicionIdioma" },
            { "Se elimino el idioma.",                                           "FormBitacora.Value.Info.BajaIdioma" },
            { "Se guardo la traduccion.",                                        "FormBitacora.Value.Info.ActualizacionTraduccion" },
            { "Se importaron traducciones JSON.",                                "FormBitacora.Value.Info.ImportacionTraducciones" },
            { "Se exportaron traducciones JSON.",                                "FormBitacora.Value.Info.ExportacionTraducciones" },
            { "Se elimino la traduccion.",                                       "FormBitacora.Value.Info.BajaTraduccion" }
        };

        public FormBitacora()
        {
            _bitacoraBLL = new BitacoraBLL();
            _observadorIdioma = Actualizar;
            InitializeComponent();
            Load += FormBitacora_Load;
            FormClosed += FormBitacora_FormClosed;
        }

        private void FormBitacora_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(_observadorIdioma);
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
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
                    : registros.Where(r => TraducirActividad(r.Actividad, idioma)
                        .IndexOf(filtroActividad, StringComparison.OrdinalIgnoreCase) >= 0);

                GrillaTraduccionHelper.Recargar(dgvBitacora, filtrados, ProyectarBitacoraFila, idioma, AplicarConfigColumnas);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de bitacora");
            }
        }

        private BitacoraFila ProyectarBitacoraFila(EventoBitacora evento, Idioma idioma)
        {
            return new BitacoraFila
            {
                Id = evento.Id,
                FechaHora = evento.FechaHora,
                UsuarioId = evento.UsuarioId,
                UsernameSnapshot = evento.UsernameSnapshot,
                Actividad = TraducirActividad(evento.Actividad, idioma),
                Informacion = TraducirInformacion(evento.Informacion, idioma),
                Nivel = ObtenerTextoNivel(idioma, evento.Nivel),
                Origen = evento.Origen,
                EsTecnica = evento.EsTecnica
            };
        }

        private string TraducirActividad(string actividad, Idioma idioma)
        {
            if (idioma == null || actividad == null)
            {
                return actividad ?? string.Empty;
            }

            string key;
            return ActividadKeys.TryGetValue(actividad, out key) ? idioma.ObtenerTraduccion(key) : actividad;
        }

        private string TraducirInformacion(string informacion, Idioma idioma)
        {
            if (idioma == null || informacion == null)
            {
                return informacion ?? string.Empty;
            }

            string key;
            return InformacionKeys.TryGetValue(informacion, out key) ? idioma.ObtenerTraduccion(key) : informacion;
        }

        private void AplicarConfigColumnas(DataGridView grilla)
        {
            if (grilla.Columns.Contains("Id"))
            {
                grilla.Columns["Id"].Width = 60;
            }

            if (grilla.Columns.Contains("Informacion"))
            {
                grilla.Columns["Informacion"].FillWeight = 180;
            }

            ActualizarEncabezados(IdiomaService.GetInstance().IdiomaActual, grilla);
        }

        private void ActualizarEncabezados(Idioma idioma, DataGridView grilla = null)
        {
            DataGridView g = grilla ?? dgvBitacora;
            if (idioma == null || g.Columns.Count == 0)
            {
                return;
            }

            GrillaTraduccionHelper.SetHeader(g, "Id",               "FormBitacora.Grid.Id", idioma);
            GrillaTraduccionHelper.SetHeader(g, "FechaHora",        "FormBitacora.Grid.FechaHora", idioma);
            GrillaTraduccionHelper.SetHeader(g, "UsuarioId",        "FormBitacora.Grid.UsuarioId", idioma);
            GrillaTraduccionHelper.SetHeader(g, "UsernameSnapshot", "FormBitacora.Grid.UsernameSnapshot", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Actividad",        "FormBitacora.Grid.Actividad", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Informacion",      "FormBitacora.Grid.Informacion", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Nivel",            "FormBitacora.Grid.Nivel", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Origen",           "FormBitacora.Grid.Origen", idioma);
            GrillaTraduccionHelper.SetHeader(g, "EsTecnica",        "FormBitacora.Grid.EsTecnica", idioma);
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            CargarNiveles(idioma);
            Buscar();
        }

        private void FormBitacora_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(_observadorIdioma);
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
                    return idioma == null ? NivelInformacion : idioma.ObtenerTraduccion("FormBitacora.Level.Information");
                case NivelAdvertencia:
                    return idioma == null ? NivelAdvertencia : idioma.ObtenerTraduccion("FormBitacora.Level.Warning");
                case NivelError:
                    return idioma == null ? NivelError : idioma.ObtenerTraduccion("FormBitacora.Level.Error");
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
