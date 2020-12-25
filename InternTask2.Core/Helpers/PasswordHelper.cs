using PasswordGenerator;
using System;
using System.Security.Cryptography;
using System.Text;

namespace InternTask2.Core.Helpers
{
    public static class PasswordHelper
    {
        private static readonly Password passwordGenerator;

        static PasswordHelper()
        {
            passwordGenerator = new Password(
                        includeUppercase: true,
                        includeNumeric: true,
                        includeLowercase: true,
                        includeSpecial: false,
                        passwordLength: 10);
        }

        public static string GetHashedPassword(string login, string password) 
            => Convert.ToBase64String( new MD5CryptoServiceProvider()
                .ComputeHash(new UTF8Encoding().GetBytes(String.Concat(password,login))));

        public static string GetRandomPassword() 
            => passwordGenerator.Next();
    }
}