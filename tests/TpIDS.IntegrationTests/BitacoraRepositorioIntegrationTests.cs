using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class BitacoraRepositorioIntegrationTests
    {
        // BitacoraDAL.Buscar abre siempre su propia conexion (no acepta la conexion
        // transaccionada del caller), asi que un rollback no seria visible para la
        // lectura: este test escribe y lee sin BeginTransactionScope, igual que el
        // resto de la bitacora, que ya es de solo-crecimiento en toda la suite.
        [TestMethod]
        public void Buscar_FiltraPorRangoDeFechas()
        {
            BitacoraRepositorio repositorio = new BitacoraRepositorio();
            string actividad = "IntegrationTest.FiltroFecha." + Guid.NewGuid().ToString("N");
            int usuarioId = new UsuarioDAL().ObtenerPorUsername("admin").Id;
            DateTime fechaAntigua = new DateTime(2020, 1, 1);
            DateTime fechaReciente = DateTime.Now;

            repositorio.Guardar(new EventoBitacora
            {
                FechaHora = fechaAntigua,
                UsuarioId = usuarioId,
                UsernameSnapshot = "admin",
                Actividad = actividad,
                Informacion = "antiguo",
                Nivel = "Informacion",
                Origen = "BitacoraRepositorioIntegrationTests",
                EsTecnica = false
            });
            repositorio.Guardar(new EventoBitacora
            {
                FechaHora = fechaReciente,
                UsuarioId = usuarioId,
                UsernameSnapshot = "admin",
                Actividad = actividad,
                Informacion = "reciente",
                Nivel = "Informacion",
                Origen = "BitacoraRepositorioIntegrationTests",
                EsTecnica = false
            });

            List<EventoBitacora> soloRecientes = repositorio.Buscar(new FiltroBitacora
            {
                Actividad = actividad,
                FechaDesde = fechaReciente.AddMinutes(-1)
            });
            List<EventoBitacora> soloAntiguos = repositorio.Buscar(new FiltroBitacora
            {
                Actividad = actividad,
                FechaHasta = fechaAntigua.AddDays(1)
            });

            Assert.IsTrue(soloRecientes.Any(item => item.Informacion == "reciente"), "El filtro FechaDesde deberia incluir el evento reciente.");
            Assert.IsFalse(soloRecientes.Any(item => item.Informacion == "antiguo"), "El filtro FechaDesde no deberia incluir el evento antiguo.");
            Assert.IsTrue(soloAntiguos.Any(item => item.Informacion == "antiguo"), "El filtro FechaHasta deberia incluir el evento antiguo.");
            Assert.IsFalse(soloAntiguos.Any(item => item.Informacion == "reciente"), "El filtro FechaHasta no deberia incluir el evento reciente.");
        }
    }
}
