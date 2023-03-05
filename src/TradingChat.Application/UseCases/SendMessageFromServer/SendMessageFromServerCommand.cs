using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;

namespace TradingChat.Application.UseCases.SendMessageFromServer;

public class SendMessageFromServerCommand : ICommand<ChatMessageInfoDto>
{
    public required Guid ChatRoomId { get; set; }

    public required string Message { get; set; } = "";
}
