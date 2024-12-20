using ProjectTemplate.Domain;

namespace ProjectTemplate.Application.Managers;

public interface IAccountsManager
{
    Task CreateParticipantAccount(
        ParticipantAccount participantAccount,
        CancellationToken cancellationToken = default);

    Task CreateStudentAccount(
        StudentAccount studentAccount,
        CancellationToken cancellationToken = default);
}