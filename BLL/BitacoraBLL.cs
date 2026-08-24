using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class BitacoraBLL : IBitacora, IBitacoraBLL
    {
        private readonly IBitacoraRepositorio _bitacoraRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;

        internal BitacoraBLL(
            IBitacoraRepositorio bitacoraRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion)
        {
            _bitacoraRepositorio = bitacoraRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return _bitacoraRepositorio.Buscar(filtro ?? new FiltroBitacora());
        }

        public void RegistrarOperacion(string actividad, string informacion, string origen)
        {
            RegistrarConUnidadPropia(ConstruirEvento(actividad, informacion, "Informacion", origen, false));
        }

        public void RegistrarFalloNegocio(string actividad, string informacion, string origen)
        {
            RegistrarConUnidadPropia(ConstruirEvento(actividad, informacion, "Advertencia", origen, false));
        }

        public void RegistrarExcepcionTecnica(string actividad, Exception exception, string origen)
        {
            string informacion = exception == null
                ? "Sin detalle."
                : string.Format("{0}: {1}", exception.GetType().Name, exception.Message);

            RegistrarConUnidadPropia(ConstruirEvento(actividad, informacion, "Error", origen, true));
        }

        internal void RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            RegistrarInterno(ConstruirEvento(actividad, informacion, "Informacion", origen, false), unidad);
        }

        internal void RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            RegistrarInterno(ConstruirEvento(actividad, informacion, "Advertencia", origen, false), unidad);
        }

        IList<IEventoBitacora> IBitacora.ConsultarBitacora(IFiltroBitacora filtro)
        {
            return Buscar(filtro as FiltroBitacora)
                .Cast<IEventoBitacora>()
                .ToList();
        }

        void IBitacoraBLL.RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            RegistrarOperacionInterno(actividad, informacion, origen, unidad);
        }

        void IBitacoraBLL.RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            RegistrarFalloNegocioInterno(actividad, informacion, origen, unidad);
        }

        private void RegistrarConUnidadPropia(EventoBitacora evento)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                RegistrarInterno(evento, unidad);
                unidad.Save();
            }
        }

        private static void RegistrarInterno(EventoBitacora evento, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            unidad.Bitacora.Guardar(evento);
        }

        private EventoBitacora ConstruirEvento(
            string actividad,
            string informacion,
            string nivel,
            string origen,
            bool esTecnica)
        {
            return new EventoBitacora
            {
                FechaHora = DateTime.Now,
                UsuarioId = _sesion.GetUsuarioActualId(),
                UsernameSnapshot = _sesion.GetUsuarioActualNombre(),
                Actividad = actividad,
                Informacion = informacion,
                Nivel = nivel,
                Origen = origen,
                EsTecnica = esTecnica
            };
        }
    }
}
