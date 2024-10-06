using Newtonsoft.Json;
using Root.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetDDList")]
    public class SFGetDDList
    {
        [QueryParam]
        public string TableName { get; set; }
        [QueryParam]
        public string Args1 { get; set; }
        [QueryParam]
        public string Args2 { get; set; }

    }
    public class SFDDListResult
    {
        [JsonProperty("Text")]
        public string Text { get; set; }
        [JsonProperty("Value")]
        public string Value { get; set; }
        [JsonProperty("Disabled")]
        public bool Disabled { get; set; }
    }
    [StoredProcedureName("GetMastersForCache")]
    public class SPGetMastersForCache
    {
        [QueryParam]
        public string TableName { get; set; }
        [QueryParam]
        public long? StateId { get; set; }

        [QueryParam]
        public bool? AccessBlocked { get; set; }

        [QueryParam(dataType: "dbo.IDList")]
        public List<RequestById> Ids { get; set; }

    }
}
