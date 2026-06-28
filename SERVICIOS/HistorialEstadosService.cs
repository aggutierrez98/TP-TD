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

        public static HistorialEstadosService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new HistorialEstadosService();
            }

            return _instancia;
        }

        public void Configurar(
            Func<int, IList<UsuarioHistorialSnapshot>> obtenedorVersionesUsuario,
            Func<int, UsuarioHistorialSnapshot> obtenedorVersionUsuario)
        {
            _obtenedorVersionesUsuario = obtenedorVersionesUsuario;
            _obtenedorVersionUsuario = obtenedorVersionUsuario;
        }

        public IList<UsuarioHistorialSnapshot> ObtenerVersionesUsuario(int usuarioId)
        {
            if (_obtenedorVersionesUsuario == null)
            {
                return new List<UsuarioHistorialSnapshot>();
            }

            return _obtenedorVersionesUsuario(usuarioId);
        }

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
