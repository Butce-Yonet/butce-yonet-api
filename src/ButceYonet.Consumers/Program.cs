using System.Reflection;
using DotBoil;

var builder = WebApplication.CreateBuilder(args);

var dotboilAssemblies = new List<string>
{
    "DotBoil",
    "DotBoil.Cors",
    "DotBoil.Localization",
    "DotBoil.Parameter",
    "DotBoil.Caching",
    "DotBoil.Logging",
    "DotBoil.Mapper",
    "DotBoil.Validator",
    "ButceYonet.Application",
    "DotBoil.EFCore",
    "DotBoil.MassTransit"
}.Select(assemblyName => Assembly.Load(assemblyName)).ToArray();

await builder.AddDotBoil(dotboilAssemblies);

var app = builder.Build();

await app.UseDotBoil(dotboilAssemblies);

app.UseHttpsRedirection();

app.Run();