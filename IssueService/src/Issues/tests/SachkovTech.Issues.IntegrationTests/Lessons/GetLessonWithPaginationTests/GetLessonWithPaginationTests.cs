using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsWithPagination;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.Issue.ValueObjects;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Infrastructure.DbContexts;
using SharedKernel;

namespace SachkovTech.Issues.IntegrationTests.Lessons.GetLessonWithPaginationTests;

public class GetLessonWithPaginationTests : LessonsTestsBase
{
    public GetLessonWithPaginationTests(LessonTestWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandlerWithResult<PagedList<LessonResponse>, GetLessonsWithPaginationQuery>>();
    }

    private IQueryHandlerWithResult<PagedList<LessonResponse>, GetLessonsWithPaginationQuery> _sut;

    [Fact]
    public async Task Get_lessons_with_pagination()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        const int countLessons = 5;
        var lessons = await SeedLessonsToDatabase(WriteDbContext, countLessons, cancellationToken);

        var lessonIds = lessons.Select(l => l.Video.FileId);
        Factory.SetupSuccessFileServiceMock(lessonIds);

        const int page = 1;
        var query = Fixture.CreateGetLessonsWithPaginationQuery(page, countLessons);

        // Act
        var result = await _sut.Handle(query, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var pagedList = result.Value;
        pagedList.Should().NotBeNull();
        pagedList.Items.Should().HaveCount(countLessons);
        pagedList.TotalCount.Should().Be(countLessons);
        pagedList.Items.First().VideoUrl.Should().Be("testUrl");
    }

    [Fact]
    public async Task Cant_get_lessons_with_invalid_query()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        int invalidPage = -1;
        int invalidPageSize = -1;
        var invalidQuery = new GetLessonsWithPaginationQuery(invalidPage, invalidPageSize);

        SetupFailureValidationResult(invalidQuery, cancellationToken);

        // Act
        var result = await _sut.Handle(invalidQuery, cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Should().ContainSingle(e => e.Message == "Page недействительно");
        result.Error.Should().ContainSingle(e => e.Message == "PageSize недействительно");
    }

    private void SetupFailureValidationResult(
        GetLessonsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        var validatorMock = Substitute.For<IValidator<GetLessonsWithPaginationQuery>>();

        var validationFailures = new List<ValidationFailure>
        {
            new(nameof(query.Page), Errors.General.ValueIsInvalid("Page").Serialize()),
            new(nameof(query.PageSize), Errors.General.ValueIsInvalid("PageSize").Serialize()),
        };
        var validationResult = new ValidationResult(validationFailures);
        validatorMock
            .ValidateAsync(query, cancellationToken)
            .Returns(validationResult);
    }

    private async Task<List<Lesson>> SeedLessonsToDatabase(
        IssuesWriteDbContext dbContext,
        int count,
        CancellationToken cancellationToken = default)
    {
        var lessons = Enumerable.Range(0, count).Select(_ => new Lesson(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Title.Create("test title").Value,
            Description.Create("test description").Value,
            Experience.Create(1).Value,
            [Guid.NewGuid()],
            [Guid.NewGuid()])).ToList();

        await dbContext.Lessons.AddRangeAsync(lessons, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return lessons;
    }
}