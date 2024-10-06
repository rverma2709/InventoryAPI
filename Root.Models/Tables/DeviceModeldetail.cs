using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Root.Models.Tables
{
    public class DeviceModeldetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long DeviceModeldetailId { get; set; }
        public long DeviceTypeId { get; set; }

        [DisplayName("Device Model Name")]
        public string? ModelName { get; set; }

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }


    }
}
