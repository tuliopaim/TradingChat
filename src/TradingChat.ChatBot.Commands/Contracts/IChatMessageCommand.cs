using TradingChat.Core;

namespace TradingChat.ChatBot.Commands.Contracts;

public interface IChatMessageCommand
{
    bool CanExecute(string message);
    Task<Result> ExecuteAsync(string message, Guid chatId);
}