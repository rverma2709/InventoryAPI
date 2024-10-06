using Root.Models.Tables;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Root.Models.ViewModels
{
    public class ViewInventoryProcurementDetail
    {
        [Key]
        [ScaffoldColumn(false)]
        public long? InventoryProcurementDetailId { get; set; }
        public long? DeviceTypeId { get; set; }
        [Required]
        public long? BrandDetailId { get; set; }
        [Required]
        public long? DeviceModeldetailId { get; set; }
        [Required]
        public long? DeviceProcessorDetailId { get; set; }
        [Required]
        public long? GenerationDetailId { get; set; }
        [Required]
        public long? RAMDetailId { get; set; }
        [Required]
        public long? HardDiskDetailId { get; set; }
        [DisplayName("Serial Number")]
        public string? SerialNumber { get; set; }
        [Required]
        [DisplayName("TTSPLID")]
        public string? TTSPLID { get; set; }
        
        [DisplayName("PO Number")]
        public string? PONumber { get; set; }
        [Required]
        public long? VendorDetailId { get; set; }
        [Required]
        [DisplayName("Purchase Date")]
        public DateTime? PurchaseDate { get; set; }
        public long? ProcurementTypeId { get; set; }
        [DisplayName("POD")]
        public string? POD { get; set; }
        [DisplayName("Warranty")]
        public long? Warranty { get; set; }
        [DisplayName("Rent Start Date")]
        [Required]
        public DateTime? RentStartDate { get; set; }
        [DisplayName("Delivery Date")]
        [Required]
        public DateTime? DeliveryDate { get; set; }
        [DisplayName("Tenure")]
        public long? Tenure { get; set; }
        [DisplayName("QC Status")]
        public bool? QualityCheckStatus { get; set; }
        [DisplayName("QualityCheck Process Date")]
        public DateTime? QualityCheckProcessDt { get; set; }
        [DisplayName("QualityCheck Return Date")]
        public DateTime? QualityCheckRetunDt { get; set; }
        [DisplayName("Return To Vendor Date")]
        public DateTime? ReturnToVendorDt { get; set; }
        public DateTime? RentStartDt { get; set; }
        [DisplayName("Rent Stop Date")]
        public DateTime? RentStopDt { get; set; }
        public bool? IsRentStatus { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }
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
        [ForeignKey("VendorDetailId")]
        public virtual VendorDetail? VendorDetail { get; set; }
        [ForeignKey("ProcurementTypeId")]
        public virtual ProcurementType? ProcurementType { get; set; }
    }
}
