using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("SaveProcLogs")]
    public class SPSaveProcLogs
    {
        [QueryParam]
        public string ProcName { get; set; }
        [QueryParam]
        public string Params { get; set; }
        [QueryParam]
        public bool Status { get; set; }
        [QueryParam]
        public string ErrorMessage { get; set; }
        [QueryParam]
        public double ExecTime { get; set; }
    }
}
