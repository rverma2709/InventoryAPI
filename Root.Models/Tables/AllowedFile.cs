using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class AllowedFile
    {
        [Key]
        [ScaffoldColumn(false)]
        public long AllowedFileId { get; set; }
        public string? FileType { get; set; }
        public string? MIMEType { get; set; }
        public string? Extensions { get; set; }
        public double Size { get; set; }
        public double MinSize { get; set; }

        // public long Size { get; set; }
        //public double MinSize { get; set; }
    }
}
