using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ProjectTemplate;
using ProjectTemplate.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

await app.Configure();

app.Run();

namespace ProjectTemplate
{
    public partial class Program;
}