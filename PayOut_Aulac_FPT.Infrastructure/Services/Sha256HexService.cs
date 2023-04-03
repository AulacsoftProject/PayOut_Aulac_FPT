using Newtonsoft.Json.Linq;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Infrastructure.Services
{
    public class Sha256HexService : ISha256HexService
    {
        public string SHA256Hex(string StringIn)
        {
            string hashString;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.Default.GetBytes(StringIn));
                StringBuilder result = new StringBuilder(hash.Length * 2);

                for (int i = 0; i < hash.Length; i++)
                    result.Append(hash[i].ToString("x2"));

                hashString = result.ToString();
            }
            return hashString;
        }
    }
}
