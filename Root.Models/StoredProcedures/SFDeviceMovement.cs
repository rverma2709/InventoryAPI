using Root.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("DeviceMovement")]
    public class SFDeviceMovement
    {
        [QueryParam(dataType: "dbo.IDList")]
        public List<RequestById> RecivingDevices { get; set; }
        [QueryParam(dataType: "dbo.DeviceMovementList")]
        public List<DeviceMovementList>? DeviceMovementList { get; set; }
        [QueryParam]
        public long? ReciverInventoryRoleId {  get; set; }
        [QueryParam]
        public long? SenderInventoryRoleId { get; set; }

    }
    public class DeviceMovementList
    {
        public long? SenderUserId { get; set; }
        public long? ReciverUserId { get; set; }
        public DateTime? SendDate { get; set; }
        public bool? QCStatus { get; set; }
        public string? QCUserRemarks { get; set; }
        public string? DeviceOldserialNumber { get; set; }
    }
}
