using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class MedioDePagoRepositorio : IMedioDePagoRepositorio
    {
        private readonly MedioDePagoDAL _medioDePagoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public MedioDePagoRepositorio()
            : this(null)
        {
        }

        internal MedioDePagoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _medioDePagoDAL = new MedioDePagoDAL();
            _accesoDatos = accesoDatos;
        }

        public MedioDePago ObtenerPorId(int id)
        {
            MedioDePago medioDePago = _medioDePagoDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(medioDePago);
            return medioDePago;
        }

        public List<MedioDePago> ObtenerPorEntidadFinanciera(int entidadFinancieraId)
        {
            List<MedioDePago> mediosDePago = _medioDePagoDAL.ObtenerPorEntidadFinanciera(entidadFinancieraId, _accesoDatos);
            mediosDePago.ForEach(Hidratar);
            return mediosDePago;
        }

        private void Hidratar(MedioDePago medioDePago)
        {
            if (medioDePago == null)
            {
                return;
            }

            medioDePago.EntidadFinanciera = new EntidadFinancieraRepositorio(_accesoDatos).ObtenerPorId(medioDePago.EntidadFinanciera.Id);
        }

        public int InsertarYObtenerId(MedioDePago medioDePago)
        {
            return _medioDePagoDAL.InsertarYObtenerId(medioDePago, _accesoDatos);
        }

        public bool Actualizar(MedioDePago medioDePago)
        {
            return _medioDePagoDAL.Actualizar(medioDePago, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _medioDePagoDAL.Eliminar(id, _accesoDatos);
        }
    }
}
