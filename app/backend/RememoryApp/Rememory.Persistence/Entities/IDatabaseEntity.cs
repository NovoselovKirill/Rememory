using MongoDB.Bson.Serialization.Attributes;

namespace Rememory.Persistence.Entities;

public interface IDatabaseEntity
{
    [BsonId]
    public Guid Id { get; set; }
}