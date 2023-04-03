﻿using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Services
{
    public class CompanyInfoService:BaseService<CompanyInfo>, ICompanyInfoService
    {
        private readonly ICompanyInfoRepository _repo;
        public CompanyInfoService(ICompanyInfoRepository repo):base(repo)
        {
            _repo = repo;
        }
    }
}
