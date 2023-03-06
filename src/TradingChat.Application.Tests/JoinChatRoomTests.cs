using FluentAssertions;
using Moq;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.JoinChatRoom;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Application.Tests;
public class JoinChatRoomTests
{
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Mock<IChatRoomRepository> _chatRoomRepositoryMock = new();
    private readonly JoinChatRoomCommandHandler _handler;

    public JoinChatRoomTests()
    {
        _handler = new JoinChatRoomCommandHandler(_currentUserMock.Object, _chatRoomRepositoryMock.Object);
    }

    [Fact]
    public async Task Should_AddUserToChatRoom_WhenChatRoomExistsAndUserIsNotInRoom()
    {
        // Arrange
        var chatRoomId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var command = new JoinChatRoomCommand(chatRoomId);
        var chatRoom = new ChatRoom("Test Room", 10, ownerId);

        _chatRoomRepositoryMock.Setup(x => x.GetWithUsersAsTracking(command.ChatRoomId)).ReturnsAsync(chatRoom);

        _currentUserMock.Setup(x => x.Id).Returns(userId);

        _chatRoomRepositoryMock.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        chatRoom.ContainsUser(_currentUserMock.Object.Id!.Value).Should().BeTrue();

        _chatRoomRepositoryMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnError_WhenChatRoomDoesNotExist()
    {
        // Arrange
        var command = new JoinChatRoomCommand(Guid.NewGuid());
        _chatRoomRepositoryMock.Setup(x => x.GetWithUsersAsTracking(command.ChatRoomId)).ReturnsAsync((ChatRoom)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "ChatRoom not found!");
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenUserAlreadyInChatRoom()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new JoinChatRoomCommand(userId);

        var ownerId = Guid.NewGuid();
        var chatRoom = new ChatRoom("Test Room", 10, ownerId);

        _currentUserMock.Setup(x => x.Id).Returns(userId);
        
        chatRoom.AddUser(_currentUserMock.Object.Id!.Value);

        _chatRoomRepositoryMock.Setup(x => x.GetWithUsersAsTracking(command.ChatRoomId)).ReturnsAsync(chatRoom);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _chatRoomRepositoryMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        result.IsSuccess.Should().BeTrue();
    }
}
