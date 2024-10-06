using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("OtpVarification")]
    public class SFOtpVarification
    {
        [QueryParam]
        public long? InventoryUserId { get; set; }
        [QueryParam]
        public long? OTP { get; set; }
    }
}
