namespace Rememory.WebApi.Options;

public class JwtAuthOptions
{
    public string Secret { get; set; } = null!;
    public int AccessTokenExpiresInMinutes { get; set; }
    public int RefreshTokenExpiresInDays { get; set; }
}