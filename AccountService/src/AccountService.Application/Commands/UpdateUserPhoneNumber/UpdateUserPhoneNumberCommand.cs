using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.UpdateUserPhoneNumber;

public record UpdateUserPhoneNumberCommand(Guid UserId, string? PhoneNumber) : ICommand;