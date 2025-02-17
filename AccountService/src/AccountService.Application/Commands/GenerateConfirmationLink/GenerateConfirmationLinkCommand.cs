using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.GenerateConfirmationLink;

public record GenerateConfirmationLinkCommand(
    Guid UserId) : ICommand;