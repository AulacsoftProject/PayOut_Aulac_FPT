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
    public class UserLoginRepository:BaseRepository<UserLogin>, IUserLoginRepository
    {
        public UserLoginRepository(IDbConnection dbConnection) : base(dbConnection, "Login_Payment") { }
    }
}
