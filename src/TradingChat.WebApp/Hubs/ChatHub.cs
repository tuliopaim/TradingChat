using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TradingChat.Application.Hubs;
using TradingChat.Application.UseCases.SendMessage;

namespace TradingChat.WebApp.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task JoinChat(Guid chatId)
    {
        await Groups
            .AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task SendMessage(SendMessageCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return;
        }

        var message = result.Value;

        await Clients
            .Group(command.ChatRoomId.ToString())
            .ReceiveMessage(message);
    }
}
