using FluentAssertions;
using Moq;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.SendMessage;
using TradingChat.Application.UseCases.Shared;
using TradingChat.Core.Messaging;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Application.Tests;

public class SendMessageTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IChatRoomRepository> _chatRoomRepositoryMock;
    private readonly Mock<IMessageProducer> _messageProducerMock;
    private readonly SendMessageCommandHandler _handler;

    public SendMessageTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _chatRoomRepositoryMock = new Mock<IChatRoomRepository>();
        _messageProducerMock = new Mock<IMessageProducer>();
        _handler = new SendMessageCommandHandler(
            _currentUserMock.Object,
            _chatRoomRepositoryMock.Object,
            _messageProducerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnError_WhenChatRoomNotFound()
    {
        // Arrange
        var request = new SendMessageCommand
        {
            ChatRoomId = Guid.NewGuid(),
            Message = "Hello"
        };

        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(userId);

        _chatRoomRepositoryMock.Setup(x => 
            x.GetChatRoomToSendMessage(request.ChatRoomId, userId, CancellationToken.None))
            .ReturnsAsync((ChatRoom?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Chat Room not found!");
    }

    [Fact]
    public async Task Should_ReturnError_WhenUserNotAllowedToSendMessage()
    {
        // Arrange
        var chatRoomId = Guid.NewGuid();
        var request = new SendMessageCommand
        {
            ChatRoomId = chatRoomId,
            Message = "Hello"
        };

        var userId = Guid.NewGuid();
        var chatRoom = new ChatRoom("Test Room", 10, Guid.NewGuid()) { Id = chatRoomId };

        _currentUserMock.Setup(x => x.Id).Returns(userId);

        _chatRoomRepositoryMock.Setup(x =>
           x.GetChatRoomToSendMessage(request.ChatRoomId, userId, CancellationToken.None))
           .ReturnsAsync(chatRoom);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "User not allowed to send messages!");
    }

    [Fact]
    public async Task Should_ReturnChatMessageInfoDto_WhenValidRequest()
    {
        // Arrange
        var chatRoomId = Guid.NewGuid();
        var request = new SendMessageCommand
        {
            ChatRoomId = chatRoomId,
            Message = "Hello"
        };

        var userId = Guid.NewGuid();
        var chatRoom = new ChatRoom("Test Room", 10, Guid.NewGuid()) { Id = chatRoomId };

        var user = new ChatUser("John Doe", userId);

        chatRoom.AddUser(userId);

        _currentUserMock.Setup(x => x.Id).Returns(userId);

        _chatRoomRepositoryMock.Setup(x =>
           x.GetChatRoomToSendMessage(request.ChatRoomId, userId, CancellationToken.None))
           .ReturnsAsync(chatRoom);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var data = result.Value;

        result.IsSuccess.Should().BeTrue();

        data.Id.Should().NotBeEmpty();
        data.Message.Should().Be(request.Message);
    }
}
