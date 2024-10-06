using Microsoft.Extensions.Caching.Memory;
using Root.Models.Application.Tables;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Root.Services.Services
{
    public static class MemberInfoGetting
    {
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
    }
    public class CacheUOM : IDisposable
    {
        private readonly ICommonService _commonService;
        private readonly ICacheService _cacheService;
        private readonly IMemoryCache _memoryCache;
        private readonly string abc = "List";
        public CacheUOM(ICacheService cacheService, ICommonService commonService, IMemoryCache memoryCache)
        {
            _commonService = commonService;
            _cacheService = cacheService;
            _memoryCache = memoryCache;
        }

        #region Generic Public Methods
        async Task<List<T>> GetValuesAsync<T>(bool update = false, long? stateId = null, List<RequestById> ids = null, [CallerMemberName] string name = "") where T : class
        {
            if (stateId.HasValue && stateId == 0)
                return new List<T>();

            var lastParenSet = name.LastIndexOf(abc);
            string propName = name.Substring(0, lastParenSet > -1 ? lastParenSet : name.Count());
            return await GetMastersData<T>(propName, update, stateId, ids);
        }
        private async Task<List<T>> GetMastersData<T>(string model, bool update = false, long? stateId = null, List<RequestById> ids = null, CacheChannels channelId = CacheChannels.Core) where T : class
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            //#if (DEBUG)
            //            update = true;
            //#endif
            try
            {
                List<T>? lst;
                //string keyName = model + (stateId.HasValue ? ":" + stateId.IsNullString() : "");
                //if (!update)
                //{
                //    lst = await _memoryCache.GetOrCreateAsync(channelId.ToString() + "." + keyName, async (cacheEntry) =>
                //    {
                //        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2);
                //        List<T> data;
                //        if (await _cacheService.KeyExists(channelId, keyName))
                //        {
                //            Debug.WriteLine("GetMastersData-Redis | " + keyName + " | 2 | " + stopwatch.ElapsedMilliseconds);
                //            data = await _cacheService.Read<List<T>>(channelId, keyName);
                //        }
                //        else
                //        {
                //            data = await UpdateMaster<T>(model, stateId, ids, channelId);
                //            Debug.WriteLine("GetMastersData-DB | " + keyName + " | 3 | " + stopwatch.ElapsedMilliseconds.ToString());
                //        }
                //        Debug.WriteLine("GetMastersData-Memory-DB | " + keyName + " | 4 | " + stopwatch.ElapsedMilliseconds);
                //        if (data is null)
                //        {
                //            cacheEntry.Dispose();
                //            data = new List<T>();
                //        }

                //        return data;
                //    }).ConfigureAwait(false);
                //}
                //else
                //{
                //    lst = await UpdateMaster<T>(model, stateId, ids, channelId);
                //}
                lst = await UpdateMaster<T>(model, stateId, ids, channelId);
                return lst;
            }
            catch (Exception ex)
            {
                throw new Exception("GetMastersData: Error while accessing the Master's data(" + channelId.ToString() + ").", ex);
            }
        }

        private protected async Task<List<T>> UpdateMaster<T>(string model, long? stateId = null, List<RequestById> ids = null, CacheChannels channelId = CacheChannels.Core) where T : class
        {
           // string keyName = model + (stateId.HasValue ? ":" + stateId.IsNullString() : "");
            GC.Collect(0);
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                SPGetMastersForCache spGetMastersForCache = new SPGetMastersForCache();
                spGetMastersForCache.TableName = model;
                spGetMastersForCache.StateId = stateId;
                spGetMastersForCache.Ids = ids;
                List<T> lst = await _commonService.ExecuteSPAsync<DBEntities, T>(spGetMastersForCache);
                //Debug.WriteLine("UpdateMaster-Read-DB | " + keyName + " | 1 | " + stopwatch.ElapsedMilliseconds);
                //string jsonList = CommonLib.ConvertObjectToJson(lst);
                //await _cacheService.Write(channelId, keyName, CommonLib.ConvertObjectToJson(lst));
                //Debug.WriteLine("UpdateMaster-Write-Redis | " + keyName + " | 2 | " + stopwatch.ElapsedMilliseconds);
                return lst;
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateAllMaters: Error while updating the Master's data(" + channelId.ToString() + ").", ex);
            }
        }
        public async Task UpdateAllMasters(CacheChannels channelId = CacheChannels.Core)
        {
            string methodName = "";
            try
            {
                foreach (var method in this.GetType().GetMethods().Where(x => x.Name.EndsWith(abc) && x.GetParameters().Count() == 1))
                {
                    methodName = method.Name;
                    Task x = (Task)this.GetType().GetMethod(method.Name).Invoke(this, new object[] { true });
                    //x.GetAwaiter().GetResult();
                    await x;
                }
                List<ParentDDResult> States = await this.StatesList();
                foreach (var method in this.GetType().GetMethods().Where(x => x.Name.EndsWith(abc) && x.GetParameters().Where(y => y.Name == "stateId").Count() == 1))
                {
                    foreach (var item in States)
                    {
                        methodName = method.Name;
                        long stateId = Convert.ToInt64(item.Value);
                        Task x = (Task)this.GetType().GetMethod(method.Name).Invoke(this, new object[] { stateId, true });
                        //x.GetAwaiter().GetResult();
                        await x;
                    }
                }
                List<ParentDDResult> Districts = await this.DistrictsList();
                foreach (var method in this.GetType().GetMethods().Where(x => x.Name.EndsWith(abc) && x.GetParameters().Where(y => y.Name == "districtId").Count() == 1))
                {
                    foreach (var item in Districts)
                    {
                        methodName = method.Name;
                        long districtId = Convert.ToInt64(item.Value);
                        Task x = (Task)this.GetType().GetMethod(method.Name).Invoke(this, new object[] { districtId, true });
                        //x.GetAwaiter().GetResult();
                        await x;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateAllMaters: Error while updating the Master's data(" + channelId.ToString() + ")." +
                    methodName, ex);
            }
        }
        #endregion
        public async Task<List<ParentDDResult>> StatesList(bool update = false) { return await GetValuesAsync<ParentDDResult>(update); }
        public async Task<List<ParentDDResult>> DistrictsList(bool update = false) { return await GetValuesAsync<ParentDDResult>(update, null); }
        public async Task<List<DeviceType>> DeviceTypeList(bool update = false) { return await GetValuesAsync<DeviceType>(update, null); }
        public async Task<List<BrandDetail>> BrandDetails(bool update = false) { return await GetValuesAsync<BrandDetail>(update, null); }
        public async Task<List<DeviceModeldetail>> DeviceModeldetails(bool update = false) { return await GetValuesAsync<DeviceModeldetail>(update, null); }
        public async Task<List<DeviceProcessorDetail>> DeviceProcessorDetails(bool update = false) { return await GetValuesAsync<DeviceProcessorDetail>(update, null); }
        public async Task<List<GenerationDetail>> GenerationDetails(bool update = false) { return await GetValuesAsync<GenerationDetail>(update, null); }
        public async Task<List<RAMDetail>> RAMDetails(bool update = false) { return await GetValuesAsync<RAMDetail>(update, null); }
        public async Task<List<HardDiskDetail>> HardDiskDetails(bool update = false) { return await GetValuesAsync<HardDiskDetail>(update, null); }
        public async Task<List<ProcurementType>> ProcurementTypes(bool update = false) { return await GetValuesAsync<ProcurementType>(update, null); }
        public async Task<List<VendorDetail>> VendorDetails(bool update = false) { return await GetValuesAsync<VendorDetail>(update, null); }
        public async Task<List<InventoryUser>> InventoryUsersDetails(bool update = false) { return await GetValuesAsync<InventoryUser>(update, null); }
        public async Task<List<InventoryRole>> InventoryUsersRoles(bool update = false) { return await GetValuesAsync<InventoryRole>(update, null); }
        public async Task<List<NotificationTemplate>> NotificationTemplatesList(bool update = false) { return await GetValuesAsync<NotificationTemplate>(update); }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
