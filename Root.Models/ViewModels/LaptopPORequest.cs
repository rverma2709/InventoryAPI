using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class LaptopPORequest
    {
        public long VendorName { get; set; }
        public long ProcurementType { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime? RentStartDate { get; set; }
        public long? Tenure { get; set; }
        public long? Warranty { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        public float? TaxableAmount { get; set; }
        public float? ActualAmount { get; set; }
        public float? DiscountAmt { get; set; }
        public float? Sgst { get; set; }
        public float? Cgst { get; set; }
        public float? TotalAmount { get; set; }
        public string? SignatureName { get; set; }

        public List<LaptopItem> laptopItems { get; set; }
    }

    public class LaptopItem
    {
        public long Brand { get; set; }
        public long Model { get; set; }
        public long Processor { get; set; }
        public long Generation { get; set; }
        public long RAM { get; set; }
        public long HardDisk { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
    }
}
