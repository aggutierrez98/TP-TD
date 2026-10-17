using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IMedioPagoRepositorio
    {
        MedioPago ObtenerPorId(int id);

        List<MedioPago> ObtenerTodos();

        int InsertarYObtenerId(MedioPago medioPago);

        bool Actualizar(MedioPago medioPago);

        bool Eliminar(int id);
    }
}
