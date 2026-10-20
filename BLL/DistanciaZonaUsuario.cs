using BE;

namespace BLL
{
    // Distancia de un punto al centro de una zona del usuario y si cae dentro de su radio.
    public class DistanciaZonaUsuario
    {
        public DistanciaZonaUsuario(ZonaUsuario zona, double distanciaMetros)
        {
            Zona = zona;
            DistanciaMetros = distanciaMetros;
        }

        public ZonaUsuario Zona { get; private set; }

        public double DistanciaMetros { get; private set; }

        public bool EstaDentroDelRadio
        {
            get { return DistanciaMetros <= Zona.RadioMetros; }
        }
    }
}
