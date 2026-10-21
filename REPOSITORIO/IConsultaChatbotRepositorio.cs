using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IConsultaChatbotRepositorio
    {
        List<ConsultaChatbot> ObtenerPorUsuario(Usuario usuario);

        int InsertarYObtenerId(ConsultaChatbot consultaChatbot);
    }
}
