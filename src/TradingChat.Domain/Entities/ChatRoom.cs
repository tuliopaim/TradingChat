using TradingChat.Domain.Shared;

namespace TradingChat.Domain.Entities;

public class ChatRoom
{
    private ChatRoom()
    {
    }

    public ChatRoom(string name)
    {
        Name = name;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; private set; }

    private readonly List<ChatRoomUser> _users = new();
    public IReadOnlyList<ChatRoomUser> Users => _users.AsReadOnly();

    public Result AddUser(ChatUser user)
    {
        if (ContainsUser(user.Id))
        {
            return new Error("User already in chat!");
        }

        _users.Add(new ChatRoomUser(Id, user.Id));

        return Result.Success();
    }

    private bool ContainsUser(Guid userId)
    {
        return _users.Any(u => u.ChatUserId == userId);
    }
}
