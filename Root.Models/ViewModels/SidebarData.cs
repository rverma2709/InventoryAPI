using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class SidebarData
    {
        public virtual List<VMSidebarList> VMSidebarLists { get; set; }
        public virtual List<RequestByString> InvalidLinks { get; set; }
        public virtual List<AllowedFile> AllowedFiles { get; set; }
    }
    public class VMSidebarList : VMGetCMSPermission
    {
        public virtual string MenuText { get; set; }
        public virtual long? CMSMenuId { get; set; }
        public virtual long? MSequenceNo { get; set; }
        public virtual long? PSequenceNo { get; set; }
        public virtual string MIcon { get; set; }
        public virtual string PIcon { get; set; }
    }
}
