namespace TradingChat.Domain.Contracts;

public interface IBaseRepository<TEntity> where TEntity : class
{
    void Add(TEntity entity);
    IQueryable<TEntity> Get();
    IQueryable<TEntity> GetAsNoTracking();
    void Remove(TEntity entity);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
