using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class RootValidateOTP
    {
        [Required]
        [DisplayName("RefCode")]
        public string RefCode { get; set; }

        [Required]
        [DisplayName("OTP")]
        public string OTPNumber { get; set; }
    }

    public class OTPbyMobile
    {
        public OTPbyMobile()
        {
            Args = new Dictionary<string, string>();
        }
        [Required]
        [DisplayName("Mobile Number")]
        public string MobileNo { get; set; }

        [DisplayName("Email ID")]
        public string EmailId { get; set; }

        [Required]
        [DisplayName("OTP For")]
        public string OTPFor { get; set; }

        public long? LoginId { get; set; }

        [DisplayName("Arg")]
        public Dictionary<string, string> Args { get; set; }
    }
    public class OTPToken
    {
        [Required]
        [DisplayName("RefCode")]
        public string RefCode { get; set; }
        [DisplayName("Expiry")]
        public long Expiry { get; set; }
        [DisplayName("ExpiryTime")]
        public DateTime ExpiryTime { get; set; }
    }
    public class ResendOTPbyMobile
    {
        [Required]
        [DisplayName("RefCode")]
        public string RefCode { get; set; }

    }

    public class ApplicantViewModel
    {
        public long ApplicantId { get; set; }

        public string MobileNo { get; set; }

    }
}
