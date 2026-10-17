using System;
using System.IO;

namespace TpIDS
{
    internal static class ArchivoPdf
    {
        public static void Guardar(string destino, byte[] contenido)
        {
            string ruta = Path.GetFullPath(destino);
            string temporal = Path.Combine(Path.GetDirectoryName(ruta), ".finazapp-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (var stream = new FileStream(temporal, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(contenido, 0, contenido.Length);
                    stream.Flush(true);
                }
                if (File.Exists(ruta)) File.Replace(temporal, ruta, null);
                else File.Move(temporal, ruta);
            }
            finally
            {
                // Un error de limpieza no debe ocultar el error original de escritura.
                try { if (File.Exists(temporal)) File.Delete(temporal); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
}
