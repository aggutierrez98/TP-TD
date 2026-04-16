namespace BE
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public Usuario() { }

        public Usuario(string username)
        {
            Username = username;
        }
    }
}
