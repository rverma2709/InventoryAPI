using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class PoDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long PoDetailId { get; set; }

        
        public string PoNumber {  get; set; }

        public long? DeviceTypeId { get; set; }
        public long? InventoryUserId { get; set; }
        public long? ProcurementTypeId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? RentStartDate { get; set; }
        public long? Tenure { get; set; }
        public long? Warranty { get; set; }
        public long? TotalItems { get; set; }
        public long? TotalQuantity { get; set; }
        public long? ReciveQuantity { get; set; }
        public double? ActualAmount { get; set; }
        public bool? DiscountType { get; set; } 
        public double? Discount { get; set; }
        public double? SGST { get; set; }
        public double? CGST { get; set; }
        public double? TaxableAmount { get; set; }
        //public double? TotalAmount { get; set; }
        public string SignatureName { get; set; }
        public string Note { get; set; }
        public string TermAndConditions { get; set; }
    }
}
