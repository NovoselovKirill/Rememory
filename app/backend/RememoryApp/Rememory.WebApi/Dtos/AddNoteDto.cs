namespace Rememory.WebApi.Dtos;

public class AddNoteDto
{
    public Guid? JourneyId { get; set; }
    public DateTime DateTime { get; set; }
    public string Content { get; set; } = null!;
}