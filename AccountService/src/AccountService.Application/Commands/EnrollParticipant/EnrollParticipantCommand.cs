using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.EnrollParticipant;

public record EnrollParticipantCommand(string Email) : ICommand;