using TradingChat.Domain.Shared;

namespace TradingChat.Domain.UseCases.CreateUser;

public interface ICreateChatUserHandler
{
    Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken);
}