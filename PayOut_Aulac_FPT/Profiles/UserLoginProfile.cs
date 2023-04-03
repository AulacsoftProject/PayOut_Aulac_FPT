using AutoMapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.DTO.UserLogin;

namespace PayOut_Aulac_FPT.Profiles
{
    public class UserLoginProfile:Profile
    {
        public UserLoginProfile() {
            CreateMap<UserLogin, UserLoginDTO>();
        }
    }
}
