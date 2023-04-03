using PayOut_Aulac_FPT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface IUserLoginService:IBaseService<UserLogin>
    {
        public UserLogin Login(string? userID, string? userPassword);
        public string HashPassword(string userPassword);
    }
}
