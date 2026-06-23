using System.Collections.Generic;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IBitacoraRepositorio
    {
        int Guardar(EventoBitacora evento, IUnidadDeTrabajo unidad = null);

        List<EventoBitacora> Buscar(FiltroBitacora filtro);
    }
}
