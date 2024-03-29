﻿using Microsoft.EntityFrameworkCore;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Application.UseCases.SendMessageFromServer;

public class SendMessageFromServerCommandHandler : ICommandHandler<SendMessageFromServerCommand, ChatMessageInfoDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public SendMessageFromServerCommandHandler(
        IChatRoomRepository chatRoomRepository)
    {
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result<ChatMessageInfoDto>> Handle(
        SendMessageFromServerCommand request,
        CancellationToken cancellationToken)
    {
        ChatRoom? chatRoom = await GetChatRoom(request, cancellationToken);

        if (chatRoom is null)
        {
            return new Error("Chat Room not found!");
        }

        var botUser = ChatUser.ChatBotUser();
        var userId = botUser.Id;

        var message = new ChatMessage(
            request.Message,
            userId,
            request.ChatRoomId);

        chatRoom.AddMessage(message);

        await _chatRoomRepository.SaveChangesAsync(cancellationToken);

        return CreateChatMessageDto(botUser.Name, message);
    }

    private async Task<ChatRoom?> GetChatRoom(SendMessageFromServerCommand request, CancellationToken cancellationToken)
    {
        return await _chatRoomRepository.Get()
            .FirstOrDefaultAsync(x => x.Id == request.ChatRoomId, cancellationToken);
    }

    private static Result<ChatMessageInfoDto> CreateChatMessageDto(
        string userName,
        ChatMessage message)
    {
        return new ChatMessageInfoDto
        {
            Id = message.Id,
            Message = message.Message,
            SentAt = message.SentAt,
            User = userName,
        };
    }
}
