using System;
using System.Collections.Generic;

namespace BE
{
    public class EjecucionActualizacionTasas
    {
        public int Id { get; set; }

        public DateTime FechaInicio { get; set; }

        public OrigenActualizacionTasas Origen { get; set; }

        public ResultadoActualizacionTasas Resultado { get; set; }

        public bool ConsumioRequest { get; set; }

        public string Mensaje { get; set; }

        public List<ActualizacionTasaDetalle> Detalles { get; set; } = new List<ActualizacionTasaDetalle>();
    }
}
