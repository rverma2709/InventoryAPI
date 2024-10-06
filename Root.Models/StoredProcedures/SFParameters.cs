using Root.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    public class SFParameters
    {
        public SFParameters()
        {
            pageNo = 1;
            order = "Asc";
            rowsPerPage = CommonLib.GetRowsPerPage();
            RowCount = 0;
        }
        [QueryParam]
        public int pageNo { get; set; }
        [QueryParam]
        public string cols { get; set; }
        [QueryParam]
        public string order { get; set; }
        [QueryParam]
        public bool? Disabled { get; set; }
        [QueryParam]
        public bool? ExportType { get; set; }
        [QueryParam]
        public long rowsPerPage { get; set; }
        [QueryParam(direction: Direction.Output)]
        public long RowCount { get; set; }
    }
}
