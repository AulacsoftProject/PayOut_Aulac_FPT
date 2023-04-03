using AutoMapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.DTO.ConnectPayment;

namespace PayOut_Aulac_FPT.Profiles
{
    public class QRCodeProfile:Profile
    {
        public QRCodeProfile()
        {
            CreateMap<QRCode, QRCodeDTO>();
        }
    }
}
