using Aulac.Global.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Utils
{
    public static class UtilsHelper
    {
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

        public static string HashPassword(string password)//, string salt
        {
            //string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            //    password: password!,
            //    salt: Encoding.Unicode.GetBytes(salt),
            //    prf: KeyDerivationPrf.HMACSHA256,
            //    iterationCount: 100000,
            //    numBytesRequested: 256 / 8));
            string hashed = SecureHelper.EncodeString(password);
            return hashed;
        }
    }
}
