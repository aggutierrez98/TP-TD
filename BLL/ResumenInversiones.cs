using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ABSTRACCIONES;
using BE;

namespace BLL
{
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    internal enum NivelInversion { Negativo, Ajustado, Comodo, Alto }

    internal sealed class ResumenInversiones
    {
        public decimal SaldoMensual { get; set; }
        public decimal GastoMensual { get; set; }
        // Solo existen si el usuario indico un porcentaje valido; sin el, se omiten del contexto.
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Porcentaje { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Monto { get; set; }
        public string Moneda { get; set; } = "ARS";
        public NivelInversion Nivel { get; set; }
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public NivelRiesgoInversor Perfil { get; set; }
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public NivelRiesgoInversor? RiesgoEfectivo { get; set; }
        public DateTime FechaPerfilUtc { get; set; }
        public int VersionPerfil { get; set; }
        public int MaximoMesesInmovilizacion { get; set; }
        public int? MaximoDiasHabilesDisponibilidad { get; set; }
        public List<string> CandidatosSinCompatibilidad { get; set; } = new List<string>();
        public List<InstrumentoInversion> Instrumentos { get; set; } = new List<InstrumentoInversion>();
        public DatoMercado Inflacion { get; set; }
        public List<string> FuentesFaltantes { get; set; } = new List<string>();
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ReferenciaAsistente> UniversoReferencias { get; set; } = new List<ReferenciaAsistente>();

        internal ContextoAsistente CrearContexto()
        {
            var referencias = Instrumentos.Select(i => Referencia(i.Dato)).ToList();
            if (Inflacion != null) referencias.Add(Referencia(Inflacion));
            return new ContextoAsistente(JsonSerializer.Serialize(this), referencias);
        }

        internal static ReferenciaAsistente Referencia(DatoMercado dato) => new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, dato.Nombre, dato.Tipo == "CEDEAR" || dato.Tipo == "Accion");
    }

    internal sealed class InstrumentoInversion
    {
        public DatoMercado Dato { get; set; }
        public decimal? Rendimiento30Dias { get; set; }
    }
}
