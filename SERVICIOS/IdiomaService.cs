using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class IdiomaService
    {
        private static IdiomaService _instancia;

        private readonly List<System.Action<Idioma>> _observadoresRegistrados;
        private Idioma _idiomaActual;

        private IdiomaService()
        {
            _observadoresRegistrados = new List<System.Action<Idioma>>();
        }

        public static IdiomaService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new IdiomaService();
            }

            return _instancia;
        }

        public Idioma IdiomaActual
        {
            get { return _idiomaActual; }
        }

        public void RegistrarObservador(System.Action<Idioma> observador)
        {
            if (observador == null || _observadoresRegistrados.Contains(observador))
            {
                return;
            }

            _observadoresRegistrados.Add(observador);
        }

        public void DesregistrarObservador(System.Action<Idioma> observador)
        {
            if (observador == null)
            {
                return;
            }

            _observadoresRegistrados.Remove(observador);
        }

        public void CambiarIdioma(Idioma idioma)
        {
            _idiomaActual = idioma;

            if (_idiomaActual == null)
            {
                return;
            }

            foreach (System.Action<Idioma> observador in _observadoresRegistrados.ToList())
            {
                observador(_idiomaActual);
            }
        }
    }
}
