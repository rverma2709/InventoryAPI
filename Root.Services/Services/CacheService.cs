using Root.Models.Config;
using Root.Models.Utils;
using Root.Services.Interfaces;
using StackExchange.Redis;

namespace Root.Services.Services
{
    public class CacheService : ICacheService, IDisposable
    {
        public static List<ChannelConfig> channelConfigs;
        public static CacheChannels parentChannelID;
        //public static string conn;
        public static ConfigurationOptions configurationOption;
        private StackExchange.Redis.IDatabase _db;
        private static Lazy<ConnectionMultiplexer> Connection = new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(configurationOption));
        public CacheService()
        {
            
           // _db = Connection.Value.GetDatabase();
            
        }
        private ChannelConfig GetConfig(CacheChannels channelId)
        {
            ChannelConfig? channelConfig = channelConfigs.FirstOrDefault(x => x.ChannelID.Equals(channelId.ToString(), StringComparison.OrdinalIgnoreCase));
            if (channelConfig == null)
            {
                throw new Exception("Config: Does not exists(" + channelId.ToString() + ").");
            }
            return channelConfig;
        }

        public async Task<string> Read(CacheChannels channelId, string key)
        {
            try
            {
                ChannelConfig channelConfig = GetConfig(channelId);
                if (channelConfig.CanRead)
                {
                    string result = await _db.StringGetAsync(channelId + ":" + key);
                    if (channelConfig.ChannelID != CacheChannels.Core.ToString() && channelConfig.SlidingExpiry > 0 && await KeyExists(channelId, key) && !key.StartsWith("OTP:"))
                    {
                        await Write(channelId, key, result);
                    }
                    return result;
                }
                else
                {
                    throw new Exception("Get: do not have enough access(" + channelId.ToString() + ").");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Get: do not have enough access(" + channelId.ToString() + ").", ex);
            }
        }

        public async Task<T> Read<T>(CacheChannels channelId, string key) where T : class
        {
            try
            {
                string result = await Read(channelId, key);
                return CommonLib.ConvertJsonToObject<T>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Get: do not have enough access(" + channelId.ToString() + ").", ex);
            }
        }

        public async Task Write(CacheChannels channelId, string key, string value, int? SlidingExpiry = null)
        {
            try
            {
                ChannelConfig channelConfig = GetConfig(channelId);
                if (channelConfig.CanWrite)
                {
                    TimeSpan? timeSpan = null;
                    if ((SlidingExpiry ?? channelConfig.SlidingExpiry) > 0)
                    {
                        timeSpan = new TimeSpan(0, 0, (SlidingExpiry * 60 ?? (channelConfig.SlidingExpiry)));
                    }
                    await _db.StringSetAsync(channelId + ":" + key, value, timeSpan);
                }
                else
                {
                    throw new Exception("Set: do not have enough access(" + channelId.ToString() + ").");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Set: do not have enough access(" + channelId.ToString() + ").", ex);
            }
        }

        public async Task Delete(CacheChannels channelId, string key)
        {
            try
            {
                ChannelConfig channelConfig = GetConfig(channelId);
                if (channelConfig.CanDelete)
                {
                    try
                    {
                        await _db.KeyDeleteAsync(channelId + ":" + key);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    throw new Exception("Delete: do not have enough access(" + channelId + ").");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Delete: do not have enough access(" + channelId + ").", ex);
            }
        }

        public async Task<bool> KeyExists(CacheChannels channelId, string key)
        {
            return await _db.KeyExistsAsync(channelId + ":" + key);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
