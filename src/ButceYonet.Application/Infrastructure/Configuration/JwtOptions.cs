using DotBoil.Configuration;

namespace ButceYonet.Application.Infrastructure.Configuration;

public class JwtOptions : IOptions
{
    public string Key => "DotBoil:AuthGuard:JwtOptions";

    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationMinutes { get; set; }
}