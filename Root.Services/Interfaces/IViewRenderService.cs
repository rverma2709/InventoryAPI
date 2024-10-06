using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Services.Interfaces
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string controllerName, string viewName, object model, string FilePath, string FileName);
        string GetPDF(string controllerName, string viewName, object model, string FilePath, string FileName);
        Task<Byte[]> GetPDFArray(string controllerName, string viewName, object model, string FilePath, string FileName);
    }
}
