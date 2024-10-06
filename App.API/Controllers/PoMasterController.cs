using App.API.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Mvc;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;

namespace App.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PoMasterController : DefaultController
    {
        public PoMasterController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor)
        {
        }

        [Route("CreateLaptopPO"), HttpPost]
       // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> CreateLaptopPO([FromBody] LaptopPORequest request)
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<PoItemDetilList> poItemDetilLists = new List<PoItemDetilList>();
                foreach(var itm in request.laptopItems)
                {
                    PoItemDetilList poItemDetilList = new PoItemDetilList() { 
                      BrandDetailId = itm.Brand,
                      DeviceModelDetailId= itm.Model,
                        DeviceProcessorDetailId= itm.Processor,
                        GenerationDetailId= itm.Generation,
                        RAMDetailId= itm.RAM,
                        HardDiskDetailId = itm.HardDisk,
                        Quantity= itm.Quantity,
                        Price= itm.Price,
                        Description = itm.Description

                    };
                    poItemDetilLists.Add(poItemDetilList);
                }
                List<PoDetailList> poDetailLists = new List<PoDetailList>() {
                    new PoDetailList()
                    {
                        DeviceTypeId = 1,
                        VendorDetailId = request.VendorName,
                        ProcurementTypeId= request.ProcurementType,
                        PurchaseDate = request.PurchaseDate,
                        DeliveryDate = request.DeliveryDate,
                        RentStartDate = request.RentStartDate,
                        Tenure = request.Tenure,
                        Warranty = request.Warranty,
                        ActualAmount=request.ActualAmount,
                        TotalAmount = request.TotalAmount,
                        TaxableAmount=request.TaxableAmount,
                       
                        Discount = request.DiscountAmt,
                        Notes = request.Notes,
                        TermsAndConditions = request.TermsAndConditions,
                        SignatureName = request.SignatureName,

                    }
                };
                await _staticService.ExecuteNonQueryAsync<DBEntities>(new SFAddPoDetails() { PoDetailList = poDetailLists, PoItemDetilList = poItemDetilLists });
                resJsonOutput.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.IsSuccess = false;
            }

            return resJsonOutput;
        }

        [Route("PoDetailList"), HttpPost]
        //[ServiceFilter(typeof(UniqueKeyMatching))]
        //[ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> PoDetailList()
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<Polist> polists = await _staticService.ExecuteSPAsync<DBEntities, Polist>(new SFGetPOList());
                List<PoItemData> poItems = await _staticService.ExecuteSPAsync<DBEntities, PoItemData>(new SFGetPOItemList());

                List<PoListAndPoItemData> poListAndPoItems = (from po in polists
                                   
                                   select new PoListAndPoItemData
                                   {
                                       PoDetailId = po.PoDetailId,
                                       PoNumber = po.PoNumber,
                                       DeviceName = po.DeviceName,
                                       ProcurementNameType = po.ProcurementNameType,
                                       TotalQuantity = po.TotalQuantity,
                                       TotalAmount = po.TotalAmount,
                                       ReciveQuantity=po.ReciveQuantity,
                                       poItemDatas = poItems.Where(x=>x.PoDetailId== po.PoDetailId).ToList()
                                   }).ToList();
                resJsonOutput.Status.IsSuccess=true;
                resJsonOutput.Data = poListAndPoItems;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }
    }
}
