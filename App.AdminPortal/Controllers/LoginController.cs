using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;

namespace App.AdminPortal.Controllers
{
    public class LoginController : AdminPortalController
    {
        private readonly IDataService<DBEntities, InventoryUser> _DPAservice;
        public LoginController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, InventoryUser> DPAservice) : base(staticService, httpContextAccessor, "Login")
        {
            _DPAservice = DPAservice;
        }

        public IActionResult Index()
        {
         
            if (LoginUserId != null)
            {
                return View();
            }

            UserViewModel model = new UserViewModel();
            return View(model);

        }
        [HttpPost]
        [PreventDuplicateRequests]
        public async Task<IActionResult> Index(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                ResJsonOutput result = await _staticService.VerifyUser(userViewModel);
                if(result.Status.IsSuccess)
                {
                    HttpContext.Session.SetObject(ProgConstants.SuccMsg, "succefully login");
                    return RedirectToAction("Index", "Home");
                }
            }
            HttpContext.Session.SetObject(ProgConstants.ErrMsg, "User Details Wrong Please Try Agin");
            return View(userViewModel);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOut()
        {
            _staticService.ClearSession();

            return RedirectToAction("Clear", "Login");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Clear()
        {
            return RedirectToAction("Index", "Login");
        }
    }
}
