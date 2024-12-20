using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ProjectTemplate;
using ProjectTemplate.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

await app.Configure();

app.MapGet("/jwk", async () =>
{
    var rsaKey = RSA.Create();
    rsaKey.ImportRSAPrivateKey(await File.ReadAllBytesAsync("etc/key"), out _);

    var publicKey = RSA.Create();
    publicKey.ImportRSAPublicKey(rsaKey.ExportRSAPublicKey(), out _);
    var key = new RsaSecurityKey(publicKey);

    return JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
});

app.MapGet("/jwk-private", async () =>
{
    var rsaKey = RSA.Create();
    rsaKey.ImportRSAPrivateKey(await File.ReadAllBytesAsync("etc/key"), out _);
    var key = new RsaSecurityKey(rsaKey);

    return JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
});

app.Run();

namespace ProjectTemplate
{
    public partial class Program;
}