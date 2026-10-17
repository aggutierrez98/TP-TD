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

        bool ActualizarValorAutomatico(int id, decimal calculoConversion);

        bool MarcarManual(int id, int usuarioId);

        bool Liberar(int id);

        bool Eliminar(int id);
    }
}
