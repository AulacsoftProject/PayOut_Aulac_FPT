using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface ICauHinhService: IBaseService<CauHinh>
    {
        public EmailConfigInfo GetEmailConfig();
        public IEnumerable<CauHinh>? CheckExist(CauHinh entityAND, CauHinh entityOR);
        public IEnumerable<CauHinh>? GetExist(CauHinh entity);
        public CauHinh? GetDuyetDangKy();
        public CauHinh? GetDuyetNghiHan();
        public CauHinh? GetKieuHopDongNghiHan();
        public CauHinh? GetHienTrang();
        public CauHinh? GetThue();
    }
}
