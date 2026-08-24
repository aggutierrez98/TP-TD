using SERVICIOS;

namespace BLL
{
    internal class PasswordHashService : IPasswordHashService
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHashService()
        {
            _passwordHasher = new PasswordHasher();
        }

        public string HashPassword(string passwordRaw)
        {
            return _passwordHasher.HashPassword(passwordRaw);
        }

        public bool VerifyPassword(string passwordRaw, string passwordHash)
        {
            return _passwordHasher.VerifyPassword(passwordRaw, passwordHash);
        }
    }
}
