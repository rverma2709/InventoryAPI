using Microsoft.EntityFrameworkCore;
using Root.Models.StoredProcedures;
using Root.Models.Utils;

namespace Root.Services.Interfaces
{
    public interface ICommonService//<TDbContext> where TDbContext : DbContext
    {
        Task<T> ExecuteScalarAsync<TDbContext, T>(object obj, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext;
        Task<int> ExecuteNonQueryAsync<TDbContext>(object obj, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext;
        Task<List<T>> ExecuteSPAsync<TDbContext, T>(object obj, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext;
        Task<JsonOutputForList> FetchList<TDbContext, T>(SFParameters model, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext
        where T : class;
        Task<bool> ExecuteSPtoCSVAsync<TDbContext, T>(object obj, string FileName, bool appendHeader = true, bool ReadOnly = false, double ProcExecTimedout = 5000, int QueryTimeOut = 60) where TDbContext : DbContext;

    }
}
