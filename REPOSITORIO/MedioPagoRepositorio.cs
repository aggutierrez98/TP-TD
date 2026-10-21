using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    // Catalogo hoja: no referencia a otras entidades, no necesita hidratacion.
    public class MedioPagoRepositorio : IMedioPagoRepositorio
    {
        private readonly MedioPagoDAL _medioPagoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public MedioPagoRepositorio()
            : this(null)
        {
        }

        internal MedioPagoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _medioPagoDAL = new MedioPagoDAL();
            _accesoDatos = accesoDatos;
        }

        public MedioPago ObtenerPorId(int id)
        {
            return _medioPagoDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<MedioPago> ObtenerTodos()
        {
            return _medioPagoDAL.ObtenerTodos(_accesoDatos);
        }

        public int InsertarYObtenerId(MedioPago medioPago)
        {
            return _medioPagoDAL.InsertarYObtenerId(medioPago, _accesoDatos);
        }

        public bool Actualizar(MedioPago medioPago)
        {
            return _medioPagoDAL.Actualizar(medioPago, _accesoDatos);
        }

        public bool Eliminar(MedioPago medioPago)
        {
            return _medioPagoDAL.Eliminar(medioPago, _accesoDatos);
        }
    }
}
