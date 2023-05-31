using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.UserRepository;
using Rememory.WebApi.Exceptions;

namespace Rememory.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public BaseController(IUserRepository userRepository) : base()
    {
        _userRepository = userRepository;
    }
    
    protected async Task<User> GetCurrentUser()
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
            throw new ForbiddenException();
        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _userRepository.GetAsync(userId);
        return user ?? throw new NotFoundException("Entity 'User' not found");
    }
}