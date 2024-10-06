using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetUserDetails")]
    public class SFGetUserDetails
    {
        [QueryParam]
        public long? InventoryRoleId { get; set; }
    }
    public class GetUserdata
    {
        public long? ReciverUserId { get; set; }
        public string? Text { get; set; }
    }
}
