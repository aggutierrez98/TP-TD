using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class PermisoBLL
    {
        private readonly PermisoDAL _permisoDAL;
        private readonly UsuarioPermisoDAL _usuarioPermisoDAL;
        private readonly PermisoService _permisoService;
        private readonly UsuarioDAL _usuarioDAL;
        private readonly UsuarioEstadoBLL _usuarioEstadoBLL;

        public PermisoBLL()
        {
            BootstrapAplicacionBLL.Inicializar();
            _permisoDAL = new PermisoDAL();
            _usuarioPermisoDAL = new UsuarioPermisoDAL();
            _permisoService = new PermisoService();
            _usuarioDAL = new UsuarioDAL();
            _usuarioEstadoBLL = new UsuarioEstadoBLL();
        }

        public List<object> ObtenerCatalogo()
        {
            List<object> nodos = _permisoDAL.ObtenerNodos();
            List<Tuple<int, int>> relaciones = _permisoDAL.ObtenerRelaciones();
            return _permisoService.ConstruirArbol(nodos, relaciones).ToList();
        }

        public void GuardarCatalogo(List<object> raices)
        {
            if (raices == null || raices.Count == 0)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedPermissionCatalogUpdate", "FormAuditLog.Value.Info.EmptyPermissionCatalog", "PermisoBLL");
                throw new ExcepcionNegocio("Error.Permission.CatalogEmpty");
            }

            _permisoService.ValidarSinCiclos(raices);

            List<object> nodos = _permisoService.Aplanar(raices).ToList();
            foreach (dynamic nodo in nodos)
            {
                _permisoService.ValidarNombre(nodo.Nombre);
                nodo.Nombre = _permisoService.NormalizarNombre(nodo.Nombre);
            }

            _permisoService.ValidarNombresUnicos(raices);
            _permisoService.ValidarGruposNoVacios(nodos);

            List<object> existentes = _permisoDAL.ObtenerNodos();
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    foreach (dynamic nodo in nodos)
                    {
                        nodo.Id = _permisoDAL.Guardar(nodo, accesoDatos);
                    }

                    _permisoDAL.ReemplazarRelaciones(_permisoService.ObtenerRelaciones(raices).ToList(), accesoDatos);

                    List<int> idsActuales = nodos.Select(item => (int)((dynamic)item).Id).Distinct().ToList();
                    List<int> idsEliminados = existentes
                        .Select(item => (int)((dynamic)item).Id)
                        .Where(id => !idsActuales.Contains(id))
                        .ToList();

                    _permisoDAL.EliminarPermisos(idsEliminados, accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.PermissionCatalogUpdate", "FormAuditLog.Value.Info.PermissionCatalogUpdate", "PermisoBLL", accesoDatos);
                    accesoDatos.CommitTransactionScope();
                }
                catch (ExcepcionNegocio)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw;
                }
                catch (Exception ex)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw new ExcepcionTecnica("Error.Permission.SaveCatalogFailed", ex);
                }
            }
        }

        public List<int> ObtenerPermisoIdsDeUsuario(int usuarioId)
        {
            return _usuarioPermisoDAL.ObtenerPermisoIdsPorUsuario(usuarioId);
        }

        public void GuardarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(usuarioId, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Permission.UserNotFound");
                    }

                    _usuarioPermisoDAL.ReemplazarPermisosUsuario(
                        usuarioId,
                        permisosId == null ? new List<int>() : permisosId.Distinct().ToList(),
                        accesoDatos);

                    _usuarioEstadoBLL.GuardarVersionActual(usuarioId, "FormUserHistory.Value.Action.PermissionsUpdate", "FormUserHistory.Value.Obs.PermissionsUpdate", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserPermissionsUpdate", "FormAuditLog.Value.Info.UserPermissionsUpdate", "PermisoBLL", accesoDatos);
                    accesoDatos.CommitTransactionScope();
                }
                catch (ExcepcionNegocio)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw;
                }
                catch (Exception ex)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw new ExcepcionTecnica("Error.Permission.SaveUserPermissionsFailed", ex);
                }
            }

            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == usuarioId)
            {
                AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(ObtenerPermisosDirectosUsuario(usuarioId));
            }
        }

        public List<object> ObtenerPermisosDirectosUsuario(int usuarioId)
        {
            List<int> permisosId = ObtenerPermisoIdsDeUsuario(usuarioId);
            List<object> catalogo = ObtenerCatalogo();
            Dictionary<int, object> indice = _permisoService.Aplanar(catalogo)
                .ToDictionary(item => (int)((dynamic)item).Id, item => item);

            return permisosId
                .Where(indice.ContainsKey)
                .Select(id => indice[id])
                .ToList();
        }

        public void CargarPermisosEnUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                return;
            }

            IList permisos = usuario.PermisosNoTipados;
            if (permisos == null)
            {
                return;
            }

            permisos.Clear();

            foreach (object permiso in ObtenerPermisosDirectosUsuario(usuario.Id))
            {
                permisos.Add(permiso);
            }
        }

        public void AsignarPerfilBasico(int usuarioId, AccesoDatos accesoDatos)
        {
            int perfilBasicoId = _permisoDAL.ObtenerIdPorNombre("basico", accesoDatos);
            if (perfilBasicoId <= 0)
            {
                return;
            }

            _usuarioPermisoDAL.ReemplazarPermisosUsuario(usuarioId, new List<int> { perfilBasicoId }, accesoDatos);
        }
    }
}
