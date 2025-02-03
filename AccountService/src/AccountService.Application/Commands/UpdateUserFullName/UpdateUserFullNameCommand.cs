using AccountService.Contracts.Dtos;
using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.UpdateUserFullName;

public record UpdateUserFullNameCommand(Guid UserId, FullNameDto FullName) : ICommand;