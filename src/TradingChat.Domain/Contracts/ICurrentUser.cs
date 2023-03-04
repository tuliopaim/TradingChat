namespace TradingChat.Domain.Contracts;

public interface ICurrentUser
{
    Guid? Id { get; }
}