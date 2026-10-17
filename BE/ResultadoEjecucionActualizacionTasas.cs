using System;

namespace BE
{
    // Resultado de una corrida de actualizacion de tasas: la ejecucion registrada en el
    // historial y, cuando se omitio por el tope horario, desde cuando se puede volver a
    // consultar a la API.
    public class ResultadoEjecucionActualizacionTasas
    {
        public EjecucionActualizacionTasas Ejecucion { get; set; }

        public DateTime? ProximaConsultaPermitida { get; set; }
    }
}
