using System.Collections.Generic;
using BE;
using REPOSITORIO;

namespace BLL
{
    // Solo lectura: el usuario pidio unicamente "que se vean las monedas creadas del
    // sistema", sin alta/baja/edicion desde esta pantalla (a diferencia de TasaConversion).
    public class MonedaBLL
    {
        private readonly IMonedaRepositorio _monedaRepositorio;

        internal MonedaBLL(IMonedaRepositorio monedaRepositorio)
        {
            _monedaRepositorio = monedaRepositorio;
        }

        public List<Moneda> ObtenerTodas()
        {
            return _monedaRepositorio.ObtenerTodas();
        }
    }
}
