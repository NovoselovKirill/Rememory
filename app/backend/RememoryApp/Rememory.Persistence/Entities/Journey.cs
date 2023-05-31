using MongoDB.Bson.Serialization.Attributes;

namespace Rememory.Persistence.Entities;

public class Journey : IDatabaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}