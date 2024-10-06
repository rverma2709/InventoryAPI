using App.AdminPortal.Common;
using App.AdminPortal.Models;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;



namespace App.AdminPortal.Controllers
{
    public class HomeController : AdminPortalController
    {
       

        public HomeController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "Home")
        {
            
        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (LoginUser == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                await CatchError(ex);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
