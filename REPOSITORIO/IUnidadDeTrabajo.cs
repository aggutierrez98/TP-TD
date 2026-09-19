using System;

namespace REPOSITORIO
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IBitacoraRepositorio Bitacora { get; }

        ICategoriaRepositorio Categorias { get; }

        ICategoriaProductoRepositorio CategoriasProducto { get; }

        IComercioRepositorio Comercios { get; }

        IIdiomaRepositorio Idiomas { get; }

        IIntegridadRepositorio Integridad { get; }

        IItemListaRepositorio ItemsLista { get; }

        IListaDeComprasRepositorio ListasDeCompras { get; }

        IMonedaRepositorio Monedas { get; }

        IMovimientoRepositorio Movimientos { get; }

        INotificacionRepositorio Notificaciones { get; }

        IPermisoRepositorio Permisos { get; }

        IProductoRepositorio Productos { get; }

        IPromocionRepositorio Promociones { get; }

        IPromocionComercioRepositorio PromocionComercio { get; }

        IRecordatorioCompraRepositorio RecordatoriosCompra { get; }

        IRecordatorioInversionRepositorio RecordatoriosInversion { get; }

        IRubroComercioRepositorio RubrosComercio { get; }

        ITasaConversionRepositorio TasasConversion { get; }

        ITraduccionRepositorio Traducciones { get; }

        IUsuarioHistorialRepositorio UsuarioHistorial { get; }

        IUsuarioPermisoRepositorio UsuarioPermisos { get; }

        IUsuarioPreferenciaRepositorio UsuarioPreferencias { get; }

        IUsuarioRepositorio Usuarios { get; }

        void Save();
    }
}
