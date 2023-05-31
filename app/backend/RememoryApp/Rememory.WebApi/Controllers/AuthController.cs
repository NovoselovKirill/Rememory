using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.RefreshSessionRepository;
using Rememory.Persistence.Repositories.UserRepository;
using Rememory.WebApi.Dtos;
using Rememory.WebApi.Options;
using Rememory.WebApi.Services;
using Telegram.Bot.Extensions.LoginWidget;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Rememory.WebApi.Controllers;

[Route("api/auth")]
public class AuthController : Controller
{
    private readonly TelegramAuthDataChecker _telegramAuthDataChecker;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshSessionRepository _refreshSessionRepository;
    private readonly JwtAuthOptions _jwtAuthOptions;

    public AuthController(
        TelegramAuthDataChecker telegramAuthDataChecker,
        IUserRepository userRepository,
        IRefreshSessionRepository refreshSessionRepository,
        IOptions<JwtAuthOptions> jwtAuthOptions)
    {
        _telegramAuthDataChecker = telegramAuthDataChecker;
        _userRepository = userRepository;
        _refreshSessionRepository = refreshSessionRepository;
        _jwtAuthOptions = jwtAuthOptions.Value;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<TokensDto>> LoginAsync(
        [FromBody] TelegramLoginRequestDto requestDto,
        [FromHeader(Name = "DeviceId")] DeviceIdDto deviceIdDto)
    {
        return _telegramAuthDataChecker.CheckAuthorization(requestDto) switch
        {
            Authorization.Valid => await LoginAsync(requestDto, deviceIdDto.DeviceId),
            _ => BadRequest()
        };
    }

    private async Task<ActionResult<TokensDto>> LoginAsync(TelegramLoginRequestDto requestDto, string deviceId)
    {
        var tgId = requestDto.Id!.Value;
        var user = await _userRepository.GetByTelegramIdAsync(tgId);
        if (user is null)
        {
            user = new User
            {
                TelegramId = tgId,
                Username = requestDto.Username,
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
            };
            await _userRepository.CreateAsync(user);
        }

        return await GetTokens(user.Id, deviceId);
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<TokensDto>> RefreshToken([FromBody] RefreshRequestDto refreshRequestDto)
    {
        var deviceId = refreshRequestDto.DeviceId;
        var oldRefreshToken = refreshRequestDto.RefreshToken ?? Request.Cookies["RefreshToken"];
        if (oldRefreshToken is null)
        {
            return Unauthorized();
        }

        var oldRefreshTokenFields = oldRefreshToken.Split(".");
        var oldRefreshTokenValue = oldRefreshTokenFields[0];
        var oldRefreshSessionId = Guid.Parse(oldRefreshTokenFields[1]);

        var session = await _refreshSessionRepository.GetAsync(oldRefreshSessionId);
        if (session is null)
            return Unauthorized();
        
        var isValid = session.RefreshToken == oldRefreshTokenValue
                      && session.DeviceId == deviceId
                      && session.ExpiresIn > DateTime.UtcNow;
        
        if (!isValid) 
            return Unauthorized();

        return await GetTokens(session.UserId, deviceId);
    }

    private async Task<ActionResult<TokensDto>> GetTokens(Guid userId, string deviceId)
    {
        var accessToken = GenerateAccessToken(userId);
        var refreshSession = CreateRefreshSession(userId, deviceId);

        await _refreshSessionRepository.CreateNewAndRemoveOldAsync(refreshSession);
        
        var refreshToken = $"{refreshSession.RefreshToken}.{refreshSession.Id}";
        
        SetCookie(refreshToken, refreshSession.ExpiresIn);

        return Ok(new TokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        });
    }
    
    private void SetCookie(string refreshToken, DateTime expiresIn)
    {
        Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Expires = expiresIn
        });
    }
    
    private string GenerateAccessToken(Guid userId)
    {
        var now = DateTime.UtcNow;
        var secret = _jwtAuthOptions.Secret;
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };
        
        var expires = now.Add(TimeSpan.FromMinutes(_jwtAuthOptions.AccessTokenExpiresInMinutes));

        var jwt = new JwtSecurityToken(
            notBefore: now,
            claims: userClaims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private RefreshSession CreateRefreshSession(Guid userId, string deviceId)
    {
        var now = DateTime.UtcNow;
        var refreshToken = GenerateRefreshToken();
        var expires = now.Add(TimeSpan.FromDays(_jwtAuthOptions.RefreshTokenExpiresInDays));
        return new RefreshSession
        {
            DeviceId = deviceId,
            RefreshToken = refreshToken,
            ExpiresIn = expires,
            UserId = userId
        };
    }

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}