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

                bool reemplazado = repositorio.ReemplazarDiasDePromocion(id, new List<BE.DiaSemana> { BE.DiaSemana.Lunes, BE.DiaSemana.Miercoles });

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

                repositorio.ReemplazarDiasDePromocion(id, new List<BE.DiaSemana> { BE.DiaSemana.Lunes, BE.DiaSemana.Martes });
                repositorio.ReemplazarDiasDePromocion(id, new List<BE.DiaSemana> { BE.DiaSemana.Domingo });

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

                bool eliminada = repositorio.Eliminar(id);

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
                Assert.AreEqual(0, sinFiltros.ComercioIds.Count);
                Assert.AreEqual(0, sinFiltros.RubroComercioIds.Count);
                Assert.AreEqual(0, sinFiltros.EntidadFinancieraIds.Count);
                Assert.AreEqual(0, sinFiltros.MedioPagoIds.Count);
                Assert.AreEqual(0, sinFiltros.Segmentos.Count);
                Assert.AreEqual(0, sinFiltros.ProductoIncluidoIds.Count);
                Assert.AreEqual(0, sinFiltros.ProductoExcluidoIds.Count);
                Assert.AreEqual(0, sinFiltros.CategoriaIncluidaIds.Count);
                Assert.AreEqual(0, sinFiltros.CategoriaExcluidaIds.Count);

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

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() => repositorio.Eliminar(id));

                accesoDatos.RollbackTransactionScope();
            }

            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = InsertarPromocionTest(repositorio, "PromocionTestBajaVaciandoFiltros");
                AsignarFiltros(accesoDatos, repositorio, id, ArmarFiltros(accesoDatos, "BajaVacia"));

                repositorio.ReemplazarDiasDePromocion(id, new List<BE.DiaSemana>());
                new PromocionComercioRepositorio(accesoDatos).ReemplazarComerciosDePromocion(id, new List<int>());
                new PromocionRubroComercioRepositorio(accesoDatos).ReemplazarRubrosComercioDePromocion(id, new List<int>());
                new PromocionEntidadFinancieraRepositorio(accesoDatos).ReemplazarEntidadesFinancierasDePromocion(id, new List<int>());
                new PromocionMedioPagoRepositorio(accesoDatos).ReemplazarMediosPagoDePromocion(id, new List<int>());
                new PromocionSegmentoRepositorio(accesoDatos).ReemplazarSegmentosDePromocion(id, new List<string>());
                new PromocionProductoRepositorio(accesoDatos).ReemplazarProductosDePromocion(id, new List<int>(), new List<int>());
                new PromocionCategoriaProductoRepositorio(accesoDatos).ReemplazarCategoriasProductoDePromocion(id, new List<int>(), new List<int>());

                Assert.IsTrue(repositorio.Eliminar(id));
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
            repositorio.ReemplazarDiasDePromocion(promocionId, new List<BE.DiaSemana> { BE.DiaSemana.Sabado });
            new PromocionComercioRepositorio(accesoDatos).ReemplazarComerciosDePromocion(promocionId, new List<int> { filtros.ComercioId });
            new PromocionRubroComercioRepositorio(accesoDatos).ReemplazarRubrosComercioDePromocion(promocionId, new List<int> { filtros.RubroComercioId });
            new PromocionEntidadFinancieraRepositorio(accesoDatos).ReemplazarEntidadesFinancierasDePromocion(promocionId, new List<int> { filtros.EntidadFinancieraId });
            new PromocionMedioPagoRepositorio(accesoDatos).ReemplazarMediosPagoDePromocion(promocionId, new List<int> { filtros.MedioPagoId });
            new PromocionSegmentoRepositorio(accesoDatos).ReemplazarSegmentosDePromocion(promocionId, new List<string> { filtros.Segmento });
            new PromocionProductoRepositorio(accesoDatos).ReemplazarProductosDePromocion(
                promocionId, new List<int> { filtros.ProductoIncluidoId }, new List<int> { filtros.ProductoExcluidoId });
            new PromocionCategoriaProductoRepositorio(accesoDatos).ReemplazarCategoriasProductoDePromocion(
                promocionId, new List<int> { filtros.CategoriaIncluidaId }, new List<int> { filtros.CategoriaExcluidaId });
        }

        private static void AssertFiltros(FiltrosDeTest filtros, BE.Promocion promocion)
        {
            Assert.IsNotNull(promocion);
            CollectionAssert.AreEqual(new List<BE.DiaSemana> { BE.DiaSemana.Sabado }, promocion.DiasSemana);
            CollectionAssert.AreEqual(new List<int> { filtros.ComercioId }, promocion.ComercioIds);
            CollectionAssert.AreEqual(new List<int> { filtros.RubroComercioId }, promocion.RubroComercioIds);
            CollectionAssert.AreEqual(new List<int> { filtros.EntidadFinancieraId }, promocion.EntidadFinancieraIds);
            CollectionAssert.AreEqual(new List<int> { filtros.MedioPagoId }, promocion.MedioPagoIds);
            CollectionAssert.AreEqual(new List<string> { filtros.Segmento }, promocion.Segmentos);
            CollectionAssert.AreEqual(new List<int> { filtros.ProductoIncluidoId }, promocion.ProductoIncluidoIds);
            CollectionAssert.AreEqual(new List<int> { filtros.ProductoExcluidoId }, promocion.ProductoExcluidoIds);
            CollectionAssert.AreEqual(new List<int> { filtros.CategoriaIncluidaId }, promocion.CategoriaIncluidaIds);
            CollectionAssert.AreEqual(new List<int> { filtros.CategoriaExcluidaId }, promocion.CategoriaExcluidaIds);
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
