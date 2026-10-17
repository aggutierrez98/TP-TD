using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ABSTRACCIONES;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;

namespace SERVICIOS
{
    public sealed class ReporteTabularPdfService : IReporteTabularPdfService
    {
        public byte[] Generar(IReporteTabularPdf reporte)
        {
            if (reporte == null) throw new ArgumentNullException(nameof(reporte));
            var document = new Document();
            document.Info.Title = reporte.Titulo;
            document.Info.Author = "FINAZAPP";
            var normal = document.Styles[StyleNames.Normal];
            normal.Font.Name = "Segoe UI";
            normal.Font.Size = 9;
            normal.Font.Color = Color.FromRgb(62, 58, 78);
            normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(3);

            var section = document.AddSection();
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.Orientation = Orientation.Landscape;
            section.PageSetup.LeftMargin = Unit.FromMillimeter(12);
            section.PageSetup.RightMargin = Unit.FromMillimeter(12);
            section.PageSetup.TopMargin = Unit.FromMillimeter(35);
            section.PageSetup.BottomMargin = Unit.FromMillimeter(16);
            section.PageSetup.HeaderDistance = Unit.FromMillimeter(8);
            section.PageSetup.FooterDistance = Unit.FromMillimeter(8);

            var header = section.Headers.Primary.AddTable();
            header.AddColumn(Unit.FromMillimeter(85));
            header.AddColumn(Unit.FromMillimeter(188));
            var headerRow = header.AddRow();
            byte[] logo = reporte.Logo;
            if (logo != null && logo.Length > 0)
            {
                var image = headerRow.Cells[0].AddImage("base64:" + Convert.ToBase64String(logo));
                image.Width = Unit.FromMillimeter(65);
                image.LockAspectRatio = true;
            }
            else
            {
                var brand = headerRow.Cells[0].AddParagraph("FINAZAPP");
                brand.Format.Font.Size = 20;
                brand.Format.Font.Bold = true;
            }
            var title = headerRow.Cells[1].AddParagraph(reporte.Titulo);
            title.Format.Alignment = ParagraphAlignment.Right;
            title.Format.Font.Size = 16;
            title.Format.Font.Bold = true;
            var generated = headerRow.Cells[1].AddParagraph(reporte.EtiquetaFechaGeneracion + ": " +
                reporte.FechaGeneracion.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
            generated.Format.Alignment = ParagraphAlignment.Right;

            foreach (string filtro in reporte.Filtros) section.AddParagraph(filtro ?? string.Empty);
            var count = section.AddParagraph(reporte.EtiquetaCantidad + ": " + reporte.Filas.Count);
            count.Format.Font.Bold = true;
            count.Format.SpaceAfter = Unit.FromMillimeter(4);

            var footer = section.Footers.Primary.AddParagraph();
            footer.Format.Alignment = ParagraphAlignment.Right;
            footer.Format.Font.Size = 8;
            AgregarNumeracion(footer, reporte.FormatoPagina);

            var table = section.AddTable();
            table.Borders.Width = Unit.FromPoint(0.25);
            table.Borders.Color = Color.FromRgb(220, 227, 222);
            table.TopPadding = Unit.FromMillimeter(2);
            table.BottomPadding = Unit.FromMillimeter(2);
            table.LeftPadding = Unit.FromMillimeter(2);
            table.RightPadding = Unit.FromMillimeter(2);
            double[] widths = { 27.3, 24.57, 40.95, 46.41, 35.49, 98.28 };
            foreach (double width in widths) table.AddColumn(Unit.FromMillimeter(width));
            var heading = table.AddRow();
            heading.HeadingFormat = true;
            heading.Shading.Color = Color.FromRgb(231, 245, 235);
            heading.Format.Font.Bold = true;
            for (int i = 0; i < 6; i++) heading.Cells[i].AddParagraph(reporte.Encabezados[i]);

            using (var measure = XGraphics.CreateMeasureContext(new XSize(1000, 1000), XGraphicsUnit.Point, XPageDirection.Downwards))
            {
                var font = new XFont("Segoe UI", 9, XFontStyleEx.Regular);
                foreach (var fila in reporte.Filas)
                {
                    // MigraDoc no divide una fila mas alta que una pagina. Fragmentar
                    // por lineas medidas conserva todo el texto, incluso palabras largas.
                    var lines = fila.Select((value, i) => AjustarLineas(value ?? string.Empty,
                        widths[i] * 72 / 25.4 - 4 * 72 / 25.4 - 2, measure, font)).ToArray();
                    int fragments = Math.Max(1, (lines.Max(l => l.Count) + 17) / 18);
                    for (int part = 0; part < fragments; part++)
                    {
                        var row = table.AddRow();
                        row.VerticalAlignment = VerticalAlignment.Top;
                        for (int i = 0; i < 6; i++)
                        {
                            var paragraph = row.Cells[i].AddParagraph();
                            paragraph.Format.SpaceAfter = Unit.FromPoint(0);
                            if (i == 4) paragraph.Format.Alignment = ParagraphAlignment.Right;
                            if (part > 0 && i == 5)
                            {
                                paragraph.AddText(reporte.TextoContinuacion + " (" + (part + 1) + ")");
                                paragraph.AddLineBreak();
                            }
                            var fragment = lines[i].Skip(part * 18).Take(18).ToArray();
                            for (int j = 0; j < fragment.Length; j++)
                            {
                                if (j > 0) paragraph.AddLineBreak();
                                paragraph.AddText(fragment[j]);
                            }
                        }
                    }
                }
            }

            var renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();
            using (var stream = new MemoryStream())
            {
                renderer.PdfDocument.Save(stream, false);
                return stream.ToArray();
            }
        }

        private static void AgregarNumeracion(Paragraph paragraph, string formato)
        {
            // Los campos de pagina se resuelven luego de maquetar el documento.
            string[] parts = (formato ?? "{0} / {1}").Split(new[] { "{0}", "{1}" }, StringSplitOptions.None);
            if (parts.Length != 3) throw new ArgumentException("Formato de numeracion invalido.");
            paragraph.AddText(parts[0]);
            paragraph.AddPageField();
            paragraph.AddText(parts[1]);
            paragraph.AddNumPagesField();
            paragraph.AddText(parts[2]);
        }

        private static List<string> AjustarLineas(string text, double width, XGraphics graphics, XFont font)
        {
            var result = new List<string>();
            foreach (string line in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                if (line.Length == 0) { result.Add(string.Empty); continue; }
                int offset = 0;
                while (offset < line.Length)
                {
                    int low = 1, high = line.Length - offset;
                    while (low < high)
                    {
                        int mid = (low + high + 1) / 2;
                        if (graphics.MeasureString(line.Substring(offset, mid), font).Width <= width) low = mid;
                        else high = mid - 1;
                    }
                    int length = low;
                    if (offset + length < line.Length)
                    {
                        int space = line.LastIndexOf(' ', offset + length - 1, length);
                        if (space > offset) length = space - offset + 1;
                        if (length > 1 && char.IsHighSurrogate(line[offset + length - 1])) length--;
                    }
                    result.Add(line.Substring(offset, length));
                    offset += length;
                }
            }
            return result;
        }
    }
}
