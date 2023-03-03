namespace TradingChat.Domain.UseCases.CreateUser;

public record CreateUserCommand(
    string Email,
    string Name,
    string Password);
