using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Issue.Commands.UpdateIssueMainInfo;

namespace SachkovTech.Issues.IntegrationTests.Issues.UpdateIssueTests;

public class UpdateIssueTests : IssueTestsBase
{
    private readonly ICommandHandler<Guid, UpdateIssueMainInfoCommand> sut;

    public UpdateIssueTests(IntegrationTestsWebFactory factory) : base(factory)
    {
        sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateIssueMainInfoCommand>>();
    }

    [Fact]
    public async Task Update_issue_successfully()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var oldModuleId = await SeedModule();

        var newModuleId = await SeedModule();

        var oldLessonId = await SeedLesson(oldModuleId);

        var newLessonId = await SeedLesson(newModuleId);

        var issueId = await SeedIssue(oldModuleId, oldLessonId);

        var command = Fixture.CreateUpdateIssueMainInfoCommand(issueId, newLessonId, newModuleId);

        // act
        var result = await sut.Handle(command, cancellationToken);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var issue = await ReadDbContext.Issues
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        issue.Should().NotBeNull();
        issue?.ModuleId.Should().Be(newModuleId);
        issue?.LessonId.Should().Be(newLessonId);
    }
}