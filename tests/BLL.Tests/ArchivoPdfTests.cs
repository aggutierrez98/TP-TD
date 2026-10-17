using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ArchivoPdfTests
    {
        [TestMethod]
        public void Guardar_ArchivoNuevoYSobrescritura_ConservaSoloElDestinoCompleto()
        {
            EnCarpetaTemporal(carpeta =>
            {
                string destino = Path.Combine(carpeta, "reporte.pdf");
                ArchivoPdf.Guardar(destino, new byte[] { 1, 2, 3 });
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, File.ReadAllBytes(destino));
                ArchivoPdf.Guardar(destino, new byte[] { 4, 5 });
                CollectionAssert.AreEqual(new byte[] { 4, 5 }, File.ReadAllBytes(destino));
                Assert.HasCount(1, Directory.GetFiles(carpeta));
            });
        }

        [TestMethod]
        public void Guardar_DestinoBloqueado_PreservaOriginalYLimpiaTemporal()
        {
            EnCarpetaTemporal(carpeta =>
            {
                string destino = Path.Combine(carpeta, "reporte.pdf");
                File.WriteAllBytes(destino, new byte[] { 7, 8 });
                using (File.Open(destino, FileMode.Open, FileAccess.Read, FileShare.None))
                    Assert.Throws<IOException>(() => ArchivoPdf.Guardar(destino, new byte[] { 9 }));
                CollectionAssert.AreEqual(new byte[] { 7, 8 }, File.ReadAllBytes(destino));
                Assert.HasCount(1, Directory.GetFiles(carpeta));
            });
        }

        [TestMethod]
        public void Guardar_DestinoEsDirectorio_NoLoReemplazaNiDejaTemporal()
        {
            EnCarpetaTemporal(carpeta =>
            {
                string destino = Path.Combine(carpeta, "reporte.pdf");
                Directory.CreateDirectory(destino);
                Assert.Throws<IOException>(() => ArchivoPdf.Guardar(destino, new byte[] { 9 }));
                Assert.IsTrue(Directory.Exists(destino));
                Assert.IsEmpty(Directory.GetFiles(carpeta));
            });
        }

        private static void EnCarpetaTemporal(Action<string> accion)
        {
            string carpeta = Path.Combine(Path.GetTempPath(), "finazapp-pdf-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
            try { accion(carpeta); }
            finally { Directory.Delete(carpeta, true); }
        }
    }
}
