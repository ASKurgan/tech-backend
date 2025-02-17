using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.UpdateUserName;

public record UpdateUserNameCommand(Guid Id, string UserName) : ICommand;