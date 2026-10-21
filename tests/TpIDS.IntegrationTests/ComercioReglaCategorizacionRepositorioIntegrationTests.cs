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
    public sealed class ComercioReglaCategorizacionRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarComerciosDeReglaCategorizacion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioReglaCategorizacionRepositorio repositorio = new ComercioReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestAsignacionComercios");
                int reglaCategorizacionId = InsertarReglaCategorizacionTest(accesoDatos, usuarioId, categoriaId, "ReglaTestAsignacionComercios");
                int comercioId1 = InsertarComercioTest(accesoDatos, "ComercioTestReglaAsignacion1");
                int comercioId2 = InsertarComercioTest(accesoDatos, "ComercioTestReglaAsignacion2");

                bool reemplazado = repositorio.ReemplazarComerciosDeReglaCategorizacion(Regla(reglaCategorizacionId), Comercios(comercioId1, comercioId2));

                Assert.IsTrue(reemplazado);
                List<int> asignados = repositorio.ObtenerComerciosPorReglaCategorizacion(Regla(reglaCategorizacionId)).Select(item => item.Id).ToList();
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains(comercioId1));
                Assert.IsTrue(asignados.Contains(comercioId2));
                Assert.IsTrue(repositorio.ObtenerComerciosPorReglaCategorizacion(Regla(reglaCategorizacionId)).All(item => item.Nombre.StartsWith("ComercioTestReglaAsignacion")));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarComerciosDeReglaCategorizacion_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioReglaCategorizacionRepositorio repositorio = new ComercioReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestReemplazoComercios");
                int reglaCategorizacionId = InsertarReglaCategorizacionTest(accesoDatos, usuarioId, categoriaId, "ReglaTestReemplazoComercios");
                int comercioIdA = InsertarComercioTest(accesoDatos, "ComercioTestReglaReemplazoA");
                int comercioIdB = InsertarComercioTest(accesoDatos, "ComercioTestReglaReemplazoB");

                repositorio.ReemplazarComerciosDeReglaCategorizacion(Regla(reglaCategorizacionId), Comercios(comercioIdA));
                repositorio.ReemplazarComerciosDeReglaCategorizacion(Regla(reglaCategorizacionId), Comercios(comercioIdB));

                List<int> asignados = repositorio.ObtenerComerciosPorReglaCategorizacion(Regla(reglaCategorizacionId)).Select(item => item.Id).ToList();
                Assert.AreEqual(1, asignados.Count);
                Assert.IsTrue(asignados.Contains(comercioIdB));
                Assert.IsFalse(asignados.Contains(comercioIdA));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorComercio_CuentaLasReglasQueLoReferencian()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioReglaCategorizacionRepositorio repositorio = new ComercioReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestContarComercio");
                int reglaAId = InsertarReglaCategorizacionTest(accesoDatos, usuarioId, categoriaId, "ReglaTestContarComercioA");
                int reglaBId = InsertarReglaCategorizacionTest(accesoDatos, usuarioId, categoriaId, "ReglaTestContarComercioB");
                int comercioUsadoId = InsertarComercioTest(accesoDatos, "ComercioTestContarReglaUsado");
                int comercioSinUsoId = InsertarComercioTest(accesoDatos, "ComercioTestContarReglaLibre");
                repositorio.ReemplazarComerciosDeReglaCategorizacion(Regla(reglaAId), Comercios(comercioUsadoId));
                repositorio.ReemplazarComerciosDeReglaCategorizacion(Regla(reglaBId), Comercios(comercioUsadoId));

                Assert.AreEqual(2, repositorio.ContarPorComercio(new BE.Comercio { Id = comercioUsadoId }));
                Assert.AreEqual(0, repositorio.ContarPorComercio(new BE.Comercio { Id = comercioSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static BE.ReglaCategorizacion Regla(int id)
        {
            return new BE.ReglaCategorizacion { Id = id };
        }

        private static List<BE.Comercio> Comercios(params int[] ids)
        {
            return ids.Select(id => new BE.Comercio { Id = id }).ToList();
        }

        private static int ObtenerUsuarioBasicoId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;
        }

        private static int InsertarCategoriaTest(AccesoDatosSqlServer accesoDatos, int usuarioId, string nombre)
        {
            return new CategoriaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Categoria { Nombre = nombre, Usuario = new BE.Usuario { Id = usuarioId } });
        }

        private static int InsertarReglaCategorizacionTest(AccesoDatosSqlServer accesoDatos, int usuarioId, int categoriaId, string nombre)
        {
            return new ReglaCategorizacionRepositorio(accesoDatos).InsertarYObtenerId(new BE.ReglaCategorizacion
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                Categoria = new BE.Categoria { Id = categoriaId },
                Nombre = nombre,
                PalabrasClave = "clave"
            });
        }

        private static int InsertarComercioTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            int rubroComercioId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "Rubro-" + nombre });
            return new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.Comercio { RubroComercio = new BE.RubroComercio { Id = rubroComercioId }, Nombre = nombre });
        }
    }
}
