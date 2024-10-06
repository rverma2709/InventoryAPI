using System.ComponentModel.DataAnnotations;

namespace Root.Models.Tables
{
    public class InquiryDetail
    {
        [Key]
        [ScaffoldColumn(false)]
        public long InquiryDetailId { get; set; }
        public string? UserName { get; set; }
        public string? ContactNo { get; set; }
        public string? EmailId { get; set; }
        public string? DeviceType { get; set; }
        public string? Quantity { get; set; }

    }
}
