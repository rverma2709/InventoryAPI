using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetVendorDetails")]
    public class SFGetVendorDetails : SFParameters
    {
        public SFGetVendorDetails()
        {
            cols = "InventoryUserId";
            order = "DESC";
        }
        
        [QueryParam]
        public string? VendorName { get; set; }
        [QueryParam]
        public string? CompanyName { get; set; }
        [QueryParam]
        public string? ContactNo { get; set; }
        [QueryParam]
        public string? EmailId { get; set; }
        [QueryParam]
        public string? GSTNo { get; set; }
    }
}
