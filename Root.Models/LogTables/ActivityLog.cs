using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.LogTables
{
    public class ActivityLog
    {
        [DisplayName("IP Address")]
        public string IPAddress { get; set; }

        [DisplayName("ChannelId")]
        public string ChannelId { get; set; }

        [StringLength(500)]
        [DisplayName("Controller Name")]
        public string ControllerName { get; set; }

        [StringLength(500)]
        [DisplayName("Action Name")]
        public string ActionName { get; set; }

        [DisplayName("URL")]
        public string URL { get; set; }

        [DisplayName("Method")]
        public string Method { get; set; }

        [DisplayName("Star tDate")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("RequestJSON")]
        public string RequestJSON { get; set; }

        [DisplayName("ResponseJSON")]
        public string ResponseJSON { get; set; }

        [DisplayName("ProcessingIP")]
        public string ProcessingIP { get; set; }
    }
}
