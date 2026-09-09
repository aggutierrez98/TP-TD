namespace ABSTRACCIONES
{
    // En ABSTRACCIONES (no en BE) porque lo referencian tanto BE.Movimiento como
    // SERVICIOS.FiltroMovimiento, y SERVICIOS no puede depender de BE.
    public enum TipoMovimiento
    {
        Ingreso,
        Egreso
    }
}
