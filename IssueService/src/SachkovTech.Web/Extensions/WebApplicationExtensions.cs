using SachkovTech.Core.Database;
using SachkovTech.Framework.Middlewares;
using Serilog;

namespace SachkovTech.Web.Extensions;

public static class WebApplicationExtensions
{
    public static async Task Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
        {
            await app.Services.RunMigrations();
            await app.Services.RunAutoSeeding();

            app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }

        app.UseExceptionMiddleware();
        app.UseSerilogRequestLogging();
        app.ConfigureCors();
        app.UseAuthentication();
        app.UseScopeDataMiddleware();
        app.UseAuthorization();
        app.MapControllers();

        if (app.Environment.IsEnvironment("Docker"))
        {
            app.MapGet("/", () => "Hello World!");
        }
    }

    private static void ConfigureCors(this WebApplication app)
    {
        app.UseCors(config =>
        {
            config.WithOrigins("http://localhost:5173", "http://localhost:5097")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
}