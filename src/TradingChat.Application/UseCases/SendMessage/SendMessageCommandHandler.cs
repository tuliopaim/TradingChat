using Microsoft.EntityFrameworkCore;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, ChatMessageInfoDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;

    public SendMessageCommandHandler(
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository)
    {
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result<ChatMessageInfoDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id!.Value;

        var chatRoom = await _chatRoomRepository.Get()
            .Where(x => x.Id == request.ChatRoomId)
            .Include(x => x.Users.Where(u => u.ChatUserId == userId))
                .ThenInclude(u => u.ChatUser)
             .FirstOrDefaultAsync(cancellationToken);

        if (chatRoom is null)
        {
            return new Error("Chat Room not found!");
        }

        if (!chatRoom.ContainsUser(userId))
        {
            return new Error("User not allowed to send messages!");
        }
        
        var message = new ChatMessage(
            request.Message,
            _currentUser.Id!.Value,
            request.ChatRoomId);

        chatRoom.AddMessage(message);

        await _chatRoomRepository.SaveChangesAsync(cancellationToken);

        var chatMessageDto = new ChatMessageInfoDto 
        {
            Id = message.Id,
            Message = message.Message,
            SentAt = message.SentAt,
            User = chatRoom.Users.First(u => u.ChatUserId == userId)!.ChatUser!.Name,
        };

        return chatMessageDto;
    }
}
