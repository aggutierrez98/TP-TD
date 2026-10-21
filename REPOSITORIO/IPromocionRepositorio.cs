using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionRepositorio
    {
        Promocion ObtenerPorId(int id);

        List<Promocion> ObtenerTodas();

        int InsertarYObtenerId(Promocion promocion);

        bool Actualizar(Promocion promocion);

        bool Eliminar(Promocion promocion);

        bool ReemplazarDiasDePromocion(Promocion promocion, List<DiaSemana> dias);
    }
}
