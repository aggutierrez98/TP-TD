using System.Collections.Generic;
using System.Linq;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class BitacoraBLL : IBitacoraBLL
    {
        public BitacoraBLL()
        {
            BootstrapAplicacionBLL.Inicializar();
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return BitacoraService.GetInstance()
                .ConsultarBitacora(filtro)
                .OfType<EventoBitacora>()
                .ToList();
        }

        public void RegistrarOperacion(string actividad, string informacion, string origen)
        {
            BitacoraService.GetInstance().RegistrarOperacion(actividad, informacion, origen);
        }

        public void RegistrarFalloNegocio(string actividad, string informacion, string origen)
        {
            BitacoraService.GetInstance().RegistrarFalloNegocio(actividad, informacion, origen);
        }

        internal void RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            BitacoraService.GetInstance().RegistrarOperacion(actividad, informacion, origen, unidad);
        }

        internal void RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            BitacoraService.GetInstance().RegistrarFalloNegocio(actividad, informacion, origen, unidad);
        }

        void IBitacoraBLL.RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            RegistrarOperacionInterno(actividad, informacion, origen, unidad);
        }

        void IBitacoraBLL.RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            RegistrarFalloNegocioInterno(actividad, informacion, origen, unidad);
        }
    }
}
