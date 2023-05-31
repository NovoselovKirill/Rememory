using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace Rememory.WebApi.Attributes;

public class RequiredSnakeCaseErrorMessageAttribute : RequiredAttribute
{
    private static readonly DefaultContractResolver ContractResolver =
        new() { NamingStrategy = new SnakeCaseNamingStrategy() };

    private static string ToSnakeCase(string str) => ContractResolver.GetResolvedPropertyName(str);

    private string? _errorMessage;

    public override string FormatErrorMessage(string name)
        => _errorMessage ??= base.FormatErrorMessage($"'{ToSnakeCase(name)}'");
}