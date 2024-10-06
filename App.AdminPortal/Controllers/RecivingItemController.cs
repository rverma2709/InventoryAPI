using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Root.Models.Application.Tables;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace App.AdminPortal.Controllers
{
    public class RecivingItemController : AdminPortalController
    {
        private readonly IDataService<DBEntities, BulkFileRecivingDetail> _BulkFileRecivingDetail;
        private readonly IDataService<DBEntities, PoDetail> _poDetail;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RecivingItemController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, BulkFileRecivingDetail> BulkFileRecivingDetail, IDataService<DBEntities, PoDetail> poDetail, IWebHostEnvironment webHostEnvironment) : base(staticService, httpContextAccessor, "PageModelName")
        {
            _BulkFileRecivingDetail = BulkFileRecivingDetail;
            _poDetail = poDetail;
            _webHostEnvironment = webHostEnvironment;
        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> Reciving()
        {
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            return View(new RecivingItemView());
        }
        [HttpPost]
        [PreventDuplicateRequests]
        public async Task<IActionResult> Reciving(RecivingItemView model, IFormFile BulkImportDocument)
        {

            Tuple<bool, string> tuple = ValidateModel(model);
            if (tuple.Item1)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string FileName = await CommonLib.SaveFile(BulkImportDocument, wwwRootPath + CommonLib.DirectorySeparatorChar()+ "Docs"+ CommonLib.DirectorySeparatorChar()+ "BulkFileRecivingDetail");

                DataTable dataTable = CommonLib.GetdataWithFile(wwwRootPath + CommonLib.DirectorySeparatorChar() + "Docs" + CommonLib.DirectorySeparatorChar() + "BulkFileRecivingDetail" +  CommonLib.DirectorySeparatorChar() + FileName);

                dataTable = CommonLib.EmptyRowRemoveCode(dataTable);
                if (dataTable.Rows.Count > 0 && model.RemainingQuantity > dataTable.Rows.Count)
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
                            CrdBy=LoginUserId

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
                            RecivingDate=model.RecivingDate
                        }

                      };
                        if (sendRequests.Any())
                        {
                            List<RecivingItemRetunData> recivingItemRetuns = await _staticService.ExecuteSPAsync<DBEntities, RecivingItemRetunData>(new SFRecevingItemDetails() { SeriolNumberData = sendRequests, RecivingItemList = recivingItemList, BulkFileRecivingDetailId= bulkFileRecivingDetail.BulkFileRecivingDetailId, InventoryUserId= LoginUserId });

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
                      
                        HttpContext.Session.SetObject(ProgConstants.SuccMsg, "success");





                    }
                    else
                    {
                        HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Invalid File");
                        return RedirectToAction("Reciving", "RecivingItem");
                    }
                }
                else
                {
                    HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Excel file empty");
                    return RedirectToAction("Reciving", "RecivingItem");
                }
            }
            else
            {
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, tuple.Item2);
                return RedirectToAction("Reciving", "RecivingItem");
            }







            return RedirectToAction("Reciving", "RecivingItem");
        }
        [HttpPost]
        public async Task<ResJsonOutput> GetGramPanchayatData(long? PoDetailId)
        {
            ResJsonOutput DDData = new ResJsonOutput();

            var gpil = (await _staticService.ExecuteSPAsync<DBEntities, GetPoNumberItemList>(new SFGetPoNumberItemDetilas() { PoDetailId = PoDetailId }));


            DDData.Data = CommonLib.ConvertObjectToJson(gpil);
            DDData.Status.IsSuccess = true;
            return DDData;
        }

        [HttpPost]
        public async Task<ResJsonOutput> QuantityData(long? PoDetailId)
        {
            ResJsonOutput DDData = new ResJsonOutput();

            ItemQuantity gpil = (await _staticService.ExecuteSPAsync<DBEntities, ItemQuantity>(new SFGetItemQuantity() { PoItemDetilId = PoDetailId })).FirstOrDefault();


            DDData.Data = CommonLib.ConvertObjectToJson(gpil);
            DDData.Status.IsSuccess = true;
            return DDData;
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
                    string wwwRootPath = _webHostEnvironment.WebRootPath +CommonLib.DirectorySeparatorChar()+ "Docs";
                    System.IO.File.WriteAllBytes(wwwRootPath + CommonLib.DirectorySeparatorChar()+ ResponseFilePath, excelBytes);
                }
            }
            catch (Exception ex)
            {

            }
           
        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> GetRecivingFile(SFGetBulkFileRecivingDetails sFGetBulkFile)
        {
            ViewBag.PageModelName = "Reciving Item File List";
            List<BulkFileRecivingDetail> bulkFileRecivingDetails = new List<BulkFileRecivingDetail>();
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            try
            {
                ResJsonOutput result = await _staticService.FetchList<BulkFileRecivingDetail>(_BulkFileRecivingDetail, sFGetBulkFile, new Expression<Func<BulkFileRecivingDetail, object>>[] { a => a.PoDetail,a=>a.PoItemDetail });
                if (result.Status.IsSuccess)
                {
                    bulkFileRecivingDetails = await FetchList<BulkFileRecivingDetail>(result, sFGetBulkFile);

                }
                else
                {
                    HttpContext.Session.SetObject(ProgConstants.ErrMsg, result.Status.Message);
                }
            }
            catch (Exception ex)
            {
                await CatchError(ex);
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Check for AJAX requests
            {
                return PartialView("_bulkrecivingfiletable", Tuple.Create(bulkFileRecivingDetails, sFGetBulkFile));
            }
            return View(Tuple.Create(bulkFileRecivingDetails, sFGetBulkFile));
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> ADDRecivingItem()
        {
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            return View(new RecivingItemView());
        }
        [HttpPost]
        [PreventDuplicateRequests]
        public async Task<IActionResult> ADDRecivingItem(RecivingItemView model)
        {
            try
            {
                if (model.SeriolNumber != null)
                {
                    Tuple<bool, string> tuple = ValidateModel(model);
                    if (tuple.Item1)
                    {

                        List<SeriolNumberData> seriolNumberDatas = new List<SeriolNumberData> { new SeriolNumberData() { SeriolNumber = model.SeriolNumber } };

                        List<RecivingItemList> recivingItemList = new List<RecivingItemList>() {
                        new RecivingItemList()
                        {
                            PoDetailId=model.PoDetailId,
                            PoItemDetilId= model.PoItemDetilId,
                            RecivingDate=model.RecivingDate
                        }

                      };
                        List<RecivingItemRetunData> recivingItemRetuns = await _staticService.ExecuteSPAsync<DBEntities, RecivingItemRetunData>(new SFRecevingItemDetails() { SeriolNumberData = seriolNumberDatas, RecivingItemList = recivingItemList });
                        if (recivingItemRetuns[0].ErrorMessage != null)
                        {
                            HttpContext.Session.SetObject(ProgConstants.ErrMsg, recivingItemRetuns[0].ErrorMessage);
                        }
                        else
                        {
                            HttpContext.Session.SetObject(ProgConstants.SuccMsg, "success");
                        }

                    }
                    else
                    {
                        HttpContext.Session.SetObject(ProgConstants.ErrMsg, tuple.Item2);
                        return RedirectToAction("ADDRecivingItem", "RecivingItem");
                    }
                    return RedirectToAction("ADDRecivingItem", "RecivingItem");
                }
                else
                {
                    HttpContext.Session.SetObject(ProgConstants.ErrMsg, "SeriolNumber not blank");
                    return RedirectToAction("ADDRecivingItem", "RecivingItem");
                }
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("ADDRecivingItem", "RecivingItem");

        }

    }
}
