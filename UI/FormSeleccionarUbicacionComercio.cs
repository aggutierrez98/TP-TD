using System;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace TpIDS
{
    // Dialogo modal para elegir la latitud/longitud de un Comercio sobre un mapa
    // embebido (GMap.NET + OpenStreetMap, sin API key). Sin geocoding ni buscador
    // de direcciones: solo pan/zoom manual y click para marcar. GMap.NET.WinForms
    // 2.1.7 no expone drag nativo por marcador (sin CanDragTool/OnMarkerDrag), asi
    // que el arrastre para ajustar se resuelve a mano con MouseDown/Move/Up +
    // OnMarkerEnter/Leave, desactivando el pan del mapa mientras dura.
    public partial class FormSeleccionarUbicacionComercio : FormTraducibleBase
    {
        private static readonly PointLatLng PosicionPorDefectoBuenosAires = new PointLatLng(-34.6037, -58.3816);
        private const int ZoomPorDefecto = 12;

        private readonly decimal? _latitudInicial;
        private readonly decimal? _longitudInicial;
        private readonly GMapOverlay _overlayMarcador = new GMapOverlay("marcadorComercio");
        private GMarkerGoogle _marcadorActual;
        private bool _mouseSobreMarcador;
        private bool _arrastrandoMarcador;

        public FormSeleccionarUbicacionComercio(decimal? latitudInicial, decimal? longitudInicial)
        {
            _latitudInicial = latitudInicial;
            _longitudInicial = longitudInicial;

            InitializeComponent();

            Load += FormSeleccionarUbicacionComercio_Load;
        }

        public decimal? LatitudSeleccionada { get; private set; }

        public decimal? LongitudSeleccionada { get; private set; }

        private void FormSeleccionarUbicacionComercio_Load(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
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

            LatitudSeleccionada = (decimal)_marcadorActual.Position.Lat;
            LongitudSeleccionada = (decimal)_marcadorActual.Position.Lng;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelarUbicacion_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
