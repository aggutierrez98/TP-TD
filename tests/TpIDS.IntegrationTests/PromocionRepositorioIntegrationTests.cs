using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion sobre
    // AccesoDatosSqlServer y hace RollbackTransactionScope() al final,
    // asi que ningun test deja datos permanentes en TpCatedraIDS_Test.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class PromocionRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_PromocionValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestPropia",
                    Descuento = 15m,
                    VigenciaDesde = new DateTime(2026, 1, 1),
                    VigenciaHasta = new DateTime(2026, 1, 31)
                });

                BE.Promocion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual("PromocionTestPropia", obtenida.Descripcion);
                Assert.AreEqual(15m, obtenida.Descuento);
                Assert.AreEqual(new DateTime(2026, 1, 1), obtenida.VigenciaDesde);
                Assert.AreEqual(new DateTime(2026, 1, 31), obtenida.VigenciaHasta);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeInsertada()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestListado",
                    Descuento = 20m,
                    VigenciaDesde = new DateTime(2026, 2, 1),
                    VigenciaHasta = new DateTime(2026, 2, 28)
                });

                System.Collections.Generic.List<BE.Promocion> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_PromocionExistente_PersisteNuevoDescuento()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestOriginal",
                    Descuento = 10m,
                    VigenciaDesde = new DateTime(2026, 3, 1),
                    VigenciaHasta = new DateTime(2026, 3, 31)
                });

                bool actualizada = repositorio.Actualizar(new BE.Promocion
                {
                    Id = id,
                    Descripcion = "PromocionTestOriginal",
                    Descuento = 30m,
                    VigenciaDesde = new DateTime(2026, 3, 1),
                    VigenciaHasta = new DateTime(2026, 3, 31)
                });

                Assert.IsTrue(actualizada);
                Assert.AreEqual(30m, repositorio.ObtenerPorId(id).Descuento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_ConTopeDescuento_PersisteElTope()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestConTope",
                    Descuento = 10m,
                    TopeDescuento = 5000m,
                    VigenciaDesde = new DateTime(2026, 5, 1),
                    VigenciaHasta = new DateTime(2026, 5, 31)
                });

                Assert.AreEqual(5000m, repositorio.ObtenerPorId(id).TopeDescuento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_SinTopeNiDias_DevuelveTopeNuloYDiasVacios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestSinTope",
                    Descuento = 10m,
                    VigenciaDesde = new DateTime(2026, 6, 1),
                    VigenciaHasta = new DateTime(2026, 6, 30)
                });

                BE.Promocion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNull(obtenida.TopeDescuento);
                Assert.IsNotNull(obtenida.DiasSemana);
                Assert.AreEqual(0, obtenida.DiasSemana.Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_PromocionExistente_PersisteNuevoTope()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestTopeOriginal",
                    Descuento = 10m,
                    VigenciaDesde = new DateTime(2026, 7, 1),
                    VigenciaHasta = new DateTime(2026, 7, 31)
                });

                repositorio.Actualizar(new BE.Promocion
                {
                    Id = id,
                    Descripcion = "PromocionTestTopeOriginal",
                    Descuento = 10m,
                    TopeDescuento = 1500.50m,
                    VigenciaDesde = new DateTime(2026, 7, 1),
                    VigenciaHasta = new DateTime(2026, 7, 31)
                });

                Assert.AreEqual(1500.50m, repositorio.ObtenerPorId(id).TopeDescuento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarDiasDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestDias");

                bool reemplazado = repositorio.ReemplazarDiasDePromocion(repositorio.ObtenerPorId(id), new List<BE.DiaSemana> { BE.DiaSemana.Lunes, BE.DiaSemana.Miercoles });

                Assert.IsTrue(reemplazado);
                List<BE.DiaSemana> dias = repositorio.ObtenerPorId(id).DiasSemana;
                Assert.AreEqual(2, dias.Count);
                Assert.IsTrue(dias.Contains(BE.DiaSemana.Lunes));
                Assert.IsTrue(dias.Contains(BE.DiaSemana.Miercoles));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarDiasDePromocion_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestDiasReemplazo");

                repositorio.ReemplazarDiasDePromocion(repositorio.ObtenerPorId(id), new List<BE.DiaSemana> { BE.DiaSemana.Lunes, BE.DiaSemana.Martes });
                repositorio.ReemplazarDiasDePromocion(repositorio.ObtenerPorId(id), new List<BE.DiaSemana> { BE.DiaSemana.Domingo });

                List<BE.DiaSemana> dias = repositorio.ObtenerTodas().Single(item => item.Id == id).DiasSemana;
                Assert.AreEqual(1, dias.Count);
                Assert.AreEqual(BE.DiaSemana.Domingo, dias[0]);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_PromocionExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestAEliminar",
                    Descuento = 25m,
                    VigenciaDesde = new DateTime(2026, 4, 1),
                    VigenciaHasta = new DateTime(2026, 4, 30)
                });

                bool eliminada = repositorio.Eliminar(repositorio.ObtenerPorId(id));

                Assert.IsTrue(eliminada);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorId_ConTodosLosFiltros_LosCarga()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestTodosLosFiltros");
                FiltrosDeTest filtros = ArmarFiltros(accesoDatos, "Carga");
                AsignarFiltros(accesoDatos, repositorio, id, filtros);

                BE.Promocion obtenida = repositorio.ObtenerPorId(id);

                AssertFiltros(filtros, obtenida);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_ConFiltros_LosCargaEnCadaPromocion()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestListadoConFiltros");
                int idSinFiltros = InsertarPromocionTest(repositorio, "PromocionTestListadoSinFiltros");
                FiltrosDeTest filtros = ArmarFiltros(accesoDatos, "Listado");
                AsignarFiltros(accesoDatos, repositorio, id, filtros);

                List<BE.Promocion> todas = repositorio.ObtenerTodas();

                AssertFiltros(filtros, todas.First(item => item.Id == id));
                BE.Promocion sinFiltros = todas.First(item => item.Id == idSinFiltros);
                Assert.AreEqual(0, sinFiltros.Comercios.Count);
                Assert.AreEqual(0, sinFiltros.RubrosComercio.Count);
                Assert.AreEqual(0, sinFiltros.EntidadesFinancieras.Count);
                Assert.AreEqual(0, sinFiltros.MediosPago.Count);
                Assert.AreEqual(0, sinFiltros.Segmentos.Count);
                Assert.AreEqual(0, sinFiltros.ProductosIncluidos.Count);
                Assert.AreEqual(0, sinFiltros.ProductosExcluidos.Count);
                Assert.AreEqual(0, sinFiltros.CategoriasIncluidas.Count);
                Assert.AreEqual(0, sinFiltros.CategoriasExcluidas.Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        // La baja de una promocion no borra sola sus filtros (las FK no tienen ON DELETE): el BLL
        // los vacia primero y despues elimina la promocion.
        [TestMethod]
        public void Eliminar_PromocionConFiltros_FallaHastaVaciarlosYDespuesSeElimina()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestBajaConFiltros");
                AsignarFiltros(accesoDatos, repositorio, id, ArmarFiltros(accesoDatos, "Baja"));

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() => repositorio.Eliminar(repositorio.ObtenerPorId(id)));

                accesoDatos.RollbackTransactionScope();
            }

            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestBajaVaciandoFiltros");
                AsignarFiltros(accesoDatos, repositorio, id, ArmarFiltros(accesoDatos, "BajaVacia"));

                BE.Promocion promocion = repositorio.ObtenerPorId(id);
                repositorio.ReemplazarDiasDePromocion(promocion, new List<BE.DiaSemana>());
                new PromocionComercioRepositorio(accesoDatos).ReemplazarComerciosDePromocion(promocion, new List<BE.Comercio>());
                new PromocionRubroComercioRepositorio(accesoDatos).ReemplazarRubrosComercioDePromocion(promocion, new List<BE.RubroComercio>());
                new PromocionEntidadFinancieraRepositorio(accesoDatos).ReemplazarEntidadesFinancierasDePromocion(promocion, new List<BE.EntidadFinanciera>());
                new PromocionMedioPagoRepositorio(accesoDatos).ReemplazarMediosPagoDePromocion(promocion, new List<BE.MedioPago>());
                new PromocionSegmentoRepositorio(accesoDatos).ReemplazarSegmentosDePromocion(promocion, new List<string>());
                new PromocionProductoRepositorio(accesoDatos).ReemplazarProductosDePromocion(promocion, new List<BE.Producto>(), new List<BE.Producto>());
                new PromocionCategoriaProductoRepositorio(accesoDatos).ReemplazarCategoriasProductoDePromocion(promocion, new List<BE.CategoriaProducto>(), new List<BE.CategoriaProducto>());

                Assert.IsTrue(repositorio.Eliminar(promocion));
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private sealed class FiltrosDeTest
        {
            public int ComercioId { get; set; }
            public int RubroComercioId { get; set; }
            public int EntidadFinancieraId { get; set; }
            public int MedioPagoId { get; set; }
            public string Segmento { get; set; }
            public int ProductoIncluidoId { get; set; }
            public int ProductoExcluidoId { get; set; }
            public int CategoriaIncluidaId { get; set; }
            public int CategoriaExcluidaId { get; set; }
        }

        private static FiltrosDeTest ArmarFiltros(AccesoDatosSqlServer accesoDatos, string sufijo)
        {
            int rubroId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "RubroTestFiltros" + sufijo });
            int comercioId = new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.Comercio
            {
                RubroComercio = new BE.RubroComercio { Id = rubroId },
                Nombre = "ComercioTestFiltros" + sufijo
            });
            int entidadId = new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "EntidadTestFiltros" + sufijo });
            int medioPagoId = new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago
            {
                Tipo = BE.TipoMedioPago.TarjetaCredito,
                Marca = "MarcaTestFiltros" + sufijo
            });
            CategoriaProductoRepositorio categorias = new CategoriaProductoRepositorio(accesoDatos);
            int categoriaIncluidaId = categorias.InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "CatIncTestFiltros" + sufijo });
            int categoriaExcluidaId = categorias.InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "CatExcTestFiltros" + sufijo });
            ProductoRepositorio productos = new ProductoRepositorio(accesoDatos);
            int productoIncluidoId = productos.InsertarYObtenerId(new BE.Producto
            {
                CategoriaProducto = new BE.CategoriaProducto { Id = categoriaIncluidaId },
                Nombre = "ProdIncTestFiltros" + sufijo
            });
            int productoExcluidoId = productos.InsertarYObtenerId(new BE.Producto
            {
                CategoriaProducto = new BE.CategoriaProducto { Id = categoriaExcluidaId },
                Nombre = "ProdExcTestFiltros" + sufijo
            });

            return new FiltrosDeTest
            {
                ComercioId = comercioId,
                RubroComercioId = rubroId,
                EntidadFinancieraId = entidadId,
                MedioPagoId = medioPagoId,
                Segmento = "Seg" + sufijo,
                ProductoIncluidoId = productoIncluidoId,
                ProductoExcluidoId = productoExcluidoId,
                CategoriaIncluidaId = categoriaIncluidaId,
                CategoriaExcluidaId = categoriaExcluidaId
            };
        }

        private static void AsignarFiltros(AccesoDatosSqlServer accesoDatos, PromocionRepositorio repositorio, int promocionId, FiltrosDeTest filtros)
        {
            BE.Promocion promocion = repositorio.ObtenerPorId(promocionId);
            repositorio.ReemplazarDiasDePromocion(promocion, new List<BE.DiaSemana> { BE.DiaSemana.Sabado });
            new PromocionComercioRepositorio(accesoDatos).ReemplazarComerciosDePromocion(promocion, new List<BE.Comercio> { new BE.Comercio { Id = filtros.ComercioId } });
            new PromocionRubroComercioRepositorio(accesoDatos).ReemplazarRubrosComercioDePromocion(promocion, new List<BE.RubroComercio> { new BE.RubroComercio { Id = filtros.RubroComercioId } });
            new PromocionEntidadFinancieraRepositorio(accesoDatos).ReemplazarEntidadesFinancierasDePromocion(promocion, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = filtros.EntidadFinancieraId } });
            new PromocionMedioPagoRepositorio(accesoDatos).ReemplazarMediosPagoDePromocion(promocion, new List<BE.MedioPago> { new BE.MedioPago { Id = filtros.MedioPagoId } });
            new PromocionSegmentoRepositorio(accesoDatos).ReemplazarSegmentosDePromocion(promocion, new List<string> { filtros.Segmento });
            new PromocionProductoRepositorio(accesoDatos).ReemplazarProductosDePromocion(
                promocion, new List<BE.Producto> { new BE.Producto { Id = filtros.ProductoIncluidoId } }, new List<BE.Producto> { new BE.Producto { Id = filtros.ProductoExcluidoId } });
            new PromocionCategoriaProductoRepositorio(accesoDatos).ReemplazarCategoriasProductoDePromocion(
                promocion, new List<BE.CategoriaProducto> { new BE.CategoriaProducto { Id = filtros.CategoriaIncluidaId } }, new List<BE.CategoriaProducto> { new BE.CategoriaProducto { Id = filtros.CategoriaExcluidaId } });
        }

        private static void AssertFiltros(FiltrosDeTest filtros, BE.Promocion promocion)
        {
            Assert.IsNotNull(promocion);
            CollectionAssert.AreEqual(new List<BE.DiaSemana> { BE.DiaSemana.Sabado }, promocion.DiasSemana);
            CollectionAssert.AreEqual(new List<int> { filtros.ComercioId }, promocion.Comercios.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<int> { filtros.RubroComercioId }, promocion.RubrosComercio.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<int> { filtros.EntidadFinancieraId }, promocion.EntidadesFinancieras.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<int> { filtros.MedioPagoId }, promocion.MediosPago.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<string> { filtros.Segmento }, promocion.Segmentos);
            CollectionAssert.AreEqual(new List<int> { filtros.ProductoIncluidoId }, promocion.ProductosIncluidos.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<int> { filtros.ProductoExcluidoId }, promocion.ProductosExcluidos.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<int> { filtros.CategoriaIncluidaId }, promocion.CategoriasIncluidas.ConvertAll(item => item.Id));
            CollectionAssert.AreEqual(new List<int> { filtros.CategoriaExcluidaId }, promocion.CategoriasExcluidas.ConvertAll(item => item.Id));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.Comercios[0].Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.Comercios[0].RubroComercio.Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.RubrosComercio[0].Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.EntidadesFinancieras[0].Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.MediosPago[0].Marca));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.ProductosIncluidos[0].Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.ProductosIncluidos[0].CategoriaProducto.Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.ProductosExcluidos[0].Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.ProductosExcluidos[0].CategoriaProducto.Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.CategoriasIncluidas[0].Nombre));
            Assert.IsFalse(string.IsNullOrWhiteSpace(promocion.CategoriasExcluidas[0].Nombre));
        }

        private static int InsertarPromocionTest(PromocionRepositorio repositorio, string descripcion)
        {
            return repositorio.InsertarYObtenerId(new BE.Promocion
            {
                Descripcion = descripcion,
                Descuento = 10m,
                VigenciaDesde = new DateTime(2026, 8, 1),
                VigenciaHasta = new DateTime(2026, 8, 31)
            });
        }
    }
}
