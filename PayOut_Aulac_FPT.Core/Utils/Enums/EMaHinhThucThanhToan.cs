using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Utils.Enums
{
    /// <summary>
    /// Mã loại thanh toán
    /// </summary>
    public enum EMaHinhThucThanhToan
    {
        /// <summary>
        /// 0 - Không có
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 1 - Tiền mặt
        /// </summary>
        CASH = 1,
        /// <summary>
        /// 2 - Chuyển khoản
        /// </summary>
        TRANSFER = 2,
        /// <summary>
        /// 3 - Tiền mặt/Chuyển khoản
        /// </summary>
        CASH_OR_TRANSFER = 3,
        /// <summary>
        /// Thẻ tín dụng
        /// </summary>
        CREDIT_CARD = 4,
        /// <summary>
        /// Đối trừ công nợ
        /// </summary>
        CLEARING_DEBTS = 5
    }
}
