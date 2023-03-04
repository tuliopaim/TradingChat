namespace TradingChat.Domain.Entities;

public class ChatRoomUser
{
    private ChatRoomUser()
    {
    }

    public ChatRoomUser(Guid chatRoomId, Guid chatUserId)
    {
        ChatRoomId = chatRoomId;
        ChatUserId = chatUserId;

        JoinedAt = DateTimeOffset.UtcNow;
    }

    public Guid ChatRoomId { get; private set; }
    public Guid ChatUserId { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }

    public virtual ChatRoom? ChatRoom { get; private set; }
    public virtual ChatUser? ChatUser { get; private set; }
}
