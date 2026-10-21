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
        public List<Promocion> Buscar(string texto, Comercio recibido)
        {
            Comercio comercio = null;
            if (recibido != null)
            {
                comercio = _comercioRepositorio.ObtenerPorId(recibido.Id);
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
        public List<Promocion> ObtenerPorComercio(Comercio recibido)
        {
            Comercio comercio = recibido == null ? null : _comercioRepositorio.ObtenerPorId(recibido.Id);
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
            Normalizar(datos);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Promocion promocion = unidad.Promociones.ObtenerPorId(datos.Id);
                    if (promocion == null)
                    {
                        throw new ExcepcionNegocio("Error.PromotionManagement.PromocionNotFound");
                    }

                    CopiarEditables(datos, promocion);
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

        public bool Eliminar(Promocion recibida)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Promocion promocion = recibida == null ? null : unidad.Promociones.ObtenerPorId(recibida.Id);
                    if (promocion == null)
                    {
                        throw new ExcepcionNegocio("Error.PromotionManagement.PromocionNotFound");
                    }

                    // Una promocion que una notificacion o recordatorio referencia no se da de baja.
                    if (unidad.Notificaciones.ContarPorPromocion(promocion) > 0)
                    {
                        throw new ExcepcionNegocio("Error.PromotionManagement.PromocionInUse");
                    }

                    // Los filtros se borran primero: sus FK no tienen ON DELETE.
                    VaciarFiltros(promocion);
                    ReemplazarFiltros(unidad, promocion, "Error.PromotionManagement.PromocionDeleteFailed");

                    if (!unidad.Promociones.Eliminar(promocion))
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
        // Normaliza la entidad recibida (texto recortado y referencias sin repetir).
        private static Promocion Normalizar(Promocion datos)
        {
            if (datos == null)
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.DescriptionRequired");
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

            Promocion promocion = datos;
            promocion.Descripcion = descripcion;
            promocion.VigenciaDesde = datos.VigenciaDesde.Date;
            promocion.VigenciaHasta = datos.VigenciaHasta.Date;
            promocion.DiasSemana = (datos.DiasSemana ?? new List<DiaSemana>()).Distinct().ToList();
            promocion.Comercios = Distintos(datos.Comercios, item => item.Id);
            promocion.RubrosComercio = Distintos(datos.RubrosComercio, item => item.Id);
            promocion.EntidadesFinancieras = Distintos(datos.EntidadesFinancieras, item => item.Id);
            promocion.MediosPago = Distintos(datos.MediosPago, item => item.Id);
            promocion.Segmentos = SegmentosDistintos(datos.Segmentos);
            promocion.ProductosIncluidos = Distintos(datos.ProductosIncluidos, item => item.Id);
            promocion.ProductosExcluidos = Distintos(datos.ProductosExcluidos, item => item.Id);
            promocion.CategoriasIncluidas = Distintos(datos.CategoriasIncluidas, item => item.Id);
            promocion.CategoriasExcluidas = Distintos(datos.CategoriasExcluidas, item => item.Id);

            // Un mismo producto o categoria no puede estar incluido y excluido a la vez.
            if (promocion.ProductosIncluidos.Any(incluido => promocion.ProductosExcluidos.Any(excluido => incluido.Id == excluido.Id)))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.ProductoIncludedAndExcluded");
            }

            if (promocion.CategoriasIncluidas.Any(incluida => promocion.CategoriasExcluidas.Any(excluida => incluida.Id == excluida.Id)))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.CategoriaIncludedAndExcluded");
            }

            return promocion;
        }

        private static void ValidarReferencias(IUnidadDeTrabajo unidad, Promocion promocion)
        {
            if (promocion.Comercios.Any(item => unidad.Comercios.ObtenerPorId(item.Id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.ComercioNotFound");
            }

            if (promocion.RubrosComercio.Any(item => unidad.RubrosComercio.ObtenerPorId(item.Id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.RubroNotFound");
            }

            if (promocion.EntidadesFinancieras.Any(item => unidad.EntidadesFinancieras.ObtenerPorId(item.Id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.EntidadNotFound");
            }

            if (promocion.MediosPago.Any(item => unidad.MediosPago.ObtenerPorId(item.Id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.MedioPagoNotFound");
            }

            if (promocion.ProductosIncluidos.Concat(promocion.ProductosExcluidos).Any(item => unidad.Productos.ObtenerPorId(item.Id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.ProductoNotFound");
            }

            if (promocion.CategoriasIncluidas.Concat(promocion.CategoriasExcluidas).Any(item => unidad.CategoriasProducto.ObtenerPorId(item.Id) == null))
            {
                throw new ExcepcionNegocio("Error.PromotionManagement.CategoriaProductoNotFound");
            }
        }

        // Reemplaza los ocho filtros de la promocion (una lista vacia deja la dimension sin
        // restriccion); si alguno falla, la unidad de trabajo no se guarda.
        private static void ReemplazarFiltros(IUnidadDeTrabajo unidad, Promocion promocion, string claveFalla)
        {
            bool todoOk =
                unidad.Promociones.ReemplazarDiasDePromocion(promocion, promocion.DiasSemana)
                && unidad.PromocionComercio.ReemplazarComerciosDePromocion(promocion, promocion.Comercios)
                && unidad.PromocionRubrosComercio.ReemplazarRubrosComercioDePromocion(promocion, promocion.RubrosComercio)
                && unidad.PromocionEntidadesFinancieras.ReemplazarEntidadesFinancierasDePromocion(promocion, promocion.EntidadesFinancieras)
                && unidad.PromocionMediosPago.ReemplazarMediosPagoDePromocion(promocion, promocion.MediosPago)
                && unidad.PromocionSegmentos.ReemplazarSegmentosDePromocion(promocion, promocion.Segmentos)
                && unidad.PromocionProductos.ReemplazarProductosDePromocion(promocion, promocion.ProductosIncluidos, promocion.ProductosExcluidos)
                && unidad.PromocionCategoriasProducto.ReemplazarCategoriasProductoDePromocion(promocion, promocion.CategoriasIncluidas, promocion.CategoriasExcluidas);

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

        private static List<T> Distintos<T>(List<T> entidades, Func<T, int> clave)
        {
            return (entidades ?? new List<T>()).GroupBy(clave).Select(grupo => grupo.First()).ToList();
        }

        private static void CopiarEditables(Promocion origen, Promocion destino)
        {
            destino.Descripcion = origen.Descripcion;
            destino.Descuento = origen.Descuento;
            destino.TopeDescuento = origen.TopeDescuento;
            destino.VigenciaDesde = origen.VigenciaDesde;
            destino.VigenciaHasta = origen.VigenciaHasta;
            destino.DiasSemana = origen.DiasSemana;
            destino.Comercios = origen.Comercios;
            destino.RubrosComercio = origen.RubrosComercio;
            destino.EntidadesFinancieras = origen.EntidadesFinancieras;
            destino.MediosPago = origen.MediosPago;
            destino.Segmentos = origen.Segmentos;
            destino.ProductosIncluidos = origen.ProductosIncluidos;
            destino.ProductosExcluidos = origen.ProductosExcluidos;
            destino.CategoriasIncluidas = origen.CategoriasIncluidas;
            destino.CategoriasExcluidas = origen.CategoriasExcluidas;
        }

        private static void VaciarFiltros(Promocion promocion)
        {
            promocion.DiasSemana = new List<DiaSemana>();
            promocion.Comercios = new List<Comercio>();
            promocion.RubrosComercio = new List<RubroComercio>();
            promocion.EntidadesFinancieras = new List<EntidadFinanciera>();
            promocion.MediosPago = new List<MedioPago>();
            promocion.Segmentos = new List<string>();
            promocion.ProductosIncluidos = new List<Producto>();
            promocion.ProductosExcluidos = new List<Producto>();
            promocion.CategoriasIncluidas = new List<CategoriaProducto>();
            promocion.CategoriasExcluidas = new List<CategoriaProducto>();
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
