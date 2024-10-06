using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Root.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Root.Services.Interfaces
{
    public interface ILogService
    {
        Task SaveLog<T>(T model) where T : class;

        Task<ResJsonOutput> HandleError(RouteData RouteData, HttpContext context, Exception ex, string modelName = "Record");
        Task<string> GetStatusMessage(string StatusCode, List<string> str = null, int languageId = (int)RootEnums.Languages.English, string Message = null);
    }
}
