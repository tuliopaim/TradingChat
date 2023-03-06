using Microsoft.EntityFrameworkCore;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, ChatMessageInfoDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageProducer _messageProducer;

    public SendMessageCommandHandler(
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository,
        IMessageProducer messageProducer)
    {
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
        _messageProducer = messageProducer;
    }

    public async Task<Result<ChatMessageInfoDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id!.Value;

        ChatRoom? chatRoom = await _chatRoomRepository
            .GetChatRoomToSendMessage(request.ChatRoomId, userId, cancellationToken);

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

    private void PublishChatCommandMessage(ChatMessage message)
    {
        var chatCommandMessage = new ChatCommandMessage 
        {
            Message = message.Message,
            ChatRoomId = message.ChatRoomId
        };

        _messageProducer.Publish(chatCommandMessage, QueueNames.ChatCommand);
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
            User = chatRoom.Users.First(u => u.ChatUserId == userId)?.ChatUser?.Name,
        };
    }
}


