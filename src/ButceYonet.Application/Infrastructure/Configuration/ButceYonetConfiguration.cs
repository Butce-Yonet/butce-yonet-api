using DotBoil.Configuration;

namespace ButceYonet.Application.Infrastructure.Configuration;

public class ButceYonetConfiguration : IOptions
{
    public string Key => "ButceYonet";

    public string MainConnectionString { get; set; }
}