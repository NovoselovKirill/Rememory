using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rememory.Persistence.Models;

namespace Rememory.Persistence.Entities;

public class Note : IDatabaseEntity
{
    public Guid Id { get; set; }
    public Guid JourneyId { get; set; }
    public DateTime DateTime { get; set; }
    
    [JsonConverter(typeof(StringEnumConverter))]
    [BsonRepresentation(BsonType.String)]
    public NoteContentType Type { get; set; }
    
    public string Content { get; set; } = null!;
}