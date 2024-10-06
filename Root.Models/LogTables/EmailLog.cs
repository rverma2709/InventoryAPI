using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.LogTables
{
    public class EmailLog : CommonProperties
    {
        [Key]
        public long EmailLogId { get; set; }

        [DisplayName("Email To")]
        [Required]
        public string EmailTo { get; set; }
        [DisplayName("Email From")]
        [Required]
        public string EmailFrom { get; set; }
        [DisplayName("Subject")]
        [Required]
        public string Subject { get; set; }

        [DisplayName("Email Content")]
        [Required]
        public string MsgBody { get; set; }

        [DisplayName("CC Email Id")]
        public string CcEmail { get; set; }

        [DisplayName("Attachments")]
        public string Attachments { get; set; }

        [DisplayName("Error Code")]
        public string ErrorCode { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        [DisplayName("Channel")]
        public string ChannelId { get; set; }

        [DisplayName("ProcessingIP")]
        public string ProcessingIP { get; set; }

        [NotMapped]
        public long? NotificationTemplateId { get; set; }
    }
}
