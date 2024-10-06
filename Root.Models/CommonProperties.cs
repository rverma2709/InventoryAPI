using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
    public class CommonProperties
    {
        public CommonProperties()
        {
            Disabled = false;
            Crd = Lmd = DateTime.Now;
            CrdBy = LmdBy = 1;
            
        }

       
        [ScaffoldColumn(false)]
        [DisplayName("Disable")]
        public bool? Disabled { get; set; }

       
        [ScaffoldColumn(false)]
        [DisplayName("Date of Creation")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Crd { get; set; }

        
        [ScaffoldColumn(false)]
        [DisplayName("Created By")]
        [DataType("UserNames")]
        public long? CrdBy { get; set; }

       
        [ScaffoldColumn(false)]
        [DisplayName("Last Modified Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Lmd { get; set; }

        
        [ScaffoldColumn(false)]
        [DataType("UserNames")]
        [DisplayName("Last Modified By")]
        public long? LmdBy { get; set; }

    }
}
