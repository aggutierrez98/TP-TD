using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IEntidadFinancieraRepositorio
    {
        EntidadFinanciera ObtenerPorId(int id);

        List<EntidadFinanciera> ObtenerTodas();

        int InsertarYObtenerId(EntidadFinanciera entidad);

        bool Actualizar(EntidadFinanciera entidad);

        bool Eliminar(int id);
    }
}
