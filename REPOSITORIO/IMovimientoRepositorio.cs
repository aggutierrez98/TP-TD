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

        List<Movimiento> ObtenerPorUsuario(Usuario usuario, FiltroMovimiento filtro);

        List<TotalPorCategoria> ObtenerTotalesPorCategoria(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta);

        List<TotalPorComercio> ObtenerTotalesPorComercio(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta);

        List<TotalPorDia> ObtenerTotalesPorDia(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta);

        List<TotalPorMes> ObtenerTotalesPorMes(Usuario usuario, DateTime desde, DateTime hasta);

        ResumenMovimientos ObtenerResumen(Usuario usuario, DateTime desde, DateTime hasta);

        int InsertarYObtenerId(Movimiento movimiento);

        bool Actualizar(Movimiento movimiento);

        bool Eliminar(Movimiento movimiento);

        int ContarPorComercio(Comercio comercio);
    }
}
