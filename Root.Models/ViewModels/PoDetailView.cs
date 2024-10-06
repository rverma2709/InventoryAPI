using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class PoDetailView
    {
        public long? DeviceTypeId {  get; set; }
        public long VendorDetailId { get; set; }
        public long ProcurementTypeId { get; set; }
        [Required]
        public DateTime? PurchaseDate { get; set; }
        [Required]
        public DateTime? DeliveryDate { get; set; }
        
        public DateTime? RentStartDate { get; set; }
        public long? Tenure { get; set; }
        public long? Warranty { get; set; }
        public List<PoItemDetil>? LaptopItems { get; set; }
        public float? ActualAmount {  get; set; }
        public float? TaxableAmount { get; set; }
        public float? TotalAmount { get; set; }
        public string ?DiscountType { get; set; }
        public float? Discount { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        public string? SignatureName { get; set; }

        [ForeignKey("VendorDetailId")]
        public virtual VendorDetail? VendorDetail { get; set; }
        [ForeignKey("ProcurementTypeId")]
        public virtual ProcurementType? ProcurementType { get; set; }
    }

    public class PoItemDetil
    {
        public long? BrandDetailId { get; set; }
        public long? DeviceModeldetailId { get; set; }
        public long? DeviceProcessorDetailId { get; set; }
        public long? GenerationDetailId { get; set; }
        public long? RAMDetailId { get; set; }
        public long? HardDiskDetailId { get; set; }
        public long? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [ForeignKey("BrandDetailId")]
        public virtual BrandDetail? BrandDetail { get; set; }
        [ForeignKey("DeviceModeldetailId")]
        public virtual DeviceModeldetail? Modeldetail { get; set; }
        [ForeignKey("DeviceProcessorDetailId")]
        public virtual DeviceProcessorDetail? DeviceProcessorDetail { get; set; }
        [ForeignKey("GenerationDetailId")]
        public virtual GenerationDetail? GenerationDetail { get; set; }
        [ForeignKey("RAMDetailId")]
        public virtual RAMDetail? RAMDetail { get; set; }
        [ForeignKey("HardDiskDetailId")]
        public virtual HardDiskDetail? HardDiskDetail { get; set; }
    }

}
