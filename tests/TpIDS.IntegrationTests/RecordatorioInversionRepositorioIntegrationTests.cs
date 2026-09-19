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
    public sealed class RecordatorioInversionRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_RecordatorioValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioInversionRepositorio repositorio = new RecordatorioInversionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioInversionTestPropio",
                    Fecha = new System.DateTime(2026, 2, 10),
                    Leida = false
                });

                BE.RecordatorioInversion obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(usuarioId, obtenido.Usuario.Id);
                Assert.AreEqual("RecordatorioInversionTestPropio", obtenido.Mensaje);
                Assert.AreEqual(new System.DateTime(2026, 2, 10), obtenido.Fecha);
                Assert.IsFalse(obtenido.Leida);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioInversionRepositorio repositorio = new RecordatorioInversionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idPropio = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioInversionTestUsuarioA",
                    Fecha = new System.DateTime(2026, 2, 11),
                    Leida = false
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Mensaje = "RecordatorioInversionTestUsuarioB",
                    Fecha = new System.DateTime(2026, 2, 12),
                    Leida = false
                });

                System.Collections.Generic.List<BE.RecordatorioInversion> propios = repositorio.ObtenerPorUsuario(usuarioId);

                Assert.IsTrue(propios.Any(item => item.Id == idPropio));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeDeTodosLosUsuarios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioInversionRepositorio repositorio = new RecordatorioInversionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idPropio = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioInversionTestObtenerTodasA",
                    Fecha = new System.DateTime(2026, 2, 15),
                    Leida = false
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Mensaje = "RecordatorioInversionTestObtenerTodasB",
                    Fecha = new System.DateTime(2026, 2, 16),
                    Leida = false
                });

                System.Collections.Generic.List<BE.RecordatorioInversion> todos = repositorio.ObtenerTodas();

                Assert.IsTrue(todos.Any(item => item.Id == idPropio));
                Assert.IsTrue(todos.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_RecordatorioExistente_PersisteNuevoMensaje()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioInversionRepositorio repositorio = new RecordatorioInversionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioInversionTestOriginal",
                    Fecha = new System.DateTime(2026, 2, 13),
                    Leida = false
                });

                bool actualizado = repositorio.Actualizar(new BE.RecordatorioInversion
                {
                    Id = id,
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioInversionTestModificado",
                    Fecha = new System.DateTime(2026, 2, 13),
                    Leida = true
                });

                Assert.IsTrue(actualizado);
                BE.RecordatorioInversion obtenido = repositorio.ObtenerPorId(id);
                Assert.AreEqual("RecordatorioInversionTestModificado", obtenido.Mensaje);
                Assert.IsTrue(obtenido.Leida);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_RecordatorioExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioInversionRepositorio repositorio = new RecordatorioInversionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioInversionTestAEliminar",
                    Fecha = new System.DateTime(2026, 2, 14),
                    Leida = false
                });

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

        private static int ObtenerUsuarioAdminId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("admin", accesoDatos).Id;
        }
    }
}
