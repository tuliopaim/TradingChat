using TradingChat.Application.Abstractions;

namespace TradingChat.Application.UseCases.GetStockPrice;

public record GetStockPriceCommand(string StockCode) : ICommand<StockDataDto>;
