using Root.Models.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class ViewVendorDetail
    {
        public long? InventoryUserId { get; set; }
        [Required]
        [DisplayName("Vendor Name")]
        public string? VendorName { get; set; }
        [Required]
        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }
        [Required]
        [RegularExpression(CommonRegex.MobileNoFormat, ErrorMessage = CommonRegex.MobileNoFormatErr)]
        [DisplayName("Phone No")]
        public string? ContactNo { get; set; }
        
        [DisplayName("Phone No 2")]
        public string? ContactNo2 { get; set; }
        [Required]
        [RegularExpression(CommonRegex.EmailIDFormat, ErrorMessage = CommonRegex.EmailIDFormatErr)]
        [DisplayName("Email Id")]
        public string? EmailId { get; set; }
        [Required]
        [RegularExpression(CommonRegex.GSTNoFormate, ErrorMessage = CommonRegex.GSTNoFormateErr)]
        [DisplayName("GST No")]
        public string? GSTNo { get; set; }
        [Required]
        [DisplayName("Address")]
        public string? VendorAddress { get; set; }
    }
}
