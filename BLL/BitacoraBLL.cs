using System.Collections.Generic;
using System.Linq;
using SERVICIOS;

namespace BLL
{
    public class BitacoraBLL
    {
        public BitacoraBLL()
        {
            InfraestructuraBLL.Inicializar();
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return Sesion.GetInstance()
                .ConsultarBitacora(filtro)
                .OfType<EventoBitacora>()
                .ToList();
        }
    }
}
