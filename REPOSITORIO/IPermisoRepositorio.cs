using System;
using System.Collections.Generic;
using ABSTRACCIONES;

namespace REPOSITORIO
{
    public interface IPermisoRepositorio
    {
        List<IPermiso> ObtenerNodos(IUnidadDeTrabajo unidad = null);

        List<Tuple<int, int>> ObtenerRelaciones(IUnidadDeTrabajo unidad = null);

        int Guardar(IPermiso permiso, IUnidadDeTrabajo unidad = null);

        bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones, IUnidadDeTrabajo unidad = null);

        bool EliminarPermisos(List<int> idsPermiso, IUnidadDeTrabajo unidad = null);

        int ObtenerIdPorNombre(string nombre, IUnidadDeTrabajo unidad = null);
    }
}
