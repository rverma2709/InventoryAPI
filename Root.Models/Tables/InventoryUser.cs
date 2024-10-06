using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class InventoryUser : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long InventoryUserId { get; set; }
        [DisplayName("First Name")]
        public string? FirstName { get; set; }
        [DisplayName("Last Name")]
        public string? LastName { get; set; }
        [DisplayName("User Name")]
        public string? UserName { get; set; }
        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }
        [DisplayName("Inventory RoleId")]
        public long? InventoryRoleId { get; set; }
        [DisplayName("Password")]
        public string? Password { get; set; }
        [DisplayName("Gender")]
        public string? Gender { get; set; }
        [DisplayName("DOB")]
        public DateTime? DOB { get; set; }
        [DisplayName("EmailId")]
        public string? EmailId { get; set; }
        [DisplayName("Contact No")]
        public string? ContactNo { get; set; }
        [DisplayName("Contact No")]
        public string? ContactNo2 { get; set; }
        [DisplayName("Image Path")]
        public string? ImagePath { get; set; }
        [DisplayName("Last Login")]
        public DateTime? LastLogin { get; set; }
        [DisplayName("Password Changed")]
        public bool? PasswordChanged { get; set; }
        [DisplayName("Last Password Changed")]
        public DateTime? LastPasswordChanged { get; set; }
        [DisplayName("Blocked Till")]
        public DateTime? BlockedTill { get; set; }
        [DisplayName("WrongAttempts")]
        public long? WrongAttempts { get; set; }
        [DisplayName("IsProfileUpdated")]
        public bool? IsProfileUpdated { get; set; }
        [DisplayName("StateId")]
        public long? StateId { get; set; }
        [DisplayName("DistrictId")]
        public long? DistrictId { get; set; }
        public string? InventoryPermissionIds { get; set; }
        public string? GSTNo { get; set; }
        public string? Address {  get; set; }
        [ForeignKey("InventoryRoleId")]
        public virtual InventoryRole InventoryRole { get; set; }
    }
}
