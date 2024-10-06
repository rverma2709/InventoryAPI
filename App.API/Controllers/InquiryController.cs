using App.API.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Mvc;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Services.DBContext;
using Root.Services.Interfaces;

namespace App.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InquiryController : DefaultController
    {
        private readonly IDataService<DBEntities, InquiryDetail> _InquiryDetail;
        public InquiryController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, InquiryDetail> InquiryDetail) : base(staticService, httpContextAccessor)
        {
            _InquiryDetail = InquiryDetail;
        }

        [Route("Index"), HttpPost]
        public async Task<ResJsonOutput> Index(InquiryData inquiryData)
        {
            ResJsonOutput resJsonOutput= new ResJsonOutput();
            try
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    UserName = inquiryData.name,
                    ContactNo = inquiryData.contactNo,
                    EmailId = inquiryData.email,
                    DeviceType = inquiryData.deviceType,
                    Quantity = inquiryData.quantity,
                };
               await _InquiryDetail.Create(inquiryDetail);
                await _InquiryDetail.Save();
                resJsonOutput.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.IsSuccess = false;
            }
            
            return resJsonOutput;
        }

        [Route("GetInquiryData"), HttpPost]
        public async Task<ResJsonOutput> GetInquiryData()
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
               List<InquiryDetail> inquiryDetails= _InquiryDetail.GetAll().ToList();
                resJsonOutput.Data = inquiryDetails;
                resJsonOutput.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.IsSuccess = false;
            }

            return resJsonOutput;
        }
    }
}
