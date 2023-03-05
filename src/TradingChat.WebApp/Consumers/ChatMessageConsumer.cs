using MediatR;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using TradingChat.Application.UseCases.SendMessageFromServer;
using TradingChat.Core.Messaging;
using TradingChat.Core.Messaging.Messages;
using TradingChat.WebApp.Hubs;

namespace TradingChat.WebApp.Consumers;

public class ChatMessageConsumer : RabbitMqConsumerBase
{
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IServiceProvider _serviceProvider;

    public ChatMessageConsumer(
        RabbitMqConnection rabbitConnection,
        IHubContext<ChatHub> chatHubContext,
        IServiceProvider serviceProvider,
        ILogger<RabbitMqConsumerBase> logger)
        : base(rabbitConnection, logger, RabbitRoutingKeys.ChatMessage)
    {
        _chatHubContext = chatHubContext;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<bool> HandleMessage(BasicDeliverEventArgs rabbitEventArgs)
    {
        var newMessage = rabbitEventArgs.GetDeserializedMessage<NewMessage>();
        
        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new SendMessageFromServerCommand
        {
            ChatRoomId = newMessage.ChatRoomId,
            Message = newMessage.Message,
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess) return false;

        await _chatHubContext.Clients
            .Group(newMessage.ChatRoomId.ToString())
            .SendAsync("ReceiveMessage", result.Value);

        return true;
    }
}
