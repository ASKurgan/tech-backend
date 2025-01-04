using System.Reflection;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using TelegramBotService.Options;
using TelegramBotService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string indexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:dd-MM-yyyy}";

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

builder.Services.AddSerilog();

builder.Services.Configure<TelegramBotOptions>(builder.Configuration.GetSection(TelegramBotOptions.TELEGRAM_BOT));

builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var options = sp.GetService<IOptions<TelegramBotOptions>>()?.Value;
        ArgumentNullException.ThrowIfNull(options);
        TelegramBotClientOptions botOptions = new(options.BotToken);
        return new TelegramBotClient(botOptions, httpClient);
    });

builder.Services.AddScoped<ITelegramService, TelegramService>();

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program
{
}