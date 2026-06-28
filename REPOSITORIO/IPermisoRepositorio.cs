using System;
using System.Collections.Generic;
using ABSTRACCIONES;

namespace REPOSITORIO
{
    public interface IPermisoRepositorio
    {
        List<IPermiso> ObtenerNodos();

        List<Tuple<int, int>> ObtenerRelaciones();

        int Guardar(IPermiso permiso);

        bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones);

        bool EliminarPermisos(List<int> idsPermiso);

        int ObtenerIdPorNombre(string nombre);
    }
}
