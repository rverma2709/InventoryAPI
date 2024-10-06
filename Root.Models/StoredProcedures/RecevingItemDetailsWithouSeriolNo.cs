using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("RecevingItemDetailsWithouSeriolNo")]
    public class SFRecevingItemDetailsWithouSeriolNo
    {
        [QueryParam(dataType: "[dbo].[RecivingItemList]")]
        public List<RecivingItemList> RecivingItemList { get; set; }
        [QueryParam]
        public long RecivingCount { get; set; }
        [QueryParam]
        public long InventoryUserId { get; set; }
    }
}
