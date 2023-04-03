using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Services
{
    public class QRCodeService:BaseService<QRCode>, IQRCodeService
    {
        private readonly IQRCodeRepository _repo;
        public QRCodeService(IQRCodeRepository repo):base(repo)
        {
            _repo = repo;
        }
    }
}
