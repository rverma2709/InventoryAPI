using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Root.Models.ViewModels
{
    public class RecivingItemView
    {
        [Required]
        public long? PoDetailId { get; set; }
        [Required]
        public long? PoItemDetilId { get; set; }
        public long? RemainingQuantity { get; set; }
        [Required]
        public DateTime? RecivingDate { get; set; }
        public long? Quantity { get; set; }
        public string? SeriolNumber { get; set; }


    }
}
