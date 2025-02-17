using AccountService.Api;
using AccountService.Api.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

await app.Configure();

app.Run();

namespace AccountService.Api
{
    public partial class Program;
}