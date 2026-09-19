using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ITasaConversionRepositorio
    {
        TasaConversion ObtenerPorId(int id);

        List<TasaConversion> ObtenerTodas();

        TasaConversion ObtenerPorParDeMonedas(int monedaBaseId, int monedaConversionId);

        int InsertarYObtenerId(TasaConversion tasa);

        bool Actualizar(TasaConversion tasa);

        bool Eliminar(int id);
    }
}
