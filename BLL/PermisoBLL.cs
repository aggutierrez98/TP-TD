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

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    List<IPermiso> existentes = unidad.Permisos.ObtenerNodos();

                    // Solo se exige cobertura para patentes que ya existian antes de este
                    // guardado: una patente recien creada en este mismo llamado todavia no
                    // fue asignada a nadie por diseno, no es una "zona de nadie" sobreviniente.
                    HashSet<int> idsPreexistentes = new HashSet<int>(existentes.Select(item => item.Id));
                    Dictionary<int, IPermiso> indicePropuesto = nodos
                        .Where(item => item.Id > 0)
                        .ToDictionary(item => item.Id);
                    HashSet<int> patentesPreexistentesEnPropuesta = new HashSet<int>(
                        nodos.Where(item => !item.EsGrupo && idsPreexistentes.Contains(item.Id)).Select(item => item.Id));
                    List<List<int>> asignacionesActivas = unidad.Usuarios.ObtenerTodos()
                        .Where(item => item.Activo)
                        .Select(item => unidad.UsuarioPermisos.ObtenerPermisosPorUsuario(item).Select(p => p.Id).ToList())
                        .ToList();

                    ValidarNingunaPatenteQuedaSinCobertura(
                        indicePropuesto,
                        patentesPreexistentesEnPropuesta,
                        asignacionesActivas,
                        "FormAuditLog.Value.Activity.RejectedPermissionCatalogUpdate",
                        unidad);

                    foreach (IPermiso nodo in nodos)
                    {
                        nodo.Id = unidad.Permisos.Guardar(nodo);
                    }

                    unidad.Permisos.ReemplazarRelaciones(_permisoService.ObtenerRelaciones(raices).ToList());

                    List<int> idsActuales = nodos.Select(item => item.Id).Distinct().ToList();
                    List<IPermiso> eliminados = existentes
                        .Where(item => !idsActuales.Contains(item.Id))
                        .ToList();

                    unidad.Permisos.EliminarPermisos(eliminados);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.PermissionCatalogUpdate", "FormAuditLog.Value.Info.PermissionCatalogUpdate", "PermisoBLL", unidad);
                    unidad.Save();
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Permission.SaveCatalogFailed", ex);
                }
            }
        }

        public List<IPermiso> ObtenerPermisosDeUsuario(Usuario usuario)
        {
            return _usuarioPermisoRepositorio.ObtenerPermisosPorUsuario(usuario);
        }

        public void GuardarPermisosUsuario(Usuario recibida, List<IPermiso> permisos)
        {
            if (recibida == null) throw new ExcepcionNegocio("Error.Permission.UserNotFound");
            int usuarioId = recibida.Id;
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Usuario usuario = unidad.Usuarios.ObtenerPorId(usuarioId);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Permission.UserNotFound");
                    }

                    ValidarNingunaPatenteQuedaSinCoberturaTrasCambio(usuarioId, permisos?.Select(p => p.Id).ToList(), unidad, "FormAuditLog.Value.Activity.RejectedUserPermissionsUpdate");

                    GuardarPermisosUsuarioInterno(usuario, permisos, unidad);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(usuario, "FormUserHistory.Value.Action.PermissionsUpdate", "FormUserHistory.Value.Obs.PermissionsUpdate", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserPermissionsUpdate", "FormAuditLog.Value.Info.UserPermissionsUpdate", "PermisoBLL", unidad);
                    unidad.Save();
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Permission.SaveUserPermissionsFailed", ex);
                }
            }

            if (_sesion.HaySesionActiva() && _sesion.ObtenerUsuarioActual().Id == usuarioId)
            {
                _autorizacion.ActualizarPermisosUsuarioActual(ObtenerPermisosDirectosUsuario(recibida));
            }
        }

        public List<IPermiso> ObtenerPermisosDirectosUsuario(Usuario usuario)
        {
            List<int> permisosId = ObtenerPermisosDeUsuario(usuario).Select(p => p.Id).ToList();
            List<IPermiso> catalogo = ObtenerCatalogo();
            Dictionary<int, IPermiso> indice = _permisoService.Aplanar(catalogo)
                .ToDictionary(item => item.Id, item => item);

            return permisosId
                .Where(indice.ContainsKey)
                .Select(id => indice[id])
                .ToList();
        }

        internal void AsignarPerfilBasicoInterno(Usuario usuario, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            IPermiso perfilBasico = unidad.Permisos.ObtenerPorNombre("basico");
            if (perfilBasico == null)
            {
                return;
            }

            unidad.UsuarioPermisos.ReemplazarPermisosUsuario(usuario, new List<IPermiso> { perfilBasico });
        }

        internal void GuardarPermisosUsuarioInterno(Usuario usuario, List<IPermiso> permisos, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            unidad.UsuarioPermisos.ReemplazarPermisosUsuario(
                usuario,
                permisos == null ? new List<IPermiso>() : permisos.GroupBy(p => p.Id).Select(g => g.First()).ToList());
        }

        internal void ValidarPuedeEliminarUsuario(Usuario usuario, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            Dictionary<int, IPermiso> indice = ConstruirIndiceCatalogo(unidad);
            HashSet<int> patentesExistentes = new HashSet<int>(
                indice.Values.Where(item => !item.EsGrupo).Select(item => item.Id));
            List<List<int>> asignacionesOtrosActivos = unidad.Usuarios.ObtenerTodos()
                .Where(item => item.Activo && item.Id != usuario.Id)
                .Select(item => unidad.UsuarioPermisos.ObtenerPermisosPorUsuario(item).Select(p => p.Id).ToList())
                .ToList();

            ValidarNingunaPatenteQuedaSinCobertura(
                indice,
                patentesExistentes,
                asignacionesOtrosActivos,
                "FormAuditLog.Value.Activity.RejectedUserHardDelete",
                unidad);
        }

        private void ValidarNingunaPatenteQuedaSinCoberturaTrasCambio(int usuarioId, List<int> permisosIdPropuestos, IUnidadDeTrabajo unidad, string actividadRechazo)
        {
            Dictionary<int, IPermiso> indice = ConstruirIndiceCatalogo(unidad);
            HashSet<int> patentesExistentes = new HashSet<int>(
                indice.Values.Where(item => !item.EsGrupo).Select(item => item.Id));

            List<List<int>> asignaciones = unidad.Usuarios.ObtenerTodos()
                .Where(item => item.Activo)
                .Select(item => item.Id == usuarioId
                    ? (permisosIdPropuestos == null ? new List<int>() : permisosIdPropuestos)
                    : unidad.UsuarioPermisos.ObtenerPermisosPorUsuario(item).Select(p => p.Id).ToList())
                .ToList();

            ValidarNingunaPatenteQuedaSinCobertura(indice, patentesExistentes, asignaciones, actividadRechazo, unidad);
        }

        private Dictionary<int, IPermiso> ConstruirIndiceCatalogo(IUnidadDeTrabajo unidad)
        {
            List<IPermiso> nodos = unidad.Permisos.ObtenerNodos();
            List<Tuple<int, int>> relaciones = unidad.Permisos.ObtenerRelaciones();
            List<IPermiso> catalogo = _permisoService.ConstruirArbol(nodos, relaciones).ToList();
            return _permisoService.Aplanar(catalogo).ToDictionary(item => item.Id);
        }

        // Invariante de control interno (T04): ninguna patente puede quedar sin al
        // menos un titular efectivo en todo el sistema (directo o heredado por
        // familia). Es una validacion global, no local al usuario/familia que se
        // esta editando: alcanza con que algun otro usuario activo la cubra.
        private void ValidarNingunaPatenteQuedaSinCobertura(
            Dictionary<int, IPermiso> indiceCatalogo,
            IEnumerable<int> idsPatentesAValidar,
            IEnumerable<List<int>> asignacionesUsuariosActivos,
            string actividadRechazo,
            IUnidadDeTrabajo unidad)
        {
            HashSet<int> patentesCubiertas = new HashSet<int>();
            foreach (List<int> permisosId in asignacionesUsuariosActivos)
            {
                List<IPermiso> nodosAsignados = permisosId
                    .Where(indiceCatalogo.ContainsKey)
                    .Select(id => indiceCatalogo[id])
                    .ToList();

                foreach (IPermiso hoja in _permisoService.ObtenerPermisosEfectivos(nodosAsignados))
                {
                    patentesCubiertas.Add(hoja.Id);
                }
            }

            if (idsPatentesAValidar.Except(patentesCubiertas).Any())
            {
                _bitacoraBLL.RegistrarFalloNegocioInterno(actividadRechazo, "FormAuditLog.Value.Info.PermissionWouldOrphanPatente", "PermisoBLL", unidad);
                throw new ExcepcionNegocio("Error.Permission.OrphanedPatente");
            }
        }

        void IPermisoBLL.AsignarPerfilBasicoInterno(Usuario usuario, IUnidadDeTrabajo unidad)
        {
            AsignarPerfilBasicoInterno(usuario, unidad);
        }

        void IPermisoBLL.GuardarPermisosUsuarioInterno(Usuario usuario, List<IPermiso> permisos, IUnidadDeTrabajo unidad)
        {
            GuardarPermisosUsuarioInterno(usuario, permisos, unidad);
        }

        void IPermisoBLL.ValidarPuedeEliminarUsuario(Usuario usuario, IUnidadDeTrabajo unidad)
        {
            ValidarPuedeEliminarUsuario(usuario, unidad);
        }
    }
}
