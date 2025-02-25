using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.RemoveIssueFromLesson;

public class RemoveIssueFromLessonHandler : ICommandHandler<RemoveIssueFromLessonCommand>
{
    private readonly IIssuesReadDbContext _readDbContext;
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveIssueFromLessonHandler> _logger;

    public RemoveIssueFromLessonHandler(
        IIssuesReadDbContext readDbContext,
        ILessonsRepository lessonsRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveIssueFromLessonHandler> logger)
    {
        _readDbContext = readDbContext;
        _lessonsRepository = lessonsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        RemoveIssueFromLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();

        var isIssueExists
            = await _readDbContext.ReadIssues.FirstOrDefaultAsync(i => i.Id == command.IssueId, cancellationToken);
        if (isIssueExists is null)
            return Errors.General.NotFound(command.IssueId, "issue").ToErrorList();

        var result = lesson.Value.RemoveIssue(command.IssueId);
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.Log(LogLevel.Information, "Remove issue with {IssueId} from {LessonId}", command.IssueId, command.LessonId);

        return UnitResult.Success<ErrorList>();
    }
}