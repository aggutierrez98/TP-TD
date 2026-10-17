using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IMedioPagoEntidadRepositorio
    {
        MedioPagoEntidad ObtenerPorId(int id);

        List<MedioPagoEntidad> ObtenerPorEntidadFinanciera(int entidadFinancieraId);

        List<MedioPagoEntidad> ObtenerPorMedioPago(int medioPagoId);

        // Segmentos ya cargados (sin repetir ni nulos): catalogo del filtro de segmentos de promociones.
        List<string> ObtenerSegmentosDistintos();

        int InsertarYObtenerId(MedioPagoEntidad medioPagoEntidad);

        bool Actualizar(MedioPagoEntidad medioPagoEntidad);

        bool Eliminar(int id);
    }
}
