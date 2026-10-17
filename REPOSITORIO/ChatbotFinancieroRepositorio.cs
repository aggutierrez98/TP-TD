using BE;
using DAL;

namespace REPOSITORIO
{
    public class ChatbotFinancieroRepositorio : IChatbotFinancieroRepositorio
    {
        private readonly ChatbotFinancieroDAL _chatbotFinancieroDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ChatbotFinancieroRepositorio()
            : this(null)
        {
        }

        internal ChatbotFinancieroRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _chatbotFinancieroDAL = new ChatbotFinancieroDAL();
            _accesoDatos = accesoDatos;
        }

        public ChatbotFinanciero ObtenerUnico()
        {
            return _chatbotFinancieroDAL.ObtenerUnico(_accesoDatos);
        }
    }
}
