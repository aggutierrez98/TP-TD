using System;
using System.Collections.Generic;

namespace SERVICIOS
{
    internal class BitacoraSesionService
    {
        private Func<EventoBitacora, object, int> _registrador;
        private Func<FiltroBitacora, IList<EventoBitacora>> _consultor;

        public void Configurar(
            Func<EventoBitacora, object, int> registrador,
            Func<FiltroBitacora, IList<EventoBitacora>> consultor)
        {
            _registrador = registrador;
            _consultor = consultor;
        }

        public void Registrar(EventoBitacora evento, object contextoPersistencia)
        {
            if (evento == null)
            {
                return;
            }

            if (_registrador == null)
            {
                throw new InvalidOperationException("La bitacora no fue configurada.");
            }

            _registrador(evento, contextoPersistencia);
        }

        public IList<EventoBitacora> Consultar(FiltroBitacora filtro)
        {
            if (_consultor == null)
            {
                return new List<EventoBitacora>();
            }

            return _consultor(filtro ?? new FiltroBitacora());
        }
    }
}
