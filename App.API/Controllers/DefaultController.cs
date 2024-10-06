using App.APIServices.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.API.Controllers
{
    [Produces("application/json")]
    [Route("API/Default")]
    public class DefaultController : Root.Services.Controller.DefaultController
    {
        
        protected readonly AppConfig _appConfig;
        protected new readonly App.APIServices.Services.APIStaticService _staticService;
        public DefaultController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor)
        {
            _appConfig = staticService._appConfig;
            _staticService = staticService;
        }
        protected internal string GetModelStateErrors(ModelStateDictionary ModelState)
        {
            //result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
            return string.Join("<br />", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }
    }
}
