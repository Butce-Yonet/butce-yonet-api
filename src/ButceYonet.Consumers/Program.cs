using System.Reflection;
using DotBoil;

var builder = WebApplication.CreateBuilder(args);

PreloadAssemblies("DotBoil");
PreloadAssemblies("ButceYonet");
await builder.AddDotBoil();

var app = builder.Build();

await app.UseDotBoil();

app.UseHttpsRedirection();

app.Run();

static void PreloadAssemblies(string prefix)
{
    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
    
    foreach (var dll in Directory.GetFiles(baseDir, $"{prefix}.*.dll"))
    {
        var name = AssemblyName.GetAssemblyName(dll);
        
        if (AppDomain.CurrentDomain.GetAssemblies().All(a => a.FullName != name.FullName))
            Assembly.Load(name);
    }
}