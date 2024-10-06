using App.API.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;

namespace App.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MasterDataController : DefaultController
    {
        public MasterDataController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor)
        {
        }
       // [Authorize]
        [Route("VenderDetails"), HttpPost]
       // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> VenderDetails()
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                 
                resJsonOutput.Data = (from c in (await _staticService._cacheRepo.InventoryUsersDetails()).Where(x => x.InventoryRoleId == 4).ToList() select new SelectListItem(){
                    Value=c.InventoryUserId.ToString(),
                    Text=c.FirstName,
                }).ToList();
                resJsonOutput.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }
        //[Authorize]
        [Route("AllMasterData"), HttpPost]
       // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> AllMasterData([FromBody] long id=1)
        {
            
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            GetAllMasterData getAllMasterData   = new GetAllMasterData();
            try
            {

                getAllMasterData.vendorDetails = (from c in (await _staticService._cacheRepo.InventoryUsersDetails()).Where(x => x.InventoryRoleId == 4).ToList()
                                      select new SelectListData()
                                      {
                                          Value = c.InventoryUserId,
                                          Text = c.FirstName,
                                      }).ToList();
                getAllMasterData.brandDetails = (from c in (await _staticService._cacheRepo.BrandDetails()).Where(x => x.DeviceTypeId == id).ToList()
                                                 select new SelectListData()
                                                  {
                                                      Value = c.BrandDetailId,
                                                      Text = c.BrandName,
                                                  }).ToList();
                getAllMasterData.modeldetails = (from c in (await _staticService._cacheRepo.DeviceModeldetails()).Where(x => x.DeviceTypeId == id).ToList()
                                                 select new SelectListData()
                                                  {
                                                      Value = c.DeviceModeldetailId,
                                                      Text = c.ModelName,
                                                  }).ToList();
                getAllMasterData.deviceProcessorDetails = (from c in (await _staticService._cacheRepo.DeviceProcessorDetails()).Where(x => x.DeviceTypeId == id).ToList()
                                                  select new SelectListData()
                                                  {
                                                      Value = c.DeviceProcessorDetailId,
                                                      Text = c.DeviceProcessorName,
                                                  }).ToList();
                getAllMasterData.generationDetails = (from c in (await _staticService._cacheRepo.GenerationDetails()).Where(x => x.DeviceTypeId == id).ToList().ToList()
                                                  select new SelectListData()
                                                  {
                                                      Value = c.GenerationDetailId,
                                                      Text = c.GenerationName,
                                                  }).ToList();
                getAllMasterData.rAMDetails = (from c in (await _staticService._cacheRepo.RAMDetails()).Where(x => x.DeviceTypeId == id).ToList()
                                               select new SelectListData()
                                                  {
                                                      Value = c.RAMDetailId,
                                                      Text = c.RAMSize,
                                                  }).ToList();
                getAllMasterData.hardDiskDetails = (from c in (await _staticService._cacheRepo.HardDiskDetails()).Where(x => x.DeviceTypeId == id).ToList()
                                                    select new SelectListData()
                                                  {
                                                      Value = c.HardDiskDetailId,
                                                      Text = c.HardDiskCompanyName+":"+c.HardDiskSize,
                                                  }).ToList();
                getAllMasterData.procurementTypes = (from c in (await _staticService._cacheRepo.ProcurementTypes())
                                                     select new SelectListData()
                                                  {
                                                      Value = c.ProcurementTypeId,
                                                      Text = c.ProcurementNameType,
                                                  }).ToList();
                resJsonOutput.Data = getAllMasterData;
                resJsonOutput.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }

        

    }
}
