using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Rememory.WebApi.Dtos;

public class DeviceIdDto
{
    [FromHeader]
    [StringLength(100, MinimumLength = 32)]
    public string DeviceId { get; set; } = null!;
}