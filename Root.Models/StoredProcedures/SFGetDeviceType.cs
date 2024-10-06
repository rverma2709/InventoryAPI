using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetDeviceType")]
    public class SFGetDeviceType: SFParameters
    {
        public SFGetDeviceType()
        {
            cols = "DeviceTypeId";
            order = "DESC";
        }
        [QueryParam]
        public string DeviceName { get; set; }
       
    }

    [StoredProcedureName("GetDeviceModeldetails")]
    public class SFGetDeviceModeldetails : SFParameters
    {
        public SFGetDeviceModeldetails()
        {
            cols = "DeviceTypeId";
            order = "DESC";
        }
        [QueryParam]
        public string ModelName { get; set; }

    }
}
