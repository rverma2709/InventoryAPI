using App.API.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace App.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StockMasterController :  DefaultController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDataService<DBEntities, BulkFileRecivingDetail> _BulkFileRecivingDetail;
        public StockMasterController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IDataService<DBEntities, BulkFileRecivingDetail> BulkFileRecivingDetail) : base(staticService, httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _BulkFileRecivingDetail= BulkFileRecivingDetail;
        }
        [Route("StockPoList"), HttpPost]
       // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> StockPoList()
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<SelectListData> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListData>(new SFGetPoNumber());
                resJsonOutput.Data = PoDetails;
                resJsonOutput.Status.IsSuccess = true;

            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
           return resJsonOutput;
            
        }
        [Route("GetStockItemList"), HttpPost]
        // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> StockItemList([FromBody] long? PoDetailId)
        {
          
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<GetPoNumberItemList> itemLists = (await _staticService.ExecuteSPAsync<DBEntities, GetPoNumberItemList>(new SFGetPoNumberItemDetilas() { PoDetailId = PoDetailId }));
                resJsonOutput.Data = itemLists;
                resJsonOutput.Status.IsSuccess = true;

            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }
        [Route("GetStockItemQuntity"), HttpPost]
        // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> GetStockItemQuntity([FromBody] long? PoItemId)
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                ItemQuantity? itemQuantity = (await _staticService.ExecuteSPAsync<DBEntities, ItemQuantity>(new SFGetItemQuantity() { PoItemDetilId = PoItemId })).FirstOrDefault();
                if(itemQuantity==null)
                {
                   
                    itemQuantity = new ItemQuantity();
                }

                resJsonOutput.Status.IsSuccess=true;
                resJsonOutput.Data = itemQuantity;
            }
            catch(Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }

        [Route("StockRecivingBulk"), HttpPost]
        // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> StockRecivingBulk([FromForm] ReciveItemsDto model)
        {
            ResJsonOutput resJsonOutput= new ResJsonOutput();
            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string FileName = await CommonLib.SaveFile(model.BulkImportDocument, wwwRootPath + CommonLib.DirectorySeparatorChar() + "Docs" + CommonLib.DirectorySeparatorChar() + "SeriolNumberFile");
                DataTable dataTable = CommonLib.GetdataWithFile(wwwRootPath + CommonLib.DirectorySeparatorChar() + "Docs" + CommonLib.DirectorySeparatorChar() + "SeriolNumberFile" + CommonLib.DirectorySeparatorChar() + FileName);

                dataTable = CommonLib.EmptyRowRemoveCode(dataTable);
                if (dataTable.Rows.Count > 0 && model.RemainingQuantity >= dataTable.Rows.Count)
                {
                    List<FieldModel> fields = GetFieldList(new SeriolNumberData());
                    bool isAll = CommonLib.IsAllColumnExist(dataTable, fields.Select(s => s.DisplayName).ToList());
                    if (isAll)
                    {
                        BulkFileRecivingDetail bulkFileRecivingDetail = new BulkFileRecivingDetail()
                        {
                            PoDetailId = model.PoDetailId,
                            PoItemDetilId = model.PoItemDetilId,
                            UploadFile = FileName,
                            TotalUploadRecod = dataTable.Rows.Count,
                            SuccessRecord = 0,
                            CrdBy = model.LoginUserId

                        };
                        List<string> FileList = _staticService.UpdateFileName(bulkFileRecivingDetail);
                        await _BulkFileRecivingDetail.Create(bulkFileRecivingDetail);
                        await _BulkFileRecivingDetail.Save(FileList);
                        List<RecivingItemRetunData> recivingItemRetunDatas = new List<RecivingItemRetunData>();
                        List<SeriolNumberData> seriolNumberDatas = dataTable.AsEnumerable()
                                       .Select(row => new SeriolNumberData
                                       {
                                           SeriolNumber = row.Field<string>("SeriolNumber"),

                                       }).ToList();
                        recivingItemRetunDatas = seriolNumberDatas.Select(item => ValidateExcel<SeriolNumberData>(item)).ToList();


                        var duplicateApplicationNo = seriolNumberDatas.GroupBy(item => item.SeriolNumber).Where(group => group.Count() > 1).Select(group => group.First()).ToList();
                        if (duplicateApplicationNo != null && duplicateApplicationNo.Count > 0)
                        {
                            foreach (var duplicate in duplicateApplicationNo)
                            {
                                recivingItemRetunDatas.Where(a => a.SeriolNumber == duplicate.SeriolNumber).ToList().ForEach(item => item.ErrorMessage = item.ErrorMessage + Environment.NewLine + "Duplicate Data");
                            }
                        }
                        List<SeriolNumberData> sendRequests = seriolNumberDatas.Where(x => recivingItemRetunDatas.Any(response => response.SeriolNumber == x.SeriolNumber && response.ErrorMessage == null)).ToList();
                        List<RecivingItemList> recivingItemList = new List<RecivingItemList>() {
                        new RecivingItemList()
                        {
                            PoDetailId=model.PoDetailId,
                            PoItemDetilId= model.PoItemDetilId,
                            RecivingDate=model.ReceivingDate
                        }

                      };
                        if (sendRequests.Any())
                        {
                            List<RecivingItemRetunData> recivingItemRetuns = await _staticService.ExecuteSPAsync<DBEntities, RecivingItemRetunData>(new SFRecevingItemDetails() { SeriolNumberData = sendRequests, RecivingItemList = recivingItemList, BulkFileRecivingDetailId = bulkFileRecivingDetail.BulkFileRecivingDetailId, InventoryUserId = model.LoginUserId });

                            foreach (var item in recivingItemRetuns)
                            {
                                recivingItemRetunDatas.Where(a => a.SeriolNumber == item.SeriolNumber).ToList().ForEach(a => { a.ErrorMessage = item.ErrorMessage; });
                            }
                        }
                        bulkFileRecivingDetail.SuccessRecord = recivingItemRetunDatas.Where(x => x.ErrorMessage == null).Count();

                        if (recivingItemRetunDatas.Where(x => x.ErrorMessage != null).Count() > 0)
                        {
                            bulkFileRecivingDetail.ErrorFile = bulkFileRecivingDetail.GetType().Name + CommonLib.DirectorySeparatorChar() + "Res-" + FileName;
                            CreateResponseFiles<RecivingItemRetunData>(recivingItemRetunDatas.Where(x => x.ErrorMessage != null).ToList(), bulkFileRecivingDetail.ErrorFile);
                        }
                        _BulkFileRecivingDetail.Update(bulkFileRecivingDetail);
                        await _BulkFileRecivingDetail.Save();
                        resJsonOutput.Status.IsSuccess = true;
                    }
                    else
                    {
                       
                        resJsonOutput.Status.IsSuccess = false;
                        resJsonOutput.Status.Message = "Invalid File";
                       
                    }
                }
                else
                {
                  
                    resJsonOutput.Status.IsSuccess = false;
                    resJsonOutput.Status.Message = "Excel file empty";
                }
            }
            catch (Exception ex)
            {

            }

            
            



            return resJsonOutput;
        }

        [Route("SeriolNumberUpload"), HttpPost]
        // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> SeriolNumberUpload([FromForm] ReciveItemsDto model)
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string FileName = await CommonLib.SaveFile(model.BulkImportDocument, wwwRootPath + CommonLib.DirectorySeparatorChar() + "Docs" + CommonLib.DirectorySeparatorChar() + "SeriolNumberFile");
                DataTable dataTable = CommonLib.GetdataWithFile(wwwRootPath + CommonLib.DirectorySeparatorChar() + "Docs" + CommonLib.DirectorySeparatorChar() + "SeriolNumberFile" + CommonLib.DirectorySeparatorChar() + FileName);

                dataTable = CommonLib.EmptyRowRemoveCode(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    List<FieldModel> fields = GetFieldList(new SeriolNumberData());
                    bool isAll = CommonLib.IsAllColumnExist(dataTable, fields.Select(s => s.DisplayName).ToList());
                    if (isAll)
                    {
                        BulkFileRecivingDetail bulkFileRecivingDetail = new BulkFileRecivingDetail()
                        {
                            PoDetailId = model.PoDetailId,
                            PoItemDetilId = model.PoItemDetilId,
                            UploadFile = FileName,
                            TotalUploadRecod = dataTable.Rows.Count,
                            SuccessRecord = 0,
                            CrdBy = model.LoginUserId

                        };
                        List<string> FileList = _staticService.UpdateFileName(bulkFileRecivingDetail);
                        await _BulkFileRecivingDetail.Create(bulkFileRecivingDetail);
                        await _BulkFileRecivingDetail.Save(FileList);
                        List<RecivingItemRetunData> recivingItemRetunDatas = new List<RecivingItemRetunData>();
                        List<SeriolNumberData> seriolNumberDatas = dataTable.AsEnumerable()
                                       .Select(row => new SeriolNumberData
                                       {
                                           SeriolNumber = row.Field<string>("SeriolNumber"),

                                       }).ToList();
                        recivingItemRetunDatas = seriolNumberDatas.Select(item => ValidateExcel<SeriolNumberData>(item)).ToList();


                        var duplicateApplicationNo = seriolNumberDatas.GroupBy(item => item.SeriolNumber).Where(group => group.Count() > 1).Select(group => group.First()).ToList();
                        if (duplicateApplicationNo != null && duplicateApplicationNo.Count > 0)
                        {
                            foreach (var duplicate in duplicateApplicationNo)
                            {
                                recivingItemRetunDatas.Where(a => a.SeriolNumber == duplicate.SeriolNumber).ToList().ForEach(item => item.ErrorMessage = item.ErrorMessage + Environment.NewLine + "Duplicate Data");
                            }
                        }
                        List<SeriolNumberData> sendRequests = seriolNumberDatas.Where(x => recivingItemRetunDatas.Any(response => response.SeriolNumber == x.SeriolNumber && response.ErrorMessage == null)).ToList();
                        List<RecivingItemList> recivingItemList = new List<RecivingItemList>() {
                        new RecivingItemList()
                        {
                            PoDetailId=model.PoDetailId,
                            PoItemDetilId= model.PoItemDetilId,
                            RecivingDate=null
                        }

                      };
                        if (sendRequests.Any())
                        {
                            List<RecivingItemRetunData> recivingItemRetuns = await _staticService.ExecuteSPAsync<DBEntities, RecivingItemRetunData>(new SFRecevingItemDetails() { SeriolNumberData = sendRequests, RecivingItemList = recivingItemList, BulkFileRecivingDetailId = bulkFileRecivingDetail.BulkFileRecivingDetailId, InventoryUserId = model.LoginUserId });

                            foreach (var item in recivingItemRetuns)
                            {
                                recivingItemRetunDatas.Where(a => a.SeriolNumber == item.SeriolNumber).ToList().ForEach(a => { a.ErrorMessage = item.ErrorMessage; });
                            }
                        }
                        bulkFileRecivingDetail.SuccessRecord = recivingItemRetunDatas.Where(x => x.ErrorMessage == null).Count();

                        if (recivingItemRetunDatas.Where(x => x.ErrorMessage != null).Count() > 0)
                        {
                            bulkFileRecivingDetail.ErrorFile = bulkFileRecivingDetail.GetType().Name + CommonLib.DirectorySeparatorChar() + "Res-" + FileName;
                            CreateResponseFiles<RecivingItemRetunData>(recivingItemRetunDatas.Where(x => x.ErrorMessage != null).ToList(), bulkFileRecivingDetail.ErrorFile);
                        }
                        _BulkFileRecivingDetail.Update(bulkFileRecivingDetail);
                        await _BulkFileRecivingDetail.Save();
                        resJsonOutput.Status.IsSuccess = true;
                    }
                    else
                    {

                        resJsonOutput.Status.IsSuccess = false;
                        resJsonOutput.Status.Message = "Invalid File";

                    }
                }
                else
                {

                    resJsonOutput.Status.IsSuccess = false;
                    resJsonOutput.Status.Message = "Excel file empty";
                }
            }
            catch (Exception ex)
            {

            }

            return resJsonOutput;
        }


            [Route("StockRecivingWithoutSeriolNumber"), HttpPost]
        // [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> StockRecivingWithoutSeriolNumber([FromBody] ReciveItemsWithoutSeriolNumber model)
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<RecivingItemList> recivingItemList = new List<RecivingItemList>() {
                        new RecivingItemList()
                        {
                            PoDetailId=model.PoDetailId,
                            PoItemDetilId= model.PoItemDetilId,
                            RecivingDate=model.ReceivingDate
                        }

                      };
               await _staticService.ExecuteNonQueryAsync<DBEntities>(new SFRecevingItemDetailsWithouSeriolNo()
                {
                    RecivingItemList=recivingItemList,
                    RecivingCount=model.RecivingQuantity,
                   InventoryUserId=model.LoginUserId,
               });
                resJsonOutput.Status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }
        protected internal List<FieldModel> GetFieldList(object obj)
        {
            List<FieldModel> fields = new List<FieldModel>();
            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                FieldModel fieldModel = new FieldModel() { ColumnName = p.Name, DisplayName = p.Name };

                DisplayNameAttribute d = (DisplayNameAttribute)(p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "DisplayNameAttribute"));
                if (d != null)
                {
                    fieldModel.DisplayName = d.DisplayName;
                }
                fields.Add(fieldModel);
            }
            return fields;
        }
        protected RecivingItemRetunData ValidateExcel<T>(T model) where T : class
        {
            RecivingItemRetunData recivingItemRetunData = new RecivingItemRetunData();
            //string errorMessage = string.Empty;
            ModelState.Clear();
            TryValidateModel(model);
            var SeriolNumberProperty = typeof(T).GetProperty("SeriolNumber");
            if (SeriolNumberProperty != null)
            {
                var SeriolNumberValue = SeriolNumberProperty.GetValue(model);
                typeof(RecivingItemRetunData).GetProperty("SeriolNumber").SetValue(recivingItemRetunData, SeriolNumberValue);
            }
            if (!ModelState.IsValid)
            {
                var errorMessage = GetModelStateErrors(ModelState).Replace("<br />", " , ");
                var prop = typeof(RecivingItemRetunData).GetProperty("ErrorMessage");
                if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
                {
                    prop.SetValue(recivingItemRetunData, errorMessage.Replace("\n", " , "));
                    typeof(RecivingItemRetunData).GetProperty("Status").SetValue(recivingItemRetunData, false);
                }
            }

            return recivingItemRetunData;
        }
        private void CreateResponseFiles<T>(List<T> model, string ResponseFilePath)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Data");

                    // Use reflection to retrieve properties for the specified type T
                    var myMemberInfoArr = typeof(T)
                        .GetProperties()
                        .Select(pi => (MemberInfo)pi)
                        .ToArray();

                    // Dynamically create the Excel file from the provided model
                    worksheet.Cells.LoadFromCollection(model, true);
                    var excelBytes = package.GetAsByteArray();

                    // Save the Excel file to the specified file path
                    string wwwRootPath = _webHostEnvironment.WebRootPath + CommonLib.DirectorySeparatorChar() + "Docs";
                    System.IO.File.WriteAllBytes(wwwRootPath + CommonLib.DirectorySeparatorChar() + ResponseFilePath, excelBytes);
                }
            }
            catch (Exception ex)
            {

            }

        }

        [Route("DeviceList"), HttpPost]
        //[ServiceFilter(typeof(UniqueKeyMatching))]
        //[ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public async Task<ResJsonOutput> DeviceList()
        {
            ResJsonOutput resJsonOutput = new ResJsonOutput();
            try
            {
                List<DeviceList> deviceLists = await _staticService.ExecuteSPAsync<DBEntities, DeviceList>(new SFDeviceDetails());
               
                resJsonOutput.Status.IsSuccess = true;
                resJsonOutput.Data = deviceLists;
            }
            catch (Exception ex)
            {
                resJsonOutput.Status.Message = ex.Message;
            }
            return resJsonOutput;
        }
    }
}
