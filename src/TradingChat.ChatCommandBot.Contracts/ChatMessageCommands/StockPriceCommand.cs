using TradingChat.ChatCommandBot.Commands.Contracts;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;
using TradingChat.Domain.Shared;

namespace TradingChat.ChatCommandBot.Commands.ChatMessageCommands;

public class StockPriceCommand : IChatMessageCommand
{
    private readonly IMessageProducer _messageProducer;
    private readonly IStockPriceService _stockPriceService;

    private const string _stockCommand = "/stock=";

    public StockPriceCommand(
        IMessageProducer messageProducer,
        IStockPriceService stockPriceService)
    {
        _messageProducer = messageProducer;
        _stockPriceService = stockPriceService;
    }

    public bool CanExecute(string message)
    {
        return message.StartsWith(_stockCommand);
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

        _messageProducer.Publish(new NewMessage
        {
            Message = responseMessage,
            ChatRoomId = chatId
        }, QueueNames.ChatMessage);

        return Result.Success();
    }
}
