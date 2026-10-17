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

        bool Eliminar(int id);

        bool ReemplazarDiasDePromocion(int promocionId, List<DiaSemana> dias);
    }
}
