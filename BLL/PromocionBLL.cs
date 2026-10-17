using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office): ABM de promociones con filtros opcionales. La regla de
    // "a quien le aplica una promocion" vive en AlcancePromocion; aca solo se orquesta.
    // La baja se bloquea con un chequeo explicito de uso (no se espera a que falle una FK).
    public class PromocionBLL
    {
        private readonly IPromocionRepositorio _promocionRepositorio;
        private readonly IComercioRepositorio _comercioRepositorio;
        private readonly IMedioPagoRepositorio _medioPagoRepositorio;
        private readonly IMedioPagoEntidadRepositorio _medioPagoEntidadRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal PromocionBLL(
            IPromocionRepositorio promocionRepositorio,
            IComercioRepositorio comercioRepositorio,
            IMedioPagoRepositorio medioPagoRepositorio,
            IMedioPagoEntidadRepositorio medioPagoEntidadRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _promocionRepositorio = promocionRepositorio;
            _comercioRepositorio = comercioRepositorio;
            _medioPagoRepositorio = medioPagoRepositorio;
            _medioPagoEntidadRepositorio = medioPagoEntidadRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        // Busqueda en memoria (el volumen es chico): texto sobre la descripcion, sin distinguir
        // mayusculas ni acentos, y/o promociones que le aplican a un comercio. Ambos con Y.
        public List<Promocion> Buscar(string texto, int? comercioId)
        {
            Comercio comercio = null;
            if (comercioId.HasValue)
            {
                comercio = _comercioRepositorio.ObtenerPorId(comercioId.Value);
                if (comercio == null)
                {
                    throw new ExcepcionNegocio("Error.PromotionManagement.ComercioNotFound");
                }
            }

            IEnumerable<Promocion> promociones = _promocionRepositorio.ObtenerTodas();

            string textoNormalizado = NormalizarParaBusqueda(texto);
            if (textoNormalizado.Length > 0)
            {
                promociones = promociones.Where(promocion => NormalizarParaBusqueda(promocion.Descripcion).Contains(textoNormalizado));
            }

            if (comercio != null)
            {
                promociones = AlcancePromocion.FiltrarPorComercio(promociones, comercio);
            }

            return Ordenar(promociones);
        }

        // Misma regla que el buscador por comercio: las promociones que muestra el detalle del
        // comercio son las que le aplican segun su filtro de comercios y de rubros.
        public List<Promocion> ObtenerPorComercio(int comercioId)
        {
            Comercio comercio = _comercioRepositorio.ObtenerPorId(comercioId);
            if (comercio == null)
            {
                return new List<Promocion>();
            }

            return Ordenar(AlcancePromocion.FiltrarPorComercio(_promocionRepositorio.ObtenerTodas(), comercio));
        }

        // Catalogos del editor: la UI no referencia REPOSITORIO.
        public List<MedioPago> ObtenerMediosPago()
        {
            return _medioPagoRepositorio.ObtenerTodos();
        }

        public List<string> ObtenerSegmentosDisponibles()
        {
            return _medioPagoEntidadRepositorio.ObtenerSegmentosDistintos();
        }

        public Promocion Crear(Promocion datos)
        {
            Promocion promocion = Normalizar(datos);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarReferencias(unidad, promocion);

                    promocion.Id = unidad.Promociones.InsertarYObtenerId(promocion);
                    ReemplazarFiltros(unidad, promocion, "Error.PromotionManagement.PromocionCreateFailed");

                    _bitacoraBLL.RegistrarOperacionInterno("FormPromotionManagement.Value.Activity.PromocionCreation", "FormPromotionManagement.Value.Info.PromocionCreation", "PromocionBLL", unidad);
                    unidad.Save();
                    return promocion;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.PromotionManagement.PromocionCreateFailed", ex);
                }
            }
        }

        public bool Actualizar(Promocion datos)
        {
            Promocion promocion = Normalizar(datos);
            promocion.Id = datos.Id;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.Promociones.ObtenerPorId(promocion.Id) == null)
                    {
                        throw new ExcepcionNegocio("Error.PromotionManagement.PromocionNotFound");
                    }

                    ValidarReferencias(unidad, promocion);

                    if (!unidad.Promociones.Actualizar(promocion))
                    {
                        throw new ExcepcionTecnica("Error.PromotionManagement.PromocionUpdateFailed");
                    }

                    ReemplazarFiltros(unidad, promocion, "Error.PromotionManagement.PromocionUpdateFailed");

                    _bitacoraBLL.RegistrarOperacionInterno("FormPromotionManagement.Value.Activity.PromocionUpdate", "FormPromotionManagement.Value.Info.PromocionUpdate", "PromocionBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.PromotionManagement.PromocionUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.Promociones.ObtenerPorId(id) == null)
                    {
                        throw new ExcepcionNegocio("Error.PromotionManagement.PromocionNotFound");
                    }

                    // Una promocion que una notificacion o recordatorio referencia no se da de baja.
                    if (unidad.Notificaciones.ContarPorPromocion(id) > 0)
                    {
                        throw new ExcepcionNegocio("Error.PromotionManagement.PromocionInUse");
                    }

                    // Los filtros se borran primero: sus FK no tienen ON DELETE.
                    ReemplazarFiltros(unidad, new Promocion { Id = id }, "Error.PromotionManagement.PromocionDeleteFailed");

                    if (!unidad.Promociones.Eliminar(id))
                    {
                        throw new ExcepcionTecnica("Error.PromotionManagement.PromocionDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormPromotionManagement.Value.Activity.PromocionDeletion", "FormPromotionManagement.Value.Info.PromocionDeletion", "PromocionBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.PromotionManagement.PromocionDeleteFailed", ex);
                }
            }
        }

        // Validaciones que no necesitan datos: se rechazan antes de abrir la unidad de trabajo.
        // Devuelve una copia normalizada (texto recortado, ids y segmentos sin repetir).
        private static Promocion Normalizar(Promocion datos)
        {
            if (datos == null)
            {
                throw new ArgumentNullException("datos");
            }

            string descripcion = (datos.Descripcion ?? string.Empty).Trim();
            if (descripcion.Length == 0)
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.DescriptionRequired");
            }

            if (datos.Descuento <= 0m || datos.Descuento > 100m)
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.DiscountInvalid");
            }

            if (datos.TopeDescuento.HasValue && datos.TopeDescuento.Value <= 0m)
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.CapInvalid");
            }

            if (datos.VigenciaDesde.Date > datos.VigenciaHasta.Date)
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.VigenciaInvalid");
            }

            Promocion promocion = new Promocion
            {
                Descripcion = descripcion,
                Descuento = datos.Descuento,
                TopeDescuento = datos.TopeDescuento,
                VigenciaDesde = datos.VigenciaDesde.Date,
                VigenciaHasta = datos.VigenciaHasta.Date,
                DiasSemana = (datos.DiasSemana ?? new List<DiaSemana>()).Distinct().ToList(),
                ComercioIds = Distintos(datos.ComercioIds),
                RubroComercioIds = Distintos(datos.RubroComercioIds),
                EntidadFinancieraIds = Distintos(datos.EntidadFinancieraIds),
                MedioPagoIds = Distintos(datos.MedioPagoIds),
                Segmentos = SegmentosDistintos(datos.Segmentos),
                ProductoIncluidoIds = Distintos(datos.ProductoIncluidoIds),
                ProductoExcluidoIds = Distintos(datos.ProductoExcluidoIds),
                CategoriaIncluidaIds = Distintos(datos.CategoriaIncluidaIds),
                CategoriaExcluidaIds = Distintos(datos.CategoriaExcluidaIds)
            };

            // Un mismo producto o categoria no puede estar incluido y excluido a la vez.
            if (promocion.ProductoIncluidoIds.Intersect(promocion.ProductoExcluidoIds).Any())
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.ProductoIncludedAndExcluded");
            }

            if (promocion.CategoriaIncluidaIds.Intersect(promocion.CategoriaExcluidaIds).Any())
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.CategoriaIncludedAndExcluded");
            }

            return promocion;
        }

        private static void ValidarReferencias(IUnidadDeTrabajo unidad, Promocion promocion)
        {
            if (promocion.ComercioIds.Any(id => unidad.Comercios.ObtenerPorId(id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.ComercioNotFound");
            }

            if (promocion.RubroComercioIds.Any(id => unidad.RubrosComercio.ObtenerPorId(id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.RubroNotFound");
            }

            if (promocion.EntidadFinancieraIds.Any(id => unidad.EntidadesFinancieras.ObtenerPorId(id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.EntidadNotFound");
            }

            if (promocion.MedioPagoIds.Any(id => unidad.MediosPago.ObtenerPorId(id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.MedioPagoNotFound");
            }

            if (promocion.ProductoIncluidoIds.Concat(promocion.ProductoExcluidoIds).Any(id => unidad.Productos.ObtenerPorId(id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.ProductoNotFound");
            }

            if (promocion.CategoriaIncluidaIds.Concat(promocion.CategoriaExcluidaIds).Any(id => unidad.CategoriasProducto.ObtenerPorId(id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.CategoriaProductoNotFound");
            }
        }

        // Reemplaza los ocho filtros de la promocion (una lista vacia deja la dimension sin
        // restriccion); si alguno falla, la unidad de trabajo no se guarda.
        private static void ReemplazarFiltros(IUnidadDeTrabajo unidad, Promocion promocion, string claveFalla)
        {
            bool todoOk =
                unidad.Promociones.ReemplazarDiasDePromocion(promocion.Id, promocion.DiasSemana)
                && unidad.PromocionComercio.ReemplazarComerciosDePromocion(promocion.Id, promocion.ComercioIds)
                && unidad.PromocionRubrosComercio.ReemplazarRubrosComercioDePromocion(promocion.Id, promocion.RubroComercioIds)
                && unidad.PromocionEntidadesFinancieras.ReemplazarEntidadesFinancierasDePromocion(promocion.Id, promocion.EntidadFinancieraIds)
                && unidad.PromocionMediosPago.ReemplazarMediosPagoDePromocion(promocion.Id, promocion.MedioPagoIds)
                && unidad.PromocionSegmentos.ReemplazarSegmentosDePromocion(promocion.Id, promocion.Segmentos)
                && unidad.PromocionProductos.ReemplazarProductosDePromocion(promocion.Id, promocion.ProductoIncluidoIds, promocion.ProductoExcluidoIds)
                && unidad.PromocionCategoriasProducto.ReemplazarCategoriasProductoDePromocion(promocion.Id, promocion.CategoriaIncluidaIds, promocion.CategoriaExcluidaIds);

            if (!todoOk)
            {
                throw new ExcepcionTecnica(claveFalla);
            }
        }

        private static List<Promocion> Ordenar(IEnumerable<Promocion> promociones)
        {
            return promociones
                .OrderByDescending(promocion => promocion.VigenciaDesde)
                .ThenBy(promocion => promocion.Id)
                .ToList();
        }

        private static List<int> Distintos(List<int> ids)
        {
            return (ids ?? new List<int>()).Distinct().ToList();
        }

        private static List<string> SegmentosDistintos(List<string> segmentos)
        {
            return (segmentos ?? new List<string>())
                .Where(segmento => !string.IsNullOrWhiteSpace(segmento))
                .Select(segmento => segmento.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Minusculas y sin acentos para comparar: "Promoción" y "PROMOCION" coinciden.
        private static string NormalizarParaBusqueda(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string descompuesto = texto.Trim().Normalize(NormalizationForm.FormD);
            StringBuilder resultado = new StringBuilder(descompuesto.Length);
            foreach (char caracter in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(caracter);
                }
            }

            return resultado.ToString().ToLowerInvariant();
        }
    }
}
