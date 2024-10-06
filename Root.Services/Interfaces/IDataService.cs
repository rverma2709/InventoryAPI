using Microsoft.EntityFrameworkCore;
using Root.Models.StoredProcedures;
using Root.Models.Utils;
using System.Linq.Expressions;

namespace Root.Services.Interfaces
{
    public interface IDataService<TDbContext, TDbEnity> where TDbContext : DbContext where TDbEnity : class
    {
        IQueryable<TDbEnity> GetAll(Expression<Func<TDbEnity, object>>[] includes = null);
        IQueryable<TDbEnity> GetWhere(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null);
        Task<TDbEnity> GetSingle(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null);
        Task<TDbEnity> Detail(long id, Expression<Func<TDbEnity, object>>[] includes = null);
        Task Create(TDbEnity obj);
        void Update(TDbEnity obj);
        Task Save(List<string> FileList = null);
        Task<JsonOutputForList> FetchList<T>(SFParameters model, Expression<Func<T, object>>[] includes = null, bool ReadOnly = false) where T : class;
    }
}
