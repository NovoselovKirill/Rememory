namespace Rememory.WebApi.Options;

public class TelegramAuthOptions
{
    public string BotToken { get; set; } = null!;
    public long AllowedTimeOffsetInMinutes { get; set; }
}