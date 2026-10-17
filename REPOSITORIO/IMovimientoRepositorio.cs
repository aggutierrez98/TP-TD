using System;
using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IMovimientoRepositorio
    {
        Movimiento ObtenerPorId(int id);

        List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro);

        List<TotalPorCategoria> ObtenerTotalesPorCategoria(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta);

        List<TotalPorComercio> ObtenerTotalesPorComercio(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta);

        List<TotalPorDia> ObtenerTotalesPorDia(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta);

        List<TotalPorMes> ObtenerTotalesPorMes(int usuarioId, DateTime desde, DateTime hasta);

        ResumenMovimientos ObtenerResumen(int usuarioId, DateTime desde, DateTime hasta);

        int InsertarYObtenerId(Movimiento movimiento);

        bool Actualizar(Movimiento movimiento);

        bool Eliminar(int id);

        int ContarPorComercio(int comercioId);
    }
}
