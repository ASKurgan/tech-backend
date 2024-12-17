using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Issue.Commands.RestoreIssue;

public class RestoreIssueHandler : ICommandHandler<Guid, RestoreIssueCommand>
{
    private readonly IIssuesRepository _issuesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreIssueHandler> _logger;

    public RestoreIssueHandler(
        IIssuesRepository issuesRepository,
        [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
        ILogger<RestoreIssueHandler> logger)
    {
        _issuesRepository = issuesRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        RestoreIssueCommand command,
        CancellationToken cancellationToken = default)
    {
        var restoreResult = await _issuesRepository
            .GetById(command.IssueId, true, cancellationToken);
        
        if (restoreResult.IsFailure)
            return restoreResult.Error.ToErrorList();
        
        restoreResult.Value.Restore();
        
        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "Issue {issueId} was restored",
            command.IssueId);

        return command.IssueId;
    }
}