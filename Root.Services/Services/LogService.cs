using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Root.Models.Application.Tables;
using Root.Models.Config;
using Root.Models.LogTables;
using Root.Models.Utils;
using Root.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Root.Services.Services
{
    public class LogService : ILogService
    {
        protected readonly CommonAppConfig _appConfig;
        private protected readonly ICommonService _commonService;
        public readonly CacheUOM _cacheRepo;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public LogService(IConfiguration configuration, ICommonService commonService, CacheUOM cacheRepo, IHttpContextAccessor httpContextAccessor)
        {
            _appConfig = CommonLib.GetAppConfig<CommonAppConfig>(configuration);
            _commonService = commonService;
            _cacheRepo = cacheRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Public Methods
        public async Task SaveLog<T>(T model) where T : class
        {
            Task.Run(() => InnerSaveLog(model));
        }

        public async Task InnerSaveLog<T>(T model) where T : class
        {
            if (model.GetType().Name == "ActivityLog" && (_appConfig.tokenConfigs.ChannelId == Enums.Channels.AdminPortal.ToString() || _appConfig.tokenConfigs.ChannelId == Enums.Channels.Website.ToString()))
            { return; }
            else
            {
                if (model != null)
                {
                    RouteData routeData = _httpContextAccessor?.HttpContext.GetRouteData();

                    string APIPath = routeData?.Values["controller"].ToString() + Path.DirectorySeparatorChar + routeData?.Values["action"].ToString();
                    #region log code comment
                    //string logFilePath = _appConfig.LogPath + Path.DirectorySeparatorChar +
                    //    _appConfig.tokenConfigs.ChannelId + Path.DirectorySeparatorChar +
                    //    DateTime.Today.ToString("dd-MM-yyyy") + Path.DirectorySeparatorChar +
                    //    DateTime.Now.ToString("HH") + Path.DirectorySeparatorChar + (_appConfig.tokenConfigs.ChannelId == Enums.Channels.API.ToString() || _appConfig.tokenConfigs.ChannelId == Enums.Channels.CronJobAPI.ToString() ? APIPath : _httpContextAccessor?.HttpContext?.Session?.Id)
                    //     + Path.DirectorySeparatorChar +
                    //    model.GetType().Name;
                    //CommonLib.CheckDir(logFilePath);

                    //string uniqueGUID = _httpContextAccessor?.HttpContext?.TraceIdentifier == null ? Guid.NewGuid().ToString() : _httpContextAccessor?.HttpContext?.TraceIdentifier;


                    //logFilePath = System.IO.Path.Combine(logFilePath, uniqueGUID.Replace(":", "-") + ".json");
                    //using (StreamWriter _sw = new StreamWriter(logFilePath, true))
                    //{
                    //    await _sw.WriteAsync(CommonLib.ConvertObjectToJson(model));
                    //}
                    #endregion
                }
            }
        }

        public async Task<string> GetStatusMessage(string statusCode, List<string> str = null, int languageId = (int)RootEnums.Languages.English, string Message = null)
        {
            if (statusCode.IsNullString() == "")
            {
                return string.Empty;
            }

            try
            {

                //string statusMessage = (await _cacheRepo.StatusCodesList()).Where(x => x.Text == statusCode.ToString()).FirstOrDefault()?.Value;
                string statusMessage = null;

                if (!string.IsNullOrEmpty(statusMessage))
                {
                    if (str != null)
                    {
                        statusCode = string.Format(statusMessage, str.ToArray());
                    }
                    else
                    {
                        statusCode = statusMessage;
                    }
                }

                return statusCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResJsonOutput> HandleError(RouteData RouteData, HttpContext context, Exception ex, string modelName = "Record")
        {
            string ControllerName = RouteData?.Values["controller"].ToString();
            string ActionName = RouteData?.Values["action"].ToString();
            string URL = context?.Request.Path;
            string IPAddress = context?.Connection.RemoteIpAddress.ToString();
            string Method = context?.Request.Method;
            ResJsonOutput result = new ResJsonOutput();

            if (ex is Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                SqlException innerEx = (SqlException)ex.InnerException;
                if (innerEx != null && innerEx.Number == 2627) // Unique key Exception
                {
                    result.Status.StatusCode = RootEnums.StatusCodes.DUPREC.ToString();
                    result.Status.Message = await GetStatusMessage(RootEnums.StatusCodes.DUPREC.ToString(), new List<string> { modelName });
                }
                else if (innerEx != null && innerEx.Number == 50000) // Unique key Exception
                {
                    result.Status.StatusCode = RootEnums.StatusCodes.DUPREC.ToString();
                    string[] lines = Regex.Split(innerEx.Message, Environment.NewLine);
                    result.Status.Message = lines[0];
                }
                else
                {
                    result.Status.StatusCode = RootEnums.StatusCodes.GNLERR.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
                }
            }
            else
            {
                result.Status.StatusCode = RootEnums.StatusCodes.GNLERR.ToString();
                result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
            }
            ErrorLog errorLog = new ErrorLog();
            {
                errorLog.ControllerName = ControllerName;
                errorLog.ActionName = ActionName;
                errorLog.URL = URL;
                errorLog.IPAddress = IPAddress;
                errorLog.Method = Method;
                errorLog.UserID = 0;
                errorLog.AdditionalInfo = modelName;
                errorLog.Exception = Convert.ToString(ex);
                errorLog.Message = ex.Message;
                errorLog.StackTrace = ex.StackTrace;
                errorLog.InnerException = CommonLib.GetAllMessages(ex);// Convert.ToString(ex.In.nerException);
                errorLog.ExceptionType = Convert.ToString(ex.GetType());
                errorLog.QueryString = context?.Request?.QueryString.IsNullString();
                errorLog.ChannelId = _appConfig?.tokenConfigs?.ChannelId;
                errorLog.ProcessingIP = CommonLib.ServerIP;
            }
            if (context != null)
            {
                errorLog.RequestJSON = CommonLib.GetRequestJSON(context?.Request);
            }

            IHeaderDictionary headers = context?.Request.Headers;
            if (headers != null)
            {
                errorLog.RequestUUID = CommonLib.GetHeaderValue(headers, ProgConstants.RequestUUID);
            }

            errorLog.TimeStamp = DateTime.Now;

            await SaveLog(errorLog);
            return result;
        }
        #endregion
    }
}
