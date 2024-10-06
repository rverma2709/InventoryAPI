using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetGenerationDetails")]
    public class SFGetGenerationDetails : SFParameters
    {
        public SFGetGenerationDetails()
        {
            cols = "GenerationDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? DeviceTypeId { get; set; }
        [QueryParam]
        public string? GenerationName { get; set; }
       
    }
}
