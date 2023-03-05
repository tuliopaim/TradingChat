using Microsoft.EntityFrameworkCore;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core;
using TradingChat.Domain.Contracts;

namespace TradingChat.Application.UseCases.GetChatMessages;

public class GetChatMessagesQueryHandler : IQueryHandler<GetChatMessagesQuery, ChatMessagesDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public GetChatMessagesQueryHandler(IChatRoomRepository chatRoomRepository)
    {
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result<ChatMessagesDto>> Handle(
        GetChatMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var chatMessagesDto = await _chatRoomRepository
            .GetAsNoTracking()
            .Where(x => x.Id == request.ChatRoomId)
            .Select(chatRoom => new ChatMessagesDto 
            {
                ChatRoomId = chatRoom.Id,
                Name = chatRoom.Name,
                Messages = chatRoom
                    .Messages
                    .OrderByDescending(x => x.SentAt)
                    .Take(50)
                    .Select(message => new ChatMessageInfoDto
                    {
                        Id = message.Id,
                        Message = message.Message,
                        SentAt = message.SentAt,
                        User = message.ChatUser!.Name,
                    })
            })
            .FirstOrDefaultAsync(cancellationToken);

        chatMessagesDto!.Messages = 
            chatMessagesDto!.Messages.OrderBy(x => x.SentAt);
        
        return chatMessagesDto;
    }
}
