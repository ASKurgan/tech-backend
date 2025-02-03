using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.UpdateUserPhoneNumber;

public record UpdateUserPhoneNumberCommand(Guid UserId, string? PhoneNumber) : ICommand;