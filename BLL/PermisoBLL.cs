using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class PermisoBLL : IPermisoBLL
    {
        private readonly IPermisoRepositorio _permisoRepositorio;
        private readonly IUsuarioPermisoRepositorio _usuarioPermisoRepositorio;
        private readonly PermisoService _permisoService;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IUsuarioEstadoBLL _usuarioEstadoBLL;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IAutorizacion _autorizacion;

        public PermisoBLL()
            : this(
                new PermisoRepositorio(),
                new UsuarioPermisoRepositorio(),
                new PermisoService(),
                new UsuarioRepositorio(),
                new UsuarioEstadoBLL(),
                new BitacoraBLL(),
                new UnidadDeTrabajoFactory(),
                CompositionRoot.Sesion,
                CompositionRoot.Autorizacion)
        {
        }

        internal PermisoBLL(
            IPermisoRepositorio permisoRepositorio,
            IUsuarioPermisoRepositorio usuarioPermisoRepositorio,
            PermisoService permisoService,
            IUsuarioRepositorio usuarioRepositorio,
            IUsuarioEstadoBLL usuarioEstadoBLL,
            IBitacoraBLL bitacoraBLL,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IAutorizacion autorizacion)
        {
            _permisoRepositorio = permisoRepositorio;
            _usuarioPermisoRepositorio = usuarioPermisoRepositorio;
            _permisoService = permisoService;
            _usuarioRepositorio = usuarioRepositorio;
            _usuarioEstadoBLL = usuarioEstadoBLL;
            _bitacoraBLL = bitacoraBLL;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _autorizacion = autorizacion;
        }

        public List<IPermiso> ObtenerCatalogo()
        {
            List<IPermiso> nodos = _permisoRepositorio.ObtenerNodos();
            List<Tuple<int, int>> relaciones = _permisoRepositorio.ObtenerRelaciones();
            return _permisoService.ConstruirArbol(nodos, relaciones).ToList();
        }

        public void GuardarCatalogo(List<IPermiso> raices)
        {
            if (raices == null || raices.Count == 0)
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedPermissionCatalogUpdate", "FormAuditLog.Value.Info.EmptyPermissionCatalog", "PermisoBLL");
                throw new ExcepcionNegocio("Error.Permission.CatalogEmpty");
            }

            _permisoService.ValidarSinCiclos(raices);

            List<IPermiso> nodos = _permisoService.Aplanar(raices).ToList();
            foreach (IPermiso nodo in nodos)
            {
                _permisoService.ValidarNombre(nodo.Nombre);
                nodo.Nombre = _permisoService.NormalizarNombre(nodo.Nombre);
            }

            _permisoService.ValidarNombresUnicos(raices);
            _permisoService.ValidarGruposNoVacios(nodos);

            List<IPermiso> existentes = _permisoRepositorio.ObtenerNodos();
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    foreach (IPermiso nodo in nodos)
                    {
                        nodo.Id = _permisoRepositorio.Guardar(nodo, unidad);
                    }

                    _permisoRepositorio.ReemplazarRelaciones(_permisoService.ObtenerRelaciones(raices).ToList(), unidad);

                    List<int> idsActuales = nodos.Select(item => item.Id).Distinct().ToList();
                    List<int> idsEliminados = existentes
                        .Select(item => item.Id)
                        .Where(id => !idsActuales.Contains(id))
                        .ToList();

                    _permisoRepositorio.EliminarPermisos(idsEliminados, unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.PermissionCatalogUpdate", "FormAuditLog.Value.Info.PermissionCatalogUpdate", "PermisoBLL", unidad);
                    unidad.Commit();
                }
                catch (ExcepcionNegocio)
                {
                    unidad.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    unidad.Rollback();
                    throw new ExcepcionTecnica("Error.Permission.SaveCatalogFailed", ex);
                }
            }
        }

        public List<int> ObtenerPermisoIdsDeUsuario(int usuarioId)
        {
            return _usuarioPermisoRepositorio.ObtenerPermisoIdsPorUsuario(usuarioId);
        }

        public void GuardarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    Usuario usuario = _usuarioRepositorio.ObtenerPorId(usuarioId, unidad);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Permission.UserNotFound");
                    }

                    GuardarPermisosUsuarioInterno(usuarioId, permisosId, unidad);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(usuarioId, "FormUserHistory.Value.Action.PermissionsUpdate", "FormUserHistory.Value.Obs.PermissionsUpdate", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserPermissionsUpdate", "FormAuditLog.Value.Info.UserPermissionsUpdate", "PermisoBLL", unidad);
                    unidad.Commit();
                }
                catch (ExcepcionNegocio)
                {
                    unidad.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    unidad.Rollback();
                    throw new ExcepcionTecnica("Error.Permission.SaveUserPermissionsFailed", ex);
                }
            }

            int? usuarioActualId = _sesion.GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == usuarioId)
            {
                _autorizacion.ActualizarPermisosUsuarioActual(ObtenerPermisosDirectosUsuario(usuarioId));
            }
        }

        public List<IPermiso> ObtenerPermisosDirectosUsuario(int usuarioId)
        {
            List<int> permisosId = ObtenerPermisoIdsDeUsuario(usuarioId);
            List<IPermiso> catalogo = ObtenerCatalogo();
            Dictionary<int, IPermiso> indice = _permisoService.Aplanar(catalogo)
                .ToDictionary(item => item.Id, item => item);

            return permisosId
                .Where(indice.ContainsKey)
                .Select(id => indice[id])
                .ToList();
        }

        internal void AsignarPerfilBasicoInterno(int usuarioId, IUnidadDeTrabajo unidad)
        {
            int perfilBasicoId = _permisoRepositorio.ObtenerIdPorNombre("basico", unidad);
            if (perfilBasicoId <= 0)
            {
                return;
            }

            _usuarioPermisoRepositorio.ReemplazarPermisosUsuario(usuarioId, new List<int> { perfilBasicoId }, unidad);
        }

        internal void GuardarPermisosUsuarioInterno(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad)
        {
            _usuarioPermisoRepositorio.ReemplazarPermisosUsuario(
                usuarioId,
                permisosId == null ? new List<int>() : permisosId.Distinct().ToList(),
                unidad);
        }

        void IPermisoBLL.AsignarPerfilBasicoInterno(int usuarioId, IUnidadDeTrabajo unidad)
        {
            AsignarPerfilBasicoInterno(usuarioId, unidad);
        }

        void IPermisoBLL.GuardarPermisosUsuarioInterno(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad)
        {
            GuardarPermisosUsuarioInterno(usuarioId, permisosId, unidad);
        }
    }
}
