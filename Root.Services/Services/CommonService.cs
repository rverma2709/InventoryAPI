using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Root.Data.UnitOfWork;
using Root.Models.StoredProcedures;
using Root.Models.Utils;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Root.Services.Services
{
    public class CommonService : ICommonService
    {
        private readonly ProcUOM _procUOM;

        public CommonService(ProcUOM procUOM)
        {
            _procUOM = procUOM;
        }

        public async Task<T> ExecuteScalarAsync<TDbContext, T>(object obj, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext
        {

            //SqlParameterExtensions sqlParameterExtensions = new SqlParameterExtensions();
            List<SqlParameter> sqlParameters = obj != null ? SqlParameterExtensions.ToSqlParamsList(obj) : (List<SqlParameter>)obj;

            bool Status = false;
            string Params = obj != null ? CommonLib.ConvertObjectToJson(obj) : null;
            string ProcName = SqlParameterExtensions.GetStoredProcedureName(obj);
            Exception innerEx = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            T result = default(T);
            try
            {
                result = await _procUOM.ExecuteScalarAsync<TDbContext, T>(ProcName, sqlParameters, ReadOnly, QueryTimeOut);
                SqlParameterExtensions.ToObject(obj, sqlParameters);
                return result;
            }
            catch (Exception ex)
            {
                innerEx = ex;
            }
            finally
            {
                stopwatch.Stop();
                Task.Run(() => SaveProcLogs(ProcName, Status, stopwatch.Elapsed.TotalSeconds, obj, innerEx, ProcExecTimedout));
            }

            if (innerEx != null)
            {
                throw innerEx;
            }
            return result;
        }
        public async Task<int> ExecuteNonQueryAsync<TDbContext>(object obj, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            //SqlParameterExtensions sqlParameterExtensions = new SqlParameterExtensions();
            List<SqlParameter> sqlParameters = obj != null ? SqlParameterExtensions.ToSqlParamsList(obj) : (List<SqlParameter>)obj;

            bool Status = false;
            string Params = obj != null ? CommonLib.ConvertObjectToJson(obj) : null;
            string ProcName = SqlParameterExtensions.GetStoredProcedureName(obj);
            Exception innerEx = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int result = 0;

            try
            {
                result = await _procUOM.ExecuteNonQueryAsync<TDbContext>(ProcName, sqlParameters, ReadOnly, QueryTimeOut);
                SqlParameterExtensions.ToObject(obj, sqlParameters);
            }
            catch (Exception ex)
            {
                innerEx = ex;
            }
            finally
            {
                stopwatch.Stop();
                Task.Run(() => SaveProcLogs(ProcName, Status, stopwatch.Elapsed.TotalSeconds, obj, innerEx, ProcExecTimedout));
            }

            if (innerEx != null)
            {
                throw innerEx;
            }
            return result;
        }
        public async Task<List<T>> ExecuteSPAsync<TDbContext, T>(object obj, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            //SqlParameterExtensions sqlParameterExtensions = new SqlParameterExtensions();
            List<SqlParameter> sqlParameters = obj != null ? SqlParameterExtensions.ToSqlParamsList(obj) : (List<SqlParameter>)obj;

            bool Status = false;
            string Params = obj != null ? CommonLib.ConvertObjectToJson(obj) : null;
            string ProcName = SqlParameterExtensions.GetStoredProcedureName(obj);
            Exception innerEx = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<T> result = null;

            try
            {
                result = await _procUOM.ExecuteSPAsync<TDbContext, T>(ProcName, sqlParameters, ReadOnly, QueryTimeOut);
                SqlParameterExtensions.ToObject(obj, sqlParameters);
            }
            catch (Exception ex)
            {
                innerEx = ex;
            }
            finally
            {
                stopwatch.Stop();
                Task.Run(() => SaveProcLogs(ProcName, Status, stopwatch.Elapsed.TotalSeconds, obj, innerEx, ProcExecTimedout));
            }

            if (innerEx != null)
            {
                throw innerEx;
            }
            return result;
        }

        public async Task<bool> ExecuteSPtoCSVAsync<TDbContext, T>(object obj, string FileName, bool appendHeader = true, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            //SqlParameterExtensions sqlParameterExtensions = new SqlParameterExtensions();
            List<SqlParameter> sqlParameters = obj != null ? SqlParameterExtensions.ToSqlParamsList(obj) : (List<SqlParameter>)obj;

            bool Status = false;
            string Params = obj != null ? CommonLib.ConvertObjectToJson(obj) : null;
            string ProcName = SqlParameterExtensions.GetStoredProcedureName(obj);
            Exception innerEx = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string SelectedColumnNamesForReport = "";

            Type type = obj.GetType();

            PropertyInfo propertyInfo = type.GetProperty("SelectedColumnNamesForReport");

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(string))
            {
                SelectedColumnNamesForReport = propertyInfo.GetValue(obj) as string;
            }

            try
            {
                Status = await _procUOM.ExecuteSPtoCSVAsync<TDbContext, T>(ProcName, FileName, sqlParameters, appendHeader, ReadOnly, SelectedColumnNamesForReport);
                SqlParameterExtensions.ToObject(obj, sqlParameters);
            }
            catch (Exception ex)
            {
                innerEx = ex;
            }
            finally
            {
                stopwatch.Stop();
                Task.Run(() => SaveProcLogs(ProcName, Status, stopwatch.Elapsed.TotalSeconds, obj, innerEx, ProcExecTimedout));
            }

            if (innerEx != null)
            {
                throw innerEx;
            }
            return Status;
        }

        public async Task<JsonOutputForList> FetchList<TDbContext, T>(SFParameters model, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext
        where T : class
        {
            JsonOutputForList result = new JsonOutputForList();
            try
            {
                List<T> resultList = await ExecuteSPAsync<TDbContext, T>(model, ReadOnly, ProcExecTimedout, QueryTimeOut);
                result.TotalCount = model.RowCount;
                result.PageNo = model.pageNo;
                result.RowsPerPage = model.rowsPerPage;
                if (resultList.Any())
                {
                    result.ResultList = resultList;
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

        private async Task SaveProcLogs(string ProcName, bool Status, double ExecTime, object obj, Exception ex = null, double ProcExecTimedout = 5000)
        {
            if (ExecTime > ProcExecTimedout || ex != null)
            {
                SPSaveProcLogs spSaveProcLogs = new SPSaveProcLogs();
                //SqlParameterExtensions sqlParameterExtensions = new SqlParameterExtensions();
                if (ProcName != SqlParameterExtensions.GetStoredProcedureName(spSaveProcLogs))
                {
                    string ErrorMessage = string.Empty;
                    string Params = obj != null ? CommonLib.ConvertObjectToJson(obj) : string.Empty;

                    if (ex != null)
                    {
                        ErrorMessage = "===========================" + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "Date         :" + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "Error Desc   :" + ex.Message + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "InnerException   :" + CommonLib.GetAllMessages(ex) + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "Source       :" + ex.Source + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "Line No      :" + ex.StackTrace + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "Help Link      :" + ex.HelpLink + Environment.NewLine;
                        ErrorMessage = ErrorMessage + "=============================";
                    }

                    spSaveProcLogs.Status = Status;
                    spSaveProcLogs.ProcName = ProcName;
                    spSaveProcLogs.Params = Params;
                    spSaveProcLogs.ErrorMessage = ErrorMessage;
                    spSaveProcLogs.ExecTime = ExecTime;
                    await ExecuteNonQueryAsync<DBEntities>(spSaveProcLogs);
                }
            }
        }
    }
}
