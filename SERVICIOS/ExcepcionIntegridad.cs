namespace SERVICIOS
{
    // Tipo de fallo de integridad detectado durante la verificacion.
    public enum TipoFalloIntegridad
    {
        // Una fila fue modificada o permutada por fuera del sistema (DVH no coincide).
        Horizontal,

        // Se agregaron o quitaron filas por fuera del sistema (DVV no coincide).
        Vertical
    }

    // Excepcion tecnica especifica para fallos de integridad de datos (T07).
    // Hereda de ExcepcionTecnica para integrarse con el manejo de excepciones
    // existente, pero permite capturar el caso de forma especifica en el arranque.
    public class ExcepcionIntegridad : ExcepcionTecnica
    {
        public ExcepcionIntegridad(TipoFalloIntegridad tipo, string tabla, ABSTRACCIONES.IIntegrable registro, string messageKey)
            : base(messageKey)
        {
            Tipo = tipo;
            Tabla = tabla;
            Registro = registro;
        }

        // Naturaleza del fallo (horizontal o vertical).
        public TipoFalloIntegridad Tipo { get; }

        // Tabla/entidad afectada.
        public string Tabla { get; }

        // Registro afectado cuando el fallo es horizontal.
        public ABSTRACCIONES.IIntegrable Registro { get; }
    }
}
