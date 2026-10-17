using System;
using DAL;

namespace REPOSITORIO
{
    internal sealed class UnidadDeTrabajoSqlServer : IUnidadDeTrabajo
    {
        private AccesoDatosSqlServer _accesoDatos;
        private bool _guardado;
        private bool _disposeInvocado;

        private IBitacoraRepositorio _bitacora;
        private ICategoriaRepositorio _categorias;
        private ICategoriaProductoRepositorio _categoriasProducto;
        private IComercioRepositorio _comercios;
        private ICuentaFinancieraRepositorio _cuentasFinancieras;
        private IDashboardRepositorio _dashboards;
        private IEntidadFinancieraRepositorio _entidadesFinancieras;
        private IIdiomaRepositorio _idiomas;
        private IIntegridadRepositorio _integridad;
        private IItemListaRepositorio _itemsLista;
        private IListaDeComprasRepositorio _listasDeCompras;
        private IMedioPagoRepositorio _mediosPago;
        private IMedioPagoEntidadRepositorio _mediosPagoEntidad;
        private IMonedaRepositorio _monedas;
        private IMovimientoRepositorio _movimientos;
        private INotificacionRepositorio _notificaciones;
        private IPermisoRepositorio _permisos;
        private IProductoRepositorio _productos;
        private IPromocionRepositorio _promociones;
        private IPromocionComercioRepositorio _promocionComercio;
        private IRecordatorioCompraRepositorio _recordatoriosCompra;
        private IRecordatorioInversionRepositorio _recordatoriosInversion;
        private IRubroComercioRepositorio _rubrosComercio;
        private IReglaCategorizacionRepositorio _reglasCategorizacion;
        private IComercioReglaCategorizacionRepositorio _comercioReglaCategorizacion;
        private ITarjetaDashboardRepositorio _tarjetasDashboard;
        private IConsultaChatbotRepositorio _consultasChatbot;
        private ITasaConversionRepositorio _tasasConversion;
        private ITraduccionRepositorio _traducciones;
        private IUsuarioHistorialRepositorio _usuarioHistorial;
        private IUsuarioPermisoRepositorio _usuarioPermisos;
        private IUsuarioPreferenciaRepositorio _usuarioPreferencias;
        private IUsuarioRepositorio _usuarios;

        public UnidadDeTrabajoSqlServer()
        {
        }

        public IBitacoraRepositorio Bitacora
        {
            get { return _bitacora ?? (_bitacora = new BitacoraRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ICategoriaRepositorio Categorias
        {
            get { return _categorias ?? (_categorias = new CategoriaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ICategoriaProductoRepositorio CategoriasProducto
        {
            get { return _categoriasProducto ?? (_categoriasProducto = new CategoriaProductoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IComercioRepositorio Comercios
        {
            get { return _comercios ?? (_comercios = new ComercioRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ICuentaFinancieraRepositorio CuentasFinancieras
        {
            get { return _cuentasFinancieras ?? (_cuentasFinancieras = new CuentaFinancieraRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IDashboardRepositorio Dashboards
        {
            get { return _dashboards ?? (_dashboards = new DashboardRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IEntidadFinancieraRepositorio EntidadesFinancieras
        {
            get { return _entidadesFinancieras ?? (_entidadesFinancieras = new EntidadFinancieraRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IIdiomaRepositorio Idiomas
        {
            get { return _idiomas ?? (_idiomas = new IdiomaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IIntegridadRepositorio Integridad
        {
            get { return _integridad ?? (_integridad = new IntegridadRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IItemListaRepositorio ItemsLista
        {
            get { return _itemsLista ?? (_itemsLista = new ItemListaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IListaDeComprasRepositorio ListasDeCompras
        {
            get { return _listasDeCompras ?? (_listasDeCompras = new ListaDeComprasRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IMedioPagoRepositorio MediosPago
        {
            get { return _mediosPago ?? (_mediosPago = new MedioPagoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IMedioPagoEntidadRepositorio MediosPagoEntidad
        {
            get { return _mediosPagoEntidad ?? (_mediosPagoEntidad = new MedioPagoEntidadRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IMonedaRepositorio Monedas
        {
            get { return _monedas ?? (_monedas = new MonedaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IMovimientoRepositorio Movimientos
        {
            get { return _movimientos ?? (_movimientos = new MovimientoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public INotificacionRepositorio Notificaciones
        {
            get { return _notificaciones ?? (_notificaciones = new NotificacionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IPermisoRepositorio Permisos
        {
            get { return _permisos ?? (_permisos = new PermisoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IProductoRepositorio Productos
        {
            get { return _productos ?? (_productos = new ProductoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IPromocionRepositorio Promociones
        {
            get { return _promociones ?? (_promociones = new PromocionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IPromocionComercioRepositorio PromocionComercio
        {
            get { return _promocionComercio ?? (_promocionComercio = new PromocionComercioRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IRecordatorioCompraRepositorio RecordatoriosCompra
        {
            get { return _recordatoriosCompra ?? (_recordatoriosCompra = new RecordatorioCompraRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IRecordatorioInversionRepositorio RecordatoriosInversion
        {
            get { return _recordatoriosInversion ?? (_recordatoriosInversion = new RecordatorioInversionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IRubroComercioRepositorio RubrosComercio
        {
            get { return _rubrosComercio ?? (_rubrosComercio = new RubroComercioRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IReglaCategorizacionRepositorio ReglasCategorizacion
        {
            get { return _reglasCategorizacion ?? (_reglasCategorizacion = new ReglaCategorizacionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IComercioReglaCategorizacionRepositorio ComercioReglaCategorizacion
        {
            get { return _comercioReglaCategorizacion ?? (_comercioReglaCategorizacion = new ComercioReglaCategorizacionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ITarjetaDashboardRepositorio TarjetasDashboard
        {
            get { return _tarjetasDashboard ?? (_tarjetasDashboard = new TarjetaDashboardRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IConsultaChatbotRepositorio ConsultasChatbot
        {
            get { return _consultasChatbot ?? (_consultasChatbot = new ConsultaChatbotRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ITasaConversionRepositorio TasasConversion
        {
            get { return _tasasConversion ?? (_tasasConversion = new TasaConversionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ITraduccionRepositorio Traducciones
        {
            get { return _traducciones ?? (_traducciones = new TraduccionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioHistorialRepositorio UsuarioHistorial
        {
            get { return _usuarioHistorial ?? (_usuarioHistorial = new UsuarioHistorialRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioPermisoRepositorio UsuarioPermisos
        {
            get { return _usuarioPermisos ?? (_usuarioPermisos = new UsuarioPermisoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioPreferenciaRepositorio UsuarioPreferencias
        {
            get { return _usuarioPreferencias ?? (_usuarioPreferencias = new UsuarioPreferenciaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioRepositorio Usuarios
        {
            get { return _usuarios ?? (_usuarios = new UsuarioRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public void Save()
        {
            if (_guardado)
            {
                return;
            }

            if (_accesoDatos != null)
            {
                _accesoDatos.CommitTransactionScope();
            }

            _guardado = true;
        }

        private AccesoDatosSqlServer ObtenerAccesoDatosTransaccional()
        {
            if (_disposeInvocado)
            {
                throw new ObjectDisposedException("UnidadDeTrabajoSqlServer");
            }

            if (_guardado)
            {
                throw new InvalidOperationException("La unidad de trabajo ya fue confirmada.");
            }

            if (_accesoDatos == null)
            {
                _accesoDatos = new AccesoDatosSqlServer();
                _accesoDatos.BeginTransactionScope();
            }

            return _accesoDatos;
        }

        public void Dispose()
        {
            if (_disposeInvocado)
            {
                return;
            }

            try
            {
                if (!_guardado && _accesoDatos != null)
                {
                    _accesoDatos.RollbackTransactionScope();
                }
            }
            catch
            {
            }
            finally
            {
                if (_accesoDatos != null)
                {
                    _accesoDatos.Dispose();
                    _accesoDatos = null;
                }

                _disposeInvocado = true;
            }
        }
    }
}
