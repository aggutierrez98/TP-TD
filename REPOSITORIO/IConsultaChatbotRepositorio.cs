using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IConsultaChatbotRepositorio
    {
        List<ConsultaChatbot> ObtenerPorUsuario(int usuarioId);

        int InsertarYObtenerId(ConsultaChatbot consultaChatbot);
    }
}
