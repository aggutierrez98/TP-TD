using BE;

namespace BLL
{
    // Producto del catalogo asociado a un item de texto libre, con el puntaje que lo justifico
    // y si salio de la comparacion semantica (respaldo) en vez de la local.
    internal sealed class CoincidenciaProducto
    {
        internal CoincidenciaProducto(Producto producto, double puntaje, bool esSemantica)
        {
            Producto = producto;
            Puntaje = puntaje;
            EsSemantica = esSemantica;
        }

        internal Producto Producto { get; private set; }

        internal double Puntaje { get; private set; }

        internal bool EsSemantica { get; private set; }
    }
}
