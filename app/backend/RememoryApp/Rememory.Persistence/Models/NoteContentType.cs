using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rememory.Persistence.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum NoteContentType
{
    Text,
}