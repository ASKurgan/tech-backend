using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.UpdatePhoneNumber;

public record UpdatePhoneNumberCommand(Guid UserId, string? PhoneNumber) : ICommand;