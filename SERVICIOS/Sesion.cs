using ABSTRACCIONES;
using BE;
using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class Sesion : ISesion, IObservado
    {
        private static Sesion _instancia;

        private readonly List<IObservador> _observadoresRegistrados;
        private Usuario _usuarioActual;
        private IIdioma _idiomaActual;

        private Sesion()
        {
            _observadoresRegistrados = new List<IObservador>();
        }

        public static Sesion GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new Sesion();
            }
            return _instancia;
        }

        public IIdioma IdiomaActual
        {
            get { return _idiomaActual; }
        }

        public void IniciarSesion(object usuario)
        {
            _usuarioActual = usuario as Usuario;
        }

        public void CerrarSesion()
        {
            _usuarioActual = null;
            _idiomaActual = null;
            _observadoresRegistrados.Clear();
        }

        public object GetUsuarioActual()
        {
            return _usuarioActual;
        }

        public bool HaySesionActiva()
        {
            return _usuarioActual != null;
        }

        public void IniciarSesion(Usuario usuario)
        {
            _usuarioActual = usuario;
        }

        public Usuario GetUsuarioActualUsuario()
        {
            return _usuarioActual;
        }

        public void RegistrarObservador(IObservador observador)
        {
            if (observador == null || _observadoresRegistrados.Contains(observador))
            {
                return;
            }

            _observadoresRegistrados.Add(observador);
        }

        public void DesregistrarObservador(IObservador observador)
        {
            if (observador == null)
            {
                return;
            }

            _observadoresRegistrados.Remove(observador);
        }

        public void ActualizarObservadores()
        {
            foreach (IObservador observador in _observadoresRegistrados.ToList())
            {
                observador.Actualizar(_idiomaActual);
            }
        }

        public void CambiarIdioma(IIdioma idioma)
        {
            _idiomaActual = idioma;

            if (_idiomaActual != null)
            {
                ActualizarObservadores();
            }
        }
    }
}
