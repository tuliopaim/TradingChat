using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommand : ICommand<ChatMessageInfoDto>
{
    public Guid ChatRoomId { get; set; }

    public string Message { get; set; } = "";
}

