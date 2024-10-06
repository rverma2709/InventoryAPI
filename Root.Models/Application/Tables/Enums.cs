using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Application.Tables
{
    public class Enums
    {
        public enum Channels
        {
            AdminPortal,
            API,
            WEBPORTAL,
            CronJobAPI,
            Website
        }
        public enum ExpotTypes
        {
            Excel
        }
        public enum ProjectScheme
        {
            Udyamimitra,
            SVANidhi,
            AHIDF,
            KCC,
            PMVishwakarma
        }
        public enum StatusCode
        {
            NOTFND,
            UNQERR,
            SESEXP,
            LOGFLD,
            ACCERR,
            INVUSER
        }
    }
}
