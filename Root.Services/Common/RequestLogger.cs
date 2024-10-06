using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
using Root.Models.Config;
using Root.Models.LogTables;
using Root.Models.Utils;
using Root.Services.Services;

namespace Root.Services.Common
{
    public class RequestLogger : ActionFilterAttribute
    {
        private readonly StaticService _staticService;
        private readonly CommonAppConfig _appconfig;

        public RequestLogger(StaticService staticService)
        {
            _staticService = staticService;
            _appconfig = staticService.GetAppConfig();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!context.HttpContext.Request.Scheme.ToLower().Contains("localhost", StringComparison.OrdinalIgnoreCase))
            {
                Microsoft.AspNetCore.Mvc.Controller controller = context.Controller as Microsoft.AspNetCore.Mvc.Controller;
                if (controller != null)
                {
                    controller.ViewBag.RequestStartTime = DateTime.Now;
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if (!context.HttpContext.Request.Path.ToString().Contains("/CustomerAPI/Data/GetFiles", System.StringComparison.OrdinalIgnoreCase))
            {
                Microsoft.AspNetCore.Mvc.Controller controller = context.Controller as Microsoft.AspNetCore.Mvc.Controller;
                if (controller != null)
                {
                    if (controller.ViewBag.RequestStartTime != null)
                    {
                        Microsoft.AspNetCore.Http.IHeaderDictionary headers = context.HttpContext.Request.Headers;
                        ActivityLog requestLog = new ActivityLog
                        {
                            ControllerName = context.RouteData.Values["controller"].ToString(),
                            ActionName = context.RouteData.Values["action"].ToString(),
                            URL = context.HttpContext.Request.GetDisplayUrl(),
                            IPAddress = CommonLib.GetRequestIP(context.HttpContext),
                            Method = context.HttpContext.Request.Method,
                            ChannelId = CommonLib.GetHeaderValue(headers, ProgConstants.ChannelId) ?? _appconfig.tokenConfigs.ChannelId,
                            StartDate = (DateTime)controller.ViewBag.RequestStartTime,
                        };
                        try
                        {
                            if (_appconfig.tokenConfigs.StoreLogs)
                            {
                                requestLog.RequestJSON = CommonLib.GetRequestJSON(context.HttpContext.Request);

                                try
                                {
                                    if (context.Result?.GetType() == typeof(Microsoft.AspNetCore.Mvc.ObjectResult))
                                    {
                                        requestLog.ResponseJSON = CommonLib.ConvertObjectToJson((context.Result as Microsoft.AspNetCore.Mvc.ObjectResult).Value);
                                    }
                                    else if (context.Result?.GetType() == typeof(Microsoft.AspNetCore.Mvc.JsonResult))
                                    {
                                        requestLog.ResponseJSON = CommonLib.ConvertObjectToJson((context.Result as Microsoft.AspNetCore.Mvc.JsonResult).Value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        //Request log
                        requestLog.EndDate = DateTime.Now;
                        System.Threading.Tasks.Task logentry = _staticService.SaveLog(requestLog);
                        logentry.Wait();
                    }
                }
            }
        }
    }
}
