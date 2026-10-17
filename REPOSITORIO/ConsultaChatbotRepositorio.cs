using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ConsultaChatbotRepositorio : IConsultaChatbotRepositorio
    {
        private readonly ConsultaChatbotDAL _consultaChatbotDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ConsultaChatbotRepositorio()
            : this(null)
        {
        }

        internal ConsultaChatbotRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _consultaChatbotDAL = new ConsultaChatbotDAL();
            _accesoDatos = accesoDatos;
        }

        public List<ConsultaChatbot> ObtenerPorUsuario(int usuarioId)
        {
            return _consultaChatbotDAL.ObtenerPorUsuario(usuarioId, _accesoDatos);
        }

        public int InsertarYObtenerId(ConsultaChatbot consultaChatbot)
        {
            return _consultaChatbotDAL.InsertarYObtenerId(consultaChatbot, _accesoDatos);
        }
    }
}
