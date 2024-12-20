using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.EnrollParticipant;

public record EnrollParticipantCommand(string Email) : ICommand;