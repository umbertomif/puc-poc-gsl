using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace POC.GSL.Data.Mongo
{
    public class MongoUnitOfWork : UnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public MongoUnitOfWork(IOptions<MongoSettings> settings)
        {
            var connectionString = new ConnStringBuilder()
                .WithconnectionString(settings.Value.connectionString)
                .Get();

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(settings.Value.Database);
        }

        public Repository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new MongoRepository<TEntity>(_database);
        }

        public async Task SaveChangesAsync()
        {
            await Task.FromException(new NotImplementedException());
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
