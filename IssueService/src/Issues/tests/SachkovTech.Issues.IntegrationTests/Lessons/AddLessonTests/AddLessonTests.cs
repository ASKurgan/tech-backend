using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;

namespace SachkovTech.Issues.IntegrationTests.Lessons.AddLessonTests;

public class AddLessonTests : LessonsTestsBase
{
    public AddLessonTests(LessonTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Add_lesson_to_database()
    {
        // Arrange
        Factory.SetupSuccessFileServiceMock();

        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var command = Fixture.CreateAddLessonCommand(moduleId);

        var sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateLessonCommand>>();

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var lesson = await ReadDbContext.ReadLessons
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        lesson.Should().NotBeNull();
        lesson?.ModuleId.Should().Be(moduleId);
    }

    [Fact]
    public async Task Cant_add_lesson_to_database()
    {
        // Arrange
        Factory.SetupFailureFileServiceMock();

        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var command = Fixture.CreateAddLessonCommand(moduleId);

        var sut = Scope.ServiceProvider.GetRequiredService<CreateLessonHandler>();

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        var lesson = await ReadDbContext.ReadLessons
            .FirstOrDefaultAsync(cancellationToken);

        result.IsFailure.Should().BeTrue();
        lesson.Should().BeNull();
    }
}