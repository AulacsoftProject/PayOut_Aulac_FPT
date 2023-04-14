using AutoMapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.DTO.VchPaymentHIS;

namespace PayOut_Aulac_FPT.Profiles
{
    public class VchPaymentHISProfile: Profile
    {
        public VchPaymentHISProfile()
        {
            CreateMap<VchPaymentHIS, VchPaymentHISDTO>();
        }
    }
}
