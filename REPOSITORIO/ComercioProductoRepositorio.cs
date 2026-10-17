using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ComercioProductoRepositorio : IComercioProductoRepositorio
    {
        private readonly ComercioProductoDAL _comercioProductoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ComercioProductoRepositorio()
            : this(null)
        {
        }

        internal ComercioProductoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _comercioProductoDAL = new ComercioProductoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<ComercioProducto> ObtenerPorComercio(int comercioId)
        {
            List<ComercioProducto> asignaciones = _comercioProductoDAL.ObtenerPorComercio(comercioId, _accesoDatos);
            asignaciones.ForEach(Hidratar);
            return asignaciones;
        }

        public ComercioProducto ObtenerPorId(int comercioId, int productoId)
        {
            ComercioProducto asignacion = _comercioProductoDAL.ObtenerPorId(comercioId, productoId, _accesoDatos);
            Hidratar(asignacion);
            return asignacion;
        }

        private void Hidratar(ComercioProducto asignacion)
        {
            if (asignacion == null)
            {
                return;
            }

            asignacion.Comercio = new ComercioRepositorio(_accesoDatos).ObtenerPorId(asignacion.Comercio.Id);
            asignacion.Producto = new ProductoRepositorio(_accesoDatos).ObtenerPorId(asignacion.Producto.Id);
            asignacion.Moneda = new MonedaRepositorio(_accesoDatos).ObtenerPorId(asignacion.Moneda.Id);
        }

        public bool Insertar(ComercioProducto comercioProducto)
        {
            return _comercioProductoDAL.Insertar(comercioProducto, _accesoDatos);
        }

        public bool ActualizarPrecio(int comercioId, int productoId, decimal precio)
        {
            return _comercioProductoDAL.ActualizarPrecio(comercioId, productoId, precio, _accesoDatos);
        }

        public bool Eliminar(int comercioId, int productoId)
        {
            return _comercioProductoDAL.Eliminar(comercioId, productoId, _accesoDatos);
        }

        public int ContarPorProducto(int productoId)
        {
            return _comercioProductoDAL.ContarPorProducto(productoId, _accesoDatos);
        }
    }
}
