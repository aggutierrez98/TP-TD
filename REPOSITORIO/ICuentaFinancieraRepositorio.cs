using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ICuentaFinancieraRepositorio
    {
        CuentaFinanciera ObtenerPorId(int id);

        List<CuentaFinanciera> ObtenerPorUsuario(int usuarioId);

        List<CuentaFinanciera> ObtenerPorEntidadFinanciera(int entidadFinancieraId);

        int InsertarYObtenerId(CuentaFinanciera cuenta);

        bool Actualizar(CuentaFinanciera cuenta);

        bool Eliminar(int id);

        bool ReemplazarMediosPagoEntidadDeCuenta(int cuentaFinancieraId, List<int> medioPagoEntidadIds);

        int ContarPorMedioPagoEntidad(int medioPagoEntidadId);
    }
}
