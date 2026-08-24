using System;
using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Contrato transversal del servicio de bitacora (auditoria). Es el servicio
    // reusable del kernel: registra eventos y permite consultarlos. La persistencia
    // transaccional (con unidad de trabajo) es responsabilidad del caso de uso en BLL,
    // no de este contrato.
    public interface IBitacora
    {
        void RegistrarOperacion(string actividad, string informacion, string origen);

        void RegistrarFalloNegocio(string actividad, string informacion, string origen);

        void RegistrarExcepcionTecnica(string actividad, Exception exception, string origen);

        IList<IEventoBitacora> ConsultarBitacora(IFiltroBitacora filtro);
    }
}
