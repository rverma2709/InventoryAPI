using Root.Models.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class OTP
    {
        [Key]
        [ScaffoldColumn(false)]
        public long OTPId { get; set; }

        [Required]
        [StringLength(15)]
        [RegularExpression(CommonRegex.MobileNoFormat, ErrorMessage = CommonRegex.MobileNoFormatErr)]
        [DisplayName("Mobile No.")]
        public string? MobileNo { get; set; }

        [DisplayName("Email ID")]
        public string? EmailId { get; set; }

        [DisplayName("Channel")]
        public string? ChannelId { get; set; }

        [DisplayName("LoginId")]
        public long? LoginId { get; set; }

        [Required]
        [DisplayName("OTP For")]
        public string? OTPFor { get; set; }

        [Required]
        [DisplayName("OTP")]
        public string? OTPNumber { get; set; }

        [Required]
        [DisplayName("No of Attempts")]
        public long? Attempts { get; set; }

        [DisplayName("Is Verified")]
        public bool? IsVerified { get; set; }

        [DisplayName("Valid From")]
        public DateTime? ValidFrom { get; set; }

        [DisplayName("Valid Upto")]
        public DateTime? ValidUpto { get; set; }

        [DisplayName("Status")]
        public bool? Status { get; set; }

        [DisplayName("Response")]
        public string? Response { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DisplayName("RefCode")]
        //[NotMapped]
        public Guid? RefCode { get; set; }
    }
}
