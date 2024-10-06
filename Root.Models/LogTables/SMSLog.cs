using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models.Helper;

namespace Root.Models.LogTables
{
    public class SMSLog : CommonProperties
    {
        [Key]
        public long SMSLogId { get; set; }

        [NotMapped]
        [DisplayName("Customer")]
        public long? CustomerId { get; set; }

        [NotMapped]
        [Required]
        [StringLength(15)]
        [RegularExpression(CommonRegex.MobileNoFormat, ErrorMessage = CommonRegex.MobileNoFormatErr)]
        [DisplayName("Mobile No.")]
        public string MobileNo { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("SMS")]
        public string SMSText { get; set; }

        //[Required]
        [StringLength(50)]
        [DisplayName("SMS Code")]
        public string StatusCode { get; set; }

        [NotMapped]
        [DisplayName("Request URL")]
        public string RequestURL { get; set; }

        [NotMapped]
        [DisplayName("Response URI")]
        public string ResponseURI { get; set; }

        [NotMapped]
        [DisplayName("Success")]
        public bool Success { get; set; }

        [NotMapped]
        [DisplayName("Processed")]
        public bool Processed { get; set; }

        [NotMapped]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [NotMapped]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [NotMapped]
        [DisplayName("Attempts")]
        public int? Attempts { get; set; }

        [NotMapped]
        [DisplayName("Channel")]
        public string ChannelId { get; set; }

        [NotMapped]
        [DisplayName("ProcessingIP")]
        public string ProcessingIP { get; set; }

        [NotMapped]
        [DisplayName("RefCode")]
        public string RefCode { get; set; }

        [NotMapped]
        [DisplayName("ReportType")]
        public string ReportType { get; set; }

        [NotMapped]
        public long? NotificationTemplateId { get; set; }

        public string TemplateCode { get; set; }

        public string Scheme { get; set; }
    }
}
