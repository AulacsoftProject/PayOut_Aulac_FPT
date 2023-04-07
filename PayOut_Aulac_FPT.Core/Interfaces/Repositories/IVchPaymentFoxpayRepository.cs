﻿using PayOut_Aulac_FPT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Repositories
{
    public interface IVchPaymentFoxpayRepository:IBaseRepository<VchPaymentFoxpay>
    {
        public IEnumerable<VchPaymentFoxpay>? CheckExist(VchPaymentFoxpay entity);
    }
}
