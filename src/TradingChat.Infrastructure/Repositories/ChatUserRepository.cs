using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Infrastructure.Context;

namespace TradingChat.Infrastructure.Repositories;

public class ChatUserRepository : BaseRepository<ChatUser>, IChatUserRepository
{
    private readonly TradingChatDbContext _context;

    public ChatUserRepository(TradingChatDbContext context) : base(context)
    {
        _context = context;
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        return _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        return _context.Database.RollbackTransactionAsync(cancellationToken);
    }

}
