using MediatR;
using Microsoft.AspNetCore.SignalR;
using TradingChat.Application.Hubs;
using TradingChat.Application.UseCases.SendMessageFromServer;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;
using TradingChat.WebApp.Hubs;

namespace TradingChat.WebApp.Consumers;

public class ChatMessageConsumer : QueueConsumerBackgroundService<NewMessage>
{
    private readonly IHubContext<ChatHub, IChatHub> _chatHubContext;
    private readonly IServiceProvider _serviceProvider;

    public ChatMessageConsumer(
        ILogger<ChatMessageConsumer> logger,
        IQueueConsumer consumer,
        IServiceProvider serviceProvider,
        IHubContext<ChatHub, IChatHub> chatHubContext) : base(logger, consumer, QueueNames.ChatMessage)
    {
        _serviceProvider = serviceProvider;
        _chatHubContext = chatHubContext;
    }

    protected override async Task<bool> HandleMessage(NewMessage message)
    {
        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new SendMessageFromServerCommand
        {
            ChatRoomId = message.ChatRoomId,
            Message = message.Message,
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess) return false;

        await _chatHubContext.Clients
            .Group(message.ChatRoomId.ToString())
            .ReceiveMessage(result.Value);

        return true;
    }
}
