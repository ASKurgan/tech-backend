using Quartz;

namespace SachkovTech.Issues.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly ProcessOutboxMessagesService _processOutboxMessagesService;

    public ProcessOutboxMessagesJob(ProcessOutboxMessagesService processOutboxMessagesService)
    {
        _processOutboxMessagesService = processOutboxMessagesService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _processOutboxMessagesService.Execute(context.CancellationToken);
    }
}