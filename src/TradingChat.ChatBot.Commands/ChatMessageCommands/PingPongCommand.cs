using TradingChat.ChatBot.Commands.Contracts;
using TradingChat.Core;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;

namespace TradingChat.ChatBot.Commands.ChatMessageCommands;

public class PingPongCommand : IChatMessageCommand
{
    private readonly IMessageProducer _messageProducer;

    public PingPongCommand(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }

    private const string _pingCommand = "/ping";
    private const string _responseMessage = "pong";
    private const string _responseMessageCapital = "Pong";

    public bool CanExecute(string message)
    {
        return message
            .Equals(_pingCommand, StringComparison.InvariantCultureIgnoreCase);
    }

    public Task<Result> ExecuteAsync(string message, Guid chatId)
    {
        _messageProducer.Publish(new NewMessage
        {
            Message = char.IsUpper(message[1])
                ? _responseMessageCapital
                : _responseMessage,
            ChatRoomId = chatId
        }, QueueNames.ChatMessage);

        return Task.FromResult(Result.Success());
    }
}
