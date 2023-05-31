using MongoDB.Bson.Serialization.Attributes;

namespace Rememory.Persistence.Entities;

public class User : IDatabaseEntity
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;
    
    [BsonIgnoreIfNull]
    public long TelegramId { get; set; }
    
    [BsonIgnoreIfNull]
    public string? FirstName { get; set; }
    
    [BsonIgnoreIfNull]
    public string? LastName { get; set; }
}