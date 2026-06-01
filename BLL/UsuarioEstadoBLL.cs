using System.Collections.Generic;
using System.Linq;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class UsuarioEstadoBLL
    {
        private readonly UsuarioDAL _usuarioDAL;
        private readonly UsuarioPermisoDAL _usuarioPermisoDAL;
        private readonly UsuarioPreferenciaDAL _usuarioPreferenciaDAL;
        private readonly PermisoDAL _permisoDAL;

        public UsuarioEstadoBLL()
        {
            InfraestructuraBLL.Inicializar();
            _usuarioDAL = new UsuarioDAL();
            _usuarioPermisoDAL = new UsuarioPermisoDAL();
            _usuarioPreferenciaDAL = new UsuarioPreferenciaDAL();
            _permisoDAL = new PermisoDAL();
        }

        public UsuarioHistorialSnapshot ConstruirSnapshotActual(int usuarioId, string accion, string observaciones, AccesoDatos accesoDatos = null)
        {
            BE.Usuario usuario = _usuarioDAL.ObtenerPorId(usuarioId, accesoDatos);
            if (usuario == null)
            {
                return null;
            }

            UsuarioPreferencia preferencia = _usuarioPreferenciaDAL.ObtenerPorUsuarioId(usuarioId, accesoDatos);

            return new UsuarioHistorialSnapshot
            {
                UsuarioId = usuario.Id,
                FechaHora = System.DateTime.Now,
                ActorUsuarioId = Sesion.GetInstance().GetUsuarioActualId(),
                ActorUsernameSnapshot = Sesion.GetInstance().GetUsuarioActualNombre(),
                Accion = accion,
                Observaciones = observaciones,
                Username = usuario.Username,
                PasswordHash = usuario.PasswordHash,
                Activo = usuario.Activo,
                IdiomaId = preferencia == null ? (int?)null : preferencia.IdiomaId,
                PermisosIds = _usuarioPermisoDAL.ObtenerPermisoIdsPorUsuario(usuarioId, accesoDatos)
            };
        }

        public int GuardarVersionActual(int usuarioId, string accion, string observaciones, AccesoDatos accesoDatos = null)
        {
            UsuarioHistorialSnapshot snapshot = ConstruirSnapshotActual(usuarioId, accion, observaciones, accesoDatos);
            return snapshot == null
                ? 0
                : HistorialEstadosService.GetInstance().GuardarVersionUsuario(snapshot, accesoDatos);
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

            AgregarDiferencia(diferencias, "Username", versionAnterior == null ? null : versionAnterior.Username, versionActual.Username);
            AgregarDiferencia(diferencias, "PasswordHash", versionAnterior == null ? null : versionAnterior.PasswordHash, versionActual.PasswordHash);
            AgregarDiferencia(diferencias, "Activo", versionAnterior == null ? null : versionAnterior.Activo.ToString(), versionActual.Activo.ToString());
            AgregarDiferencia(diferencias, "IdiomaId", versionAnterior == null ? null : FormatearNullable(versionAnterior.IdiomaId), FormatearNullable(versionActual.IdiomaId));
            AgregarDiferencia(diferencias, "Permisos", versionAnterior == null ? null : FormatearPermisos(versionAnterior.PermisosIds), FormatearPermisos(versionActual.PermisosIds));

            if (diferencias.Count == 0)
            {
                diferencias.Add(new ResultadoComparacionEstado
                {
                    Campo = "Sin cambios",
                    ValorAnterior = versionAnterior == null ? "-" : versionAnterior.Accion,
                    ValorNuevo = versionActual.Accion
                });
            }

            return diferencias;
        }

        public void AplicarSnapshot(UsuarioHistorialSnapshot snapshot, AccesoDatos accesoDatos)
        {
            BE.Usuario usuario = new BE.Usuario
            {
                Id = snapshot.UsuarioId,
                Username = snapshot.Username,
                PasswordHash = snapshot.PasswordHash,
                Activo = snapshot.Activo
            };

            _usuarioDAL.GuardarEstadoCompleto(usuario, accesoDatos);
            _usuarioPermisoDAL.ReemplazarPermisosUsuario(snapshot.UsuarioId, snapshot.PermisosIds == null ? new List<int>() : snapshot.PermisosIds.ToList(), accesoDatos);
            _usuarioPreferenciaDAL.Guardar(snapshot.UsuarioId, snapshot.IdiomaId, accesoDatos);
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

        private string FormatearNullable(int? valor)
        {
            return valor.HasValue ? valor.Value.ToString() : "-";
        }

        private string FormatearPermisos(IList<int> permisosIds)
        {
            if (permisosIds == null || permisosIds.Count == 0)
            {
                return "-";
            }

            Dictionary<int, string> indice = _permisoDAL.ObtenerNodos()
                .GroupBy(item => item.Id)
                .ToDictionary(group => group.Key, group => group.First().Nombre);

            List<string> nombres = permisosIds
                .Distinct()
                .OrderBy(item => item)
                .Select(item => indice.ContainsKey(item) ? indice[item] : "Permiso #" + item)
                .ToList();

            return string.Join(", ", nombres);
        }
    }
}
