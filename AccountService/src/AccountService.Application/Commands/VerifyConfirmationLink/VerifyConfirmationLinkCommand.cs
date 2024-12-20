using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.VerifyConfirmationLink;

public record VerifyConfirmationLinkCommand(
    Guid UserId,
    string Code) : ICommand;