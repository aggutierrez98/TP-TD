using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IRecordatorioCompraRepositorio
    {
        RecordatorioCompra ObtenerPorId(int id);

        List<RecordatorioCompra> ObtenerPorUsuario(int usuarioId);

        List<RecordatorioCompra> ObtenerTodas();

        int InsertarYObtenerId(RecordatorioCompra recordatorio);

        bool Actualizar(RecordatorioCompra recordatorio);

        bool Eliminar(int id);
    }
}
