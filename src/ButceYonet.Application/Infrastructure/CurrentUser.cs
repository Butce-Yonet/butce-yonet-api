using System.IdentityModel.Tokens.Jwt;
using DotBoil.EFCore;
using Microsoft.AspNetCore.Http;

namespace ButceYonet.Application.Infrastructure;

public class CurrentUser : IAuditUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<string> GetModifierName()
    {
        return _httpContextAccessor.HttpContext?.User.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.PreferredUsername)?.Value ?? string.Empty;
    }
}