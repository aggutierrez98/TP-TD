namespace ABSTRACCIONES
{
    public interface IUsuarioPreferenciaRepository
    {
        IUsuarioPreferencia ObtenerPorUsuarioId(int usuarioId);

        bool Guardar(IUsuarioPreferencia preferencia);
    }
}
