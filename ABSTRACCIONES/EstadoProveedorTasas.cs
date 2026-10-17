namespace ABSTRACCIONES
{
    // Causas por las que el proveedor de tasas de cambio no pudo devolver datos. El
    // llamador las registra en el historial sin interpretar mensajes. La cancelacion
    // voluntaria no es un estado: se propaga como OperationCanceledException.
    public enum EstadoProveedorTasas
    {
        ClaveFaltante,
        ClaveInvalida,
        LimiteExcedido,
        Rechazado,
        ConexionFallida,
        RespuestaInvalida
    }
}
