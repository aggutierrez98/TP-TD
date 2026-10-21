using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IMedioPagoEntidadRepositorio
    {
        MedioPagoEntidad ObtenerPorId(int id);

        List<MedioPagoEntidad> ObtenerPorEntidadFinanciera(EntidadFinanciera entidad);

        List<MedioPagoEntidad> ObtenerPorMedioPago(MedioPago medioPago);

        // Segmentos ya cargados (sin repetir ni nulos): catalogo del filtro de segmentos de promociones.
        List<string> ObtenerSegmentosDistintos();

        int InsertarYObtenerId(MedioPagoEntidad medioPagoEntidad);

        bool Actualizar(MedioPagoEntidad medioPagoEntidad);

        bool Eliminar(MedioPagoEntidad medioPagoEntidad);
    }
}
