using TradingChat.Domain.Entities;

namespace TradingChat.Domain.Contracts;

public interface IChatRoomRepository : IBaseRepository<ChatRoom>
{
    Task<ChatRoom?> GetWithUsers(Guid chatRoomId);
    Task<bool> NameAlreadyTaken(string name);
}
