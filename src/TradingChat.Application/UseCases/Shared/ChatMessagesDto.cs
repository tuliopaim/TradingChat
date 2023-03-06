namespace TradingChat.Application.UseCases.Shared;

public class ChatMessagesDto
{
    public required Guid ChatRoomId { get; init; }
    public required string Name { get; init; }
    public IEnumerable<ChatMessageInfoDto> Messages { get; set; } = new List<ChatMessageInfoDto>();
}

public class ChatMessageInfoDto
{
    public required Guid Id { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset SentAt { get; init; }
    public required string? User { get; init; }

    public string SentAtString => SentAt
        .LocalDateTime
        .ToString("dd/MM/yyyy HH:mm");
}
