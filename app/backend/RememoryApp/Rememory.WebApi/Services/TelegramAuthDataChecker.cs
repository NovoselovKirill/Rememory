using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rememory.WebApi.Dtos;
using Rememory.WebApi.Options;
using Telegram.Bot.Extensions.LoginWidget;

namespace Rememory.WebApi.Services;

public class TelegramAuthDataChecker
{
    private const int SecondsInMinute = 60;
    private readonly LoginWidget _loginWidget;
    private static readonly (string Name, MethodInfo Getter)[] Properties;

    static TelegramAuthDataChecker()
    { 
        Properties = typeof(TelegramLoginRequestDto)
            .GetProperties()
            .Select(p =>
            {
                var bindNameObj = p
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attr => attr is BindPropertyAttribute);
                var bindName = (bindNameObj as BindPropertyAttribute)?.Name ?? p.Name;
                return (bindName, p.GetMethod!);
            })
            .ToArray();
    }

    public TelegramAuthDataChecker(IOptions<TelegramAuthOptions> options)
    {
        var botToken = options.Value.BotToken;
        var allowedTimeOffsetInMinutes = options.Value.AllowedTimeOffsetInMinutes;
        var allowedTimeOffsetInSeconds = allowedTimeOffsetInMinutes * SecondsInMinute;
        _loginWidget = new LoginWidget(botToken) { AllowedTimeOffset = allowedTimeOffsetInSeconds };
    }
        
    public Authorization CheckAuthorization(TelegramLoginRequestDto tgData)
    {
        var fields = new SortedDictionary<string, string>();
        foreach (var (name, getter) in Properties)
        {
            var getterRes = getter.Invoke(tgData, Array.Empty<object>());
            if (getterRes is not null)
                fields[name] = getterRes.ToString()!;
        }
         
        return _loginWidget.CheckAuthorization(fields);
    }
}