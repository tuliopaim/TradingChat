namespace TradingChat.Application.Abstractions;

public interface ICurrentUser
{
    Guid? Id { get; }
}