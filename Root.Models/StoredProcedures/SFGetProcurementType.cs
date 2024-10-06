using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetProcurementType")]
    public class SFGetProcurementType : SFParameters
    {
        public SFGetProcurementType()
        {
            cols = "ProcurementTypeId";
            order = "DESC";
        }
        
        [QueryParam]
        public string? ProcurementNameType { get; set; }
        
    }
}
