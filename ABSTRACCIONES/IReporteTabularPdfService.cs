using System;
using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IReporteTabularPdf
    {
        string Titulo { get; }
        IReadOnlyList<string> Encabezados { get; }
        IReadOnlyList<IReadOnlyList<string>> Filas { get; }
        IReadOnlyList<string> Filtros { get; }
        byte[] Logo { get; }
        DateTime FechaGeneracion { get; }
        string EtiquetaFechaGeneracion { get; }
        string EtiquetaCantidad { get; }
        string FormatoPagina { get; }
        string TextoContinuacion { get; }
    }

    public interface IReporteTabularPdfService
    {
        byte[] Generar(IReporteTabularPdf reporte);
    }
}
