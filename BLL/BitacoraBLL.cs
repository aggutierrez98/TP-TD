using System.Collections.Generic;
using System.Linq;
using SERVICIOS;

namespace BLL
{
    public class BitacoraBLL
    {
        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return BitacoraService.GetInstance()
                .ConsultarBitacora(filtro)
                .OfType<EventoBitacora>()
                .ToList();
        }
    }
}
