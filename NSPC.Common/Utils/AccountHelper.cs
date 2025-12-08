using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace NSPC.Common
{
    public class AccountHelper
    {
        public static string CreateEmailConfirmToken()
        {
            byte[] data = new byte[64];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
            }
            return Regex.Replace(Convert.ToBase64String(data), "[/]", ".");
        }

        public static string CreatePasswordSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public static string HashPassword(string password, string salt)
        {
            var salt1 = Convert.FromBase64String(salt);

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt1,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
