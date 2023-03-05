﻿using System.Text.Json.Serialization;

namespace TradingChat.Core.Messaging.Messages;

public class ChatCommandMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("chatRoomId")]
    public Guid ChatRoomId { get; set; }
}

