using PayOut_Aulac_FPT.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public abstract class BaseEntity:IEntity
    {
        public Guid? TaoBoi { get; set; }
        public DateTime? TaoLuc { get; set; }
        public Guid? CapNhatBoi { get; set; }
        public DateTime? CapNhatLuc { get; set; }
    }
}
