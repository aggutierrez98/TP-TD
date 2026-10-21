using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL;
using REPOSITORIO;
using SERVICIOS;

namespace BLL.Tests.TestDoubles
{
    internal sealed class FakeUnidadDeTrabajoFactory : IUnidadDeTrabajoFactory
    {
        public FakeUnidadDeTrabajoFactory()
            : this(new FakeUnidadDeTrabajo())
        {
        }

        public FakeUnidadDeTrabajoFactory(FakeUnidadDeTrabajo unidad)
        {
            NextUnidad = unidad;
        }

        public FakeUnidadDeTrabajo NextUnidad { get; set; }

        public FakeUnidadDeTrabajo UltimaUnidadCreada { get; private set; }

        public int CrearCallCount { get; private set; }

        public IUnidadDeTrabajo Crear()
        {
            CrearCallCount++;
            UltimaUnidadCreada = NextUnidad ?? new FakeUnidadDeTrabajo();
            return UltimaUnidadCreada;
        }
    }

    internal sealed class FakeUnidadDeTrabajo : IUnidadDeTrabajo
    {
        public IPerfilInversorRepositorio PerfilesInversor { get; set; }

        public FakeGeneracionNotificacionesRepositorio GeneracionNotificacionesRepo { get; set; } = new FakeGeneracionNotificacionesRepositorio();

        public IGeneracionNotificacionesRepositorio GeneracionNotificaciones { get { return GeneracionNotificacionesRepo; } }
        public FakeUnidadDeTrabajo()
        {
            BitacoraRepo = new FakeBitacoraRepositorio();
            CategoriaRepo = new FakeCategoriaRepositorio();
            CategoriaProductoRepo = new FakeCategoriaProductoRepositorio();
            ComercioRepo = new FakeComercioRepositorio();
            ComercioReglaCategorizacionRepo = new FakeComercioReglaCategorizacionRepositorio();
            CuentaFinancieraRepo = new FakeCuentaFinancieraRepositorio();
            EntidadFinancieraRepo = new FakeEntidadFinancieraRepositorio();
            MedioPagoRepo = new FakeMedioPagoRepositorio();
            MedioPagoEntidadRepo = new FakeMedioPagoEntidadRepositorio();
            DashboardRepo = new FakeDashboardRepositorio();
            TarjetaDashboardRepo = new FakeTarjetaDashboardRepositorio();
            MonedaRepo = new FakeMonedaRepositorio();
            TasaConversionRepo = new FakeTasaConversionRepositorio();
            EjecucionActualizacionTasasRepo = new FakeEjecucionActualizacionTasasRepositorio();
            IdiomaRepo = new FakeIdiomaRepositorio();
            IntegridadRepo = new FakeIntegridadRepositorio();
            ItemListaRepo = new FakeItemListaRepositorio();
            ListaDeComprasRepo = new FakeListaDeComprasRepositorio();
            MovimientoRepo = new FakeMovimientoRepositorio();
            NotificacionRepo = new FakeNotificacionRepositorio();
            PermisoRepo = new FakePermisoRepositorio();
            ProductoRepo = new FakeProductoRepositorio();
            ComercioProductoRepo = new FakeComercioProductoRepositorio();
            PromocionRepo = new FakePromocionRepositorio();
            PromocionComercioRepo = new FakePromocionComercioRepositorio();
            PromocionRubroComercioRepo = new FakePromocionRubroComercioRepositorio();
            PromocionEntidadFinancieraRepo = new FakePromocionEntidadFinancieraRepositorio();
            PromocionMedioPagoRepo = new FakePromocionMedioPagoRepositorio();
            PromocionSegmentoRepo = new FakePromocionSegmentoRepositorio();
            PromocionProductoRepo = new FakePromocionProductoRepositorio();
            PromocionCategoriaProductoRepo = new FakePromocionCategoriaProductoRepositorio();
            RecordatorioCompraRepo = new FakeRecordatorioCompraRepositorio();
            RecordatorioInversionRepo = new FakeRecordatorioInversionRepositorio();
            RubroComercioRepo = new FakeRubroComercioRepositorio();
            ConsultaChatbotRepo = new FakeConsultaChatbotRepositorio();
            TraduccionRepo = new FakeTraduccionRepositorio();
            UsuarioHistorialRepo = new FakeUsuarioHistorialRepositorio();
            UsuarioPermisoRepo = new FakeUsuarioPermisoRepositorio();
            UsuarioPreferenciaRepo = new FakeUsuarioPreferenciaRepositorio();
            UsuarioRepo = new FakeUsuarioRepositorio();
            ZonaUsuarioRepo = new FakeZonaUsuarioRepositorio();
            ProductoEmbeddingRepo = new FakeProductoEmbeddingRepositorio();
            NotificacionRepo.Fuentes = () => NotificacionRepo.Notificaciones.Values
                .Concat(RecordatorioCompraRepo.Recordatorios.Values)
                .Concat(RecordatorioInversionRepo.Recordatorios.Values.Cast<Notificacion>());
        }

        public FakeBitacoraRepositorio BitacoraRepo { get; set; }

        public FakeCategoriaRepositorio CategoriaRepo { get; set; }

        public FakeCategoriaProductoRepositorio CategoriaProductoRepo { get; set; }

        public FakeComercioRepositorio ComercioRepo { get; set; }

        public FakeCuentaFinancieraRepositorio CuentaFinancieraRepo { get; set; }

        public FakeEntidadFinancieraRepositorio EntidadFinancieraRepo { get; set; }

        public FakeMedioPagoRepositorio MedioPagoRepo { get; set; }

        public FakeMedioPagoEntidadRepositorio MedioPagoEntidadRepo { get; set; }

        public FakeDashboardRepositorio DashboardRepo { get; set; }

        public FakeTarjetaDashboardRepositorio TarjetaDashboardRepo { get; set; }

        public FakeMonedaRepositorio MonedaRepo { get; set; }

        public FakeTasaConversionRepositorio TasaConversionRepo { get; set; }

        public FakeEjecucionActualizacionTasasRepositorio EjecucionActualizacionTasasRepo { get; set; }

        public FakeIdiomaRepositorio IdiomaRepo { get; set; }

        public FakeIntegridadRepositorio IntegridadRepo { get; set; }

        public FakeItemListaRepositorio ItemListaRepo { get; set; }

        public FakeListaDeComprasRepositorio ListaDeComprasRepo { get; set; }

        public FakeMovimientoRepositorio MovimientoRepo { get; set; }

        public FakeNotificacionRepositorio NotificacionRepo { get; set; }

        public FakePermisoRepositorio PermisoRepo { get; set; }

        public FakeProductoRepositorio ProductoRepo { get; set; }

        public FakeComercioProductoRepositorio ComercioProductoRepo { get; set; }

        public FakePromocionRepositorio PromocionRepo { get; set; }

        public FakePromocionComercioRepositorio PromocionComercioRepo { get; set; }

        public FakePromocionRubroComercioRepositorio PromocionRubroComercioRepo { get; set; }

        public FakePromocionEntidadFinancieraRepositorio PromocionEntidadFinancieraRepo { get; set; }

        public FakePromocionMedioPagoRepositorio PromocionMedioPagoRepo { get; set; }

        public FakePromocionSegmentoRepositorio PromocionSegmentoRepo { get; set; }

        public FakePromocionProductoRepositorio PromocionProductoRepo { get; set; }

        public FakePromocionCategoriaProductoRepositorio PromocionCategoriaProductoRepo { get; set; }

        public FakeRecordatorioCompraRepositorio RecordatorioCompraRepo { get; set; }

        public FakeRecordatorioInversionRepositorio RecordatorioInversionRepo { get; set; }

        public FakeRubroComercioRepositorio RubroComercioRepo { get; set; }

        public FakeReglaCategorizacionRepositorio ReglaCategorizacionRepo { get; set; }

        public FakeComercioReglaCategorizacionRepositorio ComercioReglaCategorizacionRepo { get; set; }

        public FakeConsultaChatbotRepositorio ConsultaChatbotRepo { get; set; }

        public FakeTraduccionRepositorio TraduccionRepo { get; set; }

        public FakeUsuarioHistorialRepositorio UsuarioHistorialRepo { get; set; }

        public FakeUsuarioPermisoRepositorio UsuarioPermisoRepo { get; set; }

        public FakeUsuarioPreferenciaRepositorio UsuarioPreferenciaRepo { get; set; }

        public FakeUsuarioRepositorio UsuarioRepo { get; set; }

        public FakeZonaUsuarioRepositorio ZonaUsuarioRepo { get; set; }

        public FakeProductoEmbeddingRepositorio ProductoEmbeddingRepo { get; set; }

        public bool SaveCalled { get; private set; }

        public int SaveCallCount { get; private set; }

        public bool DisposeCalled { get; private set; }

        public bool ThrowOnSave { get; set; }

        public IBitacoraRepositorio Bitacora { get { return BitacoraRepo; } }

        public ICategoriaRepositorio Categorias { get { return CategoriaRepo; } }

        public ICategoriaProductoRepositorio CategoriasProducto { get { return CategoriaProductoRepo; } }

        public IComercioRepositorio Comercios { get { return ComercioRepo; } }

        public ICuentaFinancieraRepositorio CuentasFinancieras { get { return CuentaFinancieraRepo; } }

        public IEntidadFinancieraRepositorio EntidadesFinancieras { get { return EntidadFinancieraRepo; } }

        public IMedioPagoRepositorio MediosPago { get { return MedioPagoRepo; } }

        public IMedioPagoEntidadRepositorio MediosPagoEntidad { get { return MedioPagoEntidadRepo; } }

        public IDashboardRepositorio Dashboards { get { return DashboardRepo; } }

        public ITarjetaDashboardRepositorio TarjetasDashboard { get { return TarjetaDashboardRepo; } }

        public IMonedaRepositorio Monedas { get { return MonedaRepo; } }

        public IIdiomaRepositorio Idiomas { get { return IdiomaRepo; } }

        public IIntegridadRepositorio Integridad { get { return IntegridadRepo; } }

        public IItemListaRepositorio ItemsLista { get { return ItemListaRepo; } }

        public IListaDeComprasRepositorio ListasDeCompras { get { return ListaDeComprasRepo; } }

        public IMovimientoRepositorio Movimientos { get { return MovimientoRepo; } }

        public INotificacionRepositorio Notificaciones { get { return NotificacionRepo; } }

        public IPermisoRepositorio Permisos { get { return PermisoRepo; } }

        public IProductoRepositorio Productos { get { return ProductoRepo; } }

        public IComercioProductoRepositorio ComercioProductos { get { return ComercioProductoRepo; } }

        public IPromocionRepositorio Promociones { get { return PromocionRepo; } }

        public IPromocionComercioRepositorio PromocionComercio { get { return PromocionComercioRepo; } }

        public IPromocionRubroComercioRepositorio PromocionRubrosComercio { get { return PromocionRubroComercioRepo; } }

        public IPromocionEntidadFinancieraRepositorio PromocionEntidadesFinancieras { get { return PromocionEntidadFinancieraRepo; } }

        public IPromocionMedioPagoRepositorio PromocionMediosPago { get { return PromocionMedioPagoRepo; } }

        public IPromocionSegmentoRepositorio PromocionSegmentos { get { return PromocionSegmentoRepo; } }

        public IPromocionProductoRepositorio PromocionProductos { get { return PromocionProductoRepo; } }

        public IPromocionCategoriaProductoRepositorio PromocionCategoriasProducto { get { return PromocionCategoriaProductoRepo; } }

        public IRecordatorioCompraRepositorio RecordatoriosCompra { get { return RecordatorioCompraRepo; } }

        public IRecordatorioInversionRepositorio RecordatoriosInversion { get { return RecordatorioInversionRepo; } }

        public IRubroComercioRepositorio RubrosComercio { get { return RubroComercioRepo; } }

        public IReglaCategorizacionRepositorio ReglasCategorizacion { get { return ReglaCategorizacionRepo; } }

        public IComercioReglaCategorizacionRepositorio ComercioReglaCategorizacion { get { return ComercioReglaCategorizacionRepo; } }

        public IConsultaChatbotRepositorio ConsultasChatbot { get { return ConsultaChatbotRepo; } }

        public ITasaConversionRepositorio TasasConversion { get { return TasaConversionRepo; } }

        public IEjecucionActualizacionTasasRepositorio EjecucionesActualizacionTasas { get { return EjecucionActualizacionTasasRepo; } }

        public ITraduccionRepositorio Traducciones { get { return TraduccionRepo; } }

        public IUsuarioHistorialRepositorio UsuarioHistorial { get { return UsuarioHistorialRepo; } }

        public IUsuarioPermisoRepositorio UsuarioPermisos { get { return UsuarioPermisoRepo; } }

        public IUsuarioPreferenciaRepositorio UsuarioPreferencias { get { return UsuarioPreferenciaRepo; } }

        public IUsuarioRepositorio Usuarios { get { return UsuarioRepo; } }

        public IZonaUsuarioRepositorio ZonasUsuario { get { return ZonaUsuarioRepo; } }

        public IProductoEmbeddingRepositorio ProductoEmbeddings { get { return ProductoEmbeddingRepo; } }

        // Numeros de intento de Save (desde 1) que fallan; permite fallas transitorias o sostenidas.
        public HashSet<int> FailSaveCalls { get; } = new HashSet<int>();

        private int _saveAttempts;

        public void Save()
        {
            _saveAttempts++;
            if (ThrowOnSave || FailSaveCalls.Contains(_saveAttempts))
            {
                throw new InvalidOperationException("Save failed.");
            }

            SaveCalled = true;
            SaveCallCount++;
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }

    internal sealed class FakeCategoriaRepositorio : ICategoriaRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Categoria> Categorias { get; } = new Dictionary<int, Categoria>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public bool ExisteNombreEnScopeResult { get; set; }

        public Categoria LastInserted { get; private set; }

        public Categoria LastUpdated { get; private set; }

        public Categoria LastDeleted { get; private set; }

        public Categoria ObtenerPorId(int id)
        {
            Categoria categoria;
            return Categorias.TryGetValue(id, out categoria) ? categoria : null;
        }

        public List<Categoria> ObtenerVisiblesParaUsuario(Usuario usuario)
        {
            return Categorias.Values
                .Where(item => item.Usuario == null || item.Usuario.Id == usuario.Id)
                .ToList();
        }

        public int InsertarYObtenerId(Categoria categoria)
        {
            if (categoria.Id <= 0)
            {
                categoria.Id = _nextId++;
            }

            Categorias[categoria.Id] = categoria;
            LastInserted = categoria;
            return categoria.Id;
        }

        public bool Actualizar(Categoria categoria)
        {
            LastUpdated = categoria;
            if (!ActualizarResult)
            {
                return false;
            }

            Categorias[categoria.Id] = categoria;
            return true;
        }

        public bool Eliminar(Categoria categoria)
        {
            LastDeleted = categoria;
            if (!EliminarResult)
            {
                return false;
            }

            return Categorias.Remove(categoria.Id);
        }

        public bool ExisteNombreEnScope(string nombre, Usuario usuario, Categoria excluida)
        {
            return Categorias.Values.Any(item =>
                item.Usuario?.Id == usuario?.Id &&
                string.Equals(item.Nombre, nombre, StringComparison.OrdinalIgnoreCase) &&
                (excluida == null || item.Id != excluida.Id));
        }
    }

    internal sealed class FakeRubroComercioRepositorio : IRubroComercioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, RubroComercio> Rubros { get; } = new Dictionary<int, RubroComercio>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnEliminar { get; set; }

        public RubroComercio LastInserted { get; private set; }

        public RubroComercio LastUpdated { get; private set; }

        public RubroComercio LastDeleted { get; private set; }

        public RubroComercio ObtenerPorId(int id)
        {
            RubroComercio rubro;
            return Rubros.TryGetValue(id, out rubro) ? rubro : null;
        }

        public List<RubroComercio> ObtenerTodas()
        {
            return Rubros.Values.ToList();
        }

        public int InsertarYObtenerId(RubroComercio rubro)
        {
            if (rubro.Id <= 0)
            {
                rubro.Id = _nextId++;
            }

            Rubros[rubro.Id] = rubro;
            LastInserted = rubro;
            return rubro.Id;
        }

        public bool Actualizar(RubroComercio rubro)
        {
            LastUpdated = rubro;
            if (!ActualizarResult)
            {
                return false;
            }

            Rubros[rubro.Id] = rubro;
            return true;
        }

        public bool Eliminar(RubroComercio rubro)
        {
            LastDeleted = rubro;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Rubros.Remove(rubro.Id);
        }
    }

    internal sealed class FakeComercioRepositorio : IComercioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Comercio> Comercios { get; } = new Dictionary<int, Comercio>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnEliminar { get; set; }

        public Comercio LastInserted { get; private set; }

        public Comercio LastUpdated { get; private set; }

        public Comercio LastDeleted { get; private set; }

        public Comercio ObtenerPorId(int id)
        {
            Comercio comercio;
            return Comercios.TryGetValue(id, out comercio) ? comercio : null;
        }

        public List<Comercio> ObtenerPorRubro(RubroComercio rubro)
        {
            return Comercios.Values
                .Where(item => item.RubroComercio != null && item.RubroComercio.Id == rubro.Id)
                .ToList();
        }

        public List<Comercio> ObtenerTodos()
        {
            return Comercios.Values.ToList();
        }

        public int InsertarYObtenerId(Comercio comercio)
        {
            if (comercio.Id <= 0)
            {
                comercio.Id = _nextId++;
            }

            Comercios[comercio.Id] = comercio;
            LastInserted = comercio;
            return comercio.Id;
        }

        public bool Actualizar(Comercio comercio)
        {
            LastUpdated = comercio;
            if (!ActualizarResult)
            {
                return false;
            }

            Comercios[comercio.Id] = comercio;
            return true;
        }

        public bool Eliminar(Comercio comercio)
        {
            LastDeleted = comercio;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Comercios.Remove(comercio.Id);
        }
    }

    // Construye, via reflexion, un SqlException real con Number = 547 (violacion de FK) para usarlo
    // como falla tecnica de entrada en los tests de baja: los BLL ya no lo interpretan, asi que
    // nunca debe terminar en un mensaje "en uso". SqlException no tiene constructor publico;
    // esta es la tecnica estandar para instanciarlo en tests sin una base de datos real.
    internal static class FakeSqlExceptionFactory
    {
        public static SqlException CrearViolacionForanea()
        {
            return CrearSqlException(547);
        }

        // Igual que lo envuelve AccesoDatosSqlServer.ExecuteInTransaction en produccion.
        public static ExcepcionTecnica CrearViolacionForaneaEnvuelta()
        {
            return new ExcepcionTecnica("Error.Technical.SqlFailure", CrearSqlException(547));
        }

        private static SqlException CrearSqlException(int number)
        {
            SqlErrorCollection collection = (SqlErrorCollection)Activator.CreateInstance(
                typeof(SqlErrorCollection), true);

            object error = CrearSqlError(number);

            typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(collection, new object[] { error });

            SqlException exception = (SqlException)typeof(SqlException)
                .GetMethod("CreateException", BindingFlags.NonPublic | BindingFlags.Static, null,
                    new[] { typeof(SqlErrorCollection), typeof(string) }, null)
                .Invoke(null, new object[] { collection, "11.0.0" });

            return exception;
        }

        private static object CrearSqlError(int number)
        {
            ConstructorInfo[] constructores = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (ConstructorInfo constructor in constructores)
            {
                ParameterInfo[] parametros = constructor.GetParameters();
                object[] args = new object[parametros.Length];
                bool ok = true;
                for (int i = 0; i < parametros.Length; i++)
                {
                    Type t = parametros[i].ParameterType;
                    if (i == 0 && t == typeof(int))
                    {
                        args[i] = number;
                    }
                    else if (t == typeof(byte))
                    {
                        args[i] = (byte)0;
                    }
                    else if (t == typeof(string))
                    {
                        args[i] = "x";
                    }
                    else if (t == typeof(int))
                    {
                        args[i] = 0;
                    }
                    else if (t == typeof(uint))
                    {
                        args[i] = (uint)0;
                    }
                    else if (t == typeof(Exception))
                    {
                        args[i] = null;
                    }
                    else if (t == typeof(Guid))
                    {
                        args[i] = Guid.Empty;
                    }
                    else
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                {
                    try
                    {
                        return constructor.Invoke(args);
                    }
                    catch
                    {
                    }
                }
            }

            throw new InvalidOperationException("No se encontro un constructor de SqlError compatible.");
        }
    }

    internal sealed class FakeCategoriaProductoRepositorio : ICategoriaProductoRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, CategoriaProducto> Categorias { get; } = new Dictionary<int, CategoriaProducto>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnEliminar { get; set; }

        public CategoriaProducto LastInserted { get; private set; }

        public CategoriaProducto LastUpdated { get; private set; }

        public CategoriaProducto LastDeleted { get; private set; }

        public CategoriaProducto ObtenerPorId(int id)
        {
            CategoriaProducto categoria;
            return Categorias.TryGetValue(id, out categoria) ? categoria : null;
        }

        public List<CategoriaProducto> ObtenerTodas()
        {
            return Categorias.Values.ToList();
        }

        public int InsertarYObtenerId(CategoriaProducto categoria)
        {
            if (categoria.Id <= 0)
            {
                categoria.Id = _nextId++;
            }

            Categorias[categoria.Id] = categoria;
            LastInserted = categoria;
            return categoria.Id;
        }

        public bool Actualizar(CategoriaProducto categoria)
        {
            LastUpdated = categoria;
            if (!ActualizarResult)
            {
                return false;
            }

            Categorias[categoria.Id] = categoria;
            return true;
        }

        public bool Eliminar(CategoriaProducto categoria)
        {
            LastDeleted = categoria;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Categorias.Remove(categoria.Id);
        }
    }

    internal sealed class FakeProductoRepositorio : IProductoRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Producto> Productos { get; } = new Dictionary<int, Producto>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Producto LastInserted { get; private set; }

        public Producto LastUpdated { get; private set; }

        public Producto LastDeleted { get; private set; }

        public Producto ObtenerPorId(int id)
        {
            Producto producto;
            return Productos.TryGetValue(id, out producto) ? producto : null;
        }

        public List<Producto> ObtenerTodos()
        {
            return Productos.Values.OrderBy(item => item.Nombre).ToList();
        }

        public int InsertarYObtenerId(Producto producto)
        {
            if (producto.Id <= 0)
            {
                producto.Id = _nextId++;
            }

            Productos[producto.Id] = producto;
            LastInserted = producto;
            return producto.Id;
        }

        public bool Actualizar(Producto producto)
        {
            LastUpdated = producto;
            if (!ActualizarResult)
            {
                return false;
            }

            Productos[producto.Id] = producto;
            return true;
        }

        public bool Eliminar(Producto producto)
        {
            LastDeleted = producto;
            if (!EliminarResult)
            {
                return false;
            }

            return Productos.Remove(producto.Id);
        }

        public int ContarPorCategoria(CategoriaProducto categoria)
        {
            return Productos.Values.Count(item => item.CategoriaProducto != null && item.CategoriaProducto.Id == categoria.Id);
        }
    }

    internal sealed class FakeComercioProductoRepositorio : IComercioProductoRepositorio
    {
        public List<ComercioProducto> Asignaciones { get; } = new List<ComercioProducto>();

        public bool InsertarResult { get; set; } = true;

        public bool ActualizarPrecioResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public ComercioProducto LastInserted { get; private set; }

        public ComercioProducto LastPrecioActualizado { get; private set; }

        public ComercioProducto LastDeleted { get; private set; }

        // Ayuda de los tests: busca por ids sin armar entidades.
        public ComercioProducto Buscar(int comercioId, int productoId)
        {
            return Asignaciones.FirstOrDefault(item => item.Comercio.Id == comercioId && item.Producto.Id == productoId);
        }

        public List<ComercioProducto> ObtenerPorComercio(Comercio comercio)
        {
            return Asignaciones.Where(item => item.Comercio.Id == comercio.Id).ToList();
        }

        public ComercioProducto ObtenerPorComercioYProducto(Comercio comercio, Producto producto)
        {
            return Buscar(comercio.Id, producto.Id);
        }

        public bool Insertar(ComercioProducto comercioProducto)
        {
            LastInserted = comercioProducto;
            if (!InsertarResult)
            {
                return false;
            }

            Asignaciones.Add(comercioProducto);
            return true;
        }

        public bool ActualizarPrecio(ComercioProducto comercioProducto)
        {
            LastPrecioActualizado = comercioProducto;
            ComercioProducto asignacion = Buscar(comercioProducto.Comercio.Id, comercioProducto.Producto.Id);
            if (!ActualizarPrecioResult || asignacion == null)
            {
                return false;
            }

            asignacion.Precio = comercioProducto.Precio;
            return true;
        }

        public bool Eliminar(ComercioProducto comercioProducto)
        {
            LastDeleted = comercioProducto;
            if (!EliminarResult)
            {
                return false;
            }

            ComercioProducto asignacion = Buscar(comercioProducto.Comercio.Id, comercioProducto.Producto.Id);
            return asignacion != null && Asignaciones.Remove(asignacion);
        }

        public int ContarPorProducto(Producto producto)
        {
            return Asignaciones.Count(item => item.Producto.Id == producto.Id);
        }
    }

    internal sealed class FakePromocionRepositorio : IPromocionRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Promocion> Promociones { get; } = new Dictionary<int, Promocion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnInsertar { get; set; }

        public Exception ExcepcionEnEliminar { get; set; }

        public Promocion LastInserted { get; private set; }

        public Promocion LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public Promocion ObtenerPorId(int id)
        {
            Promocion promocion;
            return Promociones.TryGetValue(id, out promocion) ? promocion : null;
        }

        public List<Promocion> ObtenerTodas()
        {
            return Promociones.Values.ToList();
        }

        public int InsertarYObtenerId(Promocion promocion)
        {
            if (ExcepcionEnInsertar != null)
            {
                throw ExcepcionEnInsertar;
            }

            if (promocion.Id <= 0)
            {
                promocion.Id = _nextId++;
            }

            Promociones[promocion.Id] = promocion;
            LastInserted = promocion;
            return promocion.Id;
        }

        public bool Actualizar(Promocion promocion)
        {
            LastUpdated = promocion;
            if (!ActualizarResult)
            {
                return false;
            }

            Promociones[promocion.Id] = promocion;
            return true;
        }

        public bool Eliminar(Promocion promocion)
        {
            LastDeletedId = promocion.Id;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Promociones.Remove(promocion.Id);
        }

        public bool ReemplazarDiasDePromocion(Promocion recibida, List<DiaSemana> dias)
        {
            Promocion promocion;
            if (!Promociones.TryGetValue(recibida.Id, out promocion))
            {
                return false;
            }

            promocion.DiasSemana = new List<DiaSemana>(dias ?? new List<DiaSemana>());
            return true;
        }
    }

    // Filtros de promocion: guardan lo que el BLL reemplaza para poder verificar la persistencia.
    internal sealed class FakePromocionRubroComercioRepositorio : IPromocionRubroComercioRepositorio
    {
        public int ContarPorRubroComercio(RubroComercio rubroComercio)
        {
            return RubroComercioIdsPorPromocion.Values.Count(ids => ids.Contains(rubroComercio.Id));
        }

        public Dictionary<int, List<int>> RubroComercioIdsPorPromocion { get; } = new Dictionary<int, List<int>>();

        public List<int> PromocionIdsReemplazados { get; } = new List<int>();

        public List<RubroComercio> ObtenerRubrosComercioPorPromocion(Promocion promocion)
        {
            List<int> ids;
            return RubroComercioIdsPorPromocion.TryGetValue(promocion.Id, out ids) ? ids.Select(id => new RubroComercio { Id = id }).ToList() : new List<RubroComercio>();
        }

        public bool ReemplazarRubrosComercioDePromocion(Promocion promocion, List<RubroComercio> rubrosComercio)
        {
            PromocionIdsReemplazados.Add(promocion.Id);
            RubroComercioIdsPorPromocion[promocion.Id] = (rubrosComercio ?? new List<RubroComercio>()).Select(item => item.Id).ToList();
            return true;
        }
    }

    internal sealed class FakePromocionEntidadFinancieraRepositorio : IPromocionEntidadFinancieraRepositorio
    {
        public int ContarPorEntidadFinanciera(EntidadFinanciera entidadFinanciera)
        {
            return EntidadFinancieraIdsPorPromocion.Values.Count(ids => ids.Contains(entidadFinanciera.Id));
        }

        public Dictionary<int, List<int>> EntidadFinancieraIdsPorPromocion { get; } = new Dictionary<int, List<int>>();

        public List<int> PromocionIdsReemplazados { get; } = new List<int>();

        public List<EntidadFinanciera> ObtenerEntidadesFinancierasPorPromocion(Promocion promocion)
        {
            List<int> ids;
            return EntidadFinancieraIdsPorPromocion.TryGetValue(promocion.Id, out ids) ? ids.Select(id => new EntidadFinanciera { Id = id }).ToList() : new List<EntidadFinanciera>();
        }

        public bool ReemplazarEntidadesFinancierasDePromocion(Promocion promocion, List<EntidadFinanciera> entidadesFinancieras)
        {
            PromocionIdsReemplazados.Add(promocion.Id);
            EntidadFinancieraIdsPorPromocion[promocion.Id] = (entidadesFinancieras ?? new List<EntidadFinanciera>()).Select(item => item.Id).ToList();
            return true;
        }
    }

    internal sealed class FakePromocionMedioPagoRepositorio : IPromocionMedioPagoRepositorio
    {
        public int ContarPorMedioPago(MedioPago medioPago)
        {
            return MedioPagoIdsPorPromocion.Values.Count(ids => ids.Contains(medioPago.Id));
        }

        public Dictionary<int, List<int>> MedioPagoIdsPorPromocion { get; } = new Dictionary<int, List<int>>();

        public List<int> PromocionIdsReemplazados { get; } = new List<int>();

        public List<MedioPago> ObtenerMediosPagoPorPromocion(Promocion promocion)
        {
            List<int> ids;
            return MedioPagoIdsPorPromocion.TryGetValue(promocion.Id, out ids) ? ids.Select(id => new MedioPago { Id = id }).ToList() : new List<MedioPago>();
        }

        public bool ReemplazarMediosPagoDePromocion(Promocion promocion, List<MedioPago> mediosPago)
        {
            PromocionIdsReemplazados.Add(promocion.Id);
            MedioPagoIdsPorPromocion[promocion.Id] = (mediosPago ?? new List<MedioPago>()).Select(item => item.Id).ToList();
            return true;
        }
    }

    internal sealed class FakePromocionSegmentoRepositorio : IPromocionSegmentoRepositorio
    {
        public Dictionary<int, List<string>> SegmentosPorPromocion { get; } = new Dictionary<int, List<string>>();

        public List<int> PromocionIdsReemplazados { get; } = new List<int>();

        public List<string> ObtenerSegmentosPorPromocion(Promocion promocion)
        {
            List<string> segmentos;
            return SegmentosPorPromocion.TryGetValue(promocion.Id, out segmentos) ? segmentos.ToList() : new List<string>();
        }

        public bool ReemplazarSegmentosDePromocion(Promocion promocion, List<string> segmentos)
        {
            PromocionIdsReemplazados.Add(promocion.Id);
            SegmentosPorPromocion[promocion.Id] = (segmentos ?? new List<string>()).ToList();
            return true;
        }
    }

    internal sealed class FakePromocionProductoRepositorio : IPromocionProductoRepositorio
    {
        public int ContarPorProducto(Producto producto)
        {
            return IncluidosPorPromocion.Values.Count(ids => ids.Contains(producto.Id))
                + ExcluidosPorPromocion.Values.Count(ids => ids.Contains(producto.Id));
        }

        public Dictionary<int, List<int>> IncluidosPorPromocion { get; } = new Dictionary<int, List<int>>();

        public Dictionary<int, List<int>> ExcluidosPorPromocion { get; } = new Dictionary<int, List<int>>();

        public List<int> PromocionIdsReemplazados { get; } = new List<int>();

        public List<Producto> ObtenerProductosPorPromocion(Promocion promocion, bool excluidos)
        {
            Dictionary<int, List<int>> origen = excluidos ? ExcluidosPorPromocion : IncluidosPorPromocion;
            List<int> ids;
            return origen.TryGetValue(promocion.Id, out ids) ? ids.Select(id => new Producto { Id = id }).ToList() : new List<Producto>();
        }

        public bool ReemplazarProductosDePromocion(Promocion promocion, List<Producto> incluidos, List<Producto> excluidos)
        {
            PromocionIdsReemplazados.Add(promocion.Id);
            IncluidosPorPromocion[promocion.Id] = (incluidos ?? new List<Producto>()).Select(item => item.Id).ToList();
            ExcluidosPorPromocion[promocion.Id] = (excluidos ?? new List<Producto>()).Select(item => item.Id).ToList();
            return true;
        }
    }

    internal sealed class FakePromocionCategoriaProductoRepositorio : IPromocionCategoriaProductoRepositorio
    {
        public int ContarPorCategoriaProducto(CategoriaProducto categoriaProducto)
        {
            return IncluidasPorPromocion.Values.Count(ids => ids.Contains(categoriaProducto.Id))
                + ExcluidasPorPromocion.Values.Count(ids => ids.Contains(categoriaProducto.Id));
        }

        public Dictionary<int, List<int>> IncluidasPorPromocion { get; } = new Dictionary<int, List<int>>();

        public Dictionary<int, List<int>> ExcluidasPorPromocion { get; } = new Dictionary<int, List<int>>();

        public List<int> PromocionIdsReemplazados { get; } = new List<int>();

        public List<CategoriaProducto> ObtenerCategoriasProductoPorPromocion(Promocion promocion, bool excluidos)
        {
            Dictionary<int, List<int>> origen = excluidos ? ExcluidasPorPromocion : IncluidasPorPromocion;
            List<int> ids;
            return origen.TryGetValue(promocion.Id, out ids) ? ids.Select(id => new CategoriaProducto { Id = id }).ToList() : new List<CategoriaProducto>();
        }

        public bool ReemplazarCategoriasProductoDePromocion(Promocion promocion, List<CategoriaProducto> incluidas, List<CategoriaProducto> excluidas)
        {
            PromocionIdsReemplazados.Add(promocion.Id);
            IncluidasPorPromocion[promocion.Id] = (incluidas ?? new List<CategoriaProducto>()).Select(item => item.Id).ToList();
            ExcluidasPorPromocion[promocion.Id] = (excluidas ?? new List<CategoriaProducto>()).Select(item => item.Id).ToList();
            return true;
        }
    }

    internal sealed class FakePromocionComercioRepositorio : IPromocionComercioRepositorio
    {
        public Dictionary<int, List<int>> ComercioIdsPorPromocion { get; } = new Dictionary<int, List<int>>();

        public Dictionary<int, List<int>> PromocionIdsPorComercio { get; } = new Dictionary<int, List<int>>();

        public int? LastPromocionIdReemplazado { get; private set; }

        public List<int> LastComercioIdsReemplazados { get; private set; }

        public List<Comercio> ObtenerComerciosPorPromocion(Promocion promocion)
        {
            List<int> comercioIds;
            return ComercioIdsPorPromocion.TryGetValue(promocion.Id, out comercioIds) ? comercioIds.Select(id => new Comercio { Id = id }).ToList() : new List<Comercio>();
        }

        public List<Promocion> ObtenerPromocionesPorComercio(Comercio comercio)
        {
            List<int> promocionIds;
            return PromocionIdsPorComercio.TryGetValue(comercio.Id, out promocionIds) ? promocionIds.Select(id => new Promocion { Id = id }).ToList() : new List<Promocion>();
        }

        public bool ReemplazarComerciosDePromocion(Promocion promocion, List<Comercio> comercios)
        {
            LastPromocionIdReemplazado = promocion.Id;
            LastComercioIdsReemplazados = comercios == null ? new List<int>() : comercios.Select(item => item.Id).ToList();
            ComercioIdsPorPromocion[promocion.Id] = LastComercioIdsReemplazados.ToList();
            return true;
        }
    }

    internal sealed class FakeMonedaRepositorio : IMonedaRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Moneda> Monedas { get; } = new Dictionary<int, Moneda>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Moneda LastInserted { get; private set; }

        public Moneda LastUpdated { get; private set; }

        public Moneda LastDeleted { get; private set; }

        public Moneda ObtenerPorId(int id)
        {
            Moneda moneda;
            return Monedas.TryGetValue(id, out moneda) ? moneda : null;
        }

        public List<Moneda> ObtenerTodas()
        {
            return Monedas.Values.ToList();
        }

        public int InsertarYObtenerId(Moneda moneda)
        {
            if (moneda.Id <= 0)
            {
                moneda.Id = _nextId++;
            }

            Monedas[moneda.Id] = moneda;
            LastInserted = moneda;
            return moneda.Id;
        }

        public bool Actualizar(Moneda moneda)
        {
            LastUpdated = moneda;
            if (!ActualizarResult)
            {
                return false;
            }

            Monedas[moneda.Id] = moneda;
            return true;
        }

        public bool Eliminar(Moneda moneda)
        {
            LastDeleted = moneda;
            if (!EliminarResult)
            {
                return false;
            }

            return Monedas.Remove(moneda.Id);
        }
    }

    internal sealed class FakeTasaConversionRepositorio : ITasaConversionRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, TasaConversion> Tasas { get; } = new Dictionary<int, TasaConversion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public TasaConversion LastInserted { get; private set; }

        public TasaConversion LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public TasaConversion ObtenerPorId(int id)
        {
            TasaConversion tasa;
            return Tasas.TryGetValue(id, out tasa) ? tasa : null;
        }

        public List<TasaConversion> ObtenerTodas()
        {
            return Tasas.Values.ToList();
        }

        public TasaConversion ObtenerPorParDeMonedas(Moneda monedaBase, Moneda monedaConversion)
        {
            return Tasas.Values.FirstOrDefault(item =>
                item.MonedaBase != null && item.MonedaBase.Id == monedaBase.Id &&
                item.MonedaConversion != null && item.MonedaConversion.Id == monedaConversion.Id);
        }

        public int InsertarYObtenerId(TasaConversion tasa)
        {
            if (tasa.Id <= 0)
            {
                tasa.Id = _nextId++;
            }

            Tasas[tasa.Id] = tasa;
            LastInserted = tasa;
            return tasa.Id;
        }

        public bool Actualizar(TasaConversion tasa)
        {
            LastUpdated = tasa;
            if (!ActualizarResult)
            {
                return false;
            }

            Tasas[tasa.Id] = tasa;
            return true;
        }

        public Action OnActualizarValorAutomatico { get; set; }

        public bool ActualizarValorAutomatico(TasaConversion recibida, decimal calculoConversion)
        {
            OnActualizarValorAutomatico?.Invoke();
            TasaConversion tasa;
            if (!Tasas.TryGetValue(recibida.Id, out tasa) || tasa.EsManual)
            {
                return false;
            }

            tasa.CalculoConversion = calculoConversion;
            tasa.FechaActualizacion = DateTime.Now;
            return true;
        }

        public bool MarcarManual(TasaConversion recibida, Usuario usuario)
        {
            TasaConversion tasa;
            if (!Tasas.TryGetValue(recibida.Id, out tasa))
            {
                return false;
            }

            tasa.EsManual = true;
            tasa.ProtegidaPorUsuario = usuario;
            tasa.FechaProteccion = DateTime.Now;
            return true;
        }

        public bool Liberar(TasaConversion recibida)
        {
            TasaConversion tasa;
            if (!Tasas.TryGetValue(recibida.Id, out tasa))
            {
                return false;
            }

            tasa.EsManual = false;
            tasa.ProtegidaPorUsuario = null;
            tasa.FechaProteccion = null;
            return true;
        }

        public bool Eliminar(TasaConversion tasa)
        {
            LastDeletedId = tasa.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Tasas.Remove(tasa.Id);
        }
    }

    internal sealed class FakeEjecucionActualizacionTasasRepositorio : IEjecucionActualizacionTasasRepositorio
    {
        private int _nextId = 1;

        private int _nextDetalleId = 1;

        public List<EjecucionActualizacionTasas> Ejecuciones { get; } = new List<EjecucionActualizacionTasas>();

        public List<ActualizacionTasaDetalle> Detalles { get; } = new List<ActualizacionTasaDetalle>();

        public int ObtenerUltimaExitosaConBloqueoCalls { get; private set; }

        public int InsertarYObtenerId(EjecucionActualizacionTasas ejecucion)
        {
            ejecucion.Id = _nextId++;
            Ejecuciones.Add(ejecucion);
            return ejecucion.Id;
        }

        public int InsertarDetalleYObtenerId(ActualizacionTasaDetalle detalle)
        {
            detalle.Id = _nextDetalleId++;
            Detalles.Add(detalle);
            return detalle.Id;
        }

        public List<EjecucionActualizacionTasas> ObtenerRecientes(int cantidad)
        {
            return Ejecuciones.OrderByDescending(item => item.FechaInicio).ThenByDescending(item => item.Id).Take(cantidad).ToList();
        }

        public List<ActualizacionTasaDetalle> ObtenerDetallePorEjecucion(EjecucionActualizacionTasas ejecucion)
        {
            return Detalles.Where(item => item.Ejecucion.Id == ejecucion.Id).ToList();
        }

        public EjecucionActualizacionTasas ObtenerUltimaExitosa()
        {
            return Ejecuciones
                .Where(item => item.Resultado == ResultadoActualizacionTasas.Exitosa)
                .OrderByDescending(item => item.FechaInicio)
                .ThenByDescending(item => item.Id)
                .FirstOrDefault();
        }

        public EjecucionActualizacionTasas ObtenerUltimaExitosaConBloqueo()
        {
            ObtenerUltimaExitosaConBloqueoCalls++;
            return ObtenerUltimaExitosa();
        }

        public int ContarRequestsDelMes(DateTime referencia)
        {
            return Ejecuciones.Count(item =>
                item.ConsumioRequest &&
                item.FechaInicio.Year == referencia.Year &&
                item.FechaInicio.Month == referencia.Month);
        }
    }

    internal sealed class FakeItemListaRepositorio : IItemListaRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, ItemLista> Items { get; } = new Dictionary<int, ItemLista>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public ItemLista LastInserted { get; private set; }

        public ItemLista LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public ItemLista ObtenerPorId(int id)
        {
            ItemLista item;
            return Items.TryGetValue(id, out item) ? item : null;
        }

        public List<ItemLista> ObtenerPorListaDeCompras(ListaDeCompras lista)
        {
            return Items.Values
                .Where(item => item.ListaDeCompras != null && item.ListaDeCompras.Id == lista.Id)
                .ToList();
        }

        public int InsertarYObtenerId(ItemLista itemLista)
        {
            if (itemLista.Id <= 0)
            {
                itemLista.Id = _nextId++;
            }

            Items[itemLista.Id] = itemLista;
            LastInserted = itemLista;
            return itemLista.Id;
        }

        public bool Actualizar(ItemLista itemLista)
        {
            LastUpdated = itemLista;
            if (!ActualizarResult)
            {
                return false;
            }

            Items[itemLista.Id] = itemLista;
            return true;
        }

        public bool Eliminar(ItemLista item)
        {
            LastDeletedId = item.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Items.Remove(item.Id);
        }
    }

    internal sealed class FakeListaDeComprasRepositorio : IListaDeComprasRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, ListaDeCompras> Listas { get; } = new Dictionary<int, ListaDeCompras>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public ListaDeCompras LastInserted { get; private set; }

        public ListaDeCompras LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public ListaDeCompras ObtenerPorId(int id)
        {
            ListaDeCompras lista;
            return Listas.TryGetValue(id, out lista) ? lista : null;
        }

        public List<ListaDeCompras> ObtenerPorUsuario(Usuario usuario)
        {
            return Listas.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id)
                .ToList();
        }

        public int InsertarYObtenerId(ListaDeCompras listaDeCompras)
        {
            if (listaDeCompras.Id <= 0)
            {
                listaDeCompras.Id = _nextId++;
            }

            Listas[listaDeCompras.Id] = listaDeCompras;
            LastInserted = listaDeCompras;
            return listaDeCompras.Id;
        }

        public bool Actualizar(ListaDeCompras listaDeCompras)
        {
            LastUpdated = listaDeCompras;
            if (!ActualizarResult)
            {
                return false;
            }

            Listas[listaDeCompras.Id] = listaDeCompras;
            return true;
        }

        public bool Eliminar(ListaDeCompras lista)
        {
            LastDeletedId = lista.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Listas.Remove(lista.Id);
        }
    }

    internal sealed class FakeNotificacionRepositorio : INotificacionRepositorio
    {
        public Notificacion ObtenerPersonal(Notificacion notificacion, Usuario usuario)
        {
            var n = ObtenerPorId(notificacion.Id);
            if (n == null || n.Usuario.Id != usuario.Id) return null;
            Notificacion copia = n is RecordatorioCompra ? (Notificacion)new RecordatorioCompra() : n is RecordatorioInversion ? new RecordatorioInversion() : new Notificacion();
            copia.Id = n.Id; copia.Usuario = n.Usuario; copia.Mensaje = n.Mensaje; copia.Fecha = n.Fecha; copia.Leida = n.Leida;
            return copia;
        }

        public bool MarcarPersonalLeida(Notificacion notificacion, Usuario usuario)
        {
            var n = ObtenerPersonal(notificacion, usuario);
            if (n == null || n.Leida || !ActualizarResult) return false;
            n.Leida = true;
            Notificaciones[n.Id] = n;
            return true;
        }

        public PaginaNotificaciones ObtenerPaginaPersonal(Usuario usuario, FiltroNotificacion filtro, int pagina)
        {
            var lista = ObtenerPorUsuario(usuario).Where(n => !filtro.FechaDesde.HasValue || n.Fecha.Date >= filtro.FechaDesde.Value.Date)
                .Where(n => !filtro.FechaHasta.HasValue || n.Fecha.Date <= filtro.FechaHasta.Value.Date)
                .OrderByDescending(n => n.Fecha).ThenByDescending(n => n.Id).ToList();
            return new PaginaNotificaciones { Total = lista.Count, Pagina = pagina, Elementos = lista.Skip((pagina - 1) * 10).Take(10).ToList() };
        }

        public int ContarNoLeidas(Usuario usuario) => ObtenerPorUsuario(usuario).Count(n => !n.Leida);

        private int _nextId = 1;

        public Dictionary<int, Notificacion> Notificaciones { get; } = new Dictionary<int, Notificacion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Notificacion LastInserted { get; private set; }

        public Notificacion LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        // Cantidad de notificaciones/recordatorios que referencian a cada promocion (el fake de
        // Notificacion no guarda PromocionId).
        public Dictionary<int, int> NotificacionesPorPromocion { get; } = new Dictionary<int, int>();

        public int ContarPorPromocion(Promocion promocion)
        {
            int cantidad;
            return NotificacionesPorPromocion.TryGetValue(promocion.Id, out cantidad) ? cantidad : 0;
        }

        public DateTime? UltimaAutomaticaInversionUtc { get; set; }

        public List<string> Llamadas { get; } = new List<string>();

        public bool UsuarioExiste { get; set; } = true;

        public DateTime? ObtenerFechaUltimaAutomaticaInversionUtc(Usuario usuario)
        {
            if (UltimaAutomaticaInversionUtc.HasValue) return UltimaAutomaticaInversionUtc;
            DateTime? ultima = Fuentes().Where(n => n.Usuario != null && n.Usuario.Id == usuario.Id && n.TipoAutomatico == TipoNotificacionAutomatica.Inversiones)
                .Select(n => (DateTime?)n.Fecha).Max();
            return ultima.HasValue ? DateTime.SpecifyKind(ultima.Value, DateTimeKind.Local).ToUniversalTime() : (DateTime?)null;
        }

        // Todas las notificaciones del fake (base y subtipos); la asigna FakeUnidadDeTrabajo.
        public Func<IEnumerable<Notificacion>> Fuentes { get; set; } = () => new Notificacion[0];

        public bool ExisteAutomatica(Usuario usuario, TipoNotificacionAutomatica tipo, DateTime jornada)
        {
            return Fuentes().Any(n => n.Usuario != null && n.Usuario.Id == usuario.Id && n.TipoAutomatico == tipo && n.FechaJornada.HasValue && n.FechaJornada.Value.Date == jornada.Date);
        }

        public bool BloquearUsuarioParaEntrega(Usuario usuario)
        {
            Llamadas.Add("bloquear:" + usuario.Id);
            return UsuarioExiste;
        }

        public Notificacion ObtenerPorId(int id)
        {
            Notificacion notificacion;
            return Notificaciones.TryGetValue(id, out notificacion) ? notificacion : null;
        }

        public List<Notificacion> ObtenerPorUsuario(Usuario usuario)
        {
            return Notificaciones.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id)
                .ToList();
        }

        public List<Notificacion> ObtenerTodas()
        {
            return Notificaciones.Values.ToList();
        }

        public int InsertarYObtenerId(Notificacion notificacion)
        {
            if (notificacion.Id <= 0)
            {
                notificacion.Id = _nextId++;
            }

            Notificaciones[notificacion.Id] = notificacion;
            LastInserted = notificacion;
            return notificacion.Id;
        }

        public bool Actualizar(Notificacion notificacion)
        {
            LastUpdated = notificacion;
            if (!ActualizarResult)
            {
                return false;
            }

            Notificaciones[notificacion.Id] = notificacion;
            return true;
        }

        public bool Eliminar(Notificacion notificacion)
        {
            LastDeletedId = notificacion.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Notificaciones.Remove(notificacion.Id);
        }
    }

    internal sealed class FakeRecordatorioCompraRepositorio : IRecordatorioCompraRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, RecordatorioCompra> Recordatorios { get; } = new Dictionary<int, RecordatorioCompra>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public RecordatorioCompra LastInserted { get; private set; }

        public RecordatorioCompra LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public RecordatorioCompra ObtenerPorId(int id)
        {
            RecordatorioCompra recordatorio;
            return Recordatorios.TryGetValue(id, out recordatorio) ? recordatorio : null;
        }

        public List<RecordatorioCompra> ObtenerPorUsuario(Usuario usuario)
        {
            return Recordatorios.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id)
                .ToList();
        }

        public List<RecordatorioCompra> ObtenerTodas()
        {
            return Recordatorios.Values.ToList();
        }

        public int InsertarYObtenerId(RecordatorioCompra recordatorio)
        {
            if (recordatorio.Id <= 0)
            {
                recordatorio.Id = _nextId++;
            }

            Recordatorios[recordatorio.Id] = recordatorio;
            LastInserted = recordatorio;
            return recordatorio.Id;
        }

        public bool Actualizar(RecordatorioCompra recordatorio)
        {
            LastUpdated = recordatorio;
            if (!ActualizarResult)
            {
                return false;
            }

            Recordatorios[recordatorio.Id] = recordatorio;
            return true;
        }

        public bool Eliminar(RecordatorioCompra recordatorio)
        {
            LastDeletedId = recordatorio.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Recordatorios.Remove(recordatorio.Id);
        }
    }

    internal sealed class FakeRecordatorioInversionRepositorio : IRecordatorioInversionRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, RecordatorioInversion> Recordatorios { get; } = new Dictionary<int, RecordatorioInversion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public RecordatorioInversion LastInserted { get; private set; }

        public RecordatorioInversion LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public RecordatorioInversion ObtenerPorId(int id)
        {
            RecordatorioInversion recordatorio;
            return Recordatorios.TryGetValue(id, out recordatorio) ? recordatorio : null;
        }

        public List<RecordatorioInversion> ObtenerPorUsuario(Usuario usuario)
        {
            return Recordatorios.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id)
                .ToList();
        }

        public List<RecordatorioInversion> ObtenerTodas()
        {
            return Recordatorios.Values.ToList();
        }

        public int InsertarYObtenerId(RecordatorioInversion recordatorio)
        {
            if (recordatorio.Id <= 0)
            {
                recordatorio.Id = _nextId++;
            }

            Recordatorios[recordatorio.Id] = recordatorio;
            LastInserted = recordatorio;
            return recordatorio.Id;
        }

        public bool Actualizar(RecordatorioInversion recordatorio)
        {
            LastUpdated = recordatorio;
            if (!ActualizarResult)
            {
                return false;
            }

            Recordatorios[recordatorio.Id] = recordatorio;
            return true;
        }

        public bool Eliminar(RecordatorioInversion recordatorio)
        {
            LastDeletedId = recordatorio.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Recordatorios.Remove(recordatorio.Id);
        }
    }

    internal sealed class FakeMovimientoRepositorio : IMovimientoRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Movimiento> Movimientos { get; } = new Dictionary<int, Movimiento>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Movimiento LastInserted { get; private set; }

        public Movimiento LastUpdated { get; private set; }

        public Movimiento LastDeleted { get; private set; }

        public Movimiento ObtenerPorId(int id)
        {
            Movimiento movimiento;
            return Movimientos.TryGetValue(id, out movimiento) ? movimiento : null;
        }

        public List<Movimiento> ObtenerPorUsuario(Usuario usuario, FiltroMovimiento filtro)
        {
            IEnumerable<Movimiento> query = Movimientos.Values.Where(item => item.Usuario.Id == usuario.Id);

            if (filtro != null)
            {
                if (filtro.FechaDesde.HasValue)
                {
                    query = query.Where(item => item.Fecha >= filtro.FechaDesde.Value);
                }

                if (filtro.FechaHasta.HasValue)
                {
                    query = query.Where(item => item.Fecha <= filtro.FechaHasta.Value);
                }

                if (filtro.Categoria != null)
                {
                    query = query.Where(item => item.Categoria.Id == filtro.Categoria.Id);
                }

                if (filtro.Tipo.HasValue)
                {
                    query = query.Where(item => item.Tipo == filtro.Tipo.Value);
                }
            }

            return query.OrderByDescending(item => item.Fecha).ToList();
        }

        public List<TotalPorCategoria> ObtenerTotalesPorCategoriaResult { get; set; } = new List<TotalPorCategoria>();

        public Tuple<Usuario, TipoMovimiento, DateTime, DateTime> LastObtenerTotalesPorCategoriaArgs { get; private set; }

        public List<TotalPorCategoria> ObtenerTotalesPorCategoria(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            LastObtenerTotalesPorCategoriaArgs = Tuple.Create(usuario, tipo, desde, hasta);
            return ObtenerTotalesPorCategoriaResult;
        }

        public List<TotalPorMes> ObtenerTotalesPorMesResult { get; set; } = new List<TotalPorMes>();

        public Tuple<Usuario, DateTime, DateTime> LastObtenerTotalesPorMesArgs { get; private set; }

        public List<TotalPorMes> ObtenerTotalesPorMes(Usuario usuario, DateTime desde, DateTime hasta)
        {
            LastObtenerTotalesPorMesArgs = Tuple.Create(usuario, desde, hasta);
            return ObtenerTotalesPorMesResult;
        }

        public List<TotalPorComercio> ObtenerTotalesPorComercioResult { get; set; } = new List<TotalPorComercio>();

        public Tuple<Usuario, TipoMovimiento, DateTime, DateTime> LastObtenerTotalesPorComercioArgs { get; private set; }

        public List<TotalPorComercio> ObtenerTotalesPorComercio(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            LastObtenerTotalesPorComercioArgs = Tuple.Create(usuario, tipo, desde, hasta);
            return ObtenerTotalesPorComercioResult;
        }

        public List<TotalPorDia> ObtenerTotalesPorDiaResult { get; set; } = new List<TotalPorDia>();

        public Tuple<Usuario, TipoMovimiento, DateTime, DateTime> LastObtenerTotalesPorDiaArgs { get; private set; }

        public List<TotalPorDia> ObtenerTotalesPorDia(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            LastObtenerTotalesPorDiaArgs = Tuple.Create(usuario, tipo, desde, hasta);
            return ObtenerTotalesPorDiaResult;
        }

        public ResumenMovimientos ObtenerResumenResult { get; set; } = new ResumenMovimientos();

        public Tuple<Usuario, DateTime, DateTime> LastObtenerResumenArgs { get; private set; }

        public ResumenMovimientos ObtenerResumen(Usuario usuario, DateTime desde, DateTime hasta)
        {
            LastObtenerResumenArgs = Tuple.Create(usuario, desde, hasta);
            return ObtenerResumenResult;
        }

        public int InsertarYObtenerId(Movimiento movimiento)
        {
            if (movimiento.Id <= 0)
            {
                movimiento.Id = _nextId++;
            }

            Movimientos[movimiento.Id] = movimiento;
            LastInserted = movimiento;
            return movimiento.Id;
        }

        public bool Actualizar(Movimiento movimiento)
        {
            LastUpdated = movimiento;
            if (!ActualizarResult)
            {
                return false;
            }

            Movimientos[movimiento.Id] = movimiento;
            return true;
        }

        public bool Eliminar(Movimiento movimiento)
        {
            LastDeleted = movimiento;
            if (!EliminarResult)
            {
                return false;
            }

            return Movimientos.Remove(movimiento.Id);
        }

        public int ContarPorComercio(Comercio comercio)
        {
            return Movimientos.Values.Count(item => item.Comercio != null && item.Comercio.Id == comercio.Id);
        }
    }

    internal sealed class FakeUsuarioRepositorio : IUsuarioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Usuario> Usuarios { get; } = new Dictionary<int, Usuario>();

        public bool ActualizarResult { get; set; } = true;

        public bool ActualizarDvhResult { get; set; } = true;

        public bool GuardarEstadoCompletoResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public bool ThrowOnInsert { get; set; }

        public bool ThrowOnActualizarEstadoBloqueo { get; set; }

        public Usuario LastInserted { get; private set; }

        public Usuario LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        // Como la base: ObtenerPorId devuelve una instancia nueva de la fila, no la guardada.
        public bool CopiarAlLeer { get; set; }

        public int ActualizarEstadoBloqueoCallCount { get; private set; }

        public Tuple<int, int, bool, DateTime?, string> LastEstadoBloqueoUpdate { get; private set; }

        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario;
            if (!Usuarios.TryGetValue(id, out usuario)) return null;
            return CopiarAlLeer ? new Usuario { Id = usuario.Id, Username = usuario.Username, PasswordHash = usuario.PasswordHash, Activo = usuario.Activo, DVH = usuario.DVH } : usuario;
        }

        public Usuario ObtenerPorUsername(string username)
        {
            return Usuarios.Values.FirstOrDefault(item => string.Equals(item.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public int InsertarYObtenerId(Usuario usuario)
        {
            if (ThrowOnInsert)
            {
                throw new InvalidOperationException("Insert failed.");
            }

            // Como la identidad de la base: el Id recibido no se respeta.
            usuario.Id = _nextId++;
            Usuarios[usuario.Id] = usuario;
            LastInserted = usuario;
            return usuario.Id;
        }

        public bool Actualizar(Usuario usuario)
        {
            LastUpdated = usuario;
            if (!ActualizarResult)
            {
                return false;
            }

            Usuarios[usuario.Id] = usuario;
            return true;
        }

        public bool ActualizarDVH(Usuario recibida, string dvh)
        {
            if (!ActualizarDvhResult)
            {
                return false;
            }

            Usuario usuario;
            if (Usuarios.TryGetValue(recibida.Id, out usuario))
            {
                usuario.DVH = dvh;
            }

            return true;
        }

        public bool GuardarEstadoCompleto(Usuario usuario)
        {
            if (!GuardarEstadoCompletoResult)
            {
                return false;
            }

            Usuarios[usuario.Id] = usuario;
            LastUpdated = usuario;
            return true;
        }

        public bool ActualizarEstadoBloqueo(Usuario recibida, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh)
        {
            int id = recibida.Id;
            if (ThrowOnActualizarEstadoBloqueo)
            {
                throw new InvalidOperationException("ActualizarEstadoBloqueo failed.");
            }

            ActualizarEstadoBloqueoCallCount++;
            LastEstadoBloqueoUpdate = Tuple.Create(id, intentosFallidos, bloqueado, bloqueadoHasta, dvh);

            Usuario usuario;
            if (Usuarios.TryGetValue(id, out usuario))
            {
                usuario.IntentosFallidos = intentosFallidos;
                usuario.Bloqueado = bloqueado;
                usuario.BloqueadoHasta = bloqueadoHasta;
                usuario.DVH = dvh;
            }

            return true;
        }

        public bool EliminarFisicamente(Usuario usuario)
        {
            LastDeletedId = usuario.Id;
            return EliminarResult && Usuarios.Remove(usuario.Id);
        }

        public List<Usuario> ObtenerActivosDespuesDe(Usuario ultimo, int cantidad)
        {
            return Usuarios.Values.Where(u => u.Activo && u.Id > (ultimo?.Id ?? 0)).OrderBy(u => u.Id).Take(cantidad).ToList();
        }

        public List<Usuario> ObtenerTodos()
        {
            return Usuarios.Values.ToList();
        }
    }

    internal sealed class FakePermisoRepositorio : IPermisoRepositorio
    {
        private int _nextId = 100;

        public List<IPermiso> Nodos { get; } = new List<IPermiso>();

        public List<Tuple<int, int>> Relaciones { get; } = new List<Tuple<int, int>>();

        public Dictionary<string, int> IdsPorNombre { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public List<IPermiso> Guardados { get; } = new List<IPermiso>();

        public List<int> IdsEliminados { get; private set; } = new List<int>();

        public List<Tuple<int, int>> RelacionesReemplazadas { get; private set; } = new List<Tuple<int, int>>();

        public bool ThrowOnGuardar { get; set; }

        public List<IPermiso> ObtenerNodos()
        {
            return Nodos.ToList();
        }

        public List<Tuple<int, int>> ObtenerRelaciones()
        {
            return Relaciones.ToList();
        }

        public int Guardar(IPermiso permiso)
        {
            if (ThrowOnGuardar)
            {
                throw new InvalidOperationException("Permission save failed.");
            }

            if (permiso.Id <= 0)
            {
                permiso.Id = _nextId++;
            }

            Guardados.Add(permiso);
            if (Nodos.All(item => item.Id != permiso.Id))
            {
                Nodos.Add(permiso);
            }

            return permiso.Id;
        }

        public bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones)
        {
            RelacionesReemplazadas = relaciones ?? new List<Tuple<int, int>>();
            Relaciones.Clear();
            Relaciones.AddRange(RelacionesReemplazadas);
            return true;
        }

        public bool EliminarPermisos(List<IPermiso> permisos)
        {
            IdsEliminados = permisos == null ? new List<int>() : permisos.Select(item => item.Id).ToList();
            Nodos.RemoveAll(item => IdsEliminados.Contains(item.Id));
            return true;
        }

        public IPermiso ObtenerPorNombre(string nombre)
        {
            int id;
            if (!IdsPorNombre.TryGetValue(nombre, out id))
            {
                return null;
            }

            return Nodos.FirstOrDefault(item => item.Id == id) ?? new PermisoGrupo { Id = id, Nombre = nombre };
        }
    }

    internal sealed class FakeUsuarioPermisoRepositorio : IUsuarioPermisoRepositorio
    {
        public Dictionary<int, List<int>> PermisosPorUsuario { get; } = new Dictionary<int, List<int>>();

        public int? LastUsuarioId { get; private set; }

        public List<int> LastPermisos { get; private set; }

        public List<IPermiso> ObtenerPermisosPorUsuario(Usuario usuario)
        {
            List<int> permisos;
            return PermisosPorUsuario.TryGetValue(usuario.Id, out permisos)
                ? permisos.Select(id => (IPermiso)new PermisoSimple { Id = id }).ToList()
                : new List<IPermiso>();
        }

        public bool ReemplazarPermisosUsuario(Usuario usuario, List<IPermiso> permisos)
        {
            LastUsuarioId = usuario.Id;
            LastPermisos = permisos == null ? new List<int>() : permisos.Select(item => item.Id).ToList();
            PermisosPorUsuario[usuario.Id] = LastPermisos.ToList();
            return true;
        }
    }

    internal sealed class FakeUsuarioHistorialRepositorio : IUsuarioHistorialRepositorio
    {
        private int _nextId = 1;

        public List<UsuarioHistorialSnapshot> Snapshots { get; } = new List<UsuarioHistorialSnapshot>();

        public UsuarioHistorialSnapshot LastSaved { get; private set; }

        public int GuardarVersion(UsuarioHistorialSnapshot snapshot)
        {
            if (snapshot.HistorialId <= 0)
            {
                snapshot.HistorialId = _nextId++;
            }

            LastSaved = snapshot;
            Snapshots.Add(snapshot);
            return snapshot.HistorialId;
        }

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(IUsuarioAutenticado usuario)
        {
            return Snapshots.Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id).ToList();
        }

        public UsuarioHistorialSnapshot ObtenerPorId(int id)
        {
            return Snapshots.FirstOrDefault(item => item.HistorialId == id);
        }
    }

    internal sealed class FakeUsuarioPreferenciaRepositorio : IUsuarioPreferenciaRepositorio
    {
        public Dictionary<int, UsuarioPreferencia> Preferencias { get; } = new Dictionary<int, UsuarioPreferencia>();

        public int? LastUsuarioId { get; private set; }

        public int? LastIdiomaId { get; private set; }

        public UsuarioPreferencia ObtenerPorUsuario(IUsuarioAutenticado usuario)
        {
            UsuarioPreferencia preferencia;
            return Preferencias.TryGetValue(usuario.Id, out preferencia) ? preferencia : null;
        }

        public bool Guardar(UsuarioPreferencia preferencia)
        {
            Preferencias[preferencia.Usuario.Id] = preferencia;
            LastUsuarioId = preferencia.Usuario.Id;
            LastIdiomaId = preferencia.Idioma?.Id;
            return true;
        }

        public bool Guardar(IUsuarioAutenticado usuario, IIdioma idioma)
        {
            LastUsuarioId = usuario.Id;
            LastIdiomaId = idioma?.Id;
            if (idioma != null)
            {
                Preferencias[usuario.Id] = new UsuarioPreferencia { Usuario = usuario, Idioma = idioma };
            }
            else
            {
                Preferencias.Remove(usuario.Id);
            }

            return true;
        }
    }

    internal sealed class FakeIdiomaRepositorio : IIdiomaRepositorio
    {
        private int _nextId = 1;

        public List<Idioma> Idiomas { get; } = new List<Idioma>();

        public List<Idioma> Insertados { get; } = new List<Idioma>();

        public Idioma LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public List<Idioma> ObtenerIdiomas()
        {
            return Idiomas.ToList();
        }

        public Idioma ObtenerPorId(int id)
        {
            return Idiomas.FirstOrDefault(item => item.Id == id);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            return Idiomas.FirstOrDefault(item => item.Default);
        }

        public int InsertarIdioma(Idioma idioma)
        {
            if (idioma.Id <= 0)
            {
                idioma.Id = _nextId++;
            }

            Insertados.Add(idioma);
            Idiomas.Add(idioma);
            return idioma.Id;
        }

        public bool ActualizarIdioma(Idioma idioma)
        {
            LastUpdated = idioma;
            Idioma existing = ObtenerPorId(idioma.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Nombre = idioma.Nombre;
            return true;
        }

        public bool EliminarIdioma(Idioma idioma)
        {
            LastDeletedId = idioma.Id;
            return Idiomas.RemoveAll(item => item.Id == idioma.Id) > 0;
        }

        public bool ConsolidarIdioma(Idioma origen, Idioma destino)
        {
            return true;
        }

        public Palabra ObtenerPalabraPorKey(string key)
        {
            return null;
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            return true;
        }

        public bool EliminarTraduccion(Traduccion traduccion)
        {
            return true;
        }

        public bool SwitchearIdiomaDefault(Idioma recibida)
        {
            Idioma idioma = ObtenerPorId(recibida.Id);
            if (idioma == null)
            {
                throw new InvalidOperationException("Language not found.");
            }

            idioma.Default = !idioma.Default;
            return true;
        }
    }

    internal sealed class FakeTraduccionRepositorio : ITraduccionRepositorio
    {
        public Dictionary<string, int> PalabraIdsPorKey { get; } = new Dictionary<string, int>(StringComparer.Ordinal);

        public List<Tuple<int, int, string>> TraduccionesGuardadas { get; } = new List<Tuple<int, int, string>>();

        public bool ThrowOnGuardar { get; set; }

        public List<Traduccion> TraduccionesEliminadas { get; } = new List<Traduccion>();

        public Palabra ObtenerPalabraPorKey(string key)
        {
            int id;
            return PalabraIdsPorKey.TryGetValue(key, out id) ? new Palabra { Id = id, Key = key } : null;
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            if (ThrowOnGuardar)
            {
                throw new InvalidOperationException("Translation save failed.");
            }

            TraduccionesGuardadas.Add(Tuple.Create(traduccion.Idioma.Id, traduccion.Palabra.Id, traduccion.Valor));
            return true;
        }

        public bool EliminarTraduccion(Traduccion traduccion)
        {
            TraduccionesEliminadas.Add(traduccion);
            return true;
        }
    }

    internal sealed class FakeIntegridadRepositorio : IIntegridadRepositorio
    {
        public Dictionary<string, string> DvvPorTabla { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

        public string LastTablaGuardada { get; private set; }

        public string LastValorGuardado { get; private set; }

        public string ObtenerDVV(string nombreTabla)
        {
            string value;
            return DvvPorTabla.TryGetValue(nombreTabla, out value) ? value : null;
        }

        public void GuardarDVV(string nombreTabla, string valor)
        {
            LastTablaGuardada = nombreTabla;
            LastValorGuardado = valor;
            DvvPorTabla[nombreTabla] = valor;
        }
    }

    internal sealed class FakeBackupRepositorio : IBackupRepositorio
    {
        public bool ThrowOnCrear { get; set; }

        public bool ThrowOnRestaurar { get; set; }

        public string UltimaRutaCreada { get; private set; }

        public string UltimaRutaRestaurada { get; private set; }

        public int CrearBackupCallCount { get; private set; }

        public int RestaurarBackupCallCount { get; private set; }

        public void CrearBackup(string rutaArchivo)
        {
            CrearBackupCallCount++;
            UltimaRutaCreada = rutaArchivo;

            if (ThrowOnCrear)
            {
                throw new ExcepcionTecnica("Error.Technical.SqlFailure");
            }
        }

        public void RestaurarBackup(string rutaArchivo)
        {
            RestaurarBackupCallCount++;
            UltimaRutaRestaurada = rutaArchivo;

            if (ThrowOnRestaurar)
            {
                throw new ExcepcionTecnica("Error.Technical.SqlFailure");
            }
        }
    }

    internal sealed class FakeBitacoraRepositorio : IBitacoraRepositorio
    {
        public List<EventoBitacora> Eventos { get; } = new List<EventoBitacora>();

        public FiltroBitacora LastFiltro { get; private set; }

        public int Guardar(EventoBitacora evento)
        {
            evento.Id = Eventos.Count + 1;
            Eventos.Add(evento);
            return evento.Id;
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            LastFiltro = filtro;
            return Eventos.ToList();
        }
    }

    internal sealed class FakeSesion : ISesion
    {
        private IUsuarioAutenticado _usuario;

        public int ActualizarNombreCallCount { get; private set; }

        public void IniciarSesion(IUsuarioAutenticado usuario)
        {
            _usuario = usuario;
        }

        public void CerrarSesion()
        {
            _usuario = null;
        }

        public IUsuarioAutenticado GetUsuarioActual()
        {
            return _usuario;
        }

        public string GetUsuarioActualNombre()
        {
            return _usuario == null ? null : _usuario.Username;
        }

        public void ActualizarUsuarioActualNombre(string username)
        {
            ActualizarNombreCallCount++;
            if (_usuario != null)
            {
                _usuario.Username = username;
            }
        }

        public bool HaySesionActiva()
        {
            return _usuario != null;
        }
    }

    internal sealed class FakeAutorizacion : IAutorizacion
    {
        public IList<IPermiso> ActualizarPermisosArgument { get; private set; }

        public int ActualizarPermisosCallCount { get; private set; }

        public bool LimpiarCalled { get; private set; }

        public void ActualizarPermisosUsuarioActual(IEnumerable<IPermiso> permisos)
        {
            ActualizarPermisosCallCount++;
            ActualizarPermisosArgument = permisos == null
                ? new List<IPermiso>()
                : permisos.ToList();
        }

        public void Limpiar()
        {
            LimpiarCalled = true;
        }

        public bool TienePermiso(string rutaPermiso)
        {
            return false;
        }
    }

    internal sealed class FakePasswordHashService : IPasswordHashService
    {
        public bool VerifyResult { get; set; } = true;

        public string LastHashInput { get; private set; }

        public string LastVerifyPassword { get; private set; }

        public string LastVerifyHash { get; private set; }

        public string HashPassword(string passwordRaw)
        {
            LastHashInput = passwordRaw;
            return "HASH:" + passwordRaw;
        }

        public bool VerifyPassword(string passwordRaw, string passwordHash)
        {
            LastVerifyPassword = passwordRaw;
            LastVerifyHash = passwordHash;
            return VerifyResult;
        }
    }

    internal sealed class SpyBitacoraBLL : IBitacoraBLL
    {
        public List<BitacoraCall> Calls { get; } = new List<BitacoraCall>();

        public List<EventoBitacora> BuscarResult { get; set; } = new List<EventoBitacora>();

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return BuscarResult;
        }

        public void RegistrarOperacion(string actividad, string informacion, string origen)
        {
            Calls.Add(new BitacoraCall("Operacion", actividad, informacion, origen, null));
        }

        public void RegistrarFalloNegocio(string actividad, string informacion, string origen)
        {
            Calls.Add(new BitacoraCall("FalloNegocio", actividad, informacion, origen, null));
        }

        public void RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            Calls.Add(new BitacoraCall("OperacionInterno", actividad, informacion, origen, unidad));
        }

        public void RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            Calls.Add(new BitacoraCall("FalloNegocioInterno", actividad, informacion, origen, unidad));
        }
    }

    internal sealed class SpyIdiomaBLL : IIdiomaBLL
    {
        public int? AplicarIdiomaSesionUsuarioId { get; private set; }

        public Idioma IdiomaDelUsuario { get; set; }

        public Action<int> AlObtenerIdioma { get; set; }

        public Idioma ObtenerIdiomaParaUsuario(Usuario usuario)
        {
            AlObtenerIdioma?.Invoke(usuario.Id);
            return IdiomaDelUsuario;
        }

        public void AplicarIdiomaSesionUsuario(Usuario usuario)
        {
            AplicarIdiomaSesionUsuarioId = usuario.Id;
        }
    }

    internal sealed class SpyPermisoBLL : IPermisoBLL
    {
        public List<IPermiso> PermisosDirectos { get; set; } = new List<IPermiso>();

        public int? ObtenerPermisosUsuarioId { get; private set; }

        public int? AsignarPerfilUsuarioId { get; private set; }

        public int? GuardarPermisosUsuarioId { get; private set; }

        public List<int> GuardarPermisosIds { get; private set; }

        public IUnidadDeTrabajo LastUnidad { get; private set; }

        public List<IPermiso> ObtenerPermisosDirectosUsuario(Usuario usuario)
        {
            ObtenerPermisosUsuarioId = usuario.Id;
            return PermisosDirectos;
        }

        public void AsignarPerfilBasicoInterno(Usuario usuario, IUnidadDeTrabajo unidad)
        {
            AsignarPerfilUsuarioId = usuario.Id;
            LastUnidad = unidad;
        }

        public void GuardarPermisosUsuarioInterno(Usuario usuario, List<IPermiso> permisos, IUnidadDeTrabajo unidad)
        {
            GuardarPermisosUsuarioId = usuario.Id;
            GuardarPermisosIds = permisos == null ? null : permisos.Select(item => item.Id).ToList();
            LastUnidad = unidad;
        }

        public int? ValidarPuedeEliminarUsuarioId { get; private set; }

        public bool ThrowOnValidarPuedeEliminarUsuario { get; set; }

        public void ValidarPuedeEliminarUsuario(Usuario usuario, IUnidadDeTrabajo unidad)
        {
            ValidarPuedeEliminarUsuarioId = usuario.Id;
            LastUnidad = unidad;
            if (ThrowOnValidarPuedeEliminarUsuario)
            {
                throw new ExcepcionNegocio("Error.Permission.OrphanedPatente");
            }
        }
    }

    internal sealed class SpyUsuarioEstadoBLL : IUsuarioEstadoBLL
    {
        public Dictionary<int, UsuarioHistorialSnapshot> Versiones { get; } = new Dictionary<int, UsuarioHistorialSnapshot>();

        public int? GuardarUsuarioId { get; private set; }

        public Usuario GuardarUsuario { get; private set; }

        public string GuardarAccion { get; private set; }

        public string GuardarObservaciones { get; private set; }

        public int? AplicarSnapshotUsuarioId { get; private set; }

        public IUnidadDeTrabajo LastUnidad { get; private set; }

        public UsuarioHistorialSnapshot GuardarVersionActual(Usuario usuario, string accion, string observaciones)
        {
            GuardarUsuarioId = usuario.Id;
            GuardarUsuario = usuario;
            GuardarAccion = accion;
            GuardarObservaciones = observaciones;
            return new UsuarioHistorialSnapshot { HistorialId = 1, Usuario = usuario, Accion = accion, Observaciones = observaciones };
        }

        public UsuarioHistorialSnapshot GuardarVersionActualInterno(Usuario usuario, string accion, string observaciones, IUnidadDeTrabajo unidad)
        {
            GuardarUsuarioId = usuario.Id;
            GuardarUsuario = usuario;
            GuardarAccion = accion;
            GuardarObservaciones = observaciones;
            LastUnidad = unidad;
            return new UsuarioHistorialSnapshot { HistorialId = 1, Usuario = usuario, Accion = accion, Observaciones = observaciones };
        }

        public List<UsuarioHistorialSnapshot> ObtenerHistorial(Usuario usuario)
        {
            return Versiones.Values.Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id).ToList();
        }

        public UsuarioHistorialSnapshot ObtenerVersion(IUsuarioSnapshot recibida)
        {
            UsuarioHistorialSnapshot snapshot;
            return recibida != null && Versiones.TryGetValue(recibida.HistorialId, out snapshot) ? snapshot : null;
        }

        public List<ResultadoComparacionEstado> CompararVersiones(UsuarioHistorialSnapshot versionActual, UsuarioHistorialSnapshot versionAnterior)
        {
            return new List<ResultadoComparacionEstado>();
        }

        public void AplicarSnapshotInterno(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad)
        {
            AplicarSnapshotUsuarioId = snapshot == null || snapshot.Usuario == null ? (int?)null : snapshot.Usuario.Id;
            LastUnidad = unidad;
        }
    }

    internal sealed class SpyIntegridadBLL : IIntegridadBLL
    {
        public string NextDvh { get; set; } = "DVH";

        public List<Tuple<string, string, string, IUnidadDeTrabajo>> ActualizarDvvCalls { get; } = new List<Tuple<string, string, string, IUnidadDeTrabajo>>();

        public string CalcularDVH<T>(T entidad) where T : class
        {
            Usuario usuario = entidad as Usuario;
            return usuario == null ? NextDvh : "DVH:" + usuario.Username + ":" + usuario.PasswordHash + ":" + usuario.Activo + ":" + usuario.IntentosFallidos + ":" + usuario.Bloqueado + ":" + usuario.BloqueadoHasta;
        }

        public void RecalcularDVVInterno(string tabla, IEnumerable<string> listaDVH, IUnidadDeTrabajo unidad)
        {
        }

        public void ActualizarDVVInterno(string tabla, string dvhAnterior, string dvhNuevo, IUnidadDeTrabajo unidad)
        {
            ActualizarDvvCalls.Add(Tuple.Create(tabla, dvhAnterior, dvhNuevo, unidad));
        }

        public void ValidarIntegridadGlobal()
        {
        }
    }

    internal sealed class FakeIntegridadService : IIntegridadService
    {
        public string NextDvh { get; set; } = "DVH";

        public string NextDvv { get; set; } = "DVV";

        public string NextIncrementalDvv { get; set; } = "DVV-INCREMENTAL";

        public object LastEntity { get; private set; }

        public IEnumerable<string> LastDvhList { get; private set; }

        public Tuple<string, string, string> LastIncrementalArgs { get; private set; }

        public string CalcularDVH<T>(T entidad) where T : class
        {
            LastEntity = entidad;
            Usuario usuario = entidad as Usuario;
            return usuario == null ? NextDvh : "DVH:" + usuario.Username + ":" + usuario.PasswordHash + ":" + usuario.Activo;
        }

        public string CalcularDVV(IEnumerable<string> listaDVH)
        {
            LastDvhList = listaDVH == null ? null : listaDVH.ToList();
            return NextDvv;
        }

        public string CalcularDVVIncremental(string dvvActual, string dvhAnterior, string dvhNuevo)
        {
            LastIncrementalArgs = Tuple.Create(dvvActual, dvhAnterior, dvhNuevo);
            return NextIncrementalDvv;
        }
    }

    internal sealed class FakeEscaneoTicketService : IEscaneoTicketService
    {
        public ITicketEscaneado NextResultado { get; set; } = new TicketEscaneado();

        public Exception ThrowException { get; set; }

        public byte[] LastImagenBytes { get; private set; }

        public Task<ITicketEscaneado> EscanearTicketAsync(byte[] imagenBytes)
        {
            LastImagenBytes = imagenBytes;
            if (ThrowException != null)
            {
                throw ThrowException;
            }

            return Task.FromResult(NextResultado);
        }
    }

    internal sealed class FakeObservado : IObservado
    {
        public IIdioma IdiomaActual { get; private set; }

        public int CambiarIdiomaCallCount { get; private set; }

        public void RegistrarObservador(IObservador observador)
        {
        }

        public void DesregistrarObservador(IObservador observador)
        {
        }

        public void Notificar()
        {
        }

        public void CambiarIdioma(IIdioma idioma)
        {
            CambiarIdiomaCallCount++;
            IdiomaActual = idioma;
        }
    }

    internal sealed class BitacoraCall
    {
        public BitacoraCall(string kind, string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            Kind = kind;
            Actividad = actividad;
            Informacion = informacion;
            Origen = origen;
            Unidad = unidad;
        }

        public string Kind { get; private set; }

        public string Actividad { get; private set; }

        public string Informacion { get; private set; }

        public string Origen { get; private set; }

        public IUnidadDeTrabajo Unidad { get; private set; }
    }

    internal sealed class FakeReglaCategorizacionRepositorio : IReglaCategorizacionRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, ReglaCategorizacion> Reglas { get; } = new Dictionary<int, ReglaCategorizacion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public ReglaCategorizacion LastInserted { get; private set; }

        public ReglaCategorizacion LastUpdated { get; private set; }

        public ReglaCategorizacion LastDeleted { get; private set; }

        public ReglaCategorizacion ObtenerPorId(int id)
        {
            ReglaCategorizacion regla;
            return Reglas.TryGetValue(id, out regla) ? regla : null;
        }

        public List<ReglaCategorizacion> ObtenerPorUsuario(Usuario usuario)
        {
            return Reglas.Values.Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id).ToList();
        }

        public int InsertarYObtenerId(ReglaCategorizacion reglaCategorizacion)
        {
            if (reglaCategorizacion.Id <= 0)
            {
                reglaCategorizacion.Id = _nextId++;
            }

            Reglas[reglaCategorizacion.Id] = reglaCategorizacion;
            LastInserted = reglaCategorizacion;
            return reglaCategorizacion.Id;
        }

        public bool Actualizar(ReglaCategorizacion reglaCategorizacion)
        {
            LastUpdated = reglaCategorizacion;
            if (!ActualizarResult)
            {
                return false;
            }

            Reglas[reglaCategorizacion.Id] = reglaCategorizacion;
            return true;
        }

        public bool Eliminar(ReglaCategorizacion reglaCategorizacion)
        {
            LastDeleted = reglaCategorizacion;
            if (!EliminarResult)
            {
                return false;
            }

            return Reglas.Remove(reglaCategorizacion.Id);
        }
    }

    internal sealed class FakeComercioReglaCategorizacionRepositorio : IComercioReglaCategorizacionRepositorio
    {
        public Dictionary<int, List<int>> ComerciosPorRegla { get; } = new Dictionary<int, List<int>>();

        public bool ReemplazarResult { get; set; } = true;

        // Ids asociados a la regla, para preparar datos y verificar en los tests.
        public List<int> ComercioIdsDe(int reglaCategorizacionId)
        {
            List<int> ids;
            return ComerciosPorRegla.TryGetValue(reglaCategorizacionId, out ids) ? new List<int>(ids) : new List<int>();
        }

        public List<Comercio> ObtenerComerciosPorReglaCategorizacion(ReglaCategorizacion reglaCategorizacion)
        {
            return ComercioIdsDe(reglaCategorizacion.Id).Select(id => new Comercio { Id = id }).ToList();
        }

        public bool ReemplazarComerciosDeReglaCategorizacion(ReglaCategorizacion reglaCategorizacion, IEnumerable<Comercio> comercios)
        {
            if (!ReemplazarResult)
            {
                return false;
            }

            ComerciosPorRegla[reglaCategorizacion.Id] = comercios == null ? new List<int>() : comercios.Select(item => item.Id).ToList();
            return true;
        }

        public int ContarPorComercio(Comercio comercio)
        {
            return ComerciosPorRegla.Values.Count(ids => ids.Contains(comercio.Id));
        }
    }

    internal sealed class FakeConsultaChatbotRepositorio : IConsultaChatbotRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, ConsultaChatbot> Consultas { get; } = new Dictionary<int, ConsultaChatbot>();

        public ConsultaChatbot LastInserted { get; private set; }

        public List<ConsultaChatbot> ObtenerPorUsuario(Usuario usuario)
        {
            return Consultas.Values.Where(item => item.Usuario?.Id == usuario.Id).ToList();
        }

        public int InsertarYObtenerId(ConsultaChatbot consultaChatbot)
        {
            if (consultaChatbot.Id <= 0)
            {
                consultaChatbot.Id = _nextId++;
            }

            Consultas[consultaChatbot.Id] = consultaChatbot;
            LastInserted = consultaChatbot;
            return consultaChatbot.Id;
        }
    }

    internal sealed class FakeDashboardRepositorio : IDashboardRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Dashboard> Dashboards { get; } = new Dictionary<int, Dashboard>();

        public bool EliminarResult { get; set; } = true;

        public Dashboard LastInserted { get; private set; }

        public int? LastDeletedId { get; private set; }

        public Dashboard ObtenerPorId(int id)
        {
            Dashboard dashboard;
            return Dashboards.TryGetValue(id, out dashboard) ? dashboard : null;
        }

        public Dashboard ObtenerPorUsuario(Usuario usuario)
        {
            return Dashboards.Values.FirstOrDefault(item => item.Usuario != null && item.Usuario.Id == usuario.Id);
        }

        public int InsertarYObtenerId(Dashboard dashboard)
        {
            if (dashboard.Id <= 0)
            {
                dashboard.Id = _nextId++;
            }

            Dashboards[dashboard.Id] = dashboard;
            LastInserted = dashboard;
            return dashboard.Id;
        }

        public bool Eliminar(Dashboard dashboard)
        {
            LastDeletedId = dashboard.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Dashboards.Remove(dashboard.Id);
        }
    }

    internal sealed class FakeTarjetaDashboardRepositorio : ITarjetaDashboardRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, TarjetaDashboard> Tarjetas { get; } = new Dictionary<int, TarjetaDashboard>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public TarjetaDashboard LastInserted { get; private set; }

        public TarjetaDashboard LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public TarjetaDashboard ObtenerPorId(int id)
        {
            TarjetaDashboard tarjeta;
            return Tarjetas.TryGetValue(id, out tarjeta) ? tarjeta : null;
        }

        public List<TarjetaDashboard> ObtenerPorDashboard(Dashboard dashboard)
        {
            return Tarjetas.Values
                .Where(item => item.Dashboard != null && item.Dashboard.Id == dashboard.Id)
                .OrderBy(item => item.Orden)
                .ToList();
        }

        public int InsertarYObtenerId(TarjetaDashboard tarjeta)
        {
            if (tarjeta.Id <= 0)
            {
                tarjeta.Id = _nextId++;
            }

            Tarjetas[tarjeta.Id] = tarjeta;
            LastInserted = tarjeta;
            return tarjeta.Id;
        }

        public bool Actualizar(TarjetaDashboard tarjeta)
        {
            LastUpdated = tarjeta;
            if (!ActualizarResult)
            {
                return false;
            }

            Tarjetas[tarjeta.Id] = tarjeta;
            return true;
        }

        public bool Eliminar(TarjetaDashboard tarjeta)
        {
            LastDeletedId = tarjeta.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Tarjetas.Remove(tarjeta.Id);
        }
    }

    internal sealed class FakeChatbotFinancieroRepositorio : IChatbotFinancieroRepositorio
    {
        public ChatbotFinanciero Chatbot { get; set; } = new ChatbotFinanciero { Id = 1, Nombre = "Asistente Financiero FINAZAPP" };

        public ChatbotFinanciero ObtenerUnico()
        {
            return Chatbot;
        }
    }

    internal sealed class FakeAsistenteFinancieroService : IAsistenteFinancieroService
    {
        public ILlamadaHerramientaAsistente LlamadaHerramienta { get; set; }
        public IHerramientaAsistente UltimaHerramienta { get; private set; }
        public List<string> OrdenLlamadas { get; } = new List<string>();
        public Action AlExtraerHerramienta { get; set; }
        public Exception ThrowOnHerramienta { get; set; }
        public Task<ILlamadaHerramientaAsistente> ObtenerLlamadaHerramientaAsync(ISolicitudAsistente solicitud, IHerramientaAsistente herramienta, System.Threading.CancellationToken cancellationToken)
        {
            UltimaHerramienta = herramienta;
            OrdenLlamadas.Add("herramienta");
            AlExtraerHerramienta?.Invoke();
            if (ThrowOnHerramienta != null) throw ThrowOnHerramienta;
            return Task.FromResult(LlamadaHerramienta);
        }
        public string NextRespuesta { get; set; } = "Respuesta simulada";

        public Exception ThrowException { get; set; }

        public string LastPregunta { get; private set; }

        public List<IIntercambioChatbot> LastHistorial { get; private set; }

        public ISolicitudAsistente LastSolicitud { get; private set; }

        public int CallCount { get; private set; }

        public Task<string> ObtenerRespuestaAsync(ISolicitudAsistente solicitud)
        {
            OrdenLlamadas.Add("respuesta");
            CallCount++;
            LastSolicitud = solicitud;
            LastPregunta = solicitud.Pregunta;
            LastHistorial = solicitud.Historial == null ? new List<IIntercambioChatbot>() : solicitud.Historial.ToList();

            if (ThrowException != null)
            {
                throw ThrowException;
            }

            return Task.FromResult(NextRespuesta);
        }
    }

    internal sealed class FakeClasificadorConsultas : IClasificadorConsultas
    {
        public CategoriaConsulta NextCategoria { get; set; } = CategoriaConsulta.FinanzasGenerales;

        public Exception ThrowException { get; set; }

        public int CallCount { get; private set; }

        public string LastPregunta { get; private set; }

        public Task<CategoriaConsulta> ClasificarAsync(string pregunta, System.Threading.CancellationToken cancellationToken)
        {
            CallCount++;
            LastPregunta = pregunta;

            if (ThrowException != null)
            {
                throw ThrowException;
            }

            return Task.FromResult(NextCategoria);
        }
    }

    internal sealed class FakeMedioPagoRepositorio : IMedioPagoRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, MedioPago> MediosPago { get; } = new Dictionary<int, MedioPago>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnEliminar { get; set; }

        public MedioPago LastInserted { get; private set; }

        public MedioPago LastUpdated { get; private set; }

        public MedioPago LastDeleted { get; private set; }

        public MedioPago ObtenerPorId(int id)
        {
            MedioPago medioPago;
            return MediosPago.TryGetValue(id, out medioPago) ? medioPago : null;
        }

        public List<MedioPago> ObtenerTodos()
        {
            return MediosPago.Values.ToList();
        }

        public int InsertarYObtenerId(MedioPago medioPago)
        {
            if (medioPago.Id <= 0)
            {
                medioPago.Id = _nextId++;
            }

            MediosPago[medioPago.Id] = medioPago;
            LastInserted = medioPago;
            return medioPago.Id;
        }

        public bool Actualizar(MedioPago medioPago)
        {
            LastUpdated = medioPago;
            if (!ActualizarResult)
            {
                return false;
            }

            MediosPago[medioPago.Id] = medioPago;
            return true;
        }

        public bool Eliminar(MedioPago medioPago)
        {
            LastDeleted = medioPago;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return MediosPago.Remove(medioPago.Id);
        }
    }

    internal sealed class FakeMedioPagoEntidadRepositorio : IMedioPagoEntidadRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, MedioPagoEntidad> MediosPagoEntidad { get; } = new Dictionary<int, MedioPagoEntidad>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnEliminar { get; set; }

        public MedioPagoEntidad LastInserted { get; private set; }

        public MedioPagoEntidad LastDeleted { get; private set; }

        public MedioPagoEntidad ObtenerPorId(int id)
        {
            MedioPagoEntidad medioPagoEntidad;
            return MediosPagoEntidad.TryGetValue(id, out medioPagoEntidad) ? medioPagoEntidad : null;
        }

        public List<MedioPagoEntidad> ObtenerPorEntidadFinanciera(EntidadFinanciera entidad)
        {
            return MediosPagoEntidad.Values
                .Where(item => item.EntidadFinanciera != null && item.EntidadFinanciera.Id == entidad.Id)
                .ToList();
        }

        public List<MedioPagoEntidad> ObtenerPorMedioPago(MedioPago medioPago)
        {
            return MediosPagoEntidad.Values
                .Where(item => item.MedioPago != null && item.MedioPago.Id == medioPago.Id)
                .ToList();
        }

        public List<string> ObtenerSegmentosDistintos()
        {
            return MediosPagoEntidad.Values
                .Select(item => item.Segmento)
                .Where(segmento => !string.IsNullOrWhiteSpace(segmento))
                .Distinct()
                .OrderBy(segmento => segmento)
                .ToList();
        }

        public int InsertarYObtenerId(MedioPagoEntidad medioPagoEntidad)
        {
            if (medioPagoEntidad.Id <= 0)
            {
                medioPagoEntidad.Id = _nextId++;
            }

            MediosPagoEntidad[medioPagoEntidad.Id] = medioPagoEntidad;
            LastInserted = medioPagoEntidad;
            return medioPagoEntidad.Id;
        }

        public bool Actualizar(MedioPagoEntidad medioPagoEntidad)
        {
            if (!ActualizarResult)
            {
                return false;
            }

            MediosPagoEntidad[medioPagoEntidad.Id] = medioPagoEntidad;
            return true;
        }

        public bool Eliminar(MedioPagoEntidad medioPagoEntidad)
        {
            LastDeleted = medioPagoEntidad;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return MediosPagoEntidad.Remove(medioPagoEntidad.Id);
        }
    }

    internal sealed class FakeEntidadFinancieraRepositorio : IEntidadFinancieraRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, EntidadFinanciera> Entidades { get; } = new Dictionary<int, EntidadFinanciera>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Exception ExcepcionEnEliminar { get; set; }

        public EntidadFinanciera LastInserted { get; private set; }

        public EntidadFinanciera LastUpdated { get; private set; }

        public EntidadFinanciera LastDeleted { get; private set; }

        public EntidadFinanciera ObtenerPorId(int id)
        {
            EntidadFinanciera entidad;
            return Entidades.TryGetValue(id, out entidad) ? entidad : null;
        }

        public List<EntidadFinanciera> ObtenerTodas()
        {
            return Entidades.Values.ToList();
        }

        public int InsertarYObtenerId(EntidadFinanciera entidad)
        {
            if (entidad.Id <= 0)
            {
                entidad.Id = _nextId++;
            }

            Entidades[entidad.Id] = entidad;
            LastInserted = entidad;
            return entidad.Id;
        }

        public bool Actualizar(EntidadFinanciera entidad)
        {
            LastUpdated = entidad;
            if (!ActualizarResult)
            {
                return false;
            }

            Entidades[entidad.Id] = entidad;
            return true;
        }

        public bool Eliminar(EntidadFinanciera entidad)
        {
            LastDeleted = entidad;
            if (ExcepcionEnEliminar != null)
            {
                throw ExcepcionEnEliminar;
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Entidades.Remove(entidad.Id);
        }
    }

    internal sealed class FakeCuentaFinancieraRepositorio : ICuentaFinancieraRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, CuentaFinanciera> Cuentas { get; } = new Dictionary<int, CuentaFinanciera>();

        // Estado de la tabla puente CuentaFinancieraMedioPagoEntidad, por cuenta.
        public Dictionary<int, List<int>> MedioPagoEntidadIdsPorCuenta { get; } = new Dictionary<int, List<int>>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public CuentaFinanciera LastInserted { get; private set; }

        public CuentaFinanciera LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public int ReemplazarCallCount { get; private set; }

        public bool ThrowOnReemplazar { get; set; }

        public CuentaFinanciera ObtenerPorId(int id)
        {
            CuentaFinanciera cuenta;
            return Cuentas.TryGetValue(id, out cuenta) ? cuenta : null;
        }

        public List<CuentaFinanciera> ObtenerPorUsuario(Usuario usuario)
        {
            return Cuentas.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id)
                .ToList();
        }

        public List<CuentaFinanciera> ObtenerPorEntidadFinanciera(EntidadFinanciera entidad)
        {
            return Cuentas.Values
                .Where(item => item.EntidadFinanciera != null && item.EntidadFinanciera.Id == entidad.Id)
                .ToList();
        }

        public int InsertarYObtenerId(CuentaFinanciera cuenta)
        {
            if (cuenta.Id <= 0)
            {
                cuenta.Id = _nextId++;
            }

            Cuentas[cuenta.Id] = cuenta;
            LastInserted = cuenta;
            return cuenta.Id;
        }

        public bool Actualizar(CuentaFinanciera cuenta)
        {
            LastUpdated = cuenta;
            if (!ActualizarResult)
            {
                return false;
            }

            Cuentas[cuenta.Id] = cuenta;
            return true;
        }

        public bool Eliminar(CuentaFinanciera cuenta)
        {
            LastDeletedId = cuenta.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Cuentas.Remove(cuenta.Id);
        }

        public bool ReemplazarMediosPagoEntidadDeCuenta(CuentaFinanciera cuenta, IEnumerable<MedioPagoEntidad> medios)
        {
            ReemplazarCallCount++;
            if (ThrowOnReemplazar)
            {
                throw new InvalidOperationException("Fallo simulado al reemplazar medios de pago.");
            }

            MedioPagoEntidadIdsPorCuenta[cuenta.Id] = medios.Select(item => item.Id).ToList();
            return true;
        }

        public int ContarPorMedioPagoEntidad(MedioPagoEntidad medio)
        {
            return MedioPagoEntidadIdsPorCuenta.Values.Count(ids => ids.Contains(medio.Id));
        }
    }

    internal sealed class FakeZonaUsuarioRepositorio : IZonaUsuarioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, ZonaUsuario> Zonas { get; } = new Dictionary<int, ZonaUsuario>();

        public ZonaUsuario LastInserted { get; private set; }

        public int? LastDeletedId { get; private set; }

        public bool EliminarResult { get; set; } = true;

        public ZonaUsuario ObtenerPorId(int id)
        {
            ZonaUsuario zona;
            return Zonas.TryGetValue(id, out zona) ? zona : null;
        }

        public List<ZonaUsuario> ObtenerPorUsuario(Usuario usuario)
        {
            return Zonas.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuario.Id)
                .OrderBy(item => item.Nombre)
                .ToList();
        }

        public int InsertarYObtenerId(ZonaUsuario zona)
        {
            while (Zonas.ContainsKey(_nextId))
            {
                _nextId++;
            }

            zona.Id = _nextId++;
            Zonas[zona.Id] = zona;
            LastInserted = zona;
            return zona.Id;
        }

        public bool Eliminar(ZonaUsuario zona)
        {
            LastDeletedId = zona.Id;
            if (!EliminarResult)
            {
                return false;
            }

            return Zonas.Remove(zona.Id);
        }
    }

    internal sealed class FakeServicioEmbeddings : IServicioEmbeddings
    {
        public const string ModeloFalso = "modelo-test";
        public string Modelo => ModeloFalso;
        public Dictionary<string, float[]> Vectores { get; } = new Dictionary<string, float[]>();
        public List<string> TextosRecibidos { get; } = new List<string>();
        public int Llamadas { get; private set; }
        public Exception ThrowException { get; set; }

        public Task<IReadOnlyList<float[]>> CalcularAsync(IReadOnlyList<string> textos, System.Threading.CancellationToken cancellationToken)
        {
            Llamadas++;
            TextosRecibidos.AddRange(textos);
            if (ThrowException != null) throw ThrowException;
            return Task.FromResult<IReadOnlyList<float[]>>(textos.Select(texto => Vectores.ContainsKey(texto) ? Vectores[texto] : new[] { 0f, 1f }).ToList());
        }
    }

    internal sealed class FakeProductoEmbeddingRepositorio : IProductoEmbeddingRepositorio
    {
        public Dictionary<int, ProductoEmbedding> Embeddings { get; } = new Dictionary<int, ProductoEmbedding>();

        public List<ProductoEmbedding> Guardados { get; } = new List<ProductoEmbedding>();

        public List<ProductoEmbedding> ObtenerTodos()
        {
            return Embeddings.Values.OrderBy(item => item.Producto.Id).ToList();
        }

        public void Guardar(ProductoEmbedding embedding)
        {
            Guardados.Add(embedding);
            Embeddings[embedding.Producto.Id] = embedding;
        }
    }

    // Cupo fijo por usuario (sin ventana): el reloj de los tests no avanza.
    internal sealed class FakeLimiteUsoAsistente : ILimiteUsoAsistente
    {
        private readonly int _maximo;
        private readonly Dictionary<int, int> _consumidasPorUsuario = new Dictionary<int, int>();

        public FakeLimiteUsoAsistente(int maximo)
        {
            _maximo = maximo;
        }

        public bool IntentarConsumir(IUsuarioAutenticado usuario)
        {
            lock (_consumidasPorUsuario)
            {
                int consumidas;
                _consumidasPorUsuario.TryGetValue(usuario.Id, out consumidas);
                if (consumidas >= _maximo)
                {
                    return false;
                }

                _consumidasPorUsuario[usuario.Id] = consumidas + 1;
                return true;
            }
        }
    }
}
