using TradingChat.Domain.Entities;

namespace TradingChat.Domain.Contracts;

public interface IChatRoomRepository : IBaseRepository<ChatRoom>
{
    Task<ChatRoom?> GetChatRoomToSendMessage(Guid chatRoomId, Guid userId, CancellationToken cancellationToken);
    Task<ChatRoom?> GetWithUsersAsTracking(Guid chatRoomId);
    Task<bool> NameAlreadyTaken(string name);
}
