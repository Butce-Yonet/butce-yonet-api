using System.Reflection;
using System.Text;
using ButceYonet.Application.Infrastructure.Configuration;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Health;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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
    "DotBoil.MassTransit",
    "DotBoil.Swag",
    "DotBoil.Health"
}.Select(assemblyName => Assembly.Load(assemblyName)).ToArray();

await builder.AddDotBoil(dotboilAssemblies);

var jwtOptions = DotBoilApp.Configuration.GetConfigurations<JwtOptions>();
        
DotBoilApp
    .Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateActor = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        };
    });
        
DotBoilApp
    .Services
    .AddAuthorization();

var app = builder.Build();

await app.UseDotBoil(dotboilAssemblies);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();