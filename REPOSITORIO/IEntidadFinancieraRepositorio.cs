using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IEntidadFinancieraRepositorio
    {
        // Hidrata el catalogo completo MediosPagoEntidad de la entidad.
        EntidadFinanciera ObtenerPorId(int id);

        // Liviano (sin MediosPagoEntidad): pensado para grillas y combos.
        List<EntidadFinanciera> ObtenerTodas();

        int InsertarYObtenerId(EntidadFinanciera entidad);

        bool Actualizar(EntidadFinanciera entidad);

        bool Eliminar(int id);
    }
}
