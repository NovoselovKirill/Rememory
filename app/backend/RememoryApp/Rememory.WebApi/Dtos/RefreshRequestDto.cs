namespace Rememory.WebApi.Dtos;

public class RefreshRequestDto
{
    public string DeviceId { get; set; } = null!;
    public string? RefreshToken { get; set; }
}