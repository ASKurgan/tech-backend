using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Issue.Commands.AddIssue;

namespace SachkovTech.Issues.IntegrationTests.Issues.AddIssueTests;

public class AddIssueTests : IssueTestsBase
{
    private readonly ICommandHandler<Guid, CreateIssueCommand> sut;

    public AddIssueTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateIssueCommand>>();
    }

    [Fact]
    public async Task Add_issue_to_database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var lessonId = await SeedLesson(moduleId);

        var command = Fixture.CreateAddIssueCommand(moduleId, lessonId);

        // act
        var result = await sut.Handle(command, cancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var issue = await ReadDbContext.Issues
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        issue.Should().NotBeNull();
        issue?.ModuleId.Should().Be(moduleId);

        var module = await ReadDbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);

        module!.IssuesPosition.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Cant_add_issue_to_database_due_to_not_existing_lesson()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var lessonId = Guid.Empty;

        var command = Fixture.CreateAddIssueCommand(moduleId, lessonId);

        var sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateIssueCommand>>();

        // act
        var result = await sut.Handle(command, cancellationToken);

        // assert
        var issue = await ReadDbContext.Issues
            .FirstOrDefaultAsync(cancellationToken);

        result.IsFailure.Should().BeTrue();
        issue.Should().BeNull();
    }
}