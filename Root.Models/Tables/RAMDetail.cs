using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class RAMDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long RAMDetailId { get; set; }
        public long DeviceTypeId { get; set; }
        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }

        [DisplayName("RAM Size")]
        public string? RAMSize { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }
    }
}
