using System;
using System.Linq;
using ABSTRACCIONES;
using BE;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class MovimientoRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_MovimientoValido_QuedaPersistido()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new Movimiento
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new Categoria { Id = categoriaId },
                    Tipo = TipoMovimiento.Egreso,
                    Monto = 150.75m,
                    Fecha = new DateTime(2026, 8, 1),
                    Descripcion = "Compra de test"
                });

                Movimiento obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(usuarioId, obtenido.Usuario.Id);
                Assert.AreEqual(categoriaId, obtenido.Categoria.Id);
                Assert.AreEqual(TipoMovimiento.Egreso, obtenido.Tipo);
                Assert.AreEqual(150.75m, obtenido.Monto);
                Assert.AreEqual(new DateTime(2026, 8, 1), obtenido.Fecha);
                Assert.AreEqual("Compra de test", obtenido.Descripcion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_ConComercio_QuedaPersistidoYHidratado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);
                int rubroId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new RubroComercio { Nombre = "RubroTestMovComercio" });
                int comercioId = new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new Comercio { RubroComercio = new RubroComercio { Id = rubroId }, Nombre = "ComercioTestMov" });

                int id = repositorio.InsertarYObtenerId(new Movimiento
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new Categoria { Id = categoriaId },
                    Comercio = new Comercio { Id = comercioId },
                    Tipo = TipoMovimiento.Egreso,
                    Monto = 320.00m,
                    Fecha = new DateTime(2026, 8, 2),
                    Descripcion = "Compra con comercio"
                });

                Movimiento obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.IsNotNull(obtenido.Comercio);
                Assert.AreEqual(comercioId, obtenido.Comercio.Id);
                Assert.AreEqual("ComercioTestMov", obtenido.Comercio.Nombre);

                // Actualizar a null (quitar comercio opcional)
                obtenido.Comercio = null;
                bool actualizado = repositorio.Actualizar(obtenido);
                Assert.IsTrue(actualizado);

                Movimiento sinComercio = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(sinComercio);
                Assert.IsNull(sinComercio.Comercio);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_FiltraPorRangoDeFechasCategoriaYTipo()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaA = ObtenerCategoriaGlobalId(accesoDatos);
                int categoriaB = new CategoriaRepositorio(accesoDatos).InsertarYObtenerId(new Categoria { Nombre = "CategoriaTestMovB", Usuario = new BE.Usuario { Id = usuarioId } });

                int idDentro = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Ingreso, Monto = 100m, Fecha = new DateTime(2026, 8, 10) });
                int idFueraDeRango = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Ingreso, Monto = 100m, Fecha = new DateTime(2026, 1, 1) });
                int idOtraCategoria = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaB }, Tipo = TipoMovimiento.Ingreso, Monto = 100m, Fecha = new DateTime(2026, 8, 11) });
                int idOtroTipo = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Egreso, Monto = 100m, Fecha = new DateTime(2026, 8, 12) });

                System.Collections.Generic.List<Movimiento> resultado = repositorio.ObtenerPorUsuario(usuarioId, new FiltroMovimiento
                {
                    FechaDesde = new DateTime(2026, 8, 1),
                    FechaHasta = new DateTime(2026, 8, 31),
                    CategoriaId = categoriaA,
                    Tipo = TipoMovimiento.Ingreso
                });

                Assert.IsTrue(resultado.Any(item => item.Id == idDentro));
                Assert.IsFalse(resultado.Any(item => item.Id == idFueraDeRango));
                Assert.IsFalse(resultado.Any(item => item.Id == idOtraCategoria));
                Assert.IsFalse(resultado.Any(item => item.Id == idOtroTipo));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_FiltraPorTextoEnDescripcion()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);

                int idCoincide = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Egreso, Monto = 100m, Fecha = new DateTime(2026, 8, 13), Descripcion = "Compra en Supermercado Coto" });
                int idNoCoincide = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Egreso, Monto = 100m, Fecha = new DateTime(2026, 8, 14), Descripcion = "Pago de luz" });

                System.Collections.Generic.List<Movimiento> resultado = repositorio.ObtenerPorUsuario(usuarioId, new FiltroMovimiento
                {
                    Texto = "coto"
                });

                Assert.IsTrue(resultado.Any(item => item.Id == idCoincide));
                Assert.IsFalse(resultado.Any(item => item.Id == idNoCoincide));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTotalesPorCategoria_AgrupaPorCategoriaYFiltraPorTipoYRango()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaA = new CategoriaRepositorio(accesoDatos).InsertarYObtenerId(new Categoria { Nombre = "CategoriaTestTotalesA", Usuario = new BE.Usuario { Id = usuarioId } });
                int categoriaB = new CategoriaRepositorio(accesoDatos).InsertarYObtenerId(new Categoria { Nombre = "CategoriaTestTotalesB", Usuario = new BE.Usuario { Id = usuarioId } });

                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Egreso, Monto = 100m, Fecha = new DateTime(2026, 8, 5) });
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Egreso, Monto = 50m, Fecha = new DateTime(2026, 8, 6) });
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaB }, Tipo = TipoMovimiento.Egreso, Monto = 30m, Fecha = new DateTime(2026, 8, 7) });
                // Fuera de rango: no debe sumar
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Egreso, Monto = 999m, Fecha = new DateTime(2026, 1, 1) });
                // Otro tipo: no debe sumar
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaA }, Tipo = TipoMovimiento.Ingreso, Monto = 777m, Fecha = new DateTime(2026, 8, 8) });

                System.Collections.Generic.List<SERVICIOS.TotalPorCategoria> resultado = repositorio.ObtenerTotalesPorCategoria(
                    usuarioId, TipoMovimiento.Egreso, new DateTime(2026, 8, 1), new DateTime(2026, 8, 31));

                SERVICIOS.TotalPorCategoria totalA = resultado.Single(item => item.CategoriaNombre == "CategoriaTestTotalesA");
                SERVICIOS.TotalPorCategoria totalB = resultado.Single(item => item.CategoriaNombre == "CategoriaTestTotalesB");
                Assert.AreEqual(150m, totalA.Total);
                Assert.AreEqual(30m, totalB.Total);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTotalesPorMes_AgrupaPorAnioMesYSeparaIngresosDeEgresos()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);

                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Ingreso, Monto = 1000m, Fecha = new DateTime(2026, 8, 1) });
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Egreso, Monto = 400m, Fecha = new DateTime(2026, 8, 2) });
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Ingreso, Monto = 500m, Fecha = new DateTime(2026, 9, 1) });

                System.Collections.Generic.List<SERVICIOS.TotalPorMes> resultado = repositorio.ObtenerTotalesPorMes(
                    usuarioId, new DateTime(2026, 8, 1), new DateTime(2026, 9, 30));

                SERVICIOS.TotalPorMes agosto = resultado.Single(item => item.Anio == 2026 && item.Mes == 8);
                SERVICIOS.TotalPorMes septiembre = resultado.Single(item => item.Anio == 2026 && item.Mes == 9);
                Assert.AreEqual(1000m, agosto.TotalIngresos);
                Assert.AreEqual(400m, agosto.TotalEgresos);
                Assert.AreEqual(500m, septiembre.TotalIngresos);
                Assert.AreEqual(0m, septiembre.TotalEgresos);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerResumen_SumaTotalesSinAgrupar()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);

                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Ingreso, Monto = 1000m, Fecha = new DateTime(2026, 8, 1) });
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Ingreso, Monto = 200m, Fecha = new DateTime(2026, 8, 10) });
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Egreso, Monto = 300m, Fecha = new DateTime(2026, 8, 15) });
                // Fuera de rango: no debe sumar
                repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Ingreso, Monto = 5000m, Fecha = new DateTime(2026, 1, 1) });

                SERVICIOS.ResumenMovimientos resumen = repositorio.ObtenerResumen(usuarioId, new DateTime(2026, 8, 1), new DateTime(2026, 8, 31));

                Assert.AreEqual(1200m, resumen.TotalIngresos);
                Assert.AreEqual(300m, resumen.TotalEgresos);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_MovimientoExistente_PersisteCambios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Ingreso, Monto = 50m, Fecha = new DateTime(2026, 8, 5) });

                Movimiento actualizado = repositorio.ObtenerPorId(id);
                actualizado.Monto = 999.99m;
                actualizado.Descripcion = "Actualizado";
                bool resultado = repositorio.Actualizar(actualizado);

                Assert.IsTrue(resultado);
                Movimiento releido = repositorio.ObtenerPorId(id);
                Assert.AreEqual(999.99m, releido.Monto);
                Assert.AreEqual("Actualizado", releido.Descripcion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_MovimientoExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new Movimiento { Usuario = new BE.Usuario { Id = usuarioId }, Categoria = new Categoria { Id = categoriaId }, Tipo = TipoMovimiento.Egreso, Monto = 20m, Fecha = new DateTime(2026, 8, 6) });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int ObtenerUsuarioBasicoId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;
        }

        private static int ObtenerCategoriaGlobalId(AccesoDatosSqlServer accesoDatos)
        {
            return new CategoriaDAL().ObtenerVisiblesParaUsuario(0, accesoDatos).First(item => item.EsGlobal).Id;
        }
    }
}
