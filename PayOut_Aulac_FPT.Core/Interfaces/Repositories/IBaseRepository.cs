using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Utils;
using PayOut_Aulac_FPT.Core.Utils.Models;

namespace PayOut_Aulac_FPT.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : IEntity
    {
        public T? Get(T entity);
        public IEnumerable<T> Gets(T entity);
        public IEnumerable<T> Gets(SortConfig<T>? sortConfig, SearchConfig<T>? searchConfig);
        public IEnumerable<T> GetAll();
        public IEnumerable<T> GetPage(int pageNumber, int pageSize, SortConfig<T>? sortConfig, SearchConfig<T>? searchConfig, out int totalRow);
        public Guid? Create(T entity);
        public bool Update(T entity);
        public bool Patch(T entity);
        public bool Delete(T entity);
    }
}
