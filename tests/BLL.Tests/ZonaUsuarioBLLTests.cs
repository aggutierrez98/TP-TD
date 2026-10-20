using System.Collections.Generic;
using System.Linq;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ZonaUsuarioBLLTests
    {
        private const int UsuarioActualId = 7;
        private const int OtroUsuarioId = 8;

        // Un grado de latitud sobre la esfera de DistanciaGeografica, en metros.
        private const decimal MetrosPorGradoDeLatitud = 111194.93m;

        [TestMethod]
        public void ObtenerMisZonas_DevuelveSoloLasDelUsuarioActual()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "Casa", -34.6m, -58.4m, 1000);
            fixture.AgregarZona(2, OtroUsuarioId, "Casa", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            List<ZonaUsuario> resultado = sut.ObtenerMisZonas();

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(1, resultado[0].Id);
        }

        [TestMethod]
        public void Crear_Valida_InsertaParaElUsuarioActualAuditaYGuarda()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ZonaUsuario creada = sut.Crear("  Trabajo  ", -34.6037221m, -58.3815924m, 600);

            ZonaUsuario insertada = fixture.ZonaRepo.Zonas[creada.Id];
            Assert.AreEqual(UsuarioActualId, insertada.Usuario.Id);
            Assert.AreEqual("Trabajo", insertada.Nombre);
            Assert.AreEqual(-34.603722m, insertada.Latitud);
            Assert.AreEqual(-58.381592m, insertada.Longitud);
            Assert.AreEqual(600, insertada.RadioMetros);
            Assert.AreEqual(1, fixture.Bitacora.Calls.Count);
            Assert.AreEqual("FormMyZones.Value.Activity.Creation", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("   ", -34.6m, -58.4m, 1000));

            Assert.AreEqual("Error.MyZones.NameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_NombreDeMasDe50Caracteres_ThrowsExcepcionNegocio()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new string('a', 51), -34.6m, -58.4m, 1000));

            Assert.AreEqual("Error.MyZones.NameTooLong", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_NombreDe50Caracteres_SePermite()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ZonaUsuario creada = sut.Crear(new string('a', 50), -34.6m, -58.4m, 1000);

            Assert.IsTrue(fixture.ZonaRepo.Zonas.ContainsKey(creada.Id));
        }

        [TestMethod]
        public void Crear_NombreRepetidoSinDistinguirMayusculas_ThrowsExcepcionNegocioYNoGuarda()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "Trabajo", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(" TRABAJO ", -34.5m, -58.5m, 500));

            Assert.AreEqual("Error.MyZones.DuplicateName", ex.Message);
            Assert.AreEqual(1, fixture.ZonaRepo.Zonas.Count);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_MismoNombreQueUnaZonaDeOtroUsuario_SePermite()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, OtroUsuarioId, "Trabajo", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ZonaUsuario creada = sut.Crear("Trabajo", -34.5m, -58.5m, 500);

            Assert.AreEqual(UsuarioActualId, fixture.ZonaRepo.Zonas[creada.Id].Usuario.Id);
        }

        [TestMethod]
        [DataRow(99)]
        [DataRow(20001)]
        [DataRow(0)]
        [DataRow(-1000)]
        public void Crear_RadioFueraDeRango_ThrowsExcepcionNegocioAndDoesNotCreateUow(int radioMetros)
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Casa", -34.6m, -58.4m, radioMetros));

            Assert.AreEqual("Error.MyZones.InvalidRadius", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        [DataRow(100)]
        [DataRow(20000)]
        public void Crear_RadioEnLosLimites_SePermite(int radioMetros)
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ZonaUsuario creada = sut.Crear("Casa", -34.6m, -58.4m, radioMetros);

            Assert.AreEqual(radioMetros, fixture.ZonaRepo.Zonas[creada.Id].RadioMetros);
        }

        [TestMethod]
        [DataRow(90.000001, 0d)]
        [DataRow(-90.000001, 0d)]
        [DataRow(0d, 180.000001)]
        [DataRow(0d, -180.000001)]
        public void Crear_CoordenadasFueraDeRango_ThrowsExcepcionNegocioAndDoesNotCreateUow(double latitud, double longitud)
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Casa", (decimal)latitud, (decimal)longitud, 1000));

            Assert.AreEqual("Error.MyZones.InvalidCoordinates", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_SinPuntoElegido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Casa", null, null, 1000));

            Assert.AreEqual("Error.MyZones.PointRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_ConElTopeDeZonasAlcanzado_ThrowsExcepcionNegocioYNoGuarda()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            for (int i = 1; i <= ZonaUsuarioBLL.MaximoZonasPorUsuario; i++)
            {
                fixture.AgregarZona(i, UsuarioActualId, "Zona" + i, -34.6m, -58.4m, 1000);
            }

            fixture.AgregarZona(100, OtroUsuarioId, "Ajena", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Otra", -34.6m, -58.4m, 1000));

            Assert.AreEqual("Error.MyZones.LimitReached", ex.Message);
            Assert.AreEqual(11, fixture.ZonaRepo.Zonas.Count);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_ConUnaZonaMenosQueElTope_SePermite()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            for (int i = 1; i < ZonaUsuarioBLL.MaximoZonasPorUsuario; i++)
            {
                fixture.AgregarZona(i, UsuarioActualId, "Zona" + i, -34.6m, -58.4m, 1000);
            }

            ZonaUsuarioBLL sut = fixture.CreateSut();

            sut.Crear("Ultima", -34.6m, -58.4m, 1000);

            Assert.AreEqual(10, ZonaUsuarioBLL.MaximoZonasPorUsuario);
            Assert.AreEqual(ZonaUsuarioBLL.MaximoZonasPorUsuario, fixture.ZonaRepo.ObtenerPorUsuario(UsuarioActualId).Count);
        }

        [TestMethod]
        public void Crear_FallaAlGuardar_ThrowsExcepcionTecnica()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.Unidad.ThrowOnSave = true;
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Crear("Casa", -34.6m, -58.4m, 1000));

            Assert.AreEqual("Error.MyZones.CreateFailed", ex.Message);
        }

        [TestMethod]
        public void Eliminar_ZonaPropia_EliminaAuditaYGuarda()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "Casa", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            bool eliminada = sut.Eliminar(1);

            Assert.IsTrue(eliminada);
            Assert.IsFalse(fixture.ZonaRepo.Zonas.ContainsKey(1));
            Assert.AreEqual("FormMyZones.Value.Activity.Deletion", fixture.Bitacora.Calls.Single().Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ZonaDeOtroUsuario_ThrowsExcepcionNegocioYNoLaElimina()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, OtroUsuarioId, "Casa", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MyZones.NotOwned", ex.Message);
            Assert.IsTrue(fixture.ZonaRepo.Zonas.ContainsKey(1));
            Assert.IsNull(fixture.ZonaRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ZonaInexistente_ThrowsExcepcionNegocio()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            ZonaUsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));

            Assert.AreEqual("Error.MyZones.NotFound", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void CalcularDistanciasAMisZonas_SoloZonasPropiasOrdenadasPorDistancia()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "Lejos", -35.0m, -58.4m, 1000);
            fixture.AgregarZona(2, UsuarioActualId, "Cerca", -34.61m, -58.4m, 1000);
            fixture.AgregarZona(3, OtroUsuarioId, "AjenaEnElPunto", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            List<DistanciaZonaUsuario> resultado = sut.CalcularDistanciasAMisZonas(-34.6m, -58.4m);

            CollectionAssert.AreEqual(new List<int> { 2, 1 }, resultado.Select(item => item.Zona.Id).ToList());
            Assert.AreEqual(1111.9, resultado[0].DistanciaMetros, 1);
            Assert.IsFalse(resultado[0].EstaDentroDelRadio);
        }

        [TestMethod]
        public void ObtenerZonaMasCercana_PuntoA600MetrosDeTrabajoConRadio1000_InformaQueEstaDentro()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "Trabajo", -34.6m, -58.4m, 1000);
            fixture.AgregarZona(2, UsuarioActualId, "Casa", -34.7m, -58.5m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();
            decimal latitudA600Metros = -34.6m + (600m / MetrosPorGradoDeLatitud);

            DistanciaZonaUsuario resultado = sut.ObtenerZonaMasCercana(latitudA600Metros, -58.4m);

            Assert.AreEqual("Trabajo", resultado.Zona.Nombre);
            Assert.AreEqual(600, resultado.DistanciaMetros, 1);
            Assert.IsTrue(resultado.EstaDentroDelRadio);
            Assert.AreEqual("Trabajo", sut.ObtenerZonaQueContiene(latitudA600Metros, -58.4m).Zona.Nombre);
        }

        [TestMethod]
        public void ObtenerZonaMasCercana_PuntoFueraDeTodas_InformaQueNoEstaDentroDeNinguna()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "Trabajo", -34.6m, -58.4m, 1000);
            fixture.AgregarZona(2, UsuarioActualId, "Casa", -34.7m, -58.5m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            DistanciaZonaUsuario masCercana = sut.ObtenerZonaMasCercana(-34.65m, -58.4m);

            Assert.AreEqual("Trabajo", masCercana.Zona.Nombre);
            Assert.IsFalse(masCercana.EstaDentroDelRadio);
            Assert.IsNull(sut.ObtenerZonaQueContiene(-34.65m, -58.4m));
        }

        [TestMethod]
        public void ObtenerZonaQueContiene_LaMasCercanaNoLoContienePeroOtraSi_DevuelveLaQueLoContiene()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, UsuarioActualId, "ChicaCercana", -34.6m, -58.4m, 100);
            fixture.AgregarZona(2, UsuarioActualId, "GrandeLejana", -34.61m, -58.4m, 5000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            DistanciaZonaUsuario resultado = sut.ObtenerZonaQueContiene(-34.599m, -58.4m);

            Assert.AreEqual("ChicaCercana", sut.ObtenerZonaMasCercana(-34.599m, -58.4m).Zona.Nombre);
            Assert.AreEqual("GrandeLejana", resultado.Zona.Nombre);
            Assert.IsTrue(resultado.EstaDentroDelRadio);
        }

        [TestMethod]
        public void ObtenerZonaMasCercana_SinZonas_DevuelveNull()
        {
            ZonaUsuarioFixture fixture = new ZonaUsuarioFixture();
            fixture.AgregarZona(1, OtroUsuarioId, "Ajena", -34.6m, -58.4m, 1000);
            ZonaUsuarioBLL sut = fixture.CreateSut();

            Assert.IsNull(sut.ObtenerZonaMasCercana(-34.6m, -58.4m));
            Assert.IsNull(sut.ObtenerZonaQueContiene(-34.6m, -58.4m));
        }

        [TestMethod]
        public void CalcularDistancias_PuntoEnElBordeDelRadio_CuentaComoDentro()
        {
            ZonaUsuario zona = new ZonaUsuario { Id = 1, Nombre = "Borde", Latitud = 0m, Longitud = 0m, RadioMetros = 1000 };
            decimal latitudEnElBorde = 999.9m / MetrosPorGradoDeLatitud;

            List<DistanciaZonaUsuario> resultado = ZonaUsuarioBLL.CalcularDistancias(new List<ZonaUsuario> { zona }, latitudEnElBorde, 0m);

            Assert.IsTrue(resultado.Single().EstaDentroDelRadio);
        }

        private sealed class ZonaUsuarioFixture
        {
            public readonly FakeZonaUsuarioRepositorio ZonaRepo = new FakeZonaUsuarioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public ZonaUsuarioFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { ZonaUsuarioRepo = ZonaRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = UsuarioActualId, Username = "test" });
            }

            public void AgregarZona(int id, int usuarioId, string nombre, decimal latitud, decimal longitud, int radioMetros)
            {
                ZonaRepo.Zonas.Add(id, new ZonaUsuario
                {
                    Id = id,
                    Usuario = new Usuario { Id = usuarioId },
                    Nombre = nombre,
                    Latitud = latitud,
                    Longitud = longitud,
                    RadioMetros = radioMetros
                });
            }

            public ZonaUsuarioBLL CreateSut()
            {
                return new ZonaUsuarioBLL(ZonaRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
