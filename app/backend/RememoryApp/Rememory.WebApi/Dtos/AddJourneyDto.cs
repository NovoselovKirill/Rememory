namespace Rememory.WebApi.Dtos;

public class AddJourneyDto
{
    public string Title { get; set; } = null!;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}