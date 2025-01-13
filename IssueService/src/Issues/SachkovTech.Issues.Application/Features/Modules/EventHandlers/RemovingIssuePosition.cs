using MediatR;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue.Events;

namespace SachkovTech.Issues.Application.Features.Modules.EventHandlers;

public class RemovingIssuePosition : INotificationHandler<IssueDeletedEvent>
{
    private readonly IModulesRepository _repository;

    public RemovingIssuePosition(IModulesRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(IssueDeletedEvent notification, CancellationToken cancellationToken = default)
    {
        var moduleIssuesResult = await _repository.GetById(notification.ModuleId, cancellationToken);
        if (moduleIssuesResult.IsFailure)
            throw new Exception(moduleIssuesResult.Error.Message);

        moduleIssuesResult.Value.DeleteIssuePosition(notification.IssueId);
    }
}