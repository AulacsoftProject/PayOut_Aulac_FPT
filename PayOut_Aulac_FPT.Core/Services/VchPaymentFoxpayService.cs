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
    public class VchPaymentFoxpayService:BaseService<VchPaymentFoxpay>, IVchPaymentFoxpayService
    {
        private readonly IVchPaymentFoxpayRepository _repo;
        public VchPaymentFoxpayService(IVchPaymentFoxpayRepository repo):base(repo)
        {
            _repo = repo;
        }
        public IEnumerable<VchPaymentFoxpay>? CheckExist(VchPaymentFoxpay entity)
        {
            return _repo.CheckExist(entity);
        }
    }
}
