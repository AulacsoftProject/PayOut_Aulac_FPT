using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface ISha256HexService
    {
        public string SHA256Hex(string StringIn);
    }
}
