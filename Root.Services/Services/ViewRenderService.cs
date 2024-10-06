using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Root.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;

namespace Root.Services.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderToStringAsync(string controllerName, string viewName, object model, string FilePath, string FileName)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
                var viewResultq = _razorViewEngine.FindPage(actionContext, viewName);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();


            }
        }


        //public virtual void CreatePDF(string html, string location, string fileName)
        //{
        //    String htmlText = html.ToString();
        //    Document document = new Document();
        //    string filePath = location + "pdf-" + fileName + ".pdf";
        //    PdfWriter.GetInstance(document, new FileStream(filePath, System.IO.FileMode.Create));
        //    document.Open();
        //    iTextSharp.text.html.simpleparser.HtmlWorker hw = new iTextSharp.text.html.simpleparser.HtmlWorker(document);
        //    hw.Parse(new StringReader(htmlText));
        //    document.Close();
        //}


        [Obsolete]
        public string GetPDF(string controllerName, string viewName, object model, string FilePath, string FileName)
        {
            string SaveOnServerPath = string.Empty;

            string Viewpath = "~/Views/" + controllerName + "/" + viewName + ".cshtml";

            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            try
            {
                var pdfAppResult1 = new ViewAsPdf(Viewpath, model)
                {

                    //  PageSize = Size.A4,
                    // IsLowQuality = true ,                 
                    //PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                    //PageSize = Rotativa.AspNetCore.Options.Size.A5,
                    //PageMargins = new Margins(0, 0, 0, 0),
                    //PageWidth = 500,
                    //PageHeight = 297
                    //, CustomSwitches = "";
                    SaveOnServerPath = FilePath + FileName.Replace("/", "-") + ".pdf",
                };

                var binaryApp1 = pdfAppResult1.BuildFile(actionContext);

                if (System.IO.File.Exists(FilePath + FileName.Replace("/", "-") + ".pdf"))
                {
                    return FileName.Replace("/", "-") + ".pdf";
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Obsolete]
        public Task<Byte[]> GetPDFArray(string controllerName, string viewName, object model, string FilePath, string FileName)
        {
            string SaveOnServerPath = string.Empty;

            string Viewpath = "~/Views/" + controllerName + "/" + viewName + ".cshtml";

            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            try
            {

                var pdfAppResult1 = new ViewAsPdf(Viewpath, model)
                {
                    SaveOnServerPath = FilePath + FileName.Replace("/", "-") + ".pdf",
                    //PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                    //CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
                };

                var binaryApp1 = pdfAppResult1.BuildFile(actionContext);

                return binaryApp1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
