using Microsoft.Extensions.Configuration;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Services
{
    public class UserLoginService:BaseService<UserLogin>, IUserLoginService
    {
        private readonly IUserLoginRepository _repo;
        private readonly IConfiguration _configuration;
        public UserLoginService(IUserLoginRepository repo, IConfiguration configuration) :base(repo)
        {
            _repo = repo;
            _configuration = configuration;
        }

        public UserLogin? Login(string userID, string userPassword)
        {
            //string hashPassword = HashPassword(userPassword);
            return _repo.Get(new UserLogin { UserPaymentID = userID, UserPaymentPassWord = userPassword });
        }

        public string HashPassword(string password)
        {
            return UtilsHelper.HashPassword(password, _configuration["Password:Salt"]);
        }
    }
}
