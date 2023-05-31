using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rememory.WebApi.Attributes;

namespace Rememory.WebApi.Dtos;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelegramLoginRequestDto
{
    [BindProperty(Name = "id")]
    [RequiredSnakeCaseErrorMessage]
    public long? Id { get; set; }
    
    [BindProperty(Name = "first_name")]
    public string? FirstName { get; set; }
    
    [BindProperty(Name = "last_name")]
    public string? LastName { get; set; }
    
    [BindProperty(Name = "username")]
    [RequiredSnakeCaseErrorMessage]
    public string Username { get; set; } = null!;
    
    [BindProperty(Name = "photo_url")]
    public string? PhotoUrl { get; set; }
    
    [BindProperty(Name = "auth_date")]
    [RequiredSnakeCaseErrorMessage]
    public long? AuthDate { get; set; }
    
    [BindProperty(Name = "hash")]
    [RequiredSnakeCaseErrorMessage]
    [StringLength(64, MinimumLength = 64, 
        ErrorMessage = "The field 'hash' must be a string of length 64")]
    public string Hash { get; set; } = null!;
}