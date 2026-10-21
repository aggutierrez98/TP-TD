using System;
using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    // Registro tecnico del proceso automatico de notificaciones (corridas y detalle por usuario y tipo).
    public interface IGeneracionNotificacionesRepositorio
    {
        int InsertarCorridaYObtenerId(CorridaGeneracionNotificaciones corrida);

        bool FinalizarCorrida(CorridaGeneracionNotificaciones corrida, DateTime finUtc, int entregadas, int omitidas, int fallidas);

        int InsertarDetalleYObtenerId(DetalleGeneracionNotificacion detalle);

        CorridaGeneracionNotificaciones ObtenerPorId(int id);

        List<CorridaGeneracionNotificaciones> ObtenerRecientes(int cantidad);

        List<DetalleGeneracionNotificacion> ObtenerDetallePorCorrida(CorridaGeneracionNotificaciones corrida);
    }
}
