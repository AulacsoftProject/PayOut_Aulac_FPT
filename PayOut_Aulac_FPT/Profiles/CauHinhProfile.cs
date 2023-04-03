using AutoMapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.DTO.CauHinh;

namespace PayOut_Aulac_FPT.Profiles
{
    public class CauHinhProfile : Profile
    {
        public CauHinhProfile()
        {
            CreateMap<CauHinh, CauHinhDTO>();
        }
    }
}
