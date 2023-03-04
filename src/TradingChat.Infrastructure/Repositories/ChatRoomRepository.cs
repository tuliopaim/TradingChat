using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Infrastructure.Context;

namespace TradingChat.Infrastructure.Repositories;

public class ChatRoomRepository : BaseRepository<ChatRoom>, IChatRoomRepository
{
    private readonly TradingChatDbContext _context;

    public ChatRoomRepository(TradingChatDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<ChatRoom?> GetWithUsers(Guid chatRoomId)
    {
        return Get()
            .Include(x => x.Users)
            .FirstOrDefaultAsync(c => c.Id == chatRoomId);
    }

    public Task<bool> NameAlreadyTaken(string name)
    {
        return GetAsNoTracking()
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }
}
