using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetBrandDetails")]
    public class SFGetBrandDetails : SFParameters
    {
        public SFGetBrandDetails()
        {
            cols = "BrandDetailId";
            order = "DESC";
        }
        [QueryParam]
        public string BrandName { get; set; }
        [QueryParam]
        public long? DeviceTypeId { get; set; }

    }
}
