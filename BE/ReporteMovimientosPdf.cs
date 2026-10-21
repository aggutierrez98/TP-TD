using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace BE
{
    // Captura del listado: las colecciones no cambian mientras se genera el reporte.
    public sealed class ReporteMovimientosPdf : IReporteTabularPdf
    {
        private readonly byte[] logo;

        public ReporteMovimientosPdf(Usuario usuario, string titulo, IEnumerable<string> encabezados,
            IEnumerable<IEnumerable<string>> filas, IEnumerable<string> filtros, byte[] logo,
            DateTime fechaGeneracion, string etiquetaFechaGeneracion, string etiquetaCantidad,
            string formatoPagina, string textoContinuacion)
        {
            Usuario = usuario;
            Titulo = titulo;
            Encabezados = encabezados == null ? null : Array.AsReadOnly(encabezados.ToArray());
            Filas = filas == null ? null : Array.AsReadOnly(filas.Select(f => f == null ? null :
                (IReadOnlyList<string>)Array.AsReadOnly(f.ToArray())).ToArray());
            Filtros = Array.AsReadOnly((filtros ?? Enumerable.Empty<string>()).ToArray());
            this.logo = logo == null ? null : (byte[])logo.Clone();
            FechaGeneracion = fechaGeneracion;
            EtiquetaFechaGeneracion = etiquetaFechaGeneracion;
            EtiquetaCantidad = etiquetaCantidad;
            FormatoPagina = formatoPagina;
            TextoContinuacion = textoContinuacion;
        }

        public Usuario Usuario { get; }
        public string Titulo { get; }
        public IReadOnlyList<string> Encabezados { get; }
        public IReadOnlyList<IReadOnlyList<string>> Filas { get; }
        public IReadOnlyList<string> Filtros { get; }
        public byte[] Logo => logo == null ? null : (byte[])logo.Clone();
        public DateTime FechaGeneracion { get; }
        public string EtiquetaFechaGeneracion { get; }
        public string EtiquetaCantidad { get; }
        public string FormatoPagina { get; }
        public string TextoContinuacion { get; }
    }
}
