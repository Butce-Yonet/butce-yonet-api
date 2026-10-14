using DotBoil.Configuration;

namespace ButceYonet.Application.Infrastructure.Configuration;

public class ButceYonetConfiguration : IOptions
{
    public string Key => "ButceYonet";

    public string MainConnectionString { get; set; }
    public string AuthorizationConnectionString { get; set; }
    public List<string> Jobs { get; set; } = new();
}