using TradingChat.Domain.Shared;

namespace TradingChat.Domain.Entities;

public class ChatRoom
{
    private ChatRoom()
    {
    }

    public ChatRoom(
        string name,
        int maxNumberOfUsers,
        Guid ownerId)
    {
        Name = name;
        MaxNumberOfUsers = maxNumberOfUsers;
        OwnerId = ownerId;

        AddUser(OwnerId);
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public int MaxNumberOfUsers { get; private set; }

    public Guid OwnerId { get; private set; }
    public virtual ChatUser? Owner { get; private set; }

    private readonly List<ChatRoomUser> _users = new();
    public virtual IReadOnlyList<ChatRoomUser> Users => _users.AsReadOnly();

    public Result AddUser(Guid userId)
    {
        if (ContainsUser(userId))
        {
            return new Error("User already in chat!");
        }

        _users.Add(new ChatRoomUser(Id, userId));

        return Result.Success();
    }

    private bool ContainsUser(Guid userId)
    {
        return _users.Any(u => u.ChatUserId == userId);
    }
}
