using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SachkovTech.Issues.Infrastructure.DbContexts;

namespace SachkovTech.Issues.IntegrationTests.Modules;

public class ModuleTestsBase : IClassFixture<ModuleTestWebFactory>, IAsyncLifetime
{
    protected readonly ModuleTestWebFactory Factory;
    protected readonly IssuesDbContext DbContext;
    protected readonly IIssuesReadDbContext ReadDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;

    private readonly Func<Task> _resetDatabase;

    protected ModuleTestsBase(ModuleTestWebFactory factory)
    {
        _resetDatabase = factory.ResetDatabaseAsync;
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<IssuesDbContext>();
        ReadDbContext = Scope.ServiceProvider.GetRequiredService<IIssuesReadDbContext>();
        Fixture = new Fixture();
        Factory = factory;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _resetDatabase();
        Scope.Dispose();
    }

    protected async Task<Guid> SeedModule()
    {
        var module = Fixture.CreateModule();

        await DbContext.Modules.AddAsync(module);

        await DbContext.SaveChangesAsync();

        return module.Id;
    }

    protected async Task<List<Module>> SeedModules(int count)
    {
        List<Module> modulesToSeed = [];

        for (int i = 0; i < count; i++)
        {
            modulesToSeed.Add(Fixture.CreateModule());
        }

        await DbContext.Modules.AddRangeAsync(modulesToSeed);

        await DbContext.SaveChangesAsync();

        return modulesToSeed;
    }

    protected async Task<Guid> SeedIssuePositions(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var module = await DbContext.Modules
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken);
        if (module is null)
            throw new Exception($"Seeded Module {moduleId} not found, something wrong with DB");

        for (int i = 0; i < 4; i++)
        {
            module.AddIssue(IssueId.NewIssueId());
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return module.IssuesPosition[3].IssueId;
    }

    protected async Task<Guid> SeedLessonPositions(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var module = await DbContext.Modules
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken);
        if (module is null)
            throw new Exception($"Seeded Module {moduleId} not found, something wrong with DB");

        for (int i = 0; i < 4; i++)
        {
            module.AddLesson(LessonId.NewLessonId());
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return module.LessonsPosition[3].LessonId;
    }
}