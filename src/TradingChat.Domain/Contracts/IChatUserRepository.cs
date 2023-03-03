using TradingChat.Domain.Entities;

namespace TradingChat.Domain.Contracts;

public interface IChatUserRepository : IBaseRepository<ChatUser>
{
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}
