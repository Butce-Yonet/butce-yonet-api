using System.IdentityModel.Tokens.Jwt;
using ButceYonet.Application.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ButceYonet.Application.Infrastructure.Services;

public class User : IUser
{
    public int Id { get; }

    public User(IHttpContextAccessor httpContextAccessor)
    {
        var sub = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type.EndsWith("nameidentifier")).Value ??
            string.Empty;

        int.TryParse(sub, out int _id);
        Id = _id; 
    }
}