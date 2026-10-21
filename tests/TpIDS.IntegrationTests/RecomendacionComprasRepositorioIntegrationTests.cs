using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Lecturas de la recomendacion de compras. Patron de aislamiento: cada test abre su
    // propia transaccion y hace RollbackTransactionScope() al final.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class RecomendacionComprasRepositorioIntegrationTests
    {
        [TestMethod]
        public void ObtenerItemsPendientesPorUsuario_SoloNoCompradosDelUsuarioConNombreDeLista()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecomendacionComprasRepositorio repositorio = new RecomendacionComprasRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioId(accesoDatos, "user");
                int otroUsuarioId = ObtenerUsuarioId(accesoDatos, "admin");
                int listaId = InsertarLista(accesoDatos, usuarioId, "ListaTestRecomendacion");
                int listaAjenaId = InsertarLista(accesoDatos, otroUsuarioId, "ListaTestAjena");
                int pendienteId = InsertarItem(accesoDatos, listaId, "ItemTestPendiente", 2, false);
                int compradoId = InsertarItem(accesoDatos, listaId, "ItemTestComprado", 1, true);
                int ajenoId = InsertarItem(accesoDatos, listaAjenaId, "ItemTestAjeno", 1, false);

                List<BE.ItemLista> items = repositorio.ObtenerItemsPendientesPorUsuario(new BE.Usuario { Id = usuarioId });

                BE.ItemLista pendiente = items.Single(item => item.Id == pendienteId);
                Assert.AreEqual("ItemTestPendiente", pendiente.Descripcion);
                Assert.AreEqual(2, pendiente.Cantidad);
                Assert.IsFalse(pendiente.Comprado);
                Assert.AreEqual(listaId, pendiente.ListaDeCompras.Id);
                Assert.AreEqual("ListaTestRecomendacion", pendiente.ListaDeCompras.Nombre);
                Assert.AreEqual(usuarioId, pendiente.ListaDeCompras.Usuario.Id);
                Assert.IsFalse(items.Any(item => item.Id == compradoId));
                Assert.IsFalse(items.Any(item => item.Id == ajenoId));
                Assert.IsTrue(items.All(item => item.ListaDeCompras.Usuario.Id == usuarioId));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerProductosConCategoria_IncluyeElNombreDeLaCategoria()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecomendacionComprasRepositorio repositorio = new RecomendacionComprasRepositorio(accesoDatos);
                int categoriaId = InsertarCategoria(accesoDatos, "CatTestRecomendacion");
                int productoId = InsertarProducto(accesoDatos, categoriaId, "ProductoTestRecomendacion");

                BE.Producto producto = repositorio.ObtenerProductosConCategoria().Single(item => item.Id == productoId);

                Assert.AreEqual("ProductoTestRecomendacion", producto.Nombre);
                Assert.AreEqual(categoriaId, producto.CategoriaProducto.Id);
                Assert.AreEqual("CatTestRecomendacion", producto.CategoriaProducto.Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerOfertas_TraeComercioConRubroYUbicacionProductoConCategoriaYMoneda()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecomendacionComprasRepositorio repositorio = new RecomendacionComprasRepositorio(accesoDatos);
                int rubroId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "RubroTestOfertas" });
                int comercioFisicoId = InsertarComercio(accesoDatos, rubroId, "ComercioTestOfertasFisico", -34.6m, -58.4m);
                int comercioOnlineId = InsertarComercio(accesoDatos, rubroId, "ComercioTestOfertasOnline", null, null);
                int categoriaId = InsertarCategoria(accesoDatos, "CatTestOfertas");
                int productoId = InsertarProducto(accesoDatos, categoriaId, "ProductoTestOfertas");
                int monedaId = new MonedaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Moneda { Codigo = "TOF", Nombre = "MonedaTestOfertas" });
                ComercioProductoRepositorio asignaciones = new ComercioProductoRepositorio(accesoDatos);
                asignaciones.Insertar(CrearOferta(comercioFisicoId, productoId, monedaId, 150.50m));
                asignaciones.Insertar(CrearOferta(comercioOnlineId, productoId, monedaId, 140m));

                List<BE.ComercioProducto> ofertas = repositorio.ObtenerOfertas().Where(item => item.Producto.Id == productoId).ToList();

                Assert.AreEqual(2, ofertas.Count);
                BE.ComercioProducto fisica = ofertas.Single(item => item.Comercio.Id == comercioFisicoId);
                Assert.AreEqual("ComercioTestOfertasFisico", fisica.Comercio.Nombre);
                Assert.AreEqual(rubroId, fisica.Comercio.RubroComercio.Id);
                Assert.AreEqual(-34.6m, fisica.Comercio.Latitud);
                Assert.AreEqual(-58.4m, fisica.Comercio.Longitud);
                Assert.AreEqual("ProductoTestOfertas", fisica.Producto.Nombre);
                Assert.AreEqual(categoriaId, fisica.Producto.CategoriaProducto.Id);
                Assert.AreEqual(monedaId, fisica.Moneda.Id);
                Assert.AreEqual("TOF", fisica.Moneda.Codigo);
                Assert.AreEqual(150.50m, fisica.Precio);
                BE.ComercioProducto online = ofertas.Single(item => item.Comercio.Id == comercioOnlineId);
                Assert.IsNull(online.Comercio.Latitud);
                Assert.IsNull(online.Comercio.Longitud);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPromocionesVigentesEntre_ExcluyeVencidasYFuturasEHidrataLosFiltros()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecomendacionComprasRepositorio repositorio = new RecomendacionComprasRepositorio(accesoDatos);
                DateTime desde = new DateTime(2031, 3, 10);
                DateTime hasta = desde.AddDays(6);
                int vigenteId = InsertarPromocion(accesoDatos, "PromoTestVigente", desde.AddDays(-5), desde.AddDays(2));
                int empiezaDentroId = InsertarPromocion(accesoDatos, "PromoTestEmpiezaDentro", hasta, hasta.AddDays(10));
                int vencidaId = InsertarPromocion(accesoDatos, "PromoTestVencida", desde.AddDays(-20), desde.AddDays(-1));
                int futuraId = InsertarPromocion(accesoDatos, "PromoTestFutura", hasta.AddDays(1), hasta.AddDays(9));
                int rubroId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "RubroTestPromoFiltro" });
                int comercioId = InsertarComercio(accesoDatos, rubroId, "ComercioTestPromoFiltro", null, null);
                int entidadId = new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "BancoTestPromoFiltro" });
                int medioId = new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago { Tipo = BE.TipoMedioPago.TarjetaCredito, Marca = "MarcaTestPromoFiltro" });
                int categoriaId = InsertarCategoria(accesoDatos, "CatTestPromoFiltro");
                int productoIncluidoId = InsertarProducto(accesoDatos, categoriaId, "ProductoTestPromoIncluido");
                int productoExcluidoId = InsertarProducto(accesoDatos, categoriaId, "ProductoTestPromoExcluido");
                BE.Promocion referencia = new BE.Promocion { Id = vigenteId };
                new PromocionRepositorio(accesoDatos).ReemplazarDiasDePromocion(referencia, new List<BE.DiaSemana> { BE.DiaSemana.Jueves });
                new PromocionComercioRepositorio(accesoDatos).ReemplazarComerciosDePromocion(referencia, new List<BE.Comercio> { new BE.Comercio { Id = comercioId } });
                new PromocionRubroComercioRepositorio(accesoDatos).ReemplazarRubrosComercioDePromocion(referencia, new List<BE.RubroComercio> { new BE.RubroComercio { Id = rubroId } });
                new PromocionEntidadFinancieraRepositorio(accesoDatos).ReemplazarEntidadesFinancierasDePromocion(referencia, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadId } });
                new PromocionMedioPagoRepositorio(accesoDatos).ReemplazarMediosPagoDePromocion(referencia, new List<BE.MedioPago> { new BE.MedioPago { Id = medioId } });
                new PromocionSegmentoRepositorio(accesoDatos).ReemplazarSegmentosDePromocion(referencia, new List<string> { "SegmentoTest" });
                new PromocionProductoRepositorio(accesoDatos).ReemplazarProductosDePromocion(referencia, new List<BE.Producto> { new BE.Producto { Id = productoIncluidoId } }, new List<BE.Producto> { new BE.Producto { Id = productoExcluidoId } });
                new PromocionCategoriaProductoRepositorio(accesoDatos).ReemplazarCategoriasProductoDePromocion(referencia, new List<BE.CategoriaProducto> { new BE.CategoriaProducto { Id = categoriaId } }, new List<BE.CategoriaProducto>());

                List<BE.Promocion> promociones = repositorio.ObtenerPromocionesVigentesEntre(desde, hasta);

                List<int> ids = promociones.Select(item => item.Id).ToList();
                CollectionAssert.Contains(ids, vigenteId);
                CollectionAssert.Contains(ids, empiezaDentroId);
                CollectionAssert.DoesNotContain(ids, vencidaId);
                CollectionAssert.DoesNotContain(ids, futuraId);
                BE.Promocion vigente = promociones.Single(item => item.Id == vigenteId);
                Assert.AreEqual(20m, vigente.Descuento);
                Assert.AreEqual(5000m, vigente.TopeDescuento);
                CollectionAssert.AreEqual(new List<BE.DiaSemana> { BE.DiaSemana.Jueves }, vigente.DiasSemana);
                CollectionAssert.AreEqual(new List<int> { comercioId }, vigente.Comercios.ConvertAll(item => item.Id));
                CollectionAssert.AreEqual(new List<int> { rubroId }, vigente.RubrosComercio.ConvertAll(item => item.Id));
                CollectionAssert.AreEqual(new List<int> { entidadId }, vigente.EntidadesFinancieras.ConvertAll(item => item.Id));
                CollectionAssert.AreEqual(new List<int> { medioId }, vigente.MediosPago.ConvertAll(item => item.Id));
                CollectionAssert.AreEqual(new List<string> { "SegmentoTest" }, vigente.Segmentos);
                CollectionAssert.AreEqual(new List<int> { productoIncluidoId }, vigente.ProductosIncluidos.ConvertAll(item => item.Id));
                CollectionAssert.AreEqual(new List<int> { productoExcluidoId }, vigente.ProductosExcluidos.ConvertAll(item => item.Id));
                CollectionAssert.AreEqual(new List<int> { categoriaId }, vigente.CategoriasIncluidas.ConvertAll(item => item.Id));
                Assert.AreEqual(0, vigente.CategoriasExcluidas.Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerCuentasConMediosPorUsuario_SoloDelUsuarioConEntidadYMediosSinCredenciales()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecomendacionComprasRepositorio repositorio = new RecomendacionComprasRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioId(accesoDatos, "user");
                int otroUsuarioId = ObtenerUsuarioId(accesoDatos, "admin");
                int entidadId = new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "BancoTestCuentasRecomendacion" });
                int medioId = new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago { Tipo = BE.TipoMedioPago.TarjetaDebito, Marca = "MarcaTestCuentasRecomendacion" });
                int medioEntidadId = new MedioPagoEntidadRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPagoEntidad
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadId },
                    MedioPago = new BE.MedioPago { Id = medioId },
                    Segmento = "SegmentoTestCuentas"
                });
                CuentaFinancieraRepositorio cuentas = new CuentaFinancieraRepositorio(accesoDatos);
                int cuentaId = InsertarCuenta(cuentas, usuarioId, entidadId, "CuentaTestRecomendacion", "credencial-secreta");
                int cuentaSinMediosId = InsertarCuenta(cuentas, usuarioId, entidadId, "CuentaTestSinMedios", null);
                int cuentaAjenaId = InsertarCuenta(cuentas, otroUsuarioId, entidadId, "CuentaTestAjena", null);
                cuentas.ReemplazarMediosPagoEntidadDeCuenta(cuentas.ObtenerPorId(cuentaId), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioEntidadId) });
                cuentas.ReemplazarMediosPagoEntidadDeCuenta(cuentas.ObtenerPorId(cuentaAjenaId), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioEntidadId) });

                List<BE.CuentaFinanciera> resultado = repositorio.ObtenerCuentasConMediosPorUsuario(new BE.Usuario { Id = usuarioId });

                BE.CuentaFinanciera cuenta = resultado.Single(item => item.Id == cuentaId);
                Assert.AreEqual("CuentaTestRecomendacion", cuenta.Alias);
                Assert.IsNull(cuenta.CredencialesCifradas);
                Assert.AreEqual(entidadId, cuenta.EntidadFinanciera.Id);
                Assert.AreEqual("BancoTestCuentasRecomendacion", cuenta.EntidadFinanciera.Nombre);
                BE.MedioPagoEntidad medio = cuenta.MediosPagoEntidad.Single();
                Assert.AreEqual(medioEntidadId, medio.Id);
                Assert.AreEqual(entidadId, medio.EntidadFinanciera.Id);
                Assert.AreEqual(medioId, medio.MedioPago.Id);
                Assert.AreEqual(BE.TipoMedioPago.TarjetaDebito, medio.MedioPago.Tipo);
                Assert.AreEqual("MarcaTestCuentasRecomendacion", medio.MedioPago.Marca);
                Assert.AreEqual("SegmentoTestCuentas", medio.Segmento);
                Assert.AreEqual(0, resultado.Single(item => item.Id == cuentaSinMediosId).MediosPagoEntidad.Count);
                Assert.IsFalse(resultado.Any(item => item.Id == cuentaAjenaId));
                Assert.IsTrue(resultado.All(item => item.CredencialesCifradas == null));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int ObtenerUsuarioId(AccesoDatosSqlServer accesoDatos, string username)
        {
            return new UsuarioDAL().ObtenerPorUsername(username, accesoDatos).Id;
        }

        private static int InsertarLista(AccesoDatosSqlServer accesoDatos, int usuarioId, string nombre)
        {
            return new ListaDeComprasRepositorio(accesoDatos).InsertarYObtenerId(new BE.ListaDeCompras
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                Nombre = nombre,
                FechaCreacion = DateTime.Now
            });
        }

        private static int InsertarItem(AccesoDatosSqlServer accesoDatos, int listaId, string descripcion, int cantidad, bool comprado)
        {
            return new ItemListaRepositorio(accesoDatos).InsertarYObtenerId(new BE.ItemLista
            {
                ListaDeCompras = new BE.ListaDeCompras { Id = listaId },
                Descripcion = descripcion,
                Cantidad = cantidad,
                Comprado = comprado
            });
        }

        private static int InsertarCategoria(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new CategoriaProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.CategoriaProducto { Nombre = nombre });
        }

        private static int InsertarProducto(AccesoDatosSqlServer accesoDatos, int categoriaId, string nombre)
        {
            return new ProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.Producto
            {
                CategoriaProducto = new BE.CategoriaProducto { Id = categoriaId },
                Nombre = nombre
            });
        }

        private static int InsertarComercio(AccesoDatosSqlServer accesoDatos, int rubroId, string nombre, decimal? latitud, decimal? longitud)
        {
            return new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.Comercio
            {
                RubroComercio = new BE.RubroComercio { Id = rubroId },
                Nombre = nombre,
                Latitud = latitud,
                Longitud = longitud
            });
        }

        private static BE.ComercioProducto CrearOferta(int comercioId, int productoId, int monedaId, decimal precio)
        {
            return new BE.ComercioProducto
            {
                Comercio = new BE.Comercio { Id = comercioId },
                Producto = new BE.Producto { Id = productoId },
                Moneda = new BE.Moneda { Id = monedaId },
                Precio = precio
            };
        }

        private static int InsertarPromocion(AccesoDatosSqlServer accesoDatos, string descripcion, DateTime desde, DateTime hasta)
        {
            return new PromocionRepositorio(accesoDatos).InsertarYObtenerId(new BE.Promocion
            {
                Descripcion = descripcion,
                Descuento = 20m,
                TopeDescuento = 5000m,
                VigenciaDesde = desde,
                VigenciaHasta = hasta
            });
        }

        private static int InsertarCuenta(CuentaFinancieraRepositorio cuentas, int usuarioId, int entidadId, string alias, string credenciales)
        {
            return cuentas.InsertarYObtenerId(new BE.CuentaFinanciera
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadId },
                Alias = alias,
                Tipo = BE.TipoCuentaFinanciera.CajaAhorro,
                CredencialesCifradas = credenciales
            });
        }
    }
}
