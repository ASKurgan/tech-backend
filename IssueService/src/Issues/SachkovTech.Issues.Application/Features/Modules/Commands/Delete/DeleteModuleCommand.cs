using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Modules.Commands.Delete;

public record DeleteModuleCommand(Guid ModuleId) : ICommand;