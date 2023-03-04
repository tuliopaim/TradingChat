using TradingChat.Application.Abstractions;

namespace TradingChat.Application.UseCases.SendMessage;

public record SendMessageCommand(
    Guid ChatRoomId,
    string Message) : ICommand;
