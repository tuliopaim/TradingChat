using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.WebApp.Application;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, ChatMessageInfoDto>
{
    private readonly ILogger<SendMessageCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageProducer _messageProducer;

    public SendMessageCommandHandler(
        ILogger<SendMessageCommandHandler> logger,
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository,
        IMessageProducer messageProducer)
    {
        _logger = logger;
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
        _messageProducer = messageProducer;
    }

    public async Task<Result<ChatMessageInfoDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id!.Value;

        using var activity = Telemetry.MyActivitySource.StartActivity(nameof(SendMessageCommandHandler));

        activity?.AddTag("UserId", userId.ToString());

        _logger.LogInformation("User {UserId} is sending message to chat room {ChatRoomId}", userId, request.ChatRoomId);

        ChatRoom? chatRoom = await _chatRoomRepository
            .GetChatRoomToSendMessage(request.ChatRoomId, userId, cancellationToken);

        if (chatRoom is null)
        {
            _logger.LogWarning("Chat Room not found!");
            return new Error("Chat Room not found!");
        }

        if (!chatRoom.ContainsUser(userId))
        {
            _logger.LogWarning("User {UserId} not allowed to send messages!", userId);
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


