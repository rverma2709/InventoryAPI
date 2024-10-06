using App.AdminPortalServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Root.Models.Application.Tables;
using Root.Models.LogTables;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using Root.Services.Services;
using System.Text;
using System.Text.RegularExpressions;

namespace App.AdminPortalServices.Services
{
    public class AdminPortalStaticService : StaticService
    {
        public readonly AppConfig _appConfig;
        public long LoginUserId;
        public InventoryUser LoginUser;
        public InventoryRole inventoryRole;
        public List<VMAllowedCMSPermission> LoginUserPermissions;
        public DateTime Dt;

        private readonly IDataService<DBEntities, InventoryUser> _DataService;
        private readonly IDataService<DBEntities, InventoryRole> _InventoryRole;
        private protected readonly IDataService<DBEntities, OTP> _otpDataService;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();


        public bool Initiated = false;

        public AdminPortalStaticService(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogService logService,
            ICommonService commonService,
            ICacheService cacheService,

            CacheUOM cacheRepo,
             IDataService<DBEntities, SMSLog> smsLogService,
            IDataService<DBEntities, InventoryRole> InventoryRole,
        IDataService<DBEntities, InventoryUser> cmsuserDataService,
            IDataService<DBEntities, OTP> otpDataService,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
           IDataService<DBEntities, EmailLog> emailLogService,


        IViewRenderService viewRenderService) : base(
                configuration,
                logService,
                commonService,
                cacheService,
                cacheRepo,
                smsLogService,
                httpContextAccessor,
                viewRenderService,
                emailLogService)
        {
            _appConfig = CommonLib.GetAppConfig<AppConfig>(configuration);
            _DataService = cmsuserDataService;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;

            Dt = DateTime.Now;
            _InventoryRole= InventoryRole;
            LoginUser = GetLoginUser();
            _otpDataService = otpDataService;
            Initiated = true;
        }

        public async Task<ResJsonOutput> hasAccess(string ControllerName, string ActionName)
        {
            ResJsonOutput result = await HasPermission(ControllerName, ActionName);
            if (result.Status.IsSuccess == false)
            {
                if (result.Status.StatusCode == Enums.StatusCode.SESEXP.ToString())
                {
                    string coockieName = CommonLib.GetApplicationName();
                    if (_httpContextAccessor.HttpContext.Request.Cookies[coockieName] != null)
                    {
                        string cookieValueFromReq = EncryptDecrypt.Decrypt(_httpContextAccessor.HttpContext.Request.Cookies[coockieName].IsNullString(), _appConfig.CookieConfigs.Key, _appConfig.CookieConfigs.IV);
                        UserViewModel loginmodel = CommonLib.ConvertJsonToObject<UserViewModel>(cookieValueFromReq);
                        if (loginmodel != null)
                        {
                            result = await VerifyUser(loginmodel);
                            if (result.Status.IsSuccess == true)
                            {
                                return await HasPermission(ControllerName, ActionName);
                            }
                        }
                    }
                    else
                    {
                        ClearSession();
                    }
                }
            }
            if (result.Status.IsSuccess == false)
            {
                result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
            }
            return result;
        }

        public T GetSessionValue<T>(string key) where T : class
        {
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    return CommonLib.ConvertJsonToObject<T>(_httpContextAccessor.HttpContext.
                           Session.GetString(key));
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }

        public async Task<ResJsonOutput> CheckSession()
        {
            ResJsonOutput result = new ResJsonOutput();
            InventoryUser cMSUser = GetSessionValue<InventoryUser>(ProgConstants.SessionLoggedInUser);
            if (cMSUser != null)
            {
                result = new ResJsonOutput(); //await PostDataAsync("/DDLists/CheckSession");
                result.Status.IsSuccess = cMSUser.InventoryUserId > 0;

                if (result.Status.IsSuccess == false)
                {
                    string coockieName = CommonLib.GetApplicationName();
                    if (_httpContextAccessor.HttpContext.Request.Cookies[coockieName] != null)
                    {
                        string cookieValueFromReq = EncryptDecrypt.Decrypt(_httpContextAccessor.HttpContext.Request.Cookies[coockieName].IsNullString(), _appConfig.CookieConfigs.Key, _appConfig.CookieConfigs.IV);
                        UserViewModel loginmodel = CommonLib.ConvertJsonToObject<UserViewModel>(cookieValueFromReq);
                        if (loginmodel != null)
                        {
                            ResJsonOutput loginResult = await VerifyUser(loginmodel);
                            if (loginResult.Status.IsSuccess == false)
                            {
                                //ClearSession().Wait();
                                var httpContext = _httpContextAccessor.HttpContext;
                                var TempData = _tempDataDictionaryFactory.GetTempData(httpContext);
                                TempData[ProgConstants.ErrMsg] = result.Status.Message;

                                _httpContextAccessor.HttpContext.Session.SetString(ProgConstants.ErrMsg, result.Status.Message);
                            }
                            result.Status.IsSuccess = loginResult.Status.IsSuccess;
                        }
                    }
                }
            }
            return result;
        }
        string GetCookieValueFromResponse(HttpResponse response, string cookieName)
        {
            foreach (var headers in response.Headers.Values)
                foreach (var header in headers)
                    if (header.StartsWith($"{cookieName}="))
                    {
                        var p1 = header.IndexOf('=');
                        var p2 = header.IndexOf(';');
                        return header.Substring(p1 + 1, p2 - p1 - 1);
                    }
            return null;
        }
        public InventoryUser GetLoginUser()
        {
            if (Initiated == true) return LoginUser;
            if (LoginUser == null)
            {
                string SessionKey = GetSessionValue<string>(ProgConstants.Authorization);
                string? RequestCookieKey = _httpContextAccessor.HttpContext?.Request.Cookies[ProgConstants.Authorization];
                string ResponseCookieKey = GetCookieValueFromResponse(_httpContextAccessor.HttpContext.Response, ProgConstants.Authorization);

                bool ResponseChecked = false;
                if (SessionKey != null && ResponseCookieKey != null)
                {
                    if (SessionKey.Equals(ResponseCookieKey))
                    {
                        ResponseChecked = true;
                    }
                    else
                    {
                        ClearSession();
                        return null;
                    }
                }

                if (!ResponseChecked && SessionKey != null && RequestCookieKey != null && !SessionKey.Equals(RequestCookieKey))
                {
                    //ClearSession();
                    return null;
                }

                ResJsonOutput result = CheckSession().GetAwaiter().GetResult();
                LoginUser = GetSessionValue<InventoryUser>(ProgConstants.SessionLoggedInUser);
            }
            //if (LoginUser != null && LoginCMSUserPermissions == null)
            //{
            //    LoginCMSUserPermissions = GetAllowedCMSPermissions(LoginUser.InventoryRoleId).GetAwaiter().GetResult();
            //}
            return LoginUser;
        }
        public async Task<SidebarData> GetSidebarData()
        {
            try
            {
                if (LoginUser != null)
                {
                    SidebarData model = new SidebarData();
                    //List<VMSidebarList> leftMenu = await _cacheRepo.LeftMenuList();
                    List<VMSidebarList>? leftMenu = null;

                    model.VMSidebarLists = leftMenu.Where(x => LoginUserPermissions.Select(y => y.ControllerName).Contains(x.ControllerName)).ToList();

                    //List<InventoryPermission> cmsPermissions = await _cacheRepo.CMSPermissionsList();
                    List<InventoryPermission> inventoryPermissions = null;

                    model.InvalidLinks = inventoryPermissions.Where(x =>
                    x.Disabled == true
                    || !LoginUserPermissions.Select(y => y.ControllerName + " " + y.ActionName).Contains(x.ControllerName + " " + x.ActionName)
                    ).Select(x => new RequestByString()
                    {
                        id = x.ControllerName + "/" + x.ActionName
                    }).ToList();

                    //model.AllowedFiles = await _cacheRepo.AllowedFilesList();
                    model.AllowedFiles = null;
                    return model;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new SidebarData();
        }

        public async Task<ResJsonOutput> VerifyUser(UserViewModel loginModel)
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                InventoryUser model = new InventoryUser();
                bool CreateUserSession = false;

               

                if (loginModel == null)
                {
                    result.Status.StatusCode = Enums.StatusCode.LOGFLD.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
                    return result;
                }
                else 
                {
                    SFInventoryUserDetails sFPMSGetUserDetails = new SFInventoryUserDetails()
                    {
                        UserName = loginModel.UserName,
                        Password =  loginModel.Password 
                        
                    };
                    SFUserData userDetail = (await ExecuteSPAsync<DBEntities, SFUserData>(sFPMSGetUserDetails)).FirstOrDefault();
                    if (sFPMSGetUserDetails.IsSuccess && userDetail != null)
                    {
                            model.InventoryUserId = userDetail.InventoryUserId;
                            model.InventoryRoleId = userDetail.InventoryRoleId;
                            model.FirstName = userDetail.FirstName;
                            model.LastName = userDetail.LastName;
                            model.CompanyName = userDetail.CompanyName;
                            model.ContactNo = userDetail.ContactNo;
                            model.InventoryPermissionIds = userDetail.InventoryPermissionIds;
                            model.InventoryRole = await _InventoryRole.GetSingle(x => x.InventoryRoleId == model.InventoryRoleId);

                            CreateUserSession = true;

                        inventoryRole = await _InventoryRole.GetSingle(x => x.InventoryRoleId == model.InventoryRoleId);

                    }
                    else
                    {
                        result.Status.StatusCode = Enums.StatusCode.LOGFLD.ToString();
                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
                        return result;
                    }

                }

                if (CreateUserSession)
                {

                    LoginUserPermissions = await GetAllowedCMSPermissions(model.InventoryPermissionIds);
                    
                    await SetUserSessions(model);
                    SetLoginCookies(loginModel);
                    result.Data = model;
                    result.Status.IsSuccess = true;
                }

            }
            catch (Exception ex)
            {
                result.Status.Message = ex.Message; // CommonLib.CatchError(ex, ModelName);
            }
            return result;
        }

        public async Task SetUserSessions(InventoryUser User)
        {
            string MyGuid = Guid.NewGuid().ToString();
            //await _cacheService.Write(CacheChannels.AdminPortal, User.InventoryUserId.ToString() + "-" + User.UserName, _httpContextAccessor.HttpContext.Session.Id.ToString());
            _httpContextAccessor.HttpContext.Session.SetString(ProgConstants.Authorization, MyGuid);
            _httpContextAccessor.HttpContext.Session.SetString(ProgConstants.SessionLoggedInUser, CommonLib.ConvertObjectToJson(User));
            _httpContextAccessor.HttpContext.Response.Cookies.Append(ProgConstants.Authorization, MyGuid);
            LoginUser = User;

        }

        public void SetLoginCookies(UserViewModel logindata)
        {
            string coockieName = CommonLib.GetApplicationName();
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(_appConfig.CookieConfigs.Expiry)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(coockieName, EncryptDecrypt.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(logindata), _appConfig.CookieConfigs.Key, _appConfig.CookieConfigs.IV), option);
        }


        public async Task<List<VMAllowedCMSPermission>> GetAllowedCMSPermissions(string ? InventoryPermissionIds)
        {
            if (LoginUserPermissions != null) return LoginUserPermissions;
            //try
            //{
            //  //  List<CMSRole> cmsRoles = await _cacheRepo.CMSRolesList();
            //    List<InventoryPermission> cmsPermissions = await _cacheRepo.CMSPermissionsList();

            //    CMSRole cmsRole = cmsRoles.FirstOrDefault(x => x.CMSRoleId == CMSRoleId);

            //    if (cmsRole != null && cmsRole.Disabled == false)
            //    {
            //        LoginUserPermissions = cmsPermissions
            //            .Where(x => CommonLib.GetIdList(cmsRole.CMSPermissionIds).Contains(x.CMSPermissionId)
            //            && x.Disabled == false)
            //            .Select(x => new VMAllowedCMSPermission()
            //            {
            //                ActionName = x.ActionName,
            //                ControllerName = x.ControllerName
            //            })
            //            .ToList();
            //        return LoginUserPermissions;
            //    }
            //}
            //catch
            //{
            //    throw;
            //}
            return null;
        }
        public async Task<ResJsonOutput> HasPermission(string ControllerName, string ActionName)
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                if (LoginUser == null)
                {
                    result.Status.StatusCode = Enums.StatusCode.SESEXP.ToString();
                    return result;
                }
                result.Status.IsSuccess = true;
                //List<InventoryRole> cmsRoles = await _cacheRepo.CMSRolesList();
                //List<InventoryPermission> cmsPermissions = await _cacheRepo.CMSPermissionsList();
                //List<InventoryRole> cmsRoles = null;
                //List<InventoryPermission> cmsPermissions = null;

                //InventoryRole cmsRole = cmsRoles.FirstOrDefault(x => x.InventoryRoleId == LoginUser.InventoryRoleId);

                //if (cmsRole != null && cmsRole.Disabled == false)
                //{
                //    //result.Status.IsSuccess = cmsPermissions
                //    //    .Any(x => CommonLib.GetIdList(cmsRole.CMSPermissionIds).Contains(x.CMSPermissionId)
                //    //    && x.Disabled == false
                //    //    && x.ControllerName.Equals(ControllerName, StringComparison.OrdinalIgnoreCase)
                //    //    && x.ActionName.Equals(ActionName, StringComparison.OrdinalIgnoreCase)
                //    //    );
                //}
                //if (!result.Status.IsSuccess)
                //{
                //    result.Status.StatusCode = Enums.StatusCode.ACCERR.ToString();
                //}
            }
            catch
            {
                throw;
            }
            return result;
        }
        public async Task<ResJsonOutput> SendOTP(OTPbyMobile model)
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {

                DateTime Dt = DateTime.Now;

                OTP otp = new OTP
                {
                    ChannelId = _appConfig.tokenConfigs.ChannelId,
                    LoginId = model.LoginId,
                    OTPFor = model.OTPFor,
                    //MobileNo = (_appConfig.smsConfigs.Mode) ? model.MobileNo : _appConfig.smsConfigs.DefaultNumber, //model.MobileNumber;
                    MobileNo = model.MobileNo,
                    EmailId = model.EmailId,
                    ValidFrom = Dt,
                    ValidUpto = Dt.AddMinutes(_appConfig.otpConfigs.Expiry),

                    OTPNumber = (model.LoginId == _appConfig.otpConfigs.TestRefId && model.LoginId.IsNullString() != "" && _appConfig.otpConfigs.TestRefId.IsNullString() != "") ? new string(_appConfig.otpConfigs.DefaultOTP, _appConfig.otpConfigs.Length) : GenerateOTP()
                };

                Dictionary<string, string> Args = new Dictionary<string, string>() {
                    { "OTP", otp.OTPNumber } ,
                    { "ExpiryMinutes", _appConfig.otpConfigs.Expiry.ToString() }
                };

                if (model.Args == null)
                {
                    model.Args = new Dictionary<string, string>();
                }
                foreach (var item in Args)
                {
                    model.Args.Add(item.Key, item.Value);
                }

                MBNotification notification = new MBNotification()
                {
                    TemplateCode = model.OTPFor,
                    Args = { { "OTP", otp.OTPNumber }, { "Scheme", _appConfig.smsConfigs.SchemeName.ToString() } },
                    RefId = model.LoginId,
                    ChannelId = otp.ChannelId,
                    MobileNo = model.MobileNo,
                    EmailId = model.EmailId
                };
                ResJsonOutput NotficationResult = new ResJsonOutput();
                NotficationResult = await SendNotification(notification);

                otp.Status = NotficationResult.Status.IsSuccess;

                if (otp.Status == true)
                {
                    await _otpDataService.Create(otp);
                    await _otpDataService.Save();

                    result.Status.IsSuccess = true;
                }
                else
                {
                    result.Status.IsSuccess = false;
                    result.Status.StatusCode = RootEnums.StatusCodes.OTPFLD.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
                }
                //await _staticService.Commit(); // do not move this code to above this lines, it has some reason

              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        
        //public async Task<bool> ResetPassword(ForgotUserPassword model)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        //        long expirationTime = timestamp + ProgConstants.EmailtSentSeconds;

        //        string PasswordResetCode = CommonLib.EpochTime();
        //        string resetPasswordLink = $"{_appConfig.ResetPasswordLink}{HtmlEncoder.Default.Encode(PasswordResetCode)}&timestamp={expirationTime}";
        //        SFUpdatePasswordResetCodeCMSUser sFUpdatePasswordResetCodeCMSUser = new SFUpdatePasswordResetCodeCMSUser
        //        {
        //            EmailId = model.EmailId,
        //            UserName = model.UserName,
        //            PasswordResetCode = PasswordResetCode,
        //            ContactNo = model.ContactNo,
        //            Password = Regex.Replace(Guid.NewGuid().ToString(), "[^0-9a-zA-Z]+", "").Substring(0, 10),
        //            CMSRoleId = model.CMSRoleId
        //        };

        //        flag = await ExecuteScalarAsync<DBEntities, bool>(sFUpdatePasswordResetCodeCMSUser);

        //        #region  According to one of the BRD(CHANGE IN PASSWORD RESET FUNCTIONALITY FOR GRAM PRADHANS_09_02_2024) I have made changes so that If a user selects mobile no then a sms notification will be trigger on mobile and If user selects email then a email will be triggered with the reset password link."  

        //        if (flag)
        //        {
        //            MBNotification notification = new MBNotification();

        //            if (!string.IsNullOrEmpty(model.EmailId) && string.IsNullOrEmpty(model.ContactNo))
        //            {
        //                notification.TemplateCode = Enums.NotificationTemplateCodes.PM_Vishwakarma_Forgot_Password.ToString();
        //                notification.EmailId = model.EmailId;
        //                notification.Args = new Dictionary<string, string> {
        //    { "var", $"<a href='{resetPasswordLink}' style='color: blue;'>Reset Password</a>" }
        //};
        //            }

        //            if (string.IsNullOrEmpty(model.EmailId) && !string.IsNullOrEmpty(model.ContactNo))
        //            {
        //                notification.TemplateCode = Enums.NotificationTemplateCodes.PM_Vishwakarma_GP_ForgotPasswordMobile.ToString();
        //                notification.MobileNo = model.ContactNo;
        //                notification.Args = new Dictionary<string, string> { { "var", sFUpdatePasswordResetCodeCMSUser.Password } };
        //            }

        //            ResJsonOutput NotficationResult = await SendNotification(notification);
        //        }
        //    }
        //    #endregion

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return flag;
        //}
        //public async Task<ResJsonOutput> CheckCMSUserExists(string ContactNo)
        //{
        //    ResJsonOutput result = new ResJsonOutput();
        //    try
        //    {
        //        SPCheckCMSUserExists spApplicantExists = new SPCheckCMSUserExists
        //        {
        //            ContactNo = ContactNo
        //        };
        //        CMSUser applicant = (await ExecuteSPAsync<DBEntities, CMSUser>(spApplicantExists)).FirstOrDefault();
        //        if (applicant != null)
        //        {
        //            result.Data = applicant;
        //            result.Status.IsSuccess = true;
        //        }
        //        else
        //        {
        //            result.Status.StatusCode = Enums.StatusCode.NOTFND.ToString();
        //            result.Status.Message = result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string> { "Applicant" });
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return result;
        //}

        //public async Task<ResJsonOutput> ValidatOTP(RootValidateOTP model)
        //{
        //    ResJsonOutput result = new ResJsonOutput();
        //    try
        //    {
        //        OTP otp = await _otpDataService.GetSingle(o => o.RefCode == Guid.Parse(model.RefCode) && o.OTPNumber == model.OTPNumber && o.ChannelId == _appConfig.tokenConfigs.ChannelId);

        //        if (_appConfig.otpConfigs.Mode == false && _appConfig.smsConfigs.Mode == true)
        //        {
        //            model.OTPNumber = new string(_appConfig.otpConfigs.DefaultOTP, _appConfig.otpConfigs.Length);
        //        }
        //        if (otp != null)
        //        {
        //            if (otp.Status == true)
        //            {
        //                if (otp.IsVerified.HasValue)
        //                {
        //                    //otp.IsVerified = false;
        //                    result.Status.StatusCode = RootEnums.StatusCodes.OTPEXP.ToString();
        //                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //                }
        //                else if (otp.ValidUpto < DateTime.Now)
        //                {
        //                    otp.IsVerified = false;
        //                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //                }
        //                else if (otp.OTPNumber != model.OTPNumber)
        //                {
        //                    otp.Attempts += 1;
        //                    if (otp.Attempts >= _appConfig.otpConfigs.Attempts)
        //                    {
        //                        otp.IsVerified = false;
        //                        result.Status.StatusCode = RootEnums.StatusCodes.OTPATMEX.ToString();
        //                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //                    }
        //                    else
        //                    {
        //                        result.Status.StatusCode = RootEnums.StatusCodes.OTPATM.ToString();
        //                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string> { (_appConfig.otpConfigs.Attempts - otp.Attempts).ToString() });
        //                    }
        //                }
        //                else
        //                {
        //                    otp.IsVerified = true;
        //                    result.Data = otp;
        //                    result.Status.IsSuccess = true;
        //                }
        //            }
        //            else
        //            {
        //                result.Status.StatusCode = RootEnums.StatusCodes.OTPINV.ToString();
        //                result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //            }
        //        }
        //        else
        //        {
        //            otp = await _otpDataService.GetSingle(o => o.RefCode == Guid.Parse(model.RefCode) && o.ChannelId == _appConfig.tokenConfigs.ChannelId);
        //            if (otp != null)
        //            {
        //                otp.Attempts += 1;
        //                if (otp.Attempts >= _appConfig.otpConfigs.Attempts)
        //                {
        //                    otp.IsVerified = false;
        //                    result.Status.StatusCode = RootEnums.StatusCodes.OTPATMEX.ToString();
        //                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //                }
        //                else
        //                {
        //                    result.Status.StatusCode = RootEnums.StatusCodes.OTPATM.ToString();
        //                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string> { (_appConfig.otpConfigs.Attempts - otp.Attempts).ToString() });
        //                }
        //            }
        //            else
        //            {
        //                result.Status.StatusCode = RootEnums.StatusCodes.OTPINV.ToString();
        //                result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //            }
        //        }
        //        if (otp != null)
        //        {
        //            _otpDataService.Update(otp);
        //            await _otpDataService.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return result;
        //}

        //public async Task<ResJsonOutput> SendOTP(OTPbyMobile model)
        //{
        //    ResJsonOutput result = new ResJsonOutput();
        //    try
        //    {
        //        bool isAlready = await _cacheService.KeyExists(CacheChannels.AdminPortal, "OTP:" + model.MobileNo + "-" + model.OTPFor);
        //        if (isAlready && (LoginCMSUser.PasswordChanged == null || LoginCMSUser.PasswordChanged == false))
        //        {
        //            string OTPTokenData = await _cacheService.Read(CacheChannels.AdminPortal, "OTP:" + model.MobileNo + "-" + model.OTPFor);
        //            OTPToken oTPToken = CommonLib.ConvertJsonToObject<OTPToken>(OTPTokenData);
        //            result.Data = oTPToken;
        //            result.Status.IsSuccess = true;
        //            result.Status.Message = "OTP Already exists";
        //            return result;
        //        }
        //        DateTime Dt = DateTime.Now;
        //        OTP otp = new OTP
        //        {
        //            ChannelId = _appConfig.tokenConfigs.ChannelId,
        //            LoginId = model.LoginId,
        //            OTPFor = model.OTPFor,
        //            MobileNo = model.MobileNo,
        //            EmailId = model.EmailId,
        //            ValidFrom = Dt,
        //            ValidUpto = Dt.AddMinutes(_appConfig.otpConfigs.Expiry),
        //            OTPNumber = (model.LoginId == _appConfig.otpConfigs.TestRefId && model.LoginId.IsNullString() != "" && _appConfig.otpConfigs.TestRefId.IsNullString() != "") ? new string(_appConfig.otpConfigs.DefaultOTP, _appConfig.otpConfigs.Length) : GenerateOTP()
        //        };

        //        Dictionary<string, string> Args = new Dictionary<string, string>() {
        //            { "OTP", otp.OTPNumber } ,
        //            { "ExpiryMinutes", _appConfig.otpConfigs.Expiry.ToString() }
        //        };

        //        if (model.Args == null)
        //        {
        //            model.Args = new Dictionary<string, string>();
        //        }
        //        foreach (var item in Args)
        //        {
        //            model.Args.Add(item.Key, item.Value);
        //        }

        //        MBNotification notification = new MBNotification()
        //        {
        //            TemplateCode = model.OTPFor,
        //            Args = { { "var", otp.OTPNumber }, { "Scheme", _appConfig.smsConfigs.SchemeName.ToString() } },
        //            RefId = model.LoginId,
        //            ChannelId = otp.ChannelId,
        //            MobileNo = model.MobileNo,
        //            EmailId = model.EmailId
        //        };
        //        ResJsonOutput NotficationResult = new ResJsonOutput();
        //        NotficationResult = await SendNotification(notification);

        //        otp.Status = NotficationResult.Status.IsSuccess;

        //        if (otp.Status == true)
        //        {
        //            await _otpDataService.Create(otp);
        //            await _otpDataService.Save();

        //            result.Status.IsSuccess = true;
        //        }
        //        else
        //        {
        //            result.Status.IsSuccess = false;
        //            result.Status.StatusCode = RootEnums.StatusCodes.OTPFLD.ToString();
        //            result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
        //        }
        //        //await _staticService.Commit(); // do not move this code to above this lines, it has some reason

        //        if (otp.Status == true)
        //        {
        //            OTPToken OTPTokenData = new OTPToken() { RefCode = otp.RefCode.ToString(), Expiry = _appConfig.otpConfigs.Expiry, ExpiryTime = otp.ValidUpto };
        //            result.Data = OTPTokenData;

        //            await _cacheService.Write(CacheChannels.AdminPortal, "OTP:" + model.MobileNo + "-" + model.OTPFor, CommonLib.ConvertObjectToJson(OTPTokenData), _appConfig.otpConfigs.Expiry);
        //            string OtpKey = otp.RefCode.ToString() + "-" + otp.OTPNumber;
        //            await _cacheService.Write(CacheChannels.AdminPortal, "OTP:" + OtpKey, CommonLib.ConvertObjectToJson(OTPTokenData), _appConfig.otpConfigs.Expiry);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return result;
        //}

        //private string GenerateOTP()
        //{
        //    if (_appConfig.otpConfigs.Mode)
        //    {
        //        return GenerateKey(_appConfig.otpConfigs.Length);
        //    }
        //    else
        //    {
        //        return new string(_appConfig.otpConfigs.DefaultOTP, _appConfig.otpConfigs.Length);
        //    }
        //}

        private string GenerateKey(int length)
        {
            string key = Regex.Replace(Guid.NewGuid().ToString(), "[^0-9]+", "");
            if (key.Length < length)
            {
                key = key + GenerateKey(length - key.Length);
            }
            key = key.Substring(0, length);
            return key;
        }


        public void ClearSession()
        {
            foreach (string cookie in _httpContextAccessor.HttpContext.Request.Cookies.Keys)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie);
            }
            _httpContextAccessor.HttpContext.Session.Clear();
            //if (LoginUser != null)
                //_cacheService.Delete(CacheChannels.AdminPortal, LoginCMSUser.CMSUserId.ToString() + "-" + LoginCMSUser.UserName);
           
        }

        //public async Task<bool> VerifyPasswordRequest(RequestByString model)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        string timestampValue = "";
        //        int index = model.id.IndexOf("timestamp=");

        //        if (index != -1)
        //        {
        //            // Extract the portion after 'timestamp='
        //            timestampValue = model.id.Substring(index + "timestamp=".Length);

        //        }
        //        timestamp = long.Parse(timestampValue);
        //        long currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        //        if (currentTimestamp - timestamp > ProgConstants.EmailExpirationSeconds)
        //        {
        //            return false;
        //        }
        //        SFResetPasswordCMSUser sFResetPasswordCMSUser = new SFResetPasswordCMSUser
        //        {
        //            PasswordResetCode = model.id.Split('&')[0],
        //            Password = Regex.Replace(Guid.NewGuid().ToString(), "[^0-9a-zA-Z]+", "").Substring(0, 10)
        //        };
        //        flag = await ExecuteScalarAsync<DBEntities, bool>(sFResetPasswordCMSUser);


        //        if (flag)
        //        {
        //            string FullName = sFResetPasswordCMSUser.FullName;
        //            string ContactNo = sFResetPasswordCMSUser.ContactNo;
        //            string EmailId = sFResetPasswordCMSUser.EmailId;
        //            MBNotification notification = new MBNotification()
        //            {
        //                TemplateCode = Enums.NotificationTemplateCodes.PM_Vishwakarma_Update_Password.ToString(),
        //                EmailId = EmailId,
        //                Args = { { "var", FullName }, { "var1", sFResetPasswordCMSUser.Password } }
        //            };

        //            ResJsonOutput NotficationResult = await SendNotification(notification);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return flag;
        //}

    }
}
