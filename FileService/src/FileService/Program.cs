using FileService;
using MassTransit;
using SachkovTech.Framework.Endpoints;
using SachkovTech.Framework.Middlewares;
using Serilog;

const string dockerEnv = "Docker";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddProgramDependencies(builder.Configuration);

builder.Services.AddMassTransit(configure =>
{
    configure.SetKebabCaseEndpointNameFormatter();

    // configure.AddConsumer<VideoProcessConsumer>();

    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMQ:Host"]!), h =>
        {
            h.Username(builder.Configuration["RabbitMQ:UserName"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment(dockerEnv))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(config =>
{
    config.WithOrigins("http://localhost:5173")
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.UseAuthentication();
app.UseScopeDataMiddleware();
app.UseAuthorization();
app.MapEndpoints();

app.Run();

namespace FileService
{
    public partial class Program;
}