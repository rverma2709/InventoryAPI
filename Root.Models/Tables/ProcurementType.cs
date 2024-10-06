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
    public class ProcurementType : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long ProcurementTypeId { get; set; }
        [DisplayName("Procurement Type")]
        public string? ProcurementNameType { get; set; }

    }
}
