using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Issue.Commands.AddIssue;
using YamlDotNet.Core.Tokens;

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
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var lessonId = await SeedLesson(moduleId);

        var command = Fixture.CreateAddIssueCommand(moduleId, lessonId);

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var issue = await ReadDbContext.ReadIssues
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        issue.Should().NotBeNull();
        issue?.ModuleId.Value.Should().Be(moduleId);

        var module = await ReadDbContext.ReadModules
            .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);

        module!.IssuesPosition.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Cant_add_issue_to_database_due_to_not_existing_lesson()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var lessonId = Guid.Empty;

        var command = Fixture.CreateAddIssueCommand(moduleId, lessonId);

        var sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateIssueCommand>>();

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        var issue = await ReadDbContext.ReadIssues
            .FirstOrDefaultAsync(cancellationToken);

        result.IsFailure.Should().BeTrue();
        issue.Should().BeNull();
    }
}