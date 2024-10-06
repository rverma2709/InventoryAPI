using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Root.Models.Utils;
using Root.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.API.Models
{
   

    public class KeyGenrate : Attribute, IAsyncResultFilter
    {
        private readonly ICacheService _cacheService;
        private readonly IMemoryCache _memoryCache;
        public KeyGenrate(ICacheService cacheService, IMemoryCache memoryCache) {
            _cacheService= cacheService;
            _memoryCache= memoryCache;
        }
        

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            
            var uniqueKey = Guid.NewGuid().ToString();

            if (_memoryCache.TryGetValue("apikey", out _))
            {
                _memoryCache.Remove("apikey");
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _memoryCache.Set("X-Unique-Key", uniqueKey, cacheEntryOptions);
          

            context.HttpContext.Response.Headers.Add("X-Unique-Key", uniqueKey);

            //if (await _cacheService.KeyExists(CacheChannels.AdminPortal, "apikey"))
            //{
            //    await _cacheService.Delete(CacheChannels.AdminPortal, "apikey");
            //}
            //await _cacheService.Write(CacheChannels.AdminPortal, "apikey", uniqueKey);

            //context.HttpContext.Response.Headers.Add("apikey", uniqueKey);


            await next();
        }
    }

}
