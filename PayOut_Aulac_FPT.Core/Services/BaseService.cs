using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Utils;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Services
{
    public class BaseService<T> : IBaseService<T> where T : IEntity
    {

        private readonly IBaseRepository<T> _repo;
        public BaseService(IBaseRepository<T> repo)
        {
            _repo = repo;
        }

        public virtual Guid? Create(T entity)
        {
            return _repo.Create(entity);
        }

        public virtual bool Delete(T entity)
        {
            return _repo.Delete(entity);
        }

        public virtual T? Get(T entity)
        {
            return _repo.Get(entity);
        }

        public virtual IEnumerable<T> Gets(T entity)
        {
            return _repo.Gets(entity);
        }

        public virtual IEnumerable<T> Gets(BaseSort? sort, BaseSearch? search)
        {
            var sortConfig = new SortConfig<T>(sort);
            var searchConfig = new SearchConfig<T>(search);
            return _repo.Gets(sortConfig, searchConfig);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _repo.GetAll();
        }

        public virtual IEnumerable<T> GetPage(int pageNumber, int pageSize, BaseSort? sort, BaseSearch? search, out int totalRow)
        {
            var sortConfig = new SortConfig<T>(sort);
            var searchConfig = new SearchConfig<T>(search);
            return _repo.GetPage(pageNumber, pageSize, sortConfig, searchConfig, out totalRow);
        }

        public virtual bool Update(T entity)
        {
            return _repo.Update(entity);
        }

        public virtual bool Patch(T entity)
        {
            return _repo.Patch(entity);
        }
    }
}
