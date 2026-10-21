using System;
using System.Collections.Generic;
using System.Linq;
using BE;

namespace BLL
{
    // Regla de alcance de una promocion, sin acceso a datos: cada filtro vacio no restringe nada,
    // los filtros con valores se combinan con Y y las opciones de un mismo filtro con O.
    internal static class AlcancePromocion
    {
        public static bool AplicaAComercio(Promocion promocion, Comercio comercio)
        {
            int comercioId = comercio == null ? 0 : comercio.Id;
            int rubroId = comercio == null || comercio.RubroComercio == null ? 0 : comercio.RubroComercio.Id;

            return CumpleFiltro(promocion.Comercios.Select(item => item.Id), comercioId)
                && CumpleFiltro(promocion.RubrosComercio.Select(item => item.Id), rubroId);
        }

        public static List<Promocion> FiltrarPorComercio(IEnumerable<Promocion> promociones, Comercio comercio)
        {
            return promociones.Where(promocion => AplicaAComercio(promocion, comercio)).ToList();
        }

        public static bool AplicaAPago(Promocion promocion, EntidadFinanciera entidadFinanciera, MedioPago medioPago, string segmento)
        {
            return CumpleFiltro(promocion.EntidadesFinancieras.Select(item => item.Id), entidadFinanciera?.Id)
                && CumpleFiltro(promocion.MediosPago.Select(item => item.Id), medioPago?.Id)
                && CumpleSegmento(promocion.Segmentos, segmento);
        }

        public static bool AplicaAProducto(Promocion promocion, Producto producto)
        {
            bool hayInclusiones = promocion.ProductosIncluidos.Count > 0 || promocion.CategoriasIncluidas.Count > 0;
            if (hayInclusiones
                && !promocion.ProductosIncluidos.Any(item => item.Id == producto?.Id)
                && !promocion.CategoriasIncluidas.Any(item => item.Id == producto?.CategoriaProducto?.Id))
            {
                return false;
            }

            return !promocion.ProductosExcluidos.Any(item => item.Id == producto?.Id)
                && !promocion.CategoriasExcluidas.Any(item => item.Id == producto?.CategoriaProducto?.Id);
        }

        // Vigencia con ambos extremos inclusivos (por dia) y, si hay dias cargados, solo esos dias.
        public static bool Vigente(Promocion promocion, DateTime fecha)
        {
            DateTime dia = fecha.Date;
            if (dia < promocion.VigenciaDesde.Date || dia > promocion.VigenciaHasta.Date)
            {
                return false;
            }

            return promocion.DiasSemana.Count == 0 || promocion.DiasSemana.Contains(ADiaSemana(dia.DayOfWeek));
        }

        private static bool CumpleFiltro(IEnumerable<int> filtro, int? valor)
        {
            return !filtro.Any() || (valor.HasValue && filtro.Contains(valor.Value));
        }

        private static bool CumpleSegmento(List<string> filtro, string segmento)
        {
            return filtro.Count == 0
                || (!string.IsNullOrWhiteSpace(segmento)
                    && filtro.Any(item => string.Equals(item, segmento.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        private static DiaSemana ADiaSemana(DayOfWeek dia)
        {
            switch (dia)
            {
                case DayOfWeek.Monday: return DiaSemana.Lunes;
                case DayOfWeek.Tuesday: return DiaSemana.Martes;
                case DayOfWeek.Wednesday: return DiaSemana.Miercoles;
                case DayOfWeek.Thursday: return DiaSemana.Jueves;
                case DayOfWeek.Friday: return DiaSemana.Viernes;
                case DayOfWeek.Saturday: return DiaSemana.Sabado;
                default: return DiaSemana.Domingo;
            }
        }
    }
}
