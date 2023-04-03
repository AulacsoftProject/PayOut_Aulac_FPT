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
    public class CompanyInfoRepository:BaseRepository<CompanyInfo>, ICompanyInfoRepository
    {
        public CompanyInfoRepository(IDbConnection dbConnection) : base(dbConnection, "Sys_CompanyInfo") { }
    }
}
