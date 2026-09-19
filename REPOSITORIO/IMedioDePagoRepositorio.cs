using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IMedioDePagoRepositorio
    {
        MedioDePago ObtenerPorId(int id);

        List<MedioDePago> ObtenerPorEntidadFinanciera(int entidadFinancieraId);

        int InsertarYObtenerId(MedioDePago medioDePago);

        bool Actualizar(MedioDePago medioDePago);

        bool Eliminar(int id);
    }
}
