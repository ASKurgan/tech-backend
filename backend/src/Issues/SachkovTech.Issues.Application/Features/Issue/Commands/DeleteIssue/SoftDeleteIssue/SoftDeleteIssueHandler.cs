using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.SharedKernel;

namespace SachkovTech.Issues.Application.Features.Issue.Commands.DeleteIssue.SoftDeleteIssue;
public class SoftDeleteIssueHandler : ICommandHandler<Guid, DeleteIssueCommand>
{
    private readonly IIssuesRepository _issuesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteIssueCommand> _validator;
    private readonly ILogger<SoftDeleteIssueHandler> _logger;

    public SoftDeleteIssueHandler(
        IIssuesRepository issuesRepository,
        [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
        IValidator<DeleteIssueCommand> validator,
        ILogger<SoftDeleteIssueHandler> logger)
    {
        _issuesRepository = issuesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteIssueCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
        {
            return validationResult.ToList();
        }

        var issueResult = await _issuesRepository.GetById(command.IssueId, false, cancellationToken);
        if (issueResult.IsFailure)
            return issueResult.Error.ToErrorList();

        issueResult.Value.SoftDelete();

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "Issue {issueId} was SOFT deleted",
            command.IssueId);

        return issueResult.Value.Id.Value;
    }
}