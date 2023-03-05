using RabbitMQ.Client.Events;
using TradingChat.ChatCommandBot.Commands.ChatMessageCommands;
using TradingChat.Core.Messaging;
using TradingChat.Core.Messaging.Messages;

namespace TradingChat.ChatCommandBot;

public class MessageCommandConsumer : RabbitMqConsumerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageCommandConsumer> _logger;

    public MessageCommandConsumer(
        RabbitMqConnection rabbitConnection,
        IServiceProvider serviceProvider,
        ILogger<MessageCommandConsumer> logger) 
        : base(rabbitConnection, logger, RabbitRoutingKeys.ChatCommand)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task<bool> HandleMessage(BasicDeliverEventArgs rabbitEventArgs)
    {
        var chatCommandMessage = rabbitEventArgs.GetDeserializedMessage<ChatCommandMessage>();

        using var scope = _serviceProvider.CreateScope();
        var commandInvoker = scope.ServiceProvider.GetRequiredService<ChatMessageCommandInvoker>();

        var result = await commandInvoker
            .ExecuteCommandAsync(chatCommandMessage.Message, chatCommandMessage.ChatRoomId);

        return true;
    }
}
