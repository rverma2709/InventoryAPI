using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Root.Models.ViewModels;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TypeLite;


namespace Root.Models.Utils
{
    public class JwtToken
    {
        public string token { get; set; }
    }
    public class ResJsonOutput: JwtToken
    {
        public ResJsonOutput()
        {
            //Header = new Header();
            Data = new object();
            Status = new ResStatus();
        }
        //public Header Header { get; set; }        
        public object Data { get; set; }
        public ResStatus Status { get; set; }

    }
   
    public class ResStatus
    {
        [JsonProperty("i")]
        [DefaultValue(false)]
        public bool IsSuccess { get; set; }
        [JsonProperty("m")]
        public string Message { get; set; }

        [DefaultValue("")]
        [JsonProperty("s")]
        public string StatusCode { get; set; }
    }
    public class JsonOutputForList
    {
        public long TotalCount { get; set; }
        public long RowsPerPage { get; set; }
        public long PageNo { get; set; }
        public object ResultList { get; set; }
    }
    public static class CommonLib
    {
        public static string ServerIP { get; set; }
        public static string LogPath { get; set; }
        public static string EpochTime(DateTime? last = null)
        {
            DateTime dateTime = DateTime.Now;
            if (last.HasValue)
                dateTime = last.Value;
            return dateTime.ToString("yyyyMMddHHmmssFFFFFFF");
           
        }
        public static int GetOriginalLengthInBytes(string base64string)
        {
            if (string.IsNullOrEmpty(base64string)) { return 0; }

            int characterCount = base64string.Length;
            int paddingCount = base64string.Substring(characterCount - 2, 2).Count(c => c == '=');
            return (3 * (characterCount / 4)) - paddingCount;
        }
        public static DataTable EmptyRowRemoveCode(DataTable dtable)
        {

            return dtable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
        }
        public static bool IsAllColumnExist(DataTable tableNameToCheck, List<string> columnsNames)
        {
            bool iscolumnExist = true;
            try
            {
                if (null != tableNameToCheck && tableNameToCheck.Columns != null && tableNameToCheck.Rows.Count > 0)
                {
                    foreach (string columnName in columnsNames)
                    {
                        if (!tableNameToCheck.Columns.Contains(columnName))
                        {
                            iscolumnExist = false;
                            break;
                        }
                    }
                }
                else
                {
                    iscolumnExist = false;
                }
            }
            catch (Exception ex)
            {
            }
            return iscolumnExist;
        }
        public static DataTable GetdataWithFile(string Path)
        {
            DataTable dtable = new DataTable();
            

                
                if (Path.Contains(ProgConstants.ExcelFileExtension) || Path.Contains(ProgConstants.CSVFileExtension))
                {

                    dtable = Path.Contains(ProgConstants.ExcelFileExtension) ? CommonLib.ConvertExcelToDataTable(Path) : CommonLib.ConvertCSVToDataTable(Path);
                }
            

           

            return dtable;
        }
        public static System.Data.DataTable ConvertCSVToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.SetDelimiters(new string[] { "," });

                string[] headers = parser.ReadFields();

                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    dataTable.Rows.Add(fields);
                }
            }

            return dataTable;
        }
        public static System.Data.DataTable ConvertExcelToDataTable(string filePath)
        {
            System.Data.DataTable dtResult = null;

            using (OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
            {
                objConn.Open();
                DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetName = string.Empty;

                if (dt != null)
                {
                    DataRow[] excelSheets = dt.Select(null, "TABLE_NAME");
                    sheetName = excelSheets.FirstOrDefault(r => !r["TABLE_NAME"].ToString().Contains("FilterDatabase"))?["TABLE_NAME"].ToString();
                }

                if (!string.IsNullOrEmpty(sheetName))
                {
                    using (OleDbCommand cmd = new OleDbCommand($"SELECT * FROM [{sheetName}]", objConn))
                    using (OleDbDataAdapter oleda = new OleDbDataAdapter(cmd))
                    using (DataSet ds = new DataSet())
                    {
                        oleda.Fill(ds, "excelData");
                        dtResult = ds.Tables["excelData"];
                    }
                }

                objConn.Close();
            }

            return dtResult;
        }
        public static async Task<string> SaveFile(IFormFile formFile,string path)
        {
          

            // Ensure the directory exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Get the file's unique name
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var extension = Path.GetExtension(formFile.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

            // Combine the folder path and file name
            var filePath = Path.Combine(path, uniqueFileName);

            // Save the file to the specified path
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }
            return uniqueFileName;
        }
        public static string GetRedisPlainConnectionString(string connStr)
        {
            string ConnKey = connStr.Substring(0, ProgConstants.ConnKeySize);
            string ConnIV = connStr.Substring(ProgConstants.ConnKeySize, ProgConstants.ConnIVSize);
            connStr = connStr.Substring(ProgConstants.ConnKeySize + ProgConstants.ConnIVSize, connStr.Length - ProgConstants.ConnKeySize - ProgConstants.ConnIVSize);
            return TripleDES.Decrypt(connStr, ConnKey, ConnIV);
        }
        public static string GetSortingClass(string fieldname, string cols, string order)
        {
            string str = "";
            if (cols == fieldname)
            {
                str = " onclick=\"CallSort(this, '" + fieldname + "','" + ((order == "Asc") ? "Desc" : "Asc") + "')\" class=\"sort-icon sorting_" + order.ToLower() + "\" ";
            }
            else
            {
                str = " onclick=\"CallSort(this, '" + fieldname + "','Asc')\" class=\" sort-icon sorting sorting_asc\" ";
            }
            return str;
        }
        public static string GetApplicationName()
        {
            return PlatformServices.Default.Application.ApplicationName;
        }
        public static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }
        public static List<string> UpdateFileName<T>(this T model, string temppath, string strpath)
        {
            string Location = model.GetType().Name;
            List<string> ListOfFiles = new List<string>();
            List<string> CopyFileList = new List<string>();

            PropertyInfo[] props = model.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    object[] attrs = prop.GetCustomAttributes(true);
                    if (attrs != null)
                    {
                        foreach (object attr in attrs)
                        {
                            DataTypeAttribute uploadAttr = attr as DataTypeAttribute;
                            if (uploadAttr != null)
                            {
                                if (uploadAttr.DataType == System.ComponentModel.DataAnnotations.DataType.Upload)
                                {
                                    try
                                    {
                                        string filepath = prop.GetValue(model, null).IsNullString();
                                        if (filepath != "")
                                        {
                                            if (filepath.Contains(CommonLib.DirectorySeparatorChar()))
                                            {
                                                string[] t = filepath.Split(CommonLib.DirectorySeparatorChar());
                                                if (t[0] != Location)
                                                {
                                                    CopyFileList.Add(filepath);
                                                    filepath = t.Last();
                                                }
                                            }
                                            if (filepath.Contains(Location + CommonLib.DirectorySeparatorChar()))
                                            {
                                                filepath = filepath.IsNullString().Replace(Location + CommonLib.DirectorySeparatorChar(), "");
                                            }
                                            filepath = filepath.IsNullString().Replace(Location + CommonLib.DirectorySeparatorChar(), "");
                                            if (filepath.IsNullString() != "")
                                            {
                                                filepath = Location + CommonLib.DirectorySeparatorChar() + filepath;
                                            }

                                            prop.SetValue(model, filepath);
                                            ListOfFiles.Add(filepath);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                else if (prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    IList itemList = prop.GetValue(model, null) as IList;
                    if (itemList != null)
                    {
                        foreach (var item in itemList)
                        {
                            ListOfFiles.AddRange(UpdateFileName(item, temppath, strpath));
                        }
                    }
                }
                else if (prop.PropertyType != typeof(string) && prop.PropertyType.IsClass)
                {
                    var itemList = prop.GetValue(model, null);
                    if (itemList != null)
                    {
                        ListOfFiles.AddRange(UpdateFileName(itemList, temppath, strpath));
                    }
                }
            }
            CopyFiles(CopyFileList, temppath, strpath);
            return ListOfFiles;
        }
        public static void CopyFiles(List<string> FileNames, string temppath, string strpath)
        {
            try
            {
                List<string> ToLocation = FileNames.Where(f => f.IsNullString() != "").Select(s => s.Split(CommonLib.DirectorySeparatorChar())[0]).ToList();
                if (ToLocation.Where(f => f.IsNullString() != "").Count() > 0)
                {
                    foreach (string item in ToLocation)
                    {
                        CommonLib.CheckDir(strpath + item);
                    }
                }

                if (FileNames.Where(f => f.IsNullString() != "").Count() > 0)
                {
                    foreach (string item in FileNames)
                    {
                        string ActualFileName = item.Split(CommonLib.DirectorySeparatorChar()).LastOrDefault();
                        if (System.IO.File.Exists(strpath + item))
                        {
                            try
                            {
                                System.IO.File.Copy(strpath + item, temppath + ActualFileName);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }
        public static string GetRequestIP(this HttpContext context)
        {
            IHeaderDictionary headers = context.Request.Headers;
            string ip = CommonLib.GetHeaderValue(headers, "X-Forwarded-For");
            if (ip.IsNullString() != string.Empty && context?.Connection?.RemoteIpAddress != null)
                ip = context.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullString() != string.Empty)
                ip = CommonLib.GetHeaderValue(headers, "REMOTE_ADDR");

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullString() != string.Empty)
                throw new Exception("Unable to determine caller's IP.");

            return ip;
        }
        public static void MoveFiles(List<string> FileNames, string temppath, string strpath)
        {
            try
            {
                foreach (string folderName in FileNames.Where(x => x.Contains(Path.DirectorySeparatorChar)).Select(x => x.Split(Path.DirectorySeparatorChar)[0]).Distinct())
                {
                    CommonLib.CheckDir(strpath + Path.DirectorySeparatorChar + folderName);
                }

                if (FileNames.Where(f => f.IsNullString() != "").Count() > 0)
                {
                    foreach (string item in FileNames)
                    {
                        string ActualFileName = item.Split(CommonLib.DirectorySeparatorChar()).LastOrDefault();
                        if (System.IO.File.Exists(temppath + ActualFileName))
                        {
                            try
                            {
                                System.IO.File.Move(temppath + ActualFileName, strpath + item);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }
        public static char DirectorySeparatorChar()
        {
            return Path.DirectorySeparatorChar;
        }
        public static void WriteToFile(string _message)
        {
            string _Location = LogPath;
            //string _Location = webRootInfo + "/Errorlogs";

            CommonLib.CheckDir(_Location);
            _Location = System.IO.Path.Combine(_Location, DateTime.Today.ToString("dd-MM-yyyy") + ".txt");

            try
            {
                StreamWriter _sw = new StreamWriter(_Location, true);
                _sw.Write(_message);
                _sw.Close();
            }
            catch
            {
                //HttpContext.Current.Application["Error"] += _message;
            }
        }
        public static string ConvertTemplateString(string TemplateFormat, string AvailableValues, Dictionary<string, string> Args)
        {
            if (TemplateFormat != null)
            {
                if (Args != null)
                {
                    foreach (KeyValuePair<string, string> argObj in Args)
                    {
                        TemplateFormat = TemplateFormat.Replace("@" + argObj.Key + "#", argObj.Value.IsNullString());
                        TemplateFormat = TemplateFormat.Replace("{#" + argObj.Key + "#}", argObj.Value.IsNullString());
                    }
                }
                foreach (string item in AvailableValues.IsNullString().Split(','))
                {
                    TemplateFormat = TemplateFormat.Replace("@" + item + "#", "");
                    TemplateFormat = TemplateFormat.Replace("{#" + item + "#}", "");
                }
            }
            return TemplateFormat;
        }

        public static string GetHeaderValue(IHeaderDictionary headers, string key)
        {
            if (headers == null) return null;
            if (!string.IsNullOrEmpty(headers[key]))
            {
                return headers[key];
            }
            return null;
        }
        public static string GetRequestJSON(HttpRequest request)
        {
            string bodyStr = null;
            if (request.Body.CanSeek)
            {
                request.Body.Position = 0;
                using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = reader.ReadToEnd();
                }
            }
            return bodyStr;
        }
        public static int GetRowsPerPage()
        {
            return 10;
        }
        public static T GetAppConfig<T>(IConfiguration configuration)
        {
            RequestByString myConfigEncrypted = configuration.GetSection("AppConfig").Get<RequestByString>();
            if (myConfigEncrypted == null || myConfigEncrypted.id == null)
            {
                return configuration.GetSection("AppConfig").Get<T>();
            }
            else
            {
                string EncryptedStr = myConfigEncrypted.id;
                string DBEntitiesKey = EncryptedStr.Substring(0, ProgConstants.ConnKeySize);
                string DBEntitiesIV = EncryptedStr.Substring(ProgConstants.ConnKeySize, ProgConstants.ConnIVSize);
                EncryptedStr = EncryptedStr.Substring(ProgConstants.ConnKeySize + ProgConstants.ConnIVSize, EncryptedStr.Length - ProgConstants.ConnKeySize - ProgConstants.ConnIVSize);
                string AppConfigStr = TripleDES.Decrypt(EncryptedStr, DBEntitiesKey, DBEntitiesIV);
                return CommonLib.ConvertJsonToObject<T>(AppConfigStr);
            }
        }
        public static T ConvertJsonToObject<T>(object obj)
        {
            return (T)JsonConvert.DeserializeObject(obj.IsNullString(), typeof(T));
        }
        public static string IsNullString(this object str)
        {
            if (str == null)
                return string.Empty;
            try
            {
                return str.ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static void CheckDir(string strPath)
        {
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
        }
        public static string ConvertObjectToJson(object obj, bool isFormat = false)
        {
            if (obj != null)
            {
                if (isFormat)
                {
                    return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(obj, new DecimalFormatConverter());
                }
            }
            return string.Empty;
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(
           this TSource source,
           Func<TSource, TSource> nextItem)
           where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(
  this TSource source,
  Func<TSource, TSource> nextItem,
  Func<TSource, bool> canContinue)
        {
            for (TSource current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }
        public static string GetAllMessages(this Exception exception)
        {
            IEnumerable<string> messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);
            return string.Join(Environment.NewLine, messages);
        }
    }

    public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal));
        }

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            writer.WriteValue(string.Format("{0:N2}", value));
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                     object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class KeyValue
    {
        public KeyValue() { }

        public KeyValue(string _Key, string _Value)
        {
            Key = _Key;
            Value = _Value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public static class EncryptDecrypt
    {
        #region Encryption
        private static byte[] GetBytes(string keyBytes, int length)
        {
            byte[] keyBytes1 = new byte[length];
            byte[] parameterKeyBytes = System.Text.Encoding.UTF8.GetBytes(keyBytes);
            Array.Copy(parameterKeyBytes, 0, keyBytes1, 0, Math.Min(parameterKeyBytes.Length, keyBytes1.Length));
            return keyBytes1;
        }

        private static string Array2String<T>(IEnumerable<T> list)
        {
            return "[" + string.Join(",", list) + "]";
        }

        public static string Encrypt(string PlainText, string key, string iv)
        {
            byte[] keyBytes = GetBytes(key, 32);
            byte[] ivBytes = GetBytes(key, 16);
            RijndaelManaged aes = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Key = keyBytes,
                IV = ivBytes
            };

            ICryptoTransform encrypto = aes.CreateEncryptor();

            byte[] plainTextByte = Encoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return BitConverter.ToString(CipherText).Replace("-", string.Empty);
        }

        public static string Decrypt(string encryptedText, string key, string iv)
        {
            try
            {
                int length = encryptedText.Length;
                byte[] keyBytes = GetBytes(key, 32);
                byte[] ivBytes = GetBytes(key, 16);

                string encrytedTextNew = "";
                char[] encrytArray = encryptedText.ToCharArray(0, encryptedText.Length);
                for (int i = 0; i < encryptedText.Length; i++)
                {
                    if (i != 0)
                    {
                        int j = i + 1;
                        if (j % 2 == 0)
                        {
                            encrytedTextNew = encrytedTextNew + encrytArray[i] + "-";
                        }
                        else
                        {
                            encrytedTextNew = encrytedTextNew + encrytArray[i];
                        }
                    }
                    else if (i == 0)
                    {
                        encrytedTextNew = encrytedTextNew + encrytArray[i];
                    }
                }

                encrytedTextNew = encrytedTextNew.Remove(encrytedTextNew.Length - 1);

                RijndaelManaged aes = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Key = keyBytes,
                    IV = ivBytes
                };
                ICryptoTransform encrypto = aes.CreateDecryptor();

                byte[] plainTextByte = Array.ConvertAll<string, byte>(encrytedTextNew.Split('-'), s => Convert.ToByte(s, 16));
                byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
                return ASCIIEncoding.UTF8.GetString(CipherText);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string md5 = s.ToString();
            return md5;
        }

        public static bool IsValidSHA1(string s)
        {
            string regex = @"^[a-fA-F0-9]{40}$";
            Match match = Regex.Match(s, regex, RegexOptions.IgnoreCase);
            return (match.Success);
        }


        public static string GetSha256HashLower(string plainText)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2").ToLower());
                }
                return builder.ToString();
            }
        }

    }
    public static class MIMEhandler
    {
        private static readonly IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }


            return _mappings.TryGetValue(extension.ToLower(), out string mime) ? mime : "application/octet-stream";
        }
    }
}
