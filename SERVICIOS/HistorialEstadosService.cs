using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class HistorialEstadosService : IServicioHistorialEstados
    {
        private static HistorialEstadosService _instancia;

        private Func<UsuarioHistorialSnapshot, object, int> _guardadorVersionUsuario;
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
            Func<UsuarioHistorialSnapshot, object, int> guardadorVersionUsuario,
            Func<int, IList<UsuarioHistorialSnapshot>> obtenedorVersionesUsuario,
            Func<int, UsuarioHistorialSnapshot> obtenedorVersionUsuario)
        {
            _guardadorVersionUsuario = guardadorVersionUsuario;
            _obtenedorVersionesUsuario = obtenedorVersionesUsuario;
            _obtenedorVersionUsuario = obtenedorVersionUsuario;
        }

        public int GuardarVersionUsuario(UsuarioHistorialSnapshot snapshot, object contextoPersistencia = null)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException("snapshot");
            }

            if (_guardadorVersionUsuario == null)
            {
                throw new InvalidOperationException("El historial de estados no fue configurado.");
            }

            return _guardadorVersionUsuario(snapshot, contextoPersistencia);
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

        int IServicioHistorialEstados.GuardarVersionUsuario(IUsuarioSnapshot snapshot, object contextoPersistencia)
        {
            return GuardarVersionUsuario(snapshot as UsuarioHistorialSnapshot, contextoPersistencia);
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
