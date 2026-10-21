using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ITasaConversionRepositorio
    {
        TasaConversion ObtenerPorId(int id);

        List<TasaConversion> ObtenerTodas();

        TasaConversion ObtenerPorParDeMonedas(Moneda monedaBase, Moneda monedaConversion);

        int InsertarYObtenerId(TasaConversion tasa);

        bool Actualizar(TasaConversion tasa);

        bool ActualizarValorAutomatico(TasaConversion tasa, decimal calculoConversion);

        bool MarcarManual(TasaConversion tasa, Usuario usuario);

        bool Liberar(TasaConversion tasa);

        bool Eliminar(TasaConversion tasa);
    }
}
