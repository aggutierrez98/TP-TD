using System.Collections.Generic;
using System.Linq;
using BE;
using REPOSITORIO;

namespace BLL
{
    // Solo lectura por ahora: las Promociones las genera un proceso automatico a futuro
    // (fuera de esta tanda). No expone Crear/Actualizar/Eliminar.
    public class PromocionBLL
    {
        private readonly IPromocionRepositorio _promocionRepositorio;
        private readonly IPromocionComercioRepositorio _promocionComercioRepositorio;

        internal PromocionBLL(
            IPromocionRepositorio promocionRepositorio,
            IPromocionComercioRepositorio promocionComercioRepositorio)
        {
            _promocionRepositorio = promocionRepositorio;
            _promocionComercioRepositorio = promocionComercioRepositorio;
        }

        public List<Promocion> ObtenerPorComercio(int comercioId)
        {
            List<int> promocionIds = _promocionComercioRepositorio.ObtenerPromocionIdsPorComercio(comercioId);

            return promocionIds
                .Select(promocionId => _promocionRepositorio.ObtenerPorId(promocionId))
                .Where(promocion => promocion != null)
                .OrderByDescending(promocion => promocion.VigenciaDesde)
                .ToList();
        }
    }
}
