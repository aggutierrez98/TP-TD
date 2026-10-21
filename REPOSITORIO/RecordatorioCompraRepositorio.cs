using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class RecordatorioCompraRepositorio : IRecordatorioCompraRepositorio
    {
        private readonly RecordatorioCompraDAL _recordatorioCompraDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public RecordatorioCompraRepositorio()
            : this(null)
        {
        }

        internal RecordatorioCompraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _recordatorioCompraDAL = new RecordatorioCompraDAL();
            _accesoDatos = accesoDatos;
        }

        public RecordatorioCompra ObtenerPorId(int id)
        {
            RecordatorioCompra recordatorio = _recordatorioCompraDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(recordatorio);
            return recordatorio;
        }

        public List<RecordatorioCompra> ObtenerPorUsuario(Usuario usuario)
        {
            List<RecordatorioCompra> recordatorios = _recordatorioCompraDAL.ObtenerPorUsuario(usuario, _accesoDatos);
            recordatorios.ForEach(Hidratar);
            return recordatorios;
        }

        public List<RecordatorioCompra> ObtenerTodas()
        {
            List<RecordatorioCompra> recordatorios = _recordatorioCompraDAL.ObtenerTodas(_accesoDatos);
            recordatorios.ForEach(Hidratar);
            return recordatorios;
        }

        private void Hidratar(RecordatorioCompra recordatorio)
        {
            if (recordatorio == null)
            {
                return;
            }

            recordatorio.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(recordatorio.Usuario.Id);
            if (recordatorio.ItemLista != null)
            {
                recordatorio.ItemLista = new ItemListaRepositorio(_accesoDatos).ObtenerPorId(recordatorio.ItemLista.Id);
            }

            if (recordatorio.Promocion != null)
            {
                recordatorio.Promocion = new PromocionRepositorio(_accesoDatos).ObtenerPorId(recordatorio.Promocion.Id);
            }
        }

        public int InsertarYObtenerId(RecordatorioCompra recordatorio)
        {
            return _recordatorioCompraDAL.InsertarYObtenerId(recordatorio, _accesoDatos);
        }

        public bool Actualizar(RecordatorioCompra recordatorio)
        {
            return _recordatorioCompraDAL.Actualizar(recordatorio, _accesoDatos);
        }

        public bool Eliminar(RecordatorioCompra recordatorio)
        {
            return _recordatorioCompraDAL.Eliminar(recordatorio, _accesoDatos);
        }
    }
}
