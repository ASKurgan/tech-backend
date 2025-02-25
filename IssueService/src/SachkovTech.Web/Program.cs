using SachkovTech.Web;
using SachkovTech.Web.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

await app.Configure();

app.Run();

namespace SachkovTech.Web
{
    public partial class Program;
}