using TradingChat.Core;

namespace TradingChat.ChatCommandBot.Commands.Contracts;

public interface IChatMessageCommand
{
    bool CanExecute(string message);
    Task<Result> ExecuteAsync(string message, Guid chatId);
}