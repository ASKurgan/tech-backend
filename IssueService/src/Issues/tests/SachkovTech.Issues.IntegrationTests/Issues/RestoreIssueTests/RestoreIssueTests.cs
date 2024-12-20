using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Issues.Application.Features.Issue.Commands.RestoreIssue;

namespace SachkovTech.Issues.IntegrationTests.Issues.RestoreIssueTests;

public class RestoreIssueTests : IssueTestsBase
{
    private readonly RestoreIssueHandler sut;

    public RestoreIssueTests(IntegrationTestsWebFactory factory) : base(factory)
    {
        sut = Scope.ServiceProvider.GetRequiredService<RestoreIssueHandler>();
    }

    [Fact]
    public async Task Restore_issue_successfully()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var issueId = await SeedSoftDeletedIssue();

        var command = Fixture.CreateRestoreIssueCommand(issueId);

        // act
        var result = await sut.Handle(command, cancellationToken);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var issue = await ReadDbContext.Issues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        issue.Should().NotBeNull();
        issue?.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task Restore_issue_successfully_when_issue_is_not_soft_deleted()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var issueId = await SeedIssue();

        var command = Fixture.CreateRestoreIssueCommand(issueId);

        var sut = Scope.ServiceProvider.GetRequiredService<RestoreIssueHandler>();

        // act
        var result = await sut.Handle(command, cancellationToken);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var issue = await ReadDbContext.Issues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        issue.Should().NotBeNull();
        issue?.IsDeleted.Should().BeFalse();
    }
}