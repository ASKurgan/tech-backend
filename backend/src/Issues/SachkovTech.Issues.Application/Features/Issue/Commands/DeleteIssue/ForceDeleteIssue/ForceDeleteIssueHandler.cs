using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue.Events;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Issue.Commands.DeleteIssue.ForceDeleteIssue;

public class ForceDeleteIssueHandler : ICommandHandler<Guid, DeleteIssueCommand>
{
    private readonly IIssuesRepository _issuesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteIssueCommand> _validator;
    private readonly ILogger<ForceDeleteIssueHandler> _logger;
    private readonly IPublisher _publisher;

    public ForceDeleteIssueHandler(
        IIssuesRepository issuesRepository,
        [FromKeyedServices(SharedKernel.Modules.Issues)]
        IUnitOfWork unitOfWork,
        IValidator<DeleteIssueCommand> validator,
        ILogger<ForceDeleteIssueHandler> logger,
        IPublisher publisher)
    {
        _issuesRepository = issuesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _publisher = publisher;
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

        var issueResult = await _issuesRepository.GetById(
            command.IssueId,
            true,
            cancellationToken);

        if (issueResult.IsFailure)
            return Errors.General.NotFound(command.IssueId).ToErrorList();

        var result = _issuesRepository.Delete(issueResult.Value);

        var deletedEvent = new IssueDeletedEvent(issueResult.Value.ModuleId, command.IssueId);

        await _publisher.Publish(deletedEvent, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "Issue {issueId} was FORCE deleted",
            command.IssueId);

        return result;
    }
}