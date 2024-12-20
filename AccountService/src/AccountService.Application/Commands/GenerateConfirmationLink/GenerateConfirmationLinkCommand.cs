using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.GenerateConfirmationLink;

public record GenerateConfirmationLinkCommand(
    Guid UserId) : ICommand;