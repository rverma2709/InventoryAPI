using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Root.Models.Config;
using Root.Models.Utils;
using Root.Services.Common;
using Root.Services.Services;


namespace Root.Services.Controller
{
    [ServiceFilter(typeof(RequestLogger))]
    [ServiceFilter(typeof(ExceptionLogger))]
   
    public class DefaultController : Microsoft.AspNetCore.Mvc.Controller
    {
        #region Variables
        protected DateTime Dt;
        protected readonly CommonAppConfig _appConfig;
        protected readonly StaticService _staticService;
        protected string ModelName = "Record";
        protected readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        public DefaultController(StaticService staticService, IHttpContextAccessor httpContextAccessor)
        {

            _staticService = staticService;
            _appConfig = staticService.GetAppConfig();
            _httpContextAccessor = httpContextAccessor;
            Dt = DateTime.Now;
            _staticService.ModelName = ModelName;
        }

        #region Methods
        
        protected async Task<ResJsonOutput> CatchError(Exception ex, string modelName = "Record")
        {
            return await _staticService.HandleError(RouteData, HttpContext, ex, modelName);
        }


       
        protected async Task<string> GetStatusMessage(string statusCode, List<string> str = null)
        {
            return await _staticService.GetStatusMessage(statusCode, str);
        }

        /// <summary>
        /// This is common method to get model state error
        /// </summary>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        protected ResJsonOutput GetModelStateErrors(ModelStateDictionary ModelState)
        {
            ResJsonOutput result = new ResJsonOutput();
            result.Status.StatusCode = RootEnums.StatusCodes.REQFLD.ToString();
            result.Status.Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return result;
        }

        protected List<string> UpdateFileName(object model)
        {
            return _staticService.UpdateFileName(model);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }




    }
}
