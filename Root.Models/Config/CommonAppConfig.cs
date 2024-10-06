namespace Root.Models.Config
{
    public class CommonAppConfig
    {
        public OTPConfigs otpConfigs { get; set; } = new OTPConfigs();
        public TokenConfig tokenConfigs { get; set; } = new TokenConfig();
        public string LogPath { get; set; }
        public DBConfig dbconfig { get; set; } = new DBConfig();
        public SMSConfigs smsConfigs { get; set; } = new SMSConfigs();
        public EmailConfigs emailConfigs { get; set; } = new EmailConfigs();
        public UploadingConfigs uploadingConfigs { get; set; } = new UploadingConfigs();
        public SessionConfigs sessionConfigs { get; set; } = new SessionConfigs();
        public List<ChannelConfig> ChannelConfig { get; set; } = new List<ChannelConfig>();
    }
    public class OTPConfigs
    {
        public int Length { get; set; }
        public int Expiry { get; set; }
        public int Attempts { get; set; }
        public char DefaultOTP { get; set; }
        public bool Mode { get; set; }
        public long TestRefId { get; set; }
    }
    public class ChannelConfig
    {
        public string ChannelID { get; set; }
        public int SlidingExpiry { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
    public class SessionConfigs
    {
        public int SessionExpiry { get; set; }
        public string? SessionMode { get; set; }
    }
    public class UploadingConfigs
    {
        public string TempFolder { get; set; }
        public string Location { get; set; }
        public string DMSLink { get; set; }
        public string ConfigFiles { get; set; }
    }
    public class EmailConfigs
    {
        public string EmailFrom { get; set; }
        public string SMTPAddress { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPPass { get; set; }
        public int SMTPPort { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsBodyHtml { get; set; }
        public string IndiaPostCircleCCEmailId { get; set; }
    }
    public class SMSConfigs
    {
        public string SMSUrl { get; set; }
        public string SMSStatusUrl { get; set; }
        public bool isSSLCertificate { get; set; }
        public bool ProcessSMS { get; set; }
        public bool Mode { get; set; }
        public string DefaultNumber { get; set; }
        public string SchemeName { get; set; }
        public string SMSEntityId { get; set; }
        public int RetryAttempt { get; set; }
    }
    public class DBConfig
    {
        public int DBConnTimeOut { get; set; }
    }
    public class TokenConfig
    {
        public string ChannelId { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public int Expiry { get; set; }
        public bool StoreLogs { get; set; }

    }
   
}
