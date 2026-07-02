using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class HistorialEstadosService : IServicioHistorialEstados
    {
        private static HistorialEstadosService _instancia;

        private Func<int, IList<UsuarioHistorialSnapshot>> _obtenedorVersionesUsuario;
        private Func<int, UsuarioHistorialSnapshot> _obtenedorVersionUsuario;

        private HistorialEstadosService()
        {
        }

        /// <summary>
        /// Metodo para obtener la instancia unica del servicio de historial de estados.
        /// La instancia delega la lectura concreta a las funciones configuradas por la aplicacion.
        /// </summary>
        /// <returns>La instancia unica de <see cref="HistorialEstadosService" />.</returns>
        public static HistorialEstadosService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new HistorialEstadosService();
            }

            return _instancia;
        }

        /// <summary>
        /// Metodo para configurar los delegados que obtienen snapshots historicos de usuarios.
        /// Usado para mantener este servicio transversal desacoplado de la persistencia concreta.
        /// </summary>
        /// <param name="obtenedorVersionesUsuario">La funcion que obtiene todas las versiones de un usuario.</param>
        /// <param name="obtenedorVersionUsuario">La funcion que obtiene una version puntual por identificador de historial.</param>
        public void Configurar(
            Func<int, IList<UsuarioHistorialSnapshot>> obtenedorVersionesUsuario,
            Func<int, UsuarioHistorialSnapshot> obtenedorVersionUsuario)
        {
            _obtenedorVersionesUsuario = obtenedorVersionesUsuario;
            _obtenedorVersionUsuario = obtenedorVersionUsuario;
        }

        /// <summary>
        /// Metodo para obtener todas las versiones historicas de un usuario.
        /// Devuelve una coleccion vacia cuando el servicio todavia no fue configurado.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario historizado.</param>
        /// <returns>La lista de snapshots historicos del usuario.</returns>
        public IList<UsuarioHistorialSnapshot> ObtenerVersionesUsuario(int usuarioId)
        {
            if (_obtenedorVersionesUsuario == null)
            {
                return new List<UsuarioHistorialSnapshot>();
            }

            return _obtenedorVersionesUsuario(usuarioId);
        }

        /// <summary>
        /// Metodo para obtener una version historica puntual de un usuario.
        /// Devuelve <see langword="null" /> cuando el servicio todavia no fue configurado.
        /// </summary>
        /// <param name="historialId">El identificador de la version historica.</param>
        /// <returns>El snapshot historico solicitado.</returns>
        public UsuarioHistorialSnapshot ObtenerVersionUsuario(int historialId)
        {
            if (_obtenedorVersionUsuario == null)
            {
                return null;
            }

            return _obtenedorVersionUsuario(historialId);
        }

        IList<IUsuarioSnapshot> IServicioHistorialEstados.ObtenerVersionesUsuario(int usuarioId)
        {
            return ObtenerVersionesUsuario(usuarioId).Cast<IUsuarioSnapshot>().ToList();
        }

        IUsuarioSnapshot IServicioHistorialEstados.ObtenerVersionUsuario(int historialId)
        {
            return ObtenerVersionUsuario(historialId);
        }
    }
}
