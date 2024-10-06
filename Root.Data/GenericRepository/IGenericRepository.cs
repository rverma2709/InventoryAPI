using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Root.Data.GenericRepository
{
    public interface IGenericRepository<TDbEnity>
        where TDbEnity : class
    {
        

        IQueryable<TDbEnity> GetAll(Expression<Func<TDbEnity, object>>[] includes = null);

        IQueryable<TDbEnity> GetWhere(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null);

        Task<TDbEnity> GetSingle(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null);

        Task<TDbEnity> GetById(long id, Expression<Func<TDbEnity, object>>[] includes = null);

        Task Create(TDbEnity entity);

        void Update(TDbEnity entity);

        Task Save();

        void Dispose();
    }
}
