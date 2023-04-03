using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Services
{
    public class CauHinhService : BaseService<CauHinh>, ICauHinhService
    {
        private readonly ICauHinhRepository _repo;
        public CauHinhService(ICauHinhRepository repo) : base(repo)
        {
            _repo = repo;
        }

        public CauHinh? GetDuyetDangKy()
        {
            string Ma = "BYPASS_DUYET_DANGKY";
            return _repo.Get(new CauHinh() { Ma = Ma });
        }

        public EmailConfigInfo GetEmailConfig()
        {
            return _repo.GetEmailConfig();
        }
        public IEnumerable<CauHinh>? CheckExist(CauHinh entityAND, CauHinh entityOR)
        {
            return _repo.CheckExist(entityAND, entityOR);
        }
        public IEnumerable<CauHinh>? GetExist(CauHinh entity)
        {
            return _repo.GetExist(entity);
        }

        public CauHinh? GetDuyetNghiHan()
        {
            string Ma = "BYPASS_DUYET_NGHIHAN";
            return _repo.Get(new CauHinh() { Ma = Ma });
        }
        public CauHinh? GetKieuHopDongNghiHan()
        {
            string Ma = "KIEUHOPDONG_NGHIHAN";
            return _repo.Get(new CauHinh() { Ma = Ma });
        }
        public CauHinh? GetHienTrang()
        {
            string Ma = "MATBANG_TRONG";
            return _repo.Get(new CauHinh() { Ma = Ma });
        }

        public CauHinh? GetThue()
        {
            string Ma = "THUE";
            return _repo.Get(new CauHinh() { Ma = Ma });
        }
    }
}
