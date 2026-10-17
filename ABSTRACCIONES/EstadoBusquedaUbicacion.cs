namespace ABSTRACCIONES
{
    // Causas tecnicas por las que una busqueda no pudo completarse. La
    // ausencia de coincidencias NO es un estado de error: se informa como lista
    // vacia. La cancelacion voluntaria tampoco: se propaga como
    // OperationCanceledException. La UI mapea cada valor a una key traducible.
    public enum EstadoBusquedaUbicacion
    {
        ConexionFallida,
        TiempoAgotado,
        LimiteExcedido,
        RespuestaInvalida,
        ConfiguracionInvalida
    }
}
