using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion sobre
    // AccesoDatosSqlServer y hace RollbackTransactionScope() al final,
    // asi que ningun test deja datos permanentes en TpCatedraIDS_Test.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class ZonaUsuarioRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ZonaValida_QuedaVisibleConSusDatos()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ZonaUsuarioRepositorio repositorio = new ZonaUsuarioRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int id = repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestCasa", -34.603722m, -58.381592m, 600));

                BE.ZonaUsuario obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual(id, obtenida.Id);
                Assert.IsNotNull(obtenida.Usuario);
                Assert.AreEqual(usuarioId, obtenida.Usuario.Id);
                Assert.AreEqual("ZonaTestCasa", obtenida.Nombre);
                Assert.AreEqual(-34.603722m, obtenida.Latitud);
                Assert.AreEqual(-58.381592m, obtenida.Longitud);
                Assert.AreEqual(600, obtenida.RadioMetros);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorId_ZonaInexistente_DevuelveNull()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ZonaUsuarioRepositorio repositorio = new ZonaUsuarioRepositorio(accesoDatos);

                Assert.IsNull(repositorio.ObtenerPorId(int.MaxValue));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeZonasDeOtroUsuarioYOrdenaPorNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ZonaUsuarioRepositorio repositorio = new ZonaUsuarioRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idTrabajo = repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestTrabajo", -34.6m, -58.4m, 1000));
                int idCasa = repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestCasa", -34.5m, -58.5m, 500));
                int idAjena = repositorio.InsertarYObtenerId(CrearZona(otroUsuarioId, "ZonaTestAjena", -34.7m, -58.3m, 800));

                List<BE.ZonaUsuario> propias = repositorio.ObtenerPorUsuario(usuarioId)
                    .Where(item => item.Nombre.StartsWith("ZonaTest"))
                    .ToList();

                CollectionAssert.AreEqual(new List<int> { idCasa, idTrabajo }, propias.Select(item => item.Id).ToList());
                Assert.IsFalse(repositorio.ObtenerPorUsuario(usuarioId).Any(item => item.Id == idAjena));
                Assert.IsTrue(propias.All(item => item.Usuario.Id == usuarioId));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_ZonaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ZonaUsuarioRepositorio repositorio = new ZonaUsuarioRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestBaja", -34.6m, -58.4m, 1000));

                bool eliminada = repositorio.Eliminar(id);

                Assert.IsTrue(eliminada);
                Assert.IsNull(repositorio.ObtenerPorId(id));
                Assert.IsFalse(repositorio.Eliminar(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_NombreRepetidoDelMismoUsuario_ViolaElUnique()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ZonaUsuarioRepositorio repositorio = new ZonaUsuarioRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestRepetida", -34.6m, -58.4m, 1000));

                ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(
                    () => repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestRepetida", -34.5m, -58.5m, 500)));

                Assert.IsInstanceOfType(ex.InnerException, typeof(System.Data.SqlClient.SqlException));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_MismoNombreParaOtroUsuario_SePermite()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ZonaUsuarioRepositorio repositorio = new ZonaUsuarioRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);
                repositorio.InsertarYObtenerId(CrearZona(usuarioId, "ZonaTestCompartida", -34.6m, -58.4m, 1000));

                int idOtro = repositorio.InsertarYObtenerId(CrearZona(otroUsuarioId, "ZonaTestCompartida", -34.5m, -58.5m, 500));

                Assert.IsTrue(idOtro > 0);
                Assert.IsTrue(repositorio.ObtenerPorUsuario(otroUsuarioId).Any(item => item.Id == idOtro));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static BE.ZonaUsuario CrearZona(int usuarioId, string nombre, decimal latitud, decimal longitud, int radioMetros)
        {
            return new BE.ZonaUsuario
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                Nombre = nombre,
                Latitud = latitud,
                Longitud = longitud,
                RadioMetros = radioMetros
            };
        }

        private static int ObtenerUsuarioBasicoId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;
        }

        private static int ObtenerUsuarioAdminId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("admin", accesoDatos).Id;
        }
    }
}
