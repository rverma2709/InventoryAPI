using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetPOItemList")]
    public class SFGetPOItemList
    {

    }
    public class PoItemData
    {
        public long PoItemDetilId { get; set; }
        public long PoDetailId { get; set; }
        public string? Text { get; set; }
        public long Quantity { get; set; }
        public long ItemRecivingQuantity {  get; set; }
        public long SerialNumberNotAvilable {  get; set; }
    }
}
