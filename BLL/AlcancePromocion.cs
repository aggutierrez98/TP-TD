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
        // Comercios y rubros se combinan con Y. Es la regla compartida por el buscador de
        // Gestion de Promociones y por las promociones que muestra el detalle del comercio.
        public static bool AplicaAComercio(Promocion promocion, Comercio comercio)
        {
            int comercioId = comercio == null ? 0 : comercio.Id;
            int rubroId = comercio == null || comercio.RubroComercio == null ? 0 : comercio.RubroComercio.Id;

            return CumpleFiltro(promocion.ComercioIds, comercioId)
                && CumpleFiltro(promocion.RubroComercioIds, rubroId);
        }

        public static List<Promocion> FiltrarPorComercio(IEnumerable<Promocion> promociones, Comercio comercio)
        {
            return promociones.Where(promocion => AplicaAComercio(promocion, comercio)).ToList();
        }

        // Entidad, medio de pago y segmento son filtros independientes: un pago sin entidad
        // (por ejemplo dinero en cuenta) solo cumple si la promocion no filtra por entidad.
        public static bool AplicaAPago(Promocion promocion, int? entidadFinancieraId, int? medioPagoId, string segmento)
        {
            return CumpleFiltro(promocion.EntidadFinancieraIds, entidadFinancieraId)
                && CumpleFiltro(promocion.MedioPagoIds, medioPagoId)
                && CumpleSegmento(promocion.Segmentos, segmento);
        }

        // Con inclusiones solo participa un producto que coincida con algun producto o categoria
        // incluidos; despues, cualquier coincidencia con una exclusion lo deja afuera.
        public static bool AplicaAProducto(Promocion promocion, int productoId, int categoriaProductoId)
        {
            bool hayInclusiones = promocion.ProductoIncluidoIds.Count > 0 || promocion.CategoriaIncluidaIds.Count > 0;
            if (hayInclusiones
                && !promocion.ProductoIncluidoIds.Contains(productoId)
                && !promocion.CategoriaIncluidaIds.Contains(categoriaProductoId))
            {
                return false;
            }

            return !promocion.ProductoExcluidoIds.Contains(productoId)
                && !promocion.CategoriaExcluidaIds.Contains(categoriaProductoId);
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

        private static bool CumpleFiltro(List<int> filtro, int? valor)
        {
            return filtro.Count == 0 || (valor.HasValue && filtro.Contains(valor.Value));
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
