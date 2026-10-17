using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ABSTRACCIONES;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Krypton.Toolkit;

namespace TpIDS
{
    // Dialogo modal para elegir la latitud/longitud de un Comercio sobre un mapa
    // embebido (GMap.NET + OpenStreetMap, sin API key). Ademas del pan/zoom manual y
    // el click para marcar, permite buscar una direccion, comercio o lugar por texto
    // (IBusquedaUbicacionesService, solo con Buscar o Enter) y elegir explicitamente
    // una coincidencia, que coloca el unico marcador y puede ajustarse a mano.
    // El ciclo de vida de la consulta (version, cancelacion, descarte de respuestas
    // obsoletas) vive en CoordinadorBusquedaUbicacion. GMap.NET.WinForms
    // 2.1.7 no expone drag nativo por marcador (sin CanDragTool/OnMarkerDrag), asi
    // que el arrastre para ajustar se resuelve a mano con MouseDown/Move/Up +
    // OnMarkerEnter/Leave, desactivando el pan del mapa mientras dura.
    public partial class FormSeleccionarUbicacionComercio : FormTraducibleBase
    {
        private static readonly PointLatLng PosicionPorDefectoBuenosAires = new PointLatLng(-34.6037, -58.3816);
        private const int ZoomPorDefecto = 12;
        private const int PasoNavegacionPixeles = 48;
        private const int ZoomCoincidenciaSinLimites = 16;
        private const double FraccionVistaEncuadre = 0.9;
        private const int MaximoCoincidencias = 5;
        private const string UrlCopyrightOpenStreetMap = "https://www.openstreetmap.org/copyright";

        private readonly decimal? _latitudInicial;
        private readonly decimal? _longitudInicial;
        private readonly GMapOverlay _overlayMarcador = new GMapOverlay("marcadorComercio");
        private readonly KryptonButton[] _botonesNavegacion;
        private GMarkerGoogle _marcadorActual;
        private bool _mouseSobreMarcador;
        private bool _arrastrandoMarcador;
        private readonly bool _busquedaDisponible;
        private readonly CoordinadorBusquedaUbicacion _coordinadorBusqueda;
        private IReadOnlyList<ResultadoBusquedaUbicacion> _coincidencias;
        private IIdioma _idioma;
        private bool _cerrado;

        public FormSeleccionarUbicacionComercio(decimal? latitudInicial, decimal? longitudInicial, IBusquedaUbicacionesService busquedaUbicaciones)
        {
            _latitudInicial = latitudInicial;
            _longitudInicial = longitudInicial;
            _busquedaDisponible = busquedaUbicaciones != null;
            _coordinadorBusqueda = new CoordinadorBusquedaUbicacion(busquedaUbicaciones);

            InitializeComponent();
            _botonesNavegacion = new[] { btnMapaArriba, btnMapaAbajo, btnMapaIzquierda, btnMapaDerecha, btnMapaAcercar, btnMapaAlejar };
            ConfigurarEstiloNavegacion();
            mapUbicacionComercio.Paint += mapUbicacionComercio_Paint;
            ActualizarAyudaNavegacion(null);
            RestablecerBusqueda();

            Load += FormSeleccionarUbicacionComercio_Load;
        }

        public decimal? LatitudSeleccionada { get; private set; }

        public decimal? LongitudSeleccionada { get; private set; }

        private void ConfigurarEstiloNavegacion()
        {
            foreach (KryptonButton boton in _botonesNavegacion)
            {
                bool esZoom = boton == btnMapaAcercar || boton == btnMapaAlejar;
                Color fondo = esZoom ? FinanzappPaleta.Fondo : FinanzappPaleta.Verde;
                boton.StateCommon.Back.ColorStyle = PaletteColorStyle.Solid;
                boton.StateCommon.Border.Rounding = esZoom ? 6 : 12;
                boton.StateCommon.Border.Width = 1;
                boton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", esZoom ? 15F : 11F, FontStyle.Bold);
                AplicarEstadoNavegacion(boton.StateNormal, fondo, FinanzappPaleta.Borde, 204, FinanzappPaleta.Texto);
                AplicarEstadoNavegacion(boton.StateTracking, esZoom ? FinanzappPaleta.Fondo : FinanzappPaleta.VerdeAcento, FinanzappPaleta.VerdeOscuro, 230, FinanzappPaleta.Texto);
                AplicarEstadoNavegacion(boton.StatePressed, esZoom ? FinanzappPaleta.VerdeSuave : FinanzappPaleta.VerdeOscuro, FinanzappPaleta.VerdeOscuro, 240, FinanzappPaleta.Texto);
                AplicarEstadoNavegacion(boton.StateDisabled, FinanzappPaleta.Fondo, FinanzappPaleta.Borde, 165, FinanzappPaleta.Deshabilitado);
                // La paleta global define un foco verde opaco para Custom1: se sobrescribe solo aca.
                boton.OverrideFocus.Back.Color1 = Color.FromArgb(230, fondo);
                boton.OverrideFocus.Back.Color2 = Color.FromArgb(230, fondo);
                boton.OverrideFocus.Border.Color1 = FinanzappPaleta.VerdeOscuro;
                boton.OverrideFocus.Border.Color2 = FinanzappPaleta.VerdeOscuro;
                boton.OverrideFocus.Border.Width = 2;
                boton.OverrideFocus.Content.ShortText.Color1 = FinanzappPaleta.Texto;
                boton.OverrideDefault.Back.Color1 = Color.FromArgb(204, fondo);
                boton.OverrideDefault.Back.Color2 = Color.FromArgb(204, fondo);
                boton.OverrideDefault.Content.ShortText.Color1 = FinanzappPaleta.Texto;
            }
        }

        private static void AplicarEstadoNavegacion(PaletteTriple estado, Color fondo, Color borde, int alfa, Color texto)
        {
            estado.Back.Color1 = Color.FromArgb(alfa, fondo);
            estado.Back.Color2 = Color.FromArgb(alfa, fondo);
            estado.Border.Color1 = borde;
            estado.Border.Color2 = borde;
            estado.Content.ShortText.Color1 = texto;
        }

        private void mapUbicacionComercio_Paint(object sender, PaintEventArgs e)
        {
            // Al componer su fondo transparente, Krypton pide al padre que pinte exactamente
            // los limites del boton. No invalidar otra vez en ese caso: produciria un bucle.
            foreach (KryptonButton boton in _botonesNavegacion)
            {
                if (e.ClipRectangle == boton.Bounds)
                {
                    return;
                }
            }

            // Pan, zoom y llegada de tiles repintan el mapa; actualizar tambien los fondos hijos.
            foreach (KryptonButton boton in _botonesNavegacion)
            {
                if (e.ClipRectangle.IntersectsWith(boton.Bounds))
                {
                    boton.Invalidate();
                }
            }
        }

        private void FormSeleccionarUbicacionComercio_Load(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            // GMap.NET trae por defecto un User-Agent que simula un Firefox real
            // (campo estatico GMapProvider.UserAgent), sin identificar la app. OpenStreetMap
            // bloquea ese patron (lo comparten miles de apps que nunca lo cambian) por no
            // cumplir su politica de uso de tiles (operations.osmfoundation.org/policies/tiles).
            // Un User-Agent y Referer propios que identifiquen la app resuelven el 403
            // "Access blocked". UserAgent es estatico (compartido por todos los providers);
            // RefererUrl es de instancia, se setea sobre el provider concreto que se usa.
            GMapProvider.UserAgent = "FinazAppTP-UAI/1.0";
            GMapProviders.OpenStreetMap.RefererUrl = "https://finazapp.tp-uai.local/";

            mapUbicacionComercio.MapProvider = GMapProviders.OpenStreetMap;
            mapUbicacionComercio.MinZoom = 2;
            mapUbicacionComercio.MaxZoom = 18;
            mapUbicacionComercio.OnMapZoomChanged += mapUbicacionComercio_OnMapZoomChanged;
            // Boton derecho para desplazar el mapa: deja el click izquierdo libre
            // para marcar la ubicacion (el arrastre del marcador ya usa el
            // izquierdo de forma independiente, resuelto por el control).
            mapUbicacionComercio.DragButton = MouseButtons.Right;
            mapUbicacionComercio.Overlays.Add(_overlayMarcador);
            mapUbicacionComercio.MouseClick += MapUbicacionComercio_MouseClick;
            mapUbicacionComercio.MouseDown += MapUbicacionComercio_MouseDown;
            mapUbicacionComercio.MouseMove += MapUbicacionComercio_MouseMove;
            mapUbicacionComercio.MouseUp += MapUbicacionComercio_MouseUp;
            mapUbicacionComercio.OnMarkerEnter += marcador => _mouseSobreMarcador = marcador == _marcadorActual;
            mapUbicacionComercio.OnMarkerLeave += marcador => _mouseSobreMarcador = marcador == _marcadorActual ? false : _mouseSobreMarcador;

            if (_latitudInicial.HasValue && _longitudInicial.HasValue)
            {
                PointLatLng posicionInicial = new PointLatLng((double)_latitudInicial.Value, (double)_longitudInicial.Value);
                mapUbicacionComercio.Position = posicionInicial;
                mapUbicacionComercio.Zoom = ZoomPorDefecto;
                ColocarMarcador(posicionInicial);
            }
            else
            {
                mapUbicacionComercio.Position = PosicionPorDefectoBuenosAires;
                mapUbicacionComercio.Zoom = ZoomPorDefecto;
                btnAceptarUbicacion.Enabled = false;
            }

            ActualizarBotonesZoom();
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            _idioma = observado.IdiomaActual;
            ActualizarAyudaNavegacion(_idioma);
            // Cambiar de idioma invalida la consulta pendiente y la lista del idioma anterior;
            // el marcador, el centro y el zoom no se tocan.
            _coordinadorBusqueda.Invalidar();
            RestablecerBusqueda();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Cerrar (Aceptar, Cancelar o el marco) cancela la consulta pendiente: una respuesta
            // tardia queda descartada y no vuelve a tocar controles.
            _cerrado = true;
            _coordinadorBusqueda.Detener();
            base.OnFormClosed(e);
        }

        // ---- Busqueda textual -------------------------------------------------------------

        private async void btnBuscarUbicacion_Click(object sender, EventArgs e)
        {
            await EjecutarBusquedaAsync();
        }

        private async void txtbBusquedaUbicacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            // Enter ejecuta Buscar y no se propaga: el dialogo no tiene AcceptButton y no debe aceptarse.
            e.Handled = true;
            e.SuppressKeyPress = true;
            await EjecutarBusquedaAsync();
        }

        private void txtbBusquedaUbicacion_TextChanged(object sender, EventArgs e)
        {
            // Escribir nunca consulta; solo invalida la consulta y la lista del texto anterior.
            _coordinadorBusqueda.Invalidar();
            RestablecerBusqueda();
        }

        private async System.Threading.Tasks.Task EjecutarBusquedaAsync()
        {
            if (!_busquedaDisponible || _coordinadorBusqueda.EnCurso || _cerrado)
            {
                return;
            }

            string texto = (txtbBusquedaUbicacion.Text ?? string.Empty).Trim();
            if (texto.Length == 0)
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchTextRequired", "Introduzca una dirección, comercio o lugar para buscar.");
                return;
            }

            ConsultaUbicacion consulta = new ConsultaUbicacion(texto, ObtenerCodigoIdioma(), ObtenerZonaVisible());
            LimpiarCoincidencias();
            btnBuscarUbicacion.Enabled = false;
            MostrarEstadoBusqueda("FormMapLocationPicker.SearchSearching", "Buscando...");

            bool restaurar = false;
            try
            {
                ResultadoCoordinado resultado = await _coordinadorBusqueda.BuscarAsync(consulta);
                if (resultado.Tipo == TipoResultadoBusqueda.Ignorada
                    || resultado.Tipo == TipoResultadoBusqueda.Descartada
                    || _cerrado
                    || IsDisposed)
                {
                    return;
                }

                restaurar = true;
                MostrarResultadoBusqueda(resultado);
            }
            finally
            {
                // Solo la consulta vigente y con el formulario vivo restaura Buscar.
                if (restaurar && !_cerrado && !IsDisposed)
                {
                    btnBuscarUbicacion.Enabled = _busquedaDisponible;
                }
            }
        }

        private void MostrarResultadoBusqueda(ResultadoCoordinado resultado)
        {
            switch (resultado.Tipo)
            {
                case TipoResultadoBusqueda.Coincidencias:
                    MostrarCoincidencias(resultado.Coincidencias);
                    break;
                case TipoResultadoBusqueda.SinCoincidencias:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchNoResults", "No se encontraron coincidencias. Agregue ciudad o dirección para precisar.");
                    break;
                case TipoResultadoBusqueda.Error:
                    MostrarErrorBusqueda(resultado.Estado.Value);
                    break;
                default:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchCoverageHelp", "La cobertura depende de OpenStreetMap. Agregue ciudad o dirección para precisar.");
                    break;
            }
        }

        private void MostrarErrorBusqueda(EstadoBusquedaUbicacion estado)
        {
            // Ningun error toca el mapa, el marcador ni los ajustes manuales hechos durante la espera.
            switch (estado)
            {
                case EstadoBusquedaUbicacion.TiempoAgotado:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchTimeout", "La búsqueda tardó demasiado. Intente nuevamente o marque la ubicación en el mapa.");
                    break;
                case EstadoBusquedaUbicacion.LimiteExcedido:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchRateLimit", "El servicio limitó temporalmente las búsquedas. Espere unos instantes o marque la ubicación en el mapa.");
                    break;
                case EstadoBusquedaUbicacion.RespuestaInvalida:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchInvalidResponse", "El servicio devolvió una respuesta no válida. Puede marcar la ubicación en el mapa.");
                    break;
                case EstadoBusquedaUbicacion.ConfiguracionInvalida:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchInvalidConfig", "La búsqueda no está configurada correctamente. Puede marcar la ubicación en el mapa.");
                    break;
                default:
                    MostrarEstadoBusqueda("FormMapLocationPicker.SearchConnectionError", "No se pudo conectar con el servicio de búsqueda. Puede marcar la ubicación en el mapa.");
                    break;
            }
        }

        private void MostrarCoincidencias(IReadOnlyList<ResultadoBusquedaUbicacion> coincidencias)
        {
            List<ResultadoBusquedaUbicacion> vigentes = new List<ResultadoBusquedaUbicacion>();
            foreach (ResultadoBusquedaUbicacion coincidencia in coincidencias)
            {
                if (coincidencia != null && vigentes.Count < MaximoCoincidencias)
                {
                    vigentes.Add(coincidencia);
                }
            }

            bool foco = FocoEnBusqueda();
            _coincidencias = vigentes;
            cmbResultadosUbicacion.BeginUpdate();
            cmbResultadosUbicacion.Items.Clear();
            foreach (ResultadoBusquedaUbicacion coincidencia in vigentes)
            {
                cmbResultadosUbicacion.Items.Add(new ItemCoincidencia(coincidencia));
            }

            // Cargar la lista no elige nada: la ubicacion solo cambia con una eleccion del usuario.
            cmbResultadosUbicacion.SelectedIndex = -1;
            cmbResultadosUbicacion.EndUpdate();
            cmbResultadosUbicacion.Enabled = vigentes.Count > 0;
            toolTipNavegacion.SetToolTip(cmbResultadosUbicacion, string.Empty);

            if (vigentes.Count == 1)
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchResultFound", "1 coincidencia. Elíjala en la lista para ubicarla en el mapa.");
            }
            else
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchResultsFound", "{0} coincidencias. Elija una en la lista para ubicarla en el mapa.", vigentes.Count);
            }

            if (foco && cmbResultadosUbicacion.Enabled)
            {
                cmbResultadosUbicacion.Focus();
            }
        }

        private bool FocoEnBusqueda()
        {
            return ActiveControl == null
                || txtbBusquedaUbicacion.ContainsFocus
                || btnBuscarUbicacion.ContainsFocus
                || cmbResultadosUbicacion.ContainsFocus;
        }

        private void cmbResultadosUbicacion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Unico punto que aplica una coincidencia: eleccion del usuario (mouse o teclado).
            // SelectedIndexChanged por binding no pasa por aca.
            int indice = cmbResultadosUbicacion.SelectedIndex;
            if (_coincidencias == null || indice < 0 || indice >= _coincidencias.Count)
            {
                return;
            }

            ResultadoBusquedaUbicacion elegida = _coincidencias[indice];
            AplicarCoincidencia(elegida);
            string descripcion = elegida.Descripcion ?? string.Empty;
            toolTipNavegacion.SetToolTip(cmbResultadosUbicacion, descripcion);
            cmbResultadosUbicacion.AccessibleDescription = descripcion;
            MostrarEstadoBusqueda("FormMapLocationPicker.SearchApplied", "Ubicación elegida: {0}", descripcion);
        }

        private void cmbResultadosUbicacion_DropDown(object sender, EventArgs e)
        {
            // Las descripciones son largas: ensanchar el desplegable sin salir del area de pantalla.
            int ancho = cmbResultadosUbicacion.Width;
            foreach (object item in cmbResultadosUbicacion.Items)
            {
                Size medida = TextRenderer.MeasureText(item.ToString(), cmbResultadosUbicacion.Font);
                ancho = Math.Max(ancho, medida.Width + SystemInformation.VerticalScrollBarWidth + 12);
            }

            Rectangle pantalla = Screen.FromControl(cmbResultadosUbicacion).WorkingArea;
            int disponible = Math.Max(cmbResultadosUbicacion.Width, pantalla.Right - cmbResultadosUbicacion.PointToScreen(Point.Empty).X - 8);
            cmbResultadosUbicacion.DropDownWidth = Math.Min(ancho, Math.Min(disponible, pantalla.Width));
        }

        private void lblAtribucionOsmUbicacion_LinkClicked(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(UrlCopyrightOpenStreetMap) { UseShellExecute = true });
            }
            catch (Exception)
            {
                // Sin navegador asociado: la atribucion sigue visible; abrir el enlace es opcional.
            }
        }

        private void LimpiarCoincidencias()
        {
            _coincidencias = null;
            cmbResultadosUbicacion.Items.Clear();
            cmbResultadosUbicacion.SelectedIndex = -1;
            cmbResultadosUbicacion.Enabled = false;
            cmbResultadosUbicacion.AccessibleDescription = null;
            toolTipNavegacion.SetToolTip(cmbResultadosUbicacion, string.Empty);
        }

        // Estado inicial/idle: sin consulta pendiente, sin lista y con la ayuda de cobertura.
        private void RestablecerBusqueda()
        {
            LimpiarCoincidencias();
            txtbBusquedaUbicacion.Enabled = _busquedaDisponible;
            btnBuscarUbicacion.Enabled = _busquedaDisponible;
            string ayuda = Traducir("FormMapLocationPicker.SearchCoverageHelp", "La cobertura depende de OpenStreetMap. Agregue ciudad o dirección para precisar.");
            if (_busquedaDisponible)
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchCoverageHelp", "La cobertura depende de OpenStreetMap. Agregue ciudad o dirección para precisar.");
            }
            else
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchInvalidConfig", "La búsqueda no está configurada correctamente. Puede marcar la ubicación en el mapa.");
            }

            txtbBusquedaUbicacion.AccessibleName = lblBusquedaUbicacion.Values.Text;
            txtbBusquedaUbicacion.AccessibleDescription = ayuda;
            toolTipNavegacion.SetToolTip(txtbBusquedaUbicacion, ayuda);
            cmbResultadosUbicacion.AccessibleName = Traducir("FormMapLocationPicker.SearchResultsLabel", "Coincidencias de la búsqueda");
        }

        private void MostrarEstadoBusqueda(string clave, string textoBase, params object[] argumentos)
        {
            string texto = Traducir(clave, textoBase, argumentos);
            lblEstadoBusquedaUbicacion.Text = texto;
            lblEstadoBusquedaUbicacion.AccessibleName = texto;
            toolTipNavegacion.SetToolTip(lblEstadoBusquedaUbicacion, texto);
        }

        private string Traducir(string clave, string textoBase, params object[] argumentos)
        {
            string texto = _idioma?.ObtenerTraduccion(clave);
            if (string.IsNullOrWhiteSpace(texto) || texto == clave)
            {
                texto = textoBase;
            }

            return argumentos != null && argumentos.Length > 0
                ? string.Format(System.Globalization.CultureInfo.CurrentCulture, texto, argumentos)
                : texto;
        }

        // "es" o "en" para orientar el idioma de las descripciones; cualquier idioma cuyo nombre
        // empiece con "en" (English) usa ingles y el resto, español.
        private string ObtenerCodigoIdioma()
        {
            string nombre = _idioma?.Nombre ?? string.Empty;
            return nombre.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? "en" : "es";
        }

        // La zona visible es una preferencia de la busqueda, no una restriccion.
        private LimitesGeograficos ObtenerZonaVisible()
        {
            try
            {
                RectLatLng vista = mapUbicacionComercio.ViewArea;
                if (vista.IsEmpty)
                {
                    return null;
                }

                return new LimitesGeograficos(vista.Lat - vista.HeightLat, vista.Lng, vista.Lat, vista.Lng + vista.WidthLng);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void AplicarCoincidencia(ResultadoBusquedaUbicacion coincidencia)
        {
            // Interrumpir cualquier arrastre activo y limpiar el hover del marcador que se reemplaza.
            _arrastrandoMarcador = false;
            _mouseSobreMarcador = false;
            mapUbicacionComercio.CanDragMap = true;

            PointLatLng punto = new PointLatLng(coincidencia.Latitud, coincidencia.Longitud);
            int zoom = (coincidencia.Limites != null ? CalcularZoomEncuadre(coincidencia.Limites) : null)
                ?? ZoomCoincidenciaSinLimites;
            zoom = Math.Max(mapUbicacionComercio.MinZoom, Math.Min(mapUbicacionComercio.MaxZoom, zoom));

            mapUbicacionComercio.Position = punto;
            mapUbicacionComercio.Zoom = zoom;
            // El centro de los limites puede diferir de la coordenada devuelta: se reafirma esta.
            mapUbicacionComercio.Position = punto;
            ColocarMarcador(punto);
            ActualizarBotonesZoom();
        }

        // Mayor zoom (dentro de los limites del mapa) en el que la extension cabe en la vista.
        private int? CalcularZoomEncuadre(LimitesGeograficos limites)
        {
            Size vista = mapUbicacionComercio.ClientSize;
            if (vista.Width <= 0 || vista.Height <= 0)
            {
                return null;
            }

            try
            {
                PureProjection proyeccion = mapUbicacionComercio.MapProvider.Projection;
                for (int zoom = mapUbicacionComercio.MaxZoom; zoom > mapUbicacionComercio.MinZoom; zoom--)
                {
                    GPoint noroeste = proyeccion.FromLatLngToPixel(limites.Norte, limites.Oeste, zoom);
                    GPoint sureste = proyeccion.FromLatLngToPixel(limites.Sur, limites.Este, zoom);
                    if (sureste.X - noroeste.X <= vista.Width * FraccionVistaEncuadre
                        && sureste.Y - noroeste.Y <= vista.Height * FraccionVistaEncuadre)
                    {
                        return zoom;
                    }
                }

                return mapUbicacionComercio.MinZoom;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private sealed class ItemCoincidencia
        {
            private readonly string _texto;

            public ItemCoincidencia(ResultadoBusquedaUbicacion coincidencia)
            {
                string descripcion = coincidencia.Descripcion ?? string.Empty;
                _texto = string.IsNullOrWhiteSpace(coincidencia.Tipo) ? descripcion : descripcion + " (" + coincidencia.Tipo + ")";
            }

            public override string ToString()
            {
                return _texto;
            }
        }

        private void ActualizarAyudaNavegacion(IIdioma idioma)
        {
            ConfigurarAyudaBoton(btnMapaArriba, idioma, "FormMapLocationPicker.PanNorth", "Explorar hacia el norte");
            ConfigurarAyudaBoton(btnMapaAbajo, idioma, "FormMapLocationPicker.PanSouth", "Explorar hacia el sur");
            ConfigurarAyudaBoton(btnMapaIzquierda, idioma, "FormMapLocationPicker.PanWest", "Explorar hacia el oeste");
            ConfigurarAyudaBoton(btnMapaDerecha, idioma, "FormMapLocationPicker.PanEast", "Explorar hacia el este");
            ConfigurarAyudaBoton(btnMapaAcercar, idioma, "FormMapLocationPicker.ZoomIn", "Acercar el mapa");
            ConfigurarAyudaBoton(btnMapaAlejar, idioma, "FormMapLocationPicker.ZoomOut", "Alejar el mapa");
        }

        private void ConfigurarAyudaBoton(KryptonButton boton, IIdioma idioma, string key, string textoBase)
        {
            string texto = idioma?.ObtenerTraduccion(key);
            if (string.IsNullOrWhiteSpace(texto) || texto == key)
            {
                texto = textoBase;
            }

            boton.AccessibleName = texto;
            toolTipNavegacion.SetToolTip(boton, texto);
        }

        private void mapUbicacionComercio_OnMapZoomChanged()
        {
            ActualizarBotonesZoom();
        }

        private void ActualizarBotonesZoom()
        {
            btnMapaAcercar.Enabled = mapUbicacionComercio.Zoom < mapUbicacionComercio.MaxZoom;
            btnMapaAlejar.Enabled = mapUbicacionComercio.Zoom > mapUbicacionComercio.MinZoom;
        }

        private void btnMapaAcercar_Click(object sender, EventArgs e)
        {
            mapUbicacionComercio.Zoom = Math.Min(mapUbicacionComercio.MaxZoom, mapUbicacionComercio.Zoom + 1);
        }

        private void btnMapaAlejar_Click(object sender, EventArgs e)
        {
            mapUbicacionComercio.Zoom = Math.Max(mapUbicacionComercio.MinZoom, mapUbicacionComercio.Zoom - 1);
        }

        private void btnMapaArriba_Click(object sender, EventArgs e)
        {
            DesplazarVista(0, -1);
        }

        private void btnMapaAbajo_Click(object sender, EventArgs e)
        {
            DesplazarVista(0, 1);
        }

        private void btnMapaIzquierda_Click(object sender, EventArgs e)
        {
            DesplazarVista(-1, 0);
        }

        private void btnMapaDerecha_Click(object sender, EventArgs e)
        {
            DesplazarVista(1, 0);
        }

        private void DesplazarVista(int direccionHorizontal, int direccionVertical)
        {
            int x = mapUbicacionComercio.ClientSize.Width / 2 + direccionHorizontal * PasoNavegacionPixeles;
            int y = mapUbicacionComercio.ClientSize.Height / 2 + direccionVertical * PasoNavegacionPixeles;
            PointLatLng destino = mapUbicacionComercio.FromLocalToLatLng(x, y);
            RectLatLng limites = mapUbicacionComercio.MapProvider.Projection.Bounds;
            double latitud = Math.Max(limites.Bottom, Math.Min(limites.Top, destino.Lat));
            double longitud = ((destino.Lng + 180) % 360 + 360) % 360 - 180;
            mapUbicacionComercio.Position = new PointLatLng(latitud, longitud);
        }

        private void MapUbicacionComercio_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _arrastrandoMarcador)
            {
                return;
            }

            PointLatLng puntoSeleccionado = mapUbicacionComercio.FromLocalToLatLng(e.X, e.Y);
            ColocarMarcador(puntoSeleccionado);
        }

        private void MapUbicacionComercio_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !_mouseSobreMarcador || _marcadorActual == null)
            {
                return;
            }

            _arrastrandoMarcador = true;
            mapUbicacionComercio.CanDragMap = false;
        }

        private void MapUbicacionComercio_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_arrastrandoMarcador || _marcadorActual == null)
            {
                return;
            }

            _marcadorActual.Position = mapUbicacionComercio.FromLocalToLatLng(e.X, e.Y);
            mapUbicacionComercio.Invalidate();
        }

        private void MapUbicacionComercio_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_arrastrandoMarcador)
            {
                return;
            }

            _arrastrandoMarcador = false;
            mapUbicacionComercio.CanDragMap = true;
        }

        private void ColocarMarcador(PointLatLng posicion)
        {
            _overlayMarcador.Markers.Clear();

            _marcadorActual = new GMarkerGoogle(posicion, GMarkerGoogleType.red_small);
            _overlayMarcador.Markers.Add(_marcadorActual);

            btnAceptarUbicacion.Enabled = true;
        }

        private void btnAceptarUbicacion_Click(object sender, EventArgs e)
        {
            if (_marcadorActual == null)
            {
                return;
            }

            _coordinadorBusqueda.Invalidar();

            LatitudSeleccionada = (decimal)_marcadorActual.Position.Lat;
            LongitudSeleccionada = (decimal)_marcadorActual.Position.Lng;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelarUbicacion_Click(object sender, EventArgs e)
        {
            _coordinadorBusqueda.Invalidar();
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
