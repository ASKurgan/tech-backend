using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Infrastructure.DbContexts;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.Issues.IntegrationTests.Modules;

public class ModuleTestsBase : IClassFixture<ModuleTestWebFactory>, IAsyncLifetime
{
    protected readonly ModuleTestWebFactory Factory;
    protected readonly IssuesWriteDbContext WriteDbContext;
    protected readonly IReadDbContext ReadDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;

    private readonly Func<Task> _resetDatabase;

    protected ModuleTestsBase(ModuleTestWebFactory factory)
    {
        _resetDatabase = factory.ResetDatabaseAsync;
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<IssuesWriteDbContext>();
        ReadDbContext = Scope.ServiceProvider.GetRequiredService<IReadDbContext>();
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
        
        await WriteDbContext.Modules.AddAsync(module);

        await WriteDbContext.SaveChangesAsync();

        return module.Id;
    }
    
    protected async Task<List<Module>> SeedModules(int count)
    {
        List<Module> modulesToSeed = [];
        
        for (var i = 0; i < count; i++)
        {
            modulesToSeed.Add(Fixture.CreateModule());
        }
        
        await WriteDbContext.Modules.AddRangeAsync(modulesToSeed);

        await WriteDbContext.SaveChangesAsync();

        return modulesToSeed;
    }
    
    protected async Task<Guid> SeedIssuePositions(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var module = await WriteDbContext.Modules
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken);
        if (module is  null)
            throw new Exception($"Seeded Module {moduleId} not found, something wrong with DB");
            
        for (var i = 0; i < 4; i++)
        {
            module.AddIssue(IssueId.NewIssueId()); 
        }
        await WriteDbContext.SaveChangesAsync(cancellationToken);
        return module.IssuesPosition[3].IssueId;
    }    
}