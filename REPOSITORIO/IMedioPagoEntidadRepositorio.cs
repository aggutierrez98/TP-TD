using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IMedioPagoEntidadRepositorio
    {
        MedioPagoEntidad ObtenerPorId(int id);

        List<MedioPagoEntidad> ObtenerPorEntidadFinanciera(int entidadFinancieraId);

        List<MedioPagoEntidad> ObtenerPorMedioPago(int medioPagoId);

        int InsertarYObtenerId(MedioPagoEntidad medioPagoEntidad);

        bool Actualizar(MedioPagoEntidad medioPagoEntidad);

        bool Eliminar(int id);
    }
}
