using FluentAssertions;
using Moq;
using TradingChat.ChatBot.Commands.Contracts;
using TradingChat.Core;

namespace TradingChat.ChatBot.Commands.Tests;

public class ChatMessageCommandInvokerTests
{
    private readonly Mock<IChatMessageCommand> _mockCommand;
    private readonly ChatMessageCommandInvoker _invoker;

    public ChatMessageCommandInvokerTests()
    {
        _mockCommand = new Mock<IChatMessageCommand>();
        _invoker = new ChatMessageCommandInvoker(new List<IChatMessageCommand> { _mockCommand.Object });
    }

    [Fact]
    public async Task ExecuteCommandAsync_Should_ReturnResultFromCommand_WhenCommandCanExecute()
    {
        // Arrange
        var message = "/test command";
        var chatId = Guid.NewGuid();
        var expectedResult = Result.Success();

        _mockCommand.Setup(c => c.CanExecute(message)).Returns(true);
        _mockCommand.Setup(c => c.ExecuteAsync(message, chatId)).ReturnsAsync(expectedResult);

        // Act
        var result = await _invoker.ExecuteCommandAsync(message, chatId);

        // Assert
        result.Should().Be(expectedResult);
        _mockCommand.Verify(c => c.CanExecute(message), Times.Once);
        _mockCommand.Verify(c => c.ExecuteAsync(message, chatId), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnSuccessResult_WhenNoCommandCanExecute()
    {
        // Arrange
        var message = "test message";
        var chatId = Guid.NewGuid();

        _mockCommand.Setup(c => c.CanExecute(message)).Returns(false);

        // Act
        var result = await _invoker.ExecuteCommandAsync(message, chatId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCommand.Verify(c => c.CanExecute(message), Times.Once);
        _mockCommand.Verify(c => c.ExecuteAsync(message, chatId), Times.Never);
    }
}