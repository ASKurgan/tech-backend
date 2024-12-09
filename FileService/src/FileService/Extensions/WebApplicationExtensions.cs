using FileService.Jobs;

namespace FileService.Extensions;

public static class WebApplicationExtensions
{
    public static void CreateHangfireDatabaseIfNotExists(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<HangfireContext>();
        context.Database.EnsureCreated();
    }
}