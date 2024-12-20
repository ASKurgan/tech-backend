using System.Text.Json;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Infrastructure.DbContexts;

namespace SachkovTech.Issues.Infrastructure.Outbox;

public class OutboxRepository : IOutboxRepository
{
    private readonly IssuesWriteDbContext _dbContext;
    public OutboxRepository(IssuesWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<T>(T message, CancellationToken cancellationToken)
    {
        var outboxMessages = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.Now,
            Type = typeof(T).FullName!,
            Payload = JsonSerializer.Serialize(message)
        };

        await _dbContext.AddAsync(outboxMessages, cancellationToken);
    }
}