using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.LogTables
{
    public class RemoteLog : CommonProperties
    {
        public RemoteLog()
        {
            StartDate = DateTime.Now;
        }
        [Key]
        [ScaffoldColumn(false)]
        public long RemoteLogId { get; set; }

        [DisplayName("URL")]
        public string URL { get; set; }

        [DisplayName("Request JSON")]
        public string RequestJSON { get; set; }

        [DisplayName("Response JSON")]
        public string ResponseJSON { get; set; }
        public string RequestUUID { get; set; }
        public string HeaderParams { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ChannelId { get; set; }
        public string ProcessingIP { get; set; }
    }
}
