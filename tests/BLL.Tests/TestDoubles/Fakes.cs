using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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
        public FakeUnidadDeTrabajo()
        {
            BitacoraRepo = new FakeBitacoraRepositorio();
            CategoriaRepo = new FakeCategoriaRepositorio();
            CategoriaProductoRepo = new FakeCategoriaProductoRepositorio();
            ComercioRepo = new FakeComercioRepositorio();
            MonedaRepo = new FakeMonedaRepositorio();
            TasaConversionRepo = new FakeTasaConversionRepositorio();
            IdiomaRepo = new FakeIdiomaRepositorio();
            IntegridadRepo = new FakeIntegridadRepositorio();
            ItemListaRepo = new FakeItemListaRepositorio();
            ListaDeComprasRepo = new FakeListaDeComprasRepositorio();
            MovimientoRepo = new FakeMovimientoRepositorio();
            NotificacionRepo = new FakeNotificacionRepositorio();
            PermisoRepo = new FakePermisoRepositorio();
            ProductoRepo = new FakeProductoRepositorio();
            PromocionRepo = new FakePromocionRepositorio();
            PromocionComercioRepo = new FakePromocionComercioRepositorio();
            RecordatorioCompraRepo = new FakeRecordatorioCompraRepositorio();
            RecordatorioInversionRepo = new FakeRecordatorioInversionRepositorio();
            RubroComercioRepo = new FakeRubroComercioRepositorio();
            TraduccionRepo = new FakeTraduccionRepositorio();
            UsuarioHistorialRepo = new FakeUsuarioHistorialRepositorio();
            UsuarioPermisoRepo = new FakeUsuarioPermisoRepositorio();
            UsuarioPreferenciaRepo = new FakeUsuarioPreferenciaRepositorio();
            UsuarioRepo = new FakeUsuarioRepositorio();
        }

        public FakeBitacoraRepositorio BitacoraRepo { get; set; }

        public FakeCategoriaRepositorio CategoriaRepo { get; set; }

        public FakeCategoriaProductoRepositorio CategoriaProductoRepo { get; set; }

        public FakeComercioRepositorio ComercioRepo { get; set; }

        public FakeMonedaRepositorio MonedaRepo { get; set; }

        public FakeTasaConversionRepositorio TasaConversionRepo { get; set; }

        public FakeIdiomaRepositorio IdiomaRepo { get; set; }

        public FakeIntegridadRepositorio IntegridadRepo { get; set; }

        public FakeItemListaRepositorio ItemListaRepo { get; set; }

        public FakeListaDeComprasRepositorio ListaDeComprasRepo { get; set; }

        public FakeMovimientoRepositorio MovimientoRepo { get; set; }

        public FakeNotificacionRepositorio NotificacionRepo { get; set; }

        public FakePermisoRepositorio PermisoRepo { get; set; }

        public FakeProductoRepositorio ProductoRepo { get; set; }

        public FakePromocionRepositorio PromocionRepo { get; set; }

        public FakePromocionComercioRepositorio PromocionComercioRepo { get; set; }

        public FakeRecordatorioCompraRepositorio RecordatorioCompraRepo { get; set; }

        public FakeRecordatorioInversionRepositorio RecordatorioInversionRepo { get; set; }

        public FakeRubroComercioRepositorio RubroComercioRepo { get; set; }

        public FakeTraduccionRepositorio TraduccionRepo { get; set; }

        public FakeUsuarioHistorialRepositorio UsuarioHistorialRepo { get; set; }

        public FakeUsuarioPermisoRepositorio UsuarioPermisoRepo { get; set; }

        public FakeUsuarioPreferenciaRepositorio UsuarioPreferenciaRepo { get; set; }

        public FakeUsuarioRepositorio UsuarioRepo { get; set; }

        public bool SaveCalled { get; private set; }

        public int SaveCallCount { get; private set; }

        public bool DisposeCalled { get; private set; }

        public bool ThrowOnSave { get; set; }

        public IBitacoraRepositorio Bitacora { get { return BitacoraRepo; } }

        public ICategoriaRepositorio Categorias { get { return CategoriaRepo; } }

        public ICategoriaProductoRepositorio CategoriasProducto { get { return CategoriaProductoRepo; } }

        public IComercioRepositorio Comercios { get { return ComercioRepo; } }

        public IMonedaRepositorio Monedas { get { return MonedaRepo; } }

        public IIdiomaRepositorio Idiomas { get { return IdiomaRepo; } }

        public IIntegridadRepositorio Integridad { get { return IntegridadRepo; } }

        public IItemListaRepositorio ItemsLista { get { return ItemListaRepo; } }

        public IListaDeComprasRepositorio ListasDeCompras { get { return ListaDeComprasRepo; } }

        public IMovimientoRepositorio Movimientos { get { return MovimientoRepo; } }

        public INotificacionRepositorio Notificaciones { get { return NotificacionRepo; } }

        public IPermisoRepositorio Permisos { get { return PermisoRepo; } }

        public IProductoRepositorio Productos { get { return ProductoRepo; } }

        public IPromocionRepositorio Promociones { get { return PromocionRepo; } }

        public IPromocionComercioRepositorio PromocionComercio { get { return PromocionComercioRepo; } }

        public IRecordatorioCompraRepositorio RecordatoriosCompra { get { return RecordatorioCompraRepo; } }

        public IRecordatorioInversionRepositorio RecordatoriosInversion { get { return RecordatorioInversionRepo; } }

        public IRubroComercioRepositorio RubrosComercio { get { return RubroComercioRepo; } }

        public ITasaConversionRepositorio TasasConversion { get { return TasaConversionRepo; } }

        public ITraduccionRepositorio Traducciones { get { return TraduccionRepo; } }

        public IUsuarioHistorialRepositorio UsuarioHistorial { get { return UsuarioHistorialRepo; } }

        public IUsuarioPermisoRepositorio UsuarioPermisos { get { return UsuarioPermisoRepo; } }

        public IUsuarioPreferenciaRepositorio UsuarioPreferencias { get { return UsuarioPreferenciaRepo; } }

        public IUsuarioRepositorio Usuarios { get { return UsuarioRepo; } }

        public void Save()
        {
            if (ThrowOnSave)
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

        public int? LastDeletedId { get; private set; }

        public Categoria ObtenerPorId(int id)
        {
            Categoria categoria;
            return Categorias.TryGetValue(id, out categoria) ? categoria : null;
        }

        public List<Categoria> ObtenerVisiblesParaUsuario(int usuarioId)
        {
            return Categorias.Values
                .Where(item => item.Usuario == null || item.Usuario.Id == usuarioId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Categorias.Remove(id);
        }

        public bool ExisteNombreEnScope(string nombre, int? usuarioId, int? idExcluido)
        {
            return Categorias.Values.Any(item =>
                item.Usuario?.Id == usuarioId &&
                string.Equals(item.Nombre, nombre, StringComparison.OrdinalIgnoreCase) &&
                (!idExcluido.HasValue || item.Id != idExcluido.Value));
        }
    }

    internal sealed class FakeRubroComercioRepositorio : IRubroComercioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, RubroComercio> Rubros { get; } = new Dictionary<int, RubroComercio>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public bool ThrowForeignKeyOnEliminar { get; set; }

        public RubroComercio LastInserted { get; private set; }

        public RubroComercio LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (ThrowForeignKeyOnEliminar)
            {
                throw FakeSqlExceptionFactory.CrearExcepcionTecnicaPorViolacionForanea();
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Rubros.Remove(id);
        }
    }

    internal sealed class FakeComercioRepositorio : IComercioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Comercio> Comercios { get; } = new Dictionary<int, Comercio>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public bool ThrowForeignKeyOnEliminar { get; set; }

        public Comercio LastInserted { get; private set; }

        public Comercio LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public Comercio ObtenerPorId(int id)
        {
            Comercio comercio;
            return Comercios.TryGetValue(id, out comercio) ? comercio : null;
        }

        public List<Comercio> ObtenerPorRubro(int rubroComercioId)
        {
            return Comercios.Values
                .Where(item => item.RubroComercio != null && item.RubroComercio.Id == rubroComercioId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (ThrowForeignKeyOnEliminar)
            {
                throw FakeSqlExceptionFactory.CrearExcepcionTecnicaPorViolacionForanea();
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Comercios.Remove(id);
        }
    }

    // Construye, via reflexion, un SqlException real con Number = 547 (violacion de FK),
    // envuelto igual que lo hace AccesoDatosSqlServer.ExecuteInTransaction en produccion.
    // SqlException no tiene constructor publico; esta es la tecnica estandar para
    // instanciarlo en tests sin depender de una base de datos real.
    internal static class FakeSqlExceptionFactory
    {
        public static ExcepcionTecnica CrearExcepcionTecnicaPorViolacionForanea()
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

        public bool ThrowForeignKeyOnEliminar { get; set; }

        public CategoriaProducto LastInserted { get; private set; }

        public CategoriaProducto LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (ThrowForeignKeyOnEliminar)
            {
                throw FakeSqlExceptionFactory.CrearExcepcionTecnicaPorViolacionForanea();
            }

            if (!EliminarResult)
            {
                return false;
            }

            return Categorias.Remove(id);
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

        public int? LastDeletedId { get; private set; }

        public Producto ObtenerPorId(int id)
        {
            Producto producto;
            return Productos.TryGetValue(id, out producto) ? producto : null;
        }

        public List<Producto> ObtenerPorComercio(int comercioId)
        {
            return Productos.Values
                .Where(item => item.Comercio != null && item.Comercio.Id == comercioId)
                .ToList();
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Productos.Remove(id);
        }
    }

    internal sealed class FakePromocionRepositorio : IPromocionRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Promocion> Promociones { get; } = new Dictionary<int, Promocion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Promociones.Remove(id);
        }
    }

    internal sealed class FakePromocionComercioRepositorio : IPromocionComercioRepositorio
    {
        public Dictionary<int, List<int>> ComercioIdsPorPromocion { get; } = new Dictionary<int, List<int>>();

        public Dictionary<int, List<int>> PromocionIdsPorComercio { get; } = new Dictionary<int, List<int>>();

        public int? LastPromocionIdReemplazado { get; private set; }

        public List<int> LastComercioIdsReemplazados { get; private set; }

        public List<int> ObtenerComercioIdsPorPromocion(int promocionId)
        {
            List<int> comercioIds;
            return ComercioIdsPorPromocion.TryGetValue(promocionId, out comercioIds) ? comercioIds.ToList() : new List<int>();
        }

        public List<int> ObtenerPromocionIdsPorComercio(int comercioId)
        {
            List<int> promocionIds;
            return PromocionIdsPorComercio.TryGetValue(comercioId, out promocionIds) ? promocionIds.ToList() : new List<int>();
        }

        public bool ReemplazarComerciosDePromocion(int promocionId, List<int> comercioIds)
        {
            LastPromocionIdReemplazado = promocionId;
            LastComercioIdsReemplazados = comercioIds == null ? new List<int>() : comercioIds.ToList();
            ComercioIdsPorPromocion[promocionId] = LastComercioIdsReemplazados.ToList();
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

        public int? LastDeletedId { get; private set; }

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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Monedas.Remove(id);
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

        public TasaConversion ObtenerPorParDeMonedas(int monedaBaseId, int monedaConversionId)
        {
            return Tasas.Values.FirstOrDefault(item =>
                item.MonedaBase != null && item.MonedaBase.Id == monedaBaseId &&
                item.MonedaConversion != null && item.MonedaConversion.Id == monedaConversionId);
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Tasas.Remove(id);
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

        public List<ItemLista> ObtenerPorListaDeCompras(int listaDeComprasId)
        {
            return Items.Values
                .Where(item => item.ListaDeCompras != null && item.ListaDeCompras.Id == listaDeComprasId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Items.Remove(id);
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

        public List<ListaDeCompras> ObtenerPorUsuario(int usuarioId)
        {
            return Listas.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuarioId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Listas.Remove(id);
        }
    }

    internal sealed class FakeNotificacionRepositorio : INotificacionRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Notificacion> Notificaciones { get; } = new Dictionary<int, Notificacion>();

        public bool ActualizarResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public Notificacion LastInserted { get; private set; }

        public Notificacion LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public Notificacion ObtenerPorId(int id)
        {
            Notificacion notificacion;
            return Notificaciones.TryGetValue(id, out notificacion) ? notificacion : null;
        }

        public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        {
            return Notificaciones.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuarioId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Notificaciones.Remove(id);
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

        public List<RecordatorioCompra> ObtenerPorUsuario(int usuarioId)
        {
            return Recordatorios.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuarioId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Recordatorios.Remove(id);
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

        public List<RecordatorioInversion> ObtenerPorUsuario(int usuarioId)
        {
            return Recordatorios.Values
                .Where(item => item.Usuario != null && item.Usuario.Id == usuarioId)
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Recordatorios.Remove(id);
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

        public int? LastDeletedId { get; private set; }

        public Movimiento ObtenerPorId(int id)
        {
            Movimiento movimiento;
            return Movimientos.TryGetValue(id, out movimiento) ? movimiento : null;
        }

        public List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro)
        {
            IEnumerable<Movimiento> query = Movimientos.Values.Where(item => item.Usuario.Id == usuarioId);

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

                if (filtro.CategoriaId.HasValue)
                {
                    query = query.Where(item => item.Categoria.Id == filtro.CategoriaId.Value);
                }

                if (filtro.Tipo.HasValue)
                {
                    query = query.Where(item => item.Tipo == filtro.Tipo.Value);
                }
            }

            return query.OrderByDescending(item => item.Fecha).ToList();
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

        public bool Eliminar(int id)
        {
            LastDeletedId = id;
            if (!EliminarResult)
            {
                return false;
            }

            return Movimientos.Remove(id);
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

        public int ActualizarEstadoBloqueoCallCount { get; private set; }

        public Tuple<int, int, bool, DateTime?, string> LastEstadoBloqueoUpdate { get; private set; }

        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario;
            return Usuarios.TryGetValue(id, out usuario) ? usuario : null;
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

            if (usuario.Id <= 0)
            {
                usuario.Id = _nextId++;
            }

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

        public bool ActualizarDVH(int id, string dvh)
        {
            if (!ActualizarDvhResult)
            {
                return false;
            }

            Usuario usuario = ObtenerPorId(id);
            if (usuario != null)
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

        public bool ActualizarEstadoBloqueo(int id, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh)
        {
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

        public bool EliminarFisicamente(int id)
        {
            LastDeletedId = id;
            return EliminarResult && Usuarios.Remove(id);
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

        public bool EliminarPermisos(List<int> idsPermiso)
        {
            IdsEliminados = idsPermiso ?? new List<int>();
            Nodos.RemoveAll(item => IdsEliminados.Contains(item.Id));
            return true;
        }

        public int ObtenerIdPorNombre(string nombre)
        {
            int id;
            return IdsPorNombre.TryGetValue(nombre, out id) ? id : 0;
        }
    }

    internal sealed class FakeUsuarioPermisoRepositorio : IUsuarioPermisoRepositorio
    {
        public Dictionary<int, List<int>> PermisosPorUsuario { get; } = new Dictionary<int, List<int>>();

        public int? LastUsuarioId { get; private set; }

        public List<int> LastPermisos { get; private set; }

        public List<int> ObtenerPermisoIdsPorUsuario(int usuarioId)
        {
            List<int> permisos;
            return PermisosPorUsuario.TryGetValue(usuarioId, out permisos) ? permisos.ToList() : new List<int>();
        }

        public bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            LastUsuarioId = usuarioId;
            LastPermisos = permisosId == null ? new List<int>() : permisosId.ToList();
            PermisosPorUsuario[usuarioId] = LastPermisos.ToList();
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

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(int usuarioId)
        {
            return Snapshots.Where(item => item.UsuarioId == usuarioId).ToList();
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            return Snapshots.FirstOrDefault(item => item.HistorialId == historialId);
        }
    }

    internal sealed class FakeUsuarioPreferenciaRepositorio : IUsuarioPreferenciaRepositorio
    {
        public Dictionary<int, UsuarioPreferencia> Preferencias { get; } = new Dictionary<int, UsuarioPreferencia>();

        public int? LastUsuarioId { get; private set; }

        public int? LastIdiomaId { get; private set; }

        public UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId)
        {
            UsuarioPreferencia preferencia;
            return Preferencias.TryGetValue(usuarioId, out preferencia) ? preferencia : null;
        }

        public bool Guardar(UsuarioPreferencia preferencia)
        {
            Preferencias[preferencia.UsuarioId] = preferencia;
            LastUsuarioId = preferencia.UsuarioId;
            LastIdiomaId = preferencia.IdiomaId;
            return true;
        }

        public bool Guardar(int usuarioId, int? idiomaId)
        {
            LastUsuarioId = usuarioId;
            LastIdiomaId = idiomaId;
            if (idiomaId.HasValue)
            {
                Preferencias[usuarioId] = new UsuarioPreferencia { UsuarioId = usuarioId, IdiomaId = idiomaId.Value };
            }
            else
            {
                Preferencias.Remove(usuarioId);
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

        public Idioma ObtenerIdiomaCompleto(int idiomaId)
        {
            return Idiomas.FirstOrDefault(item => item.Id == idiomaId);
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
            Idioma existing = ObtenerIdiomaCompleto(idioma.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Nombre = idioma.Nombre;
            return true;
        }

        public bool EliminarIdioma(int idiomaId)
        {
            LastDeletedId = idiomaId;
            return Idiomas.RemoveAll(item => item.Id == idiomaId) > 0;
        }

        public bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId)
        {
            return true;
        }

        public int ObtenerPalabraIdPorKey(string key)
        {
            return 0;
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
        {
            return true;
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            return true;
        }

        public bool SwitchearIdiomaDefault(int idiomaId)
        {
            Idioma idioma = ObtenerIdiomaCompleto(idiomaId);
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

        public int ObtenerPalabraIdPorKey(string key)
        {
            int id;
            return PalabraIdsPorKey.TryGetValue(key, out id) ? id : 0;
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
        {
            if (ThrowOnGuardar)
            {
                throw new InvalidOperationException("Translation save failed.");
            }

            TraduccionesGuardadas.Add(Tuple.Create(idiomaId, palabraId, valor));
            return true;
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
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

        public int? GetUsuarioActualId()
        {
            return _usuario == null ? (int?)null : _usuario.Id;
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

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            AplicarIdiomaSesionUsuarioId = usuarioId;
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

        public List<IPermiso> ObtenerPermisosDirectosUsuario(int usuarioId)
        {
            ObtenerPermisosUsuarioId = usuarioId;
            return PermisosDirectos;
        }

        public void AsignarPerfilBasicoInterno(int usuarioId, IUnidadDeTrabajo unidad)
        {
            AsignarPerfilUsuarioId = usuarioId;
            LastUnidad = unidad;
        }

        public void GuardarPermisosUsuarioInterno(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad)
        {
            GuardarPermisosUsuarioId = usuarioId;
            GuardarPermisosIds = permisosId;
            LastUnidad = unidad;
        }

        public int? ValidarPuedeEliminarUsuarioId { get; private set; }

        public bool ThrowOnValidarPuedeEliminarUsuario { get; set; }

        public void ValidarPuedeEliminarUsuario(int usuarioId, IUnidadDeTrabajo unidad)
        {
            ValidarPuedeEliminarUsuarioId = usuarioId;
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

        public string GuardarAccion { get; private set; }

        public string GuardarObservaciones { get; private set; }

        public int? AplicarSnapshotUsuarioId { get; private set; }

        public IUnidadDeTrabajo LastUnidad { get; private set; }

        public int GuardarVersionActual(int usuarioId, string accion, string observaciones)
        {
            GuardarUsuarioId = usuarioId;
            GuardarAccion = accion;
            GuardarObservaciones = observaciones;
            return 1;
        }

        public int GuardarVersionActualInterno(int usuarioId, string accion, string observaciones, IUnidadDeTrabajo unidad)
        {
            GuardarUsuarioId = usuarioId;
            GuardarAccion = accion;
            GuardarObservaciones = observaciones;
            LastUnidad = unidad;
            return 1;
        }

        public List<UsuarioHistorialSnapshot> ObtenerHistorial(int usuarioId)
        {
            return Versiones.Values.Where(item => item.UsuarioId == usuarioId).ToList();
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            UsuarioHistorialSnapshot snapshot;
            return Versiones.TryGetValue(historialId, out snapshot) ? snapshot : null;
        }

        public List<ResultadoComparacionEstado> CompararVersiones(UsuarioHistorialSnapshot versionActual, UsuarioHistorialSnapshot versionAnterior)
        {
            return new List<ResultadoComparacionEstado>();
        }

        public void AplicarSnapshotInterno(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad)
        {
            AplicarSnapshotUsuarioId = snapshot == null ? (int?)null : snapshot.UsuarioId;
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
}
