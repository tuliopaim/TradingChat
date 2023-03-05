using TradingChat.ChatCommandBot.Commands.Contracts;
using TradingChat.Core.Messaging;
using TradingChat.Core.Messaging.Messages;
using TradingChat.Domain.Shared;

namespace TradingChat.ChatCommandBot.Commands.ChatMessageCommands.StockPrice;

public class StockPriceCommand : IChatMessageCommand
{
    private readonly RabbitMqProducer _rabbitMqProducer;
    private readonly IStockPriceService _stockPriceService;

    public StockPriceCommand(
        RabbitMqProducer rabbitMqProducer,
        IStockPriceService stockPriceService)
    {
        _rabbitMqProducer = rabbitMqProducer;
        _stockPriceService = stockPriceService;
    }

    public bool CanExecute(string message)
    {
        return message.StartsWith("/stock=");
    }

    public async Task<Result> ExecuteAsync(string message, Guid chatId)
    {
        string[] parts = message.Split('=');
        if (parts.Length != 2)
        {
            return new Error("Invalid stock command");
        }

        string stockCode = parts[1];

        var stockDataResult = await _stockPriceService.GetStockPrice(stockCode);
        string responseMessage;

        if (stockDataResult.IsSuccess)
        {
            var stockData = stockDataResult.Value;

            responseMessage = $"Stock data for {stockData.Symbol} on {stockData.Date} at {stockData.Time}:\n" +
                  $"Open price: {stockData.Open}\n" +
                  $"High price: {stockData.High}\n" +
                  $"Low price: {stockData.Low}\n" +
                  $"Close price: {stockData.Close}\n" +
                  $"Volume: {stockData.Volume}";
        }
        else
        {
            responseMessage = $"Unable to retrieve stock data for symbol {stockCode}. Please try again later.";
        }

        _rabbitMqProducer.Publish(new NewMessage
        {
            Message = responseMessage,
            ChatRoomId = chatId
        }, RabbitRoutingKeys.ChatMessage);

        return Result.Success();
    }
}
