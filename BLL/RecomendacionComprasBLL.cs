using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;

namespace BLL
{
    internal sealed class RecomendacionComprasBLL : IRecomendacionComprasBLL
    {
        private readonly IRecomendacionComprasRepositorio _repositorio;
        private readonly IZonaUsuarioRepositorio _zonas;
        private readonly ITasaConversionRepositorio _tasas;
        private readonly CoincidenciaSemanticaProductos _coincidencia;
        private readonly Func<DateTime> _ahora;

        internal RecomendacionComprasBLL(IRecomendacionComprasRepositorio repositorio, IZonaUsuarioRepositorio zonas, ITasaConversionRepositorio tasas, CoincidenciaSemanticaProductos coincidencia, Func<DateTime> ahora)
        {
            _repositorio = repositorio;
            _zonas = zonas;
            _tasas = tasas;
            _coincidencia = coincidencia;
            _ahora = ahora;
        }

        public async Task<ResumenCompras> CalcularAsync(int usuarioId, string nombreLista, CancellationToken cancellationToken)
        {
            // Id capturado al enviar por AsistenteFinancieroBLL, nunca un argumento del modelo.
            // No se vuelve a consultar la sesion despues de esperar una llamada de red.
            DateTime desde = _ahora().Date;
            List<ItemLista> items = _repositorio.ObtenerItemsPendientesPorUsuario(usuarioId)
                .Where(item => !item.Comprado && item.ListaDeCompras?.Usuario?.Id == usuarioId).OrderBy(item => item.Id).ToList();
            if (!string.IsNullOrWhiteSpace(nombreLista))
            {
                List<ItemLista> seleccionados = items.Where(item => CoincidenciaLocalProductos.Normalizar(item.ListaDeCompras.Nombre) == CoincidenciaLocalProductos.Normalizar(nombreLista)).ToList();
                if (seleccionados.Count > 0) items = seleccionados;
            }
            List<ZonaUsuario> zonas = _zonas.ObtenerPorUsuario(usuarioId).Where(zona => zona.Usuario?.Id == usuarioId).ToList();
            ResumenCompras resumen = new ResumenCompras { SinZonas = zonas.Count == 0, ItemsOmitidos = Math.Max(0, items.Count - ResumenCompras.MaximoItemsAnalizados) };
            items = items.Take(ResumenCompras.MaximoItemsAnalizados).ToList();
            List<Producto> productos = _repositorio.ObtenerProductosConCategoria();
            List<ComercioProducto> ofertas = _repositorio.ObtenerOfertas();
            List<Promocion> promociones = _repositorio.ObtenerPromocionesVigentesEntre(desde, desde.AddDays(6));
            List<CuentaFinanciera> cuentas = _repositorio.ObtenerCuentasConMediosPorUsuario(usuarioId).Where(cuenta => cuenta.Usuario?.Id == usuarioId).ToList();
            List<TasaConversion> tasas = _tasas.ObtenerTodas();
            Dictionary<int, List<CoincidenciaProducto>> coincidencias = items.ToDictionary(item => item.Id, item => CoincidenciaLocalProductos.Buscar(item.Descripcion, productos));
            List<ItemLista> sinCoincidencia = items.Where(item => coincidencias[item.Id].Count == 0).ToList();
            Dictionary<int, List<CoincidenciaProducto>> semanticas = await _coincidencia.BuscarAsync(sinCoincidencia, productos, cancellationToken).ConfigureAwait(false);
            foreach (var par in semanticas) coincidencias[par.Key] = par.Value;
            foreach (ItemLista item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                HashSet<int> ids = new HashSet<int>(coincidencias[item.Id].Select(coincidencia => coincidencia.Producto.Id));
                List<OfertaCompra> candidatas = new List<OfertaCompra>();
                foreach (ComercioProducto oferta in ofertas.Where(oferta => ids.Contains(oferta.Producto.Id)))
                {
                    DistanciaZonaUsuario cercana = null;
                    bool sinUbicacion = !oferta.Comercio.Latitud.HasValue || !oferta.Comercio.Longitud.HasValue;
                    if (!sinUbicacion && zonas.Count > 0)
                    {
                        cercana = ZonaUsuarioBLL.CalcularDistancias(zonas, oferta.Comercio.Latitud.Value, oferta.Comercio.Longitud.Value).FirstOrDefault(zona => zona.EstaDentroDelRadio);
                        if (cercana == null) continue;
                    }
                    OfertaCompra candidata = CrearOferta(oferta, item.Cantidad, cercana, sinUbicacion, tasas);
                    foreach (Promocion promocion in promociones.Where(p => AlcancePromocion.AplicaAComercio(p, oferta.Comercio) && AlcancePromocion.AplicaAProducto(p, oferta.Producto.Id, oferta.Producto.CategoriaProducto?.Id ?? 0)))
                    {
                        foreach (CuentaFinanciera cuenta in cuentas)
                        foreach (MedioPagoEntidad medio in cuenta.MediosPagoEntidad)
                        {
                            if (!AlcancePromocion.AplicaAPago(promocion, cuenta.EntidadFinanciera?.Id, medio.MedioPago?.Id, medio.Segmento)) continue;
                            for (int dia = 0; dia < 7; dia++)
                            {
                                DateTime fecha = desde.AddDays(dia);
                                if (!AlcancePromocion.Vigente(promocion, fecha)) continue;
                                decimal? ahorro = null;
                                if (candidata.TasaAArs.HasValue)
                                {
                                    decimal ahorroArs = candidata.Importe * candidata.TasaAArs.Value * promocion.Descuento / 100m;
                                    if (promocion.TopeDescuento.HasValue) ahorroArs = Math.Min(ahorroArs, promocion.TopeDescuento.Value);
                                    // Redondeo conservador en la moneda original: subir un centavo
                                    // podria prometer un ahorro que excede el tope expresado en ARS.
                                    ahorro = Math.Floor(ahorroArs / candidata.TasaAArs.Value * 100m) / 100m;
                                }
                                candidata.Promociones.Add(new PromocionCompra
                                {
                                    Descripcion = promocion.Descripcion, Fecha = fecha, Porcentaje = promocion.Descuento, TopeArs = promocion.TopeDescuento,
                                    Cuenta = cuenta.Alias, MedioPago = medio.MedioPago?.Marca, TipoMedioPago = medio.MedioPago?.Tipo.ToString(), Segmento = medio.Segmento,
                                    Ahorro = ahorro, PrecioFinal = ahorro.HasValue ? candidata.Importe - ahorro.Value : (decimal?)null
                                });
                            }
                        }
                    }
                    if (candidata.TasaAArs.HasValue)
                    {
                        decimal mejor = candidata.Promociones.Where(p => p.PrecioFinal.HasValue).Select(p => p.PrecioFinal.Value).DefaultIfEmpty(candidata.Importe).Min();
                        candidata.PrecioFinalArs = mejor * candidata.TasaAArs.Value;
                    }
                    candidatas.Add(candidata);
                }
                // Un comercio puede vender varias coincidencias: se conserva su mejor alternativa.
                List<OfertaCompra> elegidas = Ordenar(candidatas).GroupBy(o => o.ComercioId).Select(grupo => grupo.First()).Take(3).ToList();
                resumen.Items.Add(new ItemCompra { Descripcion = item.Descripcion, Cantidad = item.Cantidad, NoEncontrado = elegidas.Count == 0, Ofertas = elegidas });
            }
            return resumen;
        }

        public IReadOnlyList<ReferenciaAsistente> ObtenerUniversoReferencias(int usuarioId)
        {
            // Catalogos compartidos, mas las cuentas propias. Nunca se leen cuentas de terceros.
            DateTime hoy = _ahora().Date;
            return _repositorio.ObtenerOfertas().Select(o => new ReferenciaAsistente(TipoReferenciaAsistente.Comercio, o.Comercio.Nombre))
                .Concat(_repositorio.ObtenerPromocionesVigentesEntre(hoy, hoy.AddDays(6)).Select(p => new ReferenciaAsistente(TipoReferenciaAsistente.Promocion, p.Descripcion)))
                .Concat(_repositorio.ObtenerCuentasConMediosPorUsuario(usuarioId).Where(c => c.Usuario?.Id == usuarioId).Select(c => new ReferenciaAsistente(TipoReferenciaAsistente.Cuenta, c.Alias))).ToList();
        }

        private static IOrderedEnumerable<OfertaCompra> Ordenar(IEnumerable<OfertaCompra> ofertas)
        {
            return ofertas.OrderBy(o => !o.PrecioFinalArs.HasValue).ThenBy(o => o.PrecioFinalArs).ThenBy(o => o.DistanciaMetros ?? int.MaxValue).ThenBy(o => o.ComercioId);
        }

        private static OfertaCompra CrearOferta(ComercioProducto oferta, int cantidad, DistanciaZonaUsuario zona, bool sinUbicacion, List<TasaConversion> tasas)
        {
            string moneda = oferta.Moneda.Codigo;
            decimal? tasa = null;
            DateTime? fecha = null;
            if (string.Equals(moneda, "ARS", StringComparison.OrdinalIgnoreCase)) tasa = 1;
            else
            {
                TasaConversion directa = tasas.FirstOrDefault(t => t.CalculoConversion > 0 && EsMoneda(t.MonedaBase, moneda) && EsMoneda(t.MonedaConversion, "ARS"));
                TasaConversion inversa = tasas.FirstOrDefault(t => t.CalculoConversion > 0 && EsMoneda(t.MonedaBase, "ARS") && EsMoneda(t.MonedaConversion, moneda));
                if (directa != null) { tasa = directa.CalculoConversion; fecha = directa.FechaActualizacion; }
                else if (inversa != null) { tasa = 1m / inversa.CalculoConversion; fecha = inversa.FechaActualizacion; }
            }
            return new OfertaCompra
            {
                ComercioId = oferta.Comercio.Id, Comercio = oferta.Comercio.Nombre, Producto = oferta.Producto.Nombre,
                PrecioUnitario = oferta.Precio, Importe = oferta.Precio * cantidad, Moneda = moneda,
                SinUbicacionFisica = sinUbicacion, Zona = zona?.Zona.Nombre,
                DistanciaMetros = zona == null ? (int?)null : (int)(Math.Round(zona.DistanciaMetros / 100, MidpointRounding.AwayFromZero) * 100),
                TasaAArs = tasa, FechaTasa = fecha, SinTasa = !tasa.HasValue
            };
        }

        private static bool EsMoneda(Moneda moneda, string codigo) => string.Equals(moneda?.Codigo, codigo, StringComparison.OrdinalIgnoreCase);
    }
}
