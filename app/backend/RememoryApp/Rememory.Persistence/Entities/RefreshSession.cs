using MongoDB.Bson.Serialization.Attributes;

namespace Rememory.Persistence.Entities;

public class RefreshSession : IDatabaseEntity
{
    public Guid Id { get; set; }
    
    [BsonRequired]
    public Guid UserId { get; set; }

    [BsonRequired]
    public string DeviceId { get; set; } = null!;

    [BsonRequired]
    public string RefreshToken { get; set; } = null!;

    [BsonRequired]
    public DateTime ExpiresIn { get; set; }
}