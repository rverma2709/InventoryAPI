using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class DeviceTypeData
    {
        public long? DeviceTypeId { get; set; }
        [DisplayName("Device Type Name")]
        public string? DeviceName { get; set; }
    }

    public class DeviceModelTypeData
    {
        public long? DeviceModeldetailId { get; set; }

        [Required]
        public long? DeviceTypeId { get; set; }

        [Required]
        [DisplayName("Device Model Name")]
        public string? ModelName { get; set; }
    }
}