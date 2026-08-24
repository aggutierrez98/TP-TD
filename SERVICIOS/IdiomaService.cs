using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class IdiomaService : IObservado
    {
        private static IdiomaService _instancia;

        private readonly List<IObservador> _observadores;
        private IIdioma _idiomaActual;

        private IdiomaService()
        {
            _observadores = new List<IObservador>();
        }

        /// <summary>
        /// Metodo para obtener la instancia unica del servicio de idioma.
        /// Centraliza el idioma actual y la notificacion a observadores registrados.
        /// </summary>
        /// <returns>La instancia unica de <see cref="IdiomaService" />.</returns>
        public static IdiomaService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new IdiomaService();
            }

            return _instancia;
        }

        public IIdioma IdiomaActual
        {
            get { return _idiomaActual; }
        }

        /// <summary>
        /// Metodo para registrar un observador del idioma actual.
        /// Si ya hay un idioma activo, actualiza inmediatamente al observador registrado.
        /// </summary>
        /// <param name="observador">El observador que debe recibir cambios de idioma.</param>
        public void RegistrarObservador(IObservador observador)
        {
            if (observador == null || _observadores.Contains(observador))
            {
                return;
            }

            _observadores.Add(observador);

            // Solo para actualizar Headers de datagrid
            // TODO: Vuela cuando tags pasen a arxchivos designer
            if (_idiomaActual != null)
            {
                observador.Actualizar(this);
            }
        }

        /// <summary>
        /// Metodo para desregistrar un observador del idioma actual.
        /// Usado cuando un formulario deja de necesitar notificaciones de cambio de idioma.
        /// </summary>
        /// <param name="observador">El observador que debe dejar de recibir cambios de idioma.</param>
        public void DesregistrarObservador(IObservador observador)
        {
            if (observador == null)
            {
                return;
            }

            _observadores.Remove(observador);
        }

        /// <summary>
        /// Metodo para notificar el idioma actual a todos los observadores registrados.
        /// No realiza ninguna accion si todavia no hay un idioma activo.
        /// </summary>
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

        /// <summary>
        /// Metodo para cambiar el idioma actual de la aplicacion.
        /// Luego del cambio notifica a los observadores registrados.
        /// </summary>
        /// <param name="idioma">El nuevo idioma activo.</param>
        public void CambiarIdioma(IIdioma idioma)
        {
            _idiomaActual = idioma;

            Notificar();
        }
    }
}
