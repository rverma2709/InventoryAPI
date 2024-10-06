using Root.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.APIServices.Models
{
    public class AppConfig : CommonAppConfig
    {
        public CookieConfig CookieConfigs { get; set; } = new CookieConfig();
        public RedisConfig RedisConfig { get; set; } = new RedisConfig();
    }
    public class RedisConfig
    {
        public string RedisConnectionString { get; set; }
        public bool IsEncConnectionString { get; set; }
    }

    public class CookieConfig
    {
        public string Key { get; set; }
        public string IV { get; set; }
        public int Expiry { get; set; }
        public string HttpOnly { get; set; }
        public string Secure { get; set; }
        public string MinimumSameSitePolicy { get; set; }
        public bool HttpOnlyFlag { get; set; }
        public bool SecureFlag { get; set; }
    }
}
