using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class GetAllMasterData
    {
        public List<SelectListData>? vendorDetails {  get; set; }
        public List<SelectListData>? brandDetails { get; set; }
        public List<SelectListData>? modeldetails { get; set; }
        public List<SelectListData>? deviceProcessorDetails { get; set; }
        public List<SelectListData>? generationDetails { get; set; }
        public List<SelectListData>? rAMDetails { get; set; }
        public List<SelectListData>? hardDiskDetails { get; set; }
        public List<SelectListData>? procurementTypes { get; set; }
    }
    public class SelectListData
    {
        public long? Value { get; set; }
        public string? Text { get; set; }

    }
   


}
