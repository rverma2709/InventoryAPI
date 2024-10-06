using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class ReciveItemsDto
    {
        public int PoDetailId { get; set; }
        public int PoItemDetilId { get; set; }
        public int Quantity { get; set; }
        public int RemainingQuantity { get; set; }
        public int LoginUserId { get; set; } = 1;
        public DateTime? ReceivingDate { get; set; }
        public IFormFile BulkImportDocument { get; set; }  // For file upload
    }
    public class ReciveItemsWithoutSeriolNumber
    {
        public int PoDetailId { get; set; }
        public int PoItemDetilId { get; set; }
        public int RecivingQuantity { get; set; }
        public DateTime ReceivingDate { get; set; }
        public int LoginUserId { get; set; } = 1;
    }
}
