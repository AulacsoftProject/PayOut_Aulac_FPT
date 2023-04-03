using PayOut_Aulac_FPT.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface IBase64Service
    {
        public string Base64Encode(string plainText);
        public string Base64Decode(string base64EncodedData);
    }
}
