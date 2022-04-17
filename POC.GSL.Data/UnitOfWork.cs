using System;
using System.Threading.Tasks;
namespace POC.GSL.Data
{
    public interface UnitOfWork : IDisposable
    {
        Repository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task SaveChangesAsync();
    }
}
