using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Issue;
using SachkovTech.Issues.Domain.IssueSolving.Entities;
using SachkovTech.Issues.Domain.IssueSolving.Enums;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.TakeOnWork;

public class TakeOnWorkHandler : ICommandHandler<Guid, TakeOnWorkCommand>
{
    private readonly IUserIssueRepository _userIssueRepository;
    private readonly IReadDbContext _readDbContext;
    private readonly ILogger<TakeOnWorkHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TakeOnWorkHandler(
        IUserIssueRepository userIssueRepository,
        IReadDbContext readDbContext,
        ILogger<TakeOnWorkHandler> logger,
        [FromKeyedServices(SharedKernel.Modules.Issues)]
        IUnitOfWork unitOfWork)
    {
        _userIssueRepository = userIssueRepository;
        _readDbContext = readDbContext;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        TakeOnWorkCommand command,
        CancellationToken cancellationToken = default)
    {
        var issueResult = await GetIssueById(command.IssueId, cancellationToken);

        if (issueResult.IsFailure)
            return issueResult.Error;

        var userIssueExisting =
            await _readDbContext.UserIssues.FirstOrDefaultAsync(ui => ui.IssueId == command.IssueId, cancellationToken);

        if (userIssueExisting is not null)
        {
            return Errors.General.ValueIsInvalid().ToErrorList();
        }

        var previousUserIssue = await _readDbContext.UserIssues
            .FirstOrDefaultAsync(u => u.UserId == command.UserId, cancellationToken);

        var previousUserIssueStatus =
            previousUserIssue is null
            ? IssueStatus.Completed
            : Enum.Parse<IssueStatus>(previousUserIssue.Status);

        if (previousUserIssueStatus != IssueStatus.Completed)
            return Error.Failure("prev.issue.not.solved", "previous issue not solved").ToErrorList();

        var userIssueId = UserIssueId.NewIssueId();
        var userId = UserId.Create(command.UserId);

        var userIssue = new UserIssue(userIssueId, userId, command.IssueId, command.ModuleId);

        var result = await _userIssueRepository.Add(userIssue, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("User took issue on work. A record was created with id {userIssueId}",
            userIssueId);

        return result;
    }

    private async Task<Result<IssueResponse, ErrorList>> GetIssueById(
        Guid issueId,
        CancellationToken cancellationToken = default)
    {
        var issueDto = await _readDbContext.Issues
            .SingleOrDefaultAsync(i => i.Id == issueId, cancellationToken);

        if (issueDto is null)
            return Errors.General.NotFound(issueId).ToErrorList();

        var response = new IssueResponse
        {
            Id = issueDto.Id,
            ModuleId = issueDto.ModuleId,
            Title = issueDto.Title,
            Description = issueDto.Description,
            LessonId = issueDto.LessonId,
        };

        return response;
    }
}