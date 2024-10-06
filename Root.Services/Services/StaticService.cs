using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Root.Models.Application.Tables;
using Root.Models.Config;
using Root.Models.LogTables;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Root.Services.Services
{
    public class StaticService
    {
        public string ModelName { get; set; }
        protected readonly ILogService _logService;
        protected readonly ICommonService _commonService;
        public readonly ICacheService _cacheService;
        public readonly CacheUOM _cacheRepo;
        public readonly CommonAppConfig _appConfig;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IViewRenderService _viewRenderService;
        private readonly IDataService<DBEntities, SMSLog> _smsLogService;
        public string httpsContextSessionId;
        private readonly IDataService<DBEntities, EmailLog> _emailLogService;

        public StaticService(
            IConfiguration configuration,
            ILogService logService,
            ICommonService commonService,
            ICacheService cacheService,
            CacheUOM cacheRepo,
            IDataService<DBEntities, SMSLog> smsLogService,
            IHttpContextAccessor httpContextAccessor,
            IViewRenderService viewRenderService,
            IDataService<DBEntities, EmailLog> emailLogService) //: base(dataService)
        {
            _appConfig = CommonLib.GetAppConfig<CommonAppConfig>(configuration);
            _logService = logService;
            _commonService = commonService;
            _cacheService = cacheService;
            _cacheRepo = cacheRepo;
            _httpContextAccessor = httpContextAccessor;
            _viewRenderService = viewRenderService;
            _smsLogService = smsLogService;
            _smsLogService = smsLogService;
            _emailLogService = emailLogService;


        }

        public CommonAppConfig GetAppConfig()
        {
            return _appConfig;
        }

        #region Log Service

        //Save log method kept separate with return type as void
        public async Task SaveLog(object model)
        {
            try
            {
                await _logService.SaveLog(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetStatusMessage(string statusCode, List<string> str = null, int languageId = (int)RootEnums.Languages.English, string Message = null)
        {
            int headerlanguageId = Convert.ToInt32(CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.LanguageId));
            if (headerlanguageId > 0)
            {
                languageId = headerlanguageId;
            }
            return await _logService.GetStatusMessage(statusCode, str, languageId, Message);
        }

        public async Task<ResJsonOutput> HandleError(RouteData RouteData, HttpContext context, Exception ex, string modelName = "Record")
        {
            return await _logService.HandleError(RouteData, context, ex, modelName);
        }

        #endregion

        #region Common Service
        public async Task<T> ExecuteScalarAsync<TDbContext, T>(object obj) where TDbContext : DbContext
        {
            try
            {
                bool readOnly = await ProcDetailsList(obj);
                return await _commonService.ExecuteScalarAsync<TDbContext, T>(obj, readOnly, QueryTimeOut: _appConfig.dbconfig.DBConnTimeOut);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> ExecuteNonQueryAsync<TDbContext>(object obj) where TDbContext : DbContext
        {
            try
            {
                bool readOnly = await ProcDetailsList(obj);
                return await _commonService.ExecuteNonQueryAsync<TDbContext>(obj, readOnly, QueryTimeOut: _appConfig.dbconfig.DBConnTimeOut);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T>> ExecuteSPAsync<TDbContext, T>(object obj) where TDbContext : DbContext
        {
            try
            {
                bool readOnly = await ProcDetailsList(obj);
                return await _commonService.ExecuteSPAsync<TDbContext, T>(obj, readOnly, QueryTimeOut: _appConfig.dbconfig.DBConnTimeOut);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GenerateOTP() => GenerateKey(6);

        public string GenerateKey(int length)
        {
            string key = Regex.Replace(Guid.NewGuid().ToString(), "[^0-9]+", "");
            if (key.Length < length)
            {
                key = key + GenerateKey(length - key.Length);
            }
            key = key.Substring(0, length);
            return key;
        }
        public async Task<bool> ExecuteSPtoCSVAsync<TDbContext, T>(object obj, string FileName, bool appendHeader = true) where TDbContext : DbContext
        {
            try
            {
                bool readOnly = await ProcDetailsList(obj);
                return await _commonService.ExecuteSPtoCSVAsync<TDbContext, T>(obj, FileName, appendHeader, readOnly);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResJsonOutput> FetchList<TDbContext, T>(SFParameters model) where TDbContext : DbContext
        where T : class
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                bool readOnly = await ProcDetailsList(model);
                JsonOutputForList listresult = await _commonService.FetchList<TDbContext, T>(model, readOnly, QueryTimeOut: _appConfig.dbconfig.DBConnTimeOut);
                result.Status.IsSuccess = true;
                result.Data = listresult;
            }
            catch (Exception ex)
            {
                return await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }

            return result;
        }

        #endregion

        /// <summary>
        /// This is common method to send SMS(currently implemented), Email and push(proposed) notification to the customer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResJsonOutput> SendNotification(MBNotification model, string DeeplinkType = "", string DeeplinkURL = "")
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                //string MobileNo = model.MobileNo;
                //string EmailId = model.EmailId;
                model.MobileNo = model.MobileNo.IsNullString();
                model.EmailId = model.EmailId.IsNullString();

              //  List<NotificationTemplate> templateq = (await _cacheRepo.NotificationTemplatesList(true)).Where(x => x.NotificationTemplateCode == model.TemplateCode).ToList();
                NotificationTemplate template = (await _cacheRepo.NotificationTemplatesList()).Where(x => x.NotificationTemplateCode == model.TemplateCode && x.Disabled == false)?.FirstOrDefault();

                model.notificationTemplate = template;
                if (template != null)
                {
                    if (template.IsSMS && model.MobileNo != "")
                    {
                        template.SMSFormat = CommonLib.ConvertTemplateString(template.SMSFormat, template.AvailableValues, model.Args);
                       
                        SMSLog smslog = await SendSMS(template, model.MobileNo);
                        smslog.NotificationTemplateId = template.NotificationTemplateId;
                        smslog.TemplateCode = template.NotificationTemplateCode;
                        smslog.Scheme = Enums.ProjectScheme.PMVishwakarma.ToString();
                        await SaveLog(smslog);
                        result.Status.IsSuccess = true;//This is specific to SMS notification
                        //if (result.Status.IsSuccess == true)
                        //{
                        //    await _smsLogService.Create(smslog);
                        //    await _smsLogService.Save();
                        //}
                    }

                    if (template.IsEmail == true && model.EmailId != "")
                    {
                        template.EmailFormat = CommonLib.ConvertTemplateString(template.EmailFormat, template.AvailableValues, model.Args);
                        template.EmailSubject = CommonLib.ConvertTemplateString(template.EmailSubject, template.AvailableValues, model.Args);
                        template.EmailGreeting = CommonLib.ConvertTemplateString(template.EmailGreeting, template.AvailableValues, model.Args);

                        EmailLog emailLog = await SendEmail(model.EmailId, template.EmailSubject, template.EmailFormat, null, model.CCEmailId, null);
                        emailLog.NotificationTemplateId = template.NotificationTemplateId;

                        emailLog.NotificationTemplateId = template.NotificationTemplateId;

                        await _emailLogService.Create(emailLog);
                        await _emailLogService.Save();
                    }

                    //if (template.IsNotification)
                    //{
                    //    SPGetFCMTokensFromMobileNo getFCMTokens = new SPGetFCMTokensFromMobileNo()
                    //    {
                    //        MobileNos = model.MobileNo,
                    //        ChannelId = model.ChannelId
                    //    };
                    //    List<ResFCMTokens> resFCMTokens = await ExecuteSPAsync<DBEntities, ResFCMTokens>(getFCMTokens);

                    //    if (resFCMTokens.Count() > 0)
                    //    {
                    //        PushMessage pushMessage = new PushMessage();
                    //        pushMessage.PushMessageTitle = CommonLib.ConvertTemplateString(template.NotificationTitle, template.AvailableValues, model.Args);
                    //        pushMessage.PushMessageBody = CommonLib.ConvertTemplateString(template.NotificationFormat, template.AvailableValues, model.Args);
                    //        pushMessage.SelectType = "DeviceIds";
                    //        pushMessage.DeviceIds = String.Join(',', resFCMTokens.Select(s => s.DeviceId).ToList());
                    //        pushMessage.ChannelId = _appConfig.tokenConfigs.ChannelId;
                    //        pushMessage.DeeplinkType = DeeplinkType;
                    //        pushMessage.DeeplinkURL = DeeplinkURL;
                    //        await _logService.SavePushMessage(pushMessage);
                    //        pushMessage.ChannelId = model.ChannelId;
                    //        pushMessage.NotificationTemplateId = template.NotificationTemplateId;
                    //        ResJsonOutput pushMessageResult = await SendPushMessage(pushMessage);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public List<string> UpdateFileName(object model)
        {
            List<string> ListOfFiles = new List<string>();
            try
            {
                ListOfFiles = CommonLib.UpdateFileName(model, _appConfig.uploadingConfigs.TempFolder + CommonLib.DirectorySeparatorChar(), _appConfig.uploadingConfigs.Location + CommonLib.DirectorySeparatorChar());
            }
            catch (Exception)
            {
            }
            return ListOfFiles;
        }

      

        public async Task<ResJsonOutput> GetDataAsync(string ApiPath, List<KeyValue> Headers = null, bool isSSLCertificate = true)
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            try
            {
                result.Data = await RequestHandler.GetDataAsync(remoteLog, ApiPath, Headers, isSSLCertificate);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }

            // await _logService.SaveRemoteLog(remoteLog);
            return result;
        }

        public async Task<ResJsonOutput> Details<T>(IDataService<DBEntities, T> service, long id, Expression<Func<T, object>>[] includes = null) where T : class
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                T model = await service.Detail(id, includes);
                if (model == null)
                {
                    result.Status.StatusCode = Enums.StatusCode.NOTFND.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string> { ModelName });
                }
                else
                {
                    result.Status.IsSuccess = true;
                    result.Data = model;
                }
            }
            catch (Exception ex)
            {
                return await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            return result;
        }

        private async Task<bool> ProcDetailsList(object model)
        {
            string ProcName = SqlParameterExtensions.GetStoredProcedureName(model);
            //ProcDetail procDetail = (await _cacheRepo.ProcDetailsList()).Where(x => x.ProcName == ProcName).FirstOrDefault();
            ProcDetail procDetail = null;
            bool readOnly = (procDetail != null) ? procDetail.ReadOnly : false;
            if (readOnly)
            {
                string demoString = _httpContextAccessor.HttpContext.Session.GetString(ProcName + "-ReadOnly");
                if (demoString != null)
                {
                    readOnly = (demoString != "false");
                }
                _httpContextAccessor.HttpContext.Session.SetString(ProcName + "-ReadOnly", (!readOnly).ToString().ToLower());
            }
            return readOnly;
        }

        public async Task<ResJsonOutput> FetchList<T>(IDataService<DBEntities, T> service, SFParameters model, Expression<Func<T, object>>[] includes = null) where T : class
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                bool readOnly = await ProcDetailsList(model);
                JsonOutputForList listresult = await service.FetchList(model, includes, readOnly);
                result.Status.IsSuccess = true;
                result.Data = listresult;
            }
            catch (Exception ex)
            {
                return await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }

            return result;
        }

        public async Task<ResJsonOutput> Disable<TDbContext, TDbEnity>(IDataService<TDbContext, TDbEnity> service, RequestByIdFlag requestByIdFlag, long LoginCMSUserId, DateTime Dt) where TDbContext : DbContext where TDbEnity : class
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                TDbEnity model = await service.Detail(requestByIdFlag.id);
                if (model == null)
                {
                    result.Status.StatusCode = Enums.StatusCode.NOTFND.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string> { ModelName });
                }
                else
                {
                    if (Convert.ToBoolean(model.GetType().GetProperty("Disabled").GetValue(model, null)) != requestByIdFlag.flag)
                    {
                        model.GetType().GetProperty("Disabled").SetValue(model, requestByIdFlag.flag);
                        model.GetType().GetProperty("LmdBy").SetValue(model, LoginCMSUserId);
                        model.GetType().GetProperty("Lmd").SetValue(model, Dt);
                        service.Update(model);
                        await service.Save();

                        result.Status.IsSuccess = true;
                        result.Data = new ViewStatusMessage() { Message = ModelName + " " + (requestByIdFlag.flag ? "disabled" : "enabled") + " successfully." };
                    }
                    else
                    {
                        result.Status.StatusCode = Enums.StatusCode.UNQERR.ToString();
                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { ModelName });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public async Task<ResJsonOutput> PostDataAsync<T>(string ApiPath, object obj, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, double? APITimeOut = null, bool isScanAPI = false)
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            remoteLog.RequestJSON = CommonLib.ConvertObjectToJson(obj);
            try
            {
                result.Data = await RequestHandler.PostDataAsync<T>(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, "", "", APITimeOut, isScanAPI);

                result.Status.IsSuccess = true;

            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }

        public async Task<ResJsonOutput> PostDataAsyncWithoutBody<T>(string ApiPath, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null)
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            try
            {
                result.Data = await RequestHandler.PostDataAsyncWithoutBody<T>(remoteLog, ApiPath, Headers, isSSLCertificate, authenticationHeaderValue);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }

        public async Task<ResJsonOutput> PostXMLDataAsync<T>(string ApiPath, object obj, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "", int timeOutInSeconds = 0)
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            remoteLog.RequestJSON = CommonLib.ConvertObjectToJson(obj);
            try
            {
                result.Data = await RequestHandler.PostXMLDataAsync<T>(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, certPath, timeOutInSeconds);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }

        public async Task<ResJsonOutput> PostJsonAsPlain(string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            remoteLog.RequestJSON = CommonLib.ConvertObjectToJson(obj);
            try
            {
                result.Data = await RequestHandler.PostJsonAsPlain(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, certPath);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }

        public async Task<ResJsonOutput> PostDownloadFile(string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            try
            {
                remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
                remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
                remoteLog.RequestJSON = CommonLib.ConvertObjectToJson(obj);
                result.Data = await RequestHandler.PostDownloadFile(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, certPath);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }
        public async Task<ResJsonOutput> PostStringAsPlain(string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            remoteLog.RequestJSON = CommonLib.ConvertObjectToJson(obj);
            try
            {
                result.Data = await RequestHandler.PostStringAsPlain(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, certPath);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }

        public async Task<ResJsonOutput> PostXMLAsPlain(string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            if (obj != null)
            {
                remoteLog.RequestJSON = obj.ToString();
            }
            try
            {
                result.Data = await RequestHandler.PostXMLAsPlain(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, certPath);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }

        public async Task<string> RenderToStringAsync(string controllerName, string viewName, object model, string FilePath, string FileName)
        {
            return await _viewRenderService.RenderToStringAsync(controllerName, viewName, model, FilePath, FileName);
        }
        public async Task<Byte[]> GetPDFArray(string controllerName, string viewName, object model, string FilePath, string FileName)
        {
            return await _viewRenderService.GetPDFArray(controllerName, viewName, model, FilePath, FileName);
        }

        private async Task<SMSLog> SendSMS(NotificationTemplate model, string MobileNo)
        {
            bool flag = false;
            string RefCode = Guid.NewGuid().ToString();

            SMSLog smslog = new SMSLog
            {
                MobileNo = MobileNo,
                SMSText = model.SMSFormat,
                RefCode = RefCode,
                //RequestURL = string.Format(
                //    _appConfig.smsConfigs.SMSUrl,
                //        HttpUtility.UrlEncode(SMSText.Trim(), Encoding.UTF8),
                //        ((_appConfig.smsConfigs.Mode) ? MobileNo : _appConfig.smsConfigs.DefaultNumber),
                //        HttpUtility.UrlEncode(RefCode.ToString(), Encoding.UTF8),
                //        HttpUtility.UrlEncode(MobileNo.ToString(), Encoding.UTF8)

                //),
                RequestURL = _appConfig.smsConfigs.SMSUrl + MobileNo.Trim() + "&text=" + HttpUtility.UrlEncode(model.SMSFormat.Trim().Replace(":", ": "), System.Text.Encoding.UTF8) + "&output=json&entityid=" + _appConfig.smsConfigs.SMSEntityId + "&templateid=" + model.TemplateId,
                Processed = _appConfig.smsConfigs.ProcessSMS,
                ChannelId = _appConfig.tokenConfigs.ChannelId
            };
            if (_appConfig.smsConfigs.ProcessSMS == true)
            {
                try
                {
                    smslog.StartDate = DateTime.Now;
                    ResJsonOutput op = new ResJsonOutput();
                    int RetryAttempt = _appConfig.smsConfigs.RetryAttempt;
                    while (RetryAttempt > 0 && !op.Status.IsSuccess)
                    {
                        op = await GetDataAsync(smslog.RequestURL, isSSLCertificate: _appConfig.smsConfigs.isSSLCertificate);
                        smslog.EndDate = DateTime.Now;

                        string result = "No Response received";
                        string sms_id = "No Response received";

                        if (op.Status.IsSuccess)
                        {
                            result = op.Data.IsNullString();
                            flag = true;
                        }
                        else
                        {
                            sms_id = op.Status.Message;
                            result = op.Status.StatusCode;
                        }

                        smslog.ResponseURI = sms_id;
                        smslog.StatusCode = result;
                        RetryAttempt--;
                    }
                    smslog.Attempts = _appConfig.smsConfigs.RetryAttempt - RetryAttempt;
                }
                catch (Exception ex)
                {
                    smslog.ResponseURI = ex.Message;
                    smslog.StatusCode = "Catch Exception";
                    await HandleError(null, _httpContextAccessor.HttpContext, ex);
                }
            }
            else
            {
                flag = true;
            }
            smslog.Success = flag;
            return smslog;
        }

        private async Task<EmailLog> SendEmail(string To, string Subject, string MsgBody, string From = "", string CcEmail = "", string[] Attachments = null)
        {

            EmailLog emaillog = new EmailLog
            {
                ChannelId = _appConfig.tokenConfigs.ChannelId,
                StartDate = DateTime.Now,
                EmailTo = To,
                EmailFrom = _appConfig.emailConfigs.EmailFrom,
                MsgBody = MsgBody,
                Subject = Subject,
                CcEmail = CcEmail,
                Attachments = (Attachments != null) ? string.Join(", ", Attachments) : ""
            };

            try
            {
                SmtpClient client = new SmtpClient(_appConfig.emailConfigs.SMTPAddress)
                {
                    Port = _appConfig.emailConfigs.SMTPPort,
                    //UseDefaultCredentials = _appConfig.emailConfigs.UseDefaultCredentials,
                    EnableSsl = _appConfig.emailConfigs.EnableSsl
                };

                if (!_appConfig.emailConfigs.UseDefaultCredentials)
                {
                    client.Credentials = new NetworkCredential(_appConfig.emailConfigs.SMTPUser, _appConfig.emailConfigs.SMTPPass);
                }

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_appConfig.emailConfigs.EmailFrom)
                };
                mailMessage.To.Add(To);
                mailMessage.Body = MsgBody;
                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = _appConfig.emailConfigs.IsBodyHtml;

                if (CcEmail != "" && CcEmail != null)
                {
                    string[] ArrCcEmails = CcEmail.Split(',');
                    foreach (string cc in ArrCcEmails)
                    {
                        MailAddress copy = new MailAddress(cc);
                        mailMessage.CC.Add(copy);
                    }
                }

                if (Attachments != null && Attachments.Length > 0)
                {
                    foreach (string eachAttachment in Attachments)
                    {
                        if (File.Exists(eachAttachment))
                        {
                            FileStream fs = new FileStream(eachAttachment, FileMode.Open, FileAccess.Read);
                            System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(fs, Path.GetFileName(eachAttachment), MediaTypeNames.Application.Octet);
                            mailMessage.Attachments.Add(data);
                        }
                    }
                }

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                await client.SendMailAsync(mailMessage);
                emaillog.Status = true;
            }
            catch (Exception ex)
            {
                emaillog.ErrorCode = ex.Message;
            }

            emaillog.EndDate = DateTime.Now;
            return emaillog;
        }

        public async Task<ResJsonOutput> PostXMLJsonDataAsync<T>(string ApiPath, object obj, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "", int timeOutInSeconds = 0)
        {
            ResJsonOutput result = new ResJsonOutput();
            RemoteLog remoteLog = new RemoteLog();
            remoteLog.ChannelId = _appConfig.tokenConfigs.ChannelId;
            remoteLog.RequestUUID = CommonLib.GetHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, ProgConstants.RequestUUID);
            remoteLog.RequestJSON = CommonLib.ConvertObjectToJson(obj);
            try
            {
                result.Data = await RequestHandler.PostXMLJsonDataAsync<T>(remoteLog, ApiPath, obj, Headers, isSSLCertificate, authenticationHeaderValue, certPath, timeOutInSeconds);
                result.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await HandleError(null, _httpContextAccessor.HttpContext, ex);
            }
            await _logService.SaveLog(remoteLog);
            return result;
        }
    }
}
