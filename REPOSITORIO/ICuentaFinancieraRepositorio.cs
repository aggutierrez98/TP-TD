using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ICuentaFinancieraRepositorio
    {
        CuentaFinanciera ObtenerPorId(int id);

        List<CuentaFinanciera> ObtenerPorUsuario(Usuario usuario);

        List<CuentaFinanciera> ObtenerPorEntidadFinanciera(EntidadFinanciera entidad);

        int InsertarYObtenerId(CuentaFinanciera cuenta);

        bool Actualizar(CuentaFinanciera cuenta);

        bool Eliminar(CuentaFinanciera cuenta);

        bool ReemplazarMediosPagoEntidadDeCuenta(CuentaFinanciera cuenta, IEnumerable<MedioPagoEntidad> medios);

        int ContarPorMedioPagoEntidad(MedioPagoEntidad medio);
    }
}
