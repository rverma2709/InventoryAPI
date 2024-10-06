using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Root.Models.Application.Tables;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using System.Text;

namespace App.AdminPortal.Controllers
{
    public class FileHandlingController : AdminPortalController
    {
        protected string ApiControllerBase = "/FileHandling/";
        public FileHandlingController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "File Handling")
        {
        }

        [HttpPost]
        //[TypeFilter(typeof(AjaxRequest))]
        public async Task<ResJsonOutput> UploadFile(IFormFile file)
        {
            ResJsonOutput result = new ResJsonOutput();
            try
            {
                if (file == null || file.Length == 0)
                {
                    result.Status.Message = "file not selected";
                }
                else
                {
                    UploadFileModel model = new UploadFileModel();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //Added code for CSV injection VAPT Issue 
                        await file.CopyToAsync(ms);
                        string fileContent = Encoding.UTF8.GetString(ms.ToArray());

                        var harmfulCheckResult = await CheckHarmfulCharacters(file, ms, fileContent);
                        if (harmfulCheckResult != null)
                        {
                            return harmfulCheckResult;
                        }

                        byte[] fileBytes = ms.ToArray();
                        model.FileName = Path.GetFileName(file.FileName);
                        model.FileString = Convert.ToBase64String(fileBytes);
                        model.AllowedFileId = Convert.ToInt32(Request.Form["AllowedFileId"].IsNullString());
                    }

                    AllowedFile allowedfile = AllowedFileList.First(x => x.AllowedFileId == model.AllowedFileId);
                    if (allowedfile == null)
                    {
                        result.Status.StatusCode = RootEnums.StatusCodes.INVREQ.ToString();
                        result.Status.Message = await _staticService.GetStatusMessage(result.Status.StatusCode);
                    }
                    else if (allowedfile.MinSize > 0 && CommonLib.GetOriginalLengthInBytes(model.FileString) < allowedfile.MinSize * 1024 * 1024)
                    {
                        result.Status.StatusCode = RootEnums.StatusCodes.MINFILSIZ.ToString();
                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { allowedfile.Size.ToString() });
                    }
                    else if (CommonLib.GetOriginalLengthInBytes(model.FileString) > allowedfile.Size * 1024 * 1024)
                    {
                        result.Status.StatusCode = RootEnums.StatusCodes.EXDFILSIZ.ToString();
                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { allowedfile.Size.ToString() });
                    }
                    else if (!allowedfile.Extensions.Trim().ToLower().Split(',').ToArray().Contains(Path.GetExtension(model.FileName).Replace(".", "").ToLower()))
                    {
                        result.Status.StatusCode = RootEnums.StatusCodes.INVFILTYP.ToString();
                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { allowedfile.Extensions.Replace(",", ", ") });
                    }
                    //else if (!allowedfile.MIMEType.Split(',').ToArray().Contains(file.ContentType))
                    //{
                    //    return Json(new APIobj { Status = false, Message = "Selected file might be of invalid type or corrupted." }, JsonRequestBehavior.AllowGet);
                    //}
                    else
                    {
                        string strpath = _appConfig.uploadingConfigs.TempFolder;
                        CommonLib.CheckDir(strpath);
                        byte[] data = Convert.FromBase64String(model.FileString);
                        model.FileName = CommonLib.EpochTime() + "_" + model.FileName;
                        strpath = strpath + CommonLib.DirectorySeparatorChar() + model.FileName;

                        System.IO.File.WriteAllBytes(strpath, data);
                        //result.Data = model.FileName;
                        result.Data = new RequestByString { id = model.FileName };
                        result.Status.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = await CatchError(ex);
            }

            return result;
        }

        private async Task<ResJsonOutput> CheckHarmfulCharacters(IFormFile file, MemoryStream ms, string fileContent)
        {
            ResJsonOutput result = new ResJsonOutput();

            if (Path.GetExtension(file.FileName).ToLower() == ".xlsx" || Path.GetExtension(file.FileName).ToLower() == ".xls")
            {
                using (ExcelPackage package = new ExcelPackage(ms))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First(); // Access the first worksheet

                        var dimension = worksheet.Dimension;

                        for (int row = 1; row <= dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1, row, dimension.Columns].All(c => c.Value == null))
                            {
                                continue; // Skip empty row
                            }

                            for (int col = 1; col <= dimension.Columns; col++)
                            {
                                var cellValue = worksheet.Cells[row, col].Value;
                                if (cellValue != null && ContainsFormula(cellValue.ToString()))
                                {
                                    result.Status.StatusCode = RootEnums.StatusCodes.HARMFULCHAR.ToString();
                                    result.Status.Message = await _staticService.GetStatusMessage(result.Status.StatusCode);
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            else if (Path.GetFileName(file.FileName).ToLower().Contains(".csv") && ContainsFormula(fileContent))
            {
                result.Status.StatusCode = RootEnums.StatusCodes.HARMFULCHAR.ToString();
                result.Status.Message = await _staticService.GetStatusMessage(result.Status.StatusCode);
                return result;
            }

            return null;
        }

        [HttpPost]
        //[TypeFilter(typeof(AjaxRequest))]
        public async Task<ResJsonOutput> CheckFileExists([FromBody] RequestByString model)
        {
            ResJsonOutput result = new ResJsonOutput();
            if (!ModelState.IsValid)
            {
                result.Status.Message = GetModelStateErrors(ModelState);
            }
            else if (model.id.Contains("../"))
            {
                result.Status.StatusCode = RootEnums.StatusCodes.NOTFND.ToString();
                result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { "File" });
            }
            else
            {
                string strpath = _appConfig.uploadingConfigs.Location + CommonLib.DirectorySeparatorChar() + model.id;
                if (System.IO.File.Exists(strpath))
                {
                    string lastFolderName = Path.GetFileName(Path.GetDirectoryName(strpath));
                    result.Data = new RequestByString() { id = model.id.Replace(CommonLib.DirectorySeparatorChar(), '/') };
                    result.Status.IsSuccess = true;
                }
                else if (System.IO.File.Exists(_appConfig.uploadingConfigs.TempFolder + CommonLib.DirectorySeparatorChar() + model.id))
                {
                    strpath = _appConfig.uploadingConfigs.TempFolder + CommonLib.DirectorySeparatorChar() + model.id;
                    string lastFolderName = Path.GetFileName(Path.GetDirectoryName(strpath));
                    result.Data = new RequestByString() { id = lastFolderName + "/" + model.id.Replace(CommonLib.DirectorySeparatorChar(), '/') };
                    result.Status.IsSuccess = true;
                }
                else
                {
                    result.Status.StatusCode = RootEnums.StatusCodes.NOTFND.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { "File" });
                }
            }
            return result;
        }

        //[HttpPost]
        //[TypeFilter(typeof(AjaxRequest))]
        public async Task<IActionResult> ViewFile(string id)
        {
            ResJsonOutput result = new ResJsonOutput();
            if (id.Contains("../"))
            {
                result.Status.StatusCode = RootEnums.StatusCodes.NOTFND.ToString();
                result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { "File" });
            }
            else
            {
                //Added condition to check only Authorizes user can view/download file for vapt issue Broken Access Control
                if (LoginUser != null)
                {
                    string strpath = _appConfig.uploadingConfigs.Location + CommonLib.DirectorySeparatorChar() + id;
                    if (System.IO.File.Exists(strpath))
                    {
                        result.Data = new ViewFileResult { Base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(strpath)) };
                        result.Status.IsSuccess = true;
                    }
                    else if (System.IO.File.Exists(_appConfig.uploadingConfigs.TempFolder + CommonLib.DirectorySeparatorChar() + id))
                    {
                        result.Data = new ViewFileResult { Base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(_appConfig.uploadingConfigs.TempFolder + CommonLib.DirectorySeparatorChar() + id)) };
                        result.Status.IsSuccess = true;
                    }
                    else
                    {
                        result.Status.StatusCode = RootEnums.StatusCodes.NOTFND.ToString();
                        result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { "File" });
                    }
                }
                else
                {
                    result.Status.StatusCode = Enums.StatusCode.INVUSER.ToString();
                    result.Status.Message = await GetStatusMessage(result.Status.StatusCode, new List<string>() { "File" });
                }
            }

            if (result.Status.IsSuccess)
            {
                ViewFileResult viewFileResult = (ViewFileResult)(result.Data);
                return File(Convert.FromBase64String(viewFileResult.Base64), MIMEhandler.GetMimeType(Path.GetExtension(id)), id.Split("\\").LastOrDefault());
            }
            return Content(result.Status.Message);
        }
        //Added code for CSV injection VAPT Issue 
        private bool ContainsFormula(string content)
        {

            // Check for other potentially harmful characters or sequences
            if ((content != null) && (content.Contains("&") || content.Contains("<") || content.Contains(">") || content.Contains(";") || content.Contains("+") || content.Contains("#")))
            {
                return true;
            }

            return false;
        }
    }
}
