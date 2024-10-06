using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.LogTables
{
    public class ErrorLog : CommonProperties
    {
        public Nullable<long> UserID { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string URL { get; set; }
        public string IPAddress { get; set; }
        public string Method { get; set; }
        public string Exception { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string ExceptionType { get; set; }
        public string RequestJSON { get; set; }
        public string RequestUUID { get; set; }
        public string QueryString { get; set; }
        public DateTime TimeStamp { get; set; }
        public string AdditionalInfo { get; set; }
        public string ChannelId { get; set; }
        public string ProcessingIP { get; set; }
    }
}
