using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBotService.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramService _telegramService;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        ITelegramService telegramService,
        ILogger<UpdateHandler> logger)
    {
        _telegramService = telegramService;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            _ => UnknownUpdateHandler(update)
        });
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    private async Task OnMessage(Message message)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);

        if (string.IsNullOrEmpty(message.Text))
            return;

        await _telegramService.SendTextMessageAsync(message.Chat, $"Сообщение было получено: {message.Text}");
    }

    private Task UnknownUpdateHandler(Update update)
    {
        _logger.LogInformation("Unknown update type {UpdateType}:", update.Type);

        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {exception}", exception);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}