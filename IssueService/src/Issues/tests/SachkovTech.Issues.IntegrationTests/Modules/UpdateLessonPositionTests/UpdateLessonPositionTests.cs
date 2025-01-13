using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Modules.Commands.UpdateLessonPosition;

namespace SachkovTech.Issues.IntegrationTests.Modules.UpdateLessonPositionTests;

public class UpdateLessonPositionTests : ModuleTestsBase
{
    private readonly ICommandHandler<Guid, UpdateLessonPositionCommand> _sut;

    public UpdateLessonPositionTests(ModuleTestWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateLessonPositionCommand>>();
    }

    [Fact]
    public async Task UpdateLessonPosition_should_move_forth_to_second_position()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        // Seed 5 IssuePositions
        var lessonId = await SeedLessonPositions(moduleId, cancellationToken); // return 4th issuePosition

        var command = Fixture.CreateUpdateLessonPositionCommand(moduleId, lessonId, 2);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var module = await ReadDbContext.Modules
            .FirstOrDefaultAsync(x => x.Id == moduleId, cancellationToken);

        module?.LessonsPosition.Should().NotBeNull();
        module?.LessonsPosition.Where(x => x.LessonId == lessonId).Select(x => x.Position)
            .Should().Equal(2);
    }
}