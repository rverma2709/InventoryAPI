using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetItemQuantity")]
    public class SFGetItemQuantity
    {
        [QueryParam]
        public long? PoItemDetilId { get; set; }
    }

    public class ItemQuantity
    {
        public long Quantity { get; set;}
        public long? RecivingQuantity { get; set; }
        public long? RemainingQuantity
        {
            get
            {
                return Quantity - (RecivingQuantity ?? 0);
            }
        }
    }
}
