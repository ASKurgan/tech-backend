using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.UpdateUserName;

public record UpdateUserNameCommand(Guid Id, string UserName) : ICommand;