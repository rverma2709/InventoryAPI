using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Root.Data.UnitOfWork;
using Root.Models.Config;
using Root.Models.StoredProcedures;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Root.Services.Services
{
    public class DataService<TDbContext, TDbEnity> : IDataService<TDbContext, TDbEnity> where TDbContext : DbContext where TDbEnity : class
    {
        private readonly WriteUOM<TDbContext> _writeUOM;
        private readonly ReadUOM<TDbContext> _readUOM;
        private readonly CommonAppConfig _appConfig;
        private readonly ICommonService _procUOM;

        ///// <summary>
        ///// Public constructor.
        ///// </summary>
        public DataService(
            WriteUOM<TDbContext> writeUOM,
            ReadUOM<TDbContext> readUOM,
            ICommonService procUOM,
            IConfiguration configuration)
        {
            _writeUOM = writeUOM;
            _readUOM = readUOM;
            _procUOM = procUOM;
            _appConfig = CommonLib.GetAppConfig<CommonAppConfig>(configuration);
        }
        ~DataService()
        {
            _writeUOM.Dispose();
            _readUOM.Dispose();
        }

        public IQueryable<TDbEnity> GetAll(Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                return (_readUOM.Repository<TDbEnity>().GetAll(includes));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IQueryable<TDbEnity> GetWhere(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                return (_readUOM.Repository<TDbEnity>().GetWhere(where, includes));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<TDbEnity> GetSingle(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                return await _readUOM.Repository<TDbEnity>().GetSingle(where, includes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TDbEnity> Detail(long id, Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                return await _readUOM.Repository<TDbEnity>().GetById(id, includes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Create(TDbEnity obj)
        {
            try
            {
                await _writeUOM.Repository<TDbEnity>().Create(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(TDbEnity obj)
        {
            try
            {
                _writeUOM.Repository<TDbEnity>().Update(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Save(List<string> FileList = null)
        {
            try
            {
                await _writeUOM.Save();
                if (FileList != null && FileList.Count > 0)
                {
                    MoveFiles(FileList);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void MoveFiles(List<string> FileNames)
        {
            try
            {
                CommonLib.MoveFiles(FileNames, _appConfig.uploadingConfigs.TempFolder + CommonLib.DirectorySeparatorChar(), _appConfig.uploadingConfigs.Location + CommonLib.DirectorySeparatorChar());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetPK<T>()
        {
            Dictionary<string, string> _dict = new Dictionary<string, string>();

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var propt = prop.CustomAttributes.Where(ele => ele.AttributeType == typeof(KeyAttribute));
                if (propt != null)
                {
                    return prop.Name;
                }
            }
            return null;
        }

        public async Task<JsonOutputForList> FetchList<T>(SFParameters model, Expression<Func<T, object>>[] includes = null, bool ReadOnly = false) where T : class
        {
            JsonOutputForList result = new JsonOutputForList();
            try
            {
                List<RequestById> resultList = await _procUOM.ExecuteSPAsync<TDbContext, RequestById>(model, ReadOnly, QueryTimeOut: _appConfig.dbconfig.DBConnTimeOut);
                List<long> IdList = resultList.Select(s => s.id).ToList();
                result.TotalCount = model.RowCount;
                result.PageNo = model.pageNo;
                result.RowsPerPage = model.rowsPerPage;

                //ParameterExpression argParam = Expression.Parameter(typeof(T), "s");
                //Expression pkProperty = Expression.Property(argParam, "AppSettingId");
                //Expression lambda = Expression.Constant(false);
                //foreach (var item in IdList)
                //{
                //    var constant = Expression.Constant(item);
                //    Expression e1 = Expression.Equal(pkProperty, constant);
                //    lambda = Expression.OrElse(lambda, e1);
                //}
                if (resultList.Any())
                {
                    string key = GetPK<T>();

                    var item = Expression.Parameter(typeof(T), "item");
                    var memberValue = key.Split('.').Aggregate((Expression)item, Expression.PropertyOrField);
                    var memberType = memberValue.Type;
                    //foreach (var item1 in resultList)
                    //{
                    //	var condition = Expression.Equal(memberValue, Expression.Constant(item1.id, memberType));
                    //	exp = exp == null ? condition : Expression.OrElse(exp, condition);
                    //}
                    //resultList.ForEach((item) => exp = exp == null ? Expression.Equal(memberValue, Expression.Constant(item.id, memberType)) : Expression.OrElse(exp, Expression.Equal(memberValue, Expression.Constant(item.id, memberType))));

                    if (resultList.Count > 0)
                    {

                        var eParam = item;
                        var method = IdList.GetType().GetMethod("Contains");
                        var call = Expression.Call(Expression.Constant(IdList), method, Expression.Property(eParam, key));

                        IQueryable<T> clctn = _writeUOM.Repository<T>().GetWhere(Expression.Lambda<Func<T, bool>>(call, item), includes);

                        List<T> ResultList = clctn.ToList();

                        result.ResultList = ResultList.OrderBy(x => IdList.IndexOf(Convert.ToInt64(x.GetType().GetProperty(key).GetValue(x, null)))).Distinct().ToList();
                    }
                }
                else
                {
                    result.ResultList = new List<T>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
