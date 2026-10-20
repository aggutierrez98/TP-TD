using System;

namespace REPOSITORIO
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IBitacoraRepositorio Bitacora { get; }

        ICategoriaRepositorio Categorias { get; }

        ICategoriaProductoRepositorio CategoriasProducto { get; }

        IComercioRepositorio Comercios { get; }

        ICuentaFinancieraRepositorio CuentasFinancieras { get; }

        IDashboardRepositorio Dashboards { get; }

        IEntidadFinancieraRepositorio EntidadesFinancieras { get; }

        IIdiomaRepositorio Idiomas { get; }

        IIntegridadRepositorio Integridad { get; }

        IItemListaRepositorio ItemsLista { get; }

        IListaDeComprasRepositorio ListasDeCompras { get; }

        IMedioPagoRepositorio MediosPago { get; }

        IMedioPagoEntidadRepositorio MediosPagoEntidad { get; }

        IMonedaRepositorio Monedas { get; }

        IMovimientoRepositorio Movimientos { get; }

        INotificacionRepositorio Notificaciones { get; }

        IPermisoRepositorio Permisos { get; }

        IProductoRepositorio Productos { get; }

        IProductoEmbeddingRepositorio ProductoEmbeddings { get; }

        IComercioProductoRepositorio ComercioProductos { get; }

        IPromocionRepositorio Promociones { get; }

        IPromocionComercioRepositorio PromocionComercio { get; }

        IPromocionRubroComercioRepositorio PromocionRubrosComercio { get; }

        IPromocionEntidadFinancieraRepositorio PromocionEntidadesFinancieras { get; }

        IPromocionMedioPagoRepositorio PromocionMediosPago { get; }

        IPromocionSegmentoRepositorio PromocionSegmentos { get; }

        IPromocionProductoRepositorio PromocionProductos { get; }

        IPromocionCategoriaProductoRepositorio PromocionCategoriasProducto { get; }

        IRecordatorioCompraRepositorio RecordatoriosCompra { get; }

        IRecordatorioInversionRepositorio RecordatoriosInversion { get; }

        IRubroComercioRepositorio RubrosComercio { get; }

        IReglaCategorizacionRepositorio ReglasCategorizacion { get; }

        IComercioReglaCategorizacionRepositorio ComercioReglaCategorizacion { get; }

        IConsultaChatbotRepositorio ConsultasChatbot { get; }

        ITarjetaDashboardRepositorio TarjetasDashboard { get; }

        ITasaConversionRepositorio TasasConversion { get; }

        IEjecucionActualizacionTasasRepositorio EjecucionesActualizacionTasas { get; }

        ITraduccionRepositorio Traducciones { get; }

        IUsuarioHistorialRepositorio UsuarioHistorial { get; }

        IUsuarioPermisoRepositorio UsuarioPermisos { get; }

        IUsuarioPreferenciaRepositorio UsuarioPreferencias { get; }

        IUsuarioRepositorio Usuarios { get; }

        IZonaUsuarioRepositorio ZonasUsuario { get; }

        IPerfilInversorRepositorio PerfilesInversor { get; }

        IGeneracionNotificacionesRepositorio GeneracionNotificaciones { get; }

        void Save();
    }
}
