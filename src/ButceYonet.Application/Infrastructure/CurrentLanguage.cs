using DotBoil.Localization;
using Microsoft.AspNetCore.Http;

namespace ButceYonet.Application.Infrastructure;

public class CurrentLanguage : ICurrentLanguage
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentLanguage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string Language { get; }
}