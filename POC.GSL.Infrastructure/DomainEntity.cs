using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POC.GSL.Infrastructure
{
    public abstract class DomainEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}