using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Application.Tables
{
    public class NotificationTemplate : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long NotificationTemplateId { get; set; }

        public string NotificationTemplateCode { get; set; }

        public string Description { get; set; }

        public bool IsSMS { get; set; }

        public string SMSFormat { get; set; }

        public string AvailableValues { get; set; }

        public string RequiredValues { get; set; }

        public bool? IsEmail { get; set; }

        public string EmailFormat { get; set; }

        public string EmailSubject { get; set; }

        public string EmailGreeting { get; set; }

        public string TemplateId { get; set; }
    }
}
