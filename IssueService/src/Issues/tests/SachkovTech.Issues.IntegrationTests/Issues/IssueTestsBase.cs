using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue;
using SachkovTech.Issues.Domain.Issue.ValueObjects;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SachkovTech.Issues.Infrastructure.DbContexts;

namespace SachkovTech.Issues.IntegrationTests.Issues;

public class IssueTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestsWebFactory Factory;
    protected readonly IssuesDbContext DbContext;
    protected readonly IIssuesReadDbContext ReadDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;

    private readonly Func<Task> _resetDatabase;

    protected IssueTestsBase(IntegrationTestsWebFactory factory)
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
        var module = new Module(
            ModuleId.NewModuleId(),
            Title.Create("title").Value,
            Description.Create("description").Value);

        await DbContext.Modules.AddAsync(module);

        await DbContext.SaveChangesAsync();

        return module.Id;
    }

    protected async Task<Guid> SeedIssue(Guid moduleId = default, Guid lessonId = default)
    {
        var issue = new Issue(
            IssueId.NewIssueId(),
            Title.Create("title").Value,
            Description.Create("description").Value,
            lessonId,
            moduleId,
            Experience.Create(5).Value,
            null);

        await DbContext.Issues.AddAsync(issue);

        var module = await DbContext.Modules.SingleOrDefaultAsync(x => x.Id == moduleId);

        module?.AddIssue(issue.Id);

        await DbContext.SaveChangesAsync();

        return issue.Id;
    }

    protected async Task<Guid> SeedSoftDeletedIssue()
    {
        var issue = new Issue(
            IssueId.NewIssueId(),
            Title.Create("title").Value,
            Description.Create("description").Value,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Experience.Create(5).Value,
            null);

        issue.SoftDelete();

        await DbContext.Issues.AddAsync(issue);

        await DbContext.SaveChangesAsync();

        return issue.Id;
    }

    protected async Task<Guid> SeedLesson(Guid moduleId)
    {
        var lesson = new Lesson(
            LessonId.NewLessonId(),
            moduleId,
            Title.Create("title").Value,
            Description.Create("description").Value,
            Experience.Create(5).Value,
            [],
            []);

        await DbContext.Lessons.AddAsync(lesson);

        await DbContext.SaveChangesAsync();

        return lesson.Id;
    }
}