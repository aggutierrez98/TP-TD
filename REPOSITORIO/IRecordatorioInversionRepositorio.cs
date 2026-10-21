using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IRecordatorioInversionRepositorio
    {
        RecordatorioInversion ObtenerPorId(int id);

        List<RecordatorioInversion> ObtenerPorUsuario(Usuario usuario);

        List<RecordatorioInversion> ObtenerTodas();

        int InsertarYObtenerId(RecordatorioInversion recordatorio);

        bool Actualizar(RecordatorioInversion recordatorio);

        bool Eliminar(RecordatorioInversion recordatorio);
    }
}
