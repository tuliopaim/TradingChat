using Microsoft.AspNetCore.Identity;

namespace TradingChat.Domain.Entities;

public class ChatUser
{
    private ChatUser()
    {
    }

    public ChatUser(string name, Guid identityUserId)
    {
        Name = name;
        Id = IdentityUserId = identityUserId;
    }

    public Guid Id { get; private set; } 

    public string Name { get; private set; } 

    public Guid IdentityUserId { get; private set; }
    public virtual IdentityUser<Guid>? IdentityUser {  get; private set; }

    private readonly List<ChatRoomUser> _chats = new();
    public virtual IReadOnlyList<ChatRoomUser>? Chats => _chats.AsReadOnly();

    public static ChatUser ChatBotUser()
    {
        return new ChatUser
        {
            Id = new Guid("63424FA2-0161-4DCA-ADD5-FA1386E91B8C"),
            Name = "Bot",
            IdentityUserId = new Guid("63424FA2-0161-4DCA-ADD5-FA1386E91B8C"),
        };
    }
}
