using System;

namespace BE
{
    // Representacion semantica precalculada de un producto: el vector que devolvio Modelo para
    // TextoFuente. Si cualquiera de los dos cambia, el vector queda desactualizado y se recalcula.
    public class ProductoEmbedding
    {
        public Producto Producto { get; set; }

        public string Modelo { get; set; }

        public string TextoFuente { get; set; }

        public float[] Vector { get; set; }

        public DateTime FechaCalculo { get; set; }
    }
}
