using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class MedioPagoEntidadRepositorio : IMedioPagoEntidadRepositorio
    {
        private readonly MedioPagoEntidadDAL _medioPagoEntidadDAL;
        private readonly MedioPagoDAL _medioPagoDAL;
        private readonly EntidadFinancieraDAL _entidadFinancieraDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public MedioPagoEntidadRepositorio()
            : this(null)
        {
        }

        internal MedioPagoEntidadRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _medioPagoEntidadDAL = new MedioPagoEntidadDAL();
            _medioPagoDAL = new MedioPagoDAL();
            _entidadFinancieraDAL = new EntidadFinancieraDAL();
            _accesoDatos = accesoDatos;
        }

        public MedioPagoEntidad ObtenerPorId(int id)
        {
            MedioPagoEntidad medioPagoEntidad = _medioPagoEntidadDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(medioPagoEntidad);
            return medioPagoEntidad;
        }

        public List<MedioPagoEntidad> ObtenerPorEntidadFinanciera(EntidadFinanciera entidad)
        {
            List<MedioPagoEntidad> mediosPagoEntidad = _medioPagoEntidadDAL.ObtenerPorEntidadFinanciera(entidad, _accesoDatos);
            mediosPagoEntidad.ForEach(Hidratar);
            return mediosPagoEntidad;
        }

        public List<MedioPagoEntidad> ObtenerPorMedioPago(MedioPago medioPago)
        {
            List<MedioPagoEntidad> mediosPagoEntidad = _medioPagoEntidadDAL.ObtenerPorMedioPago(medioPago, _accesoDatos);
            mediosPagoEntidad.ForEach(Hidratar);
            return mediosPagoEntidad;
        }

        public List<string> ObtenerSegmentosDistintos()
        {
            return _medioPagoEntidadDAL.ObtenerSegmentosDistintos(_accesoDatos);
        }

        // Compone DALs directamente (no EntidadFinancieraRepositorio): la entidad queda
        // liviana (Id + Nombre, sin su catalogo MediosPagoEntidad) para no recursar.
        private void Hidratar(MedioPagoEntidad medioPagoEntidad)
        {
            if (medioPagoEntidad == null)
            {
                return;
            }

            medioPagoEntidad.MedioPago = _medioPagoDAL.ObtenerPorId(medioPagoEntidad.MedioPago.Id, _accesoDatos);
            medioPagoEntidad.EntidadFinanciera = _entidadFinancieraDAL.ObtenerPorId(medioPagoEntidad.EntidadFinanciera.Id, _accesoDatos);
        }

        public int InsertarYObtenerId(MedioPagoEntidad medioPagoEntidad)
        {
            return _medioPagoEntidadDAL.InsertarYObtenerId(medioPagoEntidad, _accesoDatos);
        }

        public bool Actualizar(MedioPagoEntidad medioPagoEntidad)
        {
            return _medioPagoEntidadDAL.Actualizar(medioPagoEntidad, _accesoDatos);
        }

        public bool Eliminar(MedioPagoEntidad medioPagoEntidad)
        {
            return _medioPagoEntidadDAL.Eliminar(medioPagoEntidad, _accesoDatos);
        }
    }
}
