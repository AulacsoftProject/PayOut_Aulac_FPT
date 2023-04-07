using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Utils.Enums
{
    /// <summary>
    /// Trạng thái thực hiện/thanh toán
    /// </summary>
    public enum EStatusReq
    {/// <summary>
     /// 0 - Không có
     /// </summary>
        None = 0,
        /// <summary>
        /// 1 - Yêu cầu thực hiện
        /// </summary>
        ReqExam = 1,
        /// <summary>
        /// 2 - Đang thực hiện
        /// </summary>
        Examming = 2,
        /// <summary>
        /// 3 - Đã hoàn thành
        /// </summary>
        Complated = 3,
        /// <summary>
        /// 4 - Không hoàn thành
        /// </summary>
        LostExam = 4,
        /// <summary>
        /// 5 - Đang cập nhật yêu cầu
        /// </summary>
        EdittingReq = 5
    }
}
