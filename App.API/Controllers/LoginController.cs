using App.API.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Mvc;
using Root.Models.Application.Tables;
using Root.Models.StoredProcedures;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using Root.Services.Services;

namespace App.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : DefaultController
    {
        private readonly IConfiguration _configuration;
        public LoginController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(staticService, httpContextAccessor)
        {
            _configuration = configuration;
        }

        [Route("LoginDetails"), HttpPost]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> LoginDetails([FromBody] UserViewModel userViewModel)
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            if (ModelState.IsValid)
            {
                SFInventoryUserDetails sFPMSGetUserDetails = new SFInventoryUserDetails()
                {
                    UserName = userViewModel.UserName,
                    Password = userViewModel.Password

                };
                SFUserData userDetail = (await _staticService.ExecuteSPAsync<DBEntities, SFUserData>(sFPMSGetUserDetails)).FirstOrDefault();
                if (sFPMSGetUserDetails.IsSuccess && userDetail != null)
                {

                    JwtTokenService jwtTokenService = new JwtTokenService(_configuration);
                    resJsonOutput.token=jwtTokenService.GenerateJwtToken(userDetail.InventoryUserId.ToString(), userDetail.FirstName);
                    resJsonOutput.Data = userDetail;
                    resJsonOutput.Status.IsSuccess = true;
                  

                }
                else
                {
                    
                    resJsonOutput.Status.IsSuccess = false;
                    resJsonOutput.Status.Message = "Login Details Wrong";
                }
            }
            return resJsonOutput;
        }
    }
}
