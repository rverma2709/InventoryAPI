using Root.Models.StoredProcedures;
using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class ViewBrandDetail
    {
        public long? BrandDetailId { get; set; }
        public long? DeviceTypeId { get; set; }
        [DisplayName("Brand Name")]
        public string? BrandName { get; set; }
    }

    public class ViewProcessorDetail
    {
        public long? DeviceProcessorDetailId { get; set; }
        public long? DeviceTypeId { get; set; }

        [DisplayName("Processor Company Name")]
        public string? ProcessorCompanyName { get; set; }

        [DisplayName("Device Processor Name")]
        public string? DeviceProcessorName { get; set; }
 
    }

    public class ViewGenerationDetails
    {
        public long? GenerationDetailId { get; set; }
        public long? DeviceTypeId { get; set; }

        [DisplayName("Generation Name")]
        public string? GenerationName { get; set; }

    }

    public class ViewRAMDetails
    {
        public long? RAMDetailId { get; set; }
        public long? DeviceTypeId { get; set; }
          
        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }

        [DisplayName("RAM Size")]
        public string? RAMSize { get; set; } 

    }
    public class ViewHardDiskDetails
    {
        public long? HardDiskDetailId { get; set; }
        public long? DeviceTypeId { get; set; }

        [DisplayName("Hard-Disk Company Name")]
        public string? HardDiskCompanyName { get; set; }

        [DisplayName("Hard-Disk Size")]
        public string? HardDiskSize { get; set; }

        [DisplayName("Hard-Disk Type")]
        public string? HardDiskType { get; set; }
    }
    public class ViewProcurementType
    {
        public long? ProcurementTypeId { get; set; }

        [DisplayName("Procurement Name Type")]
        public string? ProcurementNameType { get; set; }

    }
}
