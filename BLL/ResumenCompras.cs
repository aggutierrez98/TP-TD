using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using ABSTRACCIONES;
using SERVICIOS;

namespace BLL
{
    // Proyecciones cerradas: ninguna entidad persistente (ni credenciales) se serializa al modelo.
    internal sealed class ResumenCompras
    {
        // Items pendientes que se analizan por consulta; el resto se informa como omitido.
        internal const int MaximoItemsAnalizados = 20;

        public bool SinZonas { get; set; }
        public int ItemsOmitidos { get; set; }
        public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();

        // Rechaza fechas explicitas inventadas por el redactor (incluidas las de fuera
        // de la ventana de siete dias). Las fechas de tasas tambien pertenecen al resumen.
        internal bool TieneFechasNoPermitidas(string respuesta)
        {
            List<DateTime> fechas = Items.SelectMany(item => item.Ofertas).SelectMany(oferta => oferta.Promociones.Select(p => p.Fecha)
                .Concat(oferta.FechaTasa.HasValue ? new[] { oferta.FechaTasa.Value } : new DateTime[0])).Select(fecha => fecha.Date).Distinct().ToList();
            foreach (Match coincidencia in Regex.Matches(respuesta ?? string.Empty, @"(?<!\d)\d{4}-\d{2}-\d{2}(?!\d)|(?<!\d)\d{1,2}/\d{1,2}(?:/\d{4})?(?!\d)"))
            {
                string texto = coincidencia.Value;
                if (texto.Count(c => c == '/') == 1)
                {
                    if (!fechas.Any(fecha => fecha.ToString("d/M", CultureInfo.InvariantCulture) == texto || fecha.ToString("dd/MM", CultureInfo.InvariantCulture) == texto)) return true;
                    continue;
                }
                DateTime fechaExplicita;
                if (!DateTime.TryParseExact(texto, new[] { "yyyy-MM-dd", "d/M/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaExplicita)
                    || !fechas.Contains(fechaExplicita.Date)) return true;
            }
            return false;
        }

        internal IContextoAsistente CrearContexto()
        {
            List<IReferenciaAsistente> referencias = new List<IReferenciaAsistente>();
            foreach (OfertaCompra oferta in Items.SelectMany(item => item.Ofertas))
            {
                referencias.Add(new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, oferta.Comercio));
                foreach (PromocionCompra promocion in oferta.Promociones)
                {
                    referencias.Add(new ReferenciaAsistente(TipoReferenciaAsistente.Promocion, promocion.Descripcion));
                    referencias.Add(new ReferenciaAsistente(TipoReferenciaAsistente.Cuenta, promocion.Cuenta));
                }
            }
            return new ContextoAsistente(JsonSerializer.Serialize(this), referencias);
        }
    }

    internal sealed class ItemCompra
    {
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public bool NoEncontrado { get; set; }
        public List<OfertaCompra> Ofertas { get; set; } = new List<OfertaCompra>();
    }

    internal sealed class OfertaCompra
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int ComercioId { get; set; }
        public string Producto { get; set; }
        public string Comercio { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
        public string Moneda { get; set; }
        public string Zona { get; set; }
        public int? DistanciaMetros { get; set; }
        public bool SinUbicacionFisica { get; set; }
        public bool SinTasa { get; set; }
        public decimal? TasaAArs { get; set; }
        public DateTime? FechaTasa { get; set; }
        public decimal? PrecioFinalArs { get; set; }
        public List<PromocionCompra> Promociones { get; set; } = new List<PromocionCompra>();
    }

    internal sealed class PromocionCompra
    {
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal? TopeArs { get; set; }
        public string MedioPago { get; set; }
        public string TipoMedioPago { get; set; }
        public string Segmento { get; set; }
        public string Cuenta { get; set; }
        public decimal? Ahorro { get; set; }
        public decimal? PrecioFinal { get; set; }
    }
}
