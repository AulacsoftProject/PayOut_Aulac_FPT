using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Utils;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface IBaseService<T> where T : IEntity
    {
        public T Get(T entity);
        public IEnumerable<T> Gets(T entity);
        public IEnumerable<T> Gets(BaseSort? sort, BaseSearch? search);
        public IEnumerable<T> GetAll();
        public IEnumerable<T> GetPage(int pageNumber, int pageSize, BaseSort? sort, BaseSearch? search, out int totalRow);
        public Guid? Create(T entity);
        public bool Update(T entity);
        public bool Patch(T entity);
        public bool Delete(T entity);
    }
}
