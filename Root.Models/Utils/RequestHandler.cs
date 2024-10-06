using Newtonsoft.Json;
using Root.Models.LogTables;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Root.Models.Utils
{
    public static class RequestHandler
    {
        static int clientTimeout = 30;
        public static async Task<string> GetDataAsync(RemoteLog remoteLog, string ApiPath, List<KeyValue> Headers = null, bool isSSLCertificate = true)
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            remoteLog.URL = ApiPath;
            //T result;
            try
            {
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    //ignore ssl certificate
                    if (!isSSLCertificate)
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }

                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value.IsNullString() != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        remoteLog.ResponseJSON = await client.GetStringAsync(ApiPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.EndDate = DateTime.Now;
            return remoteLog.ResponseJSON;
        }

        public static async Task<T> PostDataAsync<T>(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "", string contentType = "", double? APITimeOut = null, bool isScanAPI = false)
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            T result;
            string resultTest = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    //ignore ssl certificate
                    if (!isSSLCertificate)
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        if (APITimeOut.HasValue)
                        {
                            client.Timeout = TimeSpan.FromMinutes(APITimeOut.Value);
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        }
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value.IsNullString() != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsJsonAsync(ApiPath, obj);

                        resultTest = await response.Content.ReadAsStringAsync();
                        if (isScanAPI)
                        {
                            result = CommonLib.ConvertJsonToObject<T>(resultTest);
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                            //result = await response.Content.ReadAsJsonAsync<T>();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string _Error = "";

                if (ex != null)
                {
                    _Error = "===========================" + Environment.NewLine;
                    _Error = _Error + "Date         :" + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
                    _Error = _Error + "Error Desc   :" + ex.Message + Environment.NewLine;
                    _Error = _Error + "Source       :" + ex.Source + Environment.NewLine;
                    _Error = _Error + "Line No      :" + ex.StackTrace + Environment.NewLine;
                    _Error = _Error + "Help Link      :" + ex.HelpLink + Environment.NewLine;
                    _Error = _Error + "ApiPath      :" + ApiPath + Environment.NewLine;
                    _Error = _Error + "Headers      :" + CommonLib.ConvertObjectToJson(Headers) + Environment.NewLine;
                    _Error = _Error + "obj      :" + CommonLib.ConvertObjectToJson(obj) + Environment.NewLine;
                    _Error = _Error + "authenticationHeaderValue      :" + CommonLib.ConvertObjectToJson(authenticationHeaderValue) + Environment.NewLine;
                    _Error = _Error + "resultTest      :" + resultTest + Environment.NewLine;
                    _Error = _Error + "=============================";
                    CommonLib.WriteToFile(_Error);
                }
                throw;
            }
            remoteLog.ResponseJSON = resultTest;
            remoteLog.EndDate = DateTime.Now;
            return result;
        }

        public static async Task<string> PostDataAsyncWithoutBody<T>(RemoteLog remoteLog, string ApiPath, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null)
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    //ignore ssl certificate
                    if (!isSSLCertificate)
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value.IsNullString() != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsync(ApiPath, null);
                        var resultTest = await response.Content.ReadAsStringAsync();
                        remoteLog.ResponseJSON = resultTest.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.EndDate = DateTime.Now;
            return remoteLog.ResponseJSON;
        }

        public static async Task<T> PostXMLDataAsync<T>(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "", int timeOutInSeconds = 0)
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            T result;
            string resultTest = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                System.Net.Http.HttpContent content = new StringContent(obj.ToString(), UTF8Encoding.UTF8, "application/xml");
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    //ignore ssl certificate
                    if (!isSSLCertificate)
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }
                    if (certPath.IsNullString() != "")
                        httpClientHandler.ClientCertificates.Add(new X509Certificate2(certPath));
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        //client.DefaultRequestHeaders.Accept.Clear();

                        if (timeOutInSeconds > 0)
                        {
                            client.Timeout = TimeSpan.FromSeconds(timeOutInSeconds);
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        }

                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value.IsNullString() != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        //response = await client.PostAsJsonAsync(ApiPath, obj);

                        response = await client.PostAsync(ApiPath, content);
                        resultTest = await response.Content.ReadAsStringAsync();
                        result = (T)(object)resultTest;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            remoteLog.ResponseJSON = resultTest;
            remoteLog.EndDate = DateTime.Now;
            return result;
        }

        public static async Task<string> PostJsonAsPlain(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            string result;
            string resultTest = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    httpClientHandler.SslProtocols = SslProtocols.Tls12;
                    if (certPath.IsNullString() != "")
                        httpClientHandler.ClientCertificates.Add(new X509Certificate2(certPath));
                    System.Net.Http.HttpContent content = new StringContent(CommonLib.ConvertObjectToJson(obj), UTF8Encoding.UTF8, "text/plain");
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsync(ApiPath, content);
                        resultTest = await response.Content.ReadAsStringAsync();
                        result = resultTest;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.ResponseJSON = resultTest;
            remoteLog.EndDate = DateTime.Now;
            return result;
        }

        public static async Task<string> PostStringAsPlain(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            string result;
            string ResponseResult = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {

                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    if (!isSSLCertificate)
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }
                    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    httpClientHandler.SslProtocols = SslProtocols.Tls12;
                    if (certPath.IsNullString() != "")
                        httpClientHandler.ClientCertificates.Add(new X509Certificate2(certPath));
                    System.Net.Http.HttpContent content = new StringContent(obj.ToString(), UTF8Encoding.UTF8, "text/plain");
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsync(ApiPath, content);
                        ResponseResult = await response.Content.ReadAsStringAsync();
                        result = ResponseResult;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.ResponseJSON = ResponseResult;
            remoteLog.EndDate = DateTime.Now;
            return result;
        }

        public static async Task<string> PostXMLAsPlain(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            string result;
            string resultTest = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    httpClientHandler.SslProtocols = SslProtocols.Tls12;
                    if (certPath.IsNullString() != "")
                        httpClientHandler.ClientCertificates.Add(new X509Certificate2(certPath));
                    System.Net.Http.HttpContent content = new StringContent(obj.ToString(), UTF8Encoding.UTF8, "text/plain");
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsync(ApiPath, content);
                        resultTest = await response.Content.ReadAsStringAsync();
                        result = resultTest;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.ResponseJSON = resultTest;
            remoteLog.EndDate = DateTime.Now;
            return result;
        }

        public static async Task<T> PostXMLJsonDataAsync<T>(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "", int timeOutInSeconds = 0)
        {
            clientTimeout = 200;
            if (Headers != null)
            {
                remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
            }
            if (authenticationHeaderValue != null)
            {
                remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
            }
            remoteLog.URL = ApiPath;
            T result;
            string resultTest = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                System.Net.Http.HttpContent content = new StringContent(obj.ToString(), UTF8Encoding.UTF8, "application/xml");
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    if (!isSSLCertificate)
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }
                    if (certPath.IsNullString() != "")
                        httpClientHandler.ClientCertificates.Add(new X509Certificate2(certPath));
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        if (timeOutInSeconds > 0)
                        {
                            client.Timeout = TimeSpan.FromSeconds(timeOutInSeconds);
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        }
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value.IsNullString() != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsJsonAsync(ApiPath, obj);
                        resultTest = await response.Content.ReadAsStringAsync();
                        result = (T)(object)resultTest;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.ResponseJSON = resultTest;
            remoteLog.EndDate = DateTime.Now;
            return result;
        }
        public static async Task<HttpResponseMessage> PostDownloadFile(RemoteLog remoteLog, string ApiPath, object obj = null, List<KeyValue> Headers = null, bool isSSLCertificate = true, AuthenticationHeaderValue authenticationHeaderValue = null, string certPath = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                if (Headers != null)
                {
                    remoteLog.HeaderParams = CommonLib.ConvertObjectToJson(Headers);
                }
                if (authenticationHeaderValue != null)
                {
                    remoteLog.HeaderParams += CommonLib.ConvertObjectToJson(authenticationHeaderValue);
                }
                remoteLog.URL = ApiPath;



                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    httpClientHandler.SslProtocols = SslProtocols.Tls12;
                    if (certPath.IsNullString() != "")
                        httpClientHandler.ClientCertificates.Add(new X509Certificate2(certPath));
                    System.Net.Http.HttpContent content = new StringContent(CommonLib.ConvertObjectToJson(obj), UTF8Encoding.UTF8, "text/plain");
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        client.BaseAddress = new Uri(ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                        if (authenticationHeaderValue != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderValue.Scheme, authenticationHeaderValue.Parameter);
                        }
                        if (Headers != null)
                        {
                            foreach (KeyValue item in Headers.Where(c => c.Value != string.Empty))
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                            }
                        }
                        response = await client.PostAsync(ApiPath, content);

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            remoteLog.ResponseJSON = response.Content.ReadAsStreamAsync().ToString();
            remoteLog.EndDate = DateTime.Now;
            return response;
        }
    }
}
