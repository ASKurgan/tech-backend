using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Infrastructure.DbContexts;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Infrastructure.Repositories;

public class LessonsRepository : ILessonsRepository
{
    private readonly IssuesWriteDbContext _dbContext;

    public LessonsRepository(IssuesWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Add(Lesson lesson, CancellationToken cancellationToken = default)
    {
        await _dbContext.Lessons.AddAsync(lesson, cancellationToken);
        return lesson.Id;
    }

    public Guid Save(Lesson module)
    {
        _dbContext.Lessons.Attach(module);
        return module.Id.Value;
    }

    public Guid Delete(Lesson module)
    {
        _dbContext.Lessons.Remove(module);

        return module.Id;
    }

    public async Task<Result<Lesson, Error>> GetById(
        LessonId lessonId, CancellationToken cancellationToken = default)
    {
        var module = await _dbContext.Lessons
            .FirstOrDefaultAsync(m => m.Id == lessonId, cancellationToken);

        if (module is null)
            return Errors.General.NotFound(lessonId);

        return module;
    }

    public async Task<Result<Lesson, Error>> GetByTitle(
        Title title, CancellationToken cancellationToken = default)
    {
        var module = await _dbContext.Lessons
            .FirstOrDefaultAsync(m => m.Title == title, cancellationToken);

        if (module is null)
            return Errors.General.NotFound();

        return module;
    }
}
