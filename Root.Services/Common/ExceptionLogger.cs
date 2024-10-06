using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Root.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models.Utils;

namespace Root.Services.Common
{
    public class ExceptionLogger : ExceptionFilterAttribute
    {
        private readonly ILogService _logService;

        public ExceptionLogger(ILogService logService)
        {
            _logService = logService;
        }

        //public override void OnException(ExceptionContext context)
        //{
        //    System.Threading.Tasks.Task<ResJsonOutput> task = _logService.HandleError(context.RouteData, context.HttpContext, context.Exception);
        //    task.Wait();
        //    context.Result = new JsonResult(task.Result);
        //}
    }
}
