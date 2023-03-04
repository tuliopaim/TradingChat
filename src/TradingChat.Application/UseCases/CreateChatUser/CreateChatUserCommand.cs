using TradingChat.Application.Abstractions;

namespace TradingChat.Application.UseCases.CreateChatUser;

public record CreateChatUserCommand(
    string Email,
    string Name,
    string Password) : ICommand;
