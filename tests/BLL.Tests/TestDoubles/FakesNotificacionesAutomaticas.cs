using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using REPOSITORIO;

namespace BLL.Tests.TestDoubles
{
    internal sealed class FakeGeneracionNotificacionesRepositorio : IGeneracionNotificacionesRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, CorridaGeneracionNotificaciones> Corridas { get; } = new Dictionary<int, CorridaGeneracionNotificaciones>();

        public List<DetalleGeneracionNotificacion> Detalles { get; } = new List<DetalleGeneracionNotificacion>();

        public int InsertarCorridaYObtenerId(CorridaGeneracionNotificaciones corrida)
        {
            corrida.Id = _nextId++;
            Corridas[corrida.Id] = corrida;
            return corrida.Id;
        }

        public bool FinalizarCorrida(int corridaId, DateTime finUtc, int entregadas, int omitidas, int fallidas)
        {
            CorridaGeneracionNotificaciones corrida;
            if (!Corridas.TryGetValue(corridaId, out corrida))
            {
                return false;
            }

            corrida.FinUtc = finUtc;
            corrida.Entregadas = entregadas;
            corrida.Omitidas = omitidas;
            corrida.Fallidas = fallidas;
            return true;
        }

        public int InsertarDetalleYObtenerId(DetalleGeneracionNotificacion detalle)
        {
            detalle.Id = Detalles.Count + 1;
            Detalles.Add(detalle);
            return detalle.Id;
        }

        public CorridaGeneracionNotificaciones ObtenerCorrida(int id)
        {
            CorridaGeneracionNotificaciones corrida;
            return Corridas.TryGetValue(id, out corrida) ? corrida : null;
        }

        public List<CorridaGeneracionNotificaciones> ObtenerRecientes(int cantidad)
        {
            return Corridas.Values.OrderByDescending(item => item.InicioUtc).Take(cantidad).ToList();
        }

        public List<DetalleGeneracionNotificacion> ObtenerDetallePorCorrida(int corridaId)
        {
            return Detalles.Where(item => item.CorridaId == corridaId).ToList();
        }
    }
}
