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
    public class DeviceProcessorDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long DeviceProcessorDetailId { get; set; }
        public long DeviceTypeId { get; set; }
        [DisplayName("Company Name")]
        public string? ProcessorCompanyName { get; set; }
        [DisplayName("Processor Name")]
        public string? DeviceProcessorName { get; set; }
       
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }
    }
}
