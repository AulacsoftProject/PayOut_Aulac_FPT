using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Repositories
{
    public interface ICauHinhRepository : IBaseRepository<CauHinh>
    {
        public EmailConfigInfo GetEmailConfig();
        public IEnumerable<CauHinh>? CheckExist(CauHinh entityAND, CauHinh entityOR);
        public IEnumerable<CauHinh>? GetExist(CauHinh entity);
    }
}
