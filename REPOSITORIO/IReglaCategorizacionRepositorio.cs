using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IReglaCategorizacionRepositorio
    {
        ReglaCategorizacion ObtenerPorId(int id);

        List<ReglaCategorizacion> ObtenerPorUsuario(int usuarioId);

        int InsertarYObtenerId(ReglaCategorizacion reglaCategorizacion);

        bool Actualizar(ReglaCategorizacion reglaCategorizacion);

        bool Eliminar(int id);
    }
}
