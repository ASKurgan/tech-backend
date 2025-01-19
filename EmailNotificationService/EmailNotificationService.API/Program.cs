using System.Reflection;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using EmailNotificationService.API;
using EmailNotificationService.API.Consumers;
using Serilog.Events;
using Serilog;
using EmailNotificationService.API.Middlewares;
using EmailNotificationService.API.Options;
using EmailNotificationService.API.Services;
using MassTransit;
using EmailNotificationService.API.Consumers.Definitions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var services = builder.Services;
var config = builder.Configuration;

// Add services to the container and configuring options.

services.Configure<MailOptions>(config.GetSection(MailOptions.SECTION_NAME));
services.AddScoped<EmailValidator>();
services.AddScoped<MailSenderService>();
services.AddScoped<HandlebarsTemplateService>();
services.AddScoped<SendEmailService>();
services.AddMemoryCache();

string indexFormat = 
    $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:dd-MM-yyyy}";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Elasticsearch(
        [new Uri("http://localhost:9200")],
        options =>
        {
            options.DataStream = new DataStreamName(indexFormat);
            options.TextFormatting = new EcsTextFormatterConfiguration();
            options.BootstrapMethod = BootstrapMethod.Silent;
        })
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .CreateLogger();

services.AddSerilog();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

builder.Services.AddMassTransit(configure =>
{
    configure.SetKebabCaseEndpointNameFormatter();

    configure.AddConsumer<SendEmailConsumer>(typeof(SendEmailConsumerDefinition));

    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(config["RabbitMQ:Host"]!), h =>
        {
            h.Username(config["RabbitMQ:UserName"]!);
            h.Password(config["RabbitMQ:Password"]!);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionHandler>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    config.AddUserSecrets<Program>(optional: true);
}

app.UseHttpsRedirection();

app.Run();