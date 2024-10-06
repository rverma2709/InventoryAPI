using Root.Models.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Root.Models.Tables
{
    public class InventoryRole : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long InventoryRoleId { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Role")]
        [RegularExpression(CommonRegex.NameFormat, ErrorMessage = CommonRegex.NameFormatErr)]
        public string? RoleName { get; set; }

       
    }
}
