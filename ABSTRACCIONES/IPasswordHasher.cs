using System;

namespace ABSTRACCIONES
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
    }
}
