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
        ChatUserId = chatUserId;
        ChatRoomId = chatRoomId;
        SentAt = DateTimeOffset.UtcNow;
        Message = ConvertMessageToHtml(message);
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; private set; } = "";
    public DateTimeOffset SentAt { get; private set; }

    public Guid ChatUserId { get; private set; }
    public ChatUser? ChatUser { get; private set; }
    
    public Guid ChatRoomId { get; private set; }
    public ChatRoom? ChatRoom { get; private set; }

    public bool IsCommand => Message.StartsWith('/');

    public static string ConvertMessageToHtml(string message)
    {
        return message.Replace("\n", "<br>");
    }
}
