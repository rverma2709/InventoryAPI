using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using System.Collections.Generic;
using static Azure.Core.HttpHeader;

namespace App.AdminPortal.Controllers
{
    public class PoMasterController : AdminPortalController
    {
        public PoMasterController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "Inventory Procurement")
        {
        }
        public async Task FormInitialise(long id)
        {
            List<DeviceType> deviceTypes = (await _staticService._cacheRepo.DeviceTypeList());
            List<BrandDetail> brandDetails = (await _staticService._cacheRepo.BrandDetails()).Where(x=>x.DeviceTypeId==id).ToList();
            List<DeviceModeldetail> modeldetails = (await _staticService._cacheRepo.DeviceModeldetails()).Where(x => x.DeviceTypeId == id).ToList();
            List<DeviceProcessorDetail> deviceProcessorDetails = (await _staticService._cacheRepo.DeviceProcessorDetails()).Where(x => x.DeviceTypeId == id).ToList();
            List<GenerationDetail> generationDetails = (await _staticService._cacheRepo.GenerationDetails()).Where(x => x.DeviceTypeId == id).ToList();
            List<RAMDetail> rAMDetails = (await _staticService._cacheRepo.RAMDetails()).Where(x => x.DeviceTypeId == id).ToList();
            List<HardDiskDetail> hardDiskDetails = (await _staticService._cacheRepo.HardDiskDetails()).Where(x => x.DeviceTypeId == id).ToList();
            List<InventoryUser> vendorDetails = (await _staticService._cacheRepo.InventoryUsersDetails()).Where(x=>x.InventoryRoleId==4).ToList();
            List<ProcurementType> procurementTypes = (await _staticService._cacheRepo.ProcurementTypes());
            ViewBag.DeviceType = (deviceTypes.Select(x => new { x.DeviceTypeId, x.DeviceName }).ToList());
            ViewBag.brandDetails = (brandDetails.Select(x => new { x.BrandDetailId, x.BrandName }).ToList());
            ViewBag.modeldetails = (modeldetails.Select(x => new { x.DeviceModeldetailId, x.ModelName }).ToList());
            ViewBag.deviceProcessorDetails = (deviceProcessorDetails.Select(x => new { x.DeviceProcessorDetailId, x.DeviceProcessorName }).ToList());
            ViewBag.generationDetails = (generationDetails.Select(x => new { x.GenerationDetailId, x.GenerationName }).ToList());
            ViewBag.rAMDetails = (rAMDetails.Select(x => new { x.RAMDetailId, x.RAMSize }).ToList());
            ViewBag.hardDiskDetails = hardDiskDetails.Select(x => new {
                x.HardDiskDetailId,
                HardDiskInfo = x.HardDiskSize + " " + x.HardDiskType
            }).ToList();
            ViewBag.vendorDetails = (vendorDetails.Select(x => new { x.InventoryUserId, x.CompanyName }).ToList());
            ViewBag.procurementTypes = (procurementTypes.Select(x => new { x.ProcurementTypeId, x.ProcurementNameType }).ToList());

            List<SelectListItem> QualityCheckStatus = new List<SelectListItem>();

            QualityCheckStatus.Add(new SelectListItem { Text = "Yes", Value = "True" });
            QualityCheckStatus.Add(new SelectListItem { Text = "No", Value = "False" });
            ViewBag.QualityCheckStatus = QualityCheckStatus;


        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> LaptopPoCreate(SFGetProcurementDetils sFGetProcurementDetils)
        {
            await FormInitialise(1);
            PoDetailView procurementDetail = new PoDetailView();

            return View(procurementDetail);
        }
        [HttpPost]
        [PreventDuplicateRequests]
        public async Task<IActionResult> LaptopPoCreate(PoDetailView model, string RepeaterData)
        {
            try
            {
                var laptopItems = JsonConvert.DeserializeObject<List<PoItemDetilList>>(RepeaterData);

                List<PoItemDetilList> poItemDetilLists = laptopItems;

                if (ModelState.IsValid)
                {
                    List<PoDetailList> poDetailLists = new List<PoDetailList>() {
                    new PoDetailList()
                    {
                        DeviceTypeId = 1,
                        VendorDetailId = model.VendorDetailId,
                        ProcurementTypeId= model.ProcurementTypeId,
                        PurchaseDate = model.PurchaseDate,
                        DeliveryDate = model.DeliveryDate,
                        RentStartDate = model.RentStartDate,
                        Tenure = model.Tenure,
                        Warranty = model.Warranty,
                        ActualAmount=model.ActualAmount,
                        TotalAmount = model.TotalAmount,
                        TaxableAmount=model.TaxableAmount,
                        DiscountType = model.DiscountType,
                        Discount = model.Discount,
                        Notes = model.Notes,
                        TermsAndConditions = model.TermsAndConditions,
                        SignatureName = model.SignatureName,

                    }
                };
                    await _staticService.ExecuteNonQueryAsync<DBEntities>(new SFAddPoDetails() { PoDetailList = poDetailLists, PoItemDetilList = poItemDetilLists });
                    HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data successfully save");
                    return RedirectToAction("LaptopPoCreate", "PoMaster");
                }
                else
                {
                    HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Please fill all required fields.");
                }
            }
            catch (Exception ex)
            {
                await CatchError(ex);
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Something Error");
            }
            await FormInitialise(1);
            // Reload the form with validation messages
            return View(new PoDetailView());
        }

    }
}
