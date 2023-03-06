using Moq;
using TradingChat.ChatBot.Commands.ChatMessageCommands;
using TradingChat.ChatBot.Commands.Contracts;
using TradingChat.Core.Messaging;
using TradingChat.Core;
using FluentAssertions;
using TradingChat.Core.Messages;

namespace TradingChat.ChatBot.Commands.Tests;
public class StockPriceCommandTests
{
    private readonly Mock<IMessageProducer> _messageProducerMock = new Mock<IMessageProducer>();
    private readonly Mock<IStockPriceService> _stockPriceServiceMock = new Mock<IStockPriceService>();

    [Fact]
    public void CanExecute_ShouldReturnTrue_WhenMessageStartsWithStockCommand()
    {
        // Arrange
        var command = new StockPriceCommand(_messageProducerMock.Object, _stockPriceServiceMock.Object);
        string message = "/stock=APPL";

        // Act
        bool canExecute = command.CanExecute(message);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    public void CanExecute_ShouldReturnFalse_WhenMessageDoesNotStartWithStockCommand()
    {
        // Arrange
        var command = new StockPriceCommand(_messageProducerMock.Object, _stockPriceServiceMock.Object);
        string message = "Invalid command!";

        // Act
        bool canExecute = command.CanExecute(message);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnErrorResult_WhenMessageHasInvalidFormat()
    {
        // Arrange
        var command = new StockPriceCommand(_messageProducerMock.Object, _stockPriceServiceMock.Object);
        string message = "/stock=";

        // Act
        Result result = await command.ExecuteAsync(message, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Invalid stock command");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnSuccessResult_WhenStockServiceReturnsData()
    {
        // Arrange
        var command = new StockPriceCommand(_messageProducerMock.Object, _stockPriceServiceMock.Object);
        string message = "/stock=APPL.US";

        var stockData = new StockDataDto(
             "APPL",
             new DateOnly(2023, 03, 06),
             new TimeOnly(16,00,00),
             135.5m,
             140.2m,
             132.5m,
             137.8m,
             50000);

        _stockPriceServiceMock.Setup(x => x.GetStockPrice("APPL.US"))
            .ReturnsAsync(Result.Success(stockData));

        // Act
        Result result = await command.ExecuteAsync(message, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPublishNewMessage_WhenStockServiceReturnsData()
    {
        // Arrange
        var command = new StockPriceCommand(_messageProducerMock.Object, _stockPriceServiceMock.Object);
        string message = "/stock=APPL.US";

        var stockData = new StockDataDto(
             "APPL",
             new DateOnly(2023, 03, 06),
             new TimeOnly(16, 00, 00),
             135.5m,
             140.2m,
             132.5m,
             137.8m,
             50000);

        _stockPriceServiceMock.Setup(x => x.GetStockPrice("APPL.US"))
            .ReturnsAsync(Result.Success(stockData));

        // Act
        await command.ExecuteAsync(message, Guid.NewGuid());

        // Assert
        _messageProducerMock.Verify(x => x.Publish(It.IsAny<NewMessage>(), QueueNames.ChatMessage), Times.Once);
    }
}
