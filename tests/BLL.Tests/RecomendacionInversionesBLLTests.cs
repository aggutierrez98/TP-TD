using System;
using System.Collections.Generic;
using BE;
using BLL;
using BLL.Tests.TestDoubles;
using SERVICIOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class RecomendacionInversionesBLLTests
    {
        [TestMethod]
        public void CalcularSaldo_TresMesesCompletos_IncluyeMesVacioComoCeroYUsuarioCapturado()
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            repo.ObtenerTotalesPorMesResult = new List<TotalPorMes>
            {
                new TotalPorMes { Anio = 2026, Mes = 7, TotalIngresos = 1000, TotalEgresos = 400 },
                new TotalPorMes { Anio = 2026, Mes = 9, TotalIngresos = 2000, TotalEgresos = 800 }
            };
            ResumenInversiones r = new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => new DateTime(2026, 10, 19)).CalcularSaldo(new Usuario { Id = 7 }, 20);
            Assert.AreEqual(Tuple.Create(7, new DateTime(2026, 7, 1), new DateTime(2026, 9, 30)), Tuple.Create(repo.LastObtenerTotalesPorMesArgs.Item1.Id, repo.LastObtenerTotalesPorMesArgs.Item2, repo.LastObtenerTotalesPorMesArgs.Item3));
            Assert.AreEqual(600m, r.SaldoMensual);
            Assert.AreEqual(400m, r.GastoMensual);
            Assert.AreEqual(120m, r.Monto);
            Assert.AreEqual("ARS", r.Moneda);
        }

        [TestMethod]
        [DataRow(1000, 1001, "Negativo")]
        [DataRow(1000, 1000, "Negativo")]
        [DataRow(0, 0, "Negativo")]
        [DataRow(1000, 901, "Ajustado")]
        [DataRow(1000, 900, "Comodo")]
        [DataRow(1000, 750, "Comodo")]
        [DataRow(1000, 749, "Alto")]
        public void CalcularSaldo_BordesDeNivel_DecideEnCodigo(int ingreso, int gasto, string esperado)
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            repo.ObtenerTotalesPorMesResult.Add(new TotalPorMes { Anio = 2026, Mes = 12, TotalIngresos = ingreso * 3, TotalEgresos = gasto * 3 });
            ResumenInversiones r = new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => new DateTime(2027, 1, 2)).CalcularSaldo(new Usuario { Id = 8 }, 100);
            Assert.AreEqual(esperado, r.Nivel.ToString());
            Assert.AreEqual(Math.Max(0, ingreso - gasto), r.Monto);
            Assert.AreEqual(new DateTime(2026, 10, 1), repo.LastObtenerTotalesPorMesArgs.Item2);
        }


        [TestMethod]
        public void CalcularSaldo_SinPorcentaje_ConservaNivelYNoInventaPorcentajeNiMonto()
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            repo.ObtenerTotalesPorMesResult.Add(new TotalPorMes { Anio = 2026, Mes = 9, TotalIngresos = 3000, TotalEgresos = 1500 });
            ResumenInversiones r = new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => new DateTime(2026, 10, 19)).CalcularSaldo(new Usuario { Id = 7 }, null);
            Assert.AreEqual(500m, r.SaldoMensual);
            Assert.AreEqual(NivelInversion.Alto, r.Nivel);
            Assert.IsNull(r.Porcentaje);
            Assert.IsNull(r.Monto);
            Assert.AreEqual(Tuple.Create(7, new DateTime(2026, 7, 1), new DateTime(2026, 9, 30)), Tuple.Create(repo.LastObtenerTotalesPorMesArgs.Item1.Id, repo.LastObtenerTotalesPorMesArgs.Item2, repo.LastObtenerTotalesPorMesArgs.Item3));
        }

        [TestMethod]
        public void CalcularSaldo_PorcentajeCien_UsaTodoElSaldoPositivo()
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            repo.ObtenerTotalesPorMesResult.Add(new TotalPorMes { Anio = 2026, Mes = 9, TotalIngresos = 3000, TotalEgresos = 1500 });
            ResumenInversiones r = new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => new DateTime(2026, 10, 19)).CalcularSaldo(new Usuario { Id = 7 }, 100m);
            Assert.AreEqual(100m, r.Porcentaje);
            Assert.AreEqual(500m, r.Monto);
        }

        [TestMethod]
        public void CalcularSaldo_SaldoNegativoConPorcentaje_MontoEsCeroReal()
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            repo.ObtenerTotalesPorMesResult.Add(new TotalPorMes { Anio = 2026, Mes = 9, TotalIngresos = 300, TotalEgresos = 900 });
            ResumenInversiones r = new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => new DateTime(2026, 10, 19)).CalcularSaldo(new Usuario { Id = 7 }, 20m);
            Assert.AreEqual(20m, r.Porcentaje);
            Assert.AreEqual(0m, r.Monto);
        }

        [TestMethod]
        public void Contexto_SinPorcentaje_NoSerializaPorcentajeNiMonto()
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            repo.ObtenerTotalesPorMesResult.Add(new TotalPorMes { Anio = 2026, Mes = 9, TotalIngresos = 3000, TotalEgresos = 1500 });
            var sut = new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => new DateTime(2026, 10, 19));
            string sin = sut.CalcularSaldo(new Usuario { Id = 7 }, null).CrearContexto().DatosJson;
            Assert.DoesNotContain("Porcentaje", sin);
            Assert.DoesNotContain("Monto", sin);
            string con = sut.CalcularSaldo(new Usuario { Id = 7 }, 20m).CrearContexto().DatosJson;
            Assert.Contains("\"Porcentaje\":20", con);
            Assert.Contains("\"Monto\":100", con);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(101)]
        [DataRow(150)]
        public void CalcularSaldo_PorcentajeInvalido_NoConsultaMovimientos(int porcentaje)
        {
            FakeMovimientoRepositorio repo = new FakeMovimientoRepositorio();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RecomendacionInversionesBLL(repo, Usuarios(7, 8), () => DateTime.Now).CalcularSaldo(new Usuario { Id = 7 }, porcentaje));
            Assert.IsNull(repo.LastObtenerTotalesPorMesArgs);
        }

        private static FakeUsuarioRepositorio Usuarios(params int[] ids)
        {
            FakeUsuarioRepositorio usuarios = new FakeUsuarioRepositorio();
            foreach (int id in ids)
            {
                usuarios.Usuarios.Add(id, new BE.Usuario { Id = id });
            }

            return usuarios;
        }
    }
}
