using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace POC.GSL.Data
{
    public interface Repository<TEntity>
    {
        IQueryable<TEntity> AsQueryable { get; }
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> filter);
        IEnumerable<TEntity> ToList();
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(object id);
        Task<TEntity> FindAsync(object id);
        Task<TEntity> FindByCustom(string filter, object value);
        IEnumerable<TEntity> FindAllByCustom(string filter, object value);
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void Remove(object id);
        void Remove(TEntity entityToRemove);
        void Update(TEntity entityToUpdate);
    }
}