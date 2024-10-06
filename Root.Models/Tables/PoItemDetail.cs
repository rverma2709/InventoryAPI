using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class PoItemDetail:CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long PoItemDetilId { get; set; }  // Primary Key

        public long? PoDetailId { get; set; }    // Foreign key to PoDetails
        public long? BrandDetailId { get; set; }
        public long? DeviceModelDetailId { get; set; }
        public long? DeviceProcessorDetailId { get; set; }
        public long? GenerationDetailId { get; set; }
        public long? RAMDetailId { get; set; }
        public long? HardDiskDetailId { get; set; }
        public long? RecivingQuantity { get; set; }
        public long? Quantity { get; set; }
        public double? Price { get; set; }
        //public double? TotalAmount { get; set; }  // Fixed typo in field name
        public string Description { get; set; }
    }
}
