using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class DeviceMovementView
    {
        public long? ReciverInventoryRoleId { get;set;}
        public long? SenderInventoryRoleId { get;set;}
        public long? ReceivingDeviceDetailId { get; set; }
        public long? SenderUserId { get; set; }
        [Required]
        public long? ReciverUserId { get; set; }
        [Required]
        public DateTime? SendDate { get; set; }
        public bool? QCStatus { get; set; }
        public string? QCUserRemarks { get; set; }
        public string? DeviceOldserialNumber { get; set; }
        [ForeignKey("ReciverInventoryRoleId")]
        public virtual InventoryRole? InventoryRole { get; set; }
    }
    public class RecivingDevices()
    {
        public long? Id { get; set; }
    }
}
