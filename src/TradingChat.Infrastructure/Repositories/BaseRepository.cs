using Microsoft.EntityFrameworkCore;
using TradingChat.Domain.Contracts;
using TradingChat.Infrastructure.Context;

namespace TradingChat.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly TradingChatDbContext _context;

    protected BaseRepository(TradingChatDbContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> Get() => _context.Set<TEntity>().AsTracking();

    public IQueryable<TEntity> GetAsNoTracking() => Get().AsNoTrackingWithIdentityResolution();

    public void Add(TEntity entity) => _context.Add(entity);

    public void Remove(TEntity entity) => _context.Remove(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}
