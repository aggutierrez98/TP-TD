using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class PermisoBLL
    {
        private readonly PermisoDAL permisoDAL;
        private readonly UsuarioPermisoDAL usuarioPermisoDAL;
        private readonly PermisoService permisoService;

        public PermisoBLL()
        {
            permisoDAL = new PermisoDAL();
            usuarioPermisoDAL = new UsuarioPermisoDAL();
            permisoService = new PermisoService();
        }

        public List<PermisoNodo> ObtenerCatalogo()
        {
            List<PermisoNodo> nodos = permisoDAL.ObtenerNodos();
            List<PermisoRelacion> relaciones = permisoDAL.ObtenerRelaciones();
            return permisoService.ConstruirArbol(nodos, relaciones).ToList();
        }

        public void GuardarCatalogo(List<PermisoNodo> raices)
        {
            if (raices == null || raices.Count == 0)
            {
                throw new Exception("Debe existir al menos un permiso o grupo.");
            }

            permisoService.ValidarSinCiclos(raices);

            List<PermisoNodo> nodos = permisoService.Aplanar(raices).ToList();
            foreach (PermisoNodo nodo in nodos)
            {
                permisoService.ValidarNombre(nodo.Nombre);
                nodo.Nombre = nodo.Nombre.Trim();
            }

            permisoService.ValidarNombresUnicos(nodos);
            permisoService.ValidarGruposNoVacios(nodos);

            List<PermisoNodo> existentes = permisoDAL.ObtenerNodos();
            foreach (PermisoNodo nodoNuevo in nodos.Where(item => item.Id == 0))
            {
                nodoNuevo.Id = permisoDAL.InsertarPermiso(nodoNuevo.Nombre);
            }

            foreach (PermisoNodo nodoExistente in nodos.Where(item => item.Id > 0))
            {
                PermisoNodo original = existentes.FirstOrDefault(item => item.Id == nodoExistente.Id);
                if (original != null &&
                    !string.Equals(original.Nombre, nodoExistente.Nombre, StringComparison.OrdinalIgnoreCase))
                {
                    permisoDAL.ActualizarNombre(nodoExistente.Id, nodoExistente.Nombre);
                }
            }

            permisoDAL.ReemplazarRelaciones(permisoService.ObtenerRelaciones(raices).ToList());

            List<int> idsActuales = nodos.Select(item => item.Id).Distinct().ToList();
            List<int> idsEliminados = existentes
                .Select(item => item.Id)
                .Where(id => !idsActuales.Contains(id))
                .ToList();

            permisoDAL.EliminarPermisos(idsEliminados);
        }

        public List<int> ObtenerPermisoIdsDeUsuario(int usuarioId)
        {
            return usuarioPermisoDAL.ObtenerPermisoIdsPorUsuario(usuarioId);
        }

        public void GuardarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            usuarioPermisoDAL.ReemplazarPermisosUsuario(
                usuarioId,
                permisosId == null ? new List<int>() : permisosId.Distinct().ToList());
        }

        public object ObtenerPermisosDirectosUsuario(int usuarioId)
        {
            List<int> permisosId = ObtenerPermisoIdsDeUsuario(usuarioId);
            List<PermisoNodo> catalogo = ObtenerCatalogo();
            Dictionary<int, PermisoNodo> indice = permisoService.Aplanar(catalogo).ToDictionary(item => item.Id);

            List<PermisoNodo> permisos = permisosId
                .Where(indice.ContainsKey)
                .Select(id => indice[id])
                .ToList();

            return permisoService.CrearColeccionPermisos(permisos);
        }

        public void CargarPermisosEnUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                return;
            }

            object permisos = ObtenerPermisosDirectosUsuario(usuario.Id);
            PropertyInfo propiedad = typeof(Usuario).GetProperty("Permisos");
            if (propiedad != null)
            {
                propiedad.SetValue(usuario, permisos);
            }
        }
    }
}
