using Dapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Utils.Models;
using PayOut_Aulac_FPT.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using System.Xml;
using PayOut_Aulac_FPT.Infrastructure.Helpers;

namespace PayOut_Aulac_FPT.Infrastructure.Repositories
{
    public class CauHinhRepository : BaseRepository<CauHinh>, ICauHinhRepository
    {
        public CauHinhRepository(IDbConnection dbConnection) : base(dbConnection, "CauHinh")
        {
        }

        protected override string GetDeleteString() => "p_CauHinh_Delete";

        protected override string GetSelectCountString() => "p_CauHinh_SelectCount";

        public EmailConfigInfo GetEmailConfig()
        {
            string where = "CauHinh.Ma Like 'EMAIL%'";
            var param = new DynamicParameters();
            if (where != null)
            {
                param.Add("Where", where);
            }
            var result = _dbConnection.Query<CauHinh>($"[p_CauHinh_Select]", param, commandType: CommandType.StoredProcedure);
            EmailConfigInfo config = new EmailConfigInfo(result.ToArray());
            return config;
        }
        public IEnumerable<CauHinh>? CheckExist(CauHinh entityAND, CauHinh entityOR)
        {
            string? whereAND = new WhereBuilder("CauHinh")
                .WithObj(entityAND)
                .WithMultiTable(true)
                .WithNot(new string[] { "Id" })
                .Build();
            string? whereOR = new WhereBuilder("CauHinh")
                .WithObj(entityOR)
                .WithMultiTable(true)
                .WithJoin(Helpers.Enums.EWhereJoin.OR)
                .Build();
            var param = new DynamicParameters();
            if (whereAND != null)
            {
                param.Add("Where", whereAND + " AND (" + whereOR + ")");
            }
            var result = _dbConnection.Query<CauHinh>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
            return result;
        }

        public IEnumerable<CauHinh>? GetExist(CauHinh entity)
        {
            string? where = new WhereBuilder("CauHinh")
                .WithObj(entity)
                .WithMultiTable(true)
                .WithJoin(Helpers.Enums.EWhereJoin.OR)
                .Build();
            var param = new DynamicParameters();
            if (where != null)
            {
                param.Add("Where", where);
            }
            var result = _dbConnection.Query<CauHinh>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
