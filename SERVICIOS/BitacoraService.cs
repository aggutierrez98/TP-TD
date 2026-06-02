using System;
using System.Collections.Generic;

namespace SERVICIOS
{
    public class BitacoraService
    {
        private static BitacoraService _instancia;

        private Func<EventoBitacora, object, int> _registrador;
        private Func<FiltroBitacora, IList<EventoBitacora>> _consultor;
        private Func<int?> _obtenedorUsuarioId;
        private Func<string> _obtenedorUsername;

        private BitacoraService()
        {
        }

        public static BitacoraService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new BitacoraService();
            }

            return _instancia;
        }

        public void Configurar(
            Func<EventoBitacora, object, int> registrador,
            Func<FiltroBitacora, IList<EventoBitacora>> consultor,
            Func<int?> obtenedorUsuarioId,
            Func<string> obtenedorUsername)
        {
            _registrador = registrador;
            _consultor = consultor;
            _obtenedorUsuarioId = obtenedorUsuarioId;
            _obtenedorUsername = obtenedorUsername;
        }

        public void RegistrarOperacion(string actividad, string informacion, string origen, object contextoPersistencia = null)
        {
            RegistrarEventoBitacora(actividad, informacion, "Informacion", origen, false, contextoPersistencia);
        }

        public void RegistrarFalloNegocio(string actividad, string informacion, string origen, object contextoPersistencia = null)
        {
            RegistrarEventoBitacora(actividad, informacion, "Advertencia", origen, false, contextoPersistencia);
        }

        public void RegistrarExcepcionTecnica(string actividad, Exception exception, string origen, object contextoPersistencia = null)
        {
            string informacion = exception == null
                ? "Sin detalle."
                : string.Format("{0}: {1}", exception.GetType().Name, exception.Message);

            RegistrarEventoBitacora(actividad, informacion, "Error", origen, true, contextoPersistencia);
        }

        public IList<EventoBitacora> ConsultarBitacora(FiltroBitacora filtro)
        {
            if (_consultor == null)
            {
                return new List<EventoBitacora>();
            }

            return _consultor(filtro ?? new FiltroBitacora());
        }

        private void RegistrarEventoBitacora(
            string actividad,
            string informacion,
            string nivel,
            string origen,
            bool esTecnica,
            object contextoPersistencia)
        {
            if (_registrador == null)
            {
                throw new InvalidOperationException("La bitacora no fue configurada.");
            }

            EventoBitacora evento = new EventoBitacora
            {
                FechaHora = DateTime.Now,
                UsuarioId = _obtenedorUsuarioId == null ? (int?)null : _obtenedorUsuarioId(),
                UsernameSnapshot = _obtenedorUsername == null ? null : _obtenedorUsername(),
                Actividad = actividad,
                Informacion = informacion,
                Nivel = nivel,
                Origen = origen,
                EsTecnica = esTecnica
            };

            _registrador(evento, contextoPersistencia);
        }
    }
}
