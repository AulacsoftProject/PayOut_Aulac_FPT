using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Infrastructure.Repositories
{
    public class VchPaymentHISRepository:BaseRepository<VchPaymentHIS>, IVchPaymentHISRepository
    {
        public VchPaymentHISRepository(IDbConnection dbConnection) : base(dbConnection, "VchPaymentHeader") { }

        protected override string GetSelectString() => "SP_VchPaymentHeader_Select";
        protected override string GetUpdateString() => "SP_VchPaymentHeader_Update";
    }
}
