using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace POC.GSL.Data.Mongo
{
    public class MongoRepository<TEntity> : Repository<TEntity> where TEntity : class
    {
        private IMongoDatabase _database;
        private IMongoCollection<TEntity> _mongoCollection;

        public MongoRepository(IMongoDatabase database)
        {
            _database = database;
            _mongoCollection = _database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public IQueryable<TEntity> AsQueryable => _mongoCollection.AsQueryable();

        public void Add(TEntity entity)
        {
            _mongoCollection.InsertOne(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _mongoCollection.InsertOneAsync(entity);
        }

        public TEntity Find(object id)
        {
            var entity = Builders<TEntity>.Filter.Eq("Id", id);
            return _mongoCollection.Find(entity).SingleOrDefault();
        }

        public async Task<TEntity> FindByCustom(string filter,object value)
        {
            var entity = Builders<TEntity>.Filter.Eq(filter, value);
            return _mongoCollection.Find(entity).SingleOrDefault();
        }

        public IEnumerable<TEntity> FindAllByCustom(string filter, object value)
        {
            var entity = Builders<TEntity>.Filter.Eq(filter, value);
            return _mongoCollection.Find(entity).ToList();
        }

        public async Task<TEntity> FindAsync(object id)
        {
            var entity = Builders<TEntity>.Filter.Eq("Id", id);
            return (await _mongoCollection.FindAsync(entity)).SingleOrDefault();
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            return _mongoCollection.Find(filter).ToList();
        }

        public IEnumerable<TEntity> ToList()
        {
            return _mongoCollection.Find(x => true).ToList();
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _mongoCollection.Find(predicate).SingleOrDefaultAsync();
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _mongoCollection.Find(predicate).FirstOrDefaultAsync();
        }

        public void Remove(object id)
        {
            _mongoCollection.DeleteOne(Builders<TEntity>.Filter.Eq("Id", id.ToString()));
        }

        public void Remove(TEntity entityToRemove)
        {
            var id = GetId(entityToRemove);
            _mongoCollection.DeleteOne(Builders<TEntity>.Filter.Eq("Id", id));
        }

        public void Update(TEntity entityToUpdate)
        {
            _mongoCollection.FindOneAndReplace(Builders<TEntity>.Filter.Eq("Id", GetId(entityToUpdate)), entityToUpdate);
        }


        private string GetId(TEntity entity)
        {
            return typeof(TEntity).GetProperty("Id", typeof(string)).GetValue(entity).ToString();
        }
    }
}
