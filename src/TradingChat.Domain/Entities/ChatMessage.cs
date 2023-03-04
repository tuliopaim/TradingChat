namespace TradingChat.Domain.Entities;

public class ChatMessage
{
    private ChatMessage()
    {
    }

    public ChatMessage(
        string message,
        Guid chatUserId,
        Guid chatRoomId)
    {
        Message = message;
        ChatUserId = chatUserId;
        ChatRoomId = chatRoomId;
        SentAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; private set; } = "";
    public DateTimeOffset SentAt { get; private set; }

    public Guid ChatUserId { get; private set; }
    public ChatUser? ChatUser { get; private set; }
    
    public Guid ChatRoomId { get; private set; }
    public ChatRoom? ChatRoom { get; private set; }
}
