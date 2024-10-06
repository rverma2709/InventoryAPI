using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetPoNumberItemDetilas")]
    public class SFGetPoNumberItemDetilas
    {
        [QueryParam]
        public long? PoDetailId {  get; set; }
    }

    
    public class GetPoNumberItemList
    {
        public long? PoItemDetilId { get; set; }
        public string? Text { get; set; }
        public long? Quantity { get; set; }
    }
}
