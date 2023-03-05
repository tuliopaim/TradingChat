namespace TradingChat.Core.Messaging.Messages;

public record struct ChatCommandMessage(
    string Message,
    Guid ChatRoomId);
