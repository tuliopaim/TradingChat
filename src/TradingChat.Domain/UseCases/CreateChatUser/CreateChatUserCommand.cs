using TradingChat.Domain.UseCases.Base;

namespace TradingChat.Domain.UseCases.CreateChatUser;

public record CreateChatUserCommand(
    string Email,
    string Name,
    string Password) : ICommand;
