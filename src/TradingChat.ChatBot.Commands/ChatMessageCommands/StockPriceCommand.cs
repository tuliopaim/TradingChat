using System.Globalization;
using TradingChat.ChatBot.Commands.Contracts;
using TradingChat.Core;
using TradingChat.Core.Messages;
using TradingChat.Core.Messaging;

namespace TradingChat.ChatBot.Commands.ChatMessageCommands;

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
        string[] parts = message.Split('=', StringSplitOptions.RemoveEmptyEntries);
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

            var formattedCloseValue = stockDataResult.Value.Close.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));

            responseMessage = $"{stockData.Symbol} quote is {formattedCloseValue} per share.";
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
