using AutoMapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.DTO.CompanyInfo;
using System.Globalization;

namespace PayOut_Aulac_FPT.Profiles
{
    public class CompanyInfoProfile:Profile
    {
        public CompanyInfoProfile() {
            CreateMap<CompanyInfo, CompanyInfoDTO>();
        }
    }
}
