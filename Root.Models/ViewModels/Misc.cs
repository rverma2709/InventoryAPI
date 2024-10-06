using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models.Application.Tables;

namespace Root.Models.ViewModels
{
    public class ViewFileResult
    {
        [DisplayName("FileName")]
        public string FileName { get; set; }

        [DisplayName("Base64")]
        public string Base64 { get; set; }
    }
    public class UploadFileModel
    {
        [Required]
        [DisplayName("FileName")]
        public string FileName { get; set; }

        [Required]
        [DisplayName("FileString")]
        public string FileString { get; set; }

        [Required]
        public long AllowedFileId { get; set; }
    }
    public class ParentDDResult
    {
        public bool Disabled { get; set; }
        public string Text { get; set; }
        public string? Value { get; set; }
        public long? ParentId { get; set; }
    }
    public class MBNotification
    {

        public MBNotification()
        {
            Args = new Dictionary<string, string>();
        }

        [DisplayName("Channel")]
        public string ChannelId { get; set; }

        [DisplayName("CIFID")]
        public long? RefId { get; set; }

        [Required]
        [DisplayName("MobileNo")]
        public string MobileNo { get; set; }

        [DisplayName("EmailId")]
        public string EmailId { get; set; }

        [DisplayName("Template Code")]
        public string TemplateCode { get; set; }

        [DisplayName("Arg")]
        public Dictionary<string, string> Args { get; set; }

        [DisplayName("CCEmailId")]
        public string CCEmailId { get; set; }
        public NotificationTemplate notificationTemplate { get; set; }
    }
    public class ViewStatusMessage
    {
        [DisplayName("Message")]
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class Notification
    {

        public Notification()
        {
            Args = new Dictionary<string, string>();
        }

        [DisplayName("Channel")]
        public string ChannelId { get; set; }

        [DisplayName("CIFID")]
        public long? RefId { get; set; }

        [Required]
        [DisplayName("MobileNo")]
        public string MobileNo { get; set; }

        [DisplayName("EmailId")]
        public string EmailId { get; set; }

        [DisplayName("Template Code")]
        public string TemplateCode { get; set; }

        [DisplayName("Arg")]
        public Dictionary<string, string> Args { get; set; }
        public object Payload { get; set; }
    }

}
