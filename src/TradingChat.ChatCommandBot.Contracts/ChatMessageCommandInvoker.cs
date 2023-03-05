using TradingChat.ChatCommandBot.Commands.Contracts;
using TradingChat.Domain.Shared;

namespace TradingChat.ChatCommandBot.Commands;

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
