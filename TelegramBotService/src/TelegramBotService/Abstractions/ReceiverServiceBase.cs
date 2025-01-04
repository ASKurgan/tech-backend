using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegramBotService.Abstractions;

public abstract class ReceiverServiceBase<TUpdateHandler> : IReceiverService
    where TUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger<ReceiverServiceBase<TUpdateHandler>> _logger;

    public ReceiverServiceBase(
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler,
        ILogger<ReceiverServiceBase<TUpdateHandler>> logger)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
    }
    
    public async Task Receive(CancellationToken cancellationToken = default)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = [],
            DropPendingUpdates = true
        };

        var me = await _botClient.GetMeAsync(cancellationToken);
        
        _logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "My Awesome Bot");

        await _botClient.ReceiveAsync(_updateHandler, receiverOptions, cancellationToken);
    }
}