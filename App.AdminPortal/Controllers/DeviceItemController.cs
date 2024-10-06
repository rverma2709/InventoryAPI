using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace App.AdminPortal.Controllers
{
    public class DeviceItemController : AdminPortalController
    {
        private readonly IDataService<DBEntities, InventoryUser> _InventoryUser;
      
        public DeviceItemController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, InventoryUser> InventoryUser) : base(staticService, httpContextAccessor, "Device Item List")
        {
           
            _InventoryUser = InventoryUser;
        }

        public async Task<IActionResult> DeviceItemList(SFGetDeviceItem sFGetDeviceItem)
        {
            
            sFGetDeviceItem.ReciverUserId = LoginUserId;
            List<DeviceItems> deviceItems = new List<DeviceItems>();
            List<InventoryRole> InventoryRoles=   new List<InventoryRole>();
            if (LoginUser.InventoryRoleId==2)
            {
                InventoryRoles = (await _staticService._cacheRepo.InventoryUsersRoles()).Where(x => x.InventoryRoleId != LoginUser.InventoryRoleId).ToList();
            }
            else
            {
                InventoryRoles = (await _staticService._cacheRepo.InventoryUsersRoles()).Where(x => x.InventoryRoleId == 2).ToList();
            }
                
                
            ViewBag.InventoryRoles = (InventoryRoles.Select(x => new { x.InventoryRoleId, x.RoleName }).ToList());
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            


            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Check for AJAX requests
            {
                try
                {
                    deviceItems = await _staticService.ExecuteSPAsync<DBEntities, DeviceItems>(sFGetDeviceItem);
                    

                }
                catch (Exception ex)
                {
                    await CatchError(ex);
                }
                return PartialView("_DeviceItemTable", Tuple.Create(deviceItems, sFGetDeviceItem));
            }
            return View(Tuple.Create(deviceItems, sFGetDeviceItem));
           
        }

        [HttpPost]
        public async  Task<ResJsonOutput> GetUserList(long? ReciverInventoryRoleId)
        {
            ResJsonOutput DDData = new ResJsonOutput();

            var gpil = (await _staticService.ExecuteSPAsync<DBEntities, GetUserdata>(new SFGetUserDetails() { InventoryRoleId = ReciverInventoryRoleId }));


            DDData.Data = CommonLib.ConvertObjectToJson(gpil);
            DDData.Status.IsSuccess = true;
            return DDData;
        }
        [HttpPost]
        public async Task<ResJsonOutput> OtpSend(long ? ReciverUserId)
        {
            InventoryUser inventoryUser =await _InventoryUser.GetSingle(x => x.InventoryUserId == ReciverUserId);
            ResJsonOutput jsonOutput = new ResJsonOutput();
            OTPbyMobile oTPbyMobile = new OTPbyMobile();
            oTPbyMobile.EmailId = inventoryUser.EmailId;
            oTPbyMobile.MobileNo = inventoryUser.ContactNo;
            oTPbyMobile.OTPFor = "OTP";
            jsonOutput =await _staticService.SendOTP(oTPbyMobile);
            if(jsonOutput.Status.IsSuccess)
            {
                jsonOutput.Status.Message = "Otp send successfully";
            }
            else
            {
                jsonOutput.Status.Message = "Otp not send successfully";
            }


            return jsonOutput;
        }
        [HttpPost]
        public async Task<ResJsonOutput> OtpVerification(long? OTP, long? ReciverUserId)
        {
            ResJsonOutput jsonOutput = new ResJsonOutput();
            //bool result = await _staticService.ExecuteScalarAsync<DBEntities, bool>(new SFOtpVarification() { InventoryUserId = ReciverUserId, OTP = OTP });
            jsonOutput.Status.IsSuccess =  await _staticService.ExecuteScalarAsync<DBEntities, bool>(new SFOtpVarification() { InventoryUserId=ReciverUserId,OTP=OTP});

            if (jsonOutput.Status.IsSuccess)
            {
                jsonOutput.Status.IsSuccess=true;
                jsonOutput.Status.Message = "Otp Verification Suceccefully";
            }
            else
            {
                jsonOutput.Status.IsSuccess = false;
                jsonOutput.Status.Message = "Otp Verification Failed";
            }


            return jsonOutput;
        }
        [HttpPost]
        public async Task<ResJsonOutput> DeviceDataMove(List<RequestById> DeviceId, DeviceMovementView deviceMovementView)
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<DeviceMovementList> deviceMovementLists = new List<DeviceMovementList>() {
               new DeviceMovementList()
               {
                   ReciverUserId = deviceMovementView.ReciverUserId,
                   SenderUserId = LoginUserId,
                   SendDate = deviceMovementView.SendDate,
                   QCStatus = deviceMovementView.QCStatus,
                   QCUserRemarks = deviceMovementView.QCUserRemarks,
                   DeviceOldserialNumber = deviceMovementView.DeviceOldserialNumber
               }
            };

                await _staticService.ExecuteNonQueryAsync<DBEntities>(new SFDeviceMovement()
                {
                    RecivingDevices = DeviceId,
                    DeviceMovementList = deviceMovementLists,
                    ReciverInventoryRoleId = deviceMovementView.ReciverInventoryRoleId,
                    SenderInventoryRoleId = LoginUser.InventoryRoleId
                });
                resJsonOutput.Status.IsSuccess = true;
                resJsonOutput.Status.Message = "Data Move Successfully";
                HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data Move Successfully");
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.IsSuccess = false;
                resJsonOutput.Status.Message = ex.Message;
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, ex.Message);
                await CatchError(ex);
            }
           

            return resJsonOutput;
        }
    }
}
