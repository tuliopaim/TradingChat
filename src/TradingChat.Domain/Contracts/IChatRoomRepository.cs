using TradingChat.Domain.Entities;

namespace TradingChat.Domain.Contracts;

public interface IChatRoomRepository : IBaseRepository<ChatRoom>
{
    Task<bool> NameAlreadyTaken(string name);
}
