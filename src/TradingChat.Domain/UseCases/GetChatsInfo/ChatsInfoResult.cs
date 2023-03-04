namespace TradingChat.Domain.UseCases.GetChatsInfo;

public class ChatsInfoResult
{
    public List<ChatInfoModel> Chats { get; set; } = new();
}

public class ChatInfoModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int NumberOfUsers { get; init; }
    public required int MaxNumberOfUsers { get; init; }
    public required string Owner { get; init; }
}
