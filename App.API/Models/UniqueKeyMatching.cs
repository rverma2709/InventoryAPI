using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Root.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace App.API.Models
{
    public class UniqueKeyMatching : Attribute, IAsyncActionFilter
    {
        private readonly ICacheService _cacheService;
        private readonly IMemoryCache _memoryCache;

        public UniqueKeyMatching(ICacheService cacheService, IMemoryCache memoryCache)
        {
            _cacheService = cacheService;
            _memoryCache = memoryCache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            var requestHeaders = context.HttpContext.Request.Headers;

            if (requestHeaders.TryGetValue("X-Unique-Key", out var requestUniqueKey))
            {
                
                //var cachedUniqueKey =  await _cacheService.Read(CacheChannels.AdminPortal, "X-Unique-Key"); ;
                var cachedUniqueKey =  _memoryCache.Get("X-Unique-Key");


                if (cachedUniqueKey == requestUniqueKey)
                {
                    
                    await next();
                }
                else
                {
                    
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Content = "Forbidden: Unique key mismatch"
                    };
                }
            }
            else
            {
                
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Bad Request: X-Unique-Key header missing"
                };
            }
        }
    }
}
