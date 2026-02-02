using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ButceYonet.Application.Infrastructure.Configuration;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Entities;
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
    "DotBoil.EFCore",
    "DotBoil.MassTransit",
    "DotBoil.TemplateEngine",
    "DotBoil.Swag",
    "DotBoil.Health",
    "DotBoil.Email",
    "ButceYonet.Application"
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
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
        };
        options.Events = new JwtBearerEvents()
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = BaseResponse.Response(
                    new { },
                    HttpStatusCode.Unauthorized);

                return context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = BaseResponse.Response(
                    new { },
                    HttpStatusCode.Forbidden);

                return context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();