namespace SachkovTech.Issues.Application.Interfaces;

public interface IOutboxRepository
{
    Task Add<T>(T message, CancellationToken cancellationToken);
}