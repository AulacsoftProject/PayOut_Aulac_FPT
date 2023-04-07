using Dapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Utils;
using PayOut_Aulac_FPT.Core.Utils.Attributes;
using PayOut_Aulac_FPT.Infrastructure.Utils;
using System.Reflection;
using System.Data;
using System.Xml;
using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Infrastructure.Helpers;

namespace PayOut_Aulac_FPT.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : IEntity
    {
        protected IDbConnection _dbConnection;
        private readonly string _nameTable;
        protected virtual string GetInsertString() => $"[SP_{_nameTable}_Insert]";

        protected virtual string GetUpdateString() => $"[SP_{_nameTable}_Update]";

        protected virtual string GetDeleteString() => $"[SP_{_nameTable}_Delete]";

        protected virtual string GetSelectString() => $"[SP_{_nameTable}_Select]";

        protected virtual string GetSelectPageString() => $"[SP_{_nameTable}_SelectPage]";

        protected virtual string GetSelectCountString() => $"[SP_{_nameTable}_SelectCount]";

        public BaseRepository(IDbConnection dbConnection, string? nameTable = null)
        {
            _dbConnection = dbConnection;
            _nameTable = nameTable ?? typeof(T).Name;
        }

        public virtual string? Create(T entity)
        {
            var param = DBQueryHelper.GetParamOnlyTable(entity, false);
            string? idName = DBQueryHelper.GetPrimaryKeyName<T>();
            if (string.IsNullOrWhiteSpace(idName))
            {
                throw new ArgumentNullException(nameof(idName));
            }
            param.Add(idName, dbType: DbType.Decimal, direction: ParameterDirection.Output);
            _dbConnection.Query<T>(GetInsertString(), param, commandType: CommandType.StoredProcedure);
            Decimal? id = param.Get<Decimal>(idName);
            return id.ToString();
        }

        public virtual bool Update(T entity)
        {
            var param = DBQueryHelper.GetParamOnlyTable(entity);
            int row = _dbConnection.Execute(GetUpdateString(), param, commandType: CommandType.StoredProcedure);
            return true;
        }

        public virtual bool Delete(T entity)
        {
            //string? where = DBQueryHelper.CreateWhere(entity, false, _nameTable, false);
            string? where = new WhereBuilder(_nameTable)
                .WithObj(entity)
                .WithMultiTable(false)
                .Build();
            var param = new DynamicParameters();
            if (where != null)
            {
                param.Add("Where", where);
            }
            _dbConnection.Execute(GetDeleteString(), param, commandType: CommandType.StoredProcedure);
            return true;
        }

        public virtual T? Get(T entity)
        {
            //string? where = DBQueryHelper.CreateWhere(entity, false, _nameTable);
            string? where = new WhereBuilder(_nameTable)
                .WithObj(entity)
                .WithMultiTable(true)
                .Build();
            var param = new DynamicParameters();
            if (where != null)
            {
                param.Add("Where", where);
            }
            var result = _dbConnection.QueryFirstOrDefault<T>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
            return result;
        }

        public virtual IEnumerable<T> Gets(T entity)
        {
            string? where = new WhereBuilder(_nameTable)
                .WithObj(entity)
                .WithMultiTable(true)
                .Build();
            var param = new DynamicParameters();
            if (where != null)
            {
                param.Add("Where", where);
            }
            return _dbConnection.Query<T>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
        }

        public virtual IEnumerable<T> Gets(SortConfig<T>? sortConfig, SearchConfig<T>? searchConfig)
        {
            var param = new DynamicParameters();
            string? where = searchConfig?.GetWhere(_nameTable);
            if (where != null)
            {
                param.Add("Where", where);
            }
            string? orderBy = sortConfig?.GetSort();
            if (orderBy != null)
            {
                param.Add("OrderBy", orderBy);
            }
            return _dbConnection.Query<T>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbConnection.Query<T>(GetSelectString(), commandType: CommandType.StoredProcedure);
        }

        public virtual IEnumerable<T> GetPage(int pageNumber, int pageSize, SortConfig<T>? sortConfig, SearchConfig<T>? searchConfig, out int totalRow)
        {
            var param = new DynamicParameters();
            param.Add("PageNumber", pageNumber);
            param.Add("PageSize", pageSize);
            //string? where = DBQueryHelper.CreateWhereByDictionary(searchConfig?.GetSearch(), true, Helpers.Enums.EWhereJoin.OR);
            string? where = searchConfig?.GetWhere(_nameTable);
            if (where != null)
            {
                param.Add("Where", where);
            }
            string? orderBy = sortConfig?.GetSort();
            if (orderBy != null)
            {
                param.Add("OrderBy", orderBy);
            }

            var result = _dbConnection.Query<T>(GetSelectPageString(), param, commandType: CommandType.StoredProcedure);
            totalRow = _dbConnection.QueryFirstOrDefault<int>(GetSelectCountString(), new { Where = where }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public virtual bool Patch(T entity)
        {
            string? idName = DBQueryHelper.GetPrimaryKeyName<T>();
            if (idName == null)
            {
                return false;
            }

            var param = new DynamicParameters();
            var typeT = typeof(T);
            string where = string.Format("{0}.{1} = '{2}'", _nameTable, idName, typeT.GetProperty(idName)?.GetValue(entity));

            param.Add("Where", where);

            T oldData = _dbConnection.QueryFirstOrDefault<T>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
            if (oldData == null)
            {
                return false;
            }

            var properties = typeT.GetProperties();
            var paramNew = new DynamicParameters();
            foreach (var property in properties)
            {
                var tableForeign = property.GetCustomAttribute<TableForeignAttribute>();
                if (tableForeign == null)
                {
                    var newValue = typeT.GetProperty(property.Name)?.GetValue(entity);
                    if (newValue != null)
                    {
                        paramNew.Add(property.Name, newValue);
                    }
                    else
                    {
                        var oldValue = typeT.GetProperty(property.Name)?.GetValue(oldData);
                        paramNew.Add(property.Name, oldValue);
                    }
                }
            }
            int row = _dbConnection.Execute(GetUpdateString(), paramNew, commandType: CommandType.StoredProcedure);
            return true;

        }
    }
}
