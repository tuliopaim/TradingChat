using FluentAssertions;
using Moq;
using TradingChat.Application.Abstractions;
using TradingChat.Application.UseCases.CreateChatRoom;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;

namespace TradingChat.Application.Tests;

public class CreateChatRoomTests
{
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Mock<IChatRoomRepository> _chatRoomRepositoryMock = new();
    private readonly CreateChatRoomHandler _handler;

    public CreateChatRoomTests()
    {
        _handler = new CreateChatRoomHandler(_currentUserMock.Object, _chatRoomRepositoryMock.Object);
    }

    [Fact]
    public async Task Should_CreatesNewChatRoom_WhenNameIsNotTaken()
    {
        // Arrange
        var command = new CreateChatRoomCommand("Test Room", 10 );

        var currentUserId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(currentUserId);

        _chatRoomRepositoryMock.Setup(x => x.NameAlreadyTaken(command.Name)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _chatRoomRepositoryMock.Verify(x => 
            x.Add(It.Is<ChatRoom>(r =>
                r.Name == command.Name &&
                r.MaxNumberOfUsers == command.MaxNumberOfUsers)), Times.Once);

        _chatRoomRepositoryMock
            .Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnError_WhenNameIsTaken()
    {
        // Arrange
        var command = new CreateChatRoomCommand("Test Room", 10 );

        _chatRoomRepositoryMock.Setup(x => x.NameAlreadyTaken(command.Name)).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Name already taken!");
    }
}