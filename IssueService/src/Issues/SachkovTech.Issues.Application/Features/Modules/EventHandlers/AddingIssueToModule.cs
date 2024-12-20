using MediatR;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue.Events;

namespace SachkovTech.Issues.Application.Features.Modules.EventHandlers;

public class AddingIssueToModule : INotificationHandler<IssueCreatedEvent>
{
    private readonly IModulesRepository _modulesRepository;

    public AddingIssueToModule(IModulesRepository modulesRepository)
    {
        _modulesRepository = modulesRepository;
    }

    public async Task Handle(IssueCreatedEvent notification, CancellationToken cancellationToken)
    {
        var moduleResult = await _modulesRepository.GetById(notification.ModuleId, cancellationToken);

        moduleResult.Value.AddIssue(notification.IssueId);
    }
}