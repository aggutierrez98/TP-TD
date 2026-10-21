using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using SERVICIOS;

namespace BLL
{
    internal static class CalculadorPerfilInversor
    {
        internal const int Version = 1;
        internal static readonly int[] CantidadesOpciones = { 3, 4, 3, 4, 3, 3, 4, 4, 4, 3 };
        internal static readonly IReadOnlyList<PreguntaPerfilInversor> Preguntas =
            Enumerable.Range(1, CantidadesOpciones.Length).Select(numero => new PreguntaPerfilInversor(numero)).ToList().AsReadOnly();

        internal static PerfilInversor Evaluar(int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            var copia = respuestas?.ToArray();
            if (version != Version || copia == null || copia.Length != 10
                || copia.Any(r => r == null || r.Pregunta == null || r.Pregunta.Id < 1 || r.Pregunta.Id > 10
                    || r.OpcionId == null || r.OpcionId.Length != 1 || r.OpcionId[0] < 'A'
                    || r.OpcionId[0] >= 'A' + CantidadesOpciones[r.Pregunta.Id - 1])
                || copia.Select(r => r.Pregunta.Id).Distinct().Count() != 10)
                throw new ExcepcionNegocio("Error.InvestorProfile.InvalidAnswers");

            var ordenadas = copia.OrderBy(r => r.Pregunta.Id)
                .Select(r => new RespuestaPerfilInversor { Pregunta = r.Pregunta, OpcionId = r.OpcionId }).ToList();
            int[] opciones = ordenadas.Select(r => r.OpcionId[0] - 'A').ToArray();
            decimal[] puntos = opciones.Select((o, i) => CantidadesOpciones[i] == 3
                ? new[] { 0m, 50m, 100m }[o] : new[] { 0m, 33m, 67m, 100m }[o]).ToArray();
            decimal c = (puntos[1] + puntos[2] + puntos[3] + puntos[4] + puntos[5]) / 5m;
            decimal[] tolerancia = { puntos[0], puntos[7], puntos[8], puntos[9] };
            decimal t = tolerancia.Average();
            decimal p = .80m * Math.Min(c, t) + .20m * Math.Max(c, t);
            var perfil = new PerfilInversor { VersionCuestionario = Version, Capacidad = c, Tolerancia = t,
                Puntaje = p, NivelRiesgo = ClasificarPuntaje(p), Respuestas = ordenadas };
            Limitar(perfil, opciones[1] == 0, NivelRiesgoInversor.Bajo, "HorizonteMuyCorto");
            Limitar(perfil, opciones[2] == 0, NivelRiesgoInversor.Bajo, "LiquidezInmediata");
            Limitar(perfil, opciones[3] == 0, NivelRiesgoInversor.Bajo, "RespaldoInsuficiente");
            Limitar(perfil, opciones[4] == 0, NivelRiesgoInversor.Bajo, "GastosNoCubiertos");
            Limitar(perfil, opciones[5] == 0, NivelRiesgoInversor.Bajo, "GastosComprometidos");
            Limitar(perfil, opciones[8] == 0, NivelRiesgoInversor.Bajo, "NoAceptaPerdidas");
            Limitar(perfil, opciones[1] == 1 || opciones[1] == 2, NivelRiesgoInversor.Moderado, "HorizonteLimitado");
            Limitar(perfil, opciones[2] == 1, NivelRiesgoInversor.Moderado, "LiquidezProxima");
            Limitar(perfil, opciones[3] == 1, NivelRiesgoInversor.Moderado, "RespaldoLimitado");
            Limitar(perfil, opciones[4] == 1 || opciones[5] == 1, NivelRiesgoInversor.Moderado, "CapacidadLimitada");
            Limitar(perfil, opciones[6] <= 1, NivelRiesgoInversor.Moderado, "ExperienciaLimitada");
            Limitar(perfil, tolerancia.Max() - tolerancia.Min() >= 67m, NivelRiesgoInversor.Moderado, "RespuestasDispares");
            return perfil;
        }

        private static void Limitar(PerfilInversor perfil, bool condicion, NivelRiesgoInversor maximo, string motivo)
        {
            if (!condicion) return;
            perfil.MotivosLimite.Add(motivo);
            if (perfil.NivelRiesgo > maximo) perfil.NivelRiesgo = maximo;
        }

        internal static NivelRiesgoInversor ClasificarPuntaje(decimal puntaje) =>
            puntaje < 40m ? NivelRiesgoInversor.Bajo : puntaje < 70m ? NivelRiesgoInversor.Moderado : NivelRiesgoInversor.Alto;
    }
}
