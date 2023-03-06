using TradingChat.ChatBot.Commands.Contracts;
using TradingChat.Core;

namespace TradingChat.ChatBot.Commands;

public class ChatMessageCommandInvoker
{
    private readonly List<IChatMessageCommand> _commands;

    public ChatMessageCommandInvoker(IEnumerable<IChatMessageCommand> commands)
    {
        _commands = commands.ToList();
    }

    public async Task<Result> ExecuteCommandAsync(string message, Guid chatId)
    {
        var command = _commands.FirstOrDefault(c => c.CanExecute(message));
        if (command is null)
        {
            return Result.Success();
        }

        return await command.ExecuteAsync(message, chatId);
    }
}
