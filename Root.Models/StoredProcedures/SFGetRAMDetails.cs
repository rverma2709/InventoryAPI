using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetRAMDetails")]
    public class SFGetRAMDetails: SFParameters
    {
        public SFGetRAMDetails()
        {
            cols = "RAMDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? DeviceTypeId { get; set; }
        [QueryParam]
        public string? CompanyName { get; set; }
        [QueryParam]
        public string? RAMSize { get; set; }
    }
}
