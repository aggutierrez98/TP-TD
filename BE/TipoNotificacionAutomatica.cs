namespace BE
{
    // Tipo de aviso que genera el proceso automatico diario. Se persiste como texto
    // (ToString) en Notificacion.TipoAutomatico; los ejemplos de laboratorio no lo usan.
    public enum TipoNotificacionAutomatica
    {
        Compras,
        Inversiones,
        Movimientos
    }
}
