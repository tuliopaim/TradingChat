using Microsoft.EntityFrameworkCore;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core.Messaging;
using TradingChat.Core.Messaging.Messages;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, ChatMessageInfoDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly RabbitMqProducer _rabbitProducer;

    public SendMessageCommandHandler(
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository,
        RabbitMqProducer rabbitProducer)
    {
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
        _rabbitProducer = rabbitProducer;
    }

    public async Task<Result<ChatMessageInfoDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id!.Value;

        ChatRoom? chatRoom = await GetChatRoom(request, userId, cancellationToken);

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

        if (message.IsCommand)
        {
            PublishChatCommandMessage(message);
        }

        return CreateChatMessageDto(userId, chatRoom, message);
    }

    private async Task<ChatRoom?> GetChatRoom(SendMessageCommand request, Guid userId, CancellationToken cancellationToken)
    {
        return await _chatRoomRepository.Get()
            .Where(x => x.Id == request.ChatRoomId)
            .Include(x => x.Users.Where(u => u.ChatUserId == userId))
                .ThenInclude(u => u.ChatUser)
             .FirstOrDefaultAsync(cancellationToken);
    }

    private void PublishChatCommandMessage(ChatMessage message)
    {
        var chatCommandMessage = new ChatCommandMessage 
        {
            Message = message.Message,
            ChatRoomId = message.ChatRoomId
        };

        _rabbitProducer.Publish(chatCommandMessage, RabbitRoutingKeys.ChatCommand);
    }

    private static Result<ChatMessageInfoDto> CreateChatMessageDto(
        Guid userId,
        ChatRoom chatRoom,
        ChatMessage message)
    {
        return new ChatMessageInfoDto
        {
            Id = message.Id,
            Message = message.Message,
            SentAt = message.SentAt,
            User = chatRoom.Users.First(u => u.ChatUserId == userId)!.ChatUser!.Name,
        };
    }
}


