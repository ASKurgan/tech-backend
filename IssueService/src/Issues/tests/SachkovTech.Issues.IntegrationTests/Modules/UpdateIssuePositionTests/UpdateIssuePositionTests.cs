using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Modules.Commands.UpdateIssuePosition;

namespace SachkovTech.Issues.IntegrationTests.Modules.UpdateIssuePositionTests;

public class UpdateIssuePositionTests: ModuleTestsBase
{
    private readonly ICommandHandler<Guid, UpdateIssuePositionCommand> _sut;
    public UpdateIssuePositionTests(ModuleTestWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateIssuePositionCommand>>();
    }
    [Fact]
    public async Task UpdateIssuePosition_should_move_forth_to_second_position()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();
        //seed 5 IssuePositions
        var issueId = await SeedIssuePositions(moduleId, cancellationToken); // return 4th issuePosition
        
        var command = Fixture.CreateUpdateIssuePositionCommand(moduleId, issueId, 2);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var module = await ReadDbContext.Modules
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken);

        module?.IssuesPosition.Should().NotBeNull();
        module?.IssuesPosition.Where(x => x.IssueId == issueId).Select(x => x.Position)
            .Should().Equal(2);
    }

    
}