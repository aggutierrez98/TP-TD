using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionMedioPagoRepositorio
    {
        List<MedioPago> ObtenerMediosPagoPorPromocion(Promocion promocion);

        bool ReemplazarMediosPagoDePromocion(Promocion promocion, List<MedioPago> mediosPago);

        int ContarPorMedioPago(MedioPago medioPago);
    }
}
