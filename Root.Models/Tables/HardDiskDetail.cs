using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Root.Models.Tables
{
    public class HardDiskDetail : CommonProperties 
    {
        [Key]
        [ScaffoldColumn(false)]
        public long HardDiskDetailId { get; set; }
        public long DeviceTypeId { get; set; }
        [DisplayName("Company Name")]
        public string? HardDiskCompanyName { get; set; }
        [DisplayName("HardDisk Type")]
        public string? HardDiskType { get; set; }
        [DisplayName("HardDisk Size")]
        public string? HardDiskSize { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }
    }
}
