namespace TradingChat.Core.Messaging;

public class RabbitRoutingKeys
{
    public const string ChatCommand = "ChatCommand";
    public const string ChatMessage = "ChatMessage";

    public static string[] All => new[]
    {
        ChatCommand,
        ChatMessage,
    };
}
