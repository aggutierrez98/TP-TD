using System;
using System.Linq;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class NotificacionBLLTests
    {
        [TestMethod]
        public void ObtenerTodas_MezclaLosTresSubtiposOrdenadosPorFechaDesc()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.NotificacionRepo.Notificaciones.Add(1, new Notificacion { Id = 1, Usuario = new Usuario { Id = 1 }, Mensaje = "N1", Fecha = new DateTime(2026, 1, 1) });
            fixture.RecordatorioCompraRepo.Recordatorios.Add(2, new RecordatorioCompra { Id = 2, Usuario = new Usuario { Id = 1 }, Mensaje = "RC1", Fecha = new DateTime(2026, 1, 3) });
            fixture.RecordatorioInversionRepo.Recordatorios.Add(3, new RecordatorioInversion { Id = 3, Usuario = new Usuario { Id = 1 }, Mensaje = "RI1", Fecha = new DateTime(2026, 1, 2) });
            NotificacionBLL sut = fixture.CreateSut();

            var resultado = sut.ObtenerTodas();

            Assert.AreEqual(3, resultado.Count);
            CollectionAssert.AreEqual(new[] { 2, 3, 1 }, resultado.Select(item => item.Id).ToList());
        }

        [TestMethod]
        public void CrearManual_Valido_InsertaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.Unidad.UsuarioRepo.Usuarios.Add(5, new Usuario { Id = 5, Username = "destino" });
            NotificacionBLL sut = fixture.CreateSut();

            Notificacion resultado = sut.CrearManual(5, "  Hola usuario  ");

            Notificacion insertada = fixture.NotificacionRepo.Notificaciones[resultado.Id];
            Assert.AreEqual("Hola usuario", insertada.Mensaje);
            Assert.AreEqual(5, insertada.Usuario.Id);
            Assert.IsFalse(insertada.Leida);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void CrearManual_UsuarioInexistente_ThrowsExcepcionNegocio()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            NotificacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearManual(999, "Hola"));
        }

        [TestMethod]
        public void CrearManual_MensajeVacio_ThrowsExcepcionNegocio()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.Unidad.UsuarioRepo.Usuarios.Add(5, new Usuario { Id = 5, Username = "destino" });
            NotificacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearManual(5, "   "));
        }

        [TestMethod]
        public void MarcarLeida_Notificacion_ActualizaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.NotificacionRepo.Notificaciones.Add(1, new Notificacion { Id = 1, Usuario = new Usuario { Id = 1 }, Mensaje = "N1", Fecha = DateTime.Now, Leida = false });
            NotificacionBLL sut = fixture.CreateSut();

            bool resultado = sut.MarcarLeida(1, true);

            Assert.IsTrue(resultado);
            Assert.IsTrue(fixture.NotificacionRepo.Notificaciones[1].Leida);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void MarcarLeida_RecordatorioCompra_ActualizaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.RecordatorioCompraRepo.Recordatorios.Add(2, new RecordatorioCompra { Id = 2, Usuario = new Usuario { Id = 1 }, Mensaje = "RC1", Fecha = DateTime.Now, Leida = false });
            NotificacionBLL sut = fixture.CreateSut();

            bool resultado = sut.MarcarLeida(2, true);

            Assert.IsTrue(resultado);
            Assert.IsTrue(fixture.RecordatorioCompraRepo.Recordatorios[2].Leida);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void MarcarLeida_RecordatorioInversion_ActualizaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.RecordatorioInversionRepo.Recordatorios.Add(3, new RecordatorioInversion { Id = 3, Usuario = new Usuario { Id = 1 }, Mensaje = "RI1", Fecha = DateTime.Now, Leida = false });
            NotificacionBLL sut = fixture.CreateSut();

            bool resultado = sut.MarcarLeida(3, true);

            Assert.IsTrue(resultado);
            Assert.IsTrue(fixture.RecordatorioInversionRepo.Recordatorios[3].Leida);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void MarcarLeida_Inexistente_ThrowsExcepcionNegocio()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            NotificacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.MarcarLeida(999, true));
        }

        [TestMethod]
        public void Eliminar_Notificacion_EliminaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.NotificacionRepo.Notificaciones.Add(1, new Notificacion { Id = 1, Usuario = new Usuario { Id = 1 }, Mensaje = "N1", Fecha = DateTime.Now });
            NotificacionBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(1);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.NotificacionRepo.Notificaciones.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_RecordatorioCompra_EliminaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.RecordatorioCompraRepo.Recordatorios.Add(2, new RecordatorioCompra { Id = 2, Usuario = new Usuario { Id = 1 }, Mensaje = "RC1", Fecha = DateTime.Now });
            NotificacionBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(2);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.RecordatorioCompraRepo.Recordatorios.ContainsKey(2));
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_RecordatorioInversion_EliminaAuditaYGuarda()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            fixture.RecordatorioInversionRepo.Recordatorios.Add(3, new RecordatorioInversion { Id = 3, Usuario = new Usuario { Id = 1 }, Mensaje = "RI1", Fecha = DateTime.Now });
            NotificacionBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(3);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.RecordatorioInversionRepo.Recordatorios.ContainsKey(3));
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            NotificacionFixture fixture = new NotificacionFixture();
            NotificacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(999));
        }

        private sealed class NotificacionFixture
        {
            public readonly FakeNotificacionRepositorio NotificacionRepo = new FakeNotificacionRepositorio();
            public readonly FakeRecordatorioCompraRepositorio RecordatorioCompraRepo = new FakeRecordatorioCompraRepositorio();
            public readonly FakeRecordatorioInversionRepositorio RecordatorioInversionRepo = new FakeRecordatorioInversionRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public NotificacionFixture()
            {
                Unidad = new FakeUnidadDeTrabajo
                {
                    NotificacionRepo = NotificacionRepo,
                    RecordatorioCompraRepo = RecordatorioCompraRepo,
                    RecordatorioInversionRepo = RecordatorioInversionRepo
                };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public NotificacionBLL CreateSut()
            {
                return new NotificacionBLL(NotificacionRepo, RecordatorioCompraRepo, RecordatorioInversionRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
