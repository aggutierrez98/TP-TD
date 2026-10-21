using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class EntidadFinancieraRepositorio : IEntidadFinancieraRepositorio
    {
        private readonly EntidadFinancieraDAL _entidadFinancieraDAL;
        private readonly MedioPagoEntidadDAL _medioPagoEntidadDAL;
        private readonly MedioPagoDAL _medioPagoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public EntidadFinancieraRepositorio()
            : this(null)
        {
        }

        internal EntidadFinancieraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _entidadFinancieraDAL = new EntidadFinancieraDAL();
            _medioPagoEntidadDAL = new MedioPagoEntidadDAL();
            _medioPagoDAL = new MedioPagoDAL();
            _accesoDatos = accesoDatos;
        }

        public EntidadFinanciera ObtenerPorId(int id)
        {
            EntidadFinanciera entidad = _entidadFinancieraDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(entidad);
            return entidad;
        }

        // Liviano a proposito: se usa para grillas y combos, no hidrata el catalogo de cada entidad.
        public List<EntidadFinanciera> ObtenerTodas()
        {
            return _entidadFinancieraDAL.ObtenerTodas(_accesoDatos);
        }

        // Se leen los medios directo del DAL (no via MedioPagoEntidadRepositorio) para no volver
        // a pedir esta misma entidad por cada item; cada item apunta a esta instancia. El catalogo
        // por entidad es chico, asi que el N+1 contra MedioPago es aceptable.
        private void Hidratar(EntidadFinanciera entidad)
        {
            if (entidad == null)
            {
                return;
            }

            entidad.MediosPagoEntidad = _medioPagoEntidadDAL.ObtenerPorEntidadFinanciera(entidad, _accesoDatos);
            foreach (MedioPagoEntidad medioPagoEntidad in entidad.MediosPagoEntidad)
            {
                medioPagoEntidad.EntidadFinanciera = entidad;
                medioPagoEntidad.MedioPago = _medioPagoDAL.ObtenerPorId(medioPagoEntidad.MedioPago.Id, _accesoDatos);
            }
        }

        public int InsertarYObtenerId(EntidadFinanciera entidad)
        {
            return _entidadFinancieraDAL.InsertarYObtenerId(entidad, _accesoDatos);
        }

        public bool Actualizar(EntidadFinanciera entidad)
        {
            return _entidadFinancieraDAL.Actualizar(entidad, _accesoDatos);
        }

        public bool Eliminar(EntidadFinanciera entidad)
        {
            return _entidadFinancieraDAL.Eliminar(entidad, _accesoDatos);
        }
    }
}
