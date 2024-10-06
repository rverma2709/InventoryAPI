using Root.Models.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class InventoryPermission : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long InventoryPermissionId { get; set; }

        [ScaffoldColumn(false)]
        public long? InventoryMenuId { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Controller Name")]
        [RegularExpression(CommonRegex.MenuFormat, ErrorMessage = CommonRegex.MenuFormatErr)]
        public string? DControllerName { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Action")]
        [RegularExpression(CommonRegex.MenuFormat, ErrorMessage = CommonRegex.MenuFormatErr)]
        public string? DActionName { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Controller Name")]
        public string? ControllerName { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Action Name")]
        public string? ActionName { get; set; }

        [DisplayName("Sequence No")]
        public long? SequenceNo { get; set; }

        [DisplayName("Icon")]
        public string? Icon { get; set; }
    }
}
