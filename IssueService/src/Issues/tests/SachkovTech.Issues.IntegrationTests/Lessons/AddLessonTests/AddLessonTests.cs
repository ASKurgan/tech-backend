using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;

namespace SachkovTech.Issues.IntegrationTests.Lessons.AddLessonTests;

public class AddLessonTests : LessonsTestsBase
{
    public AddLessonTests(LessonTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Add_lesson_to_database()
    {
        // arrange
        Factory.SetupSuccessFileServiceMock();

        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var command = Fixture.CreateAddLessonCommand(moduleId);

        var sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddLessonCommand>>();

        // act
        var result = await sut.Handle(command, cancellationToken);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var lesson = await ReadDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == result.Value, cancellationToken);

        lesson.Should().NotBeNull();
        lesson?.ModuleId.Should().Be(moduleId);
    }

    [Fact]
    public async Task Cant_add_lesson_to_database()
    {
        // arrange
        Factory.SetupFailureFileServiceMock();

        var cancellationToken = new CancellationTokenSource().Token;

        var moduleId = await SeedModule();

        var command = Fixture.CreateAddLessonCommand(moduleId);

        var sut = Scope.ServiceProvider.GetRequiredService<AddLessonHandler>();

        // act
        var result = await sut.Handle(command, cancellationToken);

        //assert
        var lesson = await ReadDbContext.Lessons
            .FirstOrDefaultAsync(cancellationToken);

        result.IsFailure.Should().BeTrue();
        lesson.Should().BeNull();
    }
}