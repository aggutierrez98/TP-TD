using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class UsuarioEstadoBLL : IUsuarioEstadoBLL
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IUsuarioPermisoRepositorio _usuarioPermisoRepositorio;
        private readonly IUsuarioPreferenciaRepositorio _usuarioPreferenciaRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;

        public UsuarioEstadoBLL()
            : this(
                new UsuarioRepositorio(),
                new UsuarioPermisoRepositorio(),
                new UsuarioPreferenciaRepositorio(),
                new UnidadDeTrabajoFactory(),
                CompositionRoot.Sesion)
        {
        }

        internal UsuarioEstadoBLL(
            IUsuarioRepositorio usuarioRepositorio,
            IUsuarioPermisoRepositorio usuarioPermisoRepositorio,
            IUsuarioPreferenciaRepositorio usuarioPreferenciaRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _usuarioPermisoRepositorio = usuarioPermisoRepositorio;
            _usuarioPreferenciaRepositorio = usuarioPreferenciaRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
        }

        public UsuarioHistorialSnapshot ConstruirSnapshotActual(int usuarioId, string accion, string observaciones)
        {
            return ConstruirSnapshotActualInterno(usuarioId, accion, observaciones, null);
        }

        internal UsuarioHistorialSnapshot ConstruirSnapshotActualInterno(int usuarioId, string accion, string observaciones, IUnidadDeTrabajo unidad)
        {
            BE.Usuario usuario = _usuarioRepositorio.ObtenerPorId(usuarioId, unidad);
            if (usuario == null)
            {
                return null;
            }

            UsuarioPreferencia preferencia = _usuarioPreferenciaRepositorio.ObtenerPorUsuarioId(usuarioId, unidad);

            return new UsuarioHistorialSnapshot
            {
                UsuarioId = usuario.Id,
                FechaHora = System.DateTime.Now,
                ActorUsuarioId = _sesion.GetUsuarioActualId(),
                ActorUsernameSnapshot = _sesion.GetUsuarioActualNombre(),
                Accion = accion,
                Observaciones = observaciones,
                Username = usuario.Username,
                PasswordHash = usuario.PasswordHash,
                Activo = usuario.Activo,
                IdiomaId = preferencia == null ? (int?)null : preferencia.IdiomaId,
                PermisosIds = _usuarioPermisoRepositorio.ObtenerPermisoIdsPorUsuario(usuarioId, unidad)
            };
        }

        public int GuardarVersionActual(int usuarioId, string accion, string observaciones)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    int resultado = GuardarVersionActualInterno(usuarioId, accion, observaciones, unidad);
                    unidad.Commit();
                    return resultado;
                }
                catch
                {
                    unidad.Rollback();
                    throw;
                }
            }
        }

        internal int GuardarVersionActualInterno(int usuarioId, string accion, string observaciones, IUnidadDeTrabajo unidad)
        {
            UsuarioHistorialSnapshot snapshot = ConstruirSnapshotActualInterno(usuarioId, accion, observaciones, unidad);
            return snapshot == null
                ? 0
                : HistorialEstadosService.GetInstance().GuardarVersionUsuario(snapshot, unidad);
        }

        public List<UsuarioHistorialSnapshot> ObtenerHistorial(int usuarioId)
        {
            return HistorialEstadosService.GetInstance()
                .ObtenerVersionesUsuario(usuarioId)
                .OfType<UsuarioHistorialSnapshot>()
                .OrderByDescending(item => item.FechaHora)
                .ThenByDescending(item => item.HistorialId)
                .ToList();
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            return HistorialEstadosService.GetInstance().ObtenerVersionUsuario(historialId) as UsuarioHistorialSnapshot;
        }

        public List<ResultadoComparacionEstado> CompararVersiones(UsuarioHistorialSnapshot versionActual, UsuarioHistorialSnapshot versionAnterior)
        {
            List<ResultadoComparacionEstado> diferencias = new List<ResultadoComparacionEstado>();
            if (versionActual == null)
            {
                return diferencias;
            }

            AgregarDiferencia(diferencias, "FormUserHistory.Value.Field.Username", versionAnterior == null ? null : versionAnterior.Username, versionActual.Username);
            AgregarCambioProtegidoPassword(diferencias, versionActual, versionAnterior);
            AgregarDiferencia(diferencias, "FormUserHistory.Value.Field.Active", versionAnterior == null ? null : FormatearBool(versionAnterior.Activo), FormatearBool(versionActual.Activo));
            AgregarDiferencia(diferencias, "FormUserHistory.Value.Field.LanguageId", versionAnterior == null ? null : FormatearNullable(versionAnterior.IdiomaId), FormatearNullable(versionActual.IdiomaId));
            AgregarDiferencia(diferencias, "FormUserHistory.Value.Field.Permissions", versionAnterior == null ? null : FormatearPermisos(versionAnterior.PermisosIds), FormatearPermisos(versionActual.PermisosIds));

            if (diferencias.Count == 0)
            {
                diferencias.Add(new ResultadoComparacionEstado
                {
                    Campo = "FormUserHistory.Value.Field.NoChanges",
                    ValorAnterior = versionAnterior == null ? "-" : versionAnterior.Accion,
                    ValorNuevo = versionActual.Accion
                });
            }

            return diferencias;
        }

        internal void AplicarSnapshotInterno(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad)
        {
            BE.Usuario usuario = new BE.Usuario
            {
                Id = snapshot.UsuarioId,
                Username = snapshot.Username,
                PasswordHash = snapshot.PasswordHash,
                Activo = snapshot.Activo
            };

            _usuarioRepositorio.GuardarEstadoCompleto(usuario, unidad);
            _usuarioPermisoRepositorio.ReemplazarPermisosUsuario(snapshot.UsuarioId, snapshot.PermisosIds == null ? new List<int>() : snapshot.PermisosIds.ToList(), unidad);
            _usuarioPreferenciaRepositorio.Guardar(snapshot.UsuarioId, snapshot.IdiomaId, unidad);
        }

        int IUsuarioEstadoBLL.GuardarVersionActualInterno(int usuarioId, string accion, string observaciones, IUnidadDeTrabajo unidad)
        {
            return GuardarVersionActualInterno(usuarioId, accion, observaciones, unidad);
        }

        void IUsuarioEstadoBLL.AplicarSnapshotInterno(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad)
        {
            AplicarSnapshotInterno(snapshot, unidad);
        }

        private void AgregarDiferencia(ICollection<ResultadoComparacionEstado> diferencias, string campo, string anterior, string actual)
        {
            if (string.Equals(anterior ?? string.Empty, actual ?? string.Empty, System.StringComparison.Ordinal))
            {
                return;
            }

            diferencias.Add(new ResultadoComparacionEstado
            {
                Campo = campo,
                ValorAnterior = string.IsNullOrWhiteSpace(anterior) ? "-" : anterior,
                ValorNuevo = string.IsNullOrWhiteSpace(actual) ? "-" : actual
            });
        }

        private void AgregarCambioProtegidoPassword(ICollection<ResultadoComparacionEstado> diferencias, UsuarioHistorialSnapshot versionActual, UsuarioHistorialSnapshot versionAnterior)
        {
            string hashAnterior = versionAnterior == null ? null : versionAnterior.PasswordHash;
            string hashActual = versionActual == null ? null : versionActual.PasswordHash;

            if (string.Equals(hashAnterior ?? string.Empty, hashActual ?? string.Empty, System.StringComparison.Ordinal))
            {
                return;
            }

            diferencias.Add(new ResultadoComparacionEstado
            {
                Campo = "FormUserHistory.Value.Field.PasswordHash",
                ValorAnterior = string.IsNullOrWhiteSpace(hashAnterior) ? "-" : "FormUserHistory.Value.Marker.Configured",
                ValorNuevo = string.IsNullOrWhiteSpace(hashActual) ? "-" : "FormUserHistory.Value.Marker.Updated"
            });
        }

        private string FormatearNullable(int? valor)
        {
            return valor.HasValue ? valor.Value.ToString() : "-";
        }

        private string FormatearBool(bool valor)
        {
            return valor ? "FormUserHistory.Value.Boolean.Yes" : "FormUserHistory.Value.Boolean.No";
        }

        private string FormatearPermisos(IEnumerable<int> permisosIds)
        {
            if (permisosIds == null)
            {
                return "-";
            }

            List<int> ordenados = permisosIds.OrderBy(id => id).ToList();
            return ordenados.Count == 0 ? "-" : string.Join(", ", ordenados);
        }
    }
}
