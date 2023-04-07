using AutoMapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.DTO.VchPaymentFoxpay;

namespace PayOut_Aulac_FPT.Profiles
{
    public class VchPaymentFoxpayProfile:Profile
    {
        public VchPaymentFoxpayProfile()
        {
            CreateMap<VchPaymentFoxpay, VchPaymentFoxpayDTO>();
        }
    }
}
