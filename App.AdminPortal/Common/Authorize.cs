using App.AdminPortal.Controllers;
using App.AdminPortalServices.Models;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Root.Models.Application.Tables;
using Root.Models.Utils;
using System.Reflection;
using Root.Models.Tables;

namespace App.AdminPortal.Common
{
    public class Authorize : ActionFilterAttribute
    {
        protected readonly AdminPortalStaticService _staticService;
        protected readonly AppConfig _appConfig;
        protected readonly bool _Ajax;
        protected string _DActionName;
        protected string _DControllerName;
        protected string _ActionName;
        protected string _ControllerName;
        
        public Authorize(AdminPortalStaticService staticService, IConfiguration configuration, bool Ajax = true, string DActionName = "", string DControllerName = "", string ActionName = "", string ControllerName = "") 
        {
            _staticService = staticService;
            _appConfig = CommonLib.GetAppConfig<AppConfig>(configuration);
            _Ajax = Ajax;
            _DActionName = DActionName;
            _DControllerName = DControllerName;
            _ActionName = ActionName;
            _ControllerName = ControllerName;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AdminPortalController? controller = context.Controller as AdminPortalController;
            InventoryUser? LoginCMSUser = controller?.LoginUser;
            string RertunType = AuthClass.GetExpectedReturnType(context).Name;
            if (_Ajax && !CommonLib.IsAjaxRequest(context.HttpContext.Request))
            {
                if (RertunType != "Void" || RertunType.Contains("Task"))
                {
                    context.HttpContext.Session.SetObject(ProgConstants.CurrURL, context.HttpContext.Request.Path.ToString());
                }

                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", (context?.RouteData?.Values["controller"]?.ToString() == "Default") ? "Home" : context?.RouteData?.Values["controller"]?.ToString() }, { "action", "Index" } });
                return;
            }

            ControllerActionDescriptor? controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            _ActionName = (_ActionName != "") ? _ActionName : controllerActionDescriptor.ActionName;
            _ControllerName = (_ControllerName != "") ? _ControllerName : controllerActionDescriptor.ControllerName;
           
            try
            {
                //CMSUser LoginCMSUser = _staticService.GetLoginUser();

                if (LoginCMSUser == null)
                {
                    _staticService.ClearSession();
                    context.Result = AuthClass.UnAuthAccess(context, RertunType);
                    return;
                }
                //else if ((!LoginCMSUser.PMSDesignationId.HasValue && LoginCMSUser.CMSRoleId != _staticService._appConfig.ULBRoleId) && (LoginCMSUser.PasswordChanged == false || LoginCMSUser.PasswordChanged == null))
                //{
                //    context.Result = AuthClass.PasswordChanged(context, RertunType);
                //    return;
                //}
                else
                {
                    System.Threading.Tasks.Task<ResJsonOutput> hasaccess = _staticService.hasAccess(_ControllerName, _ActionName);
                    hasaccess.Wait();
                    if (!hasaccess.Result.Status.IsSuccess)
                    {
                        if (hasaccess.Result.Status.StatusCode == Enums.StatusCode.SESEXP.ToString())
                        {
                            _staticService.ClearSession();
                        }
                        context.Result = AuthClass.UnAuthAccess(context, RertunType);
                        return;
                    }
                }
            }
            catch
            {
                context.Result = AuthClass.UnAuthAccess(context, RertunType);
                return;
            }
        }
    }

    public class AjaxRequest : ActionFilterAttribute
    {
        protected readonly AdminPortalStaticService _staticService;
        //protected readonly AppConfig _appConfig;
        public AjaxRequest(AdminPortalStaticService staticService) //, IConfiguration configuration, bool IsTrusted = false)
        {
            _staticService = staticService;
            //this._appConfig = CommonLib.GetAppConfig<AppConfig>(configuration);
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string RertunType = AuthClass.GetExpectedReturnType(filterContext).Name;

            try
            {
                AdminPortalController controller = filterContext.Controller as AdminPortalController;
                InventoryUser LoginCMSUser = controller.LoginUser;

                if (LoginCMSUser == null)
                {
                    filterContext.Result = AuthClass.UnAuthAccess(filterContext, RertunType);
                    return;
                }
                else if (LoginCMSUser.PasswordChanged == true)
                {
                    filterContext.Result = AuthClass.PasswordChanged(filterContext, RertunType);
                    return;
                }
            }
            catch
            {
                filterContext.Result = AuthClass.UnAuthAccess(filterContext, RertunType);
                return;
            }
        }
    }

    public static class AuthClass
    {
        public static IActionResult UnAuthAccess(ActionExecutingContext filterContext, string RertunType)
        {
            //if (RertunType == "Void" || RertunType.Contains("Task"))
            //         {
            //             return new ViewResult { ViewName = "~/Views/Shared/Reload.cshtml", ViewData = ((Controller)filterContext.Controller).ViewData};
            //         }
            return new ViewResult
            {
                ViewName = "~/Views/Shared/Authfailed.cshtml",
                ViewData = ((Controller)filterContext.Controller).ViewData
            };
        }
        public static IActionResult PasswordChanged(ActionExecutingContext filterContext, string RertunType)
        {
            
            return new ViewResult
            {
                ViewName = "~/Views/CMSUsers/ChangePassword.cshtml",
                ViewData = ((Controller)filterContext.Controller).ViewData
            };
        }

        public static Type GetExpectedReturnType(ActionExecutingContext filterContext)
        {
            // Find out what type is expected to be returned
            ControllerActionDescriptor? controllerActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;

            string actionName = controllerActionDescriptor.ActionName;
            Type controllerType = filterContext.Controller.GetType();
            MethodInfo? actionMethodInfo = default(MethodInfo);
            try
            {
                actionMethodInfo = controllerType.GetMethod(actionName);
            }
            catch (AmbiguousMatchException)
            {
                // Try to find a match using the parameters passed through
                IList<Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor> actionParams = controllerActionDescriptor.Parameters;

                List<Type> paramTypes = new List<Type>();
                foreach (Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor p in actionParams)
                {
                    paramTypes.Add(p.ParameterType);
                }
                actionMethodInfo = controllerType.GetMethod(actionName, paramTypes.ToArray());
            }
            return actionMethodInfo.ReturnType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PreventDuplicateRequests : ActionFilterAttribute
    {
        private const string uniqformuid = "lastprocessedtoken";
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            IAntiforgery? antiforgery = context.HttpContext.RequestServices.GetService(typeof(IAntiforgery)) as IAntiforgery;
            AntiforgeryTokenSet? tokens = antiforgery.GetAndStoreTokens(context.HttpContext);

            if (!context.HttpContext.Request.Form.ContainsKey(tokens.FormFieldName))
            {
                return;
            }

            var currentformid = context.HttpContext.Request.Form[tokens.FormFieldName].ToString();
            var lasttoken = "" + context.HttpContext.Session.GetString(uniqformuid);

            if (lasttoken.Equals(currentformid))
            {
                context.ModelState.AddModelError(string.Empty, "Duplicate request found");
                context.Result = new StatusCodeResult(429);
                return;
            }
            context.HttpContext.Session.Remove(uniqformuid);
            context.HttpContext.Session.SetString(uniqformuid, currentformid);
            await context.HttpContext.Session.CommitAsync();
        }
    }
}
