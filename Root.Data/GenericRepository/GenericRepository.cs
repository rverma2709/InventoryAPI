using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Root.Data.GenericRepository
{
    public class GenericRepository<TDbContext, TDbEnity> : IGenericRepository<TDbEnity>
         where TDbEnity : class
    {
        private readonly DbContext _dbContext;
        internal IQueryable<TDbEnity> query;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            query = _dbContext.Set<TDbEnity>();
        }

        public IQueryable<TDbEnity> GetAll(Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                if (includes != null)
                {
                    //IQueryable<T> query = _dbContext.Set<T>(); ;
                    query = includes.Aggregate<Expression<Func<TDbEnity, object>>, IQueryable<TDbEnity>>(query, (current, expression) => current.Include(expression));

                    return query.AsQueryable();
                }
                return query.AsQueryable();
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
                if (includes != null)
                {
                    query = includes.Aggregate<Expression<Func<TDbEnity, object>>, IQueryable<TDbEnity>>
                        (query, (current, expression) => current.Include(expression));

                    return query.Where(where).AsQueryable();
                }
                else
                {
                    return query.Where(where).AsQueryable();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TDbEnity> GetSingle(Expression<Func<TDbEnity, bool>> where, Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                if (includes != null)
                {
                    query = includes.Aggregate<Expression<Func<TDbEnity, object>>, IQueryable<TDbEnity>>(query, (current, expression) => current.Include(expression));

                    return await query.Where(where).AsNoTracking().FirstOrDefaultAsync();
                }
                else
                {
                    return await query.Where(where).AsNoTracking().FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetPath<T>(Expression<Func<T, object>> expr)
        {
            var stack = new Stack<string>();

            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            while (me != null)
            {
                stack.Push(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            return string.Join(".", stack.ToArray());
        }

        public async Task<TDbEnity> GetById(long id, Expression<Func<TDbEnity, object>>[] includes = null)
        {
            try
            {
                TDbEnity model = await _dbContext.Set<TDbEnity>().FindAsync(id);
                if (includes != null)
                {
                    foreach (Expression<Func<TDbEnity, object>> path in includes)
                    {
                        string str = GetPath(path);
                        try
                        {
                            await _dbContext.Entry(model).Reference(str).LoadAsync();
                        }
                        catch
                        {
                            try
                            {
                                await _dbContext.Entry(model).Collection(str).LoadAsync();
                            }
                            catch
                            {
                                throw;
                            }
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Create(TDbEnity entity)
        {
            try
            {
                await _dbContext.Set<TDbEnity>().AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(TDbEnity entity)
        {
            try
            {
                //_dbContext.Entry(entity).State = EntityState.Modified; //TODO : Rectify, wherever update method is called on Create
                _dbContext.Set<TDbEnity>().Update(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Save()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveRange(List<TDbEnity> entities)
        {
            try
            {
                //_dbContext.Entry(entity).State = EntityState.Modified; //TODO : Rectify, wherever update method is called on Create
                foreach (TDbEnity entity in entities)
                {
                    _dbContext.Set<TDbEnity>().Remove(entity);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
