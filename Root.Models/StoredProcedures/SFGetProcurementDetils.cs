using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetProcurementDetils")]
    public class SFGetProcurementDetils:SFParameters
    {
        public SFGetProcurementDetils()
        {
            cols = "InventoryProcurementDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? DeviceTypeId { get; set; }
    }
}
