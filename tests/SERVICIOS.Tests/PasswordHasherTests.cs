using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class PasswordHasherTests
    {
        private readonly PasswordHasher _hasher = new PasswordHasher();

        [TestMethod]
        public void HashPassword_ValidPassword_ReturnsNonPlainHash()
        {
            string hash = _hasher.HashPassword("Password123!");

            Assert.IsFalse(string.IsNullOrWhiteSpace(hash));
            Assert.AreNotEqual("Password123!", hash);
        }

        [TestMethod]
        public void HashPassword_SamePasswordTwice_ReturnsDifferentHashes()
        {
            string first = _hasher.HashPassword("Password123!");
            string second = _hasher.HashPassword("Password123!");

            Assert.AreNotEqual(first, second);
        }

        [TestMethod]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            string hash = _hasher.HashPassword("Password123!");

            bool result = _hasher.VerifyPassword("Password123!", hash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            string hash = _hasher.HashPassword("Password123!");

            bool result = _hasher.VerifyPassword("WrongPassword", hash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerifyPassword_InvalidStoredHash_ThrowsFormatException()
        {
            Assert.ThrowsExactly<FormatException>(() => _hasher.VerifyPassword("Password123!", "not-base64"));
        }
    }
}
