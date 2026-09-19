using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ICuentaFinancieraRepositorio
    {
        CuentaFinanciera ObtenerPorId(int id);

        List<CuentaFinanciera> ObtenerPorUsuario(int usuarioId);

        int InsertarYObtenerId(CuentaFinanciera cuenta);

        bool Actualizar(CuentaFinanciera cuenta);

        bool Eliminar(int id);
    }
}
