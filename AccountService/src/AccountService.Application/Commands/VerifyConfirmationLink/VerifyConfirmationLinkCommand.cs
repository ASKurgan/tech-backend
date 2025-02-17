using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.VerifyConfirmationLink;

public record VerifyConfirmationLinkCommand(
    Guid UserId,
    string Code) : ICommand;