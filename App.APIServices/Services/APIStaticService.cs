using App.APIServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Root.Models.LogTables;
using Root.Models.Utils;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using Root.Services.Services;

namespace App.APIServices.Services
{
    public class APIStaticService : StaticService
    {
        public readonly AppConfig _appConfig;

        public APIStaticService(IConfiguration configuration, ILogService logService, ICommonService commonService, ICacheService cacheService, CacheUOM cacheRepo, IDataService<DBEntities, SMSLog> smsLogService, IHttpContextAccessor httpContextAccessor, IViewRenderService viewRenderService, IDataService<DBEntities, EmailLog> emailLogService) : base(configuration, logService, commonService, cacheService, cacheRepo, smsLogService, httpContextAccessor, viewRenderService, emailLogService)
        {
            _appConfig = CommonLib.GetAppConfig<AppConfig>(configuration);
        }
    }
}
