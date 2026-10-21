using System;
using System.Collections.Generic;

namespace BE
{
    public enum NivelRiesgoInversor { Bajo = 0, Moderado = 1, Alto = 2 }

    // Pregunta del cuestionario. El catalogo es fijo (no hay tabla): Id es su numero, de 1 a 10,
    // el que guarda PerfilInversorRespuesta. Inmutable: las respuestas copiadas la comparten.
    public sealed class PreguntaPerfilInversor
    {
        public PreguntaPerfilInversor(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public sealed class RespuestaPerfilInversor
    {
        public PreguntaPerfilInversor Pregunta { get; set; }
        public string OpcionId { get; set; }
    }

    public sealed class PerfilInversor
    {
        public int Id { get; set; }
        public Usuario Usuario { get; set; }
        public int VersionCuestionario { get; set; }
        public DateTime FechaEvaluacionUtc { get; set; }
        public decimal Capacidad { get; set; }
        public decimal Tolerancia { get; set; }
        public decimal Puntaje { get; set; }
        public NivelRiesgoInversor NivelRiesgo { get; set; }
        public List<string> MotivosLimite { get; set; } = new List<string>();
        public List<RespuestaPerfilInversor> Respuestas { get; set; } = new List<RespuestaPerfilInversor>();
    }
}
