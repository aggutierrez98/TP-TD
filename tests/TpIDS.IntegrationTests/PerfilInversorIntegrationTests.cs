using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE;
using BLL;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class PerfilInversorIntegrationTests
    {
        [TestMethod]
        public async Task Guardar_Concurrentes_SerializaPorPropietarioSinMezclarRespuestas()
        {
            int usuario;
            using (var acceso = new AccesoDatosSqlServer())
                usuario = new UsuarioDAL().ObtenerPorUsername("user", acceso).Id;
            try
            {
                using (var primero = new UnidadDeTrabajoFactory().Crear())
                using (var iniciado = new ManualResetEventSlim())
                {
                    primero.PerfilesInversor.Guardar(CrearPerfil(usuario, "AAAAAAAAAA"));
                    var segundo = Task.Run(() =>
                    {
                        using (var unidad = new UnidadDeTrabajoFactory().Crear())
                        {
                            iniciado.Set();
                            unidad.PerfilesInversor.Guardar(CrearPerfil(usuario, "BCCCCCCCCB"));
                            unidad.Save();
                        }
                    });
                    Assert.IsTrue(iniciado.Wait(TimeSpan.FromSeconds(5)));
                    primero.Save();
                    await segundo;
                }
                var final = new PerfilInversorRepositorio().ObtenerPorUsuario(usuario);
                Assert.AreEqual("BCCCCCCCCB", string.Concat(final.Respuestas.Select(r => r.OpcionId)));
                Assert.AreEqual(64.16m, final.Puntaje);
                Assert.AreEqual(NivelRiesgoInversor.Moderado, final.NivelRiesgo);
            }
            finally { new PerfilInversorRepositorio().Guardar(CrearPerfil(usuario, "CDCDCCDDDC")); }
        }

        [TestMethod]
        public void Guardar_ErrorEnRespuestas_RevierteCabeceraYRespuestasAnteriores()
        {
            int usuario;
            using (var acceso = new AccesoDatosSqlServer())
                usuario = new UsuarioDAL().ObtenerPorUsername("user", acceso).Id;
            using (var unidad = new UnidadDeTrabajoFactory().Crear())
            {
                var invalido = CrearPerfil(usuario, "AAAAAAAAAA");
                invalido.Respuestas[9].PreguntaId = 1;
                Assert.ThrowsExactly<ExcepcionTecnica>(() => unidad.PerfilesInversor.Guardar(invalido));
            }
            var final = new PerfilInversorRepositorio().ObtenerPorUsuario(usuario);
            Assert.AreEqual(100m, final.Puntaje);
            Assert.AreEqual("CDCDCCDDDC", string.Concat(final.Respuestas.Select(r => r.OpcionId)));
        }
        [TestMethod]
        public void Guardar_AltaReemplazoYAislamiento_PersisteDiezRespuestasSinMezcla()
        {
            using (var acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repo = new PerfilInversorRepositorio(acceso);
                int admin = new UsuarioDAL().ObtenerPorUsername("admin", acceso).Id;
                int user = new UsuarioDAL().ObtenerPorUsername("user", acceso).Id;
                Assert.IsNull(repo.ObtenerPorUsuario(admin));
                var alto = CrearPerfil(admin, "CDCDCCDDDC");
                repo.Guardar(alto);
                var leido = repo.ObtenerPorUsuario(admin);
                Assert.AreEqual(admin, leido.UsuarioId);
                Assert.AreEqual(NivelRiesgoInversor.Alto, leido.NivelRiesgo);
                Assert.AreEqual(10, leido.Respuestas.Count);
                var bajo = CrearPerfil(admin, "AAAAAAAAAA");
                repo.Guardar(bajo);
                var nuevo = repo.ObtenerPorUsuario(admin);
                Assert.AreEqual(leido.Id, nuevo.Id);
                Assert.AreEqual(NivelRiesgoInversor.Bajo, nuevo.NivelRiesgo);
                Assert.IsTrue(nuevo.Respuestas.All(r => r.OpcionId == "A"));
                Assert.AreEqual(10, nuevo.Respuestas.Count);
                Assert.Contains("NoAceptaPerdidas", nuevo.MotivosLimite);
                Assert.AreEqual(NivelRiesgoInversor.Alto, repo.ObtenerPorUsuario(user).NivelRiesgo);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Guardar_SinConfirmarUnidad_RevierteElReemplazoCompleto()
        {
            int user;
            using (var acceso = new AccesoDatosSqlServer()) user = new UsuarioDAL().ObtenerPorUsername("user", acceso).Id;
            using (var unidad = new UnidadDeTrabajoFactory().Crear())
                unidad.PerfilesInversor.Guardar(CrearPerfil(user, "AAAAAAAAAA"));
            var original = new PerfilInversorRepositorio().ObtenerPorUsuario(user);
            Assert.AreEqual(NivelRiesgoInversor.Alto, original.NivelRiesgo);
            Assert.AreEqual("CDCDCCDDDC", string.Concat(original.Respuestas.Select(r => r.OpcionId)));
        }

        private static PerfilInversor CrearPerfil(int usuarioId, string opciones)
        {
            var perfil = CalculadorPerfilInversor.Evaluar(1, opciones.Select((o, i) =>
                new RespuestaPerfilInversor { PreguntaId = i + 1, OpcionId = o.ToString() }));
            perfil.UsuarioId = usuarioId;
            perfil.FechaEvaluacionUtc = DateTime.UtcNow;
            return perfil;
        }

        [TestMethod]
        public void ScriptSeed_User_TienePerfilAltoCompletoYRecalculable()
        {
            using (var acceso = new AccesoDatosSqlServer())
            {
                var datos = acceso.Read(@"SELECT p.*, r.PreguntaId, r.OpcionId FROM PerfilInversor p
                    JOIN Usuarios u ON p.UsuarioId=u.Id JOIN PerfilInversorRespuesta r ON r.PerfilInversorId=p.Id
                    WHERE u.Username='user' ORDER BY r.PreguntaId", null);
                Assert.AreEqual(10, datos.Rows.Count);
                Assert.AreEqual(1, datos.Rows.Cast<DataRow>().Select(r => (int)r["Id"]).Distinct().Count());
                Assert.AreEqual("CDCDCCDDDC", string.Concat(datos.Rows.Cast<DataRow>().Select(r => r["OpcionId"].ToString())));
                Assert.IsTrue(datos.Rows.Cast<DataRow>().All(r => (decimal)r["Capacidad"] == 100m
                    && (decimal)r["Tolerancia"] == 100m && (decimal)r["Puntaje"] == 100m
                    && (int)r["NivelRiesgo"] == 2 && (int)r["VersionCuestionario"] == 1));
                Assert.AreEqual("[]", datos.Rows[0]["MotivosLimite"]);
                var recalculado = CalculadorPerfilInversor.Evaluar(1, datos.Rows.Cast<DataRow>().Select(r =>
                    new RespuestaPerfilInversor { PreguntaId = (int)r["PreguntaId"], OpcionId = r["OpcionId"].ToString() }));
                Assert.AreEqual((decimal)datos.Rows[0]["Puntaje"], recalculado.Puntaje);
                Assert.AreEqual((NivelRiesgoInversor)(int)datos.Rows[0]["NivelRiesgo"], recalculado.NivelRiesgo);
                Assert.AreEqual(0, acceso.Read(@"SELECT p.Id FROM PerfilInversor p JOIN Usuarios u ON u.Id=p.UsuarioId WHERE u.Username<>'user'", null).Rows.Count);
            }
        }

        [TestMethod]
        public void ScriptSchema_UnicidadYForeignKey_RechazaPropietariosInvalidos()
        {
            using (var acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                Assert.ThrowsExactly<ExcepcionTecnica>(() => acceso.Write(@"INSERT INTO PerfilInversor
                    (UsuarioId, VersionCuestionario, FechaEvaluacionUtc, Capacidad, Tolerancia, Puntaje, NivelRiesgo, MotivosLimite)
                    SELECT UsuarioId,1,GETUTCDATE(),100,100,100,2,'[]' FROM PerfilInversor", null));
                acceso.RollbackTransactionScope();
            }
            using (var acceso = new AccesoDatosSqlServer())
            {
                Assert.ThrowsExactly<ExcepcionTecnica>(() => acceso.Write(@"INSERT INTO PerfilInversor
                    (UsuarioId, VersionCuestionario, FechaEvaluacionUtc, Capacidad, Tolerancia, Puntaje, NivelRiesgo, MotivosLimite)
                    VALUES (2147483647,1,GETUTCDATE(),100,100,100,2,'[]')", null));
            }
        }
    }
}
