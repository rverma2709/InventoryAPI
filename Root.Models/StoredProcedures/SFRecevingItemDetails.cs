using Root.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("RecevingItemDetails")]
    public class SFRecevingItemDetails
    {
        [QueryParam(dataType: "[dbo].[SeriolNumberData]")]
        public List<SeriolNumberData> SeriolNumberData { get; set; }
        [QueryParam(dataType: "[dbo].[RecivingItemList]")]
        public List<RecivingItemList> RecivingItemList { get; set; }
        [QueryParam]
        public long? BulkFileRecivingDetailId {  get; set; }
        [QueryParam]
        public long? InventoryUserId { get; set; }
    }
    public class RecivingItemList
    {
        public long? PoDetailId { get; set; }
        public long? PoItemDetilId { get; set; }
        public DateTime? RecivingDate { get; set; }
    }
    
    public class RecivingItemRetunData
    {
        public string SeriolNumber { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
