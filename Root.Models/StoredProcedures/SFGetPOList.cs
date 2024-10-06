using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetPOList")]
    public class SFGetPOList
    {
    }
    public class Polist
    {
        public long PoDetailId { get; set; }
        public string? PoNumber { get; set; }
        public string? DeviceName { get; set; }
        public string? ProcurementNameType { get; set; }
        public long TotalQuantity { get; set; }
        public float TotalAmount { get; set; }
        public long ReciveQuantity {  get; set; }
    }

    public class PoListAndPoItemData: Polist
    {
        public List<PoItemData> poItemDatas { get; set; }
    }
}
