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
                    UsuarioId = usuarioId,
                    CategoriaId = categoriaId,
                    Tipo = TipoMovimiento.Egreso,
                    Monto = 150.75m,
                    Fecha = new DateTime(2026, 8, 1),
                    Descripcion = "Compra de test"
                });

                Movimiento obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(usuarioId, obtenido.UsuarioId);
                Assert.AreEqual(categoriaId, obtenido.CategoriaId);
                Assert.AreEqual(TipoMovimiento.Egreso, obtenido.Tipo);
                Assert.AreEqual(150.75m, obtenido.Monto);
                Assert.AreEqual(new DateTime(2026, 8, 1), obtenido.Fecha);
                Assert.AreEqual("Compra de test", obtenido.Descripcion);

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
                int categoriaB = new CategoriaRepositorio(accesoDatos).InsertarYObtenerId(new Categoria { Nombre = "CategoriaTestMovB", UsuarioId = usuarioId });

                int idDentro = repositorio.InsertarYObtenerId(new Movimiento { UsuarioId = usuarioId, CategoriaId = categoriaA, Tipo = TipoMovimiento.Ingreso, Monto = 100m, Fecha = new DateTime(2026, 8, 10) });
                int idFueraDeRango = repositorio.InsertarYObtenerId(new Movimiento { UsuarioId = usuarioId, CategoriaId = categoriaA, Tipo = TipoMovimiento.Ingreso, Monto = 100m, Fecha = new DateTime(2026, 1, 1) });
                int idOtraCategoria = repositorio.InsertarYObtenerId(new Movimiento { UsuarioId = usuarioId, CategoriaId = categoriaB, Tipo = TipoMovimiento.Ingreso, Monto = 100m, Fecha = new DateTime(2026, 8, 11) });
                int idOtroTipo = repositorio.InsertarYObtenerId(new Movimiento { UsuarioId = usuarioId, CategoriaId = categoriaA, Tipo = TipoMovimiento.Egreso, Monto = 100m, Fecha = new DateTime(2026, 8, 12) });

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
        public void Actualizar_MovimientoExistente_PersisteCambios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MovimientoRepositorio repositorio = new MovimientoRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = ObtenerCategoriaGlobalId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new Movimiento { UsuarioId = usuarioId, CategoriaId = categoriaId, Tipo = TipoMovimiento.Ingreso, Monto = 50m, Fecha = new DateTime(2026, 8, 5) });

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
                int id = repositorio.InsertarYObtenerId(new Movimiento { UsuarioId = usuarioId, CategoriaId = categoriaId, Tipo = TipoMovimiento.Egreso, Monto = 20m, Fecha = new DateTime(2026, 8, 6) });

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
