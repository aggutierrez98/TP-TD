using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class IdiomaService : IObservado
    {
        private static IdiomaService _instancia;

        private readonly List<IObservador> _observadores;
        private Idioma _idiomaActual;

        private IdiomaService()
        {
            _observadores = new List<IObservador>();
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

        public void RegistrarObservador(IObservador observador)
        {
            if (observador == null || _observadores.Contains(observador))
            {
                return;
            }

            _observadores.Add(observador);

            if (_idiomaActual != null)
            {
                //observador.Act(_idiomaActual); // sincronización al adjuntar

                observador.Actualizar(this);
            }
        }

        public void DesregistrarObservador(IObservador observador)
        {
            if (observador == null)
            {
                return;
            }

            _observadores.Remove(observador);
        }

        public void Notificar()
        {
            if (_idiomaActual == null)
            {
                return;
            }

            foreach (IObservador observador in _observadores.ToList())
            {
                observador.Actualizar(this);
            }
        }

        public void CambiarIdioma(Idioma idioma)
        {
            _idiomaActual = idioma;

            Notificar();
        }
    }
}
