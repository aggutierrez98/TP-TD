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

        public List<ComercioProducto> ObtenerPorComercio(Comercio comercio)
        {
            List<ComercioProducto> asignaciones = _comercioProductoDAL.ObtenerPorComercio(comercio, _accesoDatos);
            asignaciones.ForEach(Hidratar);
            return asignaciones;
        }

        public ComercioProducto ObtenerPorComercioYProducto(Comercio comercio, Producto producto)
        {
            ComercioProducto asignacion = _comercioProductoDAL.ObtenerPorComercioYProducto(comercio, producto, _accesoDatos);
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

        public bool ActualizarPrecio(ComercioProducto comercioProducto)
        {
            return _comercioProductoDAL.ActualizarPrecio(comercioProducto, _accesoDatos);
        }

        public bool Eliminar(ComercioProducto comercioProducto)
        {
            return _comercioProductoDAL.Eliminar(comercioProducto, _accesoDatos);
        }

        public int ContarPorProducto(Producto producto)
        {
            return _comercioProductoDAL.ContarPorProducto(producto, _accesoDatos);
        }
    }
}
