using Microsoft.AspNetCore.Authorization;
using Rememory.Persistence.Repositories.UserRepository;
using Microsoft.AspNetCore.Mvc;

namespace Rememory.WebApi.Controllers;

[Authorize]
public class UsersController : BaseController
{
    public UsersController(IUserRepository userRepository) : base(userRepository)
    {
    }
    
    
    [HttpGet("current")]
    public async Task<ActionResult> GetUser()
    {
        var user = await GetCurrentUser();
        return Ok(user);
    }
}