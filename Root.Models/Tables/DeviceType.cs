using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class DeviceType : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long DeviceTypeId { get; set; }
        [DisplayName("Device Type Name")]
        public string? DeviceName { get; set; }
    }
}
