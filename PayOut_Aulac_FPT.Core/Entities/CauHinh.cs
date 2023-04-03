using PayOut_Aulac_FPT.Core.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class CauHinh : BaseEntity
    {
        [PrimaryKey]
        public Guid? Id { get; set; }
        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public string? GiaTri { get; set; }
        public string? MoTa { get; set; }
    }
}
