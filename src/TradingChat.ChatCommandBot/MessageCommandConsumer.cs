using TradingChat.ChatCommandBot.Commands;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;

namespace TradingChat.ChatCommandBot;

public class MessageCommandConsumer : QueueConsumerBackgroundService<ChatCommandMessage>
{
    private readonly IServiceProvider _serviceProvider;

    public MessageCommandConsumer(
        ILogger<MessageCommandConsumer> logger,
        IServiceProvider serviceProvider,
        IQueueConsumer consumer) : base(logger, consumer, QueueNames.ChatCommand)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<bool> HandleMessage(ChatCommandMessage message)
    {
        using var scope = _serviceProvider.CreateScope();
        var commandInvoker = scope.ServiceProvider.GetRequiredService<ChatMessageCommandInvoker>();

        var result = await commandInvoker.ExecuteCommandAsync(message.Message, message.ChatRoomId);

        return true;
    }
}
