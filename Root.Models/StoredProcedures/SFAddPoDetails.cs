using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("AddPoDetails")]
    public class SFAddPoDetails
    {
        [QueryParam(dataType: "[dbo].[PoDetailList]")]
        public List<PoDetailList> PoDetailList { get; set; }
        [QueryParam(dataType: "[dbo].[PoItemDetilList]")]
        public List<PoItemDetilList> PoItemDetilList { get; set; }
    }

    public class PoDetailList
    {
        public long? DeviceTypeId { get; set; }
        public long? VendorDetailId { get; set; }
        public long? ProcurementTypeId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? RentStartDate { get; set; }
        public long? Tenure { get; set; }
        public long? Warranty { get; set; }
        public float? ActualAmount { get; set; }
        public float? TaxableAmount { get; set; }
        public float? TotalAmount { get; set; }
        public string? DiscountType { get; set; }
        public float? Discount { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        public string? SignatureName { get; set; }
    }
    public class PoItemDetilList
    {
        public long? BrandDetailId { get; set; }
        public long? DeviceModelDetailId { get; set; }
        public long? DeviceProcessorDetailId { get; set; }
        public long? GenerationDetailId { get; set; }
        public long? RAMDetailId { get; set; }
        public long? HardDiskDetailId { get; set; }
        public long? Quantity { get; set; }
        public float? Price { get; set; }
        public string? Description { get; set; }

    }
}
