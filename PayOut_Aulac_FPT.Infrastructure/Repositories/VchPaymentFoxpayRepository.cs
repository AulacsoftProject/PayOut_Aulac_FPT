using Dapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Infrastructure.Repositories
{
    public class VchPaymentFoxpayRepository:BaseRepository<VchPaymentFoxpay>, IVchPaymentFoxpayRepository
    {
        public VchPaymentFoxpayRepository(IDbConnection dbConnection) : base(dbConnection, "VchPaymentFoxpay") { }

        public IEnumerable<VchPaymentFoxpay>? CheckExist(VchPaymentFoxpay entity)
        {
            string? where = new WhereBuilder("VchPaymentFoxpay")
                .WithObj(entity)
                .WithMultiTable(true)
                .WithNot(new string[] { "VchPmtFoxPayPrkID" })
                .Build();
            
            var result = _dbConnection.Query<VchPaymentFoxpay>(GetSelectString(), new { Where = where}, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
