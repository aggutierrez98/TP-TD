using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IUsuarioRepositorio
    {
        Usuario ObtenerPorId(int id, IUnidadDeTrabajo unidad = null);

        Usuario ObtenerPorUsername(string username, IUnidadDeTrabajo unidad = null);

        int InsertarYObtenerId(Usuario usuario, IUnidadDeTrabajo unidad = null);

        bool Actualizar(Usuario usuario, IUnidadDeTrabajo unidad = null);

        bool ActualizarDVH(int id, string dvh, IUnidadDeTrabajo unidad = null);

        bool GuardarEstadoCompleto(Usuario usuario, IUnidadDeTrabajo unidad = null);

        bool EliminarFisicamente(int id, IUnidadDeTrabajo unidad = null);

        List<Usuario> ObtenerTodos(IUnidadDeTrabajo unidad = null);
    }
}
