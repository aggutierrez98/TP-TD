namespace ABSTRACCIONES
{
    // Categorias cerradas que devuelve un IClasificadorConsultas. Solo Compras, Inversiones
    // y FinanzasGenerales son admitidas; FueraDeTema e Inyeccion se rechazan siempre.
    public enum CategoriaConsulta
    {
        Compras,
        Inversiones,
        FinanzasGenerales,
        FueraDeTema,
        Inyeccion
    }
}
