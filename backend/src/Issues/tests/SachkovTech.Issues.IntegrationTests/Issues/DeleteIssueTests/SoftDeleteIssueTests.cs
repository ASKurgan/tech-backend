using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Issues.Application.Features.Issue.Commands.DeleteIssue.SoftDeleteIssue;

namespace SachkovTech.Issues.IntegrationTests.Issues.DeleteIssueTests;

public class SoftDeleteIssueTests : IssueTestsBase
{
    private readonly SoftDeleteIssueHandler _sut;

    public SoftDeleteIssueTests(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<SoftDeleteIssueHandler>();
    }

    [Fact]
    public async Task Soft_delete_issue_successfully()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var issueId = await SeedIssue();

        var command = Fixture.CreateDeleteIssueCommand(issueId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var issue = await ReadDbContext.Issues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        issue.Should().NotBeNull();
        issue?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteIssue_when_issue_already_deleted_should_be_failure()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var issueId = await SeedSoftDeletedIssue();

        var command = Fixture.CreateDeleteIssueCommand(issueId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        //assert
        result.IsSuccess.Should().Be(false);

        var issue = await ReadDbContext.Issues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == issueId, cancellationToken);;

        issue.Should().NotBeNull();
        issue?.IsDeleted.Should().BeTrue();
    }
}