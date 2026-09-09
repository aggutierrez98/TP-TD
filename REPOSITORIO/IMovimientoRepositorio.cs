using System.Collections.Generic;
using BE;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IMovimientoRepositorio
    {
        Movimiento ObtenerPorId(int id);

        List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro);

        int InsertarYObtenerId(Movimiento movimiento);

        bool Actualizar(Movimiento movimiento);

        bool Eliminar(int id);
    }
}
