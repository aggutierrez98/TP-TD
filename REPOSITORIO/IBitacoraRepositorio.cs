using System.Collections.Generic;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IBitacoraRepositorio
    {
        int Guardar(EventoBitacora evento);

        List<EventoBitacora> Buscar(FiltroBitacora filtro);
    }
}
