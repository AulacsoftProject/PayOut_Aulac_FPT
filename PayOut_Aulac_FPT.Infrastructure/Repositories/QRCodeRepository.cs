using Dapper;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PayOut_Aulac_FPT.Infrastructure.Repositories
{
    public class QRCodeRepository:BaseRepository<QRCode>, IQRCodeRepository
    {
        public QRCodeRepository(IDbConnection connection) : base(connection, "MdcFile_PntExam") { }

        public QRCode? Get_QRCode(QRCode entity)
        {
            //string? where = DBQueryHelper.CreateWhere(entity, false, _nameTable);
            string? where = new WhereBuilder("MdcFile_PntExam")
                .WithObj(entity)
                .WithMultiTable(true)
                .Build();

            if (where.Contains("VchPaymentFoxpay.StatusReq = N'1'"))
            {
                string trangThai = where.Substring(where.IndexOf("VchPaymentFoxpay.StatusReq = N'1'"));
                trangThai = trangThai.Contains(" AND ") ? trangThai.Substring(0, trangThai.IndexOf(" AND ")) : trangThai;
                string trangThaiThayThe = trangThai.Replace("VchPaymentFoxpay.StatusReq = N'1'", "VchPaymentFoxpay.StatusReq = N'2'");
                trangThaiThayThe = "(" + trangThai + " OR " + trangThaiThayThe + ")";

                where = where.Replace(trangThai, trangThaiThayThe);
            }

            var param = new DynamicParameters();
            if (where != null)
            {
                param.Add("Where", where);
            }
            var result = _dbConnection.QueryFirstOrDefault<QRCode>(GetSelectString(), param, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
