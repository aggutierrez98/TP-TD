using System;
using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IEjecucionActualizacionTasasRepositorio
    {
        int InsertarYObtenerId(EjecucionActualizacionTasas ejecucion);

        int InsertarDetalleYObtenerId(ActualizacionTasaDetalle detalle);

        List<EjecucionActualizacionTasas> ObtenerRecientes(int cantidad);

        List<ActualizacionTasaDetalle> ObtenerDetallePorEjecucion(int ejecucionId);

        EjecucionActualizacionTasas ObtenerUltimaExitosa();

        EjecucionActualizacionTasas ObtenerUltimaExitosaConBloqueo();

        int ContarRequestsDelMes(DateTime referencia);
    }
}
