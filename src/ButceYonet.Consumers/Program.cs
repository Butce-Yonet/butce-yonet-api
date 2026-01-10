using System.Reflection;
using ButceYonet.Application.Infrastructure.MailTemplates;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Email;
using DotBoil.Email.Configuration;
using DotBoil.Email.Models;
using DotBoil.TemplateEngine;

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
    "DotBoil.MassTransit",
    "DotBoil.TemplateEngine",
    "DotBoil.Email",
    "ButceYonet.Application",
    "DotBoil.EFCore",
}.Select(assemblyName => Assembly.Load(assemblyName)).ToArray();

await builder.AddDotBoil(dotboilAssemblies);

var app = builder.Build();

await app.UseDotBoil(dotboilAssemblies);

app.UseHttpsRedirection();

app.Run();