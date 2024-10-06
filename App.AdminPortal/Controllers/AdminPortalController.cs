using App.AdminPortal.Common;
using App.AdminPortalServices.Models;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Root.Models.Application.Tables;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.Controller;

namespace App.AdminPortal.Controllers
{
    public class AdminPortalController : DefaultController
    {
        #region Variables
        protected new readonly AdminPortalStaticService _staticService;
        public InventoryUser LoginUser;
        public long? LoginUserId;
        protected string _PageModelName;
        public List<AllowedFile> AllowedFileList;
        protected new readonly AppConfig _appConfig;

        #endregion

        public AdminPortalController(
            AdminPortalStaticService staticService,
            IHttpContextAccessor httpContextAccessor,
            string PageModelName) : base(staticService, httpContextAccessor)
        {
            _staticService = staticService;
            _appConfig = staticService._appConfig;
            LoginUser = _staticService.LoginUser;
            LoginUserId = _staticService.LoginUserId;
           // AllowedFileList = _staticService._cacheRepo.AllowedFilesList().GetAwaiter().GetResult();
            _PageModelName = PageModelName;

            Dt = DateTime.Now;
            LoginUser = _staticService.GetLoginUser();
            if (LoginUser != null)
            {
                LoginUserId = LoginUser.InventoryUserId;
            }
            //LoginApiToken = staticService.GetSessionValue<string>(ProgConstants.ApiToken);
        }

        protected internal async Task<ResJsonOutput> CatchError(Exception ex)
        {
            return await _staticService.HandleError(RouteData, HttpContext, ex);

        }

        protected internal string GetModelStateErrors(ModelStateDictionary ModelState)
        {
            //result.Status.Message = await GetStatusMessage(result.Status.StatusCode);
            return string.Join("<br />", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }

        protected internal async Task<List<T>> FetchList<T>(ResJsonOutput result, SFParameters model) where T : class
        {
            List<T> clctnlist = new List<T>();
            if (result.Status.IsSuccess)
            {
                if (result.Data != null)
                {
                    JsonOutputForList jsonOutputForList = (JsonOutputForList)(result.Data);
                    clctnlist = (List<T>)(jsonOutputForList.ResultList);
                    if (model.ExportType.IsNullString().Equals(Enums.ExpotTypes.Excel.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        jsonOutputForList.PageNo = 1;
                    }
                    ViewBag.Index = ((jsonOutputForList.PageNo - 1) * jsonOutputForList.RowsPerPage) + 1;
                    ViewBag.rowCount = jsonOutputForList.TotalCount;
                    ViewBag.Page = jsonOutputForList.PageNo;
                    ViewBag.PageCount = Math.Ceiling(jsonOutputForList.TotalCount / (decimal)jsonOutputForList.RowsPerPage);
                }
            }
            else
            {
                ViewBag.ErrMsg = result.Status.Message;
            }
            ViewBag.NoRecords = "No records found";
            return clctnlist;
        }

        protected internal async Task<T> GetDetails<T>(ResJsonOutput result)
        {
            if (result.Status.IsSuccess)
            {
                if (result.Data != null)
                {
                    T model = (T)(result.Data);
                    return model;
                }
            }
            else
            {
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, result.Status.Message);
            }
            return default(T);
        }
        protected internal async Task<bool> CreatOrUpdate<T>(string ApiPath, T model)
        {
            try
            {
                ResJsonOutput result = new ResJsonOutput(); //await _staticService.PostDataAsync(ApiPath, model);
                if (result.Status.IsSuccess)
                {
                    ViewStatusMessage viewStatusMessage = CommonLib.ConvertJsonToObject<ViewStatusMessage>(result.Data);
                    if (viewStatusMessage.Data != null)
                    {
                        model = CommonLib.ConvertJsonToObject<T>(CommonLib.ConvertObjectToJson(viewStatusMessage.Data));
                    }
                    HttpContext.Session.SetObject(ProgConstants.SuccMsg, viewStatusMessage.Message);
                    return true;
                }
                else
                {
                    ViewBag.ErrMsg = result.Status.Message;
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected internal async Task DoDisabled(string ApiPath, long id, string D)
        {
            ResJsonOutput result = new ResJsonOutput(); //await _staticService.PostDataAsync(ApiPath, new RequestByIdFlag() { id = id, flag = (D == "D") });
            if (result.Status.IsSuccess)
            {
                HttpContext.Session.SetObject(ProgConstants.SuccMsg, CommonLib.ConvertJsonToObject<ViewStatusMessage>(result.Data).Message);
            }
            else
            {
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, result.Status.Message);
            }
        }
        //protected internal List<FieldModel> GetFieldList(object obj)
        //{
        //    List<FieldModel> fields = new List<FieldModel>();
        //    foreach (PropertyInfo p in obj.GetType().GetProperties())
        //    {
        //        FieldModel fieldModel = new FieldModel() { ColumnName = p.Name, DisplayName = p.Name };

        //        DisplayNameAttribute d = (DisplayNameAttribute)(p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "DisplayNameAttribute"));
        //        if (d != null)
        //        {
        //            fieldModel.DisplayName = d.DisplayName;
        //        }
        //        fields.Add(fieldModel);
        //    }
        //    return fields;
        //}
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.LoginUser = LoginUser;
            ViewBag.PageModelName = _PageModelName;
            return;
        }
        public async Task<FileContentResult> CreateExcelGeneric<T>(List<T> model, string workSheetName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(workSheetName);
                worksheet.Cells.LoadFromCollection(model, true);

                // Generate the Excel file bytes
                var excelBytes = package.GetAsByteArray();

                // Return the Excel file as a downloadable attachment
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", workSheetName + ".xlsx");
            }
        }
       
        protected void SetSession(string key, object value)
        {
            _httpContextAccessor?.HttpContext?.Session.SetObject(key, value);
        }
        //public async Task<List<ParentDDResult>> GetSearchStatesList()
        //{
        //    List<ParentDDResult> searchStates = new List<ParentDDResult>();

        //    if (LoginUser.StateId != null)
        //    {
        //        var AssignStates = CommonLib.GetIdList(LoginUser.StateIds)
        //            .Select(x => new RequestById() { id = x }).ToList();

        //        List<ParentDDResult> states = await _staticService._cacheRepo.StatesList();

        //        foreach (var searchState in AssignStates)
        //        {
        //            searchStates.AddRange(states.Where(x => x.Value == searchState.id.ToString()));
        //        }
        //    }
        //    else
        //    {
        //        searchStates = await _staticService._cacheRepo.StatesList();
        //    }

        //    return searchStates;
        //}

        public List<SelectListItem> GetYesNoList()
        {
            return new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Yes", Value = "true" },
                            new SelectListItem { Text = "No", Value = "false" }
                        };
        }

        #region  Model Validate
        public  Tuple<bool, string> ValidateModel<T>(T model) where T : class
        {
            string errorMessage = string.Empty;
            ModelState.Clear();
            TryValidateModel(model);
            if (!ModelState.IsValid)
            {
                errorMessage = GetModelStateErrors(ModelState);
                return Tuple.Create(false, errorMessage);
            }

            return Tuple.Create(true, errorMessage);
        }
        #endregion


        protected T GetSession<T>(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetObject<T>(key);
        }
        protected void RemoveSession(string key)
        {
            _httpContextAccessor?.HttpContext?.Session.Remove(key);
        }
    }
}
