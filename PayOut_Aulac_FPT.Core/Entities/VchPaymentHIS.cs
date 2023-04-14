﻿using PayOut_Aulac_FPT.Core.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class VchPaymentHIS:BaseEntity
    {
        [PrimaryKey]
        public int? VchPmntPrkID { get; set; }
        public DateTime? VchDate { get; set; }
        public int? MdcFilePrkID { get; set; }
        public int? StatusReq { get; set; }
    }
}
