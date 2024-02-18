using System.Text.Json.Serialization;

namespace TradingChat.Core.Messages;

public class ChatCommandMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("chatRoomId")]
    public Guid ChatRoomId { get; set; }
}

