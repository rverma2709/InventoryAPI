using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Services.Interfaces
{
    public interface ICacheService
    {
        Task<string?> Read(CacheChannels channelId, string key);
        Task<T> Read<T>(CacheChannels channelId, string key) where T : class;
        Task Write(CacheChannels channelId, string key, string value, int? SlidingExpiry = null);
        Task Delete(CacheChannels channelId, string key);
        Task<bool> KeyExists(CacheChannels channelId, string key);
    }
    public enum CacheChannels
    {
        Core,
        AdminPortal,
        Website,
        CronJobAPI,
        API
    }
}
