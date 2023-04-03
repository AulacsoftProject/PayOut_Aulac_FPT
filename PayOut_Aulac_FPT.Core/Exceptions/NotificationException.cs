using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Exceptions
{
    public class NotificationException: Exception
    {
        public NotificationException(string notification): base(notification) { }
    }
}
