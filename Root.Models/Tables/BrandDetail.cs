using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Root.Models.Tables
{
    public class BrandDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long BrandDetailId { get; set; }
        public long DeviceTypeId { get; set; }
        [DisplayName("Brand Name")]
        public string? BrandName { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType?  DeviceType { get; set; }
    }
}
