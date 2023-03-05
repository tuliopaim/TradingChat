using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;

namespace TradingChat.Application.UseCases.GetChatMessages;

public record GetChatMessagesQuery(Guid ChatRoomId) : IQuery<ChatMessagesDto>;
    


