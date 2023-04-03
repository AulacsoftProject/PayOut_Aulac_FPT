using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface IMinioService
    {
        public Task<string> UploadFile(string path, string contentType, string extension, string name, string? parent);
    }
}
