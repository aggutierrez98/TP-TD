using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class RecordatorioInversionRepositorio : IRecordatorioInversionRepositorio
    {
        private readonly RecordatorioInversionDAL _recordatorioInversionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public RecordatorioInversionRepositorio()
            : this(null)
        {
        }

        internal RecordatorioInversionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _recordatorioInversionDAL = new RecordatorioInversionDAL();
            _accesoDatos = accesoDatos;
        }

        public RecordatorioInversion ObtenerPorId(int id)
        {
            RecordatorioInversion recordatorio = _recordatorioInversionDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(recordatorio);
            return recordatorio;
        }

        public List<RecordatorioInversion> ObtenerPorUsuario(Usuario usuario)
        {
            List<RecordatorioInversion> recordatorios = _recordatorioInversionDAL.ObtenerPorUsuario(usuario, _accesoDatos);
            recordatorios.ForEach(Hidratar);
            return recordatorios;
        }

        public List<RecordatorioInversion> ObtenerTodas()
        {
            List<RecordatorioInversion> recordatorios = _recordatorioInversionDAL.ObtenerTodas(_accesoDatos);
            recordatorios.ForEach(Hidratar);
            return recordatorios;
        }

        private void Hidratar(RecordatorioInversion recordatorio)
        {
            if (recordatorio == null)
            {
                return;
            }

            recordatorio.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(recordatorio.Usuario.Id);
        }

        public int InsertarYObtenerId(RecordatorioInversion recordatorio)
        {
            return _recordatorioInversionDAL.InsertarYObtenerId(recordatorio, _accesoDatos);
        }

        public bool Actualizar(RecordatorioInversion recordatorio)
        {
            return _recordatorioInversionDAL.Actualizar(recordatorio, _accesoDatos);
        }

        public bool Eliminar(RecordatorioInversion recordatorio)
        {
            return _recordatorioInversionDAL.Eliminar(recordatorio, _accesoDatos);
        }
    }
}
