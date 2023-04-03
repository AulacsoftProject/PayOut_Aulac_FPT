using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Core.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class UserLogin:IEntity
    {
        [PrimaryKey]
        public string? UserPaymentPrkID { get; set; }
        public string? UserPaymentID { get; set; }
        public string? UserPaymentPassWord { get; set; }
        public string? UserPaymentName { get; set; }
        public Guid? merchant_id { get; set; }
        public Guid? terminal_id { get; set; }
        public string? secret_key { get; set; }
        public bool? IsActive { get; set; }
    }
}
