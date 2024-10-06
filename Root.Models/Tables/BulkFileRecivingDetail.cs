//using Root.Models.Attribute;
using Root.Models.Attribute;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Root.Models.Tables
{
    public class BulkFileRecivingDetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long BulkFileRecivingDetailId { get; set; }


        public long? PoDetailId { get; set; }
        public long? PoItemDetilId { get; set; }

        [DisplayName("Upload Document")]
        [AdditionalMetadata("filetype", "4")]
        [DataType(DataType.Upload)]
        public string? UploadFile { get; set; }

        [DisplayName("Error File")]
        [DataType(DataType.Upload)]
        [AdditionalMetadata("filetype", "4")]
        public string? ErrorFile { get; set; }

        [DisplayName("Total Records")]
        public long? TotalUploadRecod { get; set; }

        [DisplayName("Success Records")]
        public long? SuccessRecord { get; set; }

        [DisplayName("Failed Records")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public long? ErrorRecord { get; set; }
        [ForeignKey("PoDetailId")]
        public virtual PoDetail? PoDetail { get; set; }
        [ForeignKey("PoItemDetilId")]
        public virtual PoItemDetail? PoItemDetail { get; set; }

    }
}
