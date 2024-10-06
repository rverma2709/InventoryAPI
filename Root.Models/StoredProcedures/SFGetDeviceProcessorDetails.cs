using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetDeviceProcessorDetails")]
    public class SFGetDeviceProcessorDetails : SFParameters
    {
        public SFGetDeviceProcessorDetails()
        {
            cols = "DeviceProcessorDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? DeviceTypeId { get; set; }
        [QueryParam]
        public string? ProcessorCompanyName { get; set; }
        [QueryParam]
        public string? DeviceProcessorName { get; set; }
    }
}
