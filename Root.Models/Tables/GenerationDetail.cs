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
    public class GenerationDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long GenerationDetailId { get; set; }
        public long DeviceTypeId { get; set; }
        [DisplayName("Generation Name")]
        public string? GenerationName { get; set; }
       

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }
    }
}
