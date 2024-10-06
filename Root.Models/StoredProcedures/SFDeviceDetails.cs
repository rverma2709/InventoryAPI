using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("DeviceDetails")]
    public class SFDeviceDetails
    {
    }
    public class DeviceList
    {
        public long ReceivingDeviceDetailId { get; set; }
       
        public string BrandName { get; set; }
        public string ModelName { get; set; }
       
        public string Barcode { get; set; }
        public string SerialNumber { get; set; }
        public bool QCStatus { get; set; }
        public string ProcurementType { get; set; }
        public string Text { get; set; }
    }
}
