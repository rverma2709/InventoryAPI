using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class VendorDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long VendorDetailId { get; set; }
        [DisplayName("Vendor Name")]
        public string ?  VendorName { get; set; }
        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }

        [DisplayName("Phone No")]
        public string? ContactNo1 { get; set; }
        [DisplayName("Phone No 2")]
        public string? ContactNo2 { get; set; }
        [DisplayName("Email Id")]
        public string? EmailId { get; set; }
        [DisplayName("GST No")]
        public string? GSTNo { get; set; }
        [DisplayName("Address")]
        public string ? VendorAddress { get; set; }

    }
}
