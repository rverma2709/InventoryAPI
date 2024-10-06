using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class VMAllowedCMSPermission
    {
        [Required]
        [StringLength(500)]
        [DisplayName("Controller Name")]
        public string ControllerName { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Action Name")]
        public string ActionName { get; set; }
    }
    public class VMSetCMSPermission : VMAllowedCMSPermission
    {
        [Required]
        [StringLength(500)]
        [DisplayName("Controller Name")]
        public string DControllerName { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Action")]
        public string DActionName { get; set; }
    }
    public class VMGetCMSPermission : VMSetCMSPermission
    {
        public long CMSPermissionId { get; set; }
    }


    public class VMGetCMSControllerName
    {
        public long CMSPermissionId { get; set; }

        [Required]
        [StringLength(500)]
        [DisplayName("Controller Name")]
        public string DControllerName { get; set; }

        public bool Disabled { get; set; }
    }
}
