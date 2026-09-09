namespace BE
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        // NULL = categoria global (precargada, visible para todos los usuarios).
        // Con valor = categoria propia de ese usuario.
        public int? UsuarioId { get; set; }

        public bool EsGlobal
        {
            get { return !UsuarioId.HasValue; }
        }
    }
}
