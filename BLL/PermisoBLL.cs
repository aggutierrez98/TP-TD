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

        public List<PermisoNodo> ObtenerCatalogo()
        {
            List<PermisoNodo> nodos = _permisoDAL.ObtenerNodos();
            List<PermisoRelacion> relaciones = _permisoDAL.ObtenerRelaciones();
            return _permisoService.ConstruirArbol(nodos, relaciones).ToList();
        }

        public void GuardarCatalogo(List<PermisoNodo> raices)
        {
            if (raices == null || raices.Count == 0)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedPermissionCatalogUpdate", "FormAuditLog.Value.Info.EmptyPermissionCatalog", "PermisoBLL");
                throw new ExcepcionNegocio("Error.Permission.CatalogEmpty");
            }

            _permisoService.ValidarSinCiclos(raices);

            List<PermisoNodo> nodos = _permisoService.Aplanar(raices).ToList();
            foreach (PermisoNodo nodo in nodos)
            {
                _permisoService.ValidarNombre(nodo.Nombre);
                nodo.Nombre = nodo.Nombre.Trim();
            }

            _permisoService.ValidarNombresUnicos(raices);
            _permisoService.ValidarGruposNoVacios(nodos);

            List<PermisoNodo> existentes = _permisoDAL.ObtenerNodos();
            foreach (PermisoNodo nodoNuevo in nodos.Where(item => item.Id == 0))
            {
                nodoNuevo.Id = _permisoDAL.InsertarPermiso(nodoNuevo.Nombre);
            }

            foreach (PermisoNodo nodoExistente in nodos.Where(item => item.Id > 0))
            {
                PermisoNodo original = existentes.FirstOrDefault(item => item.Id == nodoExistente.Id);
                if (original != null &&
                    !string.Equals(original.Nombre, nodoExistente.Nombre, StringComparison.OrdinalIgnoreCase))
                {
                    _permisoDAL.ActualizarNombre(nodoExistente.Id, nodoExistente.Nombre);
                }
            }

            _permisoDAL.ReemplazarRelaciones(_permisoService.ObtenerRelaciones(raices).ToList());

            List<int> idsActuales = nodos.Select(item => item.Id).Distinct().ToList();
            List<int> idsEliminados = existentes
                .Select(item => item.Id)
                .Where(id => !idsActuales.Contains(id))
                .ToList();

            _permisoDAL.EliminarPermisos(idsEliminados);
            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.PermissionCatalogUpdate", "FormAuditLog.Value.Info.PermissionCatalogUpdate", "PermisoBLL");
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

        public List<PermisoNodo> ObtenerPermisosDirectosUsuario(int usuarioId)
        {
            List<int> permisosId = ObtenerPermisoIdsDeUsuario(usuarioId);
            List<PermisoNodo> catalogo = ObtenerCatalogo();
            Dictionary<int, PermisoNodo> indice = _permisoService.Aplanar(catalogo).ToDictionary(item => item.Id);

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

            foreach (PermisoNodo permiso in ObtenerPermisosDirectosUsuario(usuario.Id))
            {
                permisos.Add(permiso);
            }
        }
    }
}
