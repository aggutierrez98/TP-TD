using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using SERVICIOS;

namespace TpIDS
{
    // Zonas habituales del usuario de la sesion (el scope lo resuelve ZonaUsuarioBLL): a la
    // izquierda la grilla y el alta (nombre, direccion buscada con el proveedor de ubicaciones
    // o clic en el mapa, y radio); a la derecha el mapa con cada zona como circulo, marcador
    // y etiqueta. Solo alta y baja. La busqueda reutiliza CoordinadorBusquedaUbicacion y las
    // mismas claves de estado que FormSeleccionarUbicacionComercio.
    public partial class FormMisZonas : FormTraducibleBase
    {
        private static readonly PointLatLng PosicionPorDefectoBuenosAires = new PointLatLng(-34.6037, -58.3816);
        private const int ZoomPorDefecto = 12;
        private const int ZoomZona = 14;
        private const int MaximoCoincidencias = 5;
        // Puntos del poligono que aproxima cada circulo: alcanzan visualmente hasta 20 km.
        private const int PuntosPorCirculo = 64;
        private const string UrlCopyrightOpenStreetMap = "https://www.openstreetmap.org/copyright";

        private readonly ZonaUsuarioBLL zonaUsuarioBLL = CompositionRoot.ZonaUsuario;
        private readonly CoordinadorBusquedaUbicacion _coordinadorBusqueda;
        private readonly bool _busquedaDisponible;
        private readonly GMapOverlay _overlayZonas = new GMapOverlay("zonasUsuario");
        private readonly GMapOverlay _overlayPuntoNuevo = new GMapOverlay("puntoNuevaZona");

        private List<ZonaUsuario> _zonas = new List<ZonaUsuario>();
        private IReadOnlyList<IResultadoBusquedaUbicacion> _coincidencias;
        private PointLatLng? _puntoNuevaZona;
        private bool _cargandoGrilla;
        private bool _cerrado;

        public FormMisZonas()
        {
            IBusquedaUbicacionesService busquedaUbicaciones = CompositionRoot.BusquedaUbicaciones;
            _busquedaDisponible = busquedaUbicaciones != null;
            _coordinadorBusqueda = new CoordinadorBusquedaUbicacion(busquedaUbicaciones);

            InitializeComponent();
            nudRadioZona.Value = ZonaUsuarioBLL.RadioPorDefectoMetros;
            Load += FormMisZonas_Load;
        }

        // View-model plano de la grilla.
        private sealed class FilaZonaUsuario
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public int RadioMetros { get; set; }
        }

        private void FormMisZonas_Load(object sender, EventArgs e)
        {
            MaximizarEnAreaMdi();
            ConfigurarMapa();
            ActualizarAyudas();
            RestablecerBusqueda();
            ActualizarPuntoNuevaZona(null);
            CargarZonas(null);

            ZonaUsuario primera = _zonas.FirstOrDefault();
            mapZonasUsuario.Position = primera == null
                ? PosicionPorDefectoBuenosAires
                : new PointLatLng((double)primera.Latitud, (double)primera.Longitud);
            mapZonasUsuario.Zoom = primera == null ? ZoomPorDefecto : ZoomZona;
            ActualizarBotonesZoom();
        }

        private void ConfigurarMapa()
        {
            // Mismo proveedor y la misma identificacion ante OpenStreetMap que
            // FormSeleccionarUbicacionComercio (su politica de tiles bloquea el User-Agent generico).
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMapProvider.UserAgent = "FinazAppTP-UAI/1.0";
            GMapProviders.OpenStreetMap.RefererUrl = "https://finazapp.tp-uai.local/";

            mapZonasUsuario.MapProvider = GMapProviders.OpenStreetMap;
            mapZonasUsuario.MinZoom = 2;
            mapZonasUsuario.MaxZoom = 18;
            // Boton derecho para desplazar: el izquierdo queda libre para elegir el punto.
            mapZonasUsuario.DragButton = MouseButtons.Right;
            mapZonasUsuario.Overlays.Add(_overlayZonas);
            mapZonasUsuario.Overlays.Add(_overlayPuntoNuevo);
            mapZonasUsuario.MouseClick += mapZonasUsuario_MouseClick;
            mapZonasUsuario.OnMapZoomChanged += mapZonasUsuario_OnMapZoomChanged;
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            ActualizarAyudas();
            ActualizarPuntoNuevaZona(_puntoNuevaZona);
            // Cambiar de idioma invalida la consulta pendiente y la lista del idioma anterior.
            _coordinadorBusqueda.Invalidar();
            RestablecerBusqueda();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Una respuesta tardia de la busqueda queda descartada y no vuelve a tocar controles.
            _cerrado = true;
            _coordinadorBusqueda.Detener();
            base.OnFormClosed(e);
        }

        // ---- Grilla y mapa ------------------------------------------------------------------

        private void CargarZonas(int? zonaSeleccionadaId)
        {
            try
            {
                _zonas = zonaUsuarioBLL.ObtenerMisZonas();

                _cargandoGrilla = true;
                dgvZonasUsuario.DataSource = null;
                dgvZonasUsuario.DataSource = _zonas
                    .Select(zona => new FilaZonaUsuario { Id = zona.Id, Nombre = zona.Nombre, RadioMetros = zona.RadioMetros })
                    .ToList();

                if (zonaSeleccionadaId.HasValue)
                {
                    SeleccionarFila(zonaSeleccionadaId.Value);
                }
                else
                {
                    dgvZonasUsuario.ClearSelection();
                    dgvZonasUsuario.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                _zonas = new List<ZonaUsuario>();
                UiExceptionHandler.Mostrar(ex, "Carga de zonas del usuario");
            }
            finally
            {
                _cargandoGrilla = false;
            }

            DibujarZonas();
        }

        private void SeleccionarFila(int zonaId)
        {
            foreach (DataGridViewRow fila in dgvZonasUsuario.Rows)
            {
                if (fila.DataBoundItem is FilaZonaUsuario item && item.Id == zonaId)
                {
                    fila.Selected = true;
                    dgvZonasUsuario.CurrentCell = fila.Cells[0];
                    return;
                }
            }
        }

        private ZonaUsuario ObtenerZonaSeleccionada()
        {
            FilaZonaUsuario fila = dgvZonasUsuario.SelectedRows.Count == 0
                ? null
                : dgvZonasUsuario.SelectedRows[0].DataBoundItem as FilaZonaUsuario;
            return fila == null ? null : _zonas.FirstOrDefault(zona => zona.Id == fila.Id);
        }

        private void dgvZonasUsuario_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoGrilla)
            {
                return;
            }

            ZonaUsuario seleccionada = ObtenerZonaSeleccionada();
            DibujarZonas();
            if (seleccionada != null)
            {
                mapZonasUsuario.Position = new PointLatLng((double)seleccionada.Latitud, (double)seleccionada.Longitud);
            }
        }

        // Cada zona: circulo de su radio, marcador y etiqueta con el nombre. La seleccionada
        // en la grilla se resalta con borde mas grueso y relleno mas intenso.
        private void DibujarZonas()
        {
            ZonaUsuario seleccionada = ObtenerZonaSeleccionada();
            _overlayZonas.Polygons.Clear();
            _overlayZonas.Markers.Clear();

            foreach (ZonaUsuario zona in _zonas)
            {
                bool resaltada = seleccionada != null && seleccionada.Id == zona.Id;
                PointLatLng centro = new PointLatLng((double)zona.Latitud, (double)zona.Longitud);

                GMapPolygon circulo = new GMapPolygon(CalcularCirculo(centro, zona.RadioMetros), "zona" + zona.Id)
                {
                    Fill = new SolidBrush(Color.FromArgb(resaltada ? 90 : 45, FinanzappPaleta.Verde)),
                    Stroke = new Pen(resaltada ? FinanzappPaleta.VerdeOscuro : FinanzappPaleta.Verde, resaltada ? 3 : 1)
                };
                _overlayZonas.Polygons.Add(circulo);

                GMarkerGoogle marcador = new GMarkerGoogle(centro, GMarkerGoogleType.green_small)
                {
                    ToolTipText = zona.Nombre,
                    ToolTipMode = MarkerTooltipMode.Always
                };
                _overlayZonas.Markers.Add(marcador);
            }

            DibujarPuntoNuevaZona();
            mapZonasUsuario.Invalidate();
        }

        // Punto provisorio de la zona que se esta por agregar, con el circulo del radio elegido.
        private void DibujarPuntoNuevaZona()
        {
            _overlayPuntoNuevo.Polygons.Clear();
            _overlayPuntoNuevo.Markers.Clear();
            if (!_puntoNuevaZona.HasValue)
            {
                return;
            }

            PointLatLng punto = _puntoNuevaZona.Value;
            _overlayPuntoNuevo.Polygons.Add(new GMapPolygon(CalcularCirculo(punto, (int)nudRadioZona.Value), "puntoNuevaZona")
            {
                Fill = new SolidBrush(Color.FromArgb(40, FinanzappPaleta.Alerta)),
                Stroke = new Pen(FinanzappPaleta.Alerta, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }
            });
            _overlayPuntoNuevo.Markers.Add(new GMarkerGoogle(punto, GMarkerGoogleType.red_small));
        }

        // Helper visual (no es regla de negocio): destino a una distancia y rumbo sobre la esfera
        // de DistanciaGeografica, para cerrar el circulo con PuntosPorCirculo vertices.
        private static List<PointLatLng> CalcularCirculo(PointLatLng centro, int radioMetros)
        {
            double distanciaAngular = radioMetros / DistanciaGeografica.RadioTerrestreMetros;
            double latitud = centro.Lat * Math.PI / 180d;
            double longitud = centro.Lng * Math.PI / 180d;
            List<PointLatLng> puntos = new List<PointLatLng>(PuntosPorCirculo);

            for (int i = 0; i < PuntosPorCirculo; i++)
            {
                double rumbo = 2 * Math.PI * i / PuntosPorCirculo;
                double latitudPunto = Math.Asin(
                    (Math.Sin(latitud) * Math.Cos(distanciaAngular))
                    + (Math.Cos(latitud) * Math.Sin(distanciaAngular) * Math.Cos(rumbo)));
                double longitudPunto = longitud + Math.Atan2(
                    Math.Sin(rumbo) * Math.Sin(distanciaAngular) * Math.Cos(latitud),
                    Math.Cos(distanciaAngular) - (Math.Sin(latitud) * Math.Sin(latitudPunto)));
                puntos.Add(new PointLatLng(latitudPunto * 180d / Math.PI, longitudPunto * 180d / Math.PI));
            }

            return puntos;
        }

        private void mapZonasUsuario_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            ActualizarPuntoNuevaZona(mapZonasUsuario.FromLocalToLatLng(e.X, e.Y));
        }

        private void ActualizarPuntoNuevaZona(PointLatLng? punto)
        {
            _puntoNuevaZona = punto;
            string texto = punto.HasValue
                ? string.Format(CultureInfo.InvariantCulture, "{0:0.000000}, {1:0.000000}", punto.Value.Lat, punto.Value.Lng)
                : Traducir("FormMyZones.Value.NoPoint", "Sin elegir");
            lblCoordenadasZona.Values.Text = texto;
            lblCoordenadasZona.AccessibleName = texto;
            DibujarPuntoNuevaZona();
            mapZonasUsuario.Invalidate();
        }

        private void nudRadioZona_ValueChanged(object sender, EventArgs e)
        {
            DibujarPuntoNuevaZona();
            mapZonasUsuario.Invalidate();
        }

        // ---- Alta y baja --------------------------------------------------------------------

        private void btnAgregarZona_Click(object sender, EventArgs e)
        {
            decimal? latitud = _puntoNuevaZona.HasValue ? (decimal)_puntoNuevaZona.Value.Lat : (decimal?)null;
            decimal? longitud = _puntoNuevaZona.HasValue ? (decimal)_puntoNuevaZona.Value.Lng : (decimal?)null;

            try
            {
                ZonaUsuario creada = zonaUsuarioBLL.Crear(txtbNombreZona.Text, latitud, longitud, (int)nudRadioZona.Value);
                LimpiarAlta();
                CargarZonas(creada.Id);
                mapZonasUsuario.Position = new PointLatLng((double)creada.Latitud, (double)creada.Longitud);
                UiDialogHelper.MostrarInfo("FormMyZones.Success.Created");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de zona del usuario");
            }
        }

        private void btnQuitarZona_Click(object sender, EventArgs e)
        {
            ZonaUsuario seleccionada = ObtenerZonaSeleccionada();
            if (seleccionada == null)
            {
                UiDialogHelper.MostrarWarning("FormMyZones.Warning.SelectZone");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMyZones.Confirm.DeleteMessage",
                "FormMyZones.Confirm.DeleteTitle",
                MessageBoxIcon.Warning,
                seleccionada.Nombre);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (zonaUsuarioBLL.Eliminar(seleccionada))
                {
                    CargarZonas(null);
                    UiDialogHelper.MostrarInfo("FormMyZones.Success.Deleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de zona del usuario");
            }
        }

        private void LimpiarAlta()
        {
            txtbNombreZona.Clear();
            nudRadioZona.Value = ZonaUsuarioBLL.RadioPorDefectoMetros;
            txtbBusquedaZona.Clear();
            _coordinadorBusqueda.Invalidar();
            RestablecerBusqueda();
            ActualizarPuntoNuevaZona(null);
        }

        // ---- Busqueda textual (mismo flujo que FormSeleccionarUbicacionComercio) -------------

        private async void btnBuscarZona_Click(object sender, EventArgs e)
        {
            await EjecutarBusquedaAsync();
        }

        private async void txtbBusquedaZona_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            await EjecutarBusquedaAsync();
        }

        private void txtbBusquedaZona_TextChanged(object sender, EventArgs e)
        {
            // Escribir nunca consulta; solo invalida la consulta y la lista del texto anterior.
            _coordinadorBusqueda.Invalidar();
            RestablecerBusqueda();
        }

        private async Task EjecutarBusquedaAsync()
        {
            if (!_busquedaDisponible || _coordinadorBusqueda.EnCurso || _cerrado)
            {
                return;
            }

            string texto = (txtbBusquedaZona.Text ?? string.Empty).Trim();
            if (texto.Length == 0)
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchTextRequired", "Introduzca una dirección, comercio o lugar para buscar.");
                return;
            }

            IConsultaUbicacion consulta = new ConsultaUbicacion(texto, ObtenerCodigoIdioma(), ObtenerZonaVisible());
            LimpiarCoincidencias();
            btnBuscarZona.Enabled = false;
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
                if (restaurar && !_cerrado && !IsDisposed)
                {
                    btnBuscarZona.Enabled = _busquedaDisponible;
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

        private void MostrarCoincidencias(IReadOnlyList<IResultadoBusquedaUbicacion> coincidencias)
        {
            List<IResultadoBusquedaUbicacion> vigentes = coincidencias
                .Where(coincidencia => coincidencia != null)
                .Take(MaximoCoincidencias)
                .ToList();

            _coincidencias = vigentes;
            cmbResultadosZona.BeginUpdate();
            cmbResultadosZona.Items.Clear();
            foreach (IResultadoBusquedaUbicacion coincidencia in vigentes)
            {
                cmbResultadosZona.Items.Add(new ItemCoincidencia(coincidencia));
            }

            // Cargar la lista no elige nada: el punto solo cambia con una eleccion del usuario.
            cmbResultadosZona.SelectedIndex = -1;
            cmbResultadosZona.EndUpdate();
            cmbResultadosZona.Enabled = vigentes.Count > 0;

            if (vigentes.Count == 1)
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchResultFound", "1 coincidencia. Elíjala en la lista para ubicarla en el mapa.");
            }
            else
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchResultsFound", "{0} coincidencias. Elija una en la lista para ubicarla en el mapa.", vigentes.Count);
            }

            if (cmbResultadosZona.Enabled)
            {
                cmbResultadosZona.Focus();
            }
        }

        private void cmbResultadosZona_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int indice = cmbResultadosZona.SelectedIndex;
            if (_coincidencias == null || indice < 0 || indice >= _coincidencias.Count)
            {
                return;
            }

            IResultadoBusquedaUbicacion elegida = _coincidencias[indice];
            PointLatLng punto = new PointLatLng(elegida.Latitud, elegida.Longitud);
            mapZonasUsuario.Position = punto;
            mapZonasUsuario.Zoom = ZoomZona;
            ActualizarPuntoNuevaZona(punto);

            string descripcion = elegida.Descripcion ?? string.Empty;
            toolTipMapaZonas.SetToolTip(cmbResultadosZona, descripcion);
            cmbResultadosZona.AccessibleDescription = descripcion;
            MostrarEstadoBusqueda("FormMapLocationPicker.SearchApplied", "Ubicación elegida: {0}", descripcion);
        }

        private void cmbResultadosZona_DropDown(object sender, EventArgs e)
        {
            // Las descripciones son largas: ensanchar el desplegable sin salir de la pantalla.
            int ancho = cmbResultadosZona.Width;
            foreach (object item in cmbResultadosZona.Items)
            {
                Size medida = TextRenderer.MeasureText(item.ToString(), cmbResultadosZona.Font);
                ancho = Math.Max(ancho, medida.Width + SystemInformation.VerticalScrollBarWidth + 12);
            }

            Rectangle pantalla = Screen.FromControl(cmbResultadosZona).WorkingArea;
            int disponible = Math.Max(cmbResultadosZona.Width, pantalla.Right - cmbResultadosZona.PointToScreen(Point.Empty).X - 8);
            cmbResultadosZona.DropDownWidth = Math.Min(ancho, Math.Min(disponible, pantalla.Width));
        }

        private void LimpiarCoincidencias()
        {
            _coincidencias = null;
            cmbResultadosZona.Items.Clear();
            cmbResultadosZona.SelectedIndex = -1;
            cmbResultadosZona.Enabled = false;
            cmbResultadosZona.AccessibleDescription = null;
            toolTipMapaZonas.SetToolTip(cmbResultadosZona, string.Empty);
        }

        private void RestablecerBusqueda()
        {
            LimpiarCoincidencias();
            txtbBusquedaZona.Enabled = _busquedaDisponible;
            btnBuscarZona.Enabled = _busquedaDisponible;
            if (_busquedaDisponible)
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchCoverageHelp", "La cobertura depende de OpenStreetMap. Agregue ciudad o dirección para precisar.");
            }
            else
            {
                MostrarEstadoBusqueda("FormMapLocationPicker.SearchInvalidConfig", "La búsqueda no está configurada correctamente. Puede marcar la ubicación en el mapa.");
            }

            txtbBusquedaZona.AccessibleName = lblBusquedaZona.Values.Text;
            cmbResultadosZona.AccessibleName = Traducir("FormMapLocationPicker.SearchResultsLabel", "Coincidencias de la búsqueda");
        }

        private void MostrarEstadoBusqueda(string clave, string textoBase, params object[] argumentos)
        {
            string texto = Traducir(clave, textoBase, argumentos);
            lblEstadoBusquedaZona.Text = texto;
            lblEstadoBusquedaZona.AccessibleName = texto;
            toolTipMapaZonas.SetToolTip(lblEstadoBusquedaZona, texto);
        }

        private static string ObtenerCodigoIdioma()
        {
            string nombre = IdiomaService.GetInstance().IdiomaActual?.Nombre ?? string.Empty;
            return nombre.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? "en" : "es";
        }

        // La zona visible orienta la busqueda; no la restringe.
        private ILimitesGeograficos ObtenerZonaVisible()
        {
            try
            {
                RectLatLng vista = mapZonasUsuario.ViewArea;
                return vista.IsEmpty
                    ? null
                    : new LimitesGeograficos(vista.Lat - vista.HeightLat, vista.Lng, vista.Lat, vista.Lng + vista.WidthLng);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private sealed class ItemCoincidencia
        {
            private readonly string _texto;

            public ItemCoincidencia(IResultadoBusquedaUbicacion coincidencia)
            {
                string descripcion = coincidencia.Descripcion ?? string.Empty;
                _texto = string.IsNullOrWhiteSpace(coincidencia.Tipo) ? descripcion : descripcion + " (" + coincidencia.Tipo + ")";
            }

            public override string ToString()
            {
                return _texto;
            }
        }

        // ---- Zoom, ayudas y atribucion ------------------------------------------------------

        private void ActualizarAyudas()
        {
            string acercar = Traducir("FormMapLocationPicker.ZoomIn", "Acercar el mapa");
            string alejar = Traducir("FormMapLocationPicker.ZoomOut", "Alejar el mapa");
            btnMapaAcercar.AccessibleName = acercar;
            btnMapaAlejar.AccessibleName = alejar;
            toolTipMapaZonas.SetToolTip(btnMapaAcercar, acercar);
            toolTipMapaZonas.SetToolTip(btnMapaAlejar, alejar);
        }

        private void mapZonasUsuario_OnMapZoomChanged()
        {
            ActualizarBotonesZoom();
        }

        private void ActualizarBotonesZoom()
        {
            btnMapaAcercar.Enabled = mapZonasUsuario.Zoom < mapZonasUsuario.MaxZoom;
            btnMapaAlejar.Enabled = mapZonasUsuario.Zoom > mapZonasUsuario.MinZoom;
        }

        private void btnMapaAcercar_Click(object sender, EventArgs e)
        {
            mapZonasUsuario.Zoom = Math.Min(mapZonasUsuario.MaxZoom, mapZonasUsuario.Zoom + 1);
        }

        private void btnMapaAlejar_Click(object sender, EventArgs e)
        {
            mapZonasUsuario.Zoom = Math.Max(mapZonasUsuario.MinZoom, mapZonasUsuario.Zoom - 1);
        }

        private void lblAtribucionOsmZonas_LinkClicked(object sender, EventArgs e)
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

        private static string Traducir(string clave, string textoBase, params object[] argumentos)
        {
            string texto = IdiomaService.GetInstance().IdiomaActual?.ObtenerTraduccion(clave);
            if (string.IsNullOrWhiteSpace(texto) || texto == clave)
            {
                texto = textoBase;
            }

            return argumentos != null && argumentos.Length > 0
                ? string.Format(CultureInfo.CurrentCulture, texto, argumentos)
                : texto;
        }
    }
}
