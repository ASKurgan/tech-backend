using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddIssueToLesson;

public class AddIssueToLessonHandler(
    IReadDbContext readDbContext,
    ILessonsRepository lessonsRepository,
    [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
    ILogger<AddIssueToLessonHandler> logger) : ICommandHandler<AddIssueToLessonCommand>
{
    public async Task<UnitResult<ErrorList>> Handle(
        AddIssueToLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();


        var isIssueExists
            = await readDbContext.Issues.FirstOrDefaultAsync(i => i.Id == command.IssueId, cancellationToken);
        if (isIssueExists is null)
            return Errors.General.NotFound(command.IssueId, "issue").ToErrorList();
        
        var result = lesson.Value.AddIssue(command.IssueId);
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await unitOfWork.SaveChanges(cancellationToken);

        logger.Log(LogLevel.Information, "Added new issue with {IssueId} to {LessonId}", command.IssueId,
            command.LessonId);

        return UnitResult.Success<ErrorList>();
    }
}
