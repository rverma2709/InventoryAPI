using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class RequestByString
    {
        [DisplayName("id")]
        public string id { get; set; }
    }
    public class RequestById
    {
        [DisplayName("id")]
        public long id { get; set; }
    }
    public class RequestByIdFlag
    {
        [DisplayName("id")]
        public long id { get; set; }

        [DisplayName("flag")]
        public bool flag { get; set; }
    }
}
